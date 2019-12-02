using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public LevelInfo[] levels;

    public void LoadLevel(int index)
    {
        GameManager.referencePaintingToLoad = levels[index].referencePainting;
        SceneManager.LoadScene("Painting");
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
