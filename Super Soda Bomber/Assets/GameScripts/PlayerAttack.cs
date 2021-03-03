﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
PlayerAttack
    Used to trigger the attacking of the player
    and how the weapon fires

    Things are needed to improve:
        The script is hard-coded. It only provides attack
        to a fixed projectile with a fixed component/perk.

        This script/other scripts are needed to be flexible
        for projectiles/weapons with different perk and
        property.

        Components that are needed to be flexible with:
            Chosen Bomb/Weapon
            Perk
            Explosion

            Different behaviours caused by a perk (i.e. cluster bomb)
*/

public class PlayerAttack : MonoBehaviour
{
    //Gameplay Debug
    public GameObject gameplayDebug;

    //Attacking source of the player. This is where the projectile comes from
    public Transform attackSource;

    //private classes
    GameplayScript gameplayScript;
    PublicScripts publicScripts;

    //weapon prefab (fix this to make it more flexible)
    public GameObject projectilePrefab;

    private float fireRate;
    private float attackTime;

    // Start is called before the first frame update
    void Awake()
    {
        // Gets GameplayScript components
		gameplayScript = gameplayDebug.GetComponent<GameplayScript>();
		publicScripts = gameplayDebug.GetComponent<PublicScripts>();

        //make this flexible to use
        fireRate = publicScripts.fireRates["sodaBomb"];
        attackTime = fireRate;
    }

    public void Attack(bool attack){
		if (attack && attackTime <= Time.time){
            //creates the projectile
            Instantiate(projectilePrefab, attackSource.position, attackSource.rotation);
            gameplayScript.AddScore(publicScripts.scores["fire"]);
            attackTime = fireRate + Time.time;
		}
	}

    // void FixedUpdate()
    // {
    //     attackTime = Time.time;
    // }
}
