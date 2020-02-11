using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int score;
    public List<int> levelScore;

    public bool music;
    public float musicVol;

    public GameData(int score, List<int> levelScore, bool music, float musicVol)
    {
        this.score = score;
        this.levelScore = levelScore;
        this.music = music;
        this.musicVol = musicVol;
    }
}
