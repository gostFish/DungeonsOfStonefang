using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    //Variables
    private GameObject player;
    private Vector3 playerPos;
    private bool mapExists;
   
    void Start()
    {
        StartCoroutine(MakeMap());
        mapExists = false;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    IEnumerator MakeMap() //Wait for player to spawn
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player"));        
        player = GameObject.FindGameObjectWithTag("Player");
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        mapExists = true;
    }

    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (mapExists && player != null)
        {
            playerPos = player.transform.position;
            transform.position = new Vector3(playerPos.x, 20, playerPos.z);
        }
    }
}
