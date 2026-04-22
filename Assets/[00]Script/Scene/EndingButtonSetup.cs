using UnityEngine;
using UnityEngine.UI;
public class EndingButtonSetup : MonoBehaviour
{
    void Start()
    {
        // หา ManagerScene จาก Instance
        ManagerScene manager = ManagerScene.Instance;

        // หา Button แต่ละอันด้วยชื่อ แล้วผูก function
        Button btnReset = GameObject.Find("Reset").GetComponent<Button>();
        Button btnMainMenu = GameObject.Find("MainMenu").GetComponent<Button>();

        // ผูก OnClick
        btnReset.onClick.AddListener(() => manager.Game());
        btnMainMenu.onClick.AddListener(() => manager.MainMenu());
    }
}
