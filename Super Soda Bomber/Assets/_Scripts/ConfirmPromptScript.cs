using UnityEngine;
using UnityEngine.UI;

/*
ConfirmPromptScript
    shows a confirm prompt when quitting, 
    restarting or selecting a new game
*/

public class ConfirmPromptScript : PublicScripts
{
    public GameplayScript gameplayScript;
    public Text messageTxt;

    //sets choices on which prompt description to add
    //confirmQuit => Are sure you want to exit the level?
    public enum Key{
        confirmQuit, confirmCheckpoint, confirmNew
    }
    public Key key;

    //automatically sets the prompt visibility to false
    void Awaken(){
        gameObject.SetActive(false);
    }
    
    //fetches the description if key is available or messageTxt was manually inputted.
    void Start(){
        if(descriptions.ContainsKey(key.ToString()) && messageTxt != null){
            messageTxt.text = descriptions[key.ToString()];
        }
    }

    //activates the prompt
    public void LoadConfirmPrompt(GameObject prompt){
        if (prompt != null){
            _TogglePrompt(prompt);
        }
        else{
            Debug.LogError("LoadConfirmPrompt has nothing to load!");
        }
    }

    //restarts the level (exclusively only for restart prompt)
    public void RestartLevel(){
        if (gameplayScript != null){
            gameplayScript.Restart();
        }
        else{
            Debug.LogError("gameplayScript is not configured!");
        }
        
    }

}
