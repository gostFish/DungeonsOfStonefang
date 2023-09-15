using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{

    //Manager objects
    private GameObject player;
    private CombatManager enemyManager;


    //Bools
    public bool isMelee;
    public bool isRanged;
    public bool isMagic;

    public bool enemyTurn;

    //Stats and properties
    public float weak;
    public float normal;
    public float high;
    public float veryHigh;

    private int speed;
    private int melee;
    private int ranged;
    private int magic;
    private int defence;

    public float health;

    public int engageDistance;
    public int attackRange;

    public float meleeRange;
    public float rangedRange;
    public float magicRange;

    public float rotateSpeed;
    public float moveSpeed;

    private float maxMoveDist;

    private float damageMultiplier;
    private float critMultiplier;

    //Temp Variables
      
    private float currentMoveDist;    
    public float obstructionDist;

    private float currentLevel;
    
    private float playerDistance;

    private float damageTaken;

    public bool done;
    private Vector3 newPos;

    //Enemy UI (Damage taken)

    public GameObject damageLabel;


    void Start()
    {
        engageDistance = 10;
        attackRange = 3;

        health = (float)(PlayerPrefs.GetInt("CurrentLevel")* 2);

        maxMoveDist = 1.0f;
        currentMoveDist = 0f;
        obstructionDist = 0.8f;

        enemyManager = GameObject.FindGameObjectWithTag("SpriteManager").GetComponent<CombatManager>();
        gameObject.GetComponent<NavMeshAgent>().isStopped = false;

        currentLevel = (float)(PlayerPrefs.GetInt("CurrentLevel"));

        //E 0 - weak | D/C = weak-normal | B = normal - high | A = high -veryHigh | S = veryHigh - 100

        //Stats increase rate slowers over time (make player feel they are getting stronger)
        weak = 1.4f;        //lvl 1 ~ stat 1,  lvl 15 ~ stat 5  | lvl 50 ~ stat 10 
        normal = 4.3f;      //lvl 1 ~ stat 4,  lvl 15 ~ stat 17 | lvl 50 ~ stat 30 
        high = 8.5f;        //lvl 1 ~ stat 9,  lvl 15 ~ stat 33 | lvl 50 ~ stat 60 
        veryHigh = 12f;     //lvl 1 ~ stat 12, lvl 15 ~ stat 46 | lvl 50 ~ stat 85 

        damageMultiplier = 0.8f;
        critMultiplier = 1.5f;

        SetStats();
        StartCoroutine(FindPlayer());
    }

    IEnumerator FindPlayer() //Wait for player to spawn
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player"));
        player = GameObject.FindGameObjectWithTag("Player");
    }

        public void MakeMove()
    {        
        playerDistance = Vector3.Distance(player.transform.position, gameObject.transform.position);
        if (playerDistance < engageDistance) //In range, so move to player
        {
            maxMoveDist = 1 + (int)((speed - 14) / 30); //Move extra tile every 30 speed

            if (isMagic && playerDistance < attackRange) //Attack is priority
            {
                MagicAttack();
                done = true;
            }
            else if (isMelee)
            {
                if (isRanged && playerDistance > 1 && playerDistance < attackRange) //May be both Meele and ranged
                {
                    RangedAttack();
                }
                else if (playerDistance < 1)
                {
                    MeleeAttack();
                }
                done = true;
            }
            else if (isRanged && playerDistance < attackRange) //Attack may be both meele and ranged (check meele first)
            {
                RangedAttack();
                done = true;
            }

            else
            {
                newPos = newPos + new Vector3(0, 0.0001f, 0); //No longer value by reference

                currentMoveDist = maxMoveDist;
                gameObject.GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
                gameObject.GetComponent<NavMeshAgent>().isStopped = false;
            }
        }
        else
        {
            done = true; //End turn for this enemy
        }
    }


    void Update()
    {
        if(currentMoveDist > 0)//Check if a new tile was moved
        {           
            if (Vector3.Distance(newPos, gameObject.transform.position) > 1){
                newPos = gameObject.transform.position;
                newPos = newPos + new Vector3(0, 0.0001f, 0);
                currentMoveDist--;
                done = true;
            }
        }        
    }

    void OnMouseDown()
    {
        enemyManager.EnemySelected(gameObject);
    }

    public void RecieveDamage(float damage)
    {
        //Armour makes asymptoptic function | at lvl 15 28% resistance, at lvl 50 70% resistance, at lvl 100 89% resistance
        damageTaken = (damage * (1- (defence / Mathf.Sqrt(Mathf.Pow(defence, 2) + 2500f))));
        
        GameObject damageText = Instantiate(damageLabel, new Vector3(0,0,0),Quaternion.identity);
        if(damage > 0)
        {
            damageText.GetComponent<Text>().text = gameObject.name.Replace("(Clone)", "") + ": " + (health + damage).ToString("F2") + " - " + damageTaken.ToString("F2");
            health = health - damageTaken; //x / (x^2 + 2500)^0.5
        }
        else if(damage == -1)
        {
            damageText.GetComponent<Text>().text = "";
        }
        else
        {
            damageText.GetComponent<Text>().text = "Attack on " + gameObject.name.Replace("(Clone)", "") + " Failed";
        }        
        damageText.transform.parent = GameObject.FindGameObjectWithTag("ScreenOverlay").transform;
        damageText.transform.localPosition = new Vector3(-130, -30, 0);
        Destroy(damageText, 3f);
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void MeleeAttack() //Attacks always on player
    {          
            if(UnityEngine.Random.Range(0f, 10f) > 1f)
            {
                player.GetComponent<PlayerController>().RecieveDamage(melee * damageMultiplier);
            }
            else
            {
                player.GetComponent<PlayerController>().RecieveDamage(melee * critMultiplier); //Crit
            }              
    }

    public void RangedAttack()
    {       
            if (UnityEngine.Random.Range(0f, 10f) > 1f)
            {
                player.GetComponent<PlayerController>().RecieveDamage(ranged * damageMultiplier);
            }
            else
            {
                player.GetComponent<PlayerController>().RecieveDamage(ranged * critMultiplier); //Crit
            }      
    }
    public void MagicAttack()
    {
    
            if (UnityEngine.Random.Range(0f, 10f) > 1f)
            {
                player.GetComponent<PlayerController>().RecieveDamage(magic * damageMultiplier);
            }
            else
            {
                player.GetComponent<PlayerController>().RecieveDamage(magic * critMultiplier); //Crit
            }
    
    }

    private void SetStats()
    {
        if (gameObject.name.Contains("Golem"))
        {
            RandomGolemStats();
        }else if (gameObject.name.Contains("Berserk"))
        {
            RandomBerserkStats();
        }
        else if (gameObject.name.Contains("Knight"))
        {
            RandomKnightStats();
        }
        else if (gameObject.name.Contains("Mage"))
        {
            RandomMageStats();
        }
        else if (gameObject.name.Contains("Rouge"))
        {
            RandomRougeStats();
        }
        else if (gameObject.name.Contains("Ghost"))
        {
            RandomGhostStats();
        }

    }
    private void RandomGolemStats() //Same as Document
    {
        speed = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(weak, normal));
        melee = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(high, veryHigh));
        ranged = 0;
        magic = 0;
        defence = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(high, veryHigh));
    }
    private void RandomBerserkStats() //knight with more attack than defence
    {
        speed = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(normal, high));
        melee = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(normal, high));
        ranged = 0;
        magic = 0;
        defence = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(weak, normal));
    }
    private void RandomKnightStats() // berserk with more defece than attack
    {
        speed = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(normal, high));
        melee = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(weak, normal));
        ranged = 0;
        magic = 0;
        defence = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(normal, high));
    }
    private void RandomMageStats() //Sceleton Archer stats (swapped range with magic)
    {
        speed = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(weak, normal));
        melee = 0;
        ranged = 0;
        magic = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(weak, normal));
        defence = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(weak, normal));
    }
    private void RandomRougeStats() //Spider stats
    {
        speed = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(normal, high));
        melee = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(normal, high));
        ranged = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(weak, normal));
        magic = 0;
        defence = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(weak, normal));
    }
    private void RandomGhostStats() //Imp stats
    {
        speed = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(veryHigh, 100));
        melee = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(weak, normal));
        ranged = 0;
        magic = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(normal, high));
        defence = (int)(Mathf.Sqrt(currentLevel) * UnityEngine.Random.Range(0, weak));
    }
}

public class DeepCopy{ //Attempt to make variable by value, not by reference
    Vector3 position;
    public DeepCopy(Vector3 location)
    {
        this.position = location;
    }
    public Vector3 GetCopy()
    {
        return position;
    }
}
