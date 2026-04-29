using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;

    [Header("UI")]
    public ScoreUI scoreUI; // referência da UI

    void Awake()
    {
        Instance = this;
    }

    public void AddScore(int amount)
    {
        score += amount;

        Debug.Log("Score: " + score);

        // chama o fade da UI
        scoreUI?.ShowScore(score);
    }
}