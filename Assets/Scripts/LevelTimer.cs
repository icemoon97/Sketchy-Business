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

        string secondsString = seconds.ToString();
        if (seconds < 10)
        {
            secondsString = "0" + secondsString;
        }
            
        string minutesString = minutes.ToString();
        if (minutes < 10)
        {
            minutesString = "0" + minutesString;
        }   

        timerDisplay.text = "Time Remaining: " + minutesString + ":" + secondsString;

        if (time < 10)
        {
            timerDisplay.color = Color.red;
        }
    }
}
