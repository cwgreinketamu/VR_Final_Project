using System.Collections;
using TMPro;
using UnityEngine;

public class PaintingProgressManager : MonoBehaviour
{
    public TMP_Text uiText;
    public TMP_Text gameOverText;
    public GameObject gameOverBackground;
    public GameObject miniMap;
    public GameObject miniMapLabel;
    public AudioClip winSound;
    public AudioClip loseSound;

    private int count = 0;
    private int max = 0;
    private bool gameEnded = false;
    private AudioSource audioSource;

    void Start()
    {
        /* this block is gonna be real problematic if we have other ui elements in the future - finding uiText manually in editor instead
        if (uiText == null)
        {
            Transform canvas = transform.Find("Canvas");
            if (canvas != null)
            {
                Transform tmp = canvas.Find("Text (TMP)");
                if (tmp != null)
                    uiText = tmp.GetComponent<TMP_Text>();
            }
            if (uiText == null)
                uiText = GetComponentInChildren<TMP_Text>();
        }
        */

        GameObject[] viewZones = GameObject.FindGameObjectsWithTag("ViewZone");
        max = viewZones.Length;
        uiText.text = "Paintings Found: " + count + " / " + max;
        uiText.enabled = true;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (gameOverText != null)
            gameOverText.enabled = false;
        if (gameOverBackground != null)
            gameOverBackground.SetActive(false);
    }

    public void PaintingFound()
    {
        if (gameEnded) return;

        count++;
        uiText.text = "Paintings Found: " + count + " / " + max;

        if (count >= max)
            WinGame();
    }

    public void EnableUI(bool enable)
    {
        uiText.enabled = enable;
        miniMap.SetActive(enable);
        miniMapLabel.SetActive(enable);
    }

    public void WinGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        uiText.text = "Paintings Found: " + count + " / " + max;

        if (gameOverText != null)
        {
            gameOverText.text = "You Win!\nAll Paintings Found!";
            gameOverText.color = new Color(0.2f, 1f, 0.2f);
        }
        if (gameOverBackground != null)
            StartCoroutine(ShowWinScreen());

        if (winSound != null)
            audioSource.PlayOneShot(winSound);

        GameTimerManager timer = FindFirstObjectByType<GameTimerManager>();
        if (timer != null)
            timer.StopTimer();
    }

    IEnumerator ShowWinScreen()
    {
        gameOverText.enabled = true;
        gameOverBackground.SetActive(true);
        yield return new WaitForSeconds(5);
        gameOverBackground.SetActive(false);
        gameOverText.enabled = false;
    }
    public void LoseGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (gameOverText != null)
        {
            gameOverText.enabled = true;
            gameOverText.text = "Time's Up!\n" + count + " / " + max + " Paintings Found";
            gameOverText.color = new Color(1f, 0.2f, 0.2f);
        }
        if (gameOverBackground != null)
            gameOverBackground.SetActive(true);

        if (loseSound != null)
            audioSource.PlayOneShot(loseSound);
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }
}
