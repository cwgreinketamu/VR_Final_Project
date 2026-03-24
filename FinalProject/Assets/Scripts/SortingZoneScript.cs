using UnityEngine;

public class SortingZoneScript : MonoBehaviour
{
    public string myTag = "statue1";
    public string otherTag = "statue2";
    public SortingManager sortingManagerScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sortingManagerScript = GameObject.Find("sortingManager").GetComponent<SortingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(myTag))
        {
            sortingManagerScript.ProcessObject(true);
        }
        else if (other.CompareTag(otherTag))
        {
            sortingManagerScript.ProcessObject(false);
        }
    }
}
