using UnityEngine;

/*
CooldownHUDManager
    Attached at the CooldownHUDManager Game Objects,
    it instantiates the cooldown icons and 
    starts the UI animation.
*/

/// <summary>
/// Location where Cooldown Icons are placed.
/// </summary>
public class CooldownHUDManager : MonoBehaviour
{
    /// <summary>
    /// Adds the UI to the HUD Manager.
    /// </summary>
    /// <param name="cooldownIcon">Icon Prefab</param>
    /// <param name="duration">Duration of the Icon</param>
    /// <param name="isCooldown">Is it labeled as a cooldown</param>
    public void AddUI(GameObject cooldownIcon, float duration, bool isCooldown){
        //instantiates the icon
        var spawnedIcon = Instantiate(cooldownIcon, transform);
        var cooldownScript = spawnedIcon.GetComponent<CooldownIcon>();

        //configure the variables
        cooldownScript.duration = duration;
        cooldownScript.isCooldown = isCooldown;

        //initiate the cooldown animation
        cooldownScript.StartUI();
    }
}
