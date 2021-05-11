using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
GameplayScript
    Responsible for the behavior of the 
    whole gameplay proper such as:
        Saving
        Loading, 
        Adding Current Score
        Scene Changes (Game Over, Stage Complete)

    and events

*/

public class GameplayScript : PublicScripts
{
    /*
    Processes:
    When user touches the checkpoint:
        - activated save function
        - change the state of the checkpoint
        - change the image

    When game starts:
        - load the scene and saved files
        - add player states
    */
    
    //Config Variables
    [SerializeField] private GameObject scoreTxtObject, hpTxtObject, player, tileObject, pausePrompt;

    private Text scoreTxt, hpTxt;
    private static int health;

    //Variables to Save
    private int score = 0;
    private Vector3 coords;
    private string checkpointTag;
    public MapName mapName;

    private bool isPaused = false;

    //Removes the object dependency using a self-static variable.
    public static GameplayScript current;
    private SaveLoadManager<PlayerData> saveLoadPlayer;
    private SaveLoadManager<MapData> saveLoadMap;

    void Awake(){
        saveLoadPlayer = new SaveLoadManager<PlayerData>();
        saveLoadMap = new SaveLoadManager<MapData>("map_data");
    }

    void Start(){
        Load();
        current = this;
        scoreTxt = scoreTxtObject.GetComponent<Text>();
        hpTxt = hpTxtObject.GetComponent<Text>();
    }

    /// <summary>
    /// Adds and updates the score
    /// </summary>
    /// <param name="amount">score to add</param>
    public void AddScore(int amount){
        if(amount > 0)
            score += amount;
    }

    /// <summary>
    /// Sets and saves the checkpoint data.
    /// </summary>
    /// <param name="checkpointCoords">Checkpoint position</param>
    /// <param name="name">Name of the checkpoint</param>
    public void SetCheckpoint(Vector3 checkpointCoords, string name){
        coords = checkpointCoords;
        checkpointTag = name;
        Save();
    }

    //save game
    public void Save(){
        coords += new Vector3(0, .5f, 0);

        //I/O
        PlayerData playerData = new PlayerData();
        playerData.score = score;
        playerData.coords = new float[] {coords[0], coords[1], coords[2]};
        playerData.checkpointTag = checkpointTag;
        playerData.projectileType = (int) ProjectileProcessor.projectileType;
        playerData.map = (int) mapName;
        playerData.abilityType = (int) AbilityProcessor.abilities;
        Debug.Log($"saved projectile: {ProjectileProcessor.projectileType}");

        MapData mapData = new MapData();
        mapData.mapLevel = (int) mapName;

        //save part
        saveLoadPlayer.SaveData(playerData);
        saveLoadMap.SaveData(mapData);
    }

    //load game
    public void Load(){
        PlayerData playerData = saveLoadPlayer.LoadData();
        if (playerData != null){
            MapName savedMap = (MapName) playerData.map;

            //don't load if the data is for a different map
            if (savedMap == 0 || savedMap.Equals(mapName)){
                score = playerData.score;
                PlayerPrefs.SetInt("CurrentScore", score);

                float[] c = playerData.coords;
                coords = new Vector3(c[0], c[1], c[2]);
                player.transform.position = coords;
                
                var playerMove = player.GetComponent<PlayerMovement>();
                AbilityProcessor.Fetch((PlayerAbilities) playerData.abilityType, playerMove);

                ProjectileProcessor.SetProjectileName((PlayerProjectiles) playerData.projectileType);

                /*
                Sample Hierarchy of GameObject Tile
                to change the checkpoint image
                    Tile
                        -> Obstacles
                        -> Checkpoint1
                            -> CheckpointScript

                Process:
                    - Find the child gameobject using the name
                    - Call the ChangeState() of the child script
                */

                //name of the loaded checkpoint
                checkpointTag = playerData.checkpointTag;

                //gets the list its children
                Transform[] childrenObj = tileObject.GetComponentsInChildren<Transform>();

                foreach(Transform obj in childrenObj){
                    //if name matches with checkpointTag, change the state
                    if (obj.name == checkpointTag){
                        CheckpointScript objScript = obj.GetComponent<CheckpointScript>();
                        objScript.ForceWave();
                        break;
                    }
                }
            }
            else
                Debug.Log("Data is for the different map. Data is not loaded");
        }
    }

    public static void SetHpUI(int newHP){
        health = newHP;
    }

    //when player dies
    public void GameOver(){
        //save the current score at PlayerPrefs
        PlayerPrefs.SetInt("CurrentScore", score);
        _Move(SceneIndex.GameOver);
    }

    //when stage has been completed
    public void StageComplete(){
        //save the current score at PlayerPrefs
        PlayerPrefs.SetInt("CurrentScore", score);
        _Move(SceneIndex.StageComplete);

    }

    public void _TogglePause(){
        _TogglePrompt(pausePrompt);
        isPaused = !isPaused;
    }

    //DevTools
    public void Restart(){
        if (File.Exists(saveLoadPlayer.savePath)){
            Load();
        }
        else{
            _Move(SceneManager.GetActiveScene().name);
        }
    }

    /*
        HOTKEYS
            r - Restart
            c - Erase Data
            esc - Pause
    */
    void Update(){
        if(Input.GetKey("r")){
            Restart();
        }
        else if(Input.GetKey("c")){
            saveLoadPlayer.ClearData();
        }
        else if(Input.GetKeyDown(KeyCode.Escape)){
            _TogglePause();
        }

        if(isPaused){
            Time.timeScale = 0f;
        }
        else{
            Time.timeScale = 1f;
        }
        
    }

    void LateUpdate()
    {
        scoreTxt.text = $"{score}";
        hpTxt.text = health.ToString();
    }
}