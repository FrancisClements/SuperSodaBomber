using UnityEngine;
using UnityEngine.UI;
using SuperSodaBomber.Events;

/*
StageCompleteScript
    simply posts the current score
    when stage is complete + save and move stage.
*/

public class StageCompleteScript : MonoBehaviour
{
    public Text scoreText;
    [SerializeField] private SceneEvent onLevelContinue;

    void Start(){
        scoreText.text = "Score: " + PlayerPrefs.GetInt("CurrentScore", 0).ToString();
    }

    void PerkChoose(){
        
    }

    public void ContinueLevel(){
        SceneIndex savedMapIndex = LevelLoader.GetSavedScene();

        Debug.Log($"[STAGECOMPLETE] Index: {savedMapIndex}");
        TransitionLoader.UseMainMenuEvents = true;
        GameManager.current.MoveScene(savedMapIndex, false);
        onLevelContinue?.Raise(savedMapIndex);
    }

}
