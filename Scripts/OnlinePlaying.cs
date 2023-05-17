using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Unity.Netcode;


using TMPro;

/*
 * Fix progress calculating algo
 * Learn to kill Network Manager and restart
 */

public class OnlinePlaying :NetworkBehaviour
{

    



    //To Store keyboard input for measuring accuracy
    private readonly string[] ManyPassages = new string[10];
    private TouchScreenKeyboard keyboard;

    //To Synchornize the passages displayed to all players
    private NetworkVariable<ushort> passageIndex = new NetworkVariable<ushort>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    

    //For something 
    [SerializeField] private TextMeshProUGUI TextObject;

    //Result Screeen Vars
    [SerializeField] private  GameObject ResultScreen;

    [SerializeField] public TextMeshProUGUI progress;
    [SerializeField] public TextMeshProUGUI winnerName;
    [SerializeField] public TextMeshProUGUI time;


    //To reference the NetworkUI
    [SerializeField] private GameObject NetworkUI;
    public NetworkUI uiScript;

    //To reference time 
    GameObject other;


    [SerializeField] private GameObject player;
    
    private PlayerData script;
   


    public float progress_float = 0;
    

    //private float AccuracyPercent;//Mistakes
    private float Mistakes_len; //Mistakes

    bool stuff = false;
    Color32 myColor32;
    Color32 redColor;
    Color32 greenColor;
   

    RectTransform m_RectTransform;
    private string Passage;
    private int passage_length;
    private int current_Index = 0;
    private int j = 0;
    private int previous_j = 0;
    private int linenum = 0;
    private int prev_linenum = 0;


    private Vector3 bottomLeft;
    private Vector3 worldBottomLeft;
    private Vector3 worldBottomLeftContainer;
    RunTimerOnline timeObj;




    //Things to do:
    /*When you finish a passage you shud get result popup and shud be asked whether u want to move on to next or no.*/


    private void Awake()
    {
        Application.targetFrameRate = 60;

        //Initialize Array of Strings 
        ManyPassages[0] = "The bikers rode down the long and narrow path to reach the city park. " +
                            "When they reached a good spot to rest, they began to look for signs of spring. The sun was bright, and a lot of bright";

        ManyPassages[1] = "Words per minute (WPM) is a measure of typing speed, commonly used in recruitment. For the purposes of WPM measurement a word is standardized to five characters or keystrokes. Therefore, brown counts as one word, " +
                            "but accounted counts as two. The benefits of a standardized measurement of input speed are that it enables comparison " +
                            "across language and hardware boundaries. " +
                            "The speed of an Afrikaans-speaking operator in Cape Town can be compared with a French-speaking operator in Paris.";

        ManyPassages[2] = "Business meetings, and professional recordings can contain sensitive data, so security is something a transcription company should not overlook when providing services. Companies should " +
            "therefore follow the various laws and industry best practice, especially so when serving law firms, " +
            "government agencies or courts. Medical Transcription specifically is governed by HIPAA";

        ManyPassages[3] = "Self-confidence is a tricky subject for many people. For some, it's impossible to feel good about themselves without outside validation. " +
            "When you're in a situation where the people in your life aren't helping you to feel better about yourself, this can become a problem in your day to day life. " +
            "Most insecurity stems from feelings of not being attractive or feelings of loneliness.";

        ManyPassages[4] = "Word processors evolved dramatically once they became software programs rather than dedicated machines. They can usefully be distinguished from text editors, the category of software they evolved from." +
            " Word processing added to the text editor the ability to control type style and size, " +
            "to manage lines (word wrap), to format documents into pages, and to number pages.";



    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        
        uiScript = NetworkUI.GetComponent<NetworkUI>();
        NetworkUI.SetActive(true);
       

        System.Random rd = new System.Random();

        

        if(IsServer)
        {
            ushort rand_num = (ushort)(rd.Next(0, 4));
            passageIndex.Value = rand_num;

        }

        TextObject = GetComponent<TextMeshProUGUI>();

        TextObject.text = ManyPassages[passageIndex.Value];

        TextObject.ForceMeshUpdate();

        redColor = new Color32(255, 0, 0, 255);
        greenColor = new Color32(14, 224, 31, 255);



        //Accuracy.text = "100%";
        //Mistakes divided by correct ones

        Mistakes_len = 0f;


        //Will get position of the box holding the text
        GameObject Container = GameObject.Find("ScrollArea");
        m_RectTransform = Container.GetComponent<RectTransform>();
        Vector3[] v = new Vector3[4];
        m_RectTransform.GetWorldCorners(v);

        //Store Box bottom coordinate in Object
        worldBottomLeftContainer = v[0];

        
        Passage = TextObject.text;
        passage_length = Passage.Length;


        //Wait till All players have joined
        yield return new WaitUntil(() => uiScript.get_start_Game());
        NetworkUI.SetActive(false);
        keyboard=TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        other = GameObject.Find("Timer");
        timeObj = other.GetComponent<RunTimerOnline>();

        progress_float = 0;

        //color_Accuracy(100f);
        //TouchScreenKeyboard.hideInput = true;


    }



