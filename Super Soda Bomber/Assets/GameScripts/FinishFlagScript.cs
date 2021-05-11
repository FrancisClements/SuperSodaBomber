using SuperSodaBomber.Events;
using UnityEngine;

public class FinishFlagScript : MonoBehaviour
{
    [SerializeField] private VoidEvent onLevelFinish;
    void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.layer == 8 || col.gameObject.layer == 13){
            onLevelFinish?.Raise();
            GameplayScript.current.StageComplete();
        }
    }
}
