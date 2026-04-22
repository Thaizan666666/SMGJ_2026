using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeSystem : MonoBehaviour
{
    [Header("Settings")]
    public float fadeDuration = 1f;
    public GameObject fadePanelPrefab;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetOrCreateCanvasGroup();
    }

    public void FadeToBlack() => StartCoroutine(FadeCoroutine(0f, 1f));
    public void FadeFromBlack() => StartCoroutine(FadeCoroutine(1f, 0f));

    IEnumerator FadeCoroutine(float from, float to)
    {
        // Recover if CanvasGroup was destroyed (e.g. scene reload)
        if (canvasGroup == null)
            canvasGroup = GetOrCreateCanvasGroup();

        canvasGroup.alpha = from;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    private CanvasGroup GetOrCreateCanvasGroup()
    {
        // Case 1: CanvasGroup already on this GameObject
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) return cg;

        // Case 2: Use prefab, attach to existing Canvas
        if (fadePanelPrefab != null)
        {
            Canvas canvas = FindAnyObjectByType<Canvas>();
            Transform parent = canvas != null ? canvas.transform : null;

            GameObject instance = parent != null
                ? Instantiate(fadePanelPrefab, parent)
                : Instantiate(fadePanelPrefab);

            if (parent == null)
                DontDestroyOnLoad(instance);

            cg = instance.GetComponentInChildren<CanvasGroup>();
            if (cg != null) return cg;
        }

        // Case 3: Fallback — add directly to this GameObject
        return gameObject.AddComponent<CanvasGroup>();
    }
}