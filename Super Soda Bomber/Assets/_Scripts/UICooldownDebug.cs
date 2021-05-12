using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/*
UICooldownDebug
    Manages the calling of the UI Icons.
        - Hud Button is located at the top of the attack button
        - Hud Score is located below the score and the HP
*/

/// <summary>
/// Manages the calling of the UI Icons. Must be included at GameplayDebug.
/// </summary>
public class UICooldownDebug : MonoBehaviour
{
    [SerializeField] private CooldownHUDManager hudButton;  //hud manager that is located at the top of the attack button
    [SerializeField] private CooldownHUDManager hudScore;   //hud manager that is located below the score
    [SerializeField] private GameObject iconPrefab;         //cooldown icon gameobject

    [Space]
    [Header("Sprite List")]

    [SerializeField] private List<SpriteDict> spriteList;   //list of icons and their names
    
    private GameObject template;                            //edited version of the prefab (for changing the image)

    private Dictionary<string, GameObject> templateCache = 
    new Dictionary<string, GameObject>();                   //list of templates. used to save resources

    public static UICooldownDebug current;      //static object of the script

    void Start(){
        //sets the static object
        current = this;
    }

    /// <summary>
    /// Generates and Calls the Cooldown UI of the ability.
    /// </summary>
    /// <param name="name">Name of the of the ability.</param>
    /// <param name="location">Location where to place the icon (button or score only)</param>
    /// <param name="duration">Duration of the UI</param>
    public void CallCooldownUI(string name, string location, float duration = -1f){
        location = location.ToLower();

        SpriteDict chosenDict;

        //if the name has an existing template, use it
        if (templateCache.ContainsKey(name)){
            template = templateCache[name];
            chosenDict = spriteList.Find(i => i.name == name);
        }

        //if not, generate a new template
        else{
            template = Resources.Load("Prefabs/UI/CooldownUIIcon") as GameObject;

            //finds if name is within the spriteList
            if (spriteList.Exists(i => i.name == name)){

                //repetitive function to prevent errors
                chosenDict = spriteList.Find(i => i.name == name);

                //sets the sprite icon
                Image[] icons = template.GetComponentsInChildren<Image>();

                foreach (Image icon in icons)
                {
                    icon.sprite = chosenDict.icon;
                }

                //adds it to cache
                templateCache.Add(name, Instantiate(template));
            }

            //if it doesn't exist, do nothing
            else
                return;
        }

        //check if the chosen HUD location is not null
        if (location == "button" && hudButton == null ||
            location == "score" && hudScore == null){
                Debug.LogError($"HUD for {location} is empty! Check the UI GameObject if the HUD is there.");
                return;
            }

        //add UI to location
        switch (location){
            case "button":
                hudButton.AddUI(template, duration, chosenDict.isCooldown);
                break;
            case "score":
                hudScore.AddUI(template, duration, chosenDict.isCooldown);
                break;
            default:
                break;
        }
    }
}

/// <summary>
/// Make up "dictionary" data structure of the sprite.
/// </summary>
[Serializable]
public struct SpriteDict
{
    public string name;
    public bool isCooldown;
    public Sprite icon;
}
