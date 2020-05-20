using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public bool hasPrimary; //Does the player have a primary selected object?

    public CanvasGroup UnitPanel; //The unit information panel
    public CanvasGroup ActionPanel;
    public ButtonSystem BM;
    private Text objInfo;

    private Vector2 boxStart; //The mouse coordinates stored when the player clicks
    private Vector2 boxEnd; //The mouse coordinates stored when the player releases the mouse button
    public bool performMulti = false;

    public GameObject primaryObject; //The primary selected game object

    private Rect selectBox; //The selection box

    public Texture boxTex; //The selection box texture

    public ObjectInfo selectedInfo; //The primary object's information

    public GameObject[] units; //An array of units
    public List<GameObject> playerUnits;
    public List<GameObject> playerBuildings;
    public List<GameObject> frontierTowers;
    private Camera cameraMain;

    void Start()
    {
        UnitPanel = GameObject.Find("UnitPanel").GetComponent<CanvasGroup>(); 
        ActionPanel = GameObject.Find("ActionPanel").GetComponent<CanvasGroup>();
        BM = FindObjectOfType<ButtonSystem>();
        objInfo = GameObject.Find("ObjInfo").GetComponent<Text>();
        cameraMain = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F1) && Input.GetKey(KeyCode.D)) //Cheat (Quick game over)
        {
            if (playerBuildings.Count > 0)
            {
                if (playerBuildings[0] != null)
                {
                    if (playerBuildings[0].GetComponent<ObjectInfo>().isAlive)
                    {
                        playerBuildings[0].GetComponent<ObjectInfo>().health = 0f;
                    }
                }
            }
        }
        else if(Input.GetKey(KeyCode.F1) && Input.GetKey(KeyCode.V)) //Cheat (Quick victory)
        {
            if(GameManager.Instance.enemyBoss != null && GameManager.Instance.enemyCastle != null)
            {
                ObjectInfo boss = GameManager.Instance.enemyBoss.GetComponent<ObjectInfo>();
                ObjectInfo castle = GameManager.Instance.enemyCastle.GetComponent<ObjectInfo>();
                if(boss.isAlive)
                {
                    boss.health = 0f;
                }
                if(castle.isAlive)
                {
                    castle.health = 0f;
                }
            }
        }

        if (primaryObject != null)
        {
            hasPrimary = true;
        }
        else
        {
            hasPrimary = false;
        }

        if (Input.GetMouseButtonDown(0) && !BuildingPlacement.buildingProcess && !IngameMenu.isPaused) //Is the player left clicking
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                {
                   LeftClick(); //Calls the LeftClick() method
                }
            }
        }


        if (Input.GetMouseButton(0) && boxStart == Vector2.zero) //Is the player holding down the left mouse button and is boxStart zero?
        {
            boxStart = Input.mousePosition; //Sets the boxStart to equal the mouse position when the player first started holding the mouse button down
        }

        if (Input.GetMouseButton(0) && boxStart != Vector2.zero) //Is the player holding down the left mouse button and is boxStart not equal to zero
        {
            boxEnd = Input.mousePosition; //Sets the boxEnd to equal the mouse position
        }

        if (Input.GetMouseButtonUp(0))
        {
            units = GameObject.FindGameObjectsWithTag("Selectable"); //Grabs all selectable objects

            MultiSelect(); //Calls the MultiSelect() method
        }

        selectBox = new Rect(boxStart.x, Screen.height - boxStart.y, boxEnd.x - boxStart.x, -1 * ((Screen.height - boxStart.y) - (Screen.height - boxEnd.y))); //Creates the selection box values

        if (primaryObject != null) //Is there a primary object?
        {
            UnitPanel.alpha = 1; //Sets the unit panel to be visible
            UnitPanel.blocksRaycasts = true; //Sets the unit panel to block raycasts
            UnitPanel.interactable = true; //Sets the unit panel to be interactable

            ObjectInfo primaryInfo = primaryObject.GetComponent<ObjectInfo>();
            if(primaryInfo.objectName == ObjectList.player_Worker || (primaryInfo.isPlayerObject && !primaryInfo.isUnit))
            {
                ActionPanel.alpha = 1; 
                ActionPanel.blocksRaycasts = true; 
                ActionPanel.interactable = true; 
            }
            else
            {
                objInfo.text = "";
                ActionPanel.alpha = 0; 
                ActionPanel.blocksRaycasts = false;
                ActionPanel.interactable = false; 
            }
        }
        else
        {
            UnitPanel.alpha = 0; 
            UnitPanel.blocksRaycasts = false; 
            UnitPanel.interactable = false; 

            ActionPanel.alpha = 0;
            ActionPanel.blocksRaycasts = false; 
            ActionPanel.interactable = false; 
        }
    }

    //Selects all units within the selection box
    public void MultiSelect()
    {
        foreach (GameObject unit in playerUnits) //For each unit in the array of selectable objects
        {
            ObjectInfo unitInfo = unit.GetComponent<ObjectInfo>();
            if (unitInfo.isUnit && unitInfo.isPlayerObject) //Is the object in question a unit?
            {

                Vector2 unitPos = cameraMain.WorldToScreenPoint(unit.transform.position); //Translate the unit's screen position into 2D coordinates

                Vector2 realPos = new Vector2(unitPos.x, Screen.height - unitPos.y);

                if (selectBox.Contains(realPos, true)) //Is the unit inside the selection box?
                {
                    if (!hasPrimary) //Is there not a primary selected object?
                    {
                        primaryObject = unit; //This unit becomes the primary object
                        unitInfo.isPrimary = true; //Sets the unit to be the primary
                    }

                    unitInfo.isSelected = true; //Sets the unit to be selected

                    selectedInfo = unit.GetComponent<ObjectInfo>();

                    //-----------------------------------------------------------------
                    if (selectedInfo.isPrimary)
                    {
                        if (selectedInfo.objectName == ObjectList.player_Worker)
                        {
                            BM.WorkerActionPanel();
                            BM.WorkerEventEntries();
                        }
                    }
                }
            }
        }

        boxStart = Vector2.zero; //Sets the boxStart to be zero
        boxEnd = Vector2.zero; //Sets the boxEnd to be zero
    }

    //Performs actions based on what the player clicks
    public void LeftClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Creates a ray from the camera to where the player clicks
        RaycastHit hit; //The object the ray hits

        if (Physics.Raycast(ray, out hit, 500)) //Does the ray hit anything?
        {
            if (hit.collider.tag == "Ground") //Did the player click the ground?
            {
                if (selectedInfo != null)
                {
                    selectedInfo.isSelected = false; //Deselects the selected units
                    selectedInfo.isPrimary = false; //Deselects the primary unit
                }
                primaryObject = null; //Clears the primary object

                units = GameObject.FindGameObjectsWithTag("Selectable"); //Grabs all selectable objects

                foreach (GameObject unit in units) //For each selectable object
                {
                    unit.GetComponent<ObjectInfo>().isSelected = false; //Deselects all selectable objects
                }

                selectedInfo = null; //Clears out the selected info
            }
            else if (hit.collider.tag == "Selectable") //Did the player click a selectable object?
            {

                units = GameObject.FindGameObjectsWithTag("Selectable"); //Grabs all selectable objects


                foreach (GameObject unit in units) //For each selectable object
                {
                    unit.GetComponent<ObjectInfo>().isSelected = false; //Deselects all selectable objects
                }

                if (hasPrimary && selectedInfo != null) //Is there a primary object?
                {
                    selectedInfo.isSelected = false; //Deselects the selected units
                    selectedInfo.isPrimary = false; //Deselects the primary unit
                    primaryObject = null; //Clears the primary object
                }

                primaryObject = hit.collider.gameObject; //Sets the primary object

                selectedInfo = primaryObject.GetComponent<ObjectInfo>(); //Sets the selected info

                if(selectedInfo.objectName == ObjectList.player_Worker)
                {
                    BM.WorkerActionPanel();
                    BM.WorkerEventEntries();
                }

                if (selectedInfo.isPlayerObject && !selectedInfo.isUnit)
                {
                    if (selectedInfo.objectName == ObjectList.playerBuild_Castle)
                    {
                        BM.CastleActionPanel(selectedInfo.gameObject);
                        BM.CastleEventEntries();
                    }
                    else
                    {
                        BM.BuildingActionPanel(selectedInfo.gameObject);
                    }
                    AudioManager.Instance.PlayBuildingSound(selectedInfo.objectName, selectedInfo.gameObject.transform.position);
                }
              

                selectedInfo.isSelected = true; //Sets the selected unit to be selected
                selectedInfo.isPrimary = true; //Sets the selected unit to be the primary unit
            }
        }
    }

    //This draws the selection box on the screen
    void OnGUI()
    {
        if (boxStart != Vector2.zero && boxEnd != Vector2.zero) //Does the selection box have any values?
        {
            GUI.DrawTexture(selectBox, boxTex); //Draws a box using the selection box values
        }
    }
}
