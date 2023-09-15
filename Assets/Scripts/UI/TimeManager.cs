using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimeManager : MonoBehaviour
{
    //Objects
    public Text currentTime;

    //Variables
    private float timeHours;
    private float timeMinutes;
    private int day;
    
    private string lightLabel;

    private void Start()
    {

        timeHours = PlayerPrefs.GetFloat("Hours");
        timeMinutes = PlayerPrefs.GetFloat("Minutes");
        day = PlayerPrefs.GetInt("Day");
        UpdateTime(0, 0);
    }

    public void UpdateTime(float hours, float minutes)
    {
        timeHours += hours;
        timeMinutes += minutes;

        if (timeMinutes >= 60)
        {
            timeMinutes -= 60;
            timeHours += 1;
        }
        if (timeHours >= 24)
        {
            timeHours -= 24;
            day += 1;            
        }

        //Ensure 2 digits for hours and minutes
        lightLabel = "";
        if (hours < 5 || hours > 18)
        {
            lightLabel = "(Night) ";
        }
        else
        {
            lightLabel = "(Day) ";
        }

        if (timeHours < 10 && timeMinutes < 10)
        {
            currentTime.text = lightLabel + " Day " + day + " Current Time (0" + timeHours + ":0" + timeMinutes + ")";
        }
        else if (timeHours < 10)
        {
            currentTime.text = lightLabel + " Day " + day + " Current Time (0" + timeHours + ":" + timeMinutes + ")";
        }
        else if(timeMinutes < 10)
        {
            currentTime.text = lightLabel + " Day " + day + " Current Time (" + timeHours + ":0" + timeMinutes + ")";
        }
        else
        {
            currentTime.text = lightLabel + " Day " + day + " Current Time (" + timeHours + ":" + timeMinutes + ")";
        }
        
        PlayerPrefs.SetFloat("Hours", timeHours);
        PlayerPrefs.SetFloat("Minutes", timeMinutes);
        PlayerPrefs.SetInt("Day", day);
    }
}
