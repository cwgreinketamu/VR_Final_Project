using System.Collections;
using TMPro;
using UnityEngine;

public class PaintingViewZoneScript : MonoBehaviour
{
    public GameObject paintingManager;
    private PaintingProgressManager managerScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        paintingManager = GameObject.Find("PaintingProgressManager");
        managerScript = paintingManager.GetComponent<PaintingProgressManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            managerScript.PaintingFound();
            GetComponent<AudioSource>().Play();
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;

            HapticManager hm = HapticManager.Instance;
            if (hm != null)
                hm.SendConfirmation();

            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
        gameObject.SetActive(false);
    }
}
