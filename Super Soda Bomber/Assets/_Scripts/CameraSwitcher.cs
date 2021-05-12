using UnityEngine;

/*
Camera Switcher
    Switches the camera. Manly used for cutscenes.
*/

public class CameraSwitcher : MonoBehaviour
{
    private Animator cameraAnimator;
    private bool cameraAtMain = true;

    void Awake(){
        cameraAnimator = GetComponent<Animator>();
    }
    
    public void SwitchCameraState(){
        if (cameraAtMain)
            cameraAnimator.Play("cutscene_camera");
        else
            cameraAnimator.Play("main_camera");

        cameraAtMain = !cameraAtMain;
    }
}
