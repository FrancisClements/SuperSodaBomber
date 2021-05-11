using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SuperSodaBomber.Events;

/*
GameManager
    Oversees and holds the fate of the Scenes.
    ...Like a God
*/
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager current {get; private set;}
    [SerializeField] private VoidEvent onSceneLoaded;
    [SerializeField] private TransitionLoader slideTransition;

    [Header("Editor Only")]
    [SerializeField] private SceneIndex startingScene = SceneIndex.MainMenu;

    //Game Manager can only accept one scene at a time.
    private SceneIndex loadedSceneIndex = SceneIndex.None;
    private SceneIndex incomingSceneIndex = SceneIndex.None;
    private AsyncOperation loadedScene;
    private AsyncOperation incomingScene;

    private bool isDoneLoading;

    void Awake()
    {
        current = this;
        LoadNewScene(startingScene);
    }

    public void MoveScene(SceneIndex sceneIndex, bool activateWhenLoaded = true){
        LoadNewScene(sceneIndex, activateWhenLoaded);
    }

    private void LoadNewScene(SceneIndex sceneIndex, bool activateWhenLoaded = true){
        /*checks the following conditions:
            - if the current scene...
                - is empty OR
                - not same as the scene to add AND it's done loading
            - if the incoming scene is none (can accept one scene at a time)
        */
        if ((loadedSceneIndex == SceneIndex.None||loadedSceneIndex != sceneIndex && loadedScene.isDone) && 
            incomingSceneIndex == SceneIndex.None){
            if (loadedSceneIndex == SceneIndex.None){
                loadedScene = SceneManager.LoadSceneAsync((int) sceneIndex, LoadSceneMode.Additive);
                loadedScene.allowSceneActivation = activateWhenLoaded;
                loadedSceneIndex = sceneIndex;
            }
            else{
                incomingScene = SceneManager.LoadSceneAsync((int) sceneIndex, LoadSceneMode.Additive);
                //determines whether if the scene is allowed activate
                incomingScene.allowSceneActivation = activateWhenLoaded;
                incomingSceneIndex = sceneIndex;
                UnloadCurrentScene();
            }
        }
    }

    private void SetActiveScene(SceneIndex sceneIndex){
        int buildIndex = (int) sceneIndex;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(buildIndex));
    }

    private void UnloadCurrentScene(){
        if (loadedSceneIndex != SceneIndex.None){
            SceneManager.UnloadSceneAsync((int) loadedSceneIndex);

            //make new scene into a current one
            loadedScene = incomingScene;
            loadedSceneIndex = incomingSceneIndex;
            incomingScene = null;
            incomingSceneIndex = SceneIndex.None;
        }
        else
            Debug.LogError("Scene is not unloading");
    }

    public void ActivateNewScene(){
        if (loadedSceneIndex != SceneIndex.None){
            loadedScene.allowSceneActivation = true;
            StartCoroutine(WaitUntilDone());
        }
    }

    private IEnumerator WaitUntilDone(){
        while (!loadedScene.isDone){
            yield return null;
        }
        SetActiveScene(loadedSceneIndex);
        yield return new WaitForEndOfFrame();

    }

    public IEnumerator WaitLoadingScene(float delay, bool activate = true){
        //pattern: load -> activate

        yield return new WaitForSeconds(delay);
        //waits until the scene is loaded in memory
            //if it reaches 90%, allow the activation of the scene
        while (loadedScene.progress != .9f){
            yield return null;
        }

        if (!loadedScene.allowSceneActivation){
            onSceneLoaded?.Raise();

            if (activate)
                ActivateNewScene();

            yield return new WaitForEndOfFrame();
        }
    }

    public void StartWaitScene(float delay = 2f){
        StartCoroutine(WaitLoadingScene(delay, false));
    }

    public void StartWaitScene(bool active){
        StartCoroutine(WaitLoadingScene(0, active));
    }

    public void TransitionStart(SceneIndex sceneIndex){
        //make an if-else statement to the transition
        slideTransition.FillStart();

    }

    public void TransitionEnd(SceneIndex sceneIndex){
        slideTransition.FillEnd();
    }
}