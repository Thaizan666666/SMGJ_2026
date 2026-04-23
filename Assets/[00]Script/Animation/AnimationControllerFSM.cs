// ════════════════════════════════════════════════════════════════════════════
//  AnimationControllerFSM.cs  —  Self-contained, one-file Animation FSM
//  ─────────────────────────────────────────────────────────────────────────
//  Reads (never rewrites):
//      PlayerMovement  →  _IsHit  (stun flag)
//      InputSystem     →  InputVector.x
//      Rigidbody2D     →  linearVelocity.y
//      MinigameCrab    →  isActive  (trapped flag)
//      HotGauge        →  currentHeat  ◀ rename to match your actual field
//
//  Setup in Inspector:
//      • Assign the same GroundCheck transform + radius + layer as PlayerJump
//      • Set Hot Threshold (default 70)
//
//  Animator bool names expected (must match exactly):
//      Idle_Normal  Idle_Hot  Run_Normal  Run_Hot
//      Jump_Normal  Jump_Hot  Falling_Normal  Falling_Hot
//      Stunned      Trapped
// ════════════════════════════════════════════════════════════════════════════

using UnityEngine;

// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  1.  MINI STATE MACHINE                                                  ║
// ╚══════════════════════════════════════════════════════════════════════════╝

internal sealed class AnimFSM
{
    public AnimState Current { get; private set; }

    public void Initialize(AnimState startState)
    {
        Current = startState;
        Current.Enter();
    }

    public void ChangeState(AnimState nextState)
    {
        if (ReferenceEquals(nextState, Current)) return;   // no self-transitions
        Current.Exit();
        Current = nextState;
        Current.Enter();
    }

    public void Tick() => Current.Update();
}

// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  2.  ABSTRACT BASE STATE                                                 ║
// ╚══════════════════════════════════════════════════════════════════════════╝

internal abstract class AnimState
{
    // Shorthand aliases
    protected readonly AnimationControllerFSM C;
    protected readonly Animator Anim;
    protected readonly AnimFSM FSM;
    private readonly string m_BoolName;

    protected AnimState(AnimationControllerFSM ctx, Animator anim, AnimFSM fsm, string boolName)
    {
        C = ctx;
        Anim = anim;
        FSM = fsm;
        m_BoolName = boolName;
    }

    // ── Lifecycle ─────────────────────────────────────────────────────────
    public virtual void Enter() => Anim.SetBool(m_BoolName, true);
    public virtual void Exit() => Anim.SetBool(m_BoolName, false);

    /// <summary>
    /// Called every frame by AnimFSM.Tick().
    /// Handles global Stunned/Trapped priority, then calls per-state Transition().
    /// </summary>
    public void Update()
    {
        // ── Priority 1: Stunned overrides everything ─────────────────────
        if (C.IsStunned && !(this is AnimStunnedState))
        {
            FSM.ChangeState(C.StunnedSt);
            return;
        }

        // ── Priority 2: Trapped overrides everything except Stunned ──────
        if (C.IsTrapped && !C.IsStunned && !(this is AnimTrappedState))
        {
            FSM.ChangeState(C.TrappedSt);
            return;
        }

        Transition();   // per-state logic
    }

    /// <summary>Implement per-state transition rules here.</summary>
    protected abstract void Transition();

    // ── Shared selection helpers ──────────────────────────────────────────

    /// Best idle state for current heat level.
    protected AnimState IdleFor() => C.IsHot ? (AnimState)C.IdleHotSt : C.IdleNormalSt;

    /// Best run state for current heat level.
    protected AnimState RunFor() => C.IsHot ? (AnimState)C.RunHotSt : C.RunNormalSt;

    /// Best jump state for current heat level.
    protected AnimState JumpFor() => C.IsHot ? (AnimState)C.JumpHotSt : C.JumpNormalSt;

    /// Best falling state for current heat level.
    protected AnimState FallFor() => C.IsHot ? (AnimState)C.FallingHotSt : C.FallingNormalSt;

