using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public GameObject panel;

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.D)) 
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}
