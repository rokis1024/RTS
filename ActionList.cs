using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionList : MonoBehaviour
{
    private GUIManager GUI;

    void Start()
    {
        GUI = FindObjectOfType<GUIManager>(); //for castle health display
    }

    public void Move(NavMeshAgent agent, RaycastHit hit, bool formation)
    {
        if (formation) //multiple units selected
        {
            ObjectInfo unitInfo;
            int length = 0;
            Vector3 total = Vector3.zero;
            Vector3 startVector = Vector3.zero;
            GameObject[] selected = GetComponent<InputManager>().units;
            foreach (GameObject unit in selected)
            {
                if (unit != null)
                {
                    unitInfo = unit.GetComponent<ObjectInfo>();
                    if (unitInfo.isPlayerObject)
                    {
                        if (unitInfo.isSelected && unitInfo.isUnit)
                        {
                            total += unit.transform.position;
                            length++;
                        }
                    }
                }
            }

            Vector3 center = total / length;

            foreach (GameObject unit in selected)
            {
                if (unit != null)
                {
                    unitInfo = unit.GetComponent<ObjectInfo>();
                    if (unitInfo.isPlayerObject)
                    {
                        if (unitInfo.isSelected && unitInfo.isUnit)
                        {
                            startVector = unit.transform.position - center;
                            if (unit.GetComponent<NavMeshAgent>().isActiveAndEnabled)
                            {
                                unit.GetComponent<NavMeshAgent>().SetDestination(hit.point + startVector);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            agent.destination = hit.point;
        }
    }

    public void Attack(GameObject target, ObjectInfo unit)
    {
        ObjectInfo targetInfo = target.GetComponent<ObjectInfo>();
        HealthSystem HS = targetInfo.GetComponent<HealthSystem>();
        float damage = 0; //for damage display

        if (unit.damageType != DamageType.None || unit.damageType != DamageType.Projectile) //projectile damage is calculated in CannonBall.cs
        {
            float bonusDmg = 1f; //bonus damage depends on the rank of the unit, causes additional damage to the target
            if(unit.rank != ObjectInfo.Ranks.None)
            {
                if(unit.rank == ObjectInfo.Ranks.Recruit)
                {
                    bonusDmg = 1.15f; //Recruit deals 15% bonus damage
                }
                else if (unit.rank == ObjectInfo.Ranks.Elite)
                {
                    bonusDmg = 1.3f; //Elite deals 30% bonus damage
                }
                else
                {
                    bonusDmg = 1.45f; //Boss deals 45% bonus damage
                }
            }

            if (targetInfo.isUnit)
            {
                if (unit.damageType == DamageType.Slash || unit.damageType == DamageType.Siege)
                {
                    damage = Mathf.Round(bonusDmg * unit.atk * (1 - (targetInfo.def * 0.005f)));
                    targetInfo.health -= damage; //Slash and Blunt dmg types cause same damage to units
                }
                else
                {
                    if (bonusDmg < 1.2f) //make pierce dmg dealing units less OP
                    {
                        bonusDmg -= 0.1f;
                    }
                    else
                    {
                        bonusDmg -= 0.2f;
                    }
                    if (bonusDmg < 1f) bonusDmg = 1f; //bonus damage must not be less than 100%!

                    damage = Mathf.Round(bonusDmg * unit.atk);
                    targetInfo.health -= damage; //Pierce dmg type ignores unit's armor value
                }
            }
            else
            {
                if (unit.damageType == DamageType.Siege)
                {
                    damage = Mathf.Round(unit.atk * 1.45f);
                    targetInfo.health -= damage; //Blunt dmg type causes 45% more damage to buildings (ignoring their armor)
                }
                else
                {
                    damage = Mathf.Round(unit.atk * (1 - (targetInfo.def * 0.005f)));
                    targetInfo.health -= damage; //Slash and Pierce dmg types cause same damage to buildings
                }
            }
        }

        HS.Hit(damage);
        HS.SetMaxHealthValue(targetInfo.maxHealth);
        HS.SetHealthValue(targetInfo.health);

        if (targetInfo.objectName == ObjectList.playerBuild_Castle)
        {
            GUI.UpdateCastleHP();
        }
    }
}
