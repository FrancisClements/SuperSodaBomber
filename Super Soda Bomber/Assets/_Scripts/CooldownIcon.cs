using UnityEngine;
using UnityEngine.UI;

/*
Cooldown Icon
    The script for Cooldown Icon prefab.
    Animates the icon to show the ability cooldown or a powerup effect.
*/

/// <summary>
/// Used to show the duration of the powerup or the cooldown.
/// </summary>
public class CooldownIcon : MonoBehaviour
{    
    [HideInInspector] public bool isCooldown { get; set; }      //used for configuring the cooldown animation
    [HideInInspector] public float duration { get; set; }       //time to take the progress animation to take place

    private bool coolingDown = false;                           //if it will use the cooldown animation or not
    [SerializeField] private Image iconFront;                   //front "overlay" of the image

    /// <summary>
    /// Starts the animating of the cooldown icon.
    /// </summary>
    public void StartUI(){
        /*
            Cooldown:
                if yes:
                    - it will start from "disabled" state to "enabled" state
                if no:
                    - it will start from "enabled" state to "disabled" state
        */
        iconFront.fillAmount = isCooldown ? 0: 1f;
        iconFront.fillOrigin = isCooldown ? 1: 0;

        //if duration is -1, it will not have a cooldown animation
        if (duration > 0)
            coolingDown = true;

    }

    void Update(){
        if (coolingDown){
            if (isCooldown)
                //if cooldown, increase the amount according to the duration
                iconFront.fillAmount += 1f/duration * Time.deltaTime;
            else
                //otherwise, decrease the amount
                iconFront.fillAmount -= 1f/duration * Time.deltaTime;

            //if it reached the threshold, remove the icon immediately.
            if (iconFront.fillAmount == 0 || iconFront.fillAmount == 1f)
                Destroy(gameObject);
        }
    }
}
