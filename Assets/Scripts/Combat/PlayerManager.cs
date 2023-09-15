using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public GameObject knightF;
    public GameObject knightM;
    public GameObject warriorF;
    public GameObject warriorM;

    private string playerClass;
    private string playerGender;
    public GameObject start;

    public GameObject player;

    public void SetupPlayer()
    {
        SetPlayerModel();        
        start = GameObject.FindGameObjectWithTag("nonFirstLevel");
        Vector3 startpos = start.transform.position;
        GameObject playerInstance = Instantiate(player, new Vector3(startpos.x, startpos.y, startpos.z + 0.5f), Quaternion.Euler(0, 0, 0));
        playerInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
    }

    private void SetPlayerModel()
    {
        playerClass = PlayerPrefs.GetString("CLASS");
        playerGender = PlayerPrefs.GetString("GENDER");
        if (playerClass == "Knight")
        {
            if(playerGender == "Male")
            {
                player = knightM;
            }
            else
            {
                player = knightF;
            }
        }
        else if (playerClass == "Barbarian")
        {
            if (playerGender == "Male")
            {
                player = warriorM;
            }
            else
            {
                player = warriorF;
            }
        }
        else //Default is knight
        {
            if (playerGender == "Male")
            {
                player = knightM;
            }
            else
            {
                player = knightF;
            }
        }
    }
    public GameObject GetPlayerPrefab()
    {
        SetPlayerModel();        
        return player;
    }    
}
