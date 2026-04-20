using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ManagerScene : MonoBehaviour
{
    public static ManagerScene Instance;

    private FadeSystem fadeSystem;
    private string sceneName;

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
        sceneName = "Prototype[02]";
        Debug.Log("---------------------------------------------------------------LoadSceneMomHappyEnding");
        StartCoroutine(LoadSceneCoroutine());
        //SceneManager.LoadScene();                //ได้ไอตืม
    }
    public void LoadSceneMomSadEnding()
    {
        sceneName = "Prototype[02]";
        Debug.Log("---------------------------------------------------------------LoadSceneMomSadEnding");
        StartCoroutine(LoadSceneCoroutine());
        //SceneManager.LoadScene();                //อดแดก
    }
    public void LoadSceneBadEnding()
    {
        sceneName = "Prototype[02]";
        Debug.Log("---------------------------------------------------------------LoadSceneBadEnding");
        StartCoroutine(LoadSceneCoroutine());
        //SceneManager.LoadScene();                //มเร็งแดก
    }
    public void MainMenu()
    {
        sceneName = "Prototype[02]";
        Debug.Log("---------------------------------------------------------------MainMenu");
        StartCoroutine(LoadSceneCoroutine());
        //SceneManager.LoadScene();                //หน้าMenu
    }
    public void Game()
    {
        sceneName = "Prototype[02]";
        Debug.Log("---------------------------------------------------------------Game");
        StartCoroutine(LoadSceneCoroutine());
        //SceneManageer.LoadScene();               //หน้าเกม
    }
    IEnumerator LoadSceneCoroutine()
    {
        Debug.Log("---------------------------------------------------------------LoadSceneCoroutine");
        fadeSystem.FadeToBlack();
        yield return new WaitForSeconds(fadeSystem.fadeDuration);
        SceneManager.LoadScene(sceneName);
    }
}
