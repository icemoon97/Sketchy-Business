﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintManager : MonoBehaviour {

    [Header("Unity Setup")]
    public GameObject paintingPanel;
    public GameObject evalPanel;
    public EvaluationManager evalManager;

    public Image referencePainting; //image user is trying to replicate

    public RawImage image; //displays canvas

    private Texture2D canvas;
    private List<Texture2D> history; //list of all previous states of the canvas
    private int historyIndex; //current place in history (tracks user if they undo/redo multiple times)

    public Transform colorPanel;
    public GameObject colorButtonPrefab;

    private LevelTimer timer;

    public Image selectionHighlightPaintTools;
    public Image selectionHighlightColor;

    public Slider brushSizeSlider;

    [Header("Paint Settings")]
    public BrushStyle brushStyle;
    public int brushSize;
    public Color brushColor;

    public int sprayCanDensity;

    private Vector3 prevMousePosition;

    [Header("Challenge Variables")]
    public float shakeFactor; //how much canvas can move per frame during shaking
    private Vector3 imageBasePos; //needed for shaking
    private Texture2D fadingCanvas;
    public float fadeSpeed; //how fast colors fade
    

    [Header("Default Level (for testing)")]
    public LevelInfo defaultLevel;

    public enum BrushStyle
    {
        SquareBrush,
        CircularBrush,
        SprayCan,
        PaintBucket
    }

    void Awake()
    {
        timer = GetComponent<LevelTimer>();
    }

	// Use this for initialization
	void Start ()
    {
        if (GameManager.levelToLoad == null)
        {
            GameManager.levelToLoad = defaultLevel;    
        }

        LoadLevel(GameManager.levelToLoad);

        paintingPanel.SetActive(true);
        evalPanel.SetActive(false);

        canvas = new Texture2D(referencePainting.mainTexture.width, referencePainting.mainTexture.height);

        image.texture = canvas;

        imageBasePos = image.transform.position;

        history = new List<Texture2D>();
        ClearCanvas(canvas);
        historyIndex = 0;

        //makes brush size actually update when slider is changed
        brushSizeSlider.onValueChanged.AddListener(delegate {
            brushSize = (int)brushSizeSlider.value;
        });

        prevMousePosition = getAdjustedMousePos();

        if (GameManager.levelToLoad.activeChallenges.disappearingInk)
        {
            fadingCanvas = new Texture2D(referencePainting.mainTexture.width, referencePainting.mainTexture.height);
            ClearCanvas(fadingCanvas);
            image.texture = fadingCanvas; 
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 mousePos = getAdjustedMousePos();

        //detecting drawing
        if (Input.GetMouseButton(0))
        {
            //draws in all positions between old and new mouse pos
            for (float i = 0; i <= 1; i += 2 / Vector3.Distance(mousePos, prevMousePosition))
            {
                Draw(Vector3.Lerp(mousePos, prevMousePosition, i), canvas);

                if (GameManager.levelToLoad.activeChallenges.disappearingInk)
                {
                    Draw(Vector3.Lerp(mousePos, prevMousePosition, i), fadingCanvas); //so users drawings so up temporarily
                }
            }

            canvas.Apply();
        }

        //key inputs for undo/redo
        if (Input.GetKeyDown(KeyCode.Z) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            Undo();
        }
        if (Input.GetKeyDown(KeyCode.Y) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            Redo();
        }

        //handling challenges
        if (GameManager.levelToLoad.activeChallenges.shaking)
        {
            Vector2 maxDistance = new Vector2(10, 10); //max distance canvas can move from base position in x,y

            Vector3 pos = image.rectTransform.position;
            Vector3 newPos = new Vector3(pos.x + Random.Range(-shakeFactor, shakeFactor), pos.y + Random.Range(-shakeFactor, shakeFactor), pos.z);
            image.rectTransform.position = new Vector3(
                Mathf.Max(Mathf.Min(imageBasePos.x + maxDistance.x, newPos.x), imageBasePos.x - maxDistance.x),
                Mathf.Max(Mathf.Min(imageBasePos.y + maxDistance.y, newPos.y), imageBasePos.y - maxDistance.y),
                pos.y);
        }
        if (GameManager.levelToLoad.activeChallenges.disappearingInk)
        {
            /*              very cool glitch
            Color[] arr = new Color[canvas.height * canvas.width];
            for (int i = 0; i < arr.Length; i++)
            {
                Color c = fadingCanvas.GetPixel(i / canvas.height, i % canvas.width);
                c = new Color(c.r + 0.01f, c.g + 0.01f, c.b + 0.01f);
                arr[i] = c;
            }
            fadingCanvas.SetPixels(arr, 0);
            */

            Color[] colors = fadingCanvas.GetPixels();
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(colors[i].r + fadeSpeed, colors[i].g + fadeSpeed, colors[i].b + fadeSpeed);
            }
            fadingCanvas.SetPixels(colors, 0);

            fadingCanvas.Apply(false);
        }

        //sumbits painting if timer has expired
        if (timer.time <= 0 && timer.enabled)
        {
            SubmitPainting();
            timer.enabled = false;
        }

        prevMousePosition = mousePos;
    }

    private void LoadLevel(LevelInfo level)
    {
        referencePainting.sprite = level.referencePainting;

        int maxWidth = 300; //max size that canvas can be
        int maxHeight = 410;

        Vector2 aspectRatio = new Vector2(referencePainting.sprite.texture.width, referencePainting.sprite.texture.height);
        aspectRatio.Normalize();
        aspectRatio *= 1 / Mathf.Min(aspectRatio.x, aspectRatio.y);
        if (aspectRatio.y / aspectRatio.x > maxHeight * maxWidth)
        {
            aspectRatio *= maxHeight;
        }
        else
        {
            aspectRatio *= maxWidth;
        }
        image.rectTransform.sizeDelta = aspectRatio;
        referencePainting.rectTransform.sizeDelta = aspectRatio;

        //creating all the color buttons
        foreach (Color c in level.colorPalette)
        {
            GameObject colorButton = Instantiate(colorButtonPrefab, colorPanel);
            colorButton.GetComponent<Image>().color = c;
            colorButton.GetComponent<Button>().onClick.AddListener( delegate { SetColor(c); } );
            colorButton.GetComponent<Button>().onClick.AddListener( delegate { moveSelectionHighlightColor(colorButton); } );
        }
        
        if (level.timeLimit > 0)
        {
            timer.time = level.timeLimit;
        }
        else
        {
            timer.enabled = false;
            timer.timerDisplay.gameObject.SetActive(false);
        }
    }

    //draws at given position with current color, size and brush style
    private void Draw(Vector3 pos, Texture2D canvas)
    {
        Rect canvasBounds = new Rect(0, 0, canvas.width, canvas.height);
        if (brushStyle == BrushStyle.SquareBrush)
        {
            for (float x = -brushSize / 2f; x < brushSize / 2f; x++)
            {
                for (float y = -brushSize / 2f; y < brushSize / 2f; y++)
                {
                    Vector2Int pixel = new Vector2Int((int)(pos.x + x), (int)(pos.y + y));
                    if (canvasBounds.Contains(pixel)) //makes sure pixels are within bounds
                    {
                        canvas.SetPixel(pixel.x, pixel.y, brushColor);
                    }
                }
            }
        }
        else if (brushStyle == BrushStyle.CircularBrush)
        {
            for (float x = -brushSize / 2f; x < brushSize / 2f; x++)
            {
                for (float y = -brushSize / 2f; y < brushSize / 2f; y++)
                {
                    Vector2Int pixel = new Vector2Int((int)(pos.x + x), (int)(pos.y + y));
                    if (canvasBounds.Contains(pixel) && Vector2.Distance(pos, pixel) < brushSize / 2) //makes sure pixels are within bounds
                    {
                        canvas.SetPixel(pixel.x, pixel.y, brushColor);
                    }
                }
            }
        }
        else if (brushStyle == BrushStyle.SprayCan)
        {
            for (float i  = 0; i < sprayCanDensity * (brushSize / 10f); i++)
            {
                Vector2 rand = Random.insideUnitCircle * (brushSize / 2f);
                Vector2Int pixel = new Vector2Int(Mathf.RoundToInt(pos.x + rand.x), Mathf.RoundToInt(pos.y + rand.y));
                if (canvasBounds.Contains(pixel))
                {
                    canvas.SetPixel(pixel.x, pixel.y, brushColor);
                }
            }
        }
    }

    //paint bucket function, which is called from a click event from the canvas. 
    //It cannot work like the other brushstyles, which use linear interpolation and are called many many times per mouse drag, 
    //which would be super intensive and produce weird behavior with the paint bucket
    public void PaintBucket()
    {
        if (brushStyle != BrushStyle.PaintBucket)
        {
            return;
        }

        Vector3 mousePos = getAdjustedMousePos();
        Color targetColor = canvas.GetPixel((int)mousePos.x, (int)mousePos.y);
        if (targetColor == brushColor)
        {
            return;
        }

        Vector2Int startNode = new Vector2Int((int)mousePos.x, (int)mousePos.y);
        canvas.SetPixel(startNode.x, startNode.y, brushColor);
        List<Vector2Int> queue = new List<Vector2Int>();
        queue.Add(startNode);

        Vector2Int[] vectors = new Vector2Int[] {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        Rect canvasBounds = new Rect(0, 0, canvas.width, canvas.height);

        while (queue.Count > 0)
        {
            Vector2Int node = queue[0];
            queue.RemoveAt(0);

            foreach (Vector2Int vector in vectors)
            {
                Vector2Int pixel = new Vector2Int(node.x + vector.x, node.y + vector.y);
                if (canvasBounds.Contains(pixel) && canvas.GetPixel(pixel.x, pixel.y) == targetColor)
                {
                    canvas.SetPixel(pixel.x, pixel.y, brushColor);
                    queue.Add(pixel);
                }
            }
        }

        SaveState();
    }

    //returns relative mouse position within canvas texture
    private Vector3 getAdjustedMousePos()
    {
        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        image.rectTransform.GetWorldCorners(corners);

        Vector2 texSize = new Vector2(canvas.width, canvas.height);
        Vector2 imageSize = new Vector2(image.rectTransform.rect.width, image.rectTransform.rect.height);
        Vector2 ratio = texSize / imageSize;

        Vector3 final = (mousePos - corners[0]) * ratio;

        return new Vector3((int)final.x, (int)final.y, 0);
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

    //moves selection highlight to the selected button
    //probably doesn't have to be 3 different methods but this seemed easiest
    public void moveSelectionHighlightPaintTools(GameObject justSelected)
    {
        selectionHighlightPaintTools.rectTransform.anchoredPosition = justSelected.GetComponent<RectTransform>().anchoredPosition;
    }

    public void moveSelectionHighlightColor(GameObject justSelected)
    {
        selectionHighlightColor.rectTransform.anchoredPosition = justSelected.GetComponent<RectTransform>().anchoredPosition;
    }

    //turns entire canvas to white
    private void ClearCanvas(Texture2D canvas)
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

    //method that buttons can actually call
    public void ClearCanvasExternal()
    {
        ClearCanvas(canvas);
        if (GameManager.levelToLoad.activeChallenges.disappearingInk)
        {
            ClearCanvas(fadingCanvas);
        }
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

    //switches to evaluation panel and starts evaluation process
    public void SubmitPainting()
    {
        SoundManager sound = GameObject.Find("Sound Manager").GetComponent<SoundManager>();
        sound.PlaySound();

        evalPanel.SetActive(true);
        paintingPanel.SetActive(false);

        evalManager.Evaluate(canvas);
    }
}
