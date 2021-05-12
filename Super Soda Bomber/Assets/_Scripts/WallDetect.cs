using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperSodaBomber.Enemies;
using SuperSodaBomber.Events;

public class WallDetect : MonoBehaviour
{
    [SerializeField] private VoidEvent onWallCollide;

    void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.layer == 9){
            onWallCollide?.Raise();
        }
    }
}
