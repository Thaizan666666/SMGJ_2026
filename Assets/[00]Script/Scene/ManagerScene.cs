using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class ManagerScene : MonoBehaviour
{
    public static ManagerScene Instance;

    private FadeSystem fadeSystem;
    private string sceneName;
    private string targetUI;

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
    }
    public void LoadSceneMomHappyEnding()
    {
        sceneName = "Ending";                //ได้ไอตืม
        targetUI = "HappyEnding";
        Debug.Log("---------------------------------------------------------------LoadSceneMomHappyEnding");
        StartCoroutine(LoadSceneCoroutine());
    }
    public void LoadSceneMomSadEnding()
    {
        sceneName = "Ending";                //อดแดก
        targetUI = "MonSad";
        Debug.Log("---------------------------------------------------------------LoadSceneMomSadEnding");
        StartCoroutine(LoadSceneCoroutine());
    }
    public void LoadSceneBadEnding()
    {
        sceneName = "Ending";                //มเร็งแดก
        targetUI = "SunBurnt";
        Debug.Log("---------------------------------------------------------------LoadSceneBadEnding");
        StartCoroutine(LoadSceneCoroutine());    
    }
    public void MainMenu()
    {
        sceneName = "Prototype[02]";                //หน้าMenu
        targetUI = "";
        Debug.Log("---------------------------------------------------------------MainMenu");
        StartCoroutine(LoadSceneCoroutine());
    }
    public void Game()
    {
        sceneName = "Prototype[02]";                //หน้าเกม
        targetUI = "";
        Debug.Log("---------------------------------------------------------------Game");
        StartCoroutine(LoadSceneCoroutine());
    }
    IEnumerator LoadSceneCoroutine()
    {
        Debug.Log("---------------------------------------------------------------LoadSceneCoroutine");
        fadeSystem.FadeToBlack();
        yield return new WaitForSeconds(fadeSystem.fadeDuration);
        yield return SceneManager.LoadSceneAsync(sceneName);
        if (!string.IsNullOrEmpty(targetUI))
        {
            Canvas canvas = FindAnyObjectByType<Canvas>();

            if (canvas != null)
            {
                Transform uiTarget = canvas.transform.Find(targetUI);
                if (uiTarget != null)
                {
                    uiTarget.gameObject.SetActive(true);
                    Debug.Log("เปิด UI: " + targetUI);
                }
                else
                {
                    Debug.LogWarning("หาไม่เจอ: " + targetUI);
                }
            }
            else
            {
                Debug.LogWarning("ไม่มี Canvas ใน scene: " + sceneName);
            }
        }
    }
}
