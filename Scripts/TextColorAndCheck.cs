using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

using TMPro;

/*
 
 * show final results: 1 . Accuracy 2. Time taken 3. WPM
 * Stop keyboard access after time is up
 * pop up result after each passage
 * pop up to ask whether u want to move on to next passage or not
 * LAN capability with 4 players
 
 */

public class TextColorAndCheck : MonoBehaviour
{

    //To Store keyboard input for measuring accuracy
    private readonly string[] ManyPassages = new string[10];
    private TouchScreenKeyboard keyboard;

    [SerializeField] private TextMeshProUGUI FinalResults;
    [SerializeField] private TextMeshProUGUI TextObject;
    [SerializeField] private TextMeshProUGUI Accuracy;
    [SerializeField] private TextMeshProUGUI AccuracyLabel;

    [SerializeField] private GameObject ResultScreen;

    private float AccuracyPercent;//Mistakes
    private float Mistakes_len; //Mistakes
    private float wpm;

    Color32 myColor32;
    Color32 redColor;
    Color32 greenColor;
    string result_String;

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
    RunTimer timeObj;

    //BG music to start
    [SerializeField] private AudioSource music;



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
    void Start()
    {
        // Set the volume of the audio source to 0.5
        music.volume = Settings_Values.volume / 100;

        System.Random rd = new System.Random();

        int rand_num = rd.Next(0, 4);

        TextObject = GetComponent<TextMeshProUGUI>();

        TextObject.text = ManyPassages[rand_num];

        TextObject.ForceMeshUpdate();

        redColor = new Color32(255, 0, 0, 255);
        greenColor = new Color32(14, 224, 31, 255);


        Accuracy.text = "100%";
        //Mistakes divided by correct ones

        Mistakes_len = 0f;
        wpm=0f;

        //Will get position of the box holding the text
        GameObject Container = GameObject.Find("ScrollArea");
        m_RectTransform = Container.GetComponent<RectTransform>();
        Vector3[] v = new Vector3[4];
        m_RectTransform.GetWorldCorners(v);

        //Store Box bottom coordinate in Object
        worldBottomLeftContainer = v[0];

        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        Passage = TextObject.text;
        passage_length = Passage.Length;

        color_Accuracy(100f);
        //TouchScreenKeyboard.hideInput = true;


    }

    void postProcess()
    {

        //If not last character yet then keep continuing
        if (current_Index < passage_length - 1)
        {
            current_Index++;

        }

        //Finished
        else
        {
            //Stop timer
            GameObject other = GameObject.Find("Timer");
            timeObj = other.GetComponent<RunTimer>();
            timeObj.lockUnlockTimer();
            keyboard.active = false;

            //Set results
           
        
            result_String=FinalResults.text;
            result_String = result_String.Replace("{time}", timeObj.GetTimer().ToString("0.0") + "s");
            result_String = result_String.Replace("{accuracy}", Accuracy.text);

            wpm= (Passage.Count(f => (f == ' ')) + 1) / (timeObj.GetTimer() / 60);

            result_String = result_String.Replace("{wpm}", wpm.ToString("0.0"));
            
            FinalResults.text=result_String;

            //Dislay results
            ResultScreen.SetActive(true);


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
        //Keep checking if inputs correct or not until end of passage
        if (current_Index < passage_length)
        {

            //j and prev_j keep in check whether anything has been typed recently or no
            j = keyboard.text.Length;

            
            //These conditions are in place because OnGui() is called multiple times per frame and we only want to check for input once per frame

            if ( (keyboard.text[j - 1] == Passage[current_Index]) && j - previous_j != 0) //if correct alphabet
            {
              

                //Change Color of string and now prepare to compare next letter
                if(Passage[current_Index]!=' ')
                    color_text(current_Index,true);

                postProcess();


            }

            else if ((j - previous_j != 0) && (keyboard.text[j - 1] != Passage[current_Index]))
            { 
                

                //Mistakes made
                Mistakes_len++;

                Accuracy.text = ((100 - ((Mistakes_len / passage_length) * 100))).ToString("0.00") + "%";
                


                //Change Color of string and now prepare to compare next letter
                if (Passage[current_Index] != ' ')
                    color_text(current_Index, false);

                //Get accuracy value except the % sign and update color
                color_Accuracy(float.Parse(Accuracy.text.Substring(0, Accuracy.text.Length - 1)));
                

                postProcess();
                
             
            }

            previous_j = j;

        }



    }
    void OnGUI()
    {

        keyboard_Update();
       // Accuracy.text = ((float)((3/passage_length)*100)).ToString();


    }

    // Update is called once per frame. iF above 80% accuracy then change color to green else red
    void color_Accuracy(float accuracy)
    {
        if (accuracy >= 80)
        {
            //Set green color
            Accuracy.color = greenColor;
            AccuracyLabel.color = greenColor;

        }
        else if (accuracy >= 50)
        {
            Accuracy.color = new Color32(255, 255, 0, 255);
            AccuracyLabel.color = new Color32(255, 255, 0, 255);
        }
        else if (accuracy >= 30)
        {
            Accuracy.color = new Color32(255, 128, 0, 255);
            AccuracyLabel.color = new Color32(255, 128, 0, 255);
        }

        else
        {
            Accuracy.color = redColor;
            AccuracyLabel.color = redColor;
        }

    }

    //Changes color of character depending upon whether or not the typed character was correct
    void color_text(int index,bool correct)
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
            vertexColors[vertexIndex + 0] = redColor ;
            vertexColors[vertexIndex + 1] = redColor ;
            vertexColors[vertexIndex + 2] = redColor ;
            vertexColors[vertexIndex + 3] = redColor; ;

        }
      
        

        TextObject.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }



}
