using UnityEngine;

public class WaterStreamSelector : MonoBehaviour
{
    public WaterStreamHaptic myStream;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GrottoGameManager gm = FindFirstObjectByType<GrottoGameManager>();
        if (gm != null && gm.currentPhase == GrottoGameManager.Phase.WaterStreams)
            gm.OnWaterStreamSelected(myStream);
    }
}
