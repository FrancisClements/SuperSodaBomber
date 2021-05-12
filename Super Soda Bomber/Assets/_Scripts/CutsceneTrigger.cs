using UnityEngine;
using SuperSodaBomber.Events;

/*
CutsceneTrigger
    Used to trigger cutscene events such as:
        - Camera Switching
        - Dialogues
        - Removing Player Movement

    As well as triggering after the cutscene is over such as:
        - Enabling Player Movement
        - Special Animation (for Milcher)
*/

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask triggerLayers;       //layers that can trigger the cutscene
    [SerializeField] private VoidEvent onCutsceneEvent;     //event/s to call when it was triggered
    [SerializeField] private VoidEvent afterCutsceneEvent;  //event/s to call when the cutscene is over
    [SerializeField] private GameObject dialogueBox;        //dialogue box GameObject
    
    private Dialogue dialogueScript;    //script attached to the dialogue box

    void Start(){
        dialogueScript = dialogueBox.GetComponent<Dialogue>();
    }

    void OnTriggerEnter2D(Collider2D col){
        if ((triggerLayers.value & 1 << col.gameObject.layer) != 0){
            onCutsceneEvent?.Raise();
        }
    }

    void Update(){
        //calls if the dialogues are done
        if (dialogueBox.activeInHierarchy && dialogueScript.isFinished){
            onCutsceneEvent?.Raise();
            afterCutsceneEvent?.Raise();
            Destroy(gameObject);
        }
    }
}
