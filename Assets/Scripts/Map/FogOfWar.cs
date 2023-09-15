using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    //Game objects

    public GameObject cloud;
    
    public List<GameObject> cloudChunks;
    private GameObject currentChunk;

    public GameObject player;

    public GameObject cloudInstancePool;
    
    //Variables

    public int viewDist;
    public int revealDist;
    public int width;
    public int height;

    private int chunkCount;
    public int chunkSize;

    private int ylimit;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MakeFog());
    }
    public IEnumerator MakeFog()
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player"));
        width = 256;
        height = 256;

        chunkCount = 0;

        GameObject chunk = new GameObject();
        cloudChunks = new List<GameObject>();
        chunk.name = "chunk" + chunkCount.ToString();
        chunk.transform.position = new Vector3((int)(chunkSize / 2), 0,(int)(chunkSize / 2));
        chunk.transform.parent = cloudInstancePool.transform;
        currentChunk = chunk;

        CreateFog();

        player = GameObject.FindGameObjectWithTag("Player");
        UpdateFog();
}


    //Divide into clusters
    public void CreateFog()
    {
        ylimit = -(int)(height / 2)-1;

        int x = -(int)(width-1)-1;
        while (x <= (int)(width/2))
        {
            if(x % chunkSize == 0 )
            {
                if(ylimit + chunkSize < height)
                {
                    x = x - chunkSize;
                    ylimit = ylimit + chunkSize;
                }
                else
                {
                    ylimit = -(int)(height / 2);                    
                }                
            }           
            
            for (int y = ylimit; y < ylimit + chunkSize; y+=2)
            {
                if ((x % chunkSize == 0) && (y % chunkSize == 0))
                {
                    chunkCount++;
                    GameObject chunk = new GameObject();                   
                    chunk.name = "chunk" + chunkCount.ToString();
                    chunk.transform.position = new Vector3(x + (int)(chunkSize/2), 0, y + (int)(chunkSize / 2));
                    chunk.transform.parent = cloudInstancePool.transform;

                    cloudChunks.Add(chunk);

                    currentChunk = chunk;
                }

                GameObject cloudInstance = Instantiate(cloud, new Vector3(x , 1.3f, y), Quaternion.identity);
                cloudInstance.transform.parent = currentChunk.transform;                
            }
            x+=2;
        }
    }
    public void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (Input.GetMouseButtonDown(0))
        {
            UpdateFog();
        }
    }

    public void UpdateFog()
    {
        //Remove clouds at a close distance to the player
        float chunkX;
        float chunkY;
        float childX;
        float childY;
        float playerX;
        float playerY;
        Transform chunk;
        for (int i = 0; i < cloudChunks.Count; i++) //Find chunks in range of player
        {
            chunkX = cloudChunks[i].transform.position.x;
            chunkY = cloudChunks[i].transform.position.z;

            playerX = player.transform.position.x;
            playerY = player.transform.position.z;

            //If chunk is close enough to player, check its clouds if they are to be revealed
            if (Mathf.Pow(Mathf.Pow(chunkX - playerX, 2) + Mathf.Pow(chunkY - playerY, 2), 0.5f) < viewDist)
            {
                chunk = cloudChunks[i].transform;
                for (int j = 0; j < cloudChunks[i].transform.childCount; j++)
                {
                    childX = chunk.GetChild(j).transform.position.x;
                    childY = chunk.GetChild(j).transform.position.z;
                    if(Mathf.Pow(Mathf.Pow(childX - playerX, 2) + Mathf.Pow(childY - playerY, 2), 0.5f) < revealDist)
                    {
                        GameObject child = chunk.GetChild(j).gameObject;
                        
                        Destroy(child);
                    }
                }
            }            
        }             
    }
}
