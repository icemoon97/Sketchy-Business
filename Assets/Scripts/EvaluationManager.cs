using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class EvaluationManager : MonoBehaviour
{
    public PaintManager paintManager;

    public RawImage referenceDisplay;
    public RawImage userCanvasDisplay;

    public Text scoreText;
    public Text feedbackText;

    [Header("Debug Stuff")]
    public RawImage[] referencesTesting;
    public RawImage[] userPaintingTesting;
    public RawImage[] differenceTesting;
    public Text[] differenceAverages;

    public void Evaluate(Texture2D painting)
    {
        userCanvasDisplay.rectTransform.sizeDelta = new Vector2(painting.width, painting.height);

        feedbackText.gameObject.SetActive(true);

        int totalScore = CalcScore(painting);

        scoreText.text = "Score: " + totalScore;

        if (totalScore >= GameManager.levelToLoad.scoreCutoff)
        {
            feedbackText.text = "What a masterpiece!";

            GameManager.score += totalScore;
            if (totalScore > GameManager.levelScore[GameManager.currentLevelIndex])
            {
                GameManager.levelScore[GameManager.currentLevelIndex] = totalScore;
            }
        }
        else
        {
            feedbackText.text = "This is clearly a forgery!";
        }

        SavePainting(painting, totalScore);
    }

    //calculates score based upon difference between reference and user painting at different levels of pixelation
    private int CalcScore(Texture2D painting)
    {
        Texture2D simplified = SimplifyColors((Texture2D)paintManager.referencePainting.mainTexture, GameManager.levelToLoad.colorPalette);

        referenceDisplay.texture = simplified;
        userCanvasDisplay.texture = painting;

        //creates a series of increasingly pixelated textures, both of user painting and reference. (pattern is 2x2, 4x4, 8x8, etc)
        for (int i = 0; i < userPaintingTesting.Length; i++)
        {
            userPaintingTesting[i].texture = Pixelate(painting, Mathf.Pow(2, i + 1));
        }

        for (int i = 0; i < referencesTesting.Length; i++)
        {
            referencesTesting[i].texture = Pixelate(simplified, Mathf.Pow(2, i + 1));
        }

        float totalScore = 0;

        for (int i = 0; i < differenceTesting.Length; i++)
        {
            //creates new texture where each pixel represents the relative difference between 
            //the pixel in the users painting and the reference painting, according to the LABColor formula
            Texture2D diffTex = CalcDistances((Texture2D)referencesTesting[i].texture, (Texture2D)userPaintingTesting[i].texture);
            differenceTesting[i].texture = diffTex;

            //score is simply calculated as average of differences
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

            totalScore += average;
        }

        totalScore = Mathf.Round((5 - totalScore) * 100);

        return (int)totalScore;
    }

    //returns a new texture, pixelated to the given factor, eg a factor of 3 means texture is turned into 9 (3x3) blocks of color
    //colors are averaged by adding up RGB values (idk how good of a method that is, I'm not a color theory expert)
    private Texture2D Pixelate(Texture2D input, float factor)
    {
        Texture2D final = new Texture2D(input.width, input.height);

        Vector2 gridSquare = new Vector2(input.width / factor, input.height / factor);

        for (int gridX = 0; gridX < factor; gridX++)
        {
            for (int gridY = 0; gridY < factor; gridY++)
            {
                //adds up all RGB values in the grid square so average color can be determined
                //there might be some better way to determine an average color from a block, but for now this works fine
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

        //only uploads texture data after all changes have been made for efficency
        final.Apply();

        return final;
    }

    //returns a new texture with all pixels changed to the closest color from the given palette
    private Texture2D SimplifyColors(Texture2D input, Color[] palette)
    {
        Texture2D final = new Texture2D(input.width, input.height);

        //converting all colors to LABColor for comparison
        LABColor[] colors = new LABColor[palette.Length];
        for (int i = 0; i < palette.Length; i++)
        {
            colors[i] = LABColor.FromColor(palette[i]);
        }

        for (int x = 0; x < input.width; x++)
        {
            for (int y = 0; y < input.height; y++)
            {
                //converts to LABColor so a proper comparison can be made
                LABColor pixel = LABColor.FromColor(input.GetPixel(x, y));

                float nearestDist = 999999;
                LABColor nearest = colors[0];

                //compares each color in palette to pixel
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

                //sets pixel to closest color
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
            throw new Exception("textures are different sizes");
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

    //saves created painting to user paintings folder as a .png
    //also saves info about the attempt
    private void SavePainting(Texture2D painting, int score)
    {
        //generating a random file name for painting image
        System.Random r = new System.Random();
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string random = "";
        for (int i = 0; i < 20; i++)
        {
            random += chars[r.Next(chars.Length)];
        }

        System.IO.File.WriteAllBytes(Application.dataPath + "\\User Paintings\\" + random + ".png", painting.EncodeToPNG());

        string date = DateTime.Now.ToString();
        date = date.Split()[0];

        PaintingInfo info = new PaintingInfo();
        info.fileName = random;
        info.score = score;
        info.caught = score < GameManager.levelToLoad.scoreCutoff;
        info.funFact = GameManager.levelToLoad.funFact;
        info.datePainted = date;
        info.paintingName = GameManager.levelToLoad.paintingName;

        GameManager.paintings.Add(info);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
