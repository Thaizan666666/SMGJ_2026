using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Xml.Linq;

public class BlinkingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float speed = 1.5f;

    private Button InGame;
    private Coroutine _blinkRoutine;

    void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshProUGUI>();

        StartCoroutine(FadeBlink());

        ManagerScene manager = ManagerScene.Instance;

        InGame = GameObject.Find("Press_Spacebar_to_play").GetComponent<Button>();

        InGame.onClick.AddListener(() => manager.LoadIntro());
    }
    void OnEnable()
    {
        // เริ่ม blink ทุกครั้งที่ GameObject ถูกเปิด
        if (_blinkRoutine != null) StopCoroutine(_blinkRoutine);
        _blinkRoutine = StartCoroutine(FadeBlink());
    }

    void OnDisable()
    {
        // หยุด blink ตอน GameObject ถูกปิด
        if (_blinkRoutine != null) StopCoroutine(_blinkRoutine);
        _blinkRoutine = null;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ManagerScene.Instance.LoadIntro();
            InGame?.onClick.Invoke();
            StopCoroutine(_blinkRoutine);
        }
    }

    IEnumerator FadeBlink()
    {
        while (true)
        {
            // Fade out
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * speed;
                textMesh.alpha = Mathf.Lerp(1, 0, t);
                yield return null;
            }

            // Fade in
            t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * speed;
                textMesh.alpha = Mathf.Lerp(0, 1, t);
                yield return null;
            }
        }
    }
}