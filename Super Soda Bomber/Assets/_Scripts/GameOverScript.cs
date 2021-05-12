using UnityEngine;
using UnityEngine.UI;
using SuperSodaBomber.Events;

/*
GameOverScript
    simply posts the current score
    when game over.
*/

public class GameOverScript : MonoBehaviour
{
    [SerializeField] private VoidEvent onLevelRestart;
    public Text scoreText;

    void Start(){
        scoreText.text = "Score: " + PlayerPrefs.GetInt("CurrentScore", 0).ToString();
    }

    public void RestartLevel(){
        onLevelRestart?.Raise();
    }

}
