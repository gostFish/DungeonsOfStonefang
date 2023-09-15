using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{

    public GameObject player;
    public bool opened;

    public float minOpenDistance;
    void Start()
    {
        minOpenDistance = 2f;
        player = GameObject.FindGameObjectWithTag("Player");
        opened = false;
    }


    void OnMouseDown()
    {
          if (Vector3.Distance(player.transform.position, gameObject.transform.position) < minOpenDistance)
        {
            if(player.GetComponent<PlayerController>().keys > 0 && !opened)
            player.GetComponent<PlayerController>().keys += -1; //Reduce collected key
            opened = true;
            gameObject.transform.Rotate(0, 0, -90);
        }
    }
}
