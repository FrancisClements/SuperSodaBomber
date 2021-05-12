using UnityEngine;

/*
CheckpointScript
    Responsible for the behaviors of the checkpoint
*/

public class CheckpointScript : PublicScripts
{
    [SerializeField] private Animator animator; //animator for the checkpoint
    private bool isTouched; //used to verify if the checkpoint has been already triggered
    private TextMesh notification;

    void Awake(){
        notification = gameObject.GetComponentInChildren<TextMesh>();
        notification.gameObject.SetActive(false);
    }

    public void ChangeState(){
        //changes the sprite of the image if it's touched
        animator.SetTrigger("rise");
        isTouched = true;
    }

    public void ForceWave(){
        //forces the checkpoint flag to be already waving
        animator.Play("check_wave");
        isTouched = true;
    }
    
    void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.layer == 8 || col.gameObject.layer == 13){
            if (!isTouched){
                ChangeState();
                GameplayScript.current.AddScore(scores["checkpoint"]);
                GameplayScript.current.SetCheckpoint(transform.position, gameObject.name);
                Debug.Log("Checkpoint Saved!");
            }
            _TogglePrompt(notification.gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D col){
        if (col.gameObject.layer == 8 || col.gameObject.layer == 13)
            _TogglePrompt(notification.gameObject);
    }
}
