using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static ManagerSound;

public class CoverSystem : MonoBehaviour
{
    public CoverSystemSetting _cfg;
    public int SampleResolution => _cfg != null ? _cfg.SampleResolution : 3;
    private Collider2D _coverCollider;

    [Header("Debugging")]
    [Tooltip("Enable or disable drawing gizmos for the cover area.")]
    public bool DebugDrawGizmos = true;

    private void Awake()
    {
        _coverCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_cfg.PlayerTag))
        {
            Debug.Log("Player entered the cover (partial)");

            StopLoopEffect("Sunburn");

#if UNITY_EDITOR
            _playerCollider = collision;
#endif            
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag(_cfg.PlayerTag)) return;

        StopLoopEffect("Sunburn");

        float percentage = GetOverlapPercentage(collision);
        HotGauge heatguage = collision.GetComponent<HotGauge>();

        if (IsPlayerFullyInside(collision))
        {
            Debug.Log($"Player is FULLY inside the cover ({percentage:F1}%) — Decrease hot gauge");
            // TODO:
            // - Decrease hot gauge by using coverSystemSetting.HotGaugeDecreaseRate per second
            // - You can use Time.deltaTime to make it frame-rate independent
            if (heatguage != null)
            {
                heatguage.IsInCover = true;
            }
        }
        else
        {
            Debug.Log($"Player is {percentage:F1}% inside the cover — Partially covered");
            // TODO: ;
            // Do what designer want. I have percentage value, you can use it to decide how much to increase/decrease hot gauge.
            // using coverSystemSetting.HotGaugeIncreaseRate and coverSystemSetting.HotGaugeDecreaseRate
            if (heatguage != null)
            {
                heatguage.IsInCover = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(_cfg.PlayerTag))
        {
            Debug.Log("Player exited the cover");
            LoopEffect("Sunburn");

#if UNITY_EDITOR
            _playerCollider = null;
#endif
        }
    }

    /// <summary>
    /// Returns true when every corner of the player's AABB is inside this collider.
    /// Works regardless of what Collider2D type this object uses.
    /// </summary>
    private bool IsPlayerFullyInside(Collider2D playerCollider)
    {
        if (_coverCollider == null) return false;

        Bounds b = playerCollider.bounds;
        Vector2[] corners =
        {
            new Vector2(b.min.x, b.min.y), // bottom-left
            new Vector2(b.max.x, b.min.y), // bottom-right
            new Vector2(b.min.x, b.max.y), // top-left
            new Vector2(b.max.x, b.max.y), // top-right
        };

        foreach (Vector2 corner in corners)
        {
            if (!_coverCollider.OverlapPoint(corner))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Samples a SampleResolution x SampleResolution grid across the player's AABB.
    /// Returns 0.0 (no overlap) to 100.0 (fully inside).
    /// Works for any Collider2D type on this GameObject.
    /// </summary>
    private float GetOverlapPercentage(Collider2D playerCollider)
    {
        if (_coverCollider == null) return 0f;
        Bounds b = playerCollider.bounds;
        int insideCount = 0;
        int totalCount = 0;
        for (int x = 0; x < SampleResolution; x++)
        {
            float tx = (x + 0.5f) / SampleResolution;
            for (int y = 0; y < SampleResolution; y++)
            {
                float ty = (y + 0.5f) / SampleResolution;
                Vector2 samplePoint = new Vector2(
                    Mathf.Lerp(b.min.x, b.max.x, tx),
                    Mathf.Lerp(b.min.y, b.max.y, ty)
                );
                if (_coverCollider.OverlapPoint(samplePoint))
                    insideCount++;
                totalCount++;
            }
        }
        
        return totalCount == 0 ? 0f : (insideCount / (float)totalCount) * 100f;
    }




#if UNITY_EDITOR
    private Collider2D _playerCollider; // for collecting player collider reference when player is inside the cover, so we can use it in OnDrawGizmos to draw the sampled points.

    /// <summary>
    /// You Know it's just for debugging, draw the polygon collider shape in the editor when selected.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!DebugDrawGizmos) return;

        Gizmos.color = _cfg.DebugColor;

        var colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            DrawCollider2DGizmo(col);
        }

        DrawPlayerPoint(_playerCollider);
    }

    private void DrawCollider2DGizmo(Collider2D col)
    {
        switch (col)
        {
            case PolygonCollider2D polygon:
                for (int i = 0; i < polygon.pathCount; i++)
                {
                    Vector2[] path = polygon.GetPath(i);
                    for (int j = 0; j < path.Length; j++)
                    {
                        Vector3 p1 = transform.TransformPoint(path[j]);
                        Vector3 p2 = transform.TransformPoint(path[(j + 1) % path.Length]);
                        Gizmos.DrawLine(p1, p2);
                    }
                }
                break;

            case BoxCollider2D box:
                Vector3 boxCenter = transform.TransformPoint(box.offset);
                Vector3 boxSize = new Vector3(
                    box.size.x * transform.lossyScale.x,
                    box.size.y * transform.lossyScale.y,
                    0f
                );
                // Draw 4 edges manually to respect rotation
                Vector2 half = boxSize * 0.5f;
                Vector3[] boxCorners =
                {
                boxCenter + transform.rotation * new Vector3(-half.x, -half.y),
                boxCenter + transform.rotation * new Vector3( half.x, -half.y),
                boxCenter + transform.rotation * new Vector3( half.x,  half.y),
                boxCenter + transform.rotation * new Vector3(-half.x,  half.y),
            };
                for (int i = 0; i < 4; i++)
                    Gizmos.DrawLine(boxCorners[i], boxCorners[(i + 1) % 4]);
                break;

            case CircleCollider2D circle:
                Vector3 circleCenter = transform.TransformPoint(circle.offset);
                float radius = circle.radius * Mathf.Max(
                    Mathf.Abs(transform.lossyScale.x),
                    Mathf.Abs(transform.lossyScale.y)
                );
                int segments = 36;
                for (int i = 0; i < segments; i++)
                {
                    float a1 = (i / (float)segments) * Mathf.PI * 2f;
                    float a2 = ((i + 1) / (float)segments) * Mathf.PI * 2f;
                    Vector3 p1 = circleCenter + new Vector3(Mathf.Cos(a1), Mathf.Sin(a1)) * radius;
                    Vector3 p2 = circleCenter + new Vector3(Mathf.Cos(a2), Mathf.Sin(a2)) * radius;
                    Gizmos.DrawLine(p1, p2);
                }
                break;

            case CapsuleCollider2D capsule:
                DrawCapsuleCollider2DGizmo(capsule);
                break;

            case EdgeCollider2D edge:
                Vector2[] edgePoints = edge.points;
                for (int i = 0; i < edgePoints.Length - 1; i++)
                {
                    Vector3 p1 = transform.TransformPoint(edgePoints[i]);
                    Vector3 p2 = transform.TransformPoint(edgePoints[i + 1]);
                    Gizmos.DrawLine(p1, p2);
                }
                break;
        }
    }

    private void DrawCapsuleCollider2DGizmo(CapsuleCollider2D capsule)
    {
        Vector2 size = capsule.size;
        Vector2 offset = capsule.offset;
        bool isVertical = capsule.direction == CapsuleDirection2D.Vertical;

        float width = size.x * Mathf.Abs(transform.lossyScale.x);
        float height = size.y * Mathf.Abs(transform.lossyScale.y);
        float radius = isVertical
            ? Mathf.Min(width * 0.5f, height * 0.5f)
            : Mathf.Min(width * 0.5f, height * 0.5f);

        Vector3 center = transform.TransformPoint(offset);

        // Straight edges
        Vector3 upDir = transform.rotation * (isVertical ? Vector3.up : Vector3.right);
        Vector3 rightDir = transform.rotation * (isVertical ? Vector3.right : Vector3.up);

        float straightLength = isVertical
            ? Mathf.Max(0f, height * 0.5f - radius)
            : Mathf.Max(0f, width * 0.5f - radius);

        Vector3 topCenter = center + upDir * straightLength;
        Vector3 bottomCenter = center - upDir * straightLength;

        // Straight sides
        Gizmos.DrawLine(topCenter + rightDir * radius, bottomCenter + rightDir * radius);
        Gizmos.DrawLine(topCenter - rightDir * radius, bottomCenter - rightDir * radius);

        // Semicircles at each end
        int segments = 18;
        DrawSemiCircle(topCenter, upDir, rightDir, radius, segments);
        DrawSemiCircle(bottomCenter, -upDir, rightDir, radius, segments);
    }

    private void DrawSemiCircle(Vector3 center, Vector3 normal, Vector3 right, float radius, int segments)
    {
        for (int i = 0; i <= segments; i++)
        {
            float a1 = (i / (float)segments) * Mathf.PI;
            float a2 = ((i + 1) / (float)segments) * Mathf.PI;
            Vector3 p1 = center + (right * Mathf.Cos(a1) + normal * Mathf.Sin(a1)) * radius;
            Vector3 p2 = center + (right * Mathf.Cos(a2) + normal * Mathf.Sin(a2)) * radius;
            Gizmos.DrawLine(p1, p2);
        }
    }

    /// <summary>
    /// Debuging method to draw the player's AABB corners and sampled points in the editor when selected.
    /// </summary>
    /// <param name="playerCollider"></param>
    private void DrawPlayerPoint(Collider2D playerCollider)
    {
        if (_coverCollider == null || playerCollider == null) return;
        Bounds b = playerCollider.bounds;
        for (int x = 0; x < SampleResolution; x++)
        {
            float tx = (x + 0.5f) / SampleResolution;
            for (int y = 0; y < SampleResolution; y++)
            {
                float ty = (y + 0.5f) / SampleResolution;
                Vector2 samplePoint = new Vector2(
                    Mathf.Lerp(b.min.x, b.max.x, tx),
                    Mathf.Lerp(b.min.y, b.max.y, ty)
                );
                if (_coverCollider.OverlapPoint(samplePoint))
                    Gizmos.color = Color.green; // inside
                else
                    Gizmos.color = Color.red; // outside
                Gizmos.DrawSphere(samplePoint, 0.05f);
            }
        }
    }

#endif
}