using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    [SerializeField]
    protected enum Difficulty { easy, normal, difficult };

    protected Difficulty diff;

    public int mapIndex = 0;

    public GameObject enemyBoss;
    public GameObject enemyCastle;
    private GameObject playerCastle;

    protected virtual void Start()
    {
        //player's choice for difficulty:
        //SetDifficulty(0);
        //map selection
        //SceneManager.LoadScene("Testing");
        if (mapIndex == 0)
        {
            LoadMap(1);
        }
    }

    public int GetDifficulty()
    {
        if (diff == Difficulty.easy)
        {
            return 0;
        }
        else if (diff == Difficulty.normal)
        {
            return 1;
        } 
        else
        {
            return 2;
        }
    }

    public void SetDifficulty(int value)
    {
        switch (value)
        {
            case 0:
                diff = Difficulty.easy;
                break;
            case 1:
                diff = Difficulty.normal;
                break;
            case 2:
                diff = Difficulty.difficult;
                break;
            default:
                diff = Difficulty.easy;
                Debug.Log("Unexpected value, difficulty will be set to EASY!");
                break;
        }
    }

    public bool WinCondition()
    {
        if (enemyCastle == null && enemyBoss == null)
        {
            enemyCastle = GameObject.Find("enemyBuild_Castle");
            enemyBoss = GameObject.Find("enemy_Boss");

            if (enemyCastle == null && enemyBoss == null)
            {
                int conqueredCount = 0;
                ResourceFrontier[] resourceFrontiers = FindObjectsOfType<ResourceFrontier>();
                foreach (ResourceFrontier rf in resourceFrontiers)
                {
                    if (rf.isConquered && !rf.isNeutral)
                    {
                        conqueredCount++;
                    }
                }

                if (conqueredCount == resourceFrontiers.Length)
                {
                    return true; //player has won the game
                }
            }
        }

        return false;
    }

    public bool LoseCondition()
    {
        if (playerCastle == null)
        {
            playerCastle = GameObject.Find("playerBuild_Castle");

            if (playerCastle == null)
            {
                return true;
            }
        }

        return false;
    }

    public void LoadMap(int index)
    {
        if(index > 0 && index < 5)
        {
            if(index == 2)
            {
                SetDifficulty(0);
            }
            else if (index == 3)
            {
                SetDifficulty(1);
            }
            else
            {
                SetDifficulty(2);
            }

            Time.timeScale = 1f;

            enemyBoss = null;
            enemyCastle = null;
            playerCastle = null;

            SceneManager.LoadScene(index);
            mapIndex = index;
        }
        else
        {
            Debug.Log("Wrong index!");
        }
    }

}
