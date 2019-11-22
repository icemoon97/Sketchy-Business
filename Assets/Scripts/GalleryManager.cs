using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{

    public GameObject galleryDisplayPrefab;

    private List<GameObject> paintings;
    private int displayIndex;

    public Transform displayLoc;
    public Transform stackLoc;

    public Button nextButton;
    public Button prevButton;


    // Start is called before the first frame update
    void Start()
    {
        DirectoryInfo paintingFolder = new DirectoryInfo(Application.dataPath + "\\User Paintings");
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

        paintings = new List<GameObject>();
        for (int i = 0; i < paintingTextures.Count; i++)
        {
            GameObject displayObject = Instantiate(galleryDisplayPrefab);
            displayObject.transform.position = new Vector3(stackLoc.position.x, stackLoc.position.y + (0.25f * i), stackLoc.position.z);
            displayObject.transform.rotation = stackLoc.rotation;

            RawImage display = displayObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<RawImage>();
            display.texture = paintingTextures[i];

            paintings.Add(displayObject);
        }

        //displays first painting
        paintings[0].transform.position = displayLoc.position;
        paintings[0].transform.rotation = displayLoc.rotation;
    }

    public void NextPainting()
    {
        if (displayIndex < paintings.Count - 1)
        {
            //moves currently displayed painting back to stack
            StartCoroutine(MovePainting(paintings[displayIndex].transform, stackLoc.position + new Vector3(0, 0.25f * displayIndex, 0), stackLoc.rotation, 0.02f));
            displayIndex++;

            //moves next painting from stack to display
            StartCoroutine(MovePainting(paintings[displayIndex].transform, displayLoc.position, displayLoc.rotation, 0.02f));
        }
    }

    public void PrevPainting()
    {
        if (displayIndex > 0)
        {
            //moves currently displayed painting back to stack
            StartCoroutine(MovePainting(paintings[displayIndex].transform, stackLoc.position + new Vector3(0, 0.25f * displayIndex, 0), stackLoc.rotation, 0.02f));
            displayIndex--;

            //moves next painting from stack to display
            StartCoroutine(MovePainting(paintings[displayIndex].transform, displayLoc.position, displayLoc.rotation, 0.02f));
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

        nextButton.interactable = true;
        prevButton.interactable = true;
    }
}
