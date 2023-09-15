using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sliders : MonoBehaviour
{
    //variables

    public float defaultVisionZoom;
    public float defaultMapZoom;

    public bool mapSlider;
    public bool playerSlider;

    //objects

    private GameObject miniMap;
    private GameObject player;        

    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        if (playerSlider)
        {
            defaultVisionZoom = 6;
            slider.minValue = 3;
            slider.maxValue = 11;
            slider.value = defaultVisionZoom;
            slider.onValueChanged.AddListener((val) =>
            {               
                player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<PlayerController>().zoomLevel = val;
            }
            );
        }
        else
        {
            defaultMapZoom = 20;
            slider.minValue = 8;
            slider.maxValue = 80;
            slider.value = defaultMapZoom;
            slider.onValueChanged.AddListener((val) =>
            {                
                miniMap = GameObject.FindGameObjectWithTag("MiniMapCam");
                miniMap.GetComponent<Camera>().orthographicSize = val;
            }
            );
        }
    }
}
