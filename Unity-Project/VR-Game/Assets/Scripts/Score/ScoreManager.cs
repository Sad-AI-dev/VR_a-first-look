using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    private void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }
    }

    public static ScoreManager instance;

    [SerializeField] private UnityEvent<float> onScoreChange;
    public float score;

    private void Start()
    {
        score = 0.0f;
    }

    public void AddScore(float toAdd)
    {
        score += toAdd;
        onScoreChange?.Invoke(score);
    }
}
