using UnityEngine;
using UnityEngine.UI;
using static ManagerSound;

public class BallIndicator : MonoBehaviour
{
    [Header("Indicator Settings")]
    public Sprite indicatorSprite;  
    public Color indicatorColor = Color.red;
    public float indicatorSize = 1000f;
    public float edgePadding = 50f;

    private RectTransform indicatorRect;
    private Canvas canvas;
    private Camera cam;
    private bool hasEnteredScreen = false;

    void Start()
    {
        cam = Camera.main;

        if (indicatorSprite == null)
        {
            Debug.LogWarning("BallIndicator: ยังไม่ได้ใส่ Sprite!", this);
            enabled = false;
            return;
        }

        PlayEffect("WarningBall");

        // หา Canvas ถ้าไม่มีสร้างใหม่
        canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("UICanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // สร้าง Indicator
        GameObject indicatorObj = new GameObject("Indicator_" + gameObject.name);
        indicatorObj.transform.SetParent(canvas.transform, false);

        Image img = indicatorObj.AddComponent<Image>();
        img.sprite = indicatorSprite;    // ใช้ Sprite จาก Inspector
        img.color = indicatorColor;
        img.SetNativeSize();             // ใช้ขนาดจริงของ Sprite ก่อน

        indicatorRect = indicatorObj.GetComponent<RectTransform>();
        indicatorRect.anchorMin = new Vector2(0.5f, 0.5f);
        indicatorRect.anchorMax = new Vector2(0.5f, 0.5f);
        indicatorRect.pivot = new Vector2(0.5f, 0.5f);
        indicatorRect.sizeDelta = new Vector2(indicatorSize, indicatorSize);
    }

    void Update()
    {
        if (indicatorRect == null) return;
        UpdateIndicatorPosition();
    }

    void UpdateIndicatorPosition()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        if (screenPos.z < 0) screenPos *= -1;

        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 dir = new Vector2(screenPos.x - center.x, screenPos.y - center.y);

        float halfW = center.x - edgePadding;
        float halfH = center.y - edgePadding;

        Vector2 edgePos;
        if (dir == Vector2.zero)
        {
            edgePos = Vector2.zero;
        }
        else if (Mathf.Abs(dir.x) * halfH > Mathf.Abs(dir.y) * halfW)
        {
            float sign = Mathf.Sign(dir.x);
            float slope = dir.y / dir.x;
            edgePos = new Vector2(sign * halfW, sign * slope * halfW);
        }
        else
        {
            float sign = Mathf.Sign(dir.y);
            float slope = dir.y / dir.x;
            edgePos = new Vector2(sign * dir.x / slope, sign * halfH);
        }

        indicatorRect.anchoredPosition = edgePos;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        indicatorRect.rotation = Quaternion.Euler(0f, 0f, angle);

        bool onScreen = screenPos.x > edgePadding && screenPos.x < Screen.width - edgePadding &&
                        screenPos.y > edgePadding && screenPos.y < Screen.height - edgePadding &&
                        screenPos.z > 0;

        indicatorRect.gameObject.SetActive(!onScreen);

        if (onScreen)
        {
            hasEnteredScreen = true;
        }

        if (hasEnteredScreen)
        {
            indicatorRect.gameObject.SetActive(false);
            return;
        }
    }

    void OnDestroy()
    {
        if (indicatorRect != null)
            Destroy(indicatorRect.gameObject);
    }
}