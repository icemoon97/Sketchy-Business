using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EvaluationManager : MonoBehaviour
{
    public PaintManager paintManager;

    public RawImage referenceDisplay;
    public RawImage userCanvasDisplay;

    public Text scoreText;

    public RawImage[] referencesTesting;
    public RawImage[] userPaintingTesting;
    public RawImage[] differenceTesting;
    public Text[] differenceAverages;

    private void Start()
    {
        //referenceDisplay.rectTransform.sizeDelta = paintManager.image.rectTransform.sizeDelta;
    }

    public void Evaluate(Texture2D painting)
    {
        Texture2D simplified = SimplifyColors((Texture2D)paintManager.referencePainting.mainTexture, GameManager.levelToLoad.colorPalette);

        referenceDisplay.texture = simplified;
        userCanvasDisplay.texture = painting;

        for (int i = 0; i < userPaintingTesting.Length; i++)
        {
            userPaintingTesting[i].texture = Pixelate(painting, Mathf.Pow(2, i + 1));
        }

        for (int i = 0; i < referencesTesting.Length; i++)
        {
            referencesTesting[i].texture = Pixelate(simplified, Mathf.Pow(2, i + 1));
        }

        for (int i = 0; i < differenceTesting.Length; i++)
        {
            Texture2D diffTex = CalcDistances((Texture2D)referencesTesting[i].texture, (Texture2D)userPaintingTesting[i].texture);
            differenceTesting[i].texture = diffTex;

            float average = 0;
            for (int x = 0; x < diffTex.width; x++)
            {
                for (int y = 0; y < diffTex.height; y++)
                {
                    average += diffTex.GetPixel(x, y).r;
                }
            }
            average /= diffTex.width * diffTex.height;

            differenceAverages[i].text = "" + average;
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

    //returns a new texture with all pixels changed to the closest color from the given palette
    private Texture2D SimplifyColors(Texture2D input, Color[] palette)
    {
        Texture2D final = new Texture2D(input.width, input.height);

        LABColor[] colors = new LABColor[palette.Length];
        for (int i = 0; i < palette.Length; i++)
        {
            colors[i] = LABColor.FromColor(palette[i]);
        }

        for (int x = 0; x < input.width; x++)
        {
            for (int y = 0; y < input.height; y++)
            {
                LABColor pixel = LABColor.FromColor(input.GetPixel(x, y));

                float nearestDist = 999999;
                LABColor nearest = colors[0];

                foreach (LABColor color in colors)
                {
                    float dist = LABColor.Distance(pixel, color);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearest = color;
                    }
                }

                Color toSet = LABColor.ToColor(nearest);

                final.SetPixel(x, y, toSet);
            }
        }

        final.Apply();
        return final;
    }

    //returns a new texture, with each pixel being the LABColor distance between the same pixels in the two given textures.
    //differences are from 0-1, with 0 being the same color and 1 being the distance from black to white
    private Texture2D CalcDistances(Texture2D a, Texture2D b)
    {
        if (a.width != b.width || a.height != b.height)
        {
            throw new System.Exception("textures are different sizes");
        }

        Texture2D final = new Texture2D(a.width, a.height);

        float[,] differences = new float[final.width, final.height];

        for (int x = 0; x < final.width; x++)
        {
            for (int y = 0; y < final.height; y++)
            {
                Color colorA = a.GetPixel(x, y);
                Color colorB = b.GetPixel(x, y);

                differences[x, y] = LABColor.Distance(LABColor.FromColor(colorA), LABColor.FromColor(colorB));
            }
        }

        float maxDiff = LABColor.Distance(LABColor.FromColor(Color.black), LABColor.FromColor(Color.white));

        for (int x = 0; x < final.width; x++)
        {
            for (int y = 0; y < final.height; y++)
            {
                differences[x, y] /= maxDiff;
                final.SetPixel(x, y, new Color(differences[x, y], 0, 0));
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
