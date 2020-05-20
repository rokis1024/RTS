using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public ObjectInfo primary; //The object information to be displayed
    public ButtonSystem buttonSystem;

    private Slider UHB; //The slider object that acts as a health bar
    private Slider CastleHP;

    private GameObject player;

    private Text unitName;
    private Text HealthDisp; //The unit health text object
    private Text ATKDisp; //The unit physical attack text object
    private Text DEFDisp; //The unit physical defense text object
    private Image ATKType;
    private Image RankType;
    private Text FrontierCnt;
    private Color FrontierCntTextColor;

    private Image unitImage;

    public Button[] buttons;
    public Sprite[] playerUnitImages;
    public Sprite[] playerBuildingImages;
    public Sprite[] enemyUnitImages;
    public Sprite[] enemyBuildingImages;
    public Sprite[] damageTypes;

    private ObjectInfo castleInfo;
    private InputManager IM;

    // Use this for initialization
    void Start()
    {
        //---------------------------------Components
        IM = FindObjectOfType<InputManager>();
        buttonSystem = FindObjectOfType<ButtonSystem>();
        player = GameObject.FindGameObjectWithTag("Player");
        UHB = GameObject.Find("Health").GetComponent<Slider>(); //Assigns the UHB object
        CastleHP = GameObject.Find("CastleHealth").GetComponent<Slider>();
        unitName = GameObject.Find("UnitName").GetComponent<Text>(); //Assigns the unitNameDisp object
        HealthDisp = GameObject.Find("HealthDisp").GetComponent<Text>(); //Assigns the unitHealthDisp object
        ATKDisp = GameObject.Find("ATKDisp").GetComponent<Text>(); //Assigns the upatkDisp object
        DEFDisp = GameObject.Find("DEFDisp").GetComponent<Text>(); //Assigns the updefDisp object
        ATKType = GameObject.Find("ATKType").GetComponent<Image>();
        RankType = GameObject.Find("RankType").GetComponent<Image>();
        FrontierCnt = GameObject.Find("FrontierCount").GetComponent<Text>();
        FrontierCntTextColor = FrontierCnt.color;
        unitImage = GameObject.Find("UnitImage").GetComponent<Image>();
        //-----------------------------------------------------
        buttons = new Button[9];
        buttons = buttonSystem.AddButtons();
        //-----------------------------------------------------
        if (IM.playerBuildings[0] != null)
        {
            castleInfo = IM.playerBuildings[0].GetComponent<ObjectInfo>();

            if (castleInfo.objectName == ObjectList.playerBuild_Castle)
            {
                CastleHP.maxValue = castleInfo.maxHealth;
                CastleHP.value = castleInfo.health;
            }
        }
        //-----------------------------------------------------
    }

    // Update is called once per frame
    void Update()
    {
        if (IM.selectedInfo != null)
        {
            primary = IM.selectedInfo;
            if (primary.isUnit)
            {
                if (RankType.enabled == false)
                {
                    RankType.enabled = true;
                }

                if (ATKType.enabled == false)
                {
                    ATKType.enabled = true;
                }

                ManageATKTypeDisplay(primary.damageType);
            }
            else
            {
                if (RankType.enabled == true)
                {
                    RankType.enabled = false;
                }

                if (primary.objectName != ObjectList.building_Frontier)
                {
                    if (ATKType.enabled == true)
                    {
                        ATKType.enabled = false;
                    }
                }
                else
                {
                    if (ATKType.enabled == false)
                    {
                        ATKType.enabled = true;
                    }
                    ManageATKTypeDisplay(primary.damageType);
                }
            }

            if (primary.isPlayerObject)
            {
                PlayerObjPanelImage();
            }
            else
            {
                EnemyObjPanelImage();
            }

            DisplayObjPanelInfo();
        }
    }

    private void PlayerObjPanelImage()
    {
        unitImage.preserveAspect = true;
        if (primary.isUnit)
        {
            switch (primary.objectName)
            {
                case ObjectList.player_Worker:
                    unitImage.sprite = playerUnitImages[0];

                    for (int i = 0; i < 8; i++)
                    {
                        buttons[i].image.enabled = true;
                        if (i == 0 || i > 3)
                        {
                            buttons[i].image.sprite = playerBuildingImages[i];
                        }
                    }
                    //some real spaghetti
                    buttons[1].image.sprite = playerBuildingImages[2];//!
                    buttons[2].image.sprite = playerBuildingImages[3];//!
                    buttons[3].image.sprite = playerBuildingImages[1];//!
                    buttons[8].image.enabled = false;

                    break;
                case ObjectList.player_Recruit:
                    unitImage.sprite = playerUnitImages[1];
                    break;
                case ObjectList.player_Soldier:
                    unitImage.sprite = playerUnitImages[2];
                    break;
                case ObjectList.player_Macefighter:
                    unitImage.sprite = playerUnitImages[3];
                    break;
                case ObjectList.player_Crossbowman:
                    unitImage.sprite = playerUnitImages[4];
                    break;
                case ObjectList.player_Pikeman:
                    unitImage.sprite = playerUnitImages[5];
                    break;
                default:
                    Debug.Log("Unexpected value!");
                    break;
            }
        }
        else
        {
            if (primary.objectName != ObjectList.playerBuild_Castle)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (i < 2)
                    {
                        buttons[i].image.enabled = true;
                        buttons[i].image.sprite = playerBuildingImages[i + 8];
                    }
                    else
                    {
                        buttons[i].image.enabled = false;
                    }
                }
            }

            switch (primary.objectName)
            {
                case ObjectList.playerBuild_Castle:
                    unitImage.sprite = playerBuildingImages[0];

                    for (int i = 0; i < 6; i++)
                    {
                        buttons[i].image.enabled = true;
                        buttons[i].image.sprite = playerUnitImages[i];
                    }

                    buttons[6].image.sprite = playerBuildingImages[9];

                    buttons[7].image.enabled = false;
                    buttons[8].image.enabled = false;

                    break;
                case ObjectList.playerBuild_Mine:
                    unitImage.sprite = playerBuildingImages[1];
                    break;
                case ObjectList.playerBuild_Smeltery:
                    unitImage.sprite = playerBuildingImages[2];
                    break;
                case ObjectList.playerBuild_Stonemason:
                    unitImage.sprite = playerBuildingImages[3];
                    break;
                case ObjectList.playerBuild_Farm:
                    unitImage.sprite = playerBuildingImages[4];
                    break;
                case ObjectList.playerBuild_Blacksmith:
                    unitImage.sprite = playerBuildingImages[5];
                    break;
                case ObjectList.playerBuild_ShootingRange:
                    unitImage.sprite = playerBuildingImages[6];
                    break;
                case ObjectList.playerBuild_Barracks:
                    unitImage.sprite = playerBuildingImages[7];
                    break;
                default:
                    Debug.Log("Unexpected value!");
                    break;
            }
        }
    }

    private void EnemyObjPanelImage()
    {
        unitImage.preserveAspect = true;
        if (primary.isUnit)
        {
            switch (primary.objectName)
            {
                case ObjectList.enemy_Soldier:
                    unitImage.sprite = enemyUnitImages[0];
                    break;
                case ObjectList.enemy_Scout:
                    unitImage.sprite = enemyUnitImages[1];
                    break;
                case ObjectList.enemy_Boss:
                    unitImage.sprite = enemyUnitImages[2];
                    break;
                case ObjectList.enemy_Maceman:
                    unitImage.sprite = enemyUnitImages[3];
                    break;
                default:
                    Debug.Log("Unexpected value!");
                    break;
            }
        }
        else
        {
            switch (primary.objectName)
            {
                case ObjectList.enemyBuild_Castle:
                    unitImage.sprite = enemyBuildingImages[0];
                    break;
                case ObjectList.enemyBuild_House:
                    unitImage.sprite = enemyBuildingImages[1];
                    break;
                case ObjectList.enemyBuild_Farm:
                    unitImage.sprite = enemyBuildingImages[2];
                    break;
                case ObjectList.building_Frontier:
                    unitImage.sprite = enemyBuildingImages[3];
                    break;
                default:
                    Debug.Log("Unexpected value!");
                    break;
            }
        }
    }

    private void DisplayObjPanelInfo()
    {
        UHB.maxValue = primary.maxHealth; 
        UHB.value = primary.health; 

        unitName.text = primary.GUIname; 
        HealthDisp.text = primary.health + "/" + primary.maxHealth; 

        DEFDisp.enabled = true;

        if (primary.isUnit)
        {
            DEFDisp.text = "" + primary.def + "\n" + primary.rank;
        }
        else 
        {
            DEFDisp.text = "" + primary.def + "\n";
        }

        if (primary.isUnit || primary.objectName == ObjectList.building_Frontier)
        {
            ATKDisp.enabled = true;
            ATKDisp.text = "" + primary.atk;
        }
        else
        {
            ATKDisp.enabled = false;
        }
    }

    public void UpdateCastleHP()
    {
        if (castleInfo.objectName == ObjectList.playerBuild_Castle)
        {
            CastleHP.maxValue = castleInfo.maxHealth;
            CastleHP.value = castleInfo.health;
        }
    }

    public int UpdateFrontierCount()
    {
        int conquered = 0;
        int total = IM.frontierTowers.Count;
        foreach(GameObject resourceFrontier in IM.frontierTowers)
        {
            ResourceFrontier frontier = resourceFrontier.GetComponent<ResourceFrontier>();
            if (frontier != null)
            {
                if (frontier.isConquered && !frontier.isNeutral)
                {
                    conquered++;
                }
            }
        }

        FrontierCnt.text = conquered + "/" + total +"\t" + "(+" + conquered*3 + ")";
        if(conquered > 0)
        {
            FrontierCnt.color = Color.green;
        }
        else
        {
            FrontierCnt.color = FrontierCntTextColor;
        }

        return conquered;
    }

    private void ManageATKTypeDisplay(DamageType dmgType)
    {
        if(dmgType == DamageType.None || dmgType == DamageType.Slash)
        {
            if (ATKType.sprite != damageTypes[0])
            {
                ATKType.sprite = damageTypes[0];
            }
        }
        else if(dmgType == DamageType.Pierce)
        {
            if (ATKType.sprite != damageTypes[1])
            {
                ATKType.sprite = damageTypes[1];
            }
        }
        else if(dmgType == DamageType.Siege)
        {
            if (ATKType.sprite != damageTypes[2])
            {
                ATKType.sprite = damageTypes[2];
            }
        }
        else
        {
            if (ATKType.sprite != damageTypes[3])
            {
                ATKType.sprite = damageTypes[3];
            }
        }
        ATKType.preserveAspect = true;
    }
}
