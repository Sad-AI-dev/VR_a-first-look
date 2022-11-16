using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSerializeHandler : MonoBehaviour
{
    public static void SetHighScore(float score)
    {
        if (score > GetHighScore()) {
            PlayerPrefs.SetFloat("HighScore", score);
        }
    }

    public static float GetHighScore()
    {
        return PlayerPrefs.GetFloat("HighScore");
    }
}
