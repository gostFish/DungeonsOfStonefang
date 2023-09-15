using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetOptions : MonoBehaviour
{
    //UI
    public InputField name;
    public Dropdown classType;
    public Dropdown gender;

    //Player models
    public GameObject KnightM;
    public GameObject KnightF;
    public GameObject WarriorM;
    public GameObject WarriorF;


    // Start is called before the first frame update
    void Start()
    {
        //Default Values
        PlayerPrefs.SetString("NAME", "");
        PlayerPrefs.SetString("CLASS", "Knight");
        PlayerPrefs.SetString("GENDER", "Male");
        PlayerPrefs.SetInt("HasStarter", 0);

        //Clear inventory data on start
        System.IO.File.Delete("Assets/Resources/SaveData/Inventory/Items.json");
        System.IO.File.Delete("Assets/Resources/SaveData/Inventory/Equipment.json");

        //Set current levels
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("NewestLevel", 1);
    }

    void Update()
    {
        if (gender.options[gender.value].text.Equals("Male"))
        {
            if (classType.options[classType.value].text.Equals("Knight"))
            {
                HideAll();
                KnightM.SetActive(true);
            }
            else if (classType.options[classType.value].text.Equals("Barbarian"))
            {
                HideAll();
                WarriorM.SetActive(true);
            }
        }
        else
        {
            if (classType.options[classType.value].text.Equals("Knight"))
            {
                HideAll();
                KnightF.SetActive(true);
            }
            else if (classType.options[classType.value].text.Equals("Barbarian"))
            {
                HideAll();
                WarriorF.SetActive(true);
            }
        }
    }

    private void HideAll()
    {
        KnightM.SetActive(false);
        KnightF.SetActive(false);
        WarriorM.SetActive(false);
        WarriorF.SetActive(false);
    }

    public void SetCharacter()
    {
        PlayerPrefs.SetString("NAME", name.text);
        PlayerPrefs.SetString("CLASS", classType.options[classType.value].text);
        PlayerPrefs.SetString("GENDER", gender.options[gender.value].text);

        SceneManager.LoadScene("SpiritPoints");
    }
}
