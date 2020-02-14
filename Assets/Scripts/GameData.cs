using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int score;
    public List<int> levelScore;

    public List<PaintingInfo> paintingInfo;

    public bool music;
    public float musicVol;

    public GameData(int score, List<int> levelScore, List<PaintingInfo> paintingInfo, bool music, float musicVol)
    {
        this.score = score;
        this.levelScore = levelScore;
        this.paintingInfo = paintingInfo;
        this.music = music;
        this.musicVol = musicVol;
    }
}
