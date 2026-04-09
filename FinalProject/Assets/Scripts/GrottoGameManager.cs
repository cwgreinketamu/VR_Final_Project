using System.Collections;
using TMPro;
using UnityEngine;

public class GrottoGameManager : MonoBehaviour
{
    public enum Phase { Inactive, WaterStreams, BlindIdentification, CrystalHunt, Complete, Failed }
    public Phase currentPhase = Phase.Inactive;

    public TMP_Text instructionText;
    public TMP_Text timerText;
    public TMP_Text resultText;
    public GameObject resultBackground;

    public WaterStreamHaptic[] waterStreams;
    public WaterStreamHaptic correctStream;

    public HapticObject[] blindObjects;
    public Transform playerTransform;

    public CrystalHuntTarget[] crystals;

    public float totalTime = 180f;
    private float timeRemaining;
    private bool timerRunning = false;

    private int blindRound = 0;
    private int wrongAnswers = 0;
    private int crystalsCollected = 0;
    private int totalCrystals = 4;

    private HapticObject.TextureType[] blindPrompts = {
        HapticObject.TextureType.Smooth,
        HapticObject.TextureType.Heavy,
        HapticObject.TextureType.Rough
    };

    private Coroutine guidanceCoroutine;

    public AudioSource audioSource;

    void Start()
    {
        timeRemaining = totalTime;
        if (resultText != null) resultText.enabled = false;
        if (resultBackground != null) resultBackground.SetActive(false);
        if (instructionText != null) instructionText.text = "";
        if (timerText != null) timerText.enabled = false;
        audioSource = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        if (currentPhase != Phase.Inactive) return;
        timerText.enabled = true;
        timerRunning = true;
        timeRemaining = totalTime;
        wrongAnswers = 0;
        crystalsCollected = 0;
        blindRound = 0;
        StartPhase(Phase.WaterStreams);
    }

    void Update()
    {
        if (!timerRunning) return;

        timeRemaining -= Time.deltaTime;
        UpdateTimerDisplay();

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            timerRunning = false;
            EndGame(false, "Time's Up!");
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        int min = Mathf.FloorToInt(timeRemaining / 60f);
        int sec = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0}:{1:00}", min, sec);

        if (timeRemaining <= 30f)
            timerText.color = new Color(1f, 0.3f, 0.3f);
        else if (timeRemaining <= 60f)
            timerText.color = new Color(1f, 0.8f, 0.2f);
        else
            timerText.color = Color.white;
    }

    void StartPhase(Phase phase)
    {
        currentPhase = phase;

        switch (phase)
        {
            case Phase.WaterStreams:
                if (instructionText != null)
                    instructionText.text = "Touch each water stream to feel its intensity.\nThen select the STRONGEST one.";
                break;
            case Phase.BlindIdentification:
                StartBlindRound();
                break;
            case Phase.CrystalHunt:
                if (instructionText != null)
                    instructionText.text = "Find the hidden crystals!\nFollow the haptic vibrations.";
                StartCrystalGuidance();
                break;
        }
    }

    public void OnWaterStreamSelected(WaterStreamHaptic selected)
    {
        if (currentPhase != Phase.WaterStreams) return;

        HapticManager hm = HapticManager.Instance;
        if (selected == correctStream)
        {
            if (hm != null) hm.SendConfirmation();
            audioSource.Play();
            Debug.Log("correct stream selected");
            StartPhase(Phase.BlindIdentification);
        }
        else
        {
            wrongAnswers++;
            Debug.Log("wrong stream selected");
            if (hm != null) hm.SendWarning();
            if (instructionText != null)
                instructionText.text = "Wrong stream! Try again.\nTouch each stream, then pick the STRONGEST.";
            CheckFailure();
        }
    }

    void StartBlindRound()
    {
        if (blindRound >= blindPrompts.Length)
        {
            StartPhase(Phase.CrystalHunt);
            return;
        }

        string typeName = blindPrompts[blindRound].ToString().ToLower();
        if (instructionText != null)
            instructionText.text = "Blind Challenge: Touch the objects.\nSelect the " + typeName + " one.";
    }

    public void OnBlindObjectSelected(HapticObject selected)
    {
        if (currentPhase != Phase.BlindIdentification) return;
        if (blindRound >= blindPrompts.Length) return;

        HapticManager hm = HapticManager.Instance;
        selected.StopHaptic();

        if (selected.textureType == blindPrompts[blindRound])
        {
            if (hm != null) hm.SendConfirmation();
            blindRound++;
            audioSource.Play();
            Debug.Log("correct object selected");
            StartBlindRound();
        }
        else
        {
            wrongAnswers++;
            Debug.Log("wrong object selected");
            if (hm != null) hm.SendWarning();
            if (instructionText != null)
                instructionText.text = "Wrong! That was " + selected.textureType.ToString().ToLower() + ".\nTry again - select the " + blindPrompts[blindRound].ToString().ToLower() + " one.";
            CheckFailure();
        }
    }

    void StartCrystalGuidance()
    {
        CrystalHuntTarget nextCrystal = GetNextCrystal();
        if (nextCrystal == null) return;

        HapticManager hm = HapticManager.Instance;
        if (hm != null && playerTransform != null)
            guidanceCoroutine = hm.StartProximityGuidance(nextCrystal.transform, playerTransform);

        nextCrystal.Activate();
    }

    CrystalHuntTarget GetNextCrystal()
    {
        if (crystals == null) return null;
        foreach (var c in crystals)
        {
            if (c != null && !c.isCollected && c.gameObject.activeSelf)
                return c;
        }
        return null;
    }

    public void OnCrystalCollected()
    {
        crystalsCollected++;

        HapticManager hm = HapticManager.Instance;
        if (guidanceCoroutine != null && hm != null)
            hm.StopContinuousTexture(guidanceCoroutine);

        if (instructionText != null)
            instructionText.text = "Crystals: " + crystalsCollected + " / " + totalCrystals;
        audioSource.Play();
        if (crystalsCollected >= totalCrystals)
        {
            EndGame(true, "Grotto Complete!");
        }
        else
        {
            StartCrystalGuidance();
        }
    }

    void CheckFailure()
    {
        if (wrongAnswers >= 3)
            EndGame(false, "Too many wrong answers!");
    }

    void EndGame(bool won, string message)
    {
        timerRunning = false;
        timerText.enabled = false;
        currentPhase = won ? Phase.Complete : Phase.Failed;

        HapticManager hm = HapticManager.Instance;

        if (won)
        {
            if (hm != null)
            {
                hm.SendConfirmation();
                hm.SendImpulse(0.7f, 0.3f);
            }
        }
        else
        {
            if (hm != null) hm.SendWarning();
        }
        StartCoroutine(DisplayResults(won, message));
    }

    public IEnumerator DisplayResults(bool won, string message) {
        if (won)
        {
            if (resultText != null)
            {
                resultText.enabled = true;
                resultText.text = message;
                resultText.color = new Color(0.2f, 1f, 0.2f);
            }
        }
        else
        {
            if (resultText != null)
            {
                resultText.enabled = true;
                resultText.text = message;
                resultText.color = new Color(1f, 0.2f, 0.2f);
            }
        }
        if (resultBackground != null) resultBackground.SetActive(true);
        if (instructionText != null) instructionText.text = "";
        yield return new WaitForSeconds(5);
        if (resultBackground != null) resultBackground.SetActive(false);
        resultText.enabled = false;
    }
}
