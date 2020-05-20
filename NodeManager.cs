using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeManager : MonoBehaviour
{
    public enum ResourceTypes { Stone, Iron, Gold }

    public ResourceTypes resourceType;

    public float availableResource;

    public int gatherers = 0;

    public Material material;

    public Material highlightedMaterial;

    private Renderer renderer;

    //------------------------------------------
    public Canvas resourcePanel;

    public Text resourceText;

    private bool showAvailableResource = false;
    private Camera cameraMain;
    //------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ResourceTick());

        resourcePanel.gameObject.SetActive(false);
        renderer = GetComponent<Renderer>();

        cameraMain = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(availableResource <= 0)
        {
            Destroy(gameObject);
        }

        if(gatherers < 0)
        {
            gatherers = 0;
        }

        if(showAvailableResource)
        {
            resourcePanel.transform.rotation = cameraMain.transform.rotation;
        }
    }

    public void ResourceGather()
    {
        if(gatherers != 0)
        {
            availableResource -= gatherers;
        }
    }

    IEnumerator ResourceTick()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);
            ResourceGather();
        }
    }

    void OnMouseOver()
    {
        showAvailableResource = true;
        resourcePanel.gameObject.SetActive(true);
        resourceText.text = resourceType + ": " + availableResource;
        renderer.material = highlightedMaterial;
    }

    void OnMouseExit()
    {
        showAvailableResource = false;
        resourcePanel.gameObject.SetActive(false);
        renderer.material = material;
    }
}
