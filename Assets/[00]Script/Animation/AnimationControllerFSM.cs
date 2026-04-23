// ════════════════════════════════════════════════════════════════════════════
//  AnimationControllerFSM.cs  —  Self-contained, one-file Animation FSM
//  ─────────────────────────────────────────────────────────────────────────
//  Reads (never rewrites):
//      PlayerMovement  →  _IsHit           (stun flag)
//      InputSystem     →  InputVector.x
//      Rigidbody2D     →  linearVelocity.y
//      MinigameCrab    →  isActive          (trapped flag)
//      HotGauge        →  CurrentGauge      ◀ rename to match your actual field
//      IceCreamCount   →  HasStarted        (carrying ice-cream flag)
//
//  Setup in Inspector:
//      • Assign the same GroundCheck transform + radius + layer as PlayerJump
//      • Set Hot Threshold (default 70)
//
//  Animator bool names expected (must match exactly):
//      Idle_Normal         Idle_Hot
//      Run_Normal          Run_Hot
//      Jump_Normal         Jump_Hot
//      Falling_Normal      Falling_Hot
//      Idle_Normal_Icecream    Idle_Hot_Icecream
//      Run_Normal_Icecream     Run_Hot_Icecream
//      Jump_Normal_Icecream    Jump_Hot_Icecream
//      Falling_Normal_Icecream Falling_Hot_Icecream
//      Stunned             Trapped
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

        Transition();
    }

    protected abstract void Transition();

    // ── Shared selection helpers ──────────────────────────────────────────
    //  Each helper branches on IsCarrying first, then IsHot.

    /// Best idle state for current carrying + heat level.
    protected AnimState IdleFor()
    {
        if (C.IsCarrying) return C.IsHot ? (AnimState)C.IdleHotIcecreamSt : C.IdleNormalIcecreamSt;
        return C.IsHot ? (AnimState)C.IdleHotSt : C.IdleNormalSt;
    }

    /// Best run state for current carrying + heat level.
    protected AnimState RunFor()
    {
        if (C.IsCarrying) return C.IsHot ? (AnimState)C.RunHotIcecreamSt : C.RunNormalIcecreamSt;
        return C.IsHot ? (AnimState)C.RunHotSt : C.RunNormalSt;
    }

    /// Best jump state for current carrying + heat level.
    protected AnimState JumpFor()
    {
        if (C.IsCarrying) return C.IsHot ? (AnimState)C.JumpHotIcecreamSt : C.JumpNormalIcecreamSt;
        return C.IsHot ? (AnimState)C.JumpHotSt : C.JumpNormalSt;
    }

    /// Best falling state for current carrying + heat level.
    protected AnimState FallFor()
    {
        if (C.IsCarrying) return C.IsHot ? (AnimState)C.FallingHotIcecreamSt : C.FallingNormalIcecreamSt;
        return C.IsHot ? (AnimState)C.FallingHotSt : C.FallingNormalSt;
    }

    /// Best grounded state (run or idle).
    protected AnimState BestGrounded() => C.IsMoving ? RunFor() : IdleFor();

    /// Best airborne state (jump or fall).
    protected AnimState BestAir() => C.VelocityY > 0.01f ? JumpFor() : FallFor();

    /// Best state regardless of ground/air.
    protected AnimState BestAny() => C.IsGrounded ? BestGrounded() : BestAir();
}

// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  3.  CONCRETE STATES — NORMAL (no ice cream)                             ║
// ╚══════════════════════════════════════════════════════════════════════════╝

// ── Idle Normal ───────────────────────────────────────────────────────────
//  Condition: grounded, not moving, not hot, not carrying
internal sealed class AnimIdleNormalState : AnimState
{
    public AnimIdleNormalState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Idle_Normal") { }

    protected override void Transition()
    {
        if (C.IsCarrying) { FSM.ChangeState(IdleFor()); return; }
        if (!C.IsGrounded) { FSM.ChangeState(BestAir()); return; }
        if (C.IsMoving) { FSM.ChangeState(RunFor()); return; }
        if (C.IsHot) { FSM.ChangeState(C.IdleHotSt); return; }
    }
}

