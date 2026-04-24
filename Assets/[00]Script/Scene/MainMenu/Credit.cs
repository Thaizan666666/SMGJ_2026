using UnityEngine;
using UnityEngine.UI;

public class Credit : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _btn;
    [SerializeField] private Button _btnStart;
    void Start()
    {
        _panel.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            CloseCradit();
        }
    }
    public void OpenCradit()
    {
        _panel.SetActive(true);
        _btn.gameObject.SetActive(false);
        _btnStart.gameObject.SetActive(false);
    }
    public void CloseCradit()
    {
        _panel.SetActive(false);
        _btn.gameObject.SetActive(true);
        _btnStart.gameObject.SetActive(true);
    }
}
