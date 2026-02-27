using NUnit.Framework;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class PaintingProgressManager : MonoBehaviour
{
    public TMP_Text uiText;
    private int count = 0;
    private int max = 0;
    private GameObject[] viewZones;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiText = GetComponentInChildren<TMP_Text>();
        viewZones = GameObject.FindGameObjectsWithTag("ViewZone");
        max = viewZones.Length;
        uiText.text = "Paintings Found: " + count + " / " + max;
        uiText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PaintingFound()
    {
        count++;
        uiText.text = "Paintings Found: " + count + " / " + max;
    }

    public void EnableUI(bool enable)
    {
        uiText.enabled = enable;
    }
}