// ── Idle Hot ──────────────────────────────────────────────────────────────
//  Condition: grounded, not moving, hot, not carrying
internal sealed class AnimIdleHotState : AnimState
{
    public AnimIdleHotState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Idle_Hot") { }

    protected override void Transition()
    {
        if (C.IsCarrying) { FSM.ChangeState(IdleFor()); return; }
        if (!C.IsGrounded) { FSM.ChangeState(BestAir()); return; }
        if (C.IsMoving) { FSM.ChangeState(RunFor()); return; }
        if (!C.IsHot) { FSM.ChangeState(C.IdleNormalSt); return; }
    }
}

// ── Run Normal ────────────────────────────────────────────────────────────
//  Condition: grounded, moving, not hot, not carrying
internal sealed class AnimRunNormalState : AnimState
{
    public AnimRunNormalState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Run_Normal") { }

    protected override void Transition()
    {
        if (C.IsCarrying) { FSM.ChangeState(RunFor()); return; }
        if (!C.IsGrounded) { FSM.ChangeState(BestAir()); return; }
        if (!C.IsMoving) { FSM.ChangeState(IdleFor()); return; }
        if (C.IsHot) { FSM.ChangeState(C.RunHotSt); return; }
    }
}

// ── Run Hot ───────────────────────────────────────────────────────────────
//  Condition: grounded, moving, hot, not carrying
internal sealed class AnimRunHotState : AnimState
{
    public AnimRunHotState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Run_Hot") { }

    protected override void Transition()
    {
        if (C.IsCarrying) { FSM.ChangeState(RunFor()); return; }
        if (!C.IsGrounded) { FSM.ChangeState(BestAir()); return; }
        if (!C.IsMoving) { FSM.ChangeState(IdleFor()); return; }
        if (!C.IsHot) { FSM.ChangeState(C.RunNormalSt); return; }
    }
}

// ── Jump Normal ───────────────────────────────────────────────────────────
//  Condition: airborne, velocityY > 0.01, not hot, not carrying
internal sealed class AnimJumpNormalState : AnimState
{
    public AnimJumpNormalState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Jump_Normal") { }

    protected override void Transition()
    {
        if (C.IsCarrying) { FSM.ChangeState(JumpFor()); return; }
        if (C.IsGrounded) { FSM.ChangeState(BestGrounded()); return; }
        if (C.IsHot) { FSM.ChangeState(C.JumpHotSt); return; }
        if (C.VelocityY <= 0.01f) { FSM.ChangeState(FallFor()); return; }
    }
}

// ── Jump Hot ──────────────────────────────────────────────────────────────
//  Condition: airborne, velocityY > 0.01, hot, not carrying
internal sealed class AnimJumpHotState : AnimState
{
    public AnimJumpHotState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Jump_Hot") { }

    protected override void Transition()
    {
        if (C.IsCarrying) { FSM.ChangeState(JumpFor()); return; }
        if (C.IsGrounded) { FSM.ChangeState(BestGrounded()); return; }
        if (!C.IsHot) { FSM.ChangeState(C.JumpNormalSt); return; }
        if (C.VelocityY <= 0.01f) { FSM.ChangeState(FallFor()); return; }
    }
}

// ── Falling Normal ────────────────────────────────────────────────────────
//  Condition: airborne, velocityY ≤ 0.01, not hot, not carrying
internal sealed class AnimFallingNormalState : AnimState
{
    public AnimFallingNormalState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Falling_Normal") { }

    protected override void Transition()
    {
        if (C.IsCarrying) { FSM.ChangeState(FallFor()); return; }
        if (C.IsGrounded) { FSM.ChangeState(BestGrounded()); return; }
        if (C.IsHot) { FSM.ChangeState(C.FallingHotSt); return; }
    }
}

// ── Falling Hot ───────────────────────────────────────────────────────────
//  Condition: airborne, velocityY ≤ 0.01, hot, not carrying
internal sealed class AnimFallingHotState : AnimState
{
    public AnimFallingHotState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Falling_Hot") { }

