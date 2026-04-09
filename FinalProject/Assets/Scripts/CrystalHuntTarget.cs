using UnityEngine;

public class CrystalHuntTarget : MonoBehaviour
{
    public AudioSource collectSound;
    public Light glowLight;
    public bool isCollected = false;

    private float bobSpeed = 1.5f;
    private float bobHeight = 0.15f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        if (glowLight == null)
            glowLight = GetComponentInChildren<Light>();

        GetComponent<MeshRenderer>().enabled = false;
        glowLight.enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    void Update()
    {
        if (isCollected) return;
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    public void Activate()
    {
        GetComponent<Collider>().enabled = true;
    }
    void OnTriggerEnter(Collider other)
    {
        GrottoGameManager gm = FindFirstObjectByType<GrottoGameManager>();
        if (gm.currentPhase != GrottoGameManager.Phase.CrystalHunt) return;
        if (isCollected || !other.CompareTag("Player")) return;

        isCollected = true;

        HapticManager hm = HapticManager.Instance;
        if (hm != null)
        {
            hm.SendConfirmation();
            hm.SendImpulse(0.5f, 0.15f);
        }

        if (collectSound != null)
            collectSound.Play();

        if (glowLight != null)
            glowLight.intensity *= 3f;

        if (gm != null)
            gm.OnCrystalCollected();

        GetComponent<MeshRenderer>().enabled = true;
        glowLight.enabled = true;

    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
