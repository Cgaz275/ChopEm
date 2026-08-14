using UnityEngine;

public class UIStateController : MonoBehaviour
{
    [Header("--- SCREENS ---")]
    [SerializeField] private GameObject homeScreen;
    [SerializeField] private GameObject gameplayHUD;
    [SerializeField] private GameObject howToPlayScreen;

    [Header("--- POPUPS ---")]
    [SerializeField] private GameObject pausePopup;
    [SerializeField] private GameObject gameOverPopup;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += ApplyState;
    }

    private void Start()
    {
        ApplyState(GameManager.Instance.CurrentState);
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= ApplyState;
    }

    private void ApplyState(GameState state)
    {
        SetActive(homeScreen, state == GameState.Home);
        SetActive(gameplayHUD, state == GameState.Gameplay || state == GameState.Pause || state == GameState.GameOver);
        SetActive(howToPlayScreen, state == GameState.HowToPlay);
        SetActive(pausePopup, state == GameState.Pause);
        SetActive(gameOverPopup, state == GameState.GameOver);
    }

    private static void SetActive(GameObject target, bool isActive)
    {
        if (target != null)
        {
            target.SetActive(isActive);
        }
    }
}
