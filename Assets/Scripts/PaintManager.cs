﻿using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class PaintManager : MonoBehaviour {

    public RawImage image;

    private Texture2D canvas;

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
        canvas.name = "testing";

        image.texture = canvas;

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

    public void SaveImage()
    {
        string date = DateTime.Now.ToString();
        date = date.Replace("/", "."); // "/" and ":" can't be in file names
        date = date.Replace(":", ".");

        System.IO.File.WriteAllBytes(Application.dataPath + "\\User Paintings\\(" + date + ").png", canvas.EncodeToPNG());
    }
}
