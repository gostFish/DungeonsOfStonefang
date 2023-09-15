using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetSpiritPoints : MonoBehaviour
{

    //UI Objects
    public GameObject confirmation;

    public Text ClassText;
    public Text StatText;

    public GameObject StartButton;
    public GameObject SpendButton;
    public GameObject BackButton;

    //Variables
    public int softMaxTokens;

    private int STR;
    private int DEX;
    private int INT;
    private int CON;
    private int LCK;

    public void Start()
    {

        //Player Prefs initially points, then become score
        STR = PlayerPrefs.GetInt("STR");
        DEX = PlayerPrefs.GetInt("DEX");
        INT = PlayerPrefs.GetInt("INT");
        CON = PlayerPrefs.GetInt("CON");
        LCK = PlayerPrefs.GetInt("LCK");

        softMaxTokens = 30;
        PlayerPrefs.SetInt("CurrentHealth",100);
        PlayerPrefs.SetInt("CurrentStamina",100);
        PlayerPrefs.SetInt("CurrentMana",100);
    }
    public void ToConfirmation()
    {
        BackButton.SetActive(false);
        SpendButton.SetActive(false);
        StartButton.SetActive(true);
        confirmation.SetActive(true);
    }
    public void CloseConfirmation()
    {
        BackButton.SetActive(true);
        SpendButton.SetActive(true);
        StartButton.SetActive(false);
        confirmation.SetActive(false);
    }
    public void Spend()
    {
        confirmation.SetActive(false);

        if (!PlayerPrefs.HasKey("CLASS"))
        {
            PlayerPrefs.SetString("CLASS","Knight");
        }

        if(PlayerPrefs.GetString("CLASS").Equals("Knight"))
        {
            SetKnight();
        }
        else if (PlayerPrefs.GetString("CLASS").Equals("Rouge"))
        {
            SetRouge();
        }
        else if (PlayerPrefs.GetString("CLASS").Equals("Sorcerer"))
        {
            SetSorcerer();
        }
        else if (PlayerPrefs.GetString("CLASS").Equals("Barbarian"))
        {
            SetBarbarian();
        }
        else if (PlayerPrefs.GetString("CLASS").Equals("Adventurer"))
        {
            SetAdventurer();
        }

        STR = PlayerPrefs.GetInt("STR");
        DEX = PlayerPrefs.GetInt("DEX");
        INT = PlayerPrefs.GetInt("INT");
        CON = PlayerPrefs.GetInt("CON");
        LCK = PlayerPrefs.GetInt("LCK");

        ClassText.text = " Your Class: " + PlayerPrefs.GetString("CLASS");
        StatText.text = " Final Stats: \n\tSTR: " + STR +
                        "\n\tDEX: " + DEX +
                        "\n\tCON: " + INT +
                        "\n\tINT: " + CON +
                        "\n\tLCK: " + LCK;

        PlayerPrefs.SetFloat("Hours", 8);
        PlayerPrefs.SetFloat("Minutes", 30);
        PlayerPrefs.SetInt("Day", 0);
    }

    private void SetKnight()
    {
            STR = Random.Range(1, 6) + 5;
            DEX = Random.Range(1, 6) + 5;
            CON = Random.Range(1, 6) + 5;
            INT = Random.Range(1, 3) + 3;
            LCK = Random.Range(1, 3) + 3;

            int over = (softMaxTokens - (STR + DEX + CON + INT + LCK));
            float ratio = (100 / 45);
            while(over < 0)            {

                //Ratio: VeryHigh: 4, High: 6, Normal: 11, Weak: 13, ignore: 1-6 (to reduce)
                //Ratio: 6 : 6 : 6 : 11 : 11 : 5 Total = 45
            
                int dice = Random.Range(0, 100);
                if (dice > (100 - (ratio * 6)))  //13.33...% chance
                {
                    STR = STR - 1;
                    over = over + 1;                
                }
                else if (dice <= (100 - (ratio * 6)) || dice > (100 -(ratio * 12))) //13.33...% chance
                {
                    DEX = DEX - 1;
                    over = over + 1;
                }
                else if (dice <= (100 -(ratio * 12)) || dice > (100 - (ratio * 18)))//13.33...% chance
                {
                    CON = CON - 1;
                    over = over + 1;
                }
                else if (dice <= (100 - (ratio * 18)) || dice > (100 - (ratio * 29)))//24.44...% chance
                {
                    INT = INT - 1;
                    over = over + 1;
                }
                else if (dice <= (100 - (ratio * 29)) || dice > (100 - (ratio * 40)))//24.44...% chance
                {
                    LCK = LCK - 1;
                    over = over + 1;
                }
                else   // remaining 11.11...% chance (forgives taking the extra away)
                {
                    over = over + 1;
                }
            }
        SaveStats(STR,DEX,CON,INT,LCK);
    }        
    private void SetRouge()
    {
        LCK = Random.Range(1, 6) + 7; //V high
        DEX = Random.Range(1, 6) + 5;//High
        STR = Random.Range(1, 3) + 3;//Normal
        INT = Random.Range(1, 3) + 3;//Normal
        CON = Random.Range(1, 3) + 1;//Weak
        
        int over = (softMaxTokens - (STR + DEX + CON + INT + LCK));

        //Ratio: 4 : 6 : 11 : 11 : 13 : 5 Total = 50
        float ratio = 100 / 50;
        while (over < 0)
        {                   
            int dice = Random.Range(0, 100);
            
            if (dice > (100 - (ratio * 4)))  
            {
                LCK = LCK - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 4)) || dice > (100 -(ratio * 10)))
            {
                DEX = DEX - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 10)) || dice > (100 - (ratio * 21)))
            {
                STR = STR - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 21)) || (100 - dice > (ratio * 32)))
            {
                INT = INT - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 32)) || (100 - dice > (ratio * 45)))
            {
                CON = CON - 1;
                over = over + 1;
            }
            else   //forgives taking the extra away
            {
                over = over + 1;
            }
        }
        SaveStats(STR, DEX, CON, INT, LCK);
    }
    private void SetSorcerer()
    {
        INT = Random.Range(1, 3) + 7;//V high
        LCK = Random.Range(1, 6) + 5; //High
        DEX = Random.Range(1, 3) + 3;//Normal
        STR = Random.Range(1, 3) + 3;//Normal        
        CON = Random.Range(1, 3) + 3;//Normal

        int over = (softMaxTokens - (STR + DEX + CON + INT + LCK));

        //Ratio: 4 : 6 : 11 : 11 : 11 : 5 Total = 48
        float ratio = 100 / 48;
        while (over < 0)
        {
            int dice = Random.Range(0, 100);

            if (dice > (100 - (ratio * 4)))
            {
                INT = INT - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 4)) || dice > (100 - (ratio * 10)))
            {
                LCK = LCK - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 10)) || dice > (100 - (ratio * 21)))
            {
                DEX = DEX - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 21)) || (100 - dice > (ratio * 32)))
            {
                STR = STR - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 32)) || (100 - dice > (ratio * 43)))
            {
                CON = CON - 1;
                over = over + 1;
            }
            else   //forgives taking the extra away
            {
                over = over + 1;
            }
        }
    SaveStats(STR, DEX, CON, INT, LCK);
    }
    private void SetBarbarian()
    {
        STR = Random.Range(1, 6) + 7;//V High
        CON = Random.Range(1, 6) + 7;//V High
        LCK = Random.Range(1, 3) + 3;//Normal
        DEX = Random.Range(1, 3) + 3;//Normal
        INT = Random.Range(1, 3) + 1;//Weak

        int over = (softMaxTokens - (STR + DEX + CON + INT + LCK));

        //Ratio: 4 : 4 : 11 : 11 : 13 : 5 Total = 48
        float ratio = 100 / 48;
        while (over < 0)
        {
            int dice = Random.Range(0, 100);

            if (dice > (100 - (ratio * 4)))
            {
                STR = STR - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 4)) || dice > (100 - (ratio * 8)))
            {
                CON = CON - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 8)) || dice > (100 - (ratio * 19)))
            {
                LCK = LCK - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 19)) || (100 - dice > (ratio * 30)))
            {
                DEX = DEX - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 30)) || (100 - dice > (ratio * 43)))
            {
                INT = INT - 1;
                over = over + 1;
            }
            else   //forgives taking the extra away
            {
                over = over + 1;
            }
        }
        SaveStats(STR, DEX, CON, INT, LCK);
    }
    private void SetAdventurer()
    {
        LCK = Random.Range(1, 3) + 3;//V High
        INT = Random.Range(1, 3) + 1;//High   
        CON = Random.Range(1, 6) + 7;//Normal
        DEX = Random.Range(1, 3) + 3;//Normal
        STR = Random.Range(1, 6) + 7;//Weak     

        int over = (softMaxTokens - (STR + DEX + CON + INT + LCK));

        //Ratio: 4 : 6 : 11 : 11 : 13 : 5 Total = 50
        float ratio = 100 / 50;
        while (over < 0)
        {
            int dice = Random.Range(0, 100);

            if (dice > (100 - (ratio * 4)))
            {
                STR = STR - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 4)) || dice > (100 - (ratio * 10)))
            {
                CON = CON - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 10)) || dice > (100 - (ratio * 21)))
            {
                LCK = LCK - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 21)) || (100 - dice > (ratio * 32)))
            {
                DEX = DEX - 1;
                over = over + 1;
            }
            else if (dice <= (100 - (ratio * 32)) || (100 - dice > (ratio * 45)))
            {
                INT = INT - 1;
                over = over + 1;
            }
            else   //forgives taking the extra away
            {
                over = over + 1;
            }
        }
        SaveStats(STR, DEX, CON, INT, LCK);
    }
    private void SaveStats(int STR, int DEX, int CON, int INT, int LCK)
    {
        int NewSTR = STR + PlayerPrefs.GetInt("STR");
        int NewDEX = DEX + PlayerPrefs.GetInt("DEX");
        int NewCON = CON + PlayerPrefs.GetInt("CON");
        int NewINT = INT + PlayerPrefs.GetInt("INT");
        int NewLCK = LCK + PlayerPrefs.GetInt("LCK");

        PlayerPrefs.SetInt("STR", NewSTR);
        PlayerPrefs.SetInt("DEX", NewDEX);
        PlayerPrefs.SetInt("CON", NewCON);
        PlayerPrefs.SetInt("INT", NewINT);
        PlayerPrefs.SetInt("LCK", NewLCK);
    }
    
    public void Back()
        {
            SceneManager.LoadScene("CharacterCreation");
        }
}
