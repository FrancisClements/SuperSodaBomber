using SuperSodaBomber.Events;
using UnityEngine;

public class FinishFlagScript : PublicScripts
{
    [SerializeField] private VoidEvent onLevelFinish;
    void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.layer == 8 || col.gameObject.layer == 13){
            onLevelFinish?.Raise();
        }
    }
}
