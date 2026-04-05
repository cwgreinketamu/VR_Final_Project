using UnityEngine;

public class BlindObjectSelector : MonoBehaviour
{
    public HapticObject myObject;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GrottoGameManager gm = FindFirstObjectByType<GrottoGameManager>();
        if (gm != null && gm.currentPhase == GrottoGameManager.Phase.BlindIdentification)
            gm.OnBlindObjectSelected(myObject);
    }
}
