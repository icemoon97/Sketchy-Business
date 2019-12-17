using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public LevelInfo[] levels;

    private void Start()
    {

    }

    public void LoadLevel(int index)
    {
        GameManager.levelToLoad = levels[index];

        SceneManager.LoadScene("Painting");
            
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
