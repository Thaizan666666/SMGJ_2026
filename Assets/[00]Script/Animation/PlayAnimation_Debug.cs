using UnityEngine;

public class PlayAnimation_Debug : MonoBehaviour
{
    private Animator _anim;

    public AnimationShow animationShow;
    private AnimationShow previousAnimation;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        previousAnimation = AnimationShow.None;
    }

    private void Update()
    {
        if (previousAnimation != animationShow)
        {
            switch (animationShow) { 
                case AnimationShow.None:
                    break;
                case AnimationShow.RunNormal:
                    previousAnimation = animationShow;
                    _anim.SetBool("RunningNormal", true);
                    _anim.SetBool("RunningHot", false);
                    _anim.SetBool("Idle", false);
                    break;
                case AnimationShow.RunHot:
                    previousAnimation = animationShow;
                    _anim.SetBool("RunningNormal", false);
                    _anim.SetBool("RunningHot", true);
                    _anim.SetBool("Idle", false);
                    break;
                case AnimationShow.RunIdle:
                    previousAnimation = animationShow;
                    _anim.SetBool("RunningNormal", false);
                    _anim.SetBool("RunningHot", false);
                    _anim.SetBool("Idle", true);
                    break;
                default:
                    _anim.SetBool("RunningNormal", false);
                    _anim.SetBool("RunningHot", false);
                    _anim.SetBool("Idle", false);
                    break;
            }
        }
    }



    public enum AnimationShow { 
        None = 0,
        RunNormal,
        RunHot,
        RunIdle,
    }    

}