    /// Best grounded state (run or idle) for current context.
    protected AnimState BestGrounded() => C.IsMoving ? RunFor() : IdleFor();

    /// Best airborne state (jump or fall) for current context.
    protected AnimState BestAir() => C.VelocityY > 0.01f ? JumpFor() : FallFor();

    /// Best state regardless of ground/air.
    protected AnimState BestAny() => C.IsGrounded ? BestGrounded() : BestAir();
}

// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  3.  CONCRETE STATES                                                     ║
// ╚══════════════════════════════════════════════════════════════════════════╝

// ── Idle Normal ───────────────────────────────────────────────────────────
//  Condition: grounded, not moving, not hot
//  Transitions: → Idle_Hot | Run_* | Jump_* | Falling_*
internal sealed class AnimIdleNormalState : AnimState
{
    public AnimIdleNormalState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Idle_Normal") { }

    protected override void Transition()
    {
        if (!C.IsGrounded) { FSM.ChangeState(BestAir()); return; }
        if (C.IsMoving) { FSM.ChangeState(RunFor()); return; }
        if (C.IsHot) { FSM.ChangeState(C.IdleHotSt); return; }
    }
}

// ── Idle Hot ──────────────────────────────────────────────────────────────
//  Condition: grounded, not moving, hot (≥ threshold)
//  Transitions: → Idle_Normal | Run_* | Jump_* | Falling_*
internal sealed class AnimIdleHotState : AnimState
{
    public AnimIdleHotState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Idle_Hot") { }

    protected override void Transition()
    {
        if (!C.IsGrounded) { FSM.ChangeState(BestAir()); return; }
        if (C.IsMoving) { FSM.ChangeState(RunFor()); return; }
        if (!C.IsHot) { FSM.ChangeState(C.IdleNormalSt); return; }
    }
}

// ── Run Normal ────────────────────────────────────────────────────────────
//  Condition: grounded, moving, not hot
//  Transitions: → Idle_* | Run_Hot | Jump_* | Falling_*
internal sealed class AnimRunNormalState : AnimState
{
    public AnimRunNormalState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Run_Normal") { }

    protected override void Transition()
    {
        if (!C.IsGrounded) { FSM.ChangeState(BestAir()); return; }
        if (!C.IsMoving) { FSM.ChangeState(IdleFor()); return; }
        if (C.IsHot) { FSM.ChangeState(C.RunHotSt); return; }
    }
}

// ── Run Hot ───────────────────────────────────────────────────────────────
//  Condition: grounded, moving, hot (≥ threshold)
//  Transitions: → Idle_* | Run_Normal | Jump_* | Falling_*
internal sealed class AnimRunHotState : AnimState
{
    public AnimRunHotState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Run_Hot") { }

    protected override void Transition()
    {
        if (!C.IsGrounded) { FSM.ChangeState(BestAir()); return; }
        if (!C.IsMoving) { FSM.ChangeState(IdleFor()); return; }
        if (!C.IsHot) { FSM.ChangeState(C.RunNormalSt); return; }
    }
}

// ── Jump Normal ───────────────────────────────────────────────────────────
//  Condition: airborne, velocityY > 0.01, not hot
//  Transitions: → Idle_* (landed) | Jump_Hot | Falling_*
internal sealed class AnimJumpNormalState : AnimState
{
    public AnimJumpNormalState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Jump_Normal") { }

    protected override void Transition()
    {
        if (C.IsGrounded) { FSM.ChangeState(BestGrounded()); return; }
        if (C.IsHot) { FSM.ChangeState(C.JumpHotSt); return; }
        if (C.VelocityY <= 0.01f) { FSM.ChangeState(FallFor()); return; }
    }
}

// ── Jump Hot ──────────────────────────────────────────────────────────────
//  Condition: airborne, velocityY > 0.01, hot (≥ threshold)
//  Transitions: → Idle_* (landed) | Jump_Normal | Falling_*
internal sealed class AnimJumpHotState : AnimState
{
    public AnimJumpHotState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Jump_Hot") { }

