using System.Collections;
using TMPro;
using UnityEngine;

public class SortingManager : MonoBehaviour
{
    public int totalObj = 10;
    public int totalProcessed = 0;
    public int correctSorted = 0;

    public TMP_Text gameOverText;
    public GameObject gameOverBackground;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameOverText != null)
            gameOverText.enabled = false;
        if (gameOverBackground != null)
            gameOverBackground.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ProcessObject(bool correct)
    {
        totalProcessed++;
        if (correct)
        {
            correctSorted++;
        }
        if (totalProcessed == totalObj)
        {
            StartCoroutine(DisplayPopUp());
        }
    }

    IEnumerator DisplayPopUp()
    {
        if (correctSorted >= 7)
        {
            gameOverText.text = "Sorted enough objects correctly!\nYou win!";
        }
        else
        {
            gameOverText.text = "Did not sort enough objects correctly.\nYou lose!";
        }
        gameOverText.enabled = true;
        gameOverBackground.SetActive(true);
        yield return new WaitForSeconds(5);
        gameOverBackground.SetActive(false);
        gameOverText.enabled = false;
    }
}
