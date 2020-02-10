
[System.Serializable]
public class GameData
{
    public int score;

    public bool music;
    public float musicVol;

    public GameData(int score, bool music, float musicVol)
    {
        this.score = score;
        this.music = music;
        this.musicVol = musicVol;
    }
}