    protected override void Transition()
    {
        if (C.IsGrounded) { FSM.ChangeState(BestGrounded()); return; }
        if (!C.IsHot) { FSM.ChangeState(C.JumpNormalSt); return; }
        if (C.VelocityY <= 0.01f) { FSM.ChangeState(FallFor()); return; }
    }
}

// ── Falling Normal ────────────────────────────────────────────────────────
//  Condition: airborne, velocityY ≤ 0.01, not hot
//  Transitions: → Idle_* (landed) | Falling_Hot
internal sealed class AnimFallingNormalState : AnimState
{
    public AnimFallingNormalState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Falling_Normal") { }

    protected override void Transition()
    {
        if (C.IsGrounded) { FSM.ChangeState(BestGrounded()); return; }
        if (C.IsHot) { FSM.ChangeState(C.FallingHotSt); return; }
    }
}

// ── Falling Hot ───────────────────────────────────────────────────────────
//  Condition: airborne, velocityY ≤ 0.01, hot (≥ threshold)
//  Transitions: → Idle_* (landed) | Falling_Normal
internal sealed class AnimFallingHotState : AnimState
{
    public AnimFallingHotState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Falling_Hot") { }

    protected override void Transition()
    {
        if (C.IsGrounded) { FSM.ChangeState(BestGrounded()); return; }
        if (!C.IsHot) { FSM.ChangeState(C.FallingNormalSt); return; }
    }
}

// ── Stunned ───────────────────────────────────────────────────────────────
//  Condition: PlayerMovement._IsHit == true  (can interrupt any state)
//  Exit: → best contextual state once stun clears
internal sealed class AnimStunnedState : AnimState
{
    public AnimStunnedState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Stunned") { }

    protected override void Transition()
    {
        if (!C.IsStunned) FSM.ChangeState(BestAny());
    }
}

// ── Trapped ───────────────────────────────────────────────────────────────
//  Condition: MinigameCrab.isActive == true  (can interrupt any non-stun state)
//  Exit: → best contextual state once trap clears
internal sealed class AnimTrappedState : AnimState
{
    public AnimTrappedState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Trapped") { }

    protected override void Transition()
    {
        if (!C.IsTrapped) FSM.ChangeState(BestAny());
    }
}

// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  4.  MONOBEHAVIOUR CONTROLLER                                            ║
// ╚══════════════════════════════════════════════════════════════════════════╝

/// <summary>
/// Drop this on the Player GameObject alongside PlayerMovement, InputSystem, etc.
/// It drives the Animator purely through the FSM — no Animator transitions needed.
/// </summary>
public sealed class AnimationControllerFSM : MonoBehaviour
{
    // ── Inspector ──────────────────────────────────────────────────────────
    [Header("Ground Check  — copy the same values from PlayerJump")]
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private float m_GroundCheckRadius = 0.1f;
    [SerializeField] private LayerMask m_GroundLayer;

    [Header("Hot Threshold (0 – 100)")]
    [SerializeField] private float m_HotThreshold = 70f;

