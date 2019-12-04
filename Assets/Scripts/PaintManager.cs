using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PaintManager : MonoBehaviour {

    public Image referencePainting;

    public RawImage image;

    private Texture2D canvas;
    private List<Texture2D> history; //list of all previous states of the canvas
    private int historyIndex; //current place in history (tracks user if they undo/redo multiple times)

    public BrushStyle brushStyle;
    public int brushSize;
    public Color brushColor;

    public int sprayCanDensity;

    private Vector3 prevMousePosition;

    public enum BrushStyle
    {
        SquareBrush,
        CircularBrush,
        SprayCan,
        PaintBucket
    }

	// Use this for initialization
	void Start ()
    {
        referencePainting.sprite = GameManager.referencePaintingToLoad;

        canvas = new Texture2D((int)image.rectTransform.rect.width, (int)image.rectTransform.rect.height); 

        image.texture = canvas;

        history = new List<Texture2D>();
        ClearCanvas();
        historyIndex = 0;

        prevMousePosition = getAdjustedMousePos();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 mousePos = getAdjustedMousePos();

        if (Input.GetMouseButton(0))
        {
            for (float i = 0; i <= 1; i += 2 / Vector3.Distance(mousePos, prevMousePosition))
            {
                Draw(Vector3.Lerp(mousePos, prevMousePosition, i));
            }
        }

        prevMousePosition = mousePos;
    }

    //draws at given position with current color, size and brush style
    private void Draw(Vector3 pos)
    {
        Rect canvasBounds = new Rect(0, 0, canvas.width, canvas.height);
        if (brushStyle == BrushStyle.SquareBrush)
        {
            for (int x = -brushSize / 2; x < brushSize / 2; x++)
            {
                for (int y = -brushSize / 2; y < brushSize / 2; y++)
                {
                    Vector2 pixel = new Vector2(Mathf.RoundToInt(pos.x + x), Mathf.RoundToInt(pos.y + y));
                    if (canvasBounds.Contains(pixel)) //makes sure pixels are within bounds
                    {
                        canvas.SetPixel((int)pixel.x, (int)pixel.y, brushColor);
                    }
                }
            }
        }
        else if (brushStyle == BrushStyle.CircularBrush)
        {
            for (int x = -brushSize / 2; x < brushSize / 2; x++)
            {
                for (int y = -brushSize / 2; y < brushSize / 2; y++)
                {
                    Vector2 pixel = new Vector2(Mathf.RoundToInt(pos.x + x), Mathf.RoundToInt(pos.y + y));
                    if (canvasBounds.Contains(pixel) && Vector2.Distance(pos, pixel) < brushSize / 2) //makes sure pixels are within bounds
                    {
                        canvas.SetPixel((int)pixel.x, (int)pixel.y, brushColor);
                    }
                }
            }
        }
        else if (brushStyle == BrushStyle.SprayCan)
        {
            for (int i  = 0; i < sprayCanDensity * (brushSize / 10); i++)
            {
                Vector2 rand = UnityEngine.Random.insideUnitCircle * (brushSize / 2);
                Vector2Int pixel = new Vector2Int(Mathf.RoundToInt(pos.x + rand.x), Mathf.RoundToInt(pos.y + rand.y));
                if (canvasBounds.Contains(pixel))
                {
                    canvas.SetPixel(pixel.x, pixel.y, brushColor);
                }
            }
        }

        canvas.Apply();
    }

    //paint bucket function, which is called from a click event from the canvas. 
    //It cannot work like the other brushstyles, which use linear interpolation and are called many many times per mouse drag, 
    //which would be super intensive and produce weird behavior with the paint bucket
    public void PaintBucket()
    {
        if (brushStyle == BrushStyle.PaintBucket)
        {
            Vector3 mousePos = getAdjustedMousePos();
            Vector2Int pixel = new Vector2Int(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
            Color overwrite = canvas.GetPixel(pixel.x, pixel.y);
            Debug.Log("Start: " + pixel + ", " + overwrite.ToString());

            Rect canvasBounds = new Rect(0, 0, canvas.width, canvas.height);

            HashSet<Vector2Int> points = new HashSet<Vector2Int>();
            points.Add(pixel);

            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

            int i = 0;

            while (points.Count > 0 && i < 1000)
            {
                i++;
                Vector2Int point = new Vector2Int(-1, -1); //placeholder
                foreach (Vector2Int item in points)
                {
                    point = item;
                    break;
                }
                Debug.Log("Point: " + point + " , Color: " + canvas.GetPixel(point.x, point.y));
                if (canvas.GetPixel(point.x, point.y) == overwrite) {
                    canvas.SetPixel(point.x, point.y, brushColor);
                    points.Remove(point);
                    visited.Add(point);
                    Debug.Log(string.Join(" ", visited));
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int test = new Vector2Int(point.x + x, point.y + y);
                            //Debug.Log("Test: " + test + ", " + !points.Contains(test) + ", " + !visited.Contains(test)  + ", " + canvasBounds.Contains(test));
                            if (!points.Contains(test) && !visited.Contains(test) && canvasBounds.Contains(test))
                            {
                                points.Add(test);
                            }
                        }
                    }
                }
            }
        }
    }

    //returns relative mouse position within canvas texture
    private Vector3 getAdjustedMousePos()
    {
        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        image.rectTransform.GetWorldCorners(corners);

        return mousePos - corners[0];
    }

    public void SetBrushSize(int size)
    {
        brushSize = size;
    }

    public void SetColor(Color color)
    {
        brushColor = color;
    }

    public void SetBrushStyle(int style)
    {
        brushStyle = (BrushStyle)style;
    }

    //gets passed in a button that was just clicked, changed color to color of button
    public void SetColor(GameObject button)
    {
        brushColor = button.GetComponent<Image>().color;
    }

    public void ClearCanvas()
    {
        for (int x = 0; x < canvas.width; x++)
        {
            for (int y = 0; y < canvas.height; y++)
            {
                canvas.SetPixel(x, y, Color.white);
            }
        }
        canvas.Apply();

        SaveState();
    }

    //saves current canvas state to user paintings folder as a .png
    public void SaveImage()
    {
        string date = DateTime.Now.ToString();
        date = date.Replace("/", "."); // "/" and ":" can't be in file names
        date = date.Replace(":", ".");

        System.IO.File.WriteAllBytes(Application.dataPath + "\\User Paintings\\(" + date + ").png", canvas.EncodeToPNG());
    }

    //adds current canvas state to history
    public void SaveState()
    {
        Texture2D state = new Texture2D(canvas.width, canvas.height);
        for (int x = 0; x < state.width; x++)
        {
            for (int y = 0; y < state.height; y++)
            {
                state.SetPixel(x, y, canvas.GetPixel(x, y));
            }
        }
        state.Apply();
        
        //if the user undoed several states, then painted more, need to remove the alternate future
        if (historyIndex + 1 < history.Count)
        {
            history.RemoveRange(historyIndex + 1, history.Count - (historyIndex + 1));
        }

        history.Add(state);
        historyIndex++;
    }

    //loads previous texture from history
    public void Undo()
    {
        if (historyIndex > 0)
        {
            historyIndex--;
            LoadState(historyIndex);
        }
    }

    public void Redo()
    {
        if (historyIndex < history.Count - 1)
        {
            historyIndex++;
            LoadState(historyIndex);
        }
    }

    //loads texture from history to canvas
    private void LoadState(int index)
    {
        for (int x = 0; x < canvas.width; x++)
        {
            for (int y = 0; y < canvas.height; y++)
            {
                canvas.SetPixel(x, y, history[index].GetPixel(x, y));
            }
        }
        canvas.Apply();
    }

    //TEMP ONLY FOR 11/25 DEMO
    public void BackToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
