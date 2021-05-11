using System.Collections;
using System.Collections.Generic;
using SuperSodaBomber.Events;
using UnityEngine;
using UnityEngine.UI;

/*
MainMenuScript
    Manages the behavior of the main menu scene.
*/
public class MainMenuScript : MonoBehaviour
{
    public GameObject continueButton;
    public Button newButton;
    
    [SerializeField] private VoidEvent onLevelNew;
    [SerializeField] private SceneEvent onLevelContinue;
    private SceneIndex savedMapIndex;

    // Start is called before the first frame update
    void Start()
    {
        savedMapIndex = LevelLoader.GetSavedScene();
        ConfigureBtnBehaviour(savedMapIndex);
    }

    void ConfigureBtnBehaviour(SceneIndex sceneIndex){
        if (newButton == null)
            return;

        if (sceneIndex == SceneIndex.None){
            newButton.onClick.AddListener(StartLevel);
            continueButton.SetActive(false);
        }
        else{
            newButton.onClick.AddListener(() => onLevelNew?.Raise());
        }
    }

    public void StartLevel(){
        savedMapIndex = LevelLoader.GetSavedScene();

        Debug.Log($"[MAINMENU] Index: {savedMapIndex}");
        TransitionLoader.UseMainMenuEvents = true;
        GameManager.current.MoveScene(savedMapIndex, false);
        onLevelContinue?.Raise(savedMapIndex);
    }

}
