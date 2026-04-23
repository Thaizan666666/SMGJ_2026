using UnityEngine;
using UnityEngine.UI;
public class EndingButtonSetup : MonoBehaviour
{
    public Button BTN_Reset;
    public Button BTN_MainMenu;

    void Start()
    {
        // หา ManagerScene จาก Instance
        ManagerScene manager = ManagerScene.Instance;
        // ผูก OnClick
        BTN_Reset.onClick.AddListener(() => manager.LoadGame());
        BTN_MainMenu.onClick.AddListener(() => manager.LoadMainMenu());
    }
}
