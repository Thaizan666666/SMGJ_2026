using UnityEngine;

/// <summary>
/// Attach this script to the same GameObject as your Animator.
/// Then in the Animation window, add an Animation Event and select "OnLoadInGame".
/// 
/// HOW TO SET UP:
/// 1. Attach this script to the GameObject that has the Animator
/// 2. Open the Animation window (Window > Animation > Animation)
/// 3. Scrub to the frame you want the scene to load
/// 4. Click the "Add Event" button (small marker icon on the timeline)
/// 5. In the Inspector, select "OnLoadInGame" from the Function dropdown
/// </summary>
public class AnimationEventHandler : MonoBehaviour
{
    // Called by Animation Event
    public void OnLoadInGame()
    {
        if (ManagerScene.Instance != null)
        {
            ManagerScene.Instance.LoadGame();
            Debug.Log("Loding Game");
        }
        else
            Debug.LogWarning("[AnimationEventHandler] ManagerScene.Instance is null.");
    }
}
