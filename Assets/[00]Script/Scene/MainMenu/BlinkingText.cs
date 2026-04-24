using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BlinkingText : MonoBehaviour
{
    public Image image;
    public float speed = 1.5f;
    private Button InGame;
    private Coroutine _blinkRoutine;

    void Awake()
    {
        if (image == null)
            image = GetComponent<Image>();

        ManagerScene manager = ManagerScene.Instance;
        InGame = GameObject.Find("Press_Spacebar_to_play").GetComponent<Button>();
        InGame.onClick.AddListener(() => manager.LoadIntro());
    }

    void OnEnable()
    {
        if (_blinkRoutine != null) StopCoroutine(_blinkRoutine);
        _blinkRoutine = StartCoroutine(FadeBlink());
    }

    void OnDisable()
    {
        if (_blinkRoutine != null) StopCoroutine(_blinkRoutine);
        _blinkRoutine = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ManagerScene.Instance.LoadIntro();
            InGame?.onClick.Invoke();
            if (_blinkRoutine != null) StopCoroutine(_blinkRoutine);
        }
    }

    IEnumerator FadeBlink()
    {
        Color c = image.color;
        while (true)
        {
            // Fade out
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * speed;
                c.a = Mathf.Lerp(1, 0, t);
                image.color = c;
                yield return null;
            }
            // Fade in
            t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * speed;
                c.a = Mathf.Lerp(0, 1, t);
                image.color = c;
                yield return null;
            }
        }
    }
}