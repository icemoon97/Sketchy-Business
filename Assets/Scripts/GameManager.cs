﻿using System.Collections.Generic;

public static class GameManager
{
    //needed for scene switching
    public static LevelInfo levelToLoad;
    public static int currentLevelIndex;

    //score
    public static int score;
    public static List<int> levelScore; //score on each level

    public static List<PaintingInfo> paintings;

    //sound settings
    public static bool music;
    public static float musicVol;
    public static bool soundEffects;
    public static float soundEffectVol;
}
