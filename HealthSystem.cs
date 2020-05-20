using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public Slider healthBar;
    public Text damageDisp;
    public Canvas health;

    float CurrectHealth;
    float MaxHealth;
    float damage;

    private float FadeOuttimer = 4f;
    float FadeOuttimer_temp = 0;
    public bool ShowHealth = false;
    private Color fontCol;
    private Color tempCol;

    private Camera cameraMain;

    void Start()
    {
        cameraMain = Camera.main;

        FadeOuttimer_temp = FadeOuttimer;
        damageDisp.fontSize = 135;
        healthBar.gameObject.SetActive(false);
        damageDisp.gameObject.SetActive(false);
        fontCol = damageDisp.color;

        CurrectHealth = GetComponent<ObjectInfo>().health;
        MaxHealth = GetComponent<ObjectInfo>().maxHealth;
    }

    void Update()
    {

        if (FadeOuttimer == FadeOuttimer_temp)
        {
            tempCol = fontCol;
        }

        if (FadeOuttimer > 0)
        {
            FadeOuttimer -= Time.deltaTime;
            if (damageDisp.fontSize > 1)
            {
                tempCol.a -= 0.04f;
                damageDisp.color = tempCol;
            }
            if (FadeOuttimer < 0)
            {
                FadeOuttimer = 0;
                ShowHealth = false;
                healthBar.gameObject.SetActive(false);
                damageDisp.gameObject.SetActive(false);
            }
        }

        if (ShowHealth)
        {
            healthBar.gameObject.SetActive(true);
            damageDisp.gameObject.SetActive(true);
            Set_HealthBar(CurrectHealth, MaxHealth);
            Set_Damage();
        }
    }

    void LateUpdate()
    {
        if (ShowHealth)
        {
            health.transform.rotation = cameraMain.transform.rotation;
        }
    }

    public void Hit(float damage)
    {
        FadeOuttimer = FadeOuttimer_temp;
        ShowHealth = true;
        this.damage = damage;
    }

    public void SetMaxHealthValue(float value)
    {
        MaxHealth = value;
    }

    public void SetHealthValue(float value)
    {
        CurrectHealth = value;
        if(value <= 0)
        {
            CurrectHealth = 0;
        }
    }

    public void Set_Damage()
    {
        if(damage > 0)
        {
            damageDisp.text = "" + damage;
        }
    }

    public void Set_HealthBar(float CurrentHealth, float MaxHealth)
    {
        healthBar.maxValue = MaxHealth;
        healthBar.value = CurrentHealth;
    }
}
