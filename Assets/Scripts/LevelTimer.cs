using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    [HideInInspector]
    public float time;

    public Text timerDisplay; //UI Text Object

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1; //Just making sure that the timeScale is right
    }

    //Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;

        updateDisplay();
    }

    //updates text display with correct formatting
    private void updateDisplay()
    {
        int seconds = (int)time % 60;
        int minutes = (int)time / 60;

        if (seconds < 10 && minutes < 10)
        {
            timerDisplay.text = ("Time Remaining: 0" + minutes + ":0" + seconds);
        }
        else if (minutes < 10)
        {
            timerDisplay.text = ("Time Remaining: 0" + minutes + ":" + seconds);
        }
        else if (seconds < 10)
        {
            timerDisplay.text = ("Time Remaining: " + minutes + ":0" + seconds);
        }
        else
        {
            timerDisplay.text = ("Time Remaining: " + minutes + ":" + seconds);
        }
    }
}
