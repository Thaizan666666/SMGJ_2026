using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ManagerScene : MonoBehaviour
{
    public static ManagerScene Instance;

    private FadeSystem fadeSystem;

    // ───────────────────────────────────────────────
    //  Scene Names  (edit here, not scattered around)
    // ───────────────────────────────────────────────
    public static class Scenes
    {
        public const string MainMenu = "Prototype[03]";
        public const string Ending = "Ending";
    }

    // ───────────────────────────────────────────────
    //  UI Object Names inside the Ending scene
    // ───────────────────────────────────────────────
    public static class EndingUI
    {
        public const string Happy = "HappyEnding";
        public const string MomSad = "MonSad";
        public const string SunBurnt = "SunBurnt";
    }

    // ───────────────────────────────────────────────
    //  Animator trigger name on each EndingUI object
    //  Make sure all 3 animators use this same trigger
    // ───────────────────────────────────────────────
    private const string AnimTrigger = "Play";

    // ──────────────────────────────────────────────────────
    //  Shortcut methods  (call these from anywhere)
    // ──────────────────────────────────────────────────────
    public void LoadHappyEnding() => LoadScene(Scenes.Ending, EndingUI.Happy);
    public void LoadMomSadEnding() => LoadScene(Scenes.Ending, EndingUI.MomSad);
    public void LoadSunBurnEnding() => LoadScene(Scenes.Ending, EndingUI.SunBurnt);
    public void LoadMainMenu() => LoadScene(Scenes.MainMenu);
    public void LoadGame() => LoadScene(Scenes.MainMenu);

    // ──────────────────────────────────────────────────────
    //  Generic method — use this to go anywhere, any time
    //  e.g.  ManagerScene.Instance.LoadScene("MyScene", "MyUI");
    // ──────────────────────────────────────────────────────
    public void LoadScene(string sceneName, string targetUI = "")
    {
        if (isLoading) return;
        StartCoroutine(LoadSceneCoroutine(sceneName, targetUI));
    }

    // ──────────────────────────────────────────────────────
    //  Internals
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

        // 1. Fade out
        fadeSystem.FadeToBlack();
        yield return new WaitForSeconds(fadeSystem.fadeDuration);

        // 2. Load scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // 3. Wait 1 frame — ensures all Awake/Start in the new scene have run
        yield return null;

        // 4. Activate target UI and trigger animation
        if (!string.IsNullOrEmpty(targetUI))
        {
            Transform uiTarget = FindUIInLoadedScene(sceneName, targetUI);

            if (uiTarget != null)
            {
                uiTarget.gameObject.SetActive(true);

                // Wait 1 more frame so Animator wakes up after SetActive
                yield return null;

                Animator animator = uiTarget.GetComponent<Animator>();
                if (animator != null)
                    animator.SetTrigger(AnimTrigger);
                else
                    Debug.LogWarning($"[ManagerScene] No Animator on: '{targetUI}'");
            }
            else
            {
                Debug.LogWarning($"[ManagerScene] '{targetUI}' not found in scene '{sceneName}'");
            }
        }

        // 5. Fade back in
        fadeSystem.FadeFromBlack();
        yield return new WaitForSeconds(fadeSystem.fadeDuration);

        isLoading = false;
    }

    // Searches only Canvases that belong to the newly loaded scene,
    // skipping the DontDestroyOnLoad fade canvas.
    private Transform FindUIInLoadedScene(string sceneName, string targetUI)
    {
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);

        foreach (Canvas canvas in allCanvases)
        {
            // Skip any canvas that lives in DontDestroyOnLoad
            if (!canvas.gameObject.scene.IsValid()) continue;
            if (canvas.gameObject.scene != loadedScene) continue;

            Transform found = canvas.transform.Find(targetUI);
            if (found != null)
            {
                Debug.Log($"[ManagerScene] Found '{targetUI}' in canvas '{canvas.name}'");
                return found;
            }
        }

        return null;
    }
}