    protected override void Transition()
    {
        if (C.IsCarrying) { FSM.ChangeState(FallFor()); return; }
        if (C.IsGrounded) { FSM.ChangeState(BestGrounded()); return; }
        if (!C.IsHot) { FSM.ChangeState(C.FallingNormalSt); return; }
    }
}

// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  4.  CONCRETE STATES — ICECREAM variants                                 ║
// ╚══════════════════════════════════════════════════════════════════════════╝

// ── Idle Normal Icecream ──────────────────────────────────────────────────
//  Condition: carrying, grounded, not moving, not hot
internal sealed class AnimIdleNormalIcecreamState : AnimState
{
    public AnimIdleNormalIcecreamState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Idle_Normal_Icecream") { }

    protected override void Transition()
    {
        if (!C.IsCarrying) { FSM.ChangeState(IdleFor()); return; }
        if (!C.IsGrounded) { FSM.ChangeState(BestAir()); return; }
        if (C.IsMoving) { FSM.ChangeState(RunFor()); return; }
        if (C.IsHot) { FSM.ChangeState(C.IdleHotIcecreamSt); return; }
    }
}

// ── Idle Hot Icecream ─────────────────────────────────────────────────────
//  Condition: carrying, grounded, not moving, hot
internal sealed class AnimIdleHotIcecreamState : AnimState
{
    public AnimIdleHotIcecreamState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Idle_Hot_Icecream") { }

    protected override void Transition()
    {
        if (!C.IsCarrying) { FSM.ChangeState(IdleFor()); return; }
        if (!C.IsGrounded) { FSM.ChangeState(BestAir()); return; }
        if (C.IsMoving) { FSM.ChangeState(RunFor()); return; }
        if (!C.IsHot) { FSM.ChangeState(C.IdleNormalIcecreamSt); return; }
    }
}

// ── Run Normal Icecream ───────────────────────────────────────────────────
//  Condition: carrying, grounded, moving, not hot
internal sealed class AnimRunNormalIcecreamState : AnimState
{
    public AnimRunNormalIcecreamState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Run_Normal_Icecream") { }

    protected override void Transition()
    {
        if (!C.IsCarrying) { FSM.ChangeState(RunFor()); return; }
        if (!C.IsGrounded) { FSM.ChangeState(BestAir()); return; }
        if (!C.IsMoving) { FSM.ChangeState(IdleFor()); return; }
        if (C.IsHot) { FSM.ChangeState(C.RunHotIcecreamSt); return; }
    }
}

// ── Run Hot Icecream ──────────────────────────────────────────────────────
//  Condition: carrying, grounded, moving, hot
internal sealed class AnimRunHotIcecreamState : AnimState
{
    public AnimRunHotIcecreamState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Run_Hot_Icecream") { }

    protected override void Transition()
    {
        if (!C.IsCarrying) { FSM.ChangeState(RunFor()); return; }
        if (!C.IsGrounded) { FSM.ChangeState(BestAir()); return; }
        if (!C.IsMoving) { FSM.ChangeState(IdleFor()); return; }
        if (!C.IsHot) { FSM.ChangeState(C.RunNormalIcecreamSt); return; }
    }
}

// ── Jump Normal Icecream ──────────────────────────────────────────────────
//  Condition: carrying, airborne, velocityY > 0.01, not hot
internal sealed class AnimJumpNormalIcecreamState : AnimState
{
    public AnimJumpNormalIcecreamState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Jump_Normal_Icecream") { }

    protected override void Transition()
    {
        if (!C.IsCarrying) { FSM.ChangeState(JumpFor()); return; }
        if (C.IsGrounded) { FSM.ChangeState(BestGrounded()); return; }
        if (C.IsHot) { FSM.ChangeState(C.JumpHotIcecreamSt); return; }
        if (C.VelocityY <= 0.01f) { FSM.ChangeState(FallFor()); return; }
    }
}

