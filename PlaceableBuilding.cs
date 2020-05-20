using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceableBuilding : MonoBehaviour
{
    [HideInInspector]
    public List<Collider> colliders = new List<Collider>();

    public Material greenMat;
    public Material redMat;
    public Material[] buildingMaterials;
    public Material[] highlightedMaterials;
    public GameObject flagPoint;
    public GameObject[] fireEffect;
    public GameObject props;

    public float requiredStone;
    public float destroyStone;
    public float repairStone;
    public NodeManager.ResourceTypes resourceType;
    public bool isDropOff;
    public bool isSpawnBuilding;
    public float populationRate;
    private Renderer buildingRenderer;
    private ObjectInfo buildingInfo;
    public bool isPlaced;

    private InputManager IM;
    private bool isBurning = false;

    private void Start()
    {
        StartCoroutine(ToggleFireEffect());
        buildingRenderer = GetComponent<Renderer>();
        buildingInfo = GetComponent<ObjectInfo>();
        IM = FindObjectOfType<InputManager>();

        AudioSourceManager sourceManager = gameObject.AddComponent<AudioSourceManager>();
    }

    private void Update()
    {
        if (BuildingPlacement.buildingProcess && !isPlaced)
        {
            if (buildingRenderer.materials.Length <= 1)
            {
                if (colliders.Count > 0 || transform.position.y > 2.5f)
                {
                    buildingRenderer.material = redMat;
                }
                else
                {
                    buildingRenderer.material = greenMat;
                }
            }
            else
            {
                if (colliders.Count > 0 || transform.position.y > 2.5f)
                {
                    Material[] redMaterials = new Material[buildingRenderer.materials.Length];
                    for (int i = 0; i < redMaterials.Length; i++)
                    {
                        redMaterials[i] = redMat;
                    }
                    buildingRenderer.materials = redMaterials;
                }
                else
                {
                    Material[] greenMaterials = new Material[buildingRenderer.materials.Length];
                    for (int i = 0; i < greenMaterials.Length; i++)
                    {
                        greenMaterials[i] = greenMat;
                    }
                    buildingRenderer.materials = greenMaterials;
                }
            }
        }

        if(!BuildingPlacement.buildingProcess && isPlaced && isSpawnBuilding)
        {
            flagPoint.SetActive(buildingInfo.isSelected);
            if (Input.GetMouseButtonDown(1) && buildingInfo.isSelected)
            {
                RightClick();
            }
        }

        if(isPlaced && buildingInfo.isSelected)
        {
            OnMouseExit();
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (c.tag == "Selectable" || c.tag == "Plant" || c.tag == "Resource" || c.tag == "Barrel")
            {
                colliders.Add(c);
            }
        }

    }

    void OnTriggerExit(Collider c)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (c.tag == "Selectable" || c.tag == "Plant" || c.tag == "Resource" || c.tag == "Barrel")
            {
                colliders.Remove(c);
            }
        }
    }

    void OnMouseOver()
    {
        if (isPlaced && !buildingInfo.isSelected && !BuildingPlacement.buildingProcess)
        {
            buildingRenderer.materials = highlightedMaterials;
        }
    }

    void OnMouseExit()
    {
        buildingRenderer.materials = buildingMaterials;
    }

    public void PlaceBuilding()
    {
        Material[] tmpMats = new Material[buildingRenderer.materials.Length];
        for (int i = 0; i < tmpMats.Length; i++)
        {
            tmpMats[i] = buildingMaterials[i];
        }
        buildingRenderer.materials = tmpMats;

        if(props != null)
        {
            props.SetActive(true);
        }

        isPlaced = true;
        IM.playerBuildings.Add(this.gameObject);

        AudioManager.Instance.PlayMessageSound("ConstructionComplete");
        AudioManager.Instance.PlayBuildingSound(buildingInfo.objectName, transform.position);
    }

    public void RightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 500))
        {
            if (hit.collider.tag == "Ground")
            {
                if (hit.point.y <= 4)
                {
                    flagPoint.transform.position = hit.point;
                    AudioManager.Instance.PlayMessageSound("FlagPoint");
                    AudioManager.Instance.PlayMessageSound("ConstructionComplete");
                }
            }
        }
    }

    private IEnumerator ToggleFireEffect()
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);

            if (buildingInfo.health < (buildingInfo.maxHealth / 1.35f))
            {
                if (!isBurning)
                {
                    for (int i = 0; i < fireEffect.Length; i++)
                    {
                        fireEffect[i].SetActive(true);
                    }
                    PlayFireSound();
                    isBurning = true;
                }
            }
            else
            {
                if (isBurning)
                {
                    for (int i = 0; i < fireEffect.Length; i++)
                    {
                        fireEffect[i].SetActive(false);
                    }
                    StopFireSound();
                    isBurning = false;
                }
            }
        }
    }

    private void PlayFireSound()
    {
        AudioSource aSource;
        Sound sound = AudioManager.Instance.GetSound("BuildingFire", 0);
        aSource = GetComponent<AudioSource>();
        if (sound != null)
        {
            aSource.clip = sound.clip;
            aSource.volume = sound.volume * AudioManager.Instance.levels[2];

            if (!aSource.isPlaying)
            {
                aSource.Play(); // start the sound
            }
        }
    }

    private void StopFireSound()
    {
        AudioSource aSource = GetComponent<AudioSource>();
        if (buildingInfo.objectName == ObjectList.playerBuild_Farm || buildingInfo.objectName == ObjectList.enemyBuild_Farm)
        {
            Sound sound = AudioManager.Instance.GetSound("Humans", 1);
            aSource.clip = sound.clip;
            aSource.volume = sound.volume * AudioManager.Instance.levels[2];
            if (!aSource.isPlaying)
            {
                aSource.Play(); // start the sound
            }
        }
        else
        {
            if (aSource.isPlaying)
            {
                aSource.Stop(); // stop the sound
            }
        }
    }
}
