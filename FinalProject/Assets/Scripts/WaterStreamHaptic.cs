using System.Collections;
using UnityEngine;

public class WaterStreamHaptic : MonoBehaviour
{
    public enum StreamIntensity { Gentle, Medium, Strong }
    public StreamIntensity intensity = StreamIntensity.Gentle;
    public AudioSource waterSound;

    private Coroutine hapticCoroutine;
    private bool playerInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;

        if (waterSound != null && !waterSound.isPlaying)
            waterSound.Play();

        hapticCoroutine = StartCoroutine(WaterHapticLoop());
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;

        if (waterSound != null)
            waterSound.Stop();

        if (hapticCoroutine != null)
            StopCoroutine(hapticCoroutine);
    }

    IEnumerator WaterHapticLoop()
    {
        HapticManager hm = HapticManager.Instance;
        if (hm == null) yield break;

        while (playerInside)
        {
            switch (intensity)
            {
                case StreamIntensity.Gentle:
                    hm.SendImpulse(0.15f, 0.1f);
                    yield return new WaitForSeconds(0.15f);
                    break;
                case StreamIntensity.Medium:
                    hm.SendImpulse(0.45f, 0.08f);
                    yield return new WaitForSeconds(0.12f);
                    hm.SendImpulse(0.25f, 0.06f);
                    yield return new WaitForSeconds(0.1f);
                    break;
                case StreamIntensity.Strong:
                    hm.SendImpulse(0.85f, 0.12f);
                    yield return new WaitForSeconds(0.08f);
                    hm.SendImpulse(0.6f, 0.1f);
                    yield return new WaitForSeconds(0.06f);
                    break;
            }
        }
    }

    public float GetIntensityValue()
    {
        switch (intensity)
        {
            case StreamIntensity.Gentle: return 0.15f;
            case StreamIntensity.Medium: return 0.45f;
            case StreamIntensity.Strong: return 0.85f;
            default: return 0f;
        }
    }
}
