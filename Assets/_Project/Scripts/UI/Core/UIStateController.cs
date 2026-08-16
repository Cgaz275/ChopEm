using UnityEngine;

public class UIStateController : MonoBehaviour
{
    [Header("--- SCREENS ---")]
    [SerializeField] private GameObject homeScreen;
    [SerializeField] private GameObject gameWorld;
    [SerializeField] private GameObject gameplayHUD;

    [Header("--- GAMEPLAY ---")]
    [SerializeField] private TreeController treeController;
    [SerializeField] private GameObject howToPlayScreen;

    [Header("--- POPUPS ---")]
    [SerializeField] private GameObject pausePopup;
    [SerializeField] private GameObject gameOverPopup;
    [SerializeField] private GameObject settingsPopup;

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
        bool isGameplayState = state == GameState.Gameplay || state == GameState.Pause || state == GameState.GameOver;

        SetActive(homeScreen, state == GameState.Home);
        SetActive(gameWorld, isGameplayState);
        SetActive(gameplayHUD, isGameplayState);
        SetActive(howToPlayScreen, state == GameState.HowToPlay);
        SetActive(pausePopup, state == GameState.Pause);
        SetActive(gameOverPopup, state == GameState.GameOver);
        SetActive(settingsPopup, state == GameState.Settings);

        if (state == GameState.Gameplay && treeController != null)
        {
            treeController.RefreshChopHighlight();
        }
    }

    private static void SetActive(GameObject target, bool isActive)
    {
        if (target != null)
        {
            target.SetActive(isActive);
        }
    }
}
