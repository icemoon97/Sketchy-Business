using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PaintManager : MonoBehaviour {

    public RawImage image;

    private Texture2D canvas;
    private List<Texture2D> history; //list of all previous states of the canvas
    private int historyIndex; //current place in history (tracks user if they undo/redo multiple times)

    public BrushStyle brushStyle;
    public int brushSize;
    public Color brushColor;

    private Vector3 prevMousePosition;

    public enum BrushStyle
    {
        SquareBrush,
        CircularBrush
    }

	// Use this for initialization
	void Start ()
    {
        canvas = new Texture2D((int)image.rectTransform.rect.width, (int)image.rectTransform.rect.height);
        ClearCanvas();

        image.texture = canvas;

        history = new List<Texture2D>();
        SaveState();
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
        for (int x = -brushSize / 2; x < brushSize / 2; x++)
        {
            for (int y = -brushSize / 2; y < brushSize / 2; y++)
            {
                Vector2 pixel = new Vector2(pos.x + x, pos.y + y);
                if (pixel.x >= 0 && pixel.x < canvas.width && pixel.y >= 0 && pixel.y < canvas.height) //makes sure pixels are within bounds
                {
                    if (brushStyle == BrushStyle.SquareBrush 
                        || (brushStyle == BrushStyle.CircularBrush && Vector2.Distance(pos, pixel) < brushSize / 2))
                    {
                        canvas.SetPixel(Mathf.RoundToInt(pos.x) + x, Mathf.RoundToInt(pos.y) + y, brushColor);
                    }
                }
            }
        }
        canvas.Apply();
    }

    //returns relative mouse position within canvas texture
    private Vector3 getAdjustedMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 adjustedMousePos = new Vector3(mousePos.x - Screen.width / 2, mousePos.y - Screen.height / 2, 0);

        Vector3 canvasPos = image.rectTransform.localPosition;

        return adjustedMousePos - canvasPos;
    }

    public void SetBrushSize(int size)
    {
        brushSize = size;
    }

    public void SetColor(Color color)
    {
        brushColor = color;
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
    private void SaveState()
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

    public void OnMouseDragEnd()
    {
        SaveState();
        Debug.Log(history.Count);
    }
}
