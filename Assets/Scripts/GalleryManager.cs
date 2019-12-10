﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{

    public float moveSpeed = 0.02f;

    public GameObject galleryDisplayPrefab;

    private List<Transform> paintings;
    private int displayIndex;

    public Transform displayLoc;
    public Transform stackLocStart;
    private List<Vector3> stackLocs;

    public Button nextButton;
    public Button prevButton;

    public Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        DirectoryInfo paintingFolder = new DirectoryInfo(Application.dataPath + "\\User Paintings");
        if (!paintingFolder.Exists)
        {
            paintingFolder.Create();
        }
        List<Texture2D> paintingTextures = new List<Texture2D>();
        foreach (FileInfo file in paintingFolder.GetFiles())
        {
            if (!(file.Name.Substring(file.Name.Length - 5) == ".meta"))
            {
                byte[] byteArray = File.ReadAllBytes(Application.dataPath + "\\User Paintings\\" + file.Name);
                Texture2D loaded = new Texture2D(2, 2);
                loaded.LoadImage(byteArray);
                paintingTextures.Add(loaded);
            }
        }

        paintings = new List<Transform>();
        stackLocs = getStackLocations(paintingTextures.Count);
        for (int i = 0; i < paintingTextures.Count; i++)
        {
            Transform displayObject = Instantiate(galleryDisplayPrefab).transform;
            displayObject.position = stackLocs[i];
            displayObject.rotation = stackLocStart.rotation;

            Vector2 aspectRatio = new Vector2(paintingTextures[i].width, paintingTextures[i].height);
            aspectRatio.Normalize();
            aspectRatio *= 5;
            displayObject.localScale = new Vector3(aspectRatio.x, aspectRatio.y, displayObject.localScale.z);

            RawImage display = displayObject.GetChild(1).GetChild(0).gameObject.GetComponent<RawImage>();
            display.texture = paintingTextures[i];

            paintings.Add(displayObject);
        }

        //displays first painting
        paintings[0].transform.position = displayLoc.position;
        paintings[0].transform.rotation = displayLoc.rotation;

        scoreText.text = "Score: " + GameManager.score;

        prevButton.interactable = false;
    }

    private List<Vector3> getStackLocations(int n)
    {
        List<Vector3> stackLocations = new List<Vector3>();
        for (int i = 0; i < n; i++)
        {
            Vector3 loc = stackLocStart.position + new Vector3(0, 0.25f * i, 0);
            stackLocations.Add(loc);
        }
        return stackLocations;
    }

    public void NextPainting()
    {
        if (displayIndex < paintings.Count - 1)
        {
            //moves currently displayed painting back to stack
            StartCoroutine(MovePainting(paintings[displayIndex], stackLocs[displayIndex], stackLocStart.rotation, moveSpeed));
            displayIndex++;

            //moves next painting from stack to display
            StartCoroutine(MovePainting(paintings[displayIndex], displayLoc.position, displayLoc.rotation, moveSpeed));
        }
    }

    public void PrevPainting()
    {
        if (displayIndex > 0)
        {
            //moves currently displayed painting back to stack
            StartCoroutine(MovePainting(paintings[displayIndex], stackLocs[displayIndex], stackLocStart.rotation, moveSpeed));
            displayIndex--;

            //moves next painting from stack to display
            StartCoroutine(MovePainting(paintings[displayIndex], displayLoc.position, displayLoc.rotation, moveSpeed));
        }
    }

    private IEnumerator MovePainting(Transform painting, Vector3 targetPos, Quaternion targetRot, float speed)
    {
        nextButton.interactable = false;
        prevButton.interactable = false;

        float progress = 0; //counts up to 1
        while (progress < 1)
        {
            progress += speed;

            //Moves the camera smoothly
            Vector3 smoothedPosition = Vector3.Lerp(painting.position, targetPos, progress);
            painting.position = smoothedPosition;

            //Rotates the camera smoothly
            Quaternion smoothedRotation = Quaternion.Lerp(painting.rotation, targetRot, progress);
            painting.rotation = smoothedRotation;

            if (Vector3.Distance(painting.position, targetPos) < 0.01f)  //stops the painting from asymptotically approaching position and taking too long
            {
                break;
            }

            yield return new WaitForSeconds(0.02f);
        }

        if (displayIndex < paintings.Count - 1)
        {
            nextButton.interactable = true;
        }
        if (displayIndex > 0)
        {
            prevButton.interactable = true;
        }
    }
}
