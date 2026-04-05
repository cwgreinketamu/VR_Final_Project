using UnityEngine;

public class GrottoEntranceTrigger : MonoBehaviour
{
    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        GrottoGameManager gm = FindFirstObjectByType<GrottoGameManager>();
        if (gm != null)
            gm.StartGame();

        HapticManager hm = HapticManager.Instance;
        if (hm != null)
            hm.SendSoftTap();
    }
}
