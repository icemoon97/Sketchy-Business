using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int score;
    public List<int> levelScore;

    public List<PaintingInfo> paintings;

    public bool music;
    public float musicVol;
    public bool soundEffects;
    public float soundEffectVol;

    public GameData(int score, List<int> levelScore, List<PaintingInfo> paintings, bool music, float musicVol, bool soundEffects, float soundEffectVol)
    {
        this.score = score;
        this.levelScore = levelScore;
        this.paintings = paintings;
        this.music = music;
        this.musicVol = musicVol;
        this.soundEffects = soundEffects;
        this.soundEffectVol = soundEffectVol;
    }
}
