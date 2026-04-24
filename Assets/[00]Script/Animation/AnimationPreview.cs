// ════════════════════════════════════════════════════════════════════════════
//  AnimationPreview.cs
//  ─────────────────────────────────────────────────────────────────────────
//  Drop this on any duplicated GameObject that has an Animator.
//  Pick the desired animation from the "State" dropdown in the Inspector.
//  On Start it sets that bool to TRUE and all others to FALSE.
//
//  Usage:
//      1. Duplicate your animated GameObject as many times as you like.
//      2. Add this component to each duplicate.
//      3. Choose a different State enum value on each one.
//      4. Hit Play — each object plays its assigned animation.
// ════════════════════════════════════════════════════════════════════════════

using System;
using UnityEngine;

// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  ENUM  —  every animator bool, one entry each                            ║
// ╚══════════════════════════════════════════════════════════════════════════╝

public enum PlayerAnimState
{
    Idle_Normal,
    Idle_Normal_Icecream,
    Idle_Hot,
    Idle_Hot_Icecream,

    Run_Normal,
    Run_Normal_Icecream,
    Run_Hot,
    Run_Hot_Icecream,

    Jump_Normal,
    Jump_Normal_Icecream,
    Jump_Hot,
    Jump_Hot_Icecream,

    Falling_Normal,
    Falling_Normal_Icecream,
    Falling_Hot,
    Falling_Hot_Icecream,

    Stunned_Normal,
    Stunned_Normal_Icecream,
    Stunned_Hot,
    Stunned_Hot_Icecream,

    Trapped_Normal,
    Trapped_Normal_Icecream,
    Trapped_Hot,
    Trapped_Hot_Icecream,
}

// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  COMPONENT                                                               ║
// ╚══════════════════════════════════════════════════════════════════════════╝

[RequireComponent(typeof(Animator))]
public class AnimationPreview : MonoBehaviour
{
    [Tooltip("Which animation this object should play.")]
    public PlayerAnimState state = PlayerAnimState.Idle_Normal;

    private Animator m_Anim;
    private static readonly string[] k_Names = Enum.GetNames(typeof(PlayerAnimState));

    private void Awake()
    {
        m_Anim = GetComponent<Animator>();
    }

    private void Start()
    {
        ApplyState();
    }

    /// <summary>
    /// Sets the chosen bool to true and every other bool to false.
    /// Called automatically on Start; also callable at runtime or from
    /// the custom Inspector button.
    /// </summary>
    public void ApplyState()
    {
        if (m_Anim == null)
            m_Anim = GetComponent<Animator>();

        string active = k_Names[(int)state];
        foreach (string name in k_Names)
            m_Anim.SetBool(name, name == active);
    }

#if UNITY_EDITOR
    // Reapply immediately when the value is changed in the Inspector
    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        ApplyState();
    }
#endif
}

// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  CUSTOM INSPECTOR  —  "Apply Now" button for quick in-editor testing     ║
// ╚══════════════════════════════════════════════════════════════════════════╝

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(AnimationPreview))]
public class AnimationPreviewEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UnityEngine.GUILayout.Space(6);

        UnityEngine.GUI.backgroundColor = new UnityEngine.Color(0.45f, 1f, 0.55f);
        if (UnityEngine.GUILayout.Button("▶  Apply Now", UnityEngine.GUILayout.Height(28)))
            ((AnimationPreview)target).ApplyState();

        UnityEngine.GUI.backgroundColor = UnityEngine.Color.white;
    }
}
#endif
