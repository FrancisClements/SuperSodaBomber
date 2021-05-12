using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
Dialogue
    Script that is used at the dialogue boxes.
    It has the following functionalities:
        - "types" the dialogue message
        - uses a list of ScriptableObjects (dialogue data) to input:
            - name
            - image
            - character
        - interruptable, can skip typing animation
*/

public class Dialogue : MonoBehaviour
{
    [SerializeField] private Text charName;     //character name display
    [SerializeField] private Image charImage;   //character image display
    [SerializeField] private Text message;      //message display
    [SerializeField] private Dialogue_ScriptObject[] dialogues;     //list of dialogue data
    [SerializeField] private float textSpeed;   //speed of the typing speed

    private int dialogueIndex;                      //current index of dialogue data
    private Dialogue_ScriptObject currentDialogue;  //current dialogue data
    private bool buttonDown;            //toggled button status
    private bool neededButtonStatus;    //needed button status to move on the next line

    public bool isFinished {get; private set;} = false;     //if all of the dialogues are finished

    private Coroutine typingCoroutine;  //asyncronous work for typing the text
    private Coroutine waitCoroutine;    //asyncronous work for waiting the tap


    //Gets called when it's enabled
    void OnEnable()
    {
        //clears the placeholder info
        dialogueIndex = 0;
        StartDialogue();
    }

    /// <summary>
    /// Starts the typing of the dialogue.
    /// </summary>
    void StartDialogue(){
        currentDialogue = dialogues[dialogueIndex];

        //sets the name, image and the message
        charImage.sprite = currentDialogue.characterImage;
        charName.text = currentDialogue.characterName;
        message.text = "";

        //starts the typing
        typingCoroutine = StartCoroutine(TypeLine(dialogues[dialogueIndex]));
    }

    /// <summary>
    /// Typing animation of the dialogue.
    /// </summary>
    /// <param name="dialogueSO">Dialogue data</param>
    IEnumerator TypeLine(Dialogue_ScriptObject dialogueSO){
        foreach (char c in dialogueSO.dialogue.ToCharArray()){
            message.text += c;
            yield return new WaitForSeconds(1f/textSpeed);
        }

        //if it was not interrupted, set the needed button status
        neededButtonStatus = !buttonDown;
        waitCoroutine = StartCoroutine(WaitForTap());
    }

    /// <summary>
    /// Waits for the tap of the user to move on to the next dialogue.
    /// </summary>
    IEnumerator WaitForTap(){
        //waits for the condition to be true.
        yield return new WaitUntil(() => neededButtonStatus == buttonDown);
        dialogueIndex++;

        //if it reached the end of the array, disable the dialogue box.
        if (dialogueIndex < dialogues.Length)
            StartDialogue();
        else
            isFinished = true;

        waitCoroutine = null;
    }

    /// <summary>
    /// Interrupts the typing animation, finishing the text
    /// </summary>
    void InterruptDialogue(){
        //stops the typing animation
        StopCoroutine(typingCoroutine);

        //fill up the entire text
        message.text = dialogues[dialogueIndex].dialogue;
        neededButtonStatus = !buttonDown;

        if (waitCoroutine == null)
            waitCoroutine = StartCoroutine(WaitForTap());
    }

    void Update(){
        if(Input.GetMouseButtonDown(0)){
            buttonDown = !buttonDown;
            //tap or click to interrupt
            if (dialogueIndex < dialogues.Length && waitCoroutine == null)
                InterruptDialogue();
        }
    }

}
