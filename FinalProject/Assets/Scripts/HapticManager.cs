using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance { get; private set; }

    public HapticImpulsePlayer leftHaptic;
    public HapticImpulsePlayer rightHaptic;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (leftHaptic == null || rightHaptic == null)
            FindControllers();
    }

    void FindControllers()
    {
        HapticImpulsePlayer[] players = FindObjectsByType<HapticImpulsePlayer>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            if (p.gameObject.name.Contains("Left"))
                leftHaptic = p;
            else if (p.gameObject.name.Contains("Right"))
                rightHaptic = p;
        }
    }

    public void SendImpulse(float amplitude, float duration, bool left = true, bool right = true)
    {
        if (left && leftHaptic != null)
            leftHaptic.SendHapticImpulse(amplitude, duration);
        if (right && rightHaptic != null)
            rightHaptic.SendHapticImpulse(amplitude, duration);
    }

    public void SendConfirmation()
    {
        StartCoroutine(ConfirmationPattern());
    }

    IEnumerator ConfirmationPattern()
    {
        SendImpulse(0.6f, 0.08f);
        yield return new WaitForSeconds(0.15f);
        SendImpulse(0.6f, 0.08f);
    }

    public void SendWarning()
    {
        StartCoroutine(WarningPattern());
    }

    IEnumerator WarningPattern()
    {
        for (int i = 0; i < 3; i++)
        {
            SendImpulse(0.9f, 0.05f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SendGrab()
    {
        SendImpulse(0.8f, 0.1f);
    }

    public void SendSoftTap()
    {
        SendImpulse(0.3f, 0.05f);
    }

    public void SendDirectional(Vector3 playerForward, Vector3 playerRight, Vector3 dirToTarget)
    {
        float dot = Vector3.Dot(playerRight, dirToTarget.normalized);
        float forwardDot = Vector3.Dot(playerForward, dirToTarget.normalized);

        if (forwardDot > 0.7f)
        {
            SendImpulse(0.4f, 0.05f, true, true);
        }
        else if (dot > 0.2f)
        {
            SendImpulse(0.5f, 0.08f, false, true);
        }
        else if (dot < -0.2f)
        {
            SendImpulse(0.5f, 0.08f, true, false);
        }
    }

    public Coroutine StartContinuousTexture(string textureType)
    {
        return StartCoroutine(TextureCoroutine(textureType));
    }

    public void StopContinuousTexture(Coroutine co)
    {
        if (co != null)
            StopCoroutine(co);
    }

    IEnumerator TextureCoroutine(string textureType)
    {
        while (true)
        {
            switch (textureType)
            {
                case "smooth":
                    SendImpulse(0.2f, 0.1f);
                    yield return new WaitForSeconds(0.12f);
                    break;
                case "rough":
                    SendImpulse(Random.Range(0.3f, 0.6f), Random.Range(0.02f, 0.06f));
                    yield return new WaitForSeconds(Random.Range(0.03f, 0.08f));
                    break;
                case "heavy":
                    SendImpulse(0.7f, 0.15f);
                    yield return new WaitForSeconds(0.2f);
                    SendImpulse(0.3f, 0.1f);
                    yield return new WaitForSeconds(0.15f);
                    break;
                default:
                    yield return new WaitForSeconds(0.1f);
                    break;
            }
        }
    }

    public Coroutine StartProximityGuidance(Transform target, Transform player)
    {
        return StartCoroutine(ProximityCoroutine(target, player));
    }

    IEnumerator ProximityCoroutine(Transform target, Transform player)
    {
        while (target != null && target.gameObject.activeSelf)
        {
            float dist = Vector3.Distance(player.position, target.position);
            float maxDist = 15f;
            float t = 1f - Mathf.Clamp01(dist / maxDist);

            float amplitude = Mathf.Lerp(0.05f, 0.8f, t);
            float interval = Mathf.Lerp(0.6f, 0.08f, t);

            Vector3 dir = (target.position - player.position).normalized;
            SendDirectional(player.forward, player.right, dir);
            SendImpulse(amplitude, 0.05f);

            yield return new WaitForSeconds(interval);
        }
    }
}
