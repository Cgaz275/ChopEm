using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Image timeFill;

    private void OnEnable()
    {
        GameManager.OnScoreChanged += UpdateScore;
        GameManager.OnHighScoreChanged += UpdateHighScore;
        GameManager.OnTimeChanged += UpdateTime;

        if (GameManager.Instance == null) return;

        UpdateScore(GameManager.Instance.CurrentScore);
        UpdateHighScore(GameManager.Instance.HighScore);
        UpdateTime(GameManager.Instance.CurrentTime, GameManager.Instance.Config.maxTime);
    }

    private void OnDisable()
    {
        GameManager.OnScoreChanged -= UpdateScore;
        GameManager.OnHighScoreChanged -= UpdateHighScore;
        GameManager.OnTimeChanged -= UpdateTime;
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

    private void UpdateTime(float currentTime, float maxTime)
    {
        if (timeFill != null)
        {
            timeFill.fillAmount = Mathf.Clamp01(currentTime / maxTime);
        }
    }
}
