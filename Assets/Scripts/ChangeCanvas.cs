using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCanvas : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelSelect;

    public Transform cameraPos;
    public Transform finalPos;

    //public CameraPan cameraP;

    public bool clicked;

    public void click()
    {
        clicked = true;
    }

    public void Update()
    {
        /*
        if (clicked) 
        {
            mainMenu.SetActive(false);
            if (cameraP.completed)
            {
                levelSelect.SetActive(true);
            }
        }
        */
    }
}
