using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    //objects and Managers

    private Camera mainCamera;    
    private GameObject ambientManager;
    private GameObject progressManager;
    private GameObject spriteManager;

    //Bools 

    public bool playerTurn;
    public bool moveMade;

    private bool moveUp;
    private bool moveDown;
    private bool moveLeft;
    private bool moveRight;

    //Properties

    public int keys;
    private float health;

    public float maxMoveDist;    

    public float rotateSpeed;
    public float moveSpeed;

    public float spellCost;

    public float obstructionDist;
    public float range;
    public float critMultiplier;

    public float zoomLevel;
    
    //Labels

    public GameObject dodged;
    public GameObject outOfRange;
    public GameObject noClearView;
    public GameObject outOfArrows;
    public GameObject missed;
    public GameObject noMana;
    public GameObject damageLabel;

    //Temp
    RaycastHit hitInfo;
    Vector3 pos;
    public float currentMoveDist;

    private float damage;
    private float defence;

    private float damageTaken;
    private float newHealth;

    private int weapon;
    private float durability;

    private int lckVal;
    private int dexVal;
    private int strVal;
    private int intVal;
    private int conVal;

    private int mysteriousPotDur;

    // Start is called before the first frame update
    void Start()
    {
        ambientManager = GameObject.FindGameObjectWithTag("AmbientManager");
        progressManager = GameObject.FindGameObjectWithTag("ProgressManager");
        spriteManager = GameObject.FindGameObjectWithTag("SpriteManager");
        if (progressManager != null)
        {
            mainCamera = Camera.main;
            mainCamera.enabled = true;
            mainCamera.transform.parent = gameObject.transform;
        
            health = PlayerPrefs.GetFloat("Health");

            //Default values
            playerTurn = true;
            moveMade = false;

            zoomLevel = 6;
            maxMoveDist = 1.0f;
            currentMoveDist = 0f;
            keys = 0;
            obstructionDist = 0.8f;
            range = 7f;
            critMultiplier = 1.5f;
            spellCost = 3f;

            moveUp = false;
            moveDown = false;
            moveLeft = false;
            moveRight = false;        
        }        
        PlayerPrefs.SetInt("Turn", 0); // 0 = player turn, 1 = enemy Turn        
    }

    // Update is called once per frame
    void Update()
    {
        if(progressManager != null)
        {
            mainCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f); //Keep Camera always in same direction
            mainCamera.transform.localPosition = new Vector3(0, zoomLevel, 0);
        }        
        if (playerTurn)
        {
            if (Input.GetKeyDown("w"))
            {
                MoveUp();
                PlayerPrefs.SetInt("Turn", 0);
                progressManager.GetComponent<TimeManager>().UpdateTime(0, 5);
            }
            else if (Input.GetKeyDown("s"))
            {
                MoveDown();
                PlayerPrefs.SetInt("Turn", 0);
                progressManager.GetComponent<TimeManager>().UpdateTime(0, 5);
            }
            else if (Input.GetKeyDown("a"))
            {
                MoveLeft();
                PlayerPrefs.SetInt("Turn", 0);
                progressManager.GetComponent<TimeManager>().UpdateTime(0, 5);
            }
            else if (Input.GetKeyDown("d"))
            {
                MoveRight();
                PlayerPrefs.SetInt("Turn", 0);
                progressManager.GetComponent<TimeManager>().UpdateTime(0, 5);
            }            
        }
    }


    IEnumerator EndofTurn() //Swap turns
    {
        ambientManager.GetComponent<FogOfWar>().UpdateFog();

        yield return new WaitForSeconds(0.2f); //Delay

        playerTurn = false;
        spriteManager.GetComponent<CombatManager>().EnemyTurn();

        mysteriousPotDur = PlayerPrefs.GetInt("MystDuration");
        PlayerPrefs.SetInt("MystDuration", mysteriousPotDur - 1);
        progressManager.GetComponent<StatsManager>().UpdateStats();
    }

    private void FixedUpdate()
    {
        if (PlayerPrefs.GetInt("Turn") == 0)
        {
            if (moveUp)
            {
                if (gameObject.transform.rotation.eulerAngles.y > 3)
                {
                    gameObject.transform.Rotate(0f, -rotateSpeed, 0f);
                }
                else if (gameObject.transform.rotation.eulerAngles.y < -3)
                {
                    gameObject.transform.Rotate(0f, rotateSpeed, 0f);
                }
                else
                {
                    if (currentMoveDist < maxMoveDist)
                    {
                        gameObject.transform.position = gameObject.transform.position + new Vector3(0f, 0f, moveSpeed);
                        currentMoveDist = currentMoveDist + moveSpeed;
                    }
                    else
                    {
                        moveUp = false;
                        StartCoroutine(EndofTurn());
                    }
                }
            }
            if (moveDown)
            {
                if (gameObject.transform.rotation.eulerAngles.y > 183)
                {
                    gameObject.transform.Rotate(0f, -rotateSpeed, 0f);
                }
                else if (gameObject.transform.rotation.eulerAngles.y < 177)
                {
                    gameObject.transform.Rotate(0f, rotateSpeed, 0f);
                }
                else
                {
                    if (currentMoveDist < maxMoveDist)
                    {
                        gameObject.transform.position = gameObject.transform.position + new Vector3(0f, 0f, -moveSpeed);
                        currentMoveDist = currentMoveDist + moveSpeed;
                    }
                    else
                    {
                        moveDown = false;
                        StartCoroutine(EndofTurn());
                    }
                }
            }
            if (moveLeft)
            {
                if (gameObject.transform.rotation.eulerAngles.y > 273)
                {
                    gameObject.transform.Rotate(0f, -rotateSpeed, 0f);
                }
                else if (gameObject.transform.rotation.eulerAngles.y < 267)
                {
                    gameObject.transform.Rotate(0f, rotateSpeed, 0f);
                }
                else
                {
                    if (currentMoveDist < maxMoveDist)
                    {
                        gameObject.transform.position = gameObject.transform.position + new Vector3(-moveSpeed, 0f, 0f);
                        currentMoveDist = currentMoveDist + moveSpeed;
                    }
                    else
                    {
                        moveLeft = false;
                        StartCoroutine(EndofTurn());
                    }
                }
            }

            if (moveRight)
            {
                if (gameObject.transform.rotation.eulerAngles.y > 93)
                {
                    gameObject.transform.Rotate(0f, -rotateSpeed, 0f);
                }
                else if (gameObject.transform.rotation.eulerAngles.y < 87)
                {
                    gameObject.transform.Rotate(0f, rotateSpeed, 0f);
                }
                else
                {
                    if (currentMoveDist < maxMoveDist)
                    {
                        gameObject.transform.position = gameObject.transform.position + new Vector3(moveSpeed, 0f, 0f);
                        currentMoveDist = currentMoveDist + moveSpeed;
                    }
                    else
                    {
                        moveRight = false;
                        StartCoroutine(EndofTurn());
                    }
                }
            }
        }
        if (health <= 0)
        {
            if(progressManager != null)
            {
                progressManager.GetComponent<StatsManager>().Death();
            }            
        }
    }
    public float Attack(GameObject target)
    {
        weapon = PlayerPrefs.GetInt("Weapon");
        durability = PlayerPrefs.GetFloat("WeaponDur");

        lckVal = PlayerPrefs.GetInt("LCK");
        dexVal = PlayerPrefs.GetInt("DEX");
        strVal = PlayerPrefs.GetInt("STR");
        intVal = PlayerPrefs.GetInt("INT");

        if (weapon == 1 || weapon == 2) //Sword or Axe
        {
            if (Vector3.Distance(gameObject.transform.position, target.transform.position) < 1.5) 
            {                
                if (weapon == 1) //Sword
                {
                    //Depends on strength (75%), dexterity(40%), durability
                    damage = (float)((strVal * 0.75f) * (dexVal * 0.4f) * (((3f * durability)/400f) + (1f/4f)));
                }
                else //Axe
                {
                    //Depends on strength (95%), dexterity(20%), durability
                    damage = (float)((strVal * 0.95f) * (dexVal * 0.2f) * (((3f * durability) / 400f) + (1f / 4f)));
                }

                StartCoroutine(EndofTurn());
                progressManager.GetComponent<TimeManager>().UpdateTime(0, 3); //Meele takes 3 minutes
                if (UnityEngine.Random.Range(0f, 1f) < (lckVal / Mathf.Sqrt(Mathf.Pow(lckVal, 2) + 2500f))) //Higher luck, less likely hit (see defence)
                {
                    return damage * critMultiplier;
                }
                else
                {
                    return damage;
                }                
            }
            else
            {
                OutofRangeWarning();
                return -1;
            }
        }        
        else if (weapon == 3) //Bow
        {
            if (Vector3.Distance(gameObject.transform.position, target.transform.position) < range)
            {
                Physics.Linecast(gameObject.transform.position, target.transform.position, out RaycastHit hit);
                if (hit.collider != null && hit.collider.tag != "Enemy")
                {                    
                    NotClearWarning();
                    return -1;
                }
                else
                {
                    if (progressManager.GetComponent<Inventory>().ConsumeArrow())
                    {
                        StartCoroutine(EndofTurn());
                        if (UnityEngine.Random.Range(0f, 1f) < (dexVal / (dexVal + 1f))) //at Dex 9, 90% to hit
                        {
                            //Depends on Dexterity (75%), Luck(20%), strength(10%), durability
                            damage = (float)((dexVal * 0.75f) * (lckVal * 0.2f) * (strVal * 0.1f) * (((3f * durability) / 400f) + (1f / 4f)));
                            if (UnityEngine.Random.Range(0f, 1f) < (lckVal / Mathf.Sqrt(Mathf.Pow(lckVal, 2) + 2500f))) //Higher luck, less likely hit (see defence)
                            {                                
                                return damage * critMultiplier;
                            }
                            else
                            {
                                return damage;
                            }
                        }
                        else
                        {
                            progressManager.GetComponentInChildren<ProgressManager>().MissedArrow(target);
                            ArrowMissedWarning();
                            return -1;
                        }                            
                    }
                    else
                    {
                        OutOfArrowsWarning();
                        return -1;
                    }
                }
                PlayerPrefs.SetInt("Turn", 0);
                progressManager.GetComponent<TimeManager>().UpdateTime(0, 2); //Ranged takes 2 minutes
            }
            else
            {
                OutofRangeWarning();
                return -1;
            }
        }
        else if (weapon == 4) //Staff
        {
            if (Vector3.Distance(gameObject.transform.position, target.transform.position) < range * 2)
            {
                Physics.Linecast(gameObject.transform.position, target.transform.position, out RaycastHit hit);
                if (hit.collider.tag != "Enemy")
                {
                    NotClearWarning();
                    return -1;
                }
                else
                {
                    if (PlayerPrefs.GetFloat("Mana") > spellCost)
                    {                        
                        PlayerPrefs.SetFloat("Mana", PlayerPrefs.GetFloat("Mana") - spellCost); //Reduce Mana
                        StartCoroutine(EndofTurn());
                        progressManager.GetComponent<TimeManager>().UpdateTime(0, 4); //Magic takes 4 minutes

                        //Depends on Intelligence (85%), dexterity(10%), durability
                        damage = (float)((intVal * 0.85f) * (dexVal * 0.1f) * (((3f * durability) / 400f) + (1f / 4f)));                 
                        if (UnityEngine.Random.Range(0f, 1f) < (lckVal / Mathf.Sqrt(Mathf.Pow(lckVal, 2) + 2500f))) //Higher luck, less likely hit (see defence)
                        {
                            return damage * critMultiplier;
                        }
                        else
                        {
                            return damage;
                        }
                    }
                    else
                    {
                        NoManaWarning();
                        return -1;
                    }
                }                                
            }
            else
            {
                OutofRangeWarning();
                return -1;
            }
        }
        return 0;
    }    
    public void OutofRangeWarning()
    {
        GameObject warning = Instantiate(outOfRange,new Vector3(0,30,0), Quaternion.identity);
        warning.transform.parent = GameObject.FindGameObjectWithTag("ScreenOverlay").transform;
        warning.transform.localPosition = new Vector3(-130, 0, 0);
        Destroy(warning, 3);
    }
    public void NotClearWarning()
    {
        GameObject warning = Instantiate(noClearView, new Vector3(0, 30, 0), Quaternion.identity);
        warning.transform.parent = GameObject.FindGameObjectWithTag("ScreenOverlay").transform;
        warning.transform.localPosition = new Vector3(-130, 0, 0);
        Destroy(warning, 3);
    }
    public void OutOfArrowsWarning()
    {
        GameObject warning = Instantiate(outOfArrows, new Vector3(0, 30, 0), Quaternion.identity);
        warning.transform.parent = GameObject.FindGameObjectWithTag("ScreenOverlay").transform;
        warning.transform.localPosition = new Vector3(-130, 0, 0);
        Destroy(warning, 3);
    }
    public void ArrowMissedWarning()
    {
        GameObject warning = Instantiate(missed, new Vector3(0, 30, 0), Quaternion.identity);
        warning.transform.parent = GameObject.FindGameObjectWithTag("ScreenOverlay").transform;
        warning.transform.localPosition = new Vector3(-130, 0, 0);
        Destroy(warning, 3);
    }
    public void NoManaWarning()
    {
        GameObject warning = Instantiate(noMana, new Vector3(0, 30, 0), Quaternion.identity);
        warning.transform.parent = GameObject.FindGameObjectWithTag("ScreenOverlay").transform;
        warning.transform.localPosition = new Vector3(-130, 0, 0);
        Destroy(warning, 3);
    }    
    public void RecieveDamage(float damage)
    {
        lckVal = PlayerPrefs.GetInt("LCK");        
        
        if (UnityEngine.Random.Range(0,1) < (lckVal / Mathf.Sqrt(Mathf.Pow(lckVal, 2) + 2500f))) //Higher luck, less likely hit (see defence)
        {
            dexVal = PlayerPrefs.GetInt("DEX");
            strVal = PlayerPrefs.GetInt("STR");
            conVal = PlayerPrefs.GetInt("CON");

            defence = (float)(strVal * 0.3) + (float)(dexVal * 0.3) + (float)(conVal * 0.1); //Defence is 30% strength, 30% Dexterity, 10% consitution
            damageTaken = damage * (defence / Mathf.Sqrt(Mathf.Pow(defence, 2) + 2500f));    //Armour makes asymptoptic function | at lvl 15 28% resistance, at lvl 50 70% resistance, at lvl 100 89% resistance                                                                         
            newHealth = PlayerPrefs.GetFloat("Health") - damageTaken;

            PlayerPrefs.SetFloat("Health", newHealth);  //x / (x^2 + 2500)^0.5
            progressManager.GetComponent<StatsManager>().UpdateStats();

            GameObject damageText = Instantiate(damageLabel, new Vector3(0, 0, 0), Quaternion.identity);
            damageText.GetComponent<Text>().text = damageTaken.ToString("F2") + " Damage taken ";
            damageText.transform.parent = GameObject.FindGameObjectWithTag("ScreenOverlay").transform;
            damageText.transform.localPosition = new Vector3(-130, 0, 0);
            health = health - damageTaken; //x / (x^2 + 2500)^0.5

            health = newHealth;
        }
        else
        {
            GameObject dodge = Instantiate(dodged, new Vector3(0, 30, 0), Quaternion.identity);
            dodge.transform.parent = GameObject.FindGameObjectWithTag("ScreenOverlay").transform;
            Destroy(dodge,3);
        }        
    }

    public void MoveUp()
    {
        pos = gameObject.transform.position;
        if (!Physics.Raycast(pos, new Vector3(0f,0f, obstructionDist), out hitInfo, obstructionDist))
        {
            moveUp = true;
            currentMoveDist = 0f;
            playerTurn = false;
            spriteManager.GetComponent<CombatManager>().EnemyTurn();
        }
    }
    public void MoveDown()
    {
        pos = gameObject.transform.position;
        if (!Physics.Raycast(pos, new Vector3(0f, 0f, -obstructionDist), out hitInfo, obstructionDist))
        {           
            moveDown = true;
            currentMoveDist = 0f;
            playerTurn = false;
            spriteManager.GetComponent<CombatManager>().EnemyTurn();
        }        
    }    
    public void MoveLeft()
    {
        pos = gameObject.transform.position;
        if (!Physics.Raycast(pos, new Vector3(-obstructionDist, 0f, 0f), out hitInfo, obstructionDist))
        {
            moveLeft = true;
            currentMoveDist = 0f;
            playerTurn = false;
            spriteManager.GetComponent<CombatManager>().EnemyTurn();
        }
    }    
    public void MoveRight()
    {
        pos = gameObject.transform.position;
        if (!Physics.Raycast(pos, new Vector3(obstructionDist, 0f, 0f), out hitInfo, obstructionDist))
        {
            moveRight = true;
            currentMoveDist = 0f;
            playerTurn = false;
            spriteManager.GetComponent<CombatManager>().EnemyTurn();
        }
    }
}