    // ── Shared read-only context (states poll these each frame) ───────────
    public bool IsGrounded { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsHot { get; private set; }
    public bool IsStunned { get; private set; }
    public bool IsTrapped { get; private set; }
    public float VelocityY { get; private set; }

    // ── State instances (internal: type visibility matches internal classes) ─
    internal AnimIdleNormalState IdleNormalSt { get; private set; }
    internal AnimIdleHotState IdleHotSt { get; private set; }
    internal AnimRunNormalState RunNormalSt { get; private set; }
    internal AnimRunHotState RunHotSt { get; private set; }
    internal AnimJumpNormalState JumpNormalSt { get; private set; }
    internal AnimJumpHotState JumpHotSt { get; private set; }
    internal AnimFallingNormalState FallingNormalSt { get; private set; }
    internal AnimFallingHotState FallingHotSt { get; private set; }
    internal AnimStunnedState StunnedSt { get; private set; }
    internal AnimTrappedState TrappedSt { get; private set; }

    // ── Private references ────────────────────────────────────────────────
    [SerializeField]private Animator m_Anim;
    private Rigidbody2D m_Rb;
    private PlayerMovement m_Movement;
    private InputSystem m_Input;
    private HotGauge m_HotGauge;
    private MinigameCrab m_MinigameCrab;

    // ── FSM ───────────────────────────────────────────────────────────────
    private AnimFSM m_FSM;

    // ─────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        //m_Anim = GetComponent<Animator>();
        m_Rb = GetComponent<Rigidbody2D>();
        m_Movement = GetComponent<PlayerMovement>();
        m_Input = GetComponent<InputSystem>();
        m_HotGauge = GetComponent<HotGauge>();
        m_MinigameCrab = FindFirstObjectByType<MinigameCrab>();
    }

    private void Start()
    {
        m_FSM = new AnimFSM();

        // Build all states (order doesn't matter)
        IdleNormalSt = new AnimIdleNormalState(this, m_Anim, m_FSM);
        IdleHotSt = new AnimIdleHotState(this, m_Anim, m_FSM);
        RunNormalSt = new AnimRunNormalState(this, m_Anim, m_FSM);
        RunHotSt = new AnimRunHotState(this, m_Anim, m_FSM);
        JumpNormalSt = new AnimJumpNormalState(this, m_Anim, m_FSM);
        JumpHotSt = new AnimJumpHotState(this, m_Anim, m_FSM);
        FallingNormalSt = new AnimFallingNormalState(this, m_Anim, m_FSM);
        FallingHotSt = new AnimFallingHotState(this, m_Anim, m_FSM);
        StunnedSt = new AnimStunnedState(this, m_Anim, m_FSM);
        TrappedSt = new AnimTrappedState(this, m_Anim, m_FSM);

        m_FSM.Initialize(IdleNormalSt);
    }

    private void Update()
    {
        RefreshContext();   // gather world-state first
        m_FSM.Tick();       // then run the FSM
    }

    /// <summary>
    /// Reads all source scripts once per frame and exposes the results as
    /// simple bool/float properties so every AnimState can stay read-only.
    /// </summary>
    private void RefreshContext()
    {
        // Ground check — same logic as PlayerJump (mirrored, not shared, to avoid coupling)
        IsGrounded = Physics2D.OverlapCircle(
            m_GroundCheck.position, m_GroundCheckRadius, m_GroundLayer);

        // Horizontal movement — from InputSystem
        IsMoving = Mathf.Abs(m_Input.InputVector.x) > 0.01f;

        // Vertical velocity — from Rigidbody2D (shared with PlayerJump)
        VelocityY = m_Rb.linearVelocity.y;

        // Stun — from PlayerMovement
        IsStunned = m_Movement._IsHit;

        // Trapped — MinigameCrab.isActive blocks movement in PlayerMovement
        IsTrapped = m_MinigameCrab != null && m_MinigameCrab.isActive;

        // Hot — from HotGauge
        // ▶ If your HotGauge field is named differently, change "currentHeat" below.
        IsHot = m_HotGauge != null && m_HotGauge.CurrentGauge >= m_HotThreshold;
    }

    // ── Editor helpers ────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        if (m_GroundCheck == null) return;
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(m_GroundCheck.position, m_GroundCheckRadius);
    }

#if UNITY_EDITOR
    // Handy label in Scene view showing the active animation state name
    private void OnGUI()
    {
        if (m_FSM?.Current == null) return;
        var style = new GUIStyle(GUI.skin.label) { fontSize = 11, normal = { textColor = Color.yellow } };
        GUI.Label(new Rect(10, 10, 250, 20), $"[AnimFSM] {m_FSM.Current.GetType().Name}", style);
    }
#endif
}