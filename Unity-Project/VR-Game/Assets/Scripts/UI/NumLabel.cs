using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class NumLabel : MonoBehaviour
{
    private TMP_Text text;
    private string startString;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        startString = text.text;
    }

    public void SetScore()
    {
        if (ScoreManager.instance != null) {
            SetLabelNum(ScoreManager.instance.score);
        }
    }

    public void SetLabelNum(float num)
    {
        text.text = startString + Mathf.RoundToInt(num).ToString();
    }
}
