using UnityEngine;
using TMPro;
using System.Collections;

public class BlinkingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float speed = 1.5f;

    void Start()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshProUGUI>();

        StartCoroutine(FadeBlink());
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