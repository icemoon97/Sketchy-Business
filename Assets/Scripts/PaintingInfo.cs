
[System.Serializable]
public class PaintingInfo
{
    public int score;
    public bool caught; //if the painting was recognized as a forgery (if you failed the level)
    public string paintingName;
    public string fileName; 
    public string datePainted;
    public string funFact; //facts about the real-life painting
}
