using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveLevel : MonoBehaviour
{
    private GameObject progressManager;

    public bool isNext;
    void Start()
    {
        progressManager = GameObject.FindGameObjectWithTag("ProgressManager");
    }

    public void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (isNext)
            {
                progressManager.GetComponent<ProgressManager>().UpdateLevel(true);
            }
            else
            {
                if (PlayerPrefs.GetInt("CurrentLevel") > 2)
                {
                    progressManager.GetComponent<ProgressManager>().UpdateLevel(false);
                }
            }
        }
    }
}