// ── Jump Hot Icecream ─────────────────────────────────────────────────────
//  Condition: carrying, airborne, velocityY > 0.01, hot
internal sealed class AnimJumpHotIcecreamState : AnimState
{
    public AnimJumpHotIcecreamState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Jump_Hot_Icecream") { }

    protected override void Transition()
    {
        if (!C.IsCarrying) { FSM.ChangeState(JumpFor()); return; }
        if (C.IsGrounded) { FSM.ChangeState(BestGrounded()); return; }
        if (!C.IsHot) { FSM.ChangeState(C.JumpNormalIcecreamSt); return; }
        if (C.VelocityY <= 0.01f) { FSM.ChangeState(FallFor()); return; }
    }
}

// ── Falling Normal Icecream ───────────────────────────────────────────────
//  Condition: carrying, airborne, velocityY ≤ 0.01, not hot
internal sealed class AnimFallingNormalIcecreamState : AnimState
{
    public AnimFallingNormalIcecreamState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Falling_Normal_Icecream") { }

    protected override void Transition()
    {
        if (!C.IsCarrying) { FSM.ChangeState(FallFor()); return; }
        if (C.IsGrounded) { FSM.ChangeState(BestGrounded()); return; }
        if (C.IsHot) { FSM.ChangeState(C.FallingHotIcecreamSt); return; }
    }
}

// ── Falling Hot Icecream ──────────────────────────────────────────────────
//  Condition: carrying, airborne, velocityY ≤ 0.01, hot
internal sealed class AnimFallingHotIcecreamState : AnimState
{
    public AnimFallingHotIcecreamState(AnimationControllerFSM c, Animator a, AnimFSM f)
        : base(c, a, f, "Falling_Hot_Icecream") { }

    protected override void Transition()
    {
        if (!C.IsCarrying) { FSM.ChangeState(FallFor()); return; }
        if (C.IsGrounded) { FSM.ChangeState(BestGrounded()); return; }
        if (!C.IsHot) { FSM.ChangeState(C.FallingNormalIcecreamSt); return; }
    }
}

// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  5.  CONCRETE STATES — INTERRUPT (Stunned / Trapped)                     ║
// ╚══════════════════════════════════════════════════════════════════════════╝

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
// ║  6.  MONOBEHAVIOUR CONTROLLER                                            ║
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
    public bool IsCarrying { get; private set; }  // true while IceCreamCount.HasStarted
    public float VelocityY { get; private set; }

    // ── State instances — normal ───────────────────────────────────────────
    internal AnimIdleNormalState IdleNormalSt { get; private set; }
    internal AnimIdleHotState IdleHotSt { get; private set; }
    internal AnimRunNormalState RunNormalSt { get; private set; }
    internal AnimRunHotState RunHotSt { get; private set; }
    internal AnimJumpNormalState JumpNormalSt { get; private set; }
    internal AnimJumpHotState JumpHotSt { get; private set; }
    internal AnimFallingNormalState FallingNormalSt { get; private set; }
    internal AnimFallingHotState FallingHotSt { get; private set; }

    // ── State instances — icecream ─────────────────────────────────────────
    internal AnimIdleNormalIcecreamState IdleNormalIcecreamSt { get; private set; }
    internal AnimIdleHotIcecreamState IdleHotIcecreamSt { get; private set; }
    internal AnimRunNormalIcecreamState RunNormalIcecreamSt { get; private set; }
    internal AnimRunHotIcecreamState RunHotIcecreamSt { get; private set; }
    internal AnimJumpNormalIcecreamState JumpNormalIcecreamSt { get; private set; }
    internal AnimJumpHotIcecreamState JumpHotIcecreamSt { get; private set; }
    internal AnimFallingNormalIcecreamState FallingNormalIcecreamSt { get; private set; }
    internal AnimFallingHotIcecreamState FallingHotIcecreamSt { get; private set; }

    // ── State instances — interrupt ────────────────────────────────────────
    internal AnimStunnedState StunnedSt { get; private set; }
    internal AnimTrappedState TrappedSt { get; private set; }

    // ── Private references ─────────────────────────────────────────────────
    [SerializeField] private Animator m_Anim;
    private Rigidbody2D m_Rb;
    private PlayerMovement m_Movement;
    private InputSystem m_Input;
    private HotGauge m_HotGauge;
    private MinigameCrab m_MinigameCrab;
    private IceCreamCount m_IceCreamCount;

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
        m_IceCreamCount = FindFirstObjectByType<IceCreamCount>();
    }

    private void Start()
    {
        m_FSM = new AnimFSM();

        // ── Normal states ─────────────────────────────────────────────────
        IdleNormalSt = new AnimIdleNormalState(this, m_Anim, m_FSM);
        IdleHotSt = new AnimIdleHotState(this, m_Anim, m_FSM);
        RunNormalSt = new AnimRunNormalState(this, m_Anim, m_FSM);
        RunHotSt = new AnimRunHotState(this, m_Anim, m_FSM);
        JumpNormalSt = new AnimJumpNormalState(this, m_Anim, m_FSM);
        JumpHotSt = new AnimJumpHotState(this, m_Anim, m_FSM);
        FallingNormalSt = new AnimFallingNormalState(this, m_Anim, m_FSM);
        FallingHotSt = new AnimFallingHotState(this, m_Anim, m_FSM);

        // ── Icecream states ───────────────────────────────────────────────
        IdleNormalIcecreamSt = new AnimIdleNormalIcecreamState(this, m_Anim, m_FSM);
        IdleHotIcecreamSt = new AnimIdleHotIcecreamState(this, m_Anim, m_FSM);
        RunNormalIcecreamSt = new AnimRunNormalIcecreamState(this, m_Anim, m_FSM);
        RunHotIcecreamSt = new AnimRunHotIcecreamState(this, m_Anim, m_FSM);
        JumpNormalIcecreamSt = new AnimJumpNormalIcecreamState(this, m_Anim, m_FSM);
        JumpHotIcecreamSt = new AnimJumpHotIcecreamState(this, m_Anim, m_FSM);
        FallingNormalIcecreamSt = new AnimFallingNormalIcecreamState(this, m_Anim, m_FSM);
        FallingHotIcecreamSt = new AnimFallingHotIcecreamState(this, m_Anim, m_FSM);

        // ── Interrupt states ──────────────────────────────────────────────
        StunnedSt = new AnimStunnedState(this, m_Anim, m_FSM);
        TrappedSt = new AnimTrappedState(this, m_Anim, m_FSM);

        m_FSM.Initialize(IdleNormalSt);
    }

    private void Update()
    {
        RefreshContext();
        m_FSM.Tick();
    }

    /// <summary>
    /// Reads all source scripts once per frame and exposes the results as
    /// simple bool/float properties so every AnimState can stay read-only.
    /// </summary>
    private void RefreshContext()
    {
        // Ground check
        IsGrounded = Physics2D.OverlapCircle(
            m_GroundCheck.position, m_GroundCheckRadius, m_GroundLayer);

        // Horizontal movement
        IsMoving = Mathf.Abs(m_Input.InputVector.x) > 0.01f;

        // Vertical velocity
        VelocityY = m_Rb.linearVelocity.y;

        // Stun
        IsStunned = m_Movement._IsHit;

        // Trapped
        IsTrapped = m_MinigameCrab != null && m_MinigameCrab.isActive;

        // Hot
        // ▶ If your HotGauge field is named differently, change "CurrentGauge" below.
        IsHot = m_HotGauge != null && m_HotGauge.CurrentGauge >= m_HotThreshold;

        // Carrying ice cream — true once the player presses E at the IceCreamCount trigger
        IsCarrying = m_IceCreamCount != null && m_IceCreamCount.HasStarted;
    }

    // ── Editor helpers ─────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        if (m_GroundCheck == null) return;
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(m_GroundCheck.position, m_GroundCheckRadius);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (m_FSM?.Current == null) return;
        var style = new GUIStyle(GUI.skin.label) { fontSize = 11, normal = { textColor = Color.yellow } };
        GUI.Label(new Rect(10, 10, 250, 20), $"[AnimFSM] {m_FSM.Current.GetType().Name}", style);
    }
#endif
}