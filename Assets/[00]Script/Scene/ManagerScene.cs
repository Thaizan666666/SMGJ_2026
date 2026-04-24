using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Security.Cryptography;

public class ManagerScene : MonoBehaviour
{
    public static ManagerScene Instance;

    private FadeSystem fadeSystem;

    // ───────────────────────────────────────────────
    //  Scene Names
    // ───────────────────────────────────────────────
    public static class Scenes
    {
        public const string MainMenu = "MainMenu";
        public const string Ending = "Ending";
        public const string InGame = "InGame";
    }

    // ───────────────────────────────────────────────
    //  UI Object Names inside the Ending scene
    // ───────────────────────────────────────────────
    public static class EndingUI
    {
        public const string Intro = "Intro";
        public const string Happy = "HappyEnding";
        public const string MomSad = "MonSad";
        public const string SunBurnt = "SunBurnt";
    }

    // ───────────────────────────────────────────────
    //  Shared state — EndingUIController reads this
    // ───────────────────────────────────────────────
    public static string PendingTargetUI = "";

    // ──────────────────────────────────────────────────────
    //  Shortcut methods
    // ──────────────────────────────────────────────────────
    public void LoadHappyEnding() => LoadScene(Scenes.Ending, EndingUI.Happy);
    public void LoadMomSadEnding() => LoadScene(Scenes.Ending, EndingUI.MomSad);
    public void LoadSunBurnEnding() => LoadScene(Scenes.Ending, EndingUI.SunBurnt);
    public void LoadIntro() => LoadScene(Scenes.Ending, EndingUI.Intro);
    public void LoadMainMenu() => LoadScene(Scenes.MainMenu);
    public void LoadGame() => LoadScene(Scenes.InGame);

    public void LoadScene(string sceneName, string targetUI = "")
    {
        if (isLoading) return;
        StartCoroutine(LoadSceneCoroutine(sceneName, targetUI));
    }

    // ──────────────────────────────────────────────────────
    private bool isLoading = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        fadeSystem = GetComponent<FadeSystem>();
        fadeSystem.FadeFromBlack();
    }

    IEnumerator LoadSceneCoroutine(string sceneName, string targetUI)
    {
        isLoading = true;

        // 1. Store which UI to open — EndingUIController will read this on Start()
        PendingTargetUI = targetUI;

        // 2. Fade out
        fadeSystem.FadeToBlack();
        yield return new WaitForSeconds(fadeSystem.fadeDuration);

        // 3. Load scene — EndingUIController.Start() runs here automatically
        yield return SceneManager.LoadSceneAsync(sceneName);

        // 4. Fade back in
        fadeSystem.FadeFromBlack();
        yield return new WaitForSeconds(fadeSystem.fadeDuration);

        isLoading = false;
    }
}