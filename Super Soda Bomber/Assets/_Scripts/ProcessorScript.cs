using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.Events;
using SuperSodaBomber.Enemies;

/*
Processor Script
    Used to determine what component/prefab to use for:
        - Projectiles
        - Abilities
        - Powerups
        - Enemies
*/

// Projectile Processor
/// <summary>
/// Automatically determines the projectile type and its component using the prefab.
/// </summary>
public static class ProjectileProcessor{
    //stores all of the projectile types
    private static Dictionary<string, Type> projectileDict = new Dictionary<string, Type>();

    public static string projectilePath = "Prefabs/Weapons/";               //directory of projectile prefabs          
    public static PlayerProjectiles projectileType { get; private set; }    //chosen projectile type
    private static string projectileName;                                   //chosen projectile type (name)

    /// <summary>
    /// Creates a lookup table of projectile classes.
    /// </summary>
    public static void Init(){
        //incase the processor gets called again, erase the existing data
        projectileDict.Clear();

        //gets all of the Projectile classes on the current framework
        var assembly = Assembly.GetAssembly(typeof(Projectile));
        
        //filters it with the following conditions:
        //  if a class inherits Projectile
        //  if it's not an abstract class
        
        var allProjectileTypes = assembly.GetTypes()
            .Where(t => typeof(Projectile).IsAssignableFrom(t) && t.IsAbstract == false);

        //loops allProjectileTypes and store it into the dictionary
        foreach (var projType in allProjectileTypes){
            projectileDict.Add(projType.FullName, projType);
        }
    }

    /// <summary>
    /// Gets the corresponding projectile prefab.
    /// </summary>
    /// <param name="projType">Projectile Type</param>
    /// <returns>Projectile GameObject</returns>
    public static GameObject GetPrefab(PlayerProjectiles projType){
        projectileType = projType;
        if (projectileName == "")
            projectileName = null;

        // if the projectile name and projType is empty, call soda bomb as default
        if (projType == PlayerProjectiles.Undefined && projectileName == null){
            projectileName = "Soda Bomb";
            Debug.Log("projectile name is empty! Redirecting to Soda Bomb");
            //retain data if projectileName exists
        }

        //if not null, get the projectile name (string) in order to get the prefab from Assets
        else if (projType != PlayerProjectiles.Undefined)
            projectileName = EnumToString(projType);

        Debug.Log($"projectile type @ get prefab: \"{projectileName}\"");

        //loads the prefab with the directory
        return Resources.Load<GameObject>(projectilePath + projectileName) as GameObject;
    }

    /// <summary>
    /// Adds the projectile script to the prefab
    /// </summary>
    /// <param name="prefab">Instantiated Projectile GameObject</param>
    /// <returns>Prefab with added script component</returns>
    public static Projectile ConfigureComponent(GameObject prefab){
        Type prefabComponent;

        //removes the "(clone)" from, example, "SodaBomb (Clone)"
        string name = prefab.name.Replace("(Clone)", "");

        //checks if prefab name is inside the dictionary
        if (projectileDict.ContainsKey(name)){
            //fetches the component from the dictionary and adds it
            prefabComponent = projectileDict[name];
            return prefab.AddComponent(prefabComponent) as Projectile;
        }
        else
            //if the type is not found, then add SodaBomb script as default
            return prefab.AddComponent<SodaBomb>();
    }

    /// <summary>
    /// Gets the projectile name.
    /// </summary>
    public static string GetProjectileName(){
        Debug.Log($"getting projectile name: {projectileName}");
        return projectileName;
    }

    /// <summary>
    /// Updates the projectile name.
    /// </summary>
    /// <param name="projType">New projectile type</param>
    public static void SetProjectileName(PlayerProjectiles projType){
        Debug.Log($"setting projectile name: {projType}");
        string projTypeString = EnumToString(projType);
        if (projTypeString != null)
            projectileName = projTypeString;
    }

    /// <summary>
    /// Converts enum into string
    /// </summary>
    private static string EnumToString(PlayerProjectiles p){
        switch (p){
            case PlayerProjectiles.SodaBomb:
                return "Soda Bomb";
            case PlayerProjectiles.Undefined:
                return null;
            default:
                return p.ToString();
        }
    }

}

