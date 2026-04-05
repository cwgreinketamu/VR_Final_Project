using UnityEngine;

public class ScramblerCollectible : MonoBehaviour
{
    public float rotateSpeed = 90f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.3f;
    public AudioClip pickupSound;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        ScramblerEffect effect = FindFirstObjectByType<ScramblerEffect>();
        if (effect != null)
            effect.ActivateScramble();

        HapticManager hm = HapticManager.Instance;
        if (hm != null)
            hm.SendWarning();

        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        gameObject.SetActive(false);
    }
}
