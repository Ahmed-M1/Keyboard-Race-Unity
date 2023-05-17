using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;

public class SpawnPosition : NetworkBehaviour
{
    private GameObject player1Position;
    private GameObject player2Position;
    private RectTransform[] positionArr;
    private NetworkVariable<Vector2> position = new NetworkVariable<Vector2>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    int i = 0;

    public void Awake()
    {
        player1Position = GameObject.Find("Player1Position");
        player2Position = GameObject.Find("Player2Position");
        positionArr = new RectTransform[2];
        positionArr[0] = player1Position.GetComponent<RectTransform>(); ;
        positionArr[1] = player2Position.GetComponent<RectTransform>(); ;
        Debug.Log("Made1");

    }

    public override void OnNetworkSpawn()
    {

        position.Value = positionArr[OwnerClientId].anchoredPosition;
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position.Value;
        Debug.Log("Made2");






        /*if (IsServer)
        {
            // Spawn logic for server
            if (player1Position!=null)
            {
                RectTransform rectTransform = 
                Debug.Log("Server spawned");

            }
        }
        else if (IsClient)
        {
            // Spawn logic for clients
            if (player2Position!=null)
            {
                RectTransform rectTransform = player2Position.GetComponent<RectTransform>();
               
                Debug.Log("Client spawned");
                
            }
        }*/
    }

    void Update()
    {


    }
    /*void Start()
    {
        i = 0;
        
        player1Position = GameObject.Find("Player1Position");
        player2Position = GameObject.Find("Player2Position");
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (IsServer)
        {
            Debug.Log("Hello.I am server");
            rectTransform.anchoredPosition = player1Position.GetComponent<RectTransform>().anchoredPosition;
            Debug.Log("P1 Spawned" + i);

        }

        else
        {
            rectTransform.anchoredPosition = player2Position.GetComponent<RectTransform>().anchoredPosition;
            Debug.Log("P2 Spawned" + i);
            //Debug.Log("I am not");
        }
        i++;
    }*/

    [ServerRpc (RequireOwnership=false)]
    public void SetPositionServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (clientId==0)
        {
            rectTransform.anchoredPosition = player1Position.GetComponent<RectTransform>().anchoredPosition;
            Debug.Log("P1 Spawned");
        }

        else
        {
            rectTransform.anchoredPosition = player2Position.GetComponent<RectTransform>().anchoredPosition;
            Debug.Log("P2 Spawned");
        }

        Debug.Log("Something Happened");
        Debug.Log("Client id " + clientId);

    }

 /*   private void Update()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position.Value;
        }
    }*/
}
