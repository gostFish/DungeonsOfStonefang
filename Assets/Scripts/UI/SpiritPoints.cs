using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiritPoints : MonoBehaviour
{
    //UI obejcts
    public Text TokensRemaining;

    public Text STRText;
    public Text DEXText;
    public Text INTText;
    public Text CONText;
    public Text LCKText;

    //Variables
    public int SpiritTokens;

    public int STR;
    public int DEX;
    public int INT;
    public int CON;
    public int LCK;

    public void Start()
    {
        STR = 0;
        DEX = 0;
        INT = 0;
        CON = 0;
        LCK = 0;

        SpiritTokens = 5;
        int rand = Random.Range(0,100);
        if (rand > 85)  //15% chance
        {
            SpiritTokens = SpiritTokens + 5;
        }
        else if (rand <= 85 || rand > 67) //18% chance
        {
            SpiritTokens = SpiritTokens + 4;
        }
        else if (rand <= 65 || rand > 45)//22% chance
        {
            SpiritTokens = SpiritTokens + 3;
        }
        else if (rand <= 45 || rand > 23)//22% chance
        {
            SpiritTokens = SpiritTokens + 2;
        }
        else if (rand <= 23 || rand > 5)//18% chance
        {
            SpiritTokens = SpiritTokens + 1;
        }

        //5% chance no bonus tokens
        TokensRemaining.text = "Spirit Points Remaining: " + SpiritTokens;

        //Reset tokens
        PlayerPrefs.SetInt("STR", 0);
        PlayerPrefs.SetInt("DEX", 0);
        PlayerPrefs.SetInt("INT", 0);
        PlayerPrefs.SetInt("CON", 0);
        PlayerPrefs.SetInt("LCK", 0);

    }


    public void AddSTR()
    {
        if(SpiritTokens > 0)
        {
            STR = STR + 1;
            SpiritTokens = SpiritTokens - 1;
            STRText.text = "STR + " + STR;
        }
        TokensRemaining.text = "Spirit Points Remaining: " + SpiritTokens;        
    }
    public void RemoveSTR()
    {
        if (STR > 0)
        {
            STR = STR - 1;
            SpiritTokens = SpiritTokens + 1;
            STRText.text = "STR + " + STR;
        }
        TokensRemaining.text = "Spirit Points Remaining: " + SpiritTokens;
    }
    public void AddDEX()
    {
        if (SpiritTokens > 0)
        {
            DEX = DEX + 1;
            SpiritTokens = SpiritTokens - 1;
            DEXText.text = "DEX + " + DEX;
        }
        TokensRemaining.text = "Spirit Points Remaining: " + SpiritTokens;
    }
    public void RemoveDEX()
    {
        if (DEX > 0)
        {
            DEX = DEX - 1;
            SpiritTokens = SpiritTokens + 1;
            DEXText.text = "DEX + " + DEX;
        }
        TokensRemaining.text = "Spirit Points Remaining: " + SpiritTokens;
    }
    public void AddINT()
    {
        if (SpiritTokens > 0)
        {
            INT = INT+ 1;
            SpiritTokens = SpiritTokens - 1;
            INTText.text = "INT + " + INT;
        }
        TokensRemaining.text = "Spirit Points Remaining: " + SpiritTokens;
    }
    public void RemoveINT()
    {
        if (INT > 0)
        {
            INT = INT - 1;
            SpiritTokens = SpiritTokens + 1;
            INTText.text = "INT + " + INT;
        }
        TokensRemaining.text = "Spirit Points Remaining: " + SpiritTokens;
    }
    public void AddCON()
    {
        if (SpiritTokens > 0)
        {
            CON = CON + 1;
            SpiritTokens = SpiritTokens - 1;
            CONText.text = "CON + " + CON;
        }
        TokensRemaining.text = "Spirit Points Remaining: " + SpiritTokens;
    }
    public void RemoveCON()
    {
        if (CON > 0)
        {
            CON = CON - 1;
            SpiritTokens = SpiritTokens + 1;
            CONText.text = "CON + " + CON;
        }
        TokensRemaining.text = "Spirit Points Remaining: " + SpiritTokens;
    }
    public void AddLCK()
    {
        if (SpiritTokens > 0)
        {
            LCK = LCK + 1;
            SpiritTokens = SpiritTokens - 1;
            LCKText.text = "LCK + " + LCK;
        }
        TokensRemaining.text = "Spirit Points Remaining: " + SpiritTokens;
    }
    public void RemoveLCK()
    {
        if (LCK > 0)
        {
            LCK = LCK - 1;
            SpiritTokens = SpiritTokens + 1;
            LCKText.text = "LCK + " + LCK;
        }
        TokensRemaining.text = "Spirit Points Remaining: " + SpiritTokens;
    }
}
