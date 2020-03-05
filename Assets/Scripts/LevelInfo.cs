using System;
using UnityEngine;

[Serializable]
public class LevelInfo
{
    public Sprite referencePainting;
    public Color[] colorPalette;
    public float timeLimit;
    public int scoreCutoff;

    public string paintingName;
    public string funFact;
}
