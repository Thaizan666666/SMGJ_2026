using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScene : MonoBehaviour
{
    public static ManagerScene Instance;

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
    public void LoadSceneMomHappyEnding()
    {
        //SceneManager.LoadScene();                //ได้ไอตืม
    }
    public void LoadSceneMomSadEnding()
    {
        //SceneManager.LoadScene();                //อดแดก
    }
    public void LoadSceneBadEnding()
    {
        //SceneManager.LoadScene();                //มเร็งแดก
    }
    public void MainMenu()
    {
        //SceneManager.LoadScene();                //หน้าMenu
    }
}
