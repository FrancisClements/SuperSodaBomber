using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/*
SaveLoadManager
    Handles the save and load internal processes of the game
*/

/*TODO
    - Migrate the loading and saving at this script
    - Make the file name dynamic
*/

/// <summary>
/// Non-generic version of Save Load Manager
/// </summary>
public class SaveLoadManager: MonoBehaviour{
    public string savePath { get; private set; }

    //used for save/load processes
    public BinaryFormatter bf = new BinaryFormatter();
    private string path = "saved_data";
    
    //path dir
    void Awake(){
        savePath = Application.persistentDataPath + path + ".soda";
    }
    public void SetPath(string newPath){
        savePath = Application.persistentDataPath + newPath + ".soda";
    }

    /// <summary>
    /// Deletes saved data
    /// </summary>
    public void ClearData(){
        File.Delete(savePath);
        Debug.Log("data erased!");
    }

    /// <summary>
    /// Deletes saved data (accepts path)
    /// </summary>
    public void ClearData(string path){
        SetPath(path);
        ClearData();
    }
}

/// <summary>
/// Loads, Saves and Clears Data
/// </summary>
/// <typeparam name="T">PlayerData or MapData</typeparam>
public class SaveLoadManager<T>
{
    public string savePath { get; private set; }

    //used for save/load processes
    public BinaryFormatter bf = new BinaryFormatter();
    
    //path dir
    public SaveLoadManager(string path = "saved_data"){
        savePath = Application.persistentDataPath + path+ ".soda";
    }
    
    /// <summary>
    /// Deletes saved data
    /// </summary>
    public void ClearData(){
        File.Delete(savePath);
        Debug.Log("data erased!");
    }

    public void SaveData(T data){
        Debug.Log($"Saved Path: {savePath}");
        FileStream file = File.Create(savePath);
        bf.Serialize(file, data);
        file.Close();
    }

    public T LoadData(){
        if (File.Exists(savePath)){
            //I/O
            FileStream file = File.Open(savePath, FileMode.Open);

            //load part
            T loadData = (T)bf.Deserialize(file);
            file.Close();
            return loadData;
        }
        return default(T);
    }
}

//saved_data.soda
[System.Serializable]
public class PlayerData{
    public int score;
    public float[] coords;
    public int projectileType;
    public int abilityType;
    public int map;

    //checkpoint data
    public string checkpointTag;
}

//map_data.soda
[System.Serializable]
public class MapData{
    public int runs;
    public int mapLevel; 
}