/// <summary>
/// Gets and calls the events that are used in an ability.
/// </summary>
public static class AbilityProcessor
{
    //UnityEvent for Active Abilities
    public class AbilityEvent: UnityEvent<Rigidbody2D>{}
    private static PassiveAbility passiveAbility;
    
    //contains the list of the ability events and their cooldown
    private static Dictionary<PlayerAbilities, UnityEvent<Rigidbody2D>> abilityDict = new Dictionary<PlayerAbilities, UnityEvent<Rigidbody2D>>();
    private static Dictionary<PlayerAbilities, float> abilityCooldown = new Dictionary<PlayerAbilities, float>();

    //handles and applies the cooldown of the ability
    private static Coroutine coroutine;
    public static PlayerAbilities abilities { get; private set; }
    
    /// <summary>
    /// Selects and configures the ability to the player according to the inputted key
    /// </summary>
    /// <param name="key">Chosen Ability/ies</param>
    /// <param name="controller">Player Control</param>
    public static void Fetch(PlayerAbilities key, PlayerMovement controller)
    {
        //don't configure anything if player does not have any abilities
        if (key == PlayerAbilities.None && abilities != PlayerAbilities.None)
            return;

        //clear the data if it's used again
        abilityDict.Clear();
        abilityCooldown.Clear();

        abilities = key;

        //small sets of if statement to choose the correct ability (multi-ability is supported)
        //key.HasFlag detects if it contains a certain ability
        if (key.HasFlag(PlayerAbilities.DoubleJump)){
            //ready the ability type w/ the corresponding class
            var tag = PlayerAbilities.DoubleJump;
            var obj = new DoubleJump();

            //configures the ability's UnityEvent
            var e = ConfigureEvent(obj);

            //add the UnityEvent and cooldown at the dictionary
            abilityDict.Add(tag, e);
            abilityCooldown.Add(tag, obj.cooldown);

        }
        if (key.HasFlag(PlayerAbilities.Dash)){
            var tag = PlayerAbilities.Dash;
            var obj = new Dash();
            var e = ConfigureEvent(obj);
            abilityDict.Add(tag, e);
            abilityCooldown.Add(tag, obj.cooldown);
            controller.flipEvent += obj.OnFlip;
        }

        if (key.HasFlag(PlayerAbilities.LongJump)){
            passiveAbility = new LongJump();
            controller.m_JumpForce = passiveAbility.ApplyPassiveAbility(
                controller.m_JumpForce);
        }
    }

    /// <summary>
    /// Initializes of the ability with the UnityEvent.
    /// </summary>
    private static UnityEvent<Rigidbody2D> ConfigureEvent(ActiveAbility activeAbility){
            var uEvent = new AbilityEvent();
            activeAbility.Init(uEvent);
            return uEvent;
    }

    /// <summary>
    /// Searches the key from the dictionary and then call it.
    /// </summary>
    /// <param name="key">Player Ability Type</param>
    /// <param name="r">Player RigidBody2D</param>
    public static void CallEvent(PlayerAbilities key, Rigidbody2D r){
        if(abilityDict.ContainsKey(key)){
            abilityDict[key].Invoke(r);
        }
    }

    /// <summary>
    /// Gets the cooldown of the active ability.
    /// </summary>
    /// <param name="key">Active Ability</param>
    public static float GetCooldown(PlayerAbilities key){
        if(abilityDict.ContainsKey(key)){
            return abilityCooldown[key];
        }
        return 0;
    }
}

/// <summary>
/// Utilizes and gets enemy info.
/// </summary>
public static class EnemyProcessor{
    //stores the enemy script along with its type.
    private static Dictionary<EnemyType, Type> enemyDict = new Dictionary<EnemyType, Type>();
    private static bool isConfig = false;

    /// <summary>
    /// Configures the Enemy Dictionary.
    /// </summary>
    private static void Configure(){
        enemyDict.Add(EnemyType.Shooter, typeof(Shooter));
        enemyDict.Add(EnemyType.Roller, typeof(Roller));
        isConfig = true;
    }

    /// <summary>
    /// Gets the corresponding enemy script.
    /// </summary>
    /// <param name="scriptObject">Enemy Scriptable Object</param>
    /// <returns></returns>
    public static Type Fetch(Enemy_ScriptObject scriptObject){
        if (!isConfig)
            Configure();

        return enemyDict[scriptObject.enemyType];
    }
}