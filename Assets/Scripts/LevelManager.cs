using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelManager : MonoBehaviour
{
    public LevelInfo[] levels;

    public GameObject[] levelButtons;
    public Sprite lockImage;

    private void Awake()
    {
        string destination = Application.dataPath + "/gamedata.dat";
        if (!File.Exists(destination))
        {
            FileStream file = File.Create(destination);
            file.Close(); //needed because I am using file.writealltext() later and leaving the file open would cause a sharing violation

            GameManager.levelScore = new List<int>(); //needs to be a list and not an array because arrays are not serializable by JsonUtility
            for (int i = 0; i < levels.Length; i++)
            {
                GameManager.levelScore.Add(-1); 
            }

            GameManager.paintings = new List<PaintingInfo>();

            //default settings
            GameManager.score = 0;
            GameManager.music = true;
            GameManager.musicVol = 0.2f;
        }

        //creates folder for paintings if it doesn't exist already
        DirectoryInfo paintingFolder = new DirectoryInfo(Application.dataPath + "\\User Paintings");
        if (!paintingFolder.Exists)
        {
            paintingFolder.Create();
        }

        if (GameManager.levelScore != null) //so that game is saved automatically when returning to main menu
        {
            SaveGame();
        }
        LoadGame(); 
    }

    private void Start()
    {
        if (GameManager.levelScore.IndexOf(-1) != -1)
        {
            for (int i = GameManager.levelScore.IndexOf(-1) + 1; i < levelButtons.Length; i++) //loops through locked levels
            {
                levelButtons[i].transform.GetChild(1).GetComponent<Image>().sprite = lockImage;
                levelButtons[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    //saves the current gamedata to a file
    public void SaveGame()
    {
        GameData data = new GameData(GameManager.score, GameManager.levelScore, GameManager.paintings, GameManager.music, GameManager.musicVol);
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

        GameData data = JsonUtility.FromJson<GameData>(contents);

        GameManager.score = data.score;
        GameManager.levelScore = data.levelScore;
        GameManager.paintings = data.paintings;
        GameManager.music = data.music;
        GameManager.musicVol = data.musicVol;
    }
    
    public void ExitGame()
    {
        SaveGame();

        Application.Quit();
    }

    //loads the level at the given index in the levels array
    public void LoadLevel(int index)
    {
        GameManager.levelToLoad = levels[index];
        GameManager.currentLevelIndex = index;

        SceneManager.LoadScene("Painting");

    }
}
