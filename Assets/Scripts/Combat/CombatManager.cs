using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    //Fight targets

    private List<GameObject> enemies;
    private GameObject player;
    public GameObject enemyPool;
    
    private GameObject targetEnemy; //Selected Enemy

    //Fight Menu

    public GameObject selectMenu;

    public GameObject cancel;

    public GameObject MeeleAttack;
    public GameObject RangedAttack;
    public GameObject MagicAttack1;
    public GameObject MagicAttack2;
    public GameObject MagicAttack3;
    public GameObject MagicAttack4;

    //Temp
    private int weapon;
    private float attack;

    void Start()
    {
        enemies = new List<GameObject>();
        StartCoroutine(FindPlayer());
    }

    IEnumerator FindPlayer() //Wait for player to spawn
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player")); //Player spawned, therefore so did enemies
        player = GameObject.FindGameObjectWithTag("Player");        
    }

    IEnumerator DoMoves() //Let each Enemy try a move
    {
        //Find enemies again each time incase one is killed
        enemies = new List<GameObject>();
        for (int i = 0; i < enemyPool.transform.childCount; i++)
        {
            enemies.Add(enemyPool.transform.GetChild(i).gameObject);            
        }

        for( int i = 0; i < enemies.Count; i++)
        {
            //Enemy stops being an obsacle and becomes an agent for its turn            
                enemies[i].transform.GetComponent<NavMeshObstacle>().enabled = false;
                enemies[i].transform.GetComponent<NavMeshAgent>().enabled = true;
                enemies[i].transform.GetComponent<EnemyController>().done = false;
                enemies[i].transform.GetComponent<EnemyController>().MakeMove();         
            
            yield return new WaitUntil(() => enemies[i].transform.GetComponent<EnemyController>().done == true);
            
            enemies[i].transform.GetComponent<NavMeshAgent>().enabled = false;
            enemies[i].transform.GetComponent<NavMeshObstacle>().enabled = true;
        }    
        if(player != null)
        {
            player.transform.GetComponent<PlayerController>().playerTurn = true;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            player.transform.GetComponent<PlayerController>().playerTurn = true;
        }
        
    }

    public void EnemySelected(GameObject enemy)
    {
        selectMenu.SetActive(true);
        
        targetEnemy = enemy;
        weapon = PlayerPrefs.GetInt("Weapon");

        if (weapon == 1 || weapon ==2)
        {
            ShowMeele();
        }
        else if (weapon == 3)
        {
            ShowRanged();
        }
        else if (weapon == 4)
        {
            ShowMagic();
        }
        cancel.SetActive(true);
    }

    //UI revealing / Hiding
    public void CancelSelect()
    {
        selectMenu.SetActive(false);

        MeeleAttack.SetActive(false);
        RangedAttack.SetActive(false);

        MagicAttack1.SetActive(false);
        MagicAttack2.SetActive(false);
        MagicAttack3.SetActive(false);
        MagicAttack4.SetActive(false);
        cancel.SetActive(false);
    }

    //More UI Methods
    public void ShowMeele()
    {
        MeeleAttack.SetActive(true);
    }
    public void ShowRanged()
    {
        RangedAttack.SetActive(true);
    }
    public void ShowMagic()
    {
        MagicAttack1.SetActive(true);
        MagicAttack2.SetActive(true);
        MagicAttack3.SetActive(true);
        MagicAttack4.SetActive(true);
    }
    public void SendAttack()
    {
        attack = player.GetComponent<PlayerController>().Attack(targetEnemy);
        targetEnemy.GetComponent<EnemyController>().RecieveDamage(attack);
        CancelSelect();
    }
    public void EnemyTurn()
    {
        StartCoroutine(DoMoves());
    }
}
