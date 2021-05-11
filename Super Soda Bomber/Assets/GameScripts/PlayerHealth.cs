using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperSodaBomber.Events;

/*
Player Health
    Manages the health behavior of the player.
    It also triggers the damage and death of the player.
*/

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField][Range(0, 10)] private int health = 3;  //health of the player
    [SerializeField] private VoidEvent onPlayerDamageEvent; //events to trigger when player takes damage (e.g. sound)
    [SerializeField] private VoidEvent onPlayerDeathEvent;  //events to trigger when player dies

    private GameObject player;          //player gameobject
    private Coroutine coroutine;        //asynchronous work
    private bool isTempImmune;          //player status if it's temporarily immuned
    private SpriteRenderer p_Renderer;  //player sprite renderer
    private Dictionary<string, int> colliderLayer;          //dictionary of prepared collider layers that the player uses

    void Awake(){
        player = gameObject;
        p_Renderer = player.GetComponent<SpriteRenderer>();

        //prepare the layer masks
        colliderLayer = new Dictionary<string, int>(){
            {"phantom", LayerMask.NameToLayer("Phantom")},
            {"player", LayerMask.NameToLayer("Player")}
        };
    }

    /// <summary>
    /// Damages the player.
    /// </summary>
    /// <param name="hp">[not needed] Hit points</param>
    public void Damage(float hp = 1){
        //if it has temp immunity, do nothing
        if (isTempImmune)
            return;

        --health;

        //activates the event (sounds) and updates hud
        onPlayerDamageEvent?.Raise();
        GameplayScript.SetHpUI(health<0 ? 0 : health);

        if (health <= 0)
            OnPlayerDeath();
        else{
            //if it has health left, call the temp immunity
            coroutine = StartCoroutine(TemporaryImmunity());
        }
    }

    /// <summary>
    /// Adds player a hit point.
    /// </summary>
    public void AddHP(){
        ++health;
        GameplayScript.SetHpUI(health);
    }

    /// <summary>
    /// Call the events when player dies.
    /// </summary>
    void OnPlayerDeath(){
        if (coroutine != null)
            StopCoroutine(coroutine);
        onPlayerDeathEvent?.Raise();
        GameplayScript.current.GameOver();
    }

    void FixedUpdate()
    {
        //if it fell, die.
        if (player.transform.position.y < 0){
            OnPlayerDeath();
        }
    }

    //triggers when player touches an enemy
	void OnCollisionStay2D(Collision2D col){
		if (col.gameObject.layer == 11 || col.gameObject.layer == 14){
			Damage();
		}
	}

    private IEnumerator TemporaryImmunity(){
        //contains number of seconds to blink 10 times
        float[] durationsArr = {2f, 1f};   //in seconds
        float blinkCycle = 10f;
        isTempImmune = true;
        gameObject.layer = colliderLayer["phantom"];

        Color oldColor = p_Renderer.color;  //opaque
        Color blinkColor = new Color(oldColor.r, oldColor.g, oldColor.b, .25f);  //semi-transparent

        //loops through the durations array
        for (int i = 0; i < durationsArr.Length; i++){
            for (int j = 0; j < blinkCycle; j++){
                
                //changes the player's renderer color using the blinkCycle
                if (j%2 == 0)
                    p_Renderer.color = blinkColor;
                else
                    p_Renderer.color = oldColor;

                //e.g. 2 seconds/10 cycles
                yield return new WaitForSeconds(durationsArr[i]/blinkCycle);
            }
        }

        gameObject.layer = colliderLayer["player"];
        isTempImmune = false;
    }
}
