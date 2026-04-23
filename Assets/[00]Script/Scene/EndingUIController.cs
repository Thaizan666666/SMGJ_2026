using UnityEngine;

/// <summary>
/// Place this script anywhere in the Ending scene.
/// It reads ManagerScene.PendingTargetUI on Start() and activates + animates the right UI.
/// 
/// HOW TO SET UP:
/// 1. Add this script to any GameObject in the Ending scene (e.g. an empty "UIController")
/// 2. In the Inspector, assign HappyEnding, MonSad, SunBurnt GameObjects to the slots below
/// 3. Make sure all 3 GameObjects are INACTIVE by default in the scene
/// 4. Make sure each has an Animator with a trigger named "Play"
/// </summary>
public class EndingUIController : MonoBehaviour
{
    [Header("Ending UI Objects (set inactive by default in scene)")]
    [SerializeField] private GameObject happyEndingUI;
    [SerializeField] private GameObject momSadUI;
    [SerializeField] private GameObject sunBurntUI;

    [Header("Animator trigger name on each UI")]
    [SerializeField] private string animTrigger = "Play";

    void Start()
    {
        string target = ManagerScene.PendingTargetUI;

        if (string.IsNullOrEmpty(target))
        {
            Debug.LogWarning("[EndingUIController] No target UI was set.");
            return;
        }

        Debug.Log($"[EndingUIController] Activating UI: {target}");

        GameObject uiToShow = target switch
        {
            "HappyEnding" => happyEndingUI,
            "MonSad"      => momSadUI,
            "SunBurnt"    => sunBurntUI,
            _             => null
        };

        if (uiToShow == null)
        {
            Debug.LogWarning($"[EndingUIController] No GameObject assigned for: '{target}'");
            return;
        }

        uiToShow.SetActive(true);

        
        Animator anim = uiToShow.GetComponentInChildren<Animator>();
        if (anim != null)
            anim.SetTrigger(animTrigger);
        else
            Debug.LogWarning($"[EndingUIController] No Animator on: {uiToShow.name}");

        // Clear so it doesn't re-trigger if scene reloads
        ManagerScene.PendingTargetUI = "";
    }

    [ContextMenu("StartGoodEnding")]
    public void StartGoodEnding() {

        string target = "HappyEnding";

        if (string.IsNullOrEmpty(target))
        {
            Debug.LogWarning("[EndingUIController] No target UI was set.");
            return;
        }

        Debug.Log($"[EndingUIController] Activating UI: {target}");

        GameObject uiToShow = target switch
        {
            "HappyEnding" => happyEndingUI,
            "MonSad" => momSadUI,
            "SunBurnt" => sunBurntUI,
            _ => null
        };

        if (uiToShow == null)
        {
            Debug.LogWarning($"[EndingUIController] No GameObject assigned for: '{target}'");
            return;
        }

        uiToShow.SetActive(true);


        Animator anim = uiToShow.GetComponentInChildren<Animator>();
        if (anim != null)
            anim.SetTrigger(animTrigger);
        else
            Debug.LogWarning($"[EndingUIController] No Animator on: {uiToShow.name}");

        // Clear so it doesn't re-trigger if scene reloads
        ManagerScene.PendingTargetUI = "";
    }

    [ContextMenu("MomSadEnd")]
    public void MomSadEnd()
    {

        string target = "MonSad";

        if (string.IsNullOrEmpty(target))
        {
            Debug.LogWarning("[EndingUIController] No target UI was set.");
            return;
        }

        Debug.Log($"[EndingUIController] Activating UI: {target}");

        GameObject uiToShow = target switch
        {
            "HappyEnding" => happyEndingUI,
            "MonSad" => momSadUI,
            "SunBurnt" => sunBurntUI,
            _ => null
        };

        if (uiToShow == null)
        {
            Debug.LogWarning($"[EndingUIController] No GameObject assigned for: '{target}'");
            return;
        }

        uiToShow.SetActive(true);


        Animator anim = uiToShow.GetComponentInChildren<Animator>();
        if (anim != null)
            anim.SetTrigger(animTrigger);
        else
            Debug.LogWarning($"[EndingUIController] No Animator on: {uiToShow.name}");

        // Clear so it doesn't re-trigger if scene reloads
        ManagerScene.PendingTargetUI = "";
    }

    [ContextMenu("SunBurntEnd")]
    public void SunBurntEnd()
    {

        string target = "SunBurnt";

        if (string.IsNullOrEmpty(target))
        {
            Debug.LogWarning("[EndingUIController] No target UI was set.");
            return;
        }

        Debug.Log($"[EndingUIController] Activating UI: {target}");

        GameObject uiToShow = target switch
        {
            "HappyEnding" => happyEndingUI,
            "MonSad" => momSadUI,
            "SunBurnt" => sunBurntUI,
            _ => null
        };

        if (uiToShow == null)
        {
            Debug.LogWarning($"[EndingUIController] No GameObject assigned for: '{target}'");
            return;
        }

        uiToShow.SetActive(true);


        Animator anim = uiToShow.GetComponentInChildren<Animator>();
        if (anim != null)
            anim.SetTrigger(animTrigger);
        else
            Debug.LogWarning($"[EndingUIController] No Animator on: {uiToShow.name}");

        // Clear so it doesn't re-trigger if scene reloads
        ManagerScene.PendingTargetUI = "";
    }
}
