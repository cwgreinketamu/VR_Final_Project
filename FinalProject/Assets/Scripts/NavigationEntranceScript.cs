using UnityEngine;

public class NavigationEntranceScript : MonoBehaviour
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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.transform.position.z > transform.position.z)
            {
                managerScript.EnableUI(true);
            }
            else
            {
                managerScript.EnableUI(false);
            }
        }
    }
}
