using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TEMP ONLY FOR 11/25 DEMO
    public void GoToLevel()
    {
        SceneManager.LoadScene("Painting");
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
