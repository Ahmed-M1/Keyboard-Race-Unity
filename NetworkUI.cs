using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using TMPro;
using System.Net;
using AddressFamily = System.Net.Sockets.AddressFamily;
using System.Net.Sockets;
using System.Text.RegularExpressions;

// Script to handle the network UI
public class NetworkUI : NetworkBehaviour
{
    [SerializeField] private Button host;
    [SerializeField] private Button client;

    // Display number of players
    [SerializeField] private TextMeshProUGUI NumberPlayers;
    [SerializeField] private TextMeshProUGUI DisplayIp; //IP displayed if host
    [SerializeField] private TextMeshProUGUI WaitingForPlayer; //Waiting for player

    [SerializeField] private RectTransform UITransform; //Waiting for player

    //Text Mesh Pro Input field
    [SerializeField] private TMP_InputField HostToConnectTo;

    //BG music to start
    [SerializeField] private AudioSource music;


    public bool proceedToGame;

    private GameObject timerScript;
    private RunTimer script;
    private NetworkVariable<int> numPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> start_Game = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private string myAddressLocal;
    private Vector3 value;

    private void Awake()
    {
        proceedToGame = false;
        // Stop timer until connected
        //Scale Down NetworkUI
        value = new Vector3(1.674381f, 1.044182f, 0.2023311f);
        // Get the Audio Source component attached to the same GameObject




        host.onClick.AddListener(() =>
        {

            // Retrieve the local IP address of the Android device
            myAddressLocal= GetLocalIPAddress();
            
            //Make sure port free
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(myAddressLocal, (ushort)5060);


            NetworkManager.Singleton.StartHost(); // Start server

            if (NetworkManager.Singleton.IsServer)
            {

                DisplayIp.gameObject.SetActive(true);
                Debug.Log("Host IP: " + myAddressLocal);
                DisplayIp.text ="Your Host IP: " +  myAddressLocal;
                client.gameObject.SetActive(false);
                HostToConnectTo.gameObject.SetActive(false);
                host.gameObject.SetActive(false);
                WaitingForPlayer.gameObject.SetActive(true);


            }


        });

        client.onClick.AddListener(() =>
        {



            //Check if HostToConnectTo.text contains an IP
            string inputText = HostToConnectTo.text;
            bool isValidIP = IsIPAddress(inputText);

            if (isValidIP)
            {

                //Make sure port free
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(inputText, (ushort)5060);

                NetworkManager.Singleton.StartClient(); // Start client
                //NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
                if (NetworkManager.Singleton.IsClient)
                {
                    Debug.Log("Connected Successfully to: " +  inputText);
                    client.gameObject.SetActive(false);
                    HostToConnectTo.gameObject.SetActive(false);
                    host.gameObject.SetActive(false);
                    proceedToGame = true;
                   

                    // script.lockUnlockTimer();
                }
                else { Debug.Log("No host with this name found!\n"); }
            }


        });
    }




    private bool IsIPAddress(string input)
    {
        // Use regular expression pattern to match IP address
        string pattern = @"^(\d{1,3}\.){3}\d{1,3}$";
        return Regex.IsMatch(input, pattern);
    }

    private string GetLocalIPAddress()
    {
        // Check if the application is running on an Android device
       /* if (Application.platform == RuntimePlatform.Android)
        {*/
            // Get the device's IP addresses
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = null;
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip;
                    break;
                }
            }

            if (ipAddress != null)
            {
                return ipAddress.ToString();
            }
            else
            {
                Debug.LogWarning("Failed to retrieve local IP address.");
                return null;
            }
    }

    public bool get_start_Game()
    {
        return start_Game.Value;
    }
    void Update()
    {
        NumberPlayers.text = "Players Connected: " + numPlayers.Value.ToString();
        //Scale Down NetworkUI
        UITransform.localScale = value;
        if (!IsServer) { return; }

        //If player 2 joins then start game
        if(numPlayers.Value>1)
        {
            start_Game.Value = true;
            WaitingForPlayer.gameObject.SetActive(false);
            StartBackgroundMusic();

        }
            
        numPlayers.Value = NetworkManager.Singleton.ConnectedClients.Count;
       
    }

    // Call this method to start playing the background music
    public void StartBackgroundMusic()
    {
        // Play the background music
        music.Play();
    }

    public void killServer()
    {


        NetworkManager.Shutdown();
        Destroy(NetworkManager.gameObject);

    }
}
