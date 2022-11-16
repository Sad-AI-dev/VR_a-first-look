using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreLabel : MonoBehaviour
{
    public NumLabel label;

    private void Start()
    {
        StartCoroutine(RefreshCo());
    }

    public void Refresh()
    {
        label.SetLabelNum(ScoreSerializeHandler.GetHighScore());
    }

    private IEnumerator RefreshCo()
    {
        yield return null;
        Refresh();
    }
}
