using System;
using System.Collections.Generic;
using UnityEngine;
using SuperSodaBomber.Events;
using UnityEngine.SceneManagement;

/*
PublicScripts
    Contains all scripts that are mostly used in the game

    Make sure to add this script at the EventSystem 
    and then use this to the OnClick() buttons if needed.
*/

public class PublicScripts : MonoBehaviour
{
    [SerializeField] private VoidEvent onSceneMove;

    //list of non-projectile scores
    protected readonly Dictionary<string, int> scores = new Dictionary<string, int>(){
        {"checkpoint", 125},
        {"ability", 10}
    };

    /*list of projectile scores
        formatting:
            if it's an explosive, add _s, _m and _l for variating scores
            otherwise, add the name as is
            
            you can find the name of the projectile at Projectile.cs, p_name
    */
    protected readonly Dictionary<string, int> projScores = new Dictionary<string, int>(){
        {"SodaBomb_s", 15},
        {"SodaBomb_m", 30},
        {"SodaBomb_l", 75},
        {"SmallCluster_s", 15},
        {"SmallCluster_m", 30},
        {"SmallCluster_l", 75},
        {"Pellet", 5},
        {"Fizztol", 15}
    };

    //description constants
    protected readonly Dictionary<string,string> descriptions = new Dictionary<string, string>(){
        {"checkSave", "Checkpoint Saved!"},
        {"confirmQuit", "Are sure you want to exit the level?"},
        {"confirmCheckpoint", "Are you sure you want to load the last checkpoint?"},
        {"confirmNew", "Are you sure you want to override your saved game data?"}
    };

    //firing rates of the weapons (shows cooldown in secs)
    protected readonly Dictionary <string, float> fireRates = new Dictionary <string, float>(){
        {"SodaBomb", .6f},
        {"Fizztol", .3f},
        {"Cannade", 1.2f},
        {"Shotgun", .8f}

    };

    /*damage matrix for projectiles
        formatting:
            if it's an explosive, add _max and _min for damage range
            otherwise, add the name as is
    */
    protected readonly Dictionary <string, float> projDamage = new Dictionary<string, float>(){
        {"SodaBomb_max", 60f},
        {"SodaBomb_min", 20f},
        {"SmallCluster_max", 45f},
        {"SmallCluster_min", 10f},
        {"Fizztol", 30f},
        {"Pellet", 15f}
    };

    private readonly Dictionary <string, SceneIndex> sceneNames = new Dictionary<string, SceneIndex>(){
        {"PersistentScene", SceneIndex.Persistence},
        {"MainMenuScene", SceneIndex.MainMenu},
        {"Options", SceneIndex.Options},
        {"Level1_Game_Level", SceneIndex.Level1_Game},
        {"Level2_Game_Level", SceneIndex.Level2_Game},
        {"Level3_Game_Level", SceneIndex.Level3_Game},
        {"Level4_Game_Level", SceneIndex.Level4_Game},
        {"StageCompleteScene", SceneIndex.StageComplete},
        {"PerkChoosingScene", SceneIndex.PerkChoose},
        {"GameOverScene", SceneIndex.GameOver}
    };

    /// <summary>
    /// Moves to selected scene
    /// </summary>
    ///

    public void _Move(string scene){
        if (sceneNames.ContainsKey(scene)){
            try{
                GameManager.current.MoveScene(sceneNames[scene], false);
                onSceneMove?.Raise();
            }
            catch{
                //prevents error in case the developer plays directly at the scene
                //instead of playing it at the persistent scene
                SceneManager.LoadScene(scene);
            }
        }
    }

    /// <summary>
    /// Moves to selected scene
    /// </summary>
    public void _Move(SceneIndex sceneIndex){
        if (sceneIndex != SceneIndex.None || sceneIndex != SceneIndex.Persistence){
            try{
                GameManager.current.MoveScene(sceneIndex, false);
                onSceneMove?.Raise();
            }
            catch{
                //prevents error in case the developer plays directly at the scene
                //instead of playing it at the persistent scene
                SceneManager.LoadScene((int)sceneIndex);
            }
        }
    }
    
    /// <summary>
    /// Toggles on/off the selected prompt
    /// </summary>
    public void _TogglePrompt(GameObject prompt){
        
        bool status = prompt.activeInHierarchy;
        prompt.SetActive(!status);
    }
}

//this will be used on abilities
[Flags]
public enum PlayerAbilities
{
    None, LongJump, DoubleJump, Dash = 4
}

public enum ProjectileTypes{
    Undefined, SodaBomb, Fizztol, Cannade, Shotgun,
    Shooter, Milcher_Bullet,
    smallCluster, pellet
}

public enum PlayerProjectiles{
    Undefined, SodaBomb, Fizztol, Cannade, Shotgun
}

public enum MapName{
    Test = -1, 
    None = 0, 
    Level1 = 1, 
    Level2 = 2, 
    Level3 = 3, 
    Level4 = 4
}

public enum SceneIndex{
    None = -1,
    Persistence,
    MainMenu,
    Options,
    Level1_Game,
    Level2_Game,
    Level3_Game,
    Level4_Game,
    StageComplete,
    PerkChoose,
    GameOver,
}

//explosion types
/// <summary>
/// Explosion Type of the Projectile
/// </summary>
    public enum ExplosionType{
        Contact = 0,        //collision triggers explosion (default)
        Detonate,           //player or time triggers explosion
        Delay,              //time triggers explosion
        Instant             //instantly explodes
    }

public interface IDamageable{
    void Damage(float hp = 1);
}