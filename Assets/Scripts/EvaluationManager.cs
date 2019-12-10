using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EvaluationManager : MonoBehaviour
{
    public RawImage canvasDisplay;

    public Text scoreText;

    public void Evaluate(Texture2D painting)
    {
        canvasDisplay.texture = painting;

        HashSet<Color> colors = new HashSet<Color>();
        for (int x = 0; x < painting.width; x++)
        {
            for (int y = 0; y < painting.height; y++)
            {
                Color color = painting.GetPixel(x, y);
                colors.Add(color);
            }
        }

        scoreText.text = "Score: " + colors.Count;

        GameManager.score += colors.Count;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
