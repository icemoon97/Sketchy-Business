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

    //improves readability and makes editting easy in inspector
    [Serializable]
    public struct Challenges
    {
        public bool shaking;
        public bool disappearingInk;
    }
    public Challenges activeChallenges;
}
