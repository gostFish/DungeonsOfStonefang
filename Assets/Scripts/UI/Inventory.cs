using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    //SaveData
    
    public string[] inventoryItems;
    public int[] inventoryQuanity;
    public int[][] itemEffects;

    public string[] equipmentItems;
    public int[][] equipmentEffects;

    //Inventory UI;

    public GameObject screenOverlay;

    public GameObject interact;
    public GameObject unequip;
    public GameObject confirmation;

    public GameObject inventoryWarning;
    public GameObject droppedWarning;

    public GameObject healthFullLabel;
    public GameObject staminaFullLabel;
    public GameObject manaFullLabel;
    public GameObject potionInEffectLabel;

    public GameObject useArrowLabel;

    public GameObject itemLabel;
    public GameObject statsLabel;
    public GameObject durabilityLabel;

    //Variables

    public float textDuration;

    private bool isOpen;
    private bool removeStack;

    public GameObject targetItem;

    private int itemPlace;
    
    private GameObject[] slots;
    private GameObject[] labels;
    private GameObject[] statLabels;
    private GameObject[] equipmentStats;
    private GameObject[] durabilities;

    private GameObject[] equipmentSlots;

    public GameObject inventoryUI;
    public GameObject iconHolder;

    private List<GameObject> warnings;

    private string itemTemp;    //For swapping equipment
    private int[] effectTemp;

    public int lckUp;
    public int dexUp;
    public int strUp;
    public int intUp;
    public int conUp;

    //3D Icons
    public GameObject helmetIcon;
    public GameObject bodyIcon;
    public GameObject legsIcon;
    public GameObject glovesIcon;
    public GameObject bootsIcon;

    public GameObject swordIcon;
    public GameObject axeIcon;
    public GameObject bowIcon;
    public GameObject staffIcon;

    public GameObject arrowIcon;
    public GameObject healthPotionIcon;
    public GameObject manaPotionIcon;
    public GameObject mysteriousPotionIcon;
    public GameObject staminaPotionIcon;

    private RenderTexture arrow;
    private RenderTexture healthPotion;
    private RenderTexture manaPotion;
    private RenderTexture mysteriousPotion;
    private RenderTexture staminaPotion;

    private RenderTexture helmet;
    private RenderTexture body;
    private RenderTexture legs;
    private RenderTexture gloves;
    private RenderTexture boots;

    private RenderTexture sword;
    private RenderTexture axe;
    private RenderTexture bow;
    private RenderTexture staff;

    //2D Icons
    public Texture inventorySlot;

    public Texture helmetSlot;
    public Texture bodySlot;
    public Texture legsSlot;
    public Texture bootsSlot;
    public Texture glovesSlot;
    public Texture weaponSlot;


    // Start is called before the first frame update
    void Start()
    {
        inventoryUI.SetActive(true); //initial activation to perform actions
        isOpen = false;
        textDuration = 4f;

        inventoryItems = new string[12];
        inventoryQuanity = new int[12];
        itemEffects = new int[12][];

        equipmentItems = new string[6];
        equipmentEffects = new int[6][];

        effectTemp = new int[6];

        warnings = new List<GameObject>();
        slots = GameObject.FindGameObjectsWithTag("InventorySlot");
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].GetComponent<RawImage>().texture = inventorySlot;
        }

        equipmentSlots = GameObject.FindGameObjectsWithTag("EquipmentSlot");
        equipmentStats = new GameObject[6];

        labels = new GameObject[12];
        statLabels = new GameObject[12];
        durabilities = new GameObject[6];

        lckUp = 0;
        dexUp = 0;
        strUp = 0;
        intUp = 0;
        conUp = 0;

        //Physical instantiation of 3d Icons
        GameObject Icon1 = Instantiate(arrowIcon, new Vector3(0, 0, 0), Quaternion.identity);
        GameObject Icon2 = Instantiate(healthPotionIcon, new Vector3(0, 1, 0), Quaternion.identity);
        GameObject Icon3 = Instantiate(manaPotionIcon, new Vector3(0, -1, 0), Quaternion.identity);
        GameObject Icon4 = Instantiate(mysteriousPotionIcon, new Vector3(0, 0, 1), Quaternion.identity);
        GameObject Icon5 = Instantiate(staminaPotionIcon, new Vector3(0, 0, -1), Quaternion.identity);

        GameObject Icon6 = Instantiate(helmetIcon, new Vector3(0, 2, 0), Quaternion.identity);
        GameObject Icon7 = Instantiate(bodyIcon, new Vector3(0, 2, 2), Quaternion.identity);
        GameObject Icon8 = Instantiate(legsIcon, new Vector3(0, 2, 4), Quaternion.identity);
        GameObject Icon9 = Instantiate(glovesIcon, new Vector3(0, 2, 6), Quaternion.identity);
        GameObject Icon10 = Instantiate(bootsIcon, new Vector3(0, 2, 8), Quaternion.identity);
        GameObject Icon11 = Instantiate(swordIcon, new Vector3(0, 4, 0), Quaternion.identity);
        GameObject Icon12 = Instantiate(axeIcon, new Vector3(0, 4, 2), Quaternion.identity);
        GameObject Icon13 = Instantiate(bowIcon, new Vector3(0, 4, 4), Quaternion.identity);
        GameObject Icon14 = Instantiate(staffIcon, new Vector3(0, 4, 6), Quaternion.identity);

        Icon1.transform.parent = iconHolder.transform;
        Icon2.transform.parent = iconHolder.transform;
        Icon3.transform.parent = iconHolder.transform;
        Icon4.transform.parent = iconHolder.transform;
        Icon5.transform.parent = iconHolder.transform;

        Icon6.transform.parent = iconHolder.transform;
        Icon7.transform.parent = iconHolder.transform;
        Icon8.transform.parent = iconHolder.transform;
        Icon9.transform.parent = iconHolder.transform;
        Icon10.transform.parent = iconHolder.transform;
        Icon11.transform.parent = iconHolder.transform;
        Icon12.transform.parent = iconHolder.transform;
        Icon13.transform.parent = iconHolder.transform;
        Icon14.transform.parent = iconHolder.transform;

        arrow = Icon1.GetComponent<Camera>().targetTexture;
        healthPotion = Icon2.GetComponent<Camera>().targetTexture;
        manaPotion = Icon3.GetComponent<Camera>().targetTexture;
        mysteriousPotion = Icon4.GetComponent<Camera>().targetTexture;
        staminaPotion = Icon5.GetComponent<Camera>().targetTexture;

        helmet = Icon6.GetComponent<Camera>().targetTexture;
        body = Icon7.GetComponent<Camera>().targetTexture;
        legs = Icon8.GetComponent<Camera>().targetTexture;
        gloves = Icon9.GetComponent<Camera>().targetTexture;
        boots = Icon10.GetComponent<Camera>().targetTexture;
        sword = Icon11.GetComponent<Camera>().targetTexture;
        axe = Icon12.GetComponent<Camera>().targetTexture;
        bow = Icon13.GetComponent<Camera>().targetTexture;
        staff = Icon14.GetComponent<Camera>().targetTexture;

        Vector3 slotPos;
        
        //Create Item labels and strength Labels (text)
        for(int i = 0; i < inventoryItems.Length; i++)
        {
            slotPos = slots[i].transform.position;
            labels[i] = Instantiate(itemLabel, new Vector3(slotPos.x+5f, slotPos.y-40f, slotPos.z), Quaternion.identity);
            statLabels[i] = Instantiate(statsLabel, new Vector3(slotPos.x + 50f, slotPos.y+15, slotPos.z), Quaternion.identity);
            labels[i].transform.parent = iconHolder.transform;
            statLabels[i].transform.parent = iconHolder.transform;
        }        
        for(int i = 0; i < equipmentItems.Length; i++)
        {
            slotPos = equipmentSlots[i].transform.position;
            durabilities[i] = Instantiate(durabilityLabel, new Vector3(slotPos.x + 30f, slotPos.y -25, slotPos.z), Quaternion.identity);
            equipmentStats[i] = Instantiate(statsLabel, new Vector3(slotPos.x + 110f, slotPos.y + 15, slotPos.z), Quaternion.identity);
            equipmentStats[i].transform.parent = iconHolder.transform;
            durabilities[i].transform.parent = iconHolder.transform;
        }
       //Ensure inventory exists
        if (System.IO.File.Exists("Assets/Resources/SaveData/Inventory/Items.json"))
        {
            LoadInventory();
        }
        else
        {
            for (int i = 0; i < inventoryItems.Length; i++)
            {
                inventoryItems[i] = "Empty";
                inventoryQuanity[i] = 0;
                itemEffects[i] = new int[6];
                for(int j = 0; j < 6; j++)
                {
                    itemEffects[i][j] = 0;
                }                
            }           
        }

        //Ensure equipment exist
        if (System.IO.File.Exists("Assets/Resources/SaveData/Inventory/Equipment.json"))
        {
            LoadEquipment();
        }
        else
        {           
            for (int i = 0; i < equipmentItems.Length; i++)
            {
                equipmentItems[i] = "Empty";

                equipmentEffects[i] = new int[6];
                for (int j = 0; j < 5; j++)
                {
                    equipmentEffects[i][j] = 0;
                }
                equipmentEffects[i][5] = 100;
            }
            UpdateEquipmentSlots();
        }
        PlayerPrefs.SetFloat("WeaponDur", equipmentEffects[5][5]);

        //If fresh spawn, spawn starter equipment near feet
        if(PlayerPrefs.GetInt("HasStarter") == 0)
        {
            Vector3 player = GameObject.FindGameObjectWithTag("Player").transform.position;
            if (PlayerPrefs.GetString("CLASS") == "Barbarian")
            {
                gameObject.GetComponentInChildren<Items>().SpawnItem("Axe",player.x, player.y, player.z);

            }
            else if (PlayerPrefs.GetString("CLASS") == "Sourcerer")
            {
                gameObject.GetComponentInChildren<Items>().SpawnItem("Staff", player.x, player.y, player.z);
            }
            else if (PlayerPrefs.GetString("CLASS") == "Rouge")
            {
                gameObject.GetComponentInChildren<Items>().SpawnItem("Bow", player.x, player.y, player.z);
                for(int i = 0; i < 20; i++) //20 free starter arrows near spawn
                {
                    gameObject.GetComponentInChildren<Items>().SpawnItem("Arrow", player.x, player.y, player.z);
                }
            }
            else
            {
                gameObject.GetComponentInChildren<Items>().SpawnItem("Sword", player.x, player.y, player.z);
            }
            PlayerPrefs.SetInt("HasStarter", 1);
        }        
        
        UpdateInventory();
        inventoryUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            targetItem = null;
            if (isOpen)
            {
                //is open, so e to close
                Cancel();                
                gameObject.transform.parent.GetComponentInChildren<StatsManager>().UpdateStats();            //Update stat values
                inventoryUI.SetActive(false);
                isOpen = false;
            }
            else
            {
                inventoryUI.SetActive(true);
                isOpen = true;
            }            
        }
    }

    
    public void InventoryFull()
    {
        GameObject textwarning = Instantiate(inventoryWarning, new Vector3(0, 0, 0), Quaternion.identity);
        textwarning.transform.parent = iconHolder.transform;
        textwarning.transform.Translate(250,0,0);
        warnings.Add(textwarning);
        Destroy(textwarning, textDuration);
    }
    public void HealthFull()
    {        
        GameObject textwarning = Instantiate(healthFullLabel, new Vector3(0, 0, 0), Quaternion.identity);
        textwarning.transform.parent = iconHolder.transform;
        textwarning.transform.Translate(250, 0, 0);
        warnings.Add(textwarning);
        Destroy(textwarning, textDuration);
    }
    public void StaminaFull()
    {
        GameObject textwarning = Instantiate(staminaFullLabel, new Vector3(0, 0, 0), Quaternion.identity);
        textwarning.transform.parent = iconHolder.transform;
        textwarning.transform.Translate(250, 0, 0);
        warnings.Add(textwarning);
        Destroy(textwarning, textDuration);
    }
    public void ManaFull()
    {
        GameObject textwarning = Instantiate(manaFullLabel, new Vector3(0, 0, 0), Quaternion.identity);
        textwarning.transform.parent = iconHolder.transform;
        textwarning.transform.Translate(250, 0, 0);
        warnings.Add(textwarning);
        Destroy(textwarning, textDuration);
    }
    public void PotionInUse()
    {
        GameObject textwarning = Instantiate(potionInEffectLabel, new Vector3(0, 0, 0), Quaternion.identity);
        textwarning.transform.parent = iconHolder.transform;
        textwarning.transform.Translate(250, 0, 0);
        warnings.Add(textwarning);
        Destroy(textwarning, textDuration);
    }
    public void CannotUseArrow()
    {
        GameObject textwarning = Instantiate(useArrowLabel, new Vector3(0, 0, 0), Quaternion.identity);
        textwarning.transform.parent = iconHolder.transform;
        textwarning.transform.Translate(250, 0, 0);
        warnings.Add(textwarning);
        Destroy(textwarning, textDuration);
    }
    public void Drop()
    {
        itemPlace = int.Parse(targetItem.name.Replace("Slot", ""));
        gameObject.GetComponentInChildren<Items>().DropItem(targetItem.transform.GetComponent<RawImage>().texture.name,itemEffects[itemPlace-1]);
        RemoveItem();
    }
    public void ItemsDropped()
    {        
        GameObject textwarning = Instantiate(droppedWarning, new Vector3(0, 0, 0), Quaternion.identity);
        textwarning.transform.parent = screenOverlay.transform;
        textwarning.transform.Translate(250, 0, 0);
        warnings.Add(textwarning);
        Destroy(textwarning, textDuration);
    }
    

    public void ShowMenu()
    {     
        itemPlace = int.Parse(targetItem.name.Replace("Slot", "")); //Get slot number to decrease
        if(Equipable(targetItem.transform.GetComponent<RawImage>().texture.name)) //Change "Use" to "Equip"
        {
            for(int i = 0; i < interact.transform.GetChildCount(); i++)
            {
                if(interact.transform.GetChild(i).name == "UseButton")
                {
                    interact.transform.GetChild(i).GetComponentInChildren<Text>().text = "Equip Item";
                }
            }
            
        }
        else
        {
            for (int i = 0; i < interact.transform.GetChildCount(); i++)
            {
                if (interact.transform.GetChild(i).name == "UseButton")
                {
                    interact.transform.GetChild(i).GetComponentInChildren<Text>().text = "Use Item";
                }
            }
        }

        if (inventoryQuanity[itemPlace -1] > 0){
            interact.SetActive(true);
        }                      
    }
    public void ShowEquipMenu()
    {
        itemPlace = int.Parse(targetItem.name.Replace("Slot", "")); //Get slot number to decrease
        if (equipmentItems[itemPlace-1] != "Empty")
        {
            unequip.SetActive(true);
        }
        
    }

    public void Use()
    {
        if (Equipable(targetItem.transform.GetComponent<RawImage>().texture.name))
        {
            Equip();
        }
        else
        {
            Consume();
        }
        interact.SetActive(false);
    }
    public void Consume()
    {
        bool used = false;
        if (targetItem.transform.GetComponent<RawImage>().texture.name.Contains("Health")){
            used = gameObject.transform.parent.GetComponentInChildren<StatsManager>().UseHealthPot();
        }
        else if (targetItem.transform.GetComponent<RawImage>().texture.name.Contains("Mana")){
            used = gameObject.transform.parent.GetComponentInChildren<StatsManager>().UseManaPot();
        }
        else if (targetItem.transform.GetComponent<RawImage>().texture.name.Contains("Stamina")){
            used= gameObject.transform.parent.GetComponentInChildren<StatsManager>().UseStaminaPot();
        }
        else if(targetItem.transform.GetComponent<RawImage>().texture.name.Contains("Myst")){
            used= gameObject.transform.parent.GetComponentInChildren<StatsManager>().UseMysteriousPot();
        }
        else if (targetItem.transform.GetComponent<RawImage>().texture.name.Contains("Arrow"))
        {
            CannotUseArrow();
        }

        if (used)
        {
            RemoveItem();
        }
    }

    public void Equip()
    {
        itemPlace = int.Parse(targetItem.name.Replace("Slot", ""));
        itemTemp = targetItem.transform.GetComponent<RawImage>().texture.name;        

        if (itemTemp.Contains("Helmet"))
        {           
            if (inventoryItems[itemPlace - 1] != "Empty")
            {
                itemTemp = equipmentItems[0];
                effectTemp = equipmentEffects[0];
            }
            equipmentItems[0] = inventoryItems[itemPlace - 1];
            equipmentEffects[0] = itemEffects[itemPlace - 1];
            
            PlayerPrefs.SetFloat("HelmetDur", equipmentEffects[0][5]);
        }            
        else if (itemTemp.Contains("Body"))
        {
            if (inventoryItems[itemPlace - 1] != "Empty")
            {
                itemTemp = equipmentItems[1];
                effectTemp = equipmentEffects[1];
            }
            equipmentItems[1] = inventoryItems[itemPlace - 1];
            equipmentEffects[1] = itemEffects[itemPlace - 1];
            
            PlayerPrefs.SetFloat("BodyDur", equipmentEffects[1][5]);
        } 
        else if (itemTemp.Contains("Glove"))
        {
            if (inventoryItems[itemPlace - 1] != "Empty")
            {
                itemTemp = equipmentItems[2];
                effectTemp = equipmentEffects[2];
            }
            equipmentItems[2] = inventoryItems[itemPlace - 1];
            equipmentEffects[2] = itemEffects[itemPlace - 1];
            PlayerPrefs.SetFloat("GloveDur", equipmentEffects[2][5]);
        }
        else if (itemTemp.Contains("Leg"))
        {
            if (inventoryItems[itemPlace - 1] != "Empty")
            {
                itemTemp = equipmentItems[3];
                effectTemp = equipmentEffects[3];
            }
            equipmentItems[3] = inventoryItems[itemPlace - 1];
            equipmentEffects[3] = itemEffects[itemPlace - 1];
            PlayerPrefs.SetFloat("LegDur", equipmentEffects[3][5]);
        }
        else if (itemTemp.Contains("Boot"))
        {
            if (inventoryItems[itemPlace - 1] != "Empty")
            {
                itemTemp = equipmentItems[4];
                effectTemp = equipmentEffects[4];
            }
            equipmentItems[4] = inventoryItems[itemPlace - 1];
            equipmentEffects[4] = itemEffects[itemPlace - 1];
            PlayerPrefs.SetFloat("BootDur", equipmentEffects[4][5]);
        }        
        else if (itemTemp.Contains("Sword") || itemTemp.Contains("Axe") || itemTemp.Contains("Bow") || itemTemp.Contains("Staff"))
        {                     
            if (inventoryItems[itemPlace - 1] != "Empty")
            {
                itemTemp = equipmentItems[5];
                effectTemp = equipmentEffects[5];                
            }
            equipmentItems[5] = inventoryItems[itemPlace - 1];
            equipmentEffects[5] = itemEffects[itemPlace - 1];
            PlayerPrefs.SetFloat("WeaponDur", equipmentEffects[5][5]);
        }
       
        UpdateEquipmentSlots();         
        AddItem(itemTemp, effectTemp);        
        RemoveItem();

        unequip.SetActive(false);
        UpdateInventory();
    }
    public void UnEquip()
    {
        itemPlace = int.Parse(targetItem.name.Replace("Slot", ""));
        if(AddItem(targetItem.transform.GetComponent<RawImage>().texture.name, equipmentEffects[itemPlace - 1]))
        {
            equipmentItems[itemPlace - 1] = "Empty";
            equipmentEffects[itemPlace - 1] = new int[6];
            UpdateEquipmentSlots();
            UpdateInventory();
        }
        else
        {
            InventoryFull();            
        }
        unequip.SetActive(false);
    }

    public bool Equipable(string item)
    {
         if (item.Contains("Helmet") || item.Contains("Body") || item.Contains("Leg"))
        {
            return true;
        }        
        else if (item.Contains("Glove") || item.Contains("Boots") || item.Contains("Sword"))
        {
            return true;
        }       
        else if (item.Contains("Axe") || item.Contains("Bow") || item.Contains("Staff"))
        {
            return true;
        }        
        else 
        {
            return false;
        }
    }

    public bool ConsumeArrow()
    {        
        int arrowSlot = CheckSlot("Arrow"); //arrow exists
        targetItem = slots[arrowSlot];
        RemoveItem();
        if (arrowSlot == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
        
    }

    public void SelectRemoveStack()
    {
        removeStack = true;
        Confirmation();
    }
    public void SelectRemove()
    {
        removeStack = false;
        Confirmation();
    }

    public void RemoveItem()
    {        
        itemPlace = int.Parse(targetItem.name.Replace("Slot", "")); //Get slot number to decrease
        if (inventoryQuanity[itemPlace - 1] > 1)
        {
            inventoryQuanity[itemPlace - 1] -= 1;
            labels[itemPlace - 1].GetComponent<Text>().text = inventoryItems[itemPlace - 1] + " " + inventoryQuanity[itemPlace - 1];
            statLabels[itemPlace - 1].GetComponent<Text>().text = "";
            confirmation.SetActive(false);
        }
        else
        {
            inventoryItems[itemPlace-1] = "Empty";
            inventoryQuanity[itemPlace - 1] = 0;
            for (int i = 0; i < 5; i++)
            {
                itemEffects[itemPlace - 1][i] = 0;
            }
            slots[itemPlace - 1].GetComponent<RawImage>().texture = inventorySlot;
            labels[itemPlace - 1].GetComponent<Text>().text = "Empty 0";
            statLabels[itemPlace - 1].GetComponent<Text>().text = "";
            confirmation.SetActive(false);
            interact.SetActive(false);
        }
        UpdateInventory();
    }

    public void RemoveStack()
    {
        itemPlace = int.Parse(targetItem.name.Replace("Slot", "")); //Get slot number to decrease
        inventoryItems[itemPlace-1] = "Empty";
        inventoryQuanity[itemPlace - 1] = 0;        
        for (int i = 0; i < 5; i++)
        {
            itemEffects[itemPlace - 1][i] = 0;
        }
        slots[itemPlace - 1].GetComponent<RawImage>().texture = inventorySlot;
        labels[itemPlace - 1].GetComponent<Text>().text = "Empty 0";
        statLabels[itemPlace - 1].GetComponent<Text>().text = "";
        UpdateInventory();
        confirmation.SetActive(false);
        interact.SetActive(false);
    }

    public void Cancel()
    {
        confirmation.SetActive(false);
        unequip.SetActive(false);
        interact.SetActive(false);
        targetItem = null;
        for(int i = 0; i < warnings.Count; i++)
        {
            Destroy(warnings[i]); //Remove existing warnings
        }        
    }

    public void Confirmation()
    {
        confirmation.SetActive(true);
    }
    public void AcceptConfirmation()
    {
        if (removeStack)
        {
            RemoveStack();
        }
        else
        {
            RemoveItem();
        }
    }

    public bool AddItem(GameObject newItem, int[] effects) //Return true if added
    {     
        string itemName = newItem.name;
        int slotNum = CheckSlot(itemName);
        if (slotNum == -1)
        {
            return false;
        }
        else
        {
            inventoryItems[slotNum] = itemName;
            inventoryQuanity[slotNum] += 1;
            itemEffects[slotNum] = effects;
            
            PopulateIcons(itemName, slotNum, effects);
            UpdateInventory();

            return true;
        }
    }
    //Overloaded Function (for Unequipping without GameObject)
    public bool AddItem(string newItem, int[] effects) //Return true if added
    {
        int slotNum = CheckSlot(newItem);        
        if (slotNum == -1)
        {
            return false;
        }
        else
        {
            inventoryItems[slotNum] = newItem;
            inventoryQuanity[slotNum] += 1;
            itemEffects[slotNum] = effects;

            PopulateIcons(newItem, slotNum, effects);
            UpdateInventory();

            return true;
        }
    }

    private void PopulateIcons(string itemName, int slotNum, int[] effects)
    {
        if (itemName.Contains("Arrow"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = arrow;
        }
        else if (itemName.Contains("HealthPotion"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = healthPotion;
        }
        else if (itemName.Contains("ManaPotion"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = manaPotion;
        }
        else if (itemName.Contains("MysteriousPotion"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = mysteriousPotion;
        }
        else if (itemName.Contains("StaminaPotion"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = staminaPotion;
        }
        else if (itemName.Contains("Helmet"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = helmet;
        }
        else if (itemName.Contains("Body"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = body;
        }
        else if (itemName.Contains("Leg"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = legs;
        }
        else if (itemName.Contains("Glove"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = gloves;
        }
        else if (itemName.Contains("Boots"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = boots;
        }
        else if (itemName.Contains("Sword"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = sword;
        }
        else if (itemName.Contains("Axe"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = axe;
        }
        else if (itemName.Contains("Bow"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = bow;
        }
        else if (itemName.Contains("Staff"))
        {
            slots[slotNum].GetComponent<RawImage>().texture = staff;
        }
        
        inventoryItems[slotNum] = inventoryItems[slotNum].Replace("(Clone)", "");
        labels[slotNum].GetComponent<Text>().text = inventoryItems[slotNum] + " " + inventoryQuanity[slotNum];
        if (Equipable(inventoryItems[slotNum])){
            statLabels[slotNum].GetComponent<Text>().text = "LCK: " + effects[0] + "\nDEX: " + effects[1] + "\nSTR: " + effects[2] + "\nINT: " + effects[3] + "\nCON: " + effects[4];
        }        
        else
        {
            statLabels[slotNum].GetComponent<Text>().text = "";
        }
    }

    public int CheckSlot(string newItem) //Return -1 if no space left,
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (newItem.Contains(inventoryItems[i])) //Same prefab (stack them) - can stack any non armour up to 100
            {
                if (newItem.Contains("Arrow")) //Arrows Stacked to 64
                {
                    if (inventoryQuanity[i] <= 64)
                    {
                        return i;
                    }
                }
                else if (newItem.Contains("Potion"))//Potions stacked to 8
                {
                    if (inventoryQuanity[i] < 16)
                    {
                        return i;
                    }
                }
            }
            else if (inventoryItems[i] == "Empty")
            {
                return i;
            }
        }
        return -1; //No inventory slot found
    }
    public void UpdateEquipmentSlots()
    {
        
        if (equipmentItems[0] == "Empty")
        {
            equipmentSlots[0].GetComponent<RawImage>().texture = helmetSlot;
            equipmentStats[0].GetComponent<Text>().text = "";
            durabilities[0].GetComponent<Text>().text = "";
        }
        else
        {           
            equipmentSlots[0].GetComponent<RawImage>().texture = helmet;
            equipmentStats[0].GetComponent<Text>().text = "LCK: " + equipmentEffects[0][0] + "\nDEX: " + equipmentEffects[0][1] + "\nSTR: " + equipmentEffects[0][2] + "\nINT: " + equipmentEffects[0][3] + "\nCON: " + equipmentEffects[0][4];
            durabilities[0].GetComponent<Text>().text = equipmentEffects[0][5].ToString() + "% ";           
            AddStats();
        }
        if (equipmentItems[1] == "Empty")
        {
            equipmentSlots[1].GetComponent<RawImage>().texture = bodySlot;
            equipmentStats[1].GetComponent<Text>().text = "";
            durabilities[1].GetComponent<Text>().text = "";
        }
        else
        {
            equipmentSlots[1].GetComponent<RawImage>().texture = body;
            equipmentStats[1].GetComponent<Text>().text = "LCK: " + equipmentEffects[1][0] + "\nDEX: " + equipmentEffects[1][1] + "\nSTR: " + equipmentEffects[1][2] + "\nINT: " + equipmentEffects[1][3] + "\nCON: " + equipmentEffects[1][4];
            durabilities[1].GetComponent<Text>().text = equipmentEffects[1][5].ToString() + "% ";
            AddStats();
        }
        if (equipmentItems[2] == "Empty")
        {
            equipmentSlots[2].GetComponent<RawImage>().texture = glovesSlot;
            equipmentStats[2].GetComponent<Text>().text = "";
            durabilities[2].GetComponent<Text>().text = "";
        }
        else
        {
            equipmentSlots[2].GetComponent<RawImage>().texture = gloves;
            equipmentStats[2].GetComponent<Text>().text = "LCK: " + equipmentEffects[2][0] + "\nDEX: " + equipmentEffects[2][1] + "\nSTR: " + equipmentEffects[2][2] + "\nINT: " + equipmentEffects[2][3] + "\nCON: " + equipmentEffects[2][4];
            durabilities[2].GetComponent<Text>().text = equipmentEffects[2][5].ToString() + "% ";
            AddStats();
        }
        if (equipmentItems[3] == "Empty")
        {
            equipmentSlots[3].GetComponent<RawImage>().texture = legsSlot;
            equipmentStats[3].GetComponent<Text>().text = "";
            durabilities[3].GetComponent<Text>().text = "";

        }
        else
        {
            equipmentSlots[3].GetComponent<RawImage>().texture = legs;
            equipmentStats[3].GetComponent<Text>().text = "LCK: " + equipmentEffects[3][0] + "\nDEX: " + equipmentEffects[3][1] + "\nSTR: " + equipmentEffects[3][2] + "\nINT: " + equipmentEffects[3][3] + "\nCON: " + equipmentEffects[3][4];
            durabilities[3].GetComponent<Text>().text = equipmentEffects[3][5].ToString() + "% ";
            AddStats();
        }
        if (equipmentItems[4] == "Empty")
        {
            equipmentSlots[4].GetComponent<RawImage>().texture = bootsSlot;
            equipmentStats[4].GetComponent<Text>().text = "";
            durabilities[4].GetComponent<Text>().text = "";
        }
        else
        {            
            equipmentSlots[4].GetComponent<RawImage>().texture = boots;
            equipmentStats[4].GetComponent<Text>().text = "LCK: " + equipmentEffects[4][0] + "\nDEX: " + equipmentEffects[4][1] + "\nSTR: " + equipmentEffects[4][2] + "\nINT: " + equipmentEffects[4][3] + "\nCON: " + equipmentEffects[4][4];
            durabilities[4].GetComponent<Text>().text = equipmentEffects[4][5].ToString() + "% ";            
            AddStats();            
        }
        if (equipmentItems[5] == "Empty")
        {
            equipmentSlots[5].GetComponent<RawImage>().texture = weaponSlot;
            equipmentStats[5].GetComponent<Text>().text = "";
            durabilities[5].GetComponent<Text>().text = "";
        }
        else
        {
            if (equipmentItems[5] == "Sword")
            {
                equipmentSlots[5].GetComponent<RawImage>().texture = sword;
                PlayerPrefs.SetInt("Weapon", 1);
            }
            else if (equipmentItems[5] == "Axe")
            {
                equipmentSlots[5].GetComponent<RawImage>().texture = axe;
                PlayerPrefs.SetInt("Weapon", 2);
            }
            else if (equipmentItems[5] == "Bow")
            {
                equipmentSlots[5].GetComponent<RawImage>().texture = bow;
                PlayerPrefs.SetInt("Weapon", 3);
            }
            else if (equipmentItems[5] == "Staff")
            {
                equipmentSlots[5].GetComponent<RawImage>().texture = staff;
                PlayerPrefs.SetInt("Weapon", 4);
            }
            equipmentStats[5].GetComponent<Text>().text = "LCK: " + equipmentEffects[5][0] + "\nDEX: " + equipmentEffects[5][1] + "\nSTR: " + equipmentEffects[5][2] + "\nINT: " + equipmentEffects[5][3] + "\nCON: " + equipmentEffects[5][4];
            
            durabilities[5].GetComponent<Text>().text = equipmentEffects[5][5].ToString() + "% ";
            AddStats();
        }
        gameObject.transform.parent.GetComponentInChildren<StatsManager>().UpdateStats();            //Update stat values
    }
   
    public void AddStats()
    {
        //Add luck of all armour pieces and weapon, reducing effect with durability
        lckUp = DurDebuf(equipmentEffects[0][0], equipmentEffects[0][5]) + DurDebuf(equipmentEffects[1][0], equipmentEffects[1][5]) + DurDebuf(equipmentEffects[2][0], equipmentEffects[2][5]);
        lckUp += DurDebuf(equipmentEffects[3][0], equipmentEffects[3][5]) + DurDebuf(equipmentEffects[4][0], equipmentEffects[4][5]) + DurDebuf(equipmentEffects[5][0], equipmentEffects[5][5]);

        dexUp = DurDebuf(equipmentEffects[0][1], equipmentEffects[0][5]) + DurDebuf(equipmentEffects[1][1], equipmentEffects[1][5]) + DurDebuf(equipmentEffects[2][1], equipmentEffects[2][5]);
        dexUp += DurDebuf(equipmentEffects[3][1], equipmentEffects[3][5]) + DurDebuf(equipmentEffects[4][1], equipmentEffects[4][5]) + DurDebuf(equipmentEffects[5][1], equipmentEffects[5][5]);

        strUp = DurDebuf(equipmentEffects[0][2], equipmentEffects[0][5]) + DurDebuf(equipmentEffects[1][2], equipmentEffects[1][5]) + DurDebuf(equipmentEffects[2][2], equipmentEffects[2][5]);
        strUp += DurDebuf(equipmentEffects[3][2], equipmentEffects[3][5]) + DurDebuf(equipmentEffects[4][2], equipmentEffects[4][5]) + DurDebuf(equipmentEffects[5][2], equipmentEffects[5][5]);

        intUp = DurDebuf(equipmentEffects[0][3], equipmentEffects[0][5]) + DurDebuf(equipmentEffects[1][3], equipmentEffects[1][5]) + DurDebuf(equipmentEffects[2][3], equipmentEffects[2][5]);
        intUp = DurDebuf(equipmentEffects[3][3], equipmentEffects[3][5]) + DurDebuf(equipmentEffects[4][3], equipmentEffects[4][5]) + DurDebuf(equipmentEffects[5][3], equipmentEffects[5][5]);

        conUp = DurDebuf(equipmentEffects[0][4], equipmentEffects[0][5]) + DurDebuf(equipmentEffects[1][4], equipmentEffects[1][5]) + DurDebuf(equipmentEffects[2][4], equipmentEffects[2][5]);
        conUp += DurDebuf(equipmentEffects[3][4], equipmentEffects[3][5]) + DurDebuf(equipmentEffects[4][4], equipmentEffects[4][5]) + DurDebuf(equipmentEffects[5][4], equipmentEffects[5][5]);
    }
    public int DurDebuf(int stat,int durability)
    {
          return (int)((float)((stat) * (((2 * (float)durability) / 300) + (1f / 3f))));
    }

    public void UpdateInventory()
    {
        SaveInventory invData = new SaveInventory(inventoryItems, inventoryQuanity, itemEffects);
        SaveInventory equipData = new SaveInventory(equipmentItems, inventoryQuanity, equipmentEffects);

        string inventoryData = JsonUtility.ToJson(invData);
        string equipmentData = JsonUtility.ToJson(equipData);

        System.IO.File.WriteAllText("Assets/Resources/SaveData/Inventory/Items.json", inventoryData);
        System.IO.File.WriteAllText("Assets/Resources/SaveData/Inventory/Equipment.json", equipmentData);
    }
    public void LoadInventory()
    {
        string inventoryData = System.IO.File.ReadAllText("Assets/Resources/SaveData/Inventory/Items.json");

        SaveInventory invData = JsonUtility.FromJson<SaveInventory>(inventoryData);

        inventoryItems = invData.GetItems();
        inventoryQuanity = invData.GetQuantities();
        itemEffects = invData.GetEffects();
                 
        for (int i = 0; i < slots.Length; i++)
        {
            PopulateIcons(inventoryItems[i], i,itemEffects[i]);            
        }        
    }
    public void LoadEquipment()
    {
        string equipmentData = System.IO.File.ReadAllText("Assets/Resources/SaveData/Inventory/Equipment.json");
        SaveInventory equipData = JsonUtility.FromJson<SaveInventory>(equipmentData);

        equipmentItems = equipData.GetItems();        
        equipmentEffects = equipData.GetEffects();
        UpdateEquipmentSlots();
    }

    [System.Serializable]
    public class SaveInventory
    {
        [SerializeField] private string[] saveInventoryItems;
        [SerializeField] private int[] saveInventoryQuanity;
        [SerializeField] private int[] saveItemEffects;

        public SaveInventory(string[] items, int[] quantities, int[][] effects)
        {
            saveInventoryItems = items;
            saveInventoryQuanity = quantities;
            saveItemEffects = MultiToSingle(effects);
        }

        public string[] GetItems()
        {
            return saveInventoryItems;
        }

        public int[] GetQuantities()
        {
            return saveInventoryQuanity;
        }
        public int[][] GetEffects()
        {
            return SingleToMulti(saveItemEffects,6);
        }
        public int[] MultiToSingle(int[][] multi)
        {
            int[] single = new int[multi.Length * multi[0].Length];

            //i counts the first dimension
            //j is the second dimension
            //index is the new single array index
            for (int i = 0, index = 0; i < multi.Length; i++)
            {
                for(int j = 0; j< multi[i].Length; j++, index++)
                {
                    single[index] = multi[i][j];
                }                
            }
            return single;
        }
        
        public int[][] SingleToMulti(int[] single, int size)
        {
            int[][] multi = new int[(int)single.Length / size][];

            //i = first dimension
            //j is the second dimension counter
            //index is the single dimension counter

            for (int i = 0, index = 0; i < multi.Length; i++)
            {
                multi[i] = new int[size];
                for(int j=0;j < size; j++,index++)
                {                   
                    multi[i][j] = single[index];
                }                
            }
            return multi;
        }
    }
}
