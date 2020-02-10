using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

public class LevelManager : MonoBehaviour
{
    public LevelInfo[] levels;

    private void Awake()
    {
        string destination = Application.dataPath + "/gamedata.dat";
        if (!File.Exists(destination))
        {
            File.Create(destination);
            SaveGame();
        }
   
        LoadGame();
    }

    private void Start()
    {
        
    }

    //loads the level at the given index in the levels array
    public void LoadLevel(int index)
    {
        GameManager.levelToLoad = levels[index];

        SceneManager.LoadScene("Painting");
            
    }

    //saves the current gamedata to a file
    public void SaveGame()
    {
        GameData data = new GameData(GameManager.score, GameManager.music, GameManager.musicVol);
        string json = JsonUtility.ToJson(data);
        Debug.Log(json);

        string destination = Application.dataPath + "/gamedata.dat";
        File.WriteAllText(destination, json);
    }

    //loads gamedata from a file
    private void LoadGame()
    {
        string destination = Application.dataPath + "/gamedata.dat";
        FileStream file;

        if (File.Exists(destination)) {
            file = File.OpenRead(destination);
        } 
        else
        {
            Debug.LogError("File not found");
            return;
        }

        string contents;
        using (var sr = new StreamReader(file))
        {
            contents = sr.ReadToEnd();
        }
        file.Close();

        Debug.Log(contents);

        GameData data = JsonUtility.FromJson<GameData>(contents);

        GameManager.score = data.score;
        GameManager.music = data.music;
        GameManager.musicVol = data.musicVol;
    }
    
    public void ExitGame()
    {
        SaveGame();
        Application.Quit();
    }
}
