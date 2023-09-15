using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{

    //UI Objects
    public GameObject labelLCK;
    public GameObject labelDEX;
    public GameObject labelSTR;
    public GameObject labelINT;
    public GameObject labelCON;

    public GameObject HealthLabel;
    public GameObject StaminaLabel;
    public GameObject ManaLabel;

    public GameObject BuffDurationLabel;
    public GameObject deathLabel;

    //variables
    private float currentHealth;
    private float currentStamina;
    private float currentMana;

    private float maxHealth;
    private float maxStamina;
    private float maxMana;

    public int mysteriousPotBoost;

    private int lckVal;
    private int dexVal;
    private int strVal;
    private int intVal;
    private int conVal;

    public int healthPotUp;
    public int staminaPotUp;
    public int manaPotUp;
    public int mysteriousPotDur;

    // Start is called before the first frame update
    void Start()
    {
        healthPotUp = 15;
        staminaPotUp = 15;
        manaPotUp = 20;
        mysteriousPotDur = 20;

        mysteriousPotBoost = 5;

        PlayerPrefs.SetFloat("Health", 1000); //Will be downsized to max
        PlayerPrefs.SetFloat("Stamina", 1000);
        PlayerPrefs.SetFloat("Mana", 1000);
        UpdateStats();
    }


    public void UpdateStats()
    {
        lckVal = PlayerPrefs.GetInt("LCK");
        dexVal = PlayerPrefs.GetInt("DEX");
        strVal = PlayerPrefs.GetInt("STR");
        intVal= PlayerPrefs.GetInt("INT");
        conVal = PlayerPrefs.GetInt("CON");

        //Player stats up

        lckVal += gameObject.transform.parent.GetComponentInChildren<Inventory>().lckUp;
        dexVal += gameObject.transform.parent.GetComponentInChildren<Inventory>().dexUp;
        strVal += gameObject.transform.parent.GetComponentInChildren<Inventory>().strUp;
        intVal += gameObject.transform.parent.GetComponentInChildren<Inventory>().intUp;
        conVal += gameObject.transform.parent.GetComponentInChildren<Inventory>().conUp;

        if (PlayerPrefs.GetInt("MystDuration") > 0)
        {
            BuffDurationLabel.GetComponent<Text>().text = "Stats Buff : " + PlayerPrefs.GetInt("MystDuration") + "turns";
            lckVal += mysteriousPotBoost;
            dexVal += mysteriousPotBoost;
            strVal += mysteriousPotBoost;
            intVal += mysteriousPotBoost;
            conVal += mysteriousPotBoost;
        }
        else
        {
            BuffDurationLabel.GetComponent<Text>().text = "";
        }

        labelLCK.GetComponent<Text>().text = "Luck: " + lckVal;
        labelDEX.GetComponent<Text>().text = "Dexterity: " + dexVal;
        labelSTR.GetComponent<Text>().text = "Strength: " + strVal;
        labelINT.GetComponent<Text>().text = "Intelligence: " + intVal;
        labelCON.GetComponent<Text>().text = "Constitution: " + conVal;

        maxHealth = 50 + (conVal * 2);
        maxStamina = 10 + dexVal + strVal + conVal;
        maxMana = 20 + (intVal * 3);

        PlayerPrefs.SetFloat("MaxHealth", maxHealth);
        PlayerPrefs.SetFloat("MaxStamina", maxStamina);
        PlayerPrefs.SetFloat("MaxMana", maxMana);

        currentHealth = PlayerPrefs.GetFloat("Health");
        currentStamina = PlayerPrefs.GetFloat("Stamina");
        currentMana = PlayerPrefs.GetFloat("Mana");

        if(currentHealth > maxHealth)
        {
            PlayerPrefs.SetFloat("Health", maxHealth);
        }
        if (currentStamina > maxStamina)
        {
            PlayerPrefs.SetFloat("Stamina", maxStamina);
        }
        if (currentMana > maxMana)
        {
            PlayerPrefs.SetFloat("Mana", maxMana);
        }

        HealthLabel.GetComponent<Text>().text = "Health: " + currentHealth.ToString("F1") + " / " + maxHealth.ToString("F1");
        StaminaLabel.GetComponent<Text>().text = "Stamina: " + currentStamina.ToString("F1") + " / " + maxStamina.ToString("F1");
        ManaLabel.GetComponent<Text>().text = "Mana: " + currentMana.ToString("F1") + " / " + maxMana.ToString("F1");
    }

    public bool UseHealthPot()
    {
        currentHealth = PlayerPrefs.GetFloat("Health");
        maxHealth = PlayerPrefs.GetFloat("MaxHealth");
        if (currentHealth < maxHealth)
        {
            currentHealth += healthPotUp;
            PlayerPrefs.SetFloat("Health", currentHealth);
            gameObject.transform.parent.GetComponentInChildren<StatsManager>().UpdateStats();
            return true;
        }
        else
        {
            gameObject.transform.parent.GetComponentInChildren<Inventory>().HealthFull();
            return false;
        }
    }
    public bool UseStaminaPot()
    {
        currentStamina = PlayerPrefs.GetFloat("Stamina");
        maxStamina = PlayerPrefs.GetFloat("MaxStamina");
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaPotUp;
            PlayerPrefs.SetFloat("CurrentStamina", currentStamina);
            gameObject.transform.parent.GetComponentInChildren<StatsManager>().UpdateStats();
            return true;
        }
        else
        {
            gameObject.transform.parent.GetComponentInChildren<Inventory>().StaminaFull();
            return false;
        }
    }
    public bool UseManaPot()
    {
        currentMana = PlayerPrefs.GetFloat("Mana");
        maxMana = PlayerPrefs.GetFloat("MaxMana");
        if (currentMana < maxMana)
        {
            currentMana += manaPotUp;
            PlayerPrefs.SetFloat("Mana", currentMana);
            gameObject.transform.parent.GetComponentInChildren<StatsManager>().UpdateStats();
            return true;
        }
        else
        {
            gameObject.transform.parent.GetComponentInChildren<Inventory>().ManaFull();
            return false;
        }
    }
    public bool UseMysteriousPot()
    {
        if (PlayerPrefs.GetFloat("MystDuration") < 1)
        {
            PlayerPrefs.SetInt("MystDuration", mysteriousPotDur);
            gameObject.transform.parent.GetComponentInChildren<StatsManager>().UpdateStats();
            return true;
        }
        else
        {
            gameObject.transform.parent.GetComponentInChildren<Inventory>().PotionInUse();
            return false;
        }
    }
    public void Death()
    {
        deathLabel.SetActive(true);
    }
}
