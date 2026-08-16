using TMPro;
using UnityEngine;

public class GameplayHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void OnEnable()
    {
        GameManager.OnScoreChanged += UpdateScore;
        GameManager.OnHighScoreChanged += UpdateHighScore;

        if (GameManager.Instance == null) return;

        UpdateScore(GameManager.Instance.CurrentScore);
        UpdateHighScore(GameManager.Instance.HighScore);
    }

    private void OnDisable()
    {
        GameManager.OnScoreChanged -= UpdateScore;
        GameManager.OnHighScoreChanged -= UpdateHighScore;
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    private void UpdateHighScore(int highScore)
    {
        if (highScoreText != null)
        {
            highScoreText.text = highScore.ToString();
        }
    }
}
