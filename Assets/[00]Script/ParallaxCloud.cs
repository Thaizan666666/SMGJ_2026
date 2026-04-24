// ════════════════════════════════════════════════════════════════════════════
//  ParallaxCloud.cs
//  ─────────────────────────────────────────────────────────────────────────
//  Seamless parallax using two sprite copies placed side by side.
//  When one copy drifts fully off-screen it is instantly repositioned
//  behind the other — the seam is never visible.
//
//  Setup:  add this script to a parent GameObject that has a SpriteRenderer.
//          The script clones the sprite into a second copy automatically.
//          No extra manual setup needed.
// ════════════════════════════════════════════════════════════════════════════

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxCloud : MonoBehaviour
{
    // ── Inspector ──────────────────────────────────────────────────────────
    [Header("Target")]
    [Tooltip("Leave empty — auto-uses Camera.main.")]
    public Transform cameraTransform;

    [Header("Parallax")]
    [Tooltip("0 = scrolls with camera (feels close). 1 = completely still (feels very far).")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.2f;

    [Tooltip("Constant rightward drift in world units/sec. 0 = no auto-scroll.")]
    public float driftSpeed = 0.5f;

    // ── Private ────────────────────────────────────────────────────────────
    private SpriteRenderer m_SprA;          // original (this object)
    private SpriteRenderer m_SprB;          // auto-created clone
    private Transform m_TransA;
    private Transform m_TransB;

    private float m_SpriteWidth;          // world-space width of one copy
    private Vector3 m_LastCamPos;

    // ─────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        // ── Camera ────────────────────────────────────────────────────────
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // ── Original sprite (this GameObject) ────────────────────────────
        m_SprA = GetComponent<SpriteRenderer>();
        m_TransA = transform;

        m_SpriteWidth = m_SprA.sprite != null
            ? m_SprA.sprite.bounds.size.x * transform.localScale.x
            : 20f;

        // ── Clone — placed exactly one sprite-width to the right ──────────
        GameObject clone = new GameObject($"{gameObject.name}_Copy");
        clone.transform.SetParent(transform.parent);          // same parent
        clone.transform.position = transform.position + Vector3.right * m_SpriteWidth;
        clone.transform.rotation = transform.rotation;
        clone.transform.localScale = transform.localScale;

        m_SprB = clone.AddComponent<SpriteRenderer>();
        m_SprB.sprite = m_SprA.sprite;
        m_SprB.sortingLayerID = m_SprA.sortingLayerID;
        m_SprB.sortingOrder = m_SprA.sortingOrder;
        m_SprB.color = m_SprA.color;
        m_SprB.flipX = m_SprA.flipX;
        m_TransB = clone.transform;
    }

    private void Start()
    {
        if (cameraTransform != null)
            m_LastCamPos = cameraTransform.position;
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        // ── How much the camera moved this frame ──────────────────────────
        float camDeltaX = cameraTransform.position.x - m_LastCamPos.x;
        m_LastCamPos = cameraTransform.position;

        // ── Parallax + drift movement ─────────────────────────────────────
        float moveX = camDeltaX * (1f - parallaxFactor) + driftSpeed * Time.deltaTime;
        m_TransA.position += Vector3.right * moveX;
        m_TransB.position += Vector3.right * moveX;

        // ── Seamless swap ─────────────────────────────────────────────────
        //  If copy A has drifted a full sprite-width to the right of copy B,
        //  teleport A to sit one width to the LEFT of B (and vice-versa).
        //  Because the swap distance equals exactly one sprite width, the
        //  pixel that was at the seam edge of the outgoing copy is now at
        //  the leading edge of the incoming copy — completely seamless.

        if (m_TransA.position.x > m_TransB.position.x + m_SpriteWidth)
            m_TransA.position = new Vector3(
                m_TransB.position.x - m_SpriteWidth,
                m_TransA.position.y,
                m_TransA.position.z);

        else if (m_TransB.position.x > m_TransA.position.x + m_SpriteWidth)
            m_TransB.position = new Vector3(
                m_TransA.position.x - m_SpriteWidth,
                m_TransB.position.y,
                m_TransB.position.z);

        else if (m_TransA.position.x < m_TransB.position.x - m_SpriteWidth)
            m_TransA.position = new Vector3(
                m_TransB.position.x + m_SpriteWidth,
                m_TransA.position.y,
                m_TransA.position.z);

        else if (m_TransB.position.x < m_TransA.position.x - m_SpriteWidth)
            m_TransB.position = new Vector3(
                m_TransA.position.x + m_SpriteWidth,
                m_TransB.position.y,
                m_TransB.position.z);
    }

    // ── Gizmos — show the two slot positions in Scene view ────────────────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.35f);
        float w = m_SpriteWidth > 0 ? m_SpriteWidth : 20f;
        Gizmos.DrawWireCube(transform.position, new Vector3(w, 1.5f, 0f));
        Gizmos.DrawWireCube(transform.position + Vector3.right * w, new Vector3(w, 1.5f, 0f));
    }
}