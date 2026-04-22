using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeSystem : MonoBehaviour
{
    [Header("Settings")]
    public float fadeDuration = 1.5f;
    public GameObject fadePanelPrefab; // ถ้ามี Prefab ลากใส่ได้เลย

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetOrCreateCanvasGroup();
    }
    private CanvasGroup GetValidCanvasGroup()
    {
        // ถ้า canvasGroup หายไป → สร้างใหม่
        if (canvasGroup == null)
        {
            Debug.Log("CanvasGroup หายไป → สร้างใหม่");
            canvasGroup = GetOrCreateCanvasGroup();
        }
        return canvasGroup;
    }
    private CanvasGroup GetOrCreateCanvasGroup()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();

        if (cg == null)
        {
            if (fadePanelPrefab != null)
            {
                // หา Canvas ที่มีอยู่ใน Scene
                Canvas existingCanvas = FindObjectOfType<Canvas>();

                if (existingCanvas != null)
                {
                    // สร้าง fadePanel เป็นลูกของ Canvas นั้น
                    GameObject instance = Instantiate(fadePanelPrefab, existingCanvas.transform);
                    cg = instance.GetComponentInChildren<CanvasGroup>();
                    Debug.Log("สร้าง fadePanel ใน Canvas ที่มีอยู่แล้ว");
                }
                else
                {
                    // ไม่มี Canvas เลย → สร้างลอยๆ แบบเดิม
                    GameObject instance = Instantiate(fadePanelPrefab);
                    cg = instance.GetComponentInChildren<CanvasGroup>();
                    DontDestroyOnLoad(instance);
                    Debug.Log("ไม่มี Canvas → สร้างลอยๆ");
                }
            }
            else
            {
                cg = gameObject.AddComponent<CanvasGroup>();
                Debug.Log("ไม่มี Prefab → AddComponent");
            }
        }
        else
        {
            Debug.Log("ใช้ CanvasGroup เดิม");
        }

        return cg;
    }

    public void FadeToBlack()
    {
        StartCoroutine(FadeCoroutine(0f, 1f, fadeDuration));
    }

    public void FadeFromBlack()
    {
        StartCoroutine(FadeCoroutine(1f, 0f, fadeDuration));
    }

    IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration)
    {
        CanvasGroup cg = GetValidCanvasGroup();
        canvasGroup.alpha = startAlpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}
