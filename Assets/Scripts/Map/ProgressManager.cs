using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressManager : MonoBehaviour
{

    public int currentLevel;
    public int newestLevel;

    //Instantiatable objects

    public GameObject caveTile1;
    public GameObject caveTile2;
    public GameObject caveWall1;
    public GameObject caveWall2;
    public GameObject tile1;
    public GameObject tile2;
    public GameObject wall1;
    public GameObject wall2;

    public GameObject nextLevel;
    public GameObject previousLevel;

    public GameObject ghost;
    public GameObject golem;
    public GameObject skeletonBererk;
    public GameObject skeletonKnight;
    public GameObject skeletonMage;
    public GameObject skeletonRouge;

    public GameObject bigCaveChest;
    public GameObject smallCaveChest;

    public GameObject bigRoomChest;
    public GameObject smallRoomChest;

    public GameObject trap1;
    public GameObject trap2;
    public GameObject trap3;

    public GameObject key;
    public GameObject door;

    public GameObject arrow;

    private static List<GameObject> levelObjects;

    [SerializeField]
    private Data levelData;

    //Script holders

    private GameObject caveGenerator;
    private GameObject roomGenerator;
    private GameObject ambientManager;

    //Instance Pools

    public GameObject floorInstancePool;
    public GameObject wallInstancePool;
    public GameObject enemyInstancePool;
    public GameObject trapInstancePool;
    public GameObject collectablesInstancePool;

    //Objects and lists
    private static Camera mainCamera;
    private GameObject player;

    private GameObject[] nonFirstLevel;
    private GameObject[] nonFinalLevel;

    //Variables
    public bool islast;
    public bool isfirst;

    public int roomSize;
    public int trapBudget;
    public int doorbudget;
    public int enemyBudget;

    //Temp
    private Vector3 startPos;
    private Vector3 endPos;
    int size;


    void Start()
    {
        //levelIndex = SceneManager.GetActiveScene().buildIndex;
        mainCamera = Camera.main;
        caveGenerator = GameObject.FindGameObjectWithTag("CaveGenerator");
        roomGenerator = GameObject.FindGameObjectWithTag("RoomGenerator");
        ambientManager = GameObject.FindGameObjectWithTag("AmbientManager");

        levelObjects = new List<GameObject>();

        nonFirstLevel = GameObject.FindGameObjectsWithTag("nonFirstLevel");
        nonFinalLevel = GameObject.FindGameObjectsWithTag("nonFinalLevel");               

        if (!PlayerPrefs.HasKey("CurrentLevel"))
        {           
            PlayerPrefs.SetInt("CurrentLevel", 1);
            PlayerPrefs.SetInt("NewestLevel", 1);
            MakeNewLevel(1);
        }
        UpdateLevel(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            UpdateLevel(true);
        }
    }

    public void UpdateLevel(bool next) //Next or previous level
    {

        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        newestLevel = PlayerPrefs.GetInt("NewestLevel");
        Debug.Log("Current Level: " + currentLevel);
        if (next)
        {            
            currentLevel = currentLevel + 1;
            if (newestLevel < currentLevel)
            {                
                    newestLevel++;                
                
                PlayerPrefs.SetInt("CurrentLevel", currentLevel);
                PlayerPrefs.SetInt("NewestLevel", newestLevel);

                MakeNewLevel(currentLevel);
            }
            else //return to level
            {
                LoadData(currentLevel);
                PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            }
        }
        else
        {
            currentLevel = currentLevel--;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);

            LoadData(currentLevel);
        }

        if (currentLevel == 1)
        {
            foreach (GameObject obj in nonFirstLevel)
            {
                obj.SetActive(false);
            }
        }
        else if (currentLevel == 50)
        {
            foreach (GameObject obj in nonFinalLevel)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject obj in nonFirstLevel)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in nonFinalLevel)
            {
                obj.SetActive(true);
            }
        }
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera.transform.parent = player.transform;
    }

    public void MakeNewLevel(int levelIndex)
    {
        SaveData(levelIndex - 1); //Save Initial level
        ClearLevel();

        //Every 5 levels (9 progressions) width/height increses by 20: so: 48,74,100,126,152,178,204,230, 256
        //At level 49 will be 256 by 256,
        //Level 50 will be one medium room

        size = 47 + ((int)(currentLevel/5)*13); //Every 5 levels gets larger by 13 x 13
        caveGenerator.GetComponent<GenerateCave>().width = size;
        caveGenerator.GetComponent<GenerateCave>().height = size;

        roomGenerator.GetComponent<GenerateRooms>().width = size;
        roomGenerator.GetComponent<GenerateRooms>().height = size;

        if (levelIndex < 10 || levelIndex % 5 == 0)
        {            
            caveGenerator.GetComponent<GenerateCave>().seed = levelIndex.ToString();
            caveGenerator.GetComponent<GenerateCave>().GenerateMap();
        }
        else
        {
            roomGenerator.GetComponent<GenerateRooms>().seed = levelIndex.ToString();
            roomGenerator.GetComponent<GenerateRooms>().GenerateMap();
        }        
    }
    private static void ClearLevel()
    {
        //Clear current level content
        List<GameObject> objects = new List<GameObject>();
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");
        GameObject[] collecatbles = GameObject.FindGameObjectsWithTag("Collectable");
        GameObject[] essentials = GameObject.FindGameObjectsWithTag("Essentials");

        foreach (GameObject obj in floors)
        {
            objects.Add(obj);
        }
        foreach (GameObject obj in walls)
        {
            objects.Add(obj);
        }
        foreach (GameObject obj in enemies)
        {
            objects.Add(obj);
        }
        foreach (GameObject obj in traps)
        {
            objects.Add(obj);
        }
        foreach (GameObject obj in collecatbles)
        {
            objects.Add(obj);
        }
        foreach (GameObject obj in essentials)
        {
            objects.Add(obj);
        }

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(false);
            Destroy(objects[i]);            
        }
        mainCamera.transform.parent = null;
        //player destroyed and spawned again in new level (Causes problem when returning - requires debugging)
        Destroy(GameObject.FindGameObjectWithTag("Player")); 
    }

    private void SaveData(int levelIndex)
    {  
        levelData = new Data();
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");
        GameObject[] collecatbles = GameObject.FindGameObjectsWithTag("Collectable");
        GameObject[] essentials = GameObject.FindGameObjectsWithTag("Essentials");


        foreach (GameObject obj in floors)
        {
            levelData.AddToList(new Data(obj.tag, obj.name, obj.transform.position, obj.transform.rotation));
        }
        foreach (GameObject obj in walls)
        {
            levelData.AddToList(new Data(obj.tag, obj.name, obj.transform.position, obj.transform.rotation));
        }
        foreach (GameObject obj in walls)
        {
            levelData.AddToList(new Data(obj.tag, obj.name, obj.transform.position, obj.transform.rotation));
        }
        foreach (GameObject obj in enemies)
        {
            levelData.AddToList(new Data(obj.tag, obj.name, obj.transform.position, obj.transform.rotation));
        }
        foreach (GameObject obj in traps)
        {
            levelData.AddToList(new Data(obj.tag, obj.name, obj.transform.position, obj.transform.rotation));
        }
        foreach (GameObject obj in collecatbles)
        {
            levelData.AddToList(new Data(obj.tag, obj.name, obj.transform.position, obj.transform.rotation));
        }
        foreach (GameObject obj in essentials)
        {
            levelData.AddToList(new Data(obj.tag, obj.name, obj.transform.position, obj.transform.rotation));
        }        
        string level = JsonUtility.ToJson(levelData);        

        System.IO.File.WriteAllText("Assets/Resources/SaveData/Levels/Level" + levelIndex + ".json", level);
    }
    private void LoadData(int levelIndex)
    {
        
        if (levelIndex > 2)
        {
            SaveData(levelIndex - 1);
        }
        ClearLevel();

        string level = System.IO.File.ReadAllText("Assets/Resources/SaveData/Levels/Level" + levelIndex + ".json");        
        Data levelData = JsonUtility.FromJson<Data>(level);       

        //Loaded data set under their pools
        foreach (Data obj in levelData.GetList())
        {
            if (string.Compare(obj.GetTag(), "Floor") == 0)
            {
                if (obj.GetObjectType().Contains("CaveTile1"))
                {
                    GameObject instance = Instantiate(caveTile1, obj.GetPosition(),obj.GetRotation());
                    instance.transform.parent = floorInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("CaveTile2"))
                {
                    GameObject instance = Instantiate(caveTile2, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = floorInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("RoomTile1"))
                {
                    GameObject instance = Instantiate(tile1, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = floorInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("RoomTile2"))
                {
                    GameObject instance = Instantiate(tile2, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = floorInstancePool.transform;
                }
            }
            else if (string.Compare(obj.GetTag(), "Wall") == 0)
            {
                if (obj.GetObjectType().Contains("CaveWall1"))
                {
                    GameObject instance = Instantiate(caveWall1, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = wallInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("CaveWall2"))
                {
                    GameObject instance = Instantiate(caveWall2, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = wallInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("RoomWall1"))
                {
                    GameObject instance = Instantiate(wall1, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = wallInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("RoomWall2"))
                {
                    GameObject instance = Instantiate(wall2, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = wallInstancePool.transform;
                }
            }
            else if (string.Compare(obj.GetTag(), "Enemy") == 0)
            {
                if (obj.GetObjectType().Contains("Ghost"))
                {
                    GameObject instance = Instantiate(ghost, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = enemyInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("Golem"))
                {
                    GameObject instance = Instantiate(golem, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = enemyInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("SkeletonBerserk"))
                {
                    GameObject instance = Instantiate(skeletonBererk, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = enemyInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("SkeletonKnight"))
                {
                    GameObject instance = Instantiate(skeletonKnight, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = enemyInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("SkeletonMage"))
                {
                    GameObject instance = Instantiate(skeletonMage, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = enemyInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("SkeletonRouge"))
                {
                    GameObject instance = Instantiate(skeletonRouge, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = enemyInstancePool.transform;
                }
            }
            else if (string.Compare(obj.GetTag(), "Trap") == 0)
            {
                if (obj.GetObjectType().Contains("Trap_Cutter"))
                {
                    GameObject instance = Instantiate(trap3, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = trapInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("Trap_Fire"))
                {
                    GameObject instance = Instantiate(trap1, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = trapInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("Trap_Needle"))
                {
                    GameObject instance = Instantiate(trap2, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = trapInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("Door"))
                {
                    GameObject instance = Instantiate(door, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = trapInstancePool.transform;
                }
            }
            else if (string.Compare(obj.GetTag(), "Collectable") == 0)
            {
                if (obj.GetObjectType().Contains("CaveChestL"))
                {
                    GameObject instance = Instantiate(bigCaveChest, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = collectablesInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("CaveChestS"))
                {
                    GameObject instance = Instantiate(smallCaveChest, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = collectablesInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("RoomChestL"))
                {
                    GameObject instance = Instantiate(bigRoomChest, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = collectablesInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("RoomChestS"))
                {
                    GameObject instance = Instantiate(smallRoomChest, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = collectablesInstancePool.transform;
                }
                else if (obj.GetObjectType().Contains("Key"))
                {
                    GameObject instance = Instantiate(key, obj.GetPosition(), obj.GetRotation());
                    instance.transform.parent = collectablesInstancePool.transform;
                }
            }
            else if (string.Compare(obj.GetTag(), "Essentials") == 0)
            {
                if (obj.GetObjectType().Contains("PreviousLevel"))
                {
                    GameObject instance = Instantiate(previousLevel, obj.GetPosition(), obj.GetRotation());
                    startPos = obj.GetPosition();
                }
                else if (obj.GetObjectType().Contains("NextLevel"))
                {
                    GameObject instance = Instantiate(nextLevel, obj.GetPosition(), obj.GetRotation());
                    endPos = obj.GetPosition();
                }
            }
            if(player.transform == null)
            {
                Instantiate(player, endPos, Quaternion.identity);
            }            
        }
    }

    public void MissedArrow(GameObject target)
    {
        GameObject missedArrow = Instantiate(arrow, target.transform.position,Quaternion.identity);
        missedArrow.transform.parent = collectablesInstancePool.transform;
    }


    [System.Serializable]
    public class Data
    {
        public string tag;        
        public string objectType;
        public Vector3 position;
        public Quaternion rotation;

        [SerializeField] public List<Data> objects;

        public Data()
        {
            objects = new List<Data>();
        }
        public Data(string tag, string objectType, Vector3 position, Quaternion rotation)
        {

            this.tag = tag;
            this.objectType = objectType;
            this.position = position;
            this.rotation = rotation;
        }
        public string GetTag()
        {
            return tag;
        }
        public string GetObjectType()
        {
            return objectType;
        }
        public Vector3 GetPosition()
        {
            return position;
        }
        public Quaternion GetRotation()
        {
            return rotation;
        }
        public void AddToList(Data entry)
        {
            objects.Add(entry);
        }
        public List<Data> GetList()
        {
            return objects;
        }
    }
}
