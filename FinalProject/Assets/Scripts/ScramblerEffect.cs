using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class ScramblerEffect : MonoBehaviour
{
    public float duration = 12f;
    public ContinuousMoveProvider moveProvider;
    public GameObject minimapUI;
    public TMP_Text scrambleIndicatorText;

    private float originalMoveSpeed;
    private bool isActive = false;

    void Start()
    {
        if (scrambleIndicatorText != null)
            scrambleIndicatorText.enabled = false;
    }

    public void ActivateScramble()
    {
        if (isActive) return;
        StartCoroutine(ScrambleCoroutine());
    }

    IEnumerator ScrambleCoroutine()
    {
        isActive = true;

        if (moveProvider == null)
            moveProvider = FindFirstObjectByType<ContinuousMoveProvider>();

        if (moveProvider != null)
        {
            originalMoveSpeed = moveProvider.moveSpeed;
            moveProvider.moveSpeed = -originalMoveSpeed;
        }

        if (minimapUI != null)
            minimapUI.SetActive(false);

        if (scrambleIndicatorText != null)
        {
            scrambleIndicatorText.enabled = true;
            scrambleIndicatorText.text = "SCRAMBLED!";
            scrambleIndicatorText.color = new Color(1f, 0f, 0.5f);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (scrambleIndicatorText != null)
            {
                float remaining = duration - elapsed;
                scrambleIndicatorText.text = "SCRAMBLED! " + Mathf.CeilToInt(remaining) + "s";
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (moveProvider != null)
            moveProvider.moveSpeed = originalMoveSpeed;

        if (minimapUI != null)
            minimapUI.SetActive(true);

        if (scrambleIndicatorText != null)
            scrambleIndicatorText.enabled = false;

        isActive = false;
    }

    public bool IsScrambleActive()
    {
        return isActive;
    }
}
