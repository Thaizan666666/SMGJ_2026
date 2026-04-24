using UnityEngine;
using static ManagerSound;

public class playBGM : MonoBehaviour
{
    private void Start()
    {
        PlayBGM("InGame",1f);
        //PlayAmbient("",1f);
    }
}
