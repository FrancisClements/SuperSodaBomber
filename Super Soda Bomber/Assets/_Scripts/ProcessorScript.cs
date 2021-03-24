using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;

// Projectile Processor
/// <summary>
/// Automatically determines the projectile type and its component using the prefab.
/// </summary>

public static class ProjectileProcessor{
    /// <summary>
    /// Dictionary containing all of the projectile classes
    /// </summary>
    private static Dictionary<string, Type> projectileDict = new Dictionary<string, Type>();

    //directory of projectile prefabs
    public static string projectilePath = "Prefabs/Weapons/";
    private static string projectileName;
    private static bool isInitialized;

    /// <summary>
    /// Creates a lookup table of projectile classes.
    /// </summary>
    public static void Init(){
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
        isInitialized = true;
    }

    //returns the projectile prefab
    public static GameObject GetPrefab(string projType){
        if (projType == "")
            projType = null;

        if (projectileName == "")
            projectileName = null;

        // if the projectile name and projType is empty, call soda bomb as default
        if (projType == null){
            if (projectileName == null){
                    projectileName = "Soda Bomb";
                    Debug.Log("projectile name is empty! Redirecting to Soda Bomb");
            }
            //retain data if projectileName exists
        }
        else
            projectileName = projType;

        Debug.Log($"projectile type @ get prefab: \"{projectileName}\"");

        //loads the prefab using the directory
        return Resources.Load<GameObject>(projectilePath + projectileName) as GameObject;
    }

    //returns a projectile prefab with its corresponding script
    public static Projectile ConfigureComponent(GameObject prefab){
        Type prefabComponent;
        string name = prefab.name.Replace("(Clone)", "");

        try{
            //fetches the component and adds it
            prefabComponent = projectileDict[name];
            return prefab.AddComponent(prefabComponent) as Projectile;

        }
        catch (KeyNotFoundException){
            //key not found
            return prefab.AddComponent<SodaBomb>();
        }
    }

    public static string GetProjectileName(){
        Debug.Log($"getting projectile name: {projectileName}");
        return projectileName;
    }

    public static void SetProjectileName(string projType){
        Debug.Log($"setting projectile name: {projType}");
        if (projType != null || projType == "")
            projectileName = projType;
    }

}

public static class AbilityProcessor
{
    public static Ability Fetch(PlayerAbilities key)
    {
        //small switch statement to choose the correct ability
        switch (key)
        {
            case PlayerAbilities.LongJump:
                return new LongJump();
            case PlayerAbilities.DoubleJump:
                return new DoubleJump();
            case PlayerAbilities.Dash:
                return new Dash();
            default:
                return new Ability();
        }

        //
    }
}
