using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EvaluationManager : MonoBehaviour
{
    public RawImage canvasDisplay;
    public RawImage paintScreenCanvas; //needs reference so that it can scale the canvasDisplay properly

    public Text scoreText;

    public RawImage[] testing;

    private void Start()
    {
        canvasDisplay.rectTransform.sizeDelta = paintScreenCanvas.rectTransform.sizeDelta;
    }

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

        for (int i = 0; i < testing.Length; i++)
        {
            testing[i].texture = Pixelate(painting, Mathf.Pow(2, i + 1));
        }
    }

    private Texture2D Pixelate(Texture2D input, float factor)
    {
        Texture2D final = new Texture2D(input.width, input.height);

        Vector2 gridSquare = new Vector2(input.width / factor, input.height / factor);

        for (int gridX = 0; gridX < factor; gridX++)
        {
            for (int gridY = 0; gridY < factor; gridY++)
            {
                Vector3 colorTotals = new Vector3();
                for (int x = 0; x < gridSquare.x; x++)
                {
                    for (int y = 0; y < gridSquare.y; y++)
                    {
                        Color pixel = input.GetPixel(x + (int)(gridSquare.x * gridX), y + (int)(gridSquare.y * gridY));
                        colorTotals.x += pixel.r;
                        colorTotals.y += pixel.g;
                        colorTotals.z += pixel.b;
                    }
                }
                colorTotals /= gridSquare.x * gridSquare.y;
                Color averageColor = new Color(Mathf.Min(colorTotals.x, 1), Mathf.Min(colorTotals.y, 1), Mathf.Min(colorTotals.z, 1));

                for (int x = 0; x < gridSquare.x; x++)
                {
                    for (int y = 0; y < gridSquare.y; y++)
                    {
                        final.SetPixel(x + (int)(gridSquare.x * gridX), y + (int)(gridSquare.y * gridY), averageColor);
                    }
                }
          

            }
        }

        final.Apply();
        return final;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
