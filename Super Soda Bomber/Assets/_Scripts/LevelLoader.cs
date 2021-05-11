using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    private SaveLoadManager<MapData> saveLoad;
    private static MapData mapData;
    private static SceneIndex savedMapIndex;

    // Start is called before the first frame update
    void Start()
    {
        saveLoad = new SaveLoadManager<MapData>("map_data");
        mapData = saveLoad.LoadData();

        //fetches the saved level
        if (mapData != null)
            savedMapIndex = (SceneIndex)(mapData.mapLevel + 2);
        else{
            savedMapIndex = SceneIndex.Level1_Game;
        }
    }

    public static SceneIndex GetSavedScene(){
        if (mapData != null)
            return savedMapIndex;
        return SceneIndex.None;
    }

    public void RestartLevel(){
        GameManager.current.MoveScene(savedMapIndex, false);
    }
}