    void postProcess()
    {

        //Finished//if 1 client reaches 100% progress or time if finished then, set stopForall as true so that all other clients cant anymore
        if (progress_float == 100f)
        {
            //Stop timer for everyone
            //StopForAllServerRpc();
            //Debug.Log("Something happened");
            keyboard.active = false;


        }
       


        else if (timeObj.getTimerState())
        {
            keyboard.active = false;        
           // StopForAllServerRpc();
           
        }

        //If not last character yet then keep continuing
        else
        {
            progress_float = ((((float)current_Index) / (float)passage_length) * 100); ;
            //Update progress

            current_Index++;
        }



        //Store coordinates of correct character
        bottomLeft = TextObject.textInfo.characterInfo[current_Index].bottomLeft;
        worldBottomLeft = TextObject.transform.TransformPoint(bottomLeft);

        prev_linenum = linenum;

        //Use number of lines to tell how frequently you need to scroll down
        linenum = TextObject.textInfo.characterInfo[current_Index].lineNumber;

        //If at the edge of box and also new line then scroll down abit.
        if (worldBottomLeft.y - worldBottomLeftContainer.y <= 6 && prev_linenum != linenum)
        {
            GameObject scroll = GameObject.Find("Scrollbar");

            scroll.GetComponent<Scrollbar>().value = scroll.GetComponent<Scrollbar>().value - 0.09F;

        }


    }


    void keyboard_Update()
    {
        if (keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            keyboard.active = false;
        }

        try
        { 
        
            //Keep checking if inputs correct or not until end of passage
            if (current_Index < passage_length)
            {

                //j and prev_j keep in check whether anything has been typed recently or no
                j = keyboard.text.Length;

                //These conditions are in place because OnGui() is called multiple times per frame and we only want to check for input once per frame

                if ((keyboard.text[j - 1] == Passage[current_Index]) && j - previous_j != 0) //if correct alphabet
                {
                    Debug.Log("This nigga inside");

                    //Change Color of string and now prepare to compare next letter
                    if (Passage[current_Index] != ' ')
                        color_text(current_Index, true);

                    postProcess();


                }



                previous_j = j;

            }
        }

        catch
        {

           
                // Get a reference to the spawned player object using its network ID
                NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject(); ;

                if (playerObject == null)
                {

                    Debug.Log("Smthing aint right");

                }

                // Get a reference to the NetworkBehaviour component on the player object
                PlayerData playerBehaviour = playerObject.GetComponent<PlayerData>();


                Debug.Log("Stop for aLL IS: " + playerBehaviour.get_stopForAll());

                if (!playerBehaviour.get_stopForAll())
                {
                    //progress_float = UnityEngine.Random.Range(90, 96);

                }
                else
                {
                    Debug.Log("Value reached");
                }

            }
          


        


    }
    void OnGUI()
    {


        if(uiScript.get_start_Game())
            keyboard_Update();

        //progress_float = UnityEngine.Random.Range(0.02f, 1.1f);

    }



    //Changes color of character depending upon whether or not the typed character was correct
    private void color_text(int index, bool correct)
    {

        int meshIndex = TextObject.textInfo.characterInfo[index].materialReferenceIndex;
        int vertexIndex = TextObject.textInfo.characterInfo[index].vertexIndex;
        Color32[] vertexColors = TextObject.textInfo.meshInfo[meshIndex].colors32;

        if (correct)
        {

            vertexColors[vertexIndex + 0] = greenColor;
            vertexColors[vertexIndex + 1] = greenColor;
            vertexColors[vertexIndex + 2] = greenColor;
            vertexColors[vertexIndex + 3] = greenColor;
        }
        //else turn red
        else
        {
            vertexColors[vertexIndex + 0] = redColor;
            vertexColors[vertexIndex + 1] = redColor;
            vertexColors[vertexIndex + 2] = redColor;
            vertexColors[vertexIndex + 3] = redColor; ;

        }



        TextObject.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    public GameObject get_ResultScreen()
    {
        return ResultScreen;
    }

}
