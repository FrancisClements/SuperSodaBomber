using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    private static SaveLoadManager<MapData> saveLoad;
    private static MapData mapData;
    private static SceneIndex savedMapIndex;

    // Start is called before the first frame update
    void Start()
    {
        saveLoad = new SaveLoadManager<MapData>("map_data");
        mapData = saveLoad.LoadData();

        FetchSceneIndex();
        
    }

    private static void FetchSceneIndex(){
        //fetches the saved level
        if (mapData != null)
            savedMapIndex = (SceneIndex)(mapData.mapLevel + 2);
        else{
            savedMapIndex = SceneIndex.Level1_Game;
        }
        Debug.Log($"[LEVELLOADER] Index: {savedMapIndex}");
    }

    public static SceneIndex GetSavedScene(){
        ReloadData();
        return savedMapIndex;
    }

    private static void ReloadData(){
        mapData = saveLoad.LoadData();
        FetchSceneIndex();
    }

    public void UpdateLevel(){
        MapData dataToSave = new MapData();

        if (mapData != null){
            if (mapData.mapLevel < 5){
                mapData.mapLevel++;
            }
            else{
                mapData.runs++;
                mapData.mapLevel = 1;
            }
            dataToSave = mapData;
        }
        else{
            dataToSave.mapLevel = 2;
            dataToSave.runs = 0;
            mapData = dataToSave;
        }

        //saves the data
        FetchSceneIndex();
        saveLoad.SaveData(dataToSave);
    }

    public void RestartLevel(){
        GameManager.current.MoveScene(savedMapIndex, false);
    }
}
