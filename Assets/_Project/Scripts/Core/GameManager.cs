using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("--- CONFIG ---")]
    [SerializeField] private TreeGameConfig config;
    public TreeGameConfig Config => config;

    // --- GAME DATA STATE ---
    public GameState CurrentState { get; private set; }
    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }
    public float CurrentTime { get; private set; }

    // --- SYSTEM EVENTS ---
    // Các UI Script & Audio chỉ cần đăng ký các Event này để tự cập nhật
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnHighScoreChanged;
    public static event Action<float, float> OnTimeChanged; // (Thời gian hiện tại, Thời gian tối đa)

    private const string HIGH_SCORE_KEY = "TimberGame_HighScore";

    private void Awake()
    {
        // Cấu hình Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load điểm kỷ lục đã lưu
        LoadHighScore();
    }

    private void Start()
    {
        // Bắt đầu game ở màn hình Home
        ChangeState(GameState.Home);
    }

    private void Update()
    {
        // Chỉ đếm ngược thời gian khi đang thực sự chơi
        if (CurrentState == GameState.Gameplay)
        {
            HandleTimer();
        }
    }

    #region --- STATE MANAGEMENT ---

    /// <summary>
    /// Chuyển đổi trạng thái Game và phát Event thông báo toàn bộ hệ thống
    /// </summary>
    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);

        // Quản lý Time.timeScale nếu tạm dừng
        Time.timeScale = (newState == GameState.Pause) ? 0f : 1f;
    }

    #endregion

    #region --- GAMEPLAY FLOW ---

    /// <summary>
    /// Bắt đầu một lượt chơi mới
    /// </summary>
    public void StartGame()
    {
        CurrentScore = 0;
        CurrentTime = config != null ? config.maxTime : 10f;

        OnScoreChanged?.Invoke(CurrentScore);
        OnTimeChanged?.Invoke(CurrentTime, config != null ? config.maxTime : 10f);

        ChangeState(GameState.Gameplay);
    }

    /// <summary>
    /// Xử lý đếm ngược thời gian trong màn chơi
    /// </summary>
    private void HandleTimer()
    {
        CurrentTime -= Time.deltaTime;
        float maxTime = config != null ? config.maxTime : 10f;

        OnTimeChanged?.Invoke(CurrentTime, maxTime);

        if (CurrentTime <= 0f)
        {
            CurrentTime = 0f;
            TriggerGameOver();
        }
    }

    /// <summary>
    /// Cộng điểm và thưởng thêm thời gian mỗi khi chặt gỗ thành công
    /// </summary>
    public void AddScore()
    {
        if (CurrentState != GameState.Gameplay) return;

        int scoreToAdd = config != null ? config.scorePerChop : 1;
        float timeBonus = config != null ? config.timeBonusPerChop : 0.25f;
        float maxTime = config != null ? config.maxTime : 10f;

        // 1. Cộng điểm
        CurrentScore += scoreToAdd;
        OnScoreChanged?.Invoke(CurrentScore);

        // 2. Thưởng thời gian (Không vượt quá maxTime)
        CurrentTime = Mathf.Clamp(CurrentTime + timeBonus, 0f, maxTime);
        OnTimeChanged?.Invoke(CurrentTime, maxTime);

        // 3. Kiểm tra phá kỷ lục (High Score)
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            SaveHighScore();
            OnHighScoreChanged?.Invoke(HighScore);
        }

        // 4. Phát âm thanh chặt gỗ
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(SoundType.Chop);
        }
    }

    /// <summary>
    /// Xử lý khi người chơi thua (Hết giờ hoặc Chặt trúng cành)
    /// </summary>
    public void TriggerGameOver()
    {
        if (CurrentState == GameState.GameOver) return;

        ChangeState(GameState.GameOver);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(SoundType.Lose);
        }
    }

    #endregion

    #region --- BUTTON ACTIONS FOR UI ---

    public void PauseGame() => ChangeState(GameState.Pause);
    public void ResumeGame() => ChangeState(GameState.Gameplay);
    public void ShowHowToPlay() => ChangeState(GameState.HowToPlay);
    public void ReturnToHome() => ChangeState(GameState.Home);
    public void RestartGame() => StartGame();

    #endregion

    #region --- DATA PERSISTENCE ---

    private void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        OnHighScoreChanged?.Invoke(HighScore);
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, HighScore);
        PlayerPrefs.Save();
    }

    #endregion
}
