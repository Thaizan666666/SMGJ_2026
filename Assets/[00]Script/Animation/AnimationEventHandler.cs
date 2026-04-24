using UnityEngine;
using static ManagerSound;

public class AnimationEventHandler : MonoBehaviour
{
    // Called by Animation Event
    public void OnLoadInGame()
    {
        if (ManagerScene.Instance != null)
        {
            ManagerScene.Instance.LoadGame();
            Debug.Log("Loading Game");
        }
        else
            Debug.LogWarning("[AnimationEventHandler] ManagerScene.Instance is null.");
    }

    // ── Sound — call these from Animation Events ──────────────────────────

    // One-shot effect    string param = effect id (e.g. "JellyFish")
    public void PlaySoundEffect(string id) => PlayEffect(id);

    // BGM with default fade    string param = bgm id
    public void PlayBGMSound(string id) => PlayBGM(id);

    // Stop BGM with default fade
    public void StopBGMSound(string id) => StopBGM();

    // Start looping effect    string param = effect id
    public void StartLoopEffect(string id) => LoopEffect(id);

    // Stop looping effect    string param = effect id
    public void StopLoopSoundEffect(string id) => StopLoopEffect(id);

    // Ambient with default fade    string param = ambient id
    public void PlayAmbientSound(string id) => PlayAmbient(id);

    // Stop ambient with default fade
    public void StopAmbientSound(string id) => StopAmbient();
}