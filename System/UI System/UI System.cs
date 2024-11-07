using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    [Header("Interaction UI Elements")]
    [SerializeField] private Canvas _prompt;

    [SerializeField] private TextMeshProUGUI _promptText;

    [Header("Loading UI")]
    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private Slider progressBar;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _loadingMap;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private List<Sprite> _loadingMaps;

    [Header("Pause")]
    [SerializeField] private Canvas _pauseCanvas;

    [SerializeField] private Canvas _settingsCanvas;
    [SerializeField] private Canvas _controlCanvas;
    [SerializeField] private Canvas _musicCanvas;

    [Header("Debugging")]
    [SerializeField] internal TextMeshProUGUI _deathCountText;

    private ServiceLocator _serviceLocator;
    private InputManager inputManager;
    private SceneManagement sceneManagement;

    private Coroutine animateLoadingTextCoroutine;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);

        inputManager = _serviceLocator.GetService<InputManager>();
        sceneManagement = _serviceLocator.GetService<SceneManagement>();
    }

    private void OnEnable()
    {
        if (inputManager == null)
            inputManager = _serviceLocator.GetService<InputManager>();

        inputManager.PauseActions.MenuOpenClose.started += _ => TogglePauseMenu();
        sceneManagement.OnLevelLoading += HandleLevelLoading;
    }

    private void OnDisable()
    {
        if (inputManager == null)
            inputManager = _serviceLocator.GetService<InputManager>();

        if (sceneManagement == null)
            sceneManagement = _serviceLocator.GetService<SceneManagement>();

        inputManager.PauseActions.MenuOpenClose.started -= _ => TogglePauseMenu();
        sceneManagement.OnLevelLoading -= HandleLevelLoading;
    }

    private void OnDestroy()
    {
        if (inputManager == null)
            inputManager = _serviceLocator.GetService<InputManager>();

        if (sceneManagement == null)
            sceneManagement = _serviceLocator.GetService<SceneManagement>();

        inputManager.PauseActions.MenuOpenClose.started -= _ => TogglePauseMenu();
        sceneManagement.OnLevelLoading -= HandleLevelLoading;
    }

    #region Interaction UI

    internal void UpdatePromptText(string _promptMessage)
    {
        _prompt.enabled = true;
        _promptText.text = _promptMessage;
    }

    internal void DisablePromptText() => _prompt.enabled = false;

    #endregion Interaction UI

    #region Loading UI

    private void HandleLevelLoading(bool isLoading, float targetProgress, float duration)
    {
        if (isLoading)
        {
            // Correctly determine the current level index
            int currentLevelIndex = sceneManagement.CurrentLevel;

            // Update the _loadingMap sprite based on the current level index
            // Ensure the index is within the bounds of the _loadingMaps list
            if (currentLevelIndex > 0 && currentLevelIndex <= _loadingMaps.Count)
            {
                if (_loadingMap != null) // Check if _loadingMap is not null
                {
                    _loadingMap.sprite = _loadingMaps[currentLevelIndex - 1];
                    // If you have any DOTween animations on _loadingMap, restart them here
                }
            }

            // Enable the loading screen
            loadingScreen.SetActive(true);
            progressBar.value = 0; // Reset the progress bar

            inputManager.DisableAllInputsExceptPause();

            // Ensure any previous loading text animation is stopped before starting a new one
            StopLoadingAnimation();

            StartCoroutine(SimulateProgressCoroutine(targetProgress, duration));
        }
        else
        {
            StopLoadingAnimation(); // Stop the loading text animation
            StartCoroutine(FadeOutLoadingScreen()); // Start fading out the loading screen
        }
    }

    private void StartLoadingAnimation() => animateLoadingTextCoroutine = StartCoroutine(AnimateLoadingText());

    private void StopLoadingAnimation()
    {
        if (animateLoadingTextCoroutine != null)
        {
            StopCoroutine(animateLoadingTextCoroutine);
            animateLoadingTextCoroutine = null; // Clear the reference
        }
    }

    private IEnumerator AnimateLoadingText()
    {
        string baseText = "Loading";
        int dotCount = 0;

        while (true) // Infinite loop to keep the animation running
        {
            _loadingText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; // Cycle dotCount between 0 and 3
            yield return new WaitForSeconds(0.5f); // Wait for half a second before updating the text again
        }
    }

    private IEnumerator FadeOutLoadingScreen()
    {
        float delayBeforeFadeStarts = 2.0f; // Delay in seconds before fade starts

        yield return new WaitForSeconds(delayBeforeFadeStarts); // Wait for the specified delay

        float duration = 1.0f; // Duration in seconds for the fade-out effect
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1.0f - (elapsedTime / duration));
            _canvasGroup.alpha = alpha;

            yield return null;
        }

        // Ensure the coroutine is stopped when loading screen is about to be hidden
        StopLoadingAnimation();

        loadingScreen.SetActive(false);
        _canvasGroup.alpha = 1.0f;
        inputManager.EnableAllInputs();
    }

    private IEnumerator SimulateProgressCoroutine(float targetProgress, float duration)
    {
        float startTime = Time.time;
        float startProgress = progressBar.value;
        float endTime = startTime + duration;
        bool loadingComplete = false;

        // Start the loading animation in parallel
        StartLoadingAnimation();

        while (Time.time < endTime || !loadingComplete)
        {
            float elapsed = Time.time - startTime;
            progressBar.value = Mathf.Lerp(startProgress, targetProgress, elapsed / duration);

            // Check if the progress bar has reached the target progress
            if (progressBar.value >= targetProgress)
            {
                loadingComplete = true;
            }

            yield return null;
        }

        progressBar.value = targetProgress;

        // Ensure the loading animation coroutine is stopped once loading is complete
        StopLoadingAnimation();
    }

    #endregion Loading UI

    #region Pause

    internal void TogglePauseMenu()
    {
        if (ServiceLocator.Instance.GetService<SceneManagement>().CurrentLevel == 1)
            return; // Do nothing if it's the main menu

        _pauseCanvas.enabled = !_pauseCanvas.enabled;

        if (inputManager != null)
        {
            if (_pauseCanvas.enabled)
            {
                // Pause menu is now active, disable all inputs except for PauseActions
                inputManager.DisableAllInputsExceptPause();
            }
            else
            {
                // Pause menu is now inactive, re-enable all inputs
                inputManager.EnableAllInputs();
            }
        }
    }

    internal void StartLevel(int level = -1)
    {
        _pauseCanvas.enabled = false;
        _controlCanvas.enabled = false;
        _settingsCanvas.enabled = false;
        _musicCanvas.enabled = false;

        ServiceLocator.Instance.GetService<SceneManagement>().StartLevel(level);
    }

    public void QuitGame() => Application.Quit();

    #endregion Pause
}