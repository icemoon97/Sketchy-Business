using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{

    public GameObject galleryDisplayPrefab;

    // Start is called before the first frame update
    void Start()
    {
        DirectoryInfo paintingFolder = new DirectoryInfo(Application.dataPath + "\\User Paintings");
        List<Texture2D> paintings = new List<Texture2D>();
        foreach (FileInfo file in paintingFolder.GetFiles())
        {
            if (!(file.Name.Substring(file.Name.Length - 5) == ".meta"))
            {
                byte[] byteArray = File.ReadAllBytes(Application.dataPath + "\\User Paintings\\" + file.Name);
                Texture2D loaded = new Texture2D(2, 2);
                loaded.LoadImage(byteArray);
                paintings.Add(loaded);
            }
        }

        for (int i = 0; i < paintings.Count; i++)
        {
            GameObject displayObject = Instantiate(galleryDisplayPrefab, new Vector3(i * 6, 0, 0), Quaternion.identity);
            RawImage display = displayObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<RawImage>();
            display.texture = paintings[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
