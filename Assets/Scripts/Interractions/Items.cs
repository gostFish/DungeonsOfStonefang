using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Items : MonoBehaviour
{
    //Controller objects
    private Inventory inventory;
    private GameObject player;    

    private GameObject inventoryManager;
    private GameObject CollectablesPool;

    //Possible Items
    public GameObject helmet;  
    public GameObject body;     
    public GameObject legs;     
    public GameObject gloves;   
    public GameObject shoes;   

    public GameObject sword;
    public GameObject axe;
    public GameObject bow;
    public GameObject staff;

    public GameObject arrow;        
    public GameObject healthPot;    
    public GameObject manaPot;      
    public GameObject mysteriousPot;
    public GameObject staminaPot;

    private GameObject itemInstance;

    //Stat Boosts For Weapons/Armour
    public int[] effects;

    //Chest Items
    private int minLoot;
    private int maxLoot;
    private int currentLevel;

    private int[] noEffect;


    public float minCollectDistance;
    public bool isKey;

    // Start is called before the first frame update
    void Start()
    {
        minCollectDistance = 2f;
        player = GameObject.FindGameObjectWithTag("Player");
        inventoryManager = GameObject.FindGameObjectWithTag("ProgressManager");
        if(inventoryManager != null)
        {
            inventory = inventoryManager.GetComponent<Inventory>();

            effects = new int[6];
            noEffect = new int[6];
            for (int i = 0; i < 6; i++)
            {
                noEffect[i] = 0;
            }
        }        
    }


    void OnMouseDown()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
            if (Vector3.Distance(player.transform.position, gameObject.transform.position) < minCollectDistance)
            {
                if (gameObject.name.Contains("Chest"))
                {
                    GameObject[] instancePools = GameObject.FindGameObjectsWithTag("levelData"); //Need instance pool only for chests
                    foreach (GameObject pool in instancePools) //Find collectables pool
                    {
                        if (pool.name == "Collectables")
                        {
                            CollectablesPool = pool;
                            break;
                        }
                    }

                    currentLevel = PlayerPrefs.GetInt("currentLevel");
                    if (gameObject.name.Contains("ChestL"))
                    {
                        //Between 3 - 8 items, every 5 levels +2 max, every 10 levels +2 min
                        minLoot = 3 + ((int)(currentLevel / 10) * 2);
                        maxLoot = 8 + ((int)(currentLevel / 5) * 2);

                        GetChestItem(minLoot, maxLoot);
                        Destroy(gameObject);
                    }
                    else
                    {
                        //Between 2 - 5 item, every 5 levels +1 max, 10 levels +1 min
                        minLoot = 2 + (int)(currentLevel / 10);
                        maxLoot = 5 + (int)(currentLevel / 5);

                        GetChestItem(minLoot, maxLoot);
                        Destroy(gameObject);
                    }
                }
                else
                {
                    AddInventory();
                }
            }        
        
    }

    public void AddInventory()
    {
        if (isKey)
        {
            player.GetComponent<PlayerController>().keys += 1; //Add collected key
            Destroy(this.gameObject);
        }
        else
        {            
            if (inventory.AddItem(this.gameObject,noEffect)) //Added Object and destroyed it
            {
                Destroy(this.gameObject);
            }
            else
            {
                inventory.InventoryFull();
            }            
        }
    }
    public void GetChestItem(int min, int max)
    {
        for (int i = 0; i < max; i++)
        {
            if (i < min || UnityEngine.Random.Range(0, 10) > 4) //60% chance for item, or less than min
            {
                GameObject newItem = AddRandom();
                if (!inventory.AddItem(newItem,effects)) //Item sent to bag unless full (if so, run the following)
                {
                    DropItem(newItem.name,effects);
                    inventory.ItemsDropped();
                }
            }
        }        
    }
    public void DropItem(string itemName, int[] effects) //Dupes as physical item
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            GameObject[] instancePools = GameObject.FindGameObjectsWithTag("levelData"); //Need instance pool only for chests
            foreach (GameObject pool in instancePools) //Find collectables pool
            {
                if (pool.name == "Collectables")
                {
                    CollectablesPool = pool;
                    break;
                }
            }
        }        

        Vector3 playerPos = player.transform.position;
        float randomX = UnityEngine.Random.Range(-0.5f, 0.5f);
        float randomY = UnityEngine.Random.Range(-0.5f, 0.5f);

        //Spawn somewhere near the player
        itemInstance = SpawnItem(itemName, playerPos.x + randomX, 0.1f, playerPos.z + randomY);
       
        itemInstance.transform.parent = CollectablesPool.transform;
    }
    public GameObject SpawnItem(string itemName, float x, float y, float z)
    {
        if (itemName.Contains("Helmet"))
        {
            itemInstance = Instantiate(helmet, new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Body"))
        {
            itemInstance = Instantiate(body, new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Leg"))
        {
            itemInstance = Instantiate(legs, new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Glove"))
        {
            itemInstance = Instantiate(gloves, new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Boot"))
        {
            itemInstance = Instantiate(shoes, new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Sword"))
        {
            itemInstance = Instantiate(sword, new Vector3(x, y, z), Quaternion.Euler(0, 0, -90));
        }
        else if (itemName.Contains("Axe"))
        {
            itemInstance = Instantiate(axe, new Vector3(x, y, z), Quaternion.Euler(0, 0, -90));
        }
        else if (itemName.Contains("Bow"))
        {
            itemInstance = Instantiate(bow, new Vector3(x, y, z), Quaternion.Euler(0, 0, -90));
        }
        else if (itemName.Contains("Staff"))
        {
            itemInstance = Instantiate(staff, new Vector3(x, y, z), Quaternion.Euler(0, 0, -90));
        }
        else if (itemName.Contains("Arrow"))
        {
            itemInstance = Instantiate(arrow, new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Health"))
        {
            itemInstance = Instantiate(healthPot, new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Mana"))
        {
            itemInstance = Instantiate(manaPot, new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Myst"))
        {
            itemInstance = Instantiate(mysteriousPot, new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Stamina"))
        {
            itemInstance = Instantiate(staminaPot, new Vector3(x, y, z), Quaternion.identity);
        }
        effects[5] = 100;
        itemInstance.transform.GetComponent<Items>().effects = effects;

        return itemInstance;
    }
    public GameObject SpawnItemWithEffects(string itemName, float x, float y, float z)
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        if (itemName.Contains("Helmet"))
        {
            itemInstance = Instantiate(NewHelmet(), new Vector3(x, y, z), Quaternion.identity);            
        }
        else if (itemName.Contains("Body"))
        {
            itemInstance = Instantiate(NewArmour(), new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Leg"))
        {
            itemInstance = Instantiate(NewLegs(), new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Glove"))
        {
            itemInstance = Instantiate(NewGloves(), new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Boot"))
        {
            itemInstance = Instantiate(NewShoes(), new Vector3(x, y, z), Quaternion.identity);
        }
        else if (itemName.Contains("Weapon"))
        {
            itemInstance = Instantiate(NewWeapon(), new Vector3(x, y, z), Quaternion.Euler(0,0,-90));
        }
        itemInstance.transform.GetComponent<Items>().effects = effects;
        return itemInstance;
    }
    public GameObject AddRandom()
    {
        float rand = UnityEngine.Random.Range(0, 100);
        if(rand <= 100 && rand > 90)        //10%
        {
            return staminaPot;
        }
        else if(rand <= 90 && rand > 89)    //1%
        {
            return mysteriousPot;
        }
        else if (rand <= 89 && rand > 79)   //10%
        {
            return manaPot;
        }
        else if (rand <= 79 && rand > 64)   //15%
        {
            return healthPot;
        }
        else if (rand <= 64 && rand > 49)   //15%
        {
            return arrow;
        }
        else if (rand <= 49 && rand > 42)  //7%
        {
            return NewHelmet();
        }
        else if (rand <= 42 && rand > 35)  //7%
        {
            return NewArmour();
        }
        else if (rand <= 35 && rand > 28)  //7%
        {
            return NewLegs();
        }
        else if (rand <= 28 && rand > 21)  //7%
        {
            return NewGloves();
        }
        else if (rand <= 21 && rand > 14)  //7%
        {
            return NewShoes();
        }
        else                                //14% (Remaining) (3.5% per weapon)
        {            
            return NewWeapon();
        }
    }
    public GameObject NewHelmet()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        effects[0] = (int)(UnityEngine.Random.Range(0, currentLevel / 3.3f));                    //Lck (min 0 max 15 at lvl 50)
        effects[1] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 5));       //Dex (min 5 max 10 at lvl 50)
        effects[2] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 5));       //Str (min 5 max 10 at lvl 50)
        effects[3] = (int)(UnityEngine.Random.Range(currentLevel / 2.5f, currentLevel / 1.25f)); //Int (min 20 max 40 at lvl 50)
        effects[4] = (int)(UnityEngine.Random.Range(currentLevel / 3.3f, currentLevel / 1.4f));  //Con (min 15 max 35 at lvl 50)
        effects[5] = 100; //Durability

        return helmet;
    }
    public GameObject NewArmour()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        effects[0] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 3.3f));  //Lck (min 5 max 15 at lvl 50)
        effects[1] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 5));     //Dex (min 5 max 10 at lvl 50)
        effects[2] = (int)(UnityEngine.Random.Range(currentLevel / 3.3f, currentLevel / 1.4f));//Str (min 15 max 35 at lvl 50)
        effects[3] = (int)(UnityEngine.Random.Range(0, currentLevel / 3.3f));                 //Int (min 0  max 15 at lvl 50)
        effects[4] = (int)(UnityEngine.Random.Range(currentLevel / 2, currentLevel / 1));      //Con (min 25 max 50 at lvl 50)
        effects[5] = 100; //Durability

        return body;
    }
    public GameObject NewLegs()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        effects[0] = (int)(UnityEngine.Random.Range(currentLevel / 3.3f, currentLevel / 1.4f)); //Lck (min 15 max 35 at lvl 50)
        effects[1] = (int)(UnityEngine.Random.Range(currentLevel / 3.3f, currentLevel / 5));    //Dex (min 15 max 10 at lvl 50)
        effects[2] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 2.5f));   //Str (min 5 max 20 at lvl 50)
        effects[3] = (int)(UnityEngine.Random.Range(0, currentLevel / 3.3f));                   //Int (min 0  max 15 at lvl 50)
        effects[4] = (int)(UnityEngine.Random.Range(currentLevel / 3.3f, currentLevel / 1.4f)); //Con (min 15 max 35 at lvl 50)
        effects[5] = 100; //Durability
                          
        return legs;
    }
    public GameObject NewGloves()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        effects[0] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 3.3f));   //Lck (min 5 max 15 at lvl 50)
        effects[1] = (int)(UnityEngine.Random.Range(currentLevel / 1.25f, currentLevel / 1));   //Dex (min 40 max 50 at lvl 50)
        effects[2] = (int)(UnityEngine.Random.Range(currentLevel / 2, currentLevel / 1.25f));   //Str (min 25 max 40 at lvl 50)
        effects[3] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 3.3f));   //Int (min 5  max 15 at lvl 50)
        effects[4] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 2.5f));   //Con (min 5 max 20 at lvl 50)
        effects[5] = 100; //Durability
     
        return gloves;
    }
    public GameObject NewShoes()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        effects[0] = (int)(UnityEngine.Random.Range(currentLevel / 1.4f, currentLevel / 1.1f));  //Lck (min 35 max 45 at lvl 50)
        effects[1] = (int)(UnityEngine.Random.Range(currentLevel / 1.4f, currentLevel / 1.25f));//Dex (min 35 max 40 at lvl 50)
        effects[2] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 2));      //Str (min 5 max 25 at lvl 50)
        effects[3] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 2.5f));   //Int (min 5  max 20 at lvl 50)
        effects[4] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 2.5f));   //Con (min 5 max 20 at lvl 50)
        effects[5] = 100; //Durability
       
        return shoes;
    }
    public GameObject NewWeapon()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        float rand = UnityEngine.Random.Range(0, 100); //All equal chance of being weapon type

        if (rand <= 100 && rand > 75)        //Sword
        {
          
            effects[0] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 3.3f));  //Lck (min 5 max 15 at lvl 50)
            effects[1] = (int)(UnityEngine.Random.Range(currentLevel / 2, currentLevel / 1.25f));//Dex (min 25 max 40 at lvl 50)
            effects[2] = (int)(UnityEngine.Random.Range(currentLevel / 2, currentLevel / 1.25f));      //Str (min 25 max 40 at lvl 50)
            effects[3] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 2.5f));   //Int (min 5  max 20 at lvl 50)
            effects[4] = (int)(UnityEngine.Random.Range(currentLevel / 3.3f, currentLevel / 1.66f));   //Con (min 15 max 30 at lvl 50)
            effects[5] = 100; //Durability
       
            return sword;
        }
        else if (rand <= 75 && rand > 50)    //Axe
        {
            effects[0] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 1.4f));   //Lck (min 5 max 35 at lvl 50)
            effects[1] = (int)(UnityEngine.Random.Range(0, currentLevel / 5));                      //Dex (min 0 max 10 at lvl 50)
            effects[2] = (int)(UnityEngine.Random.Range(currentLevel / 1.25f, currentLevel));       //Str (min 40 max 50 at lvl 50)
            effects[3] = (int)(UnityEngine.Random.Range(0, currentLevel / 10));                     //Int (min 0  max 5 at lvl 50)
            effects[4] = (int)(UnityEngine.Random.Range(currentLevel / 2.5f, currentLevel / 1.66f));   //Con (min 20 max 30 at lvl 50)
            effects[5] = 100; //Durability
          
            return axe;
        }
        else if (rand <= 50 && rand > 25)   //Bow
        {           
            effects[0] = (int)(UnityEngine.Random.Range(currentLevel / 1.25f, currentLevel / 1.1f));//Lck (min 40 max 45 at lvl 50)
            effects[1] = (int)(UnityEngine.Random.Range(currentLevel / 1.25f, currentLevel / 1.1f));//Dex (min 40 max 45 at lvl 50)
            effects[2] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 3.3f));   //Str (min 5 max 15 at lvl 50)
            effects[3] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 2.5f));   //Int (min 5  max 20 at lvl 50)
            effects[4] = (int)(UnityEngine.Random.Range(0, currentLevel / 10));                     //Con (min 0 max 5 at lvl 50)
            effects[5] = 100; //Durability
          
            return bow;
        }
        else  //Staff
        {
            
            effects[0] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 5));  //Lck (min 5 max 10 at lvl 50)
            effects[1] = (int)(UnityEngine.Random.Range(currentLevel / 10, currentLevel / 5));  //Dex (min 5 max 10 at lvl 50)
            effects[2] = (int)(UnityEngine.Random.Range(0, currentLevel / 10));                 //Str (min 0 max 5 at lvl 50)
            effects[3] = (int)(UnityEngine.Random.Range(currentLevel, currentLevel / 0.83f));   //Int (min 50  max 60 at lvl 50)
            effects[4] = (int)(UnityEngine.Random.Range(0, currentLevel / 10));                 //Con (min 0 max 5 at lvl 50)
            effects[5] = 100; //Durability
         
            return staff;
        }        
    }   
}
