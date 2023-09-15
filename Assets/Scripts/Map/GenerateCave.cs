using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class GenerateCave : MonoBehaviour
{

    //Using Cellular Automata

    //Game objects to instantiate
    public GameObject CaveTile1;
    public GameObject CaveTile2;
    public GameObject CaveWall1;
    public GameObject CaveWall2;

    public GameObject nextLevel;
    public GameObject previousLevel;

    public GameObject ghost;
    public GameObject golem;
    public GameObject skeletonBererk;
    public GameObject skeletonKnight;
    public GameObject skeletonMage;
    public GameObject skeletonRouge;

    public GameObject bigChest;
    public GameObject smallChest;

    public GameObject trap1;
    public GameObject trap2;

    public GameObject key;
    public GameObject door;

    public GameObject pathV;
    public GameObject pathH;

    //Instance pools

    public GameObject floorInstancePool;
    public GameObject wallInstancePool;
    public GameObject enemyInstancePool;
    public GameObject trapInstancePool;
    public GameObject collectablesInstancePool;

    //Scene Manager objects

    public GameObject playerManager;
    private GameObject progressManager;

    public NavMeshSurface nav;

    //Variables

    public int doorBudget;
    public int enemyBudget;
    public int trapBudget;
    public int dropBudget;

    public int width;
    public int height;

    public int minRoom;
    public int minWall;

    public string seed;

    public int smoothIterations;
    [Range(0, 9)]
    public int smoothLevel;
    [Range(0, 7)]
    public int smoothGap;
    [Range(0, 100)]
    public int randomFillPercent;

    private int[,] map;
    private int[,] objects;
    private List<List<Coord>> roomRegions;
    private List<List<Coord>> wallRegions;
    private List<Coord> edges;

    private List<Coord> doorLocs;

    private Coord startPos;
    private Coord stopPos;

    private bool started;
    private bool finished;

    private bool fullyConnected;

    private int currentLevel;

    void Start()
    {
        started = false;
        finished = false;
        
        progressManager = GameObject.FindGameObjectWithTag("ProgressManager");

        GameObject dead = GameObject.FindGameObjectWithTag("DeathScreen");
        if (dead != null)
        {
            dead.SetActive(false);
        }

        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        doorBudget = 2 * (int)(currentLevel / 5);
        enemyBudget = 15 * (int)((currentLevel / 5) + 1);
        trapBudget = 5 * (int)(currentLevel / 5);
        dropBudget = 20 * (int)((currentLevel / 5) + 1);
    }

    public void GenerateMap() //Build the new map
    {
        objects = new int[width, height];
        map = new int[width, height]; //define map
        RandomfillMap();             //initial random map

        for (int i = 0; i < smoothIterations; i++)    //Smoothen map (make into caves)
        {
            SmoothMap();
        }

        roomRegions = GetRegions(0);
        wallRegions = GetRegions(1);

        RemoveTrivialRegions();     //Remove remaining outlier bits

        List<CaveRoom> rooms = RegionsToRooms(roomRegions);
        EndPoints(rooms);
        ChestPositions(rooms);

        //Ensure fully connected
        fullyConnected = false;
        while (!fullyConnected)
        {
            roomRegions = GetRegions(0);
            if(roomRegions.Count == 1)
            {
                fullyConnected = true;
            }
            else
            {
                rooms = RegionsToRooms(roomRegions);
                ConnectClosestRooms(rooms, false);                
            }
        }
        
        BuildMap();                 //Instantiate Map
        PopulateMap();
        KeyLocations(startPos.tileX, startPos.tileY);

        nav.BuildNavMesh();
    }

    void PopulateMap()
    {        
        doorLocs = new List<Coord>();
        doorBudget = 2;
        enemyBudget = 15;
        trapBudget = 5;
        float random;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(map[x,y] == 0)
                {
                    if (CheckNarrow(x, y))
                    {                        
                        if (doorBudget > 0 && objects[x, y] != 1)
                        {
                            random = UnityEngine.Random.Range(0f, 10f);
                            if (random <= 4) //40% to generate door at narrow path
                            {
                                doorBudget--;
                                if (map[x - 1, y] == 1 && map[x + 1, y] == 1)
                                {
                                    GameObject doorInstance = Instantiate(door, new Vector3(x - 1, 1.3f, y), Quaternion.Euler(90, 0, 0));
                                    doorInstance.transform.parent = trapInstancePool.transform;
                                    objects[x, y] = 1;
                                }
                                else
                                {
                                    GameObject doorInstance = Instantiate(door, new Vector3(x - 1, 1.3f, y), Quaternion.Euler(90, 0, 90));
                                    doorInstance.transform.parent = trapInstancePool.transform;
                                    objects[x, y] = 1;
                                }
                                doorLocs.Add(new Coord(x, y));
                            }
                        }
                        if (trapBudget > 0 && objects[x,y] != 1)
                        {                        
                            random = UnityEngine.Random.Range(0f, 10f);
                            if (random <= 4) //40% to generate trap at narrow path
                            {
                                random = UnityEngine.Random.Range(0f, 10f);
                                trapBudget--;
                                if (random <= 0.5)
                                {
                                    GameObject trapInstance = Instantiate(trap1, new Vector3(x - 0.5f, 0.1f, y+0.5f), Quaternion.Euler(0, -90, 0));
                                    trapInstance.transform.parent = trapInstancePool.transform;
                                    objects[x, y] = 1;
                                }
                                else
                                {
                                    GameObject trapInstance = Instantiate(trap2, new Vector3(x - 0.5f, 0.1f, y+0.5f), Quaternion.Euler(0, -90, 0));
                                    trapInstance.transform.parent = trapInstancePool.transform;
                                    objects[x, y] = 1;
                                }
                            }
                        }
                    }
                    random = UnityEngine.Random.Range(0f, 10f);
                    if (random < (0.3f * (int)((currentLevel / 5) + 1))) //3% to generate enemy (depending on current level)
                    {
                        if (enemyBudget > 0 && !edges.Contains(new Coord(x, y)) && objects[x, y] != 1)
                        {
                            if (UnityEngine.Random.Range(0f, 100f) < currentLevel) //at level 50, 50% to spawn a strong enemy
                            {
                                if (UnityEngine.Random.Range(0f, 10f) < 0.25f) //75% ghost
                                {
                                    GameObject enemy = Instantiate(ghost, new Vector3(x - 0.5f, 0.1f, y + 0.5f), Quaternion.Euler(0, 0, 0));
                                    enemy.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                                    enemy.transform.parent = enemyInstancePool.transform;
                                    objects[x, y] = 1;
                                }
                                else //25% golem
                                {
                                    GameObject enemy = Instantiate(golem, new Vector3(x - 0.5f, 0.1f, y + 0.5f), Quaternion.Euler(0, 0, 0));
                                    enemy.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                                    enemy.transform.parent = enemyInstancePool.transform;
                                    objects[x, y] = 1;
                                }
                            }
                            else
                            {
                                random = UnityEngine.Random.Range(0f, 100f);
                                if (random <= 35f)//35% Berserk
                                {
                                    GameObject enemy = Instantiate(skeletonBererk, new Vector3(x - 0.5f, 0.1f, y + 0.5f), Quaternion.Euler(0, 0, 0));
                                    enemy.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                                    enemy.transform.parent = enemyInstancePool.transform;
                                    objects[x, y] = 1;
                                }
                                else if (random > 35f && random <= 65f)//30% Knight
                                {
                                    GameObject enemy = Instantiate(skeletonKnight, new Vector3(x - 0.5f, 0.1f, y + 0.5f), Quaternion.Euler(0, 0, 0));
                                    enemy.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                                    enemy.transform.parent = enemyInstancePool.transform;
                                    objects[x, y] = 1;
                                }
                                else if (random > 65f && random <= 85)//20% Mage
                                {
                                    GameObject enemy = Instantiate(skeletonMage, new Vector3(x - 0.5f, 0.1f, y + 0.5f), Quaternion.Euler(0, 0, 0));
                                    enemy.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                                    enemy.transform.parent = enemyInstancePool.transform;
                                    objects[x, y] = 1;
                                }
                                else//15% Rouge
                                {
                                    GameObject enemy = Instantiate(skeletonRouge, new Vector3(x - 0.5f, 0.1f, y + 0.5f), Quaternion.Euler(0, 0, 0));
                                    enemy.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                                    enemy.transform.parent = enemyInstancePool.transform;
                                    objects[x, y] = 1;
                                }
                                enemyBudget--;
                            }
                        }                                          
                    }

                    if (dropBudget > 0 && !edges.Contains(new Coord(x, y)) && objects[x, y] != 1)
                    {
                        random = UnityEngine.Random.Range(0f, 10f);
                        if (random <= 0.002f / (int)((currentLevel / 5) + 1)) //0.2% to generate Item
                        {
                            random = UnityEngine.Random.Range(0f, 10f);
                            if (random < 0.5f) //5% Weapon
                            {
                                GameObject item = progressManager.GetComponentInChildren<Items>().SpawnItemWithEffects("Weapon", x - 0.5f, 0.1f, y + 0.5f);
                                item.transform.parent = collectablesInstancePool.transform;
                                objects[x, y] = 1;
                            }
                            else if (random < 2.1f && random >= 0.5f) //16% Health Potion
                            {
                                GameObject item = progressManager.GetComponentInChildren<Items>().SpawnItem("Health", x - 0.5f, 0.1f, y + 0.5f);
                                item.transform.parent = collectablesInstancePool.transform;
                                objects[x, y] = 1;
                            }
                            else if (random < 3.2f && random >= 2.1)//11% Mana Potion
                            {
                                GameObject item = progressManager.GetComponentInChildren<Items>().SpawnItem("Mana", x - 0.5f, 0.1f, y + 0.5f);
                                item.transform.parent = collectablesInstancePool.transform;
                                objects[x, y] = 1;
                            }
                            else if (random < 4.3 && random >= 3.2f)//11% Stamina Potion
                            {
                                GameObject item = progressManager.GetComponentInChildren<Items>().SpawnItem("Stamina", x - 0.5f, 0.1f, y + 0.5f);
                                item.transform.parent = collectablesInstancePool.transform;
                                objects[x, y] = 1;
                            }
                            else if (random < 8.5f && random >= 7)//2% Mysterious Potion
                            {
                                GameObject item = progressManager.GetComponentInChildren<Items>().SpawnItem("Myst", x - 0.5f, 0.1f, y + 0.5f);
                                item.transform.parent = collectablesInstancePool.transform;
                                objects[x, y] = 1;
                            }
                            else//55% Arrow
                            {
                                GameObject item = progressManager.GetComponentInChildren<Items>().SpawnItem("Arrow", x - 0.5f, 0.1f, y + 0.5f);
                                item.transform.parent = collectablesInstancePool.transform;
                                objects[x, y] = 1;
                            }
                            dropBudget--;
                        }                        
                    }
                }               
            }
        }
    }

    void RandomfillMap()
    {

        System.Random rand = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) //Is a wall (edge)
                {
                    map[x, y] = 1;
                }
                else//May or may not be a wall
                {
                    map[x, y] = (rand.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void BuildMap()
    {
        edges = new List<Coord>();
        string seed = Time.time.ToString();
        System.Random rand = new System.Random(seed.GetHashCode());
        for (int x = 1; x < width; x++)
        {
            for (int y = 1; y < height; y++)
            {
                if (map[x, y] == 0)
                {
                    if (x == startPos.tileX && y == startPos.tileY && !started)
                    {                        
                        if(currentLevel > 1)
                        {
                            Instantiate(previousLevel, new Vector3(x + 0.5f, 0, y + 0.5f), Quaternion.Euler(0, -90, 0));
                        }
                        else
                        {
                            currentLevel = PlayerPrefs.GetInt("CurrentLevel");
                        }                   
                        started = false;
                        GameObject caveTile  = Instantiate(CaveTile1, new Vector3(x, 0, y), Quaternion.identity);
                        caveTile.transform.parent = floorInstancePool.transform;

                        GameObject playerPrefab = playerManager.GetComponent<PlayerManager>().GetPlayerPrefab();
                        if (nextLevel)
                        {
                            GameObject player = Instantiate(playerPrefab, new Vector3(x + 0.5f, 0.3f, y + 0.5f), Quaternion.Euler(0, -180, 0));
                            player.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        }
                    }
                    else if (x == stopPos.tileX && y == stopPos.tileY && !finished && currentLevel < 50)
                    {
                        Instantiate(nextLevel, new Vector3(x, 0, y), Quaternion.Euler(0, 0, 0));
                        finished = false;
                    }
                    else
                    {
                        GameObject caveTile;
                        if (rand.Next(0, 1) == 1)
                        {
                            caveTile = Instantiate(CaveTile1, new Vector3(x, 0, y), Quaternion.identity);
                            caveTile.transform.parent = floorInstancePool.transform;
                        }
                        else
                        {
                            caveTile = Instantiate(CaveTile2, new Vector3(x, 0, y), Quaternion.identity);
                            caveTile.transform.parent = floorInstancePool.transform;
                        }
                        GameObject pathHorizontal = Instantiate(pathH, new Vector3(x, 0.01f, y), Quaternion.identity);
                        GameObject pathVertical = Instantiate(pathV, new Vector3(x, 0.01f, y), Quaternion.identity);
                        pathHorizontal.transform.parent = caveTile.transform;
                        pathVertical.transform.parent = caveTile.transform;
                    }
                }
                if (map[x - 1, y] != map[x, y])
                {                    
                    if (rand.Next(0, 1) == 1)
                    {
                        GameObject caveWall = Instantiate(CaveWall1, new Vector3(x - 1, 0, y), Quaternion.Euler(0, -90, 0));
                        caveWall.transform.parent = wallInstancePool.transform;
                    }
                    else
                    {
                        GameObject caveWall = Instantiate(CaveWall2, new Vector3(x - 1, 0, y), Quaternion.Euler(0, -90, 0));
                        caveWall.transform.parent = wallInstancePool.transform;
                    }
                    edges.Add(new Coord(x, y));                    
                }
                if (map[x, y - 1] != map[x, y])
                {
                    if (rand.Next(0, 1) < 1)
                    {
                        GameObject caveWall = Instantiate(CaveWall1, new Vector3(x, 0, y), Quaternion.identity);
                        caveWall.transform.parent = wallInstancePool.transform;
                    }
                    else
                    {
                        GameObject caveWall = Instantiate(CaveWall2, new Vector3(x, 0, y), Quaternion.identity);
                        caveWall.transform.parent = wallInstancePool.transform;
                    }
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int wallCount = GetWallCount(x, y);

                if (wallCount > smoothLevel)
                {
                    map[x, y] = 1;
                }
                else if (wallCount <= smoothLevel - smoothGap)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    int GetWallCount(int xpos, int ypos) //check how many sorrounding tiles are walls
    {
        int walls = 0;
        for (int x = xpos - 1; x <= (xpos + 1); x++)
        {
            for (int y = ypos - 1; y <= (ypos + 1); y++)
            {
                if ((x >= 0 && x < width) && (y >= 0 && y < height))
                {
                    if (x != xpos || y != ypos) //Not the actual centre tile
                    {
                        walls += map[x, y];
                    }
                }
                else
                {
                    walls++;
                }
            }
        }
        return walls;
    }

    bool CheckBlock(int xPos, int yPos) //Check if sorrounding tiles are narrow
    {
        if (xPos > 0 && xPos < width &&
            yPos > 0 && yPos < height)
        {
            if (CheckNarrow(xPos+1,yPos) || CheckNarrow(xPos-1, yPos) ||
                CheckNarrow(xPos, yPos+1) || CheckNarrow(xPos, yPos-1))
            {
                return true;
            }           
        }
        return false;
    }
    bool CheckNarrow(int xPos,int yPos)
    {
        if(xPos > 0 && xPos < width &&
            yPos > 0 && yPos < height)
        {
            if (map[xPos, yPos] == 0)
            {
                if (map[xPos - 1, yPos] == 1 && map[xPos + 1, yPos] == 1)
                {
                    return true;
                }
                else if (map[xPos, yPos-1] == 1 && map[xPos, yPos+1] == 1)
                {
                    return true;
                }                
            }            
        }
        return false;
    }
    
    void KeyLocations(int startX, int startY)
    {
        List<List<Coord>> placeableTiles = new List<List<Coord>>();        

        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height && (y == tile.tileY || x == tile.tileX))
                    {                        
                        if (mapFlags[x, y] == 0 && map[x, y] == map[startX, startY])
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                        if (!tiles.Contains(new Coord(x, y))){
                            foreach (Coord d in doorLocs)
                            {
                                if (d.tileX == x && d.tileY == y)
                                {                                    
                                    List<Coord> regionCopy = new List<Coord>(tiles); //All modes reachable until reaching next gate                                    
                                    placeableTiles.Add(regionCopy);                 
                                }
                            }                            
                        }                        
                    }
                }
            }
        }
        int random;
        foreach (List<Coord> spaces in placeableTiles)
        {
            random = UnityEngine.Random.Range(0, spaces.Count);
            GameObject keyInstance = Instantiate(key, new Vector3(spaces[random].tileX-0.5f, 0.1f, spaces[random].tileY+0.5f), Quaternion.Euler(-90, 0, 0));
            objects[spaces[random].tileX, spaces[random].tileY] = 1;
            keyInstance.transform.parent = collectablesInstancePool.transform;
        }
    }
    

    //Get regions (flood method)
    List<Coord> GetRegionTiles(int startX, int startY) //Flood the region the same as starting tile
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height && (y == tile.tileY || x == tile.tileX))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == map[startX, startY])
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    List<List<Coord>> GetRegions(int tileType) //Exclude flooded tiles and find non flooded
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    void RemoveTrivialRegions()
    {
        List<Coord> region = new List<Coord>();
        for (int regionIndex = 0; regionIndex < roomRegions.Count; regionIndex++) //Remove Rooms that are too small
        {
            region = roomRegions[regionIndex];
            if (region.Count < minRoom)
            {
                foreach (Coord tile in region)
                {
                    map[tile.tileX, tile.tileY] = 1;
                }
                roomRegions.Remove(region); //Remove no longer existing region
            }
        }
        for (int regionIndex = 0; regionIndex < wallRegions.Count; regionIndex++) //Remove walls that are too small
        {
            region = wallRegions[regionIndex];
            if (region.Count < minWall)
            {
                foreach (Coord tile in region)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }
        //Reset surviving rooms
        roomRegions = GetRegions(0);
        wallRegions = GetRegions(1);
    }

    List<CaveRoom> RegionsToRooms(List<List<Coord>> regions)
    {
        List<CaveRoom> rooms = new List<CaveRoom>();
        foreach (List<Coord> region in regions)
        {
            rooms.Add(new CaveRoom(region, map,width,height));       //Make a Room Equivalent of the regions
        }

        rooms.Sort();
        rooms[0].isMain = true;
        rooms[0].startAccessable = true;

        return rooms;
    }

    //Define start and end room
    void EndPoints(List<CaveRoom> rooms)
    {
        CaveRoom exit = new CaveRoom();        

        float longest;

        foreach (CaveRoom room in rooms)
        {            
            longest = 0;
            
            if (room == rooms[0]) //same Room
            {
                continue;
            }

            //Compare all edge tiles of all rooms to find the furthest connections
            foreach (Coord tileA in room.edgeTiles)
            {
                foreach (Coord tileB in rooms[0].edgeTiles)
                {
                    float distance = Mathf.Pow(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2), 0.5f);
                    if (distance > longest)
                    {
                        longest = distance;
                        exit = room;
                    }
                }
            }                       
        }

        startPos = GetRandomTile(rooms[0]);
        stopPos = GetRandomTile(exit);
    }
    void ChestPositions(List<CaveRoom> rooms)
    {
        foreach(CaveRoom room in rooms)
        {
            float random = UnityEngine.Random.Range(0f, 10f);
            Coord chestTile = GetRandomTile(room);

            if(random >= 8) //20% chance large chest
            {
                GameObject chestIntance = Instantiate(bigChest, new Vector3(chestTile.tileX - 0.5f, 0.1f, chestTile.tileY + 0.5f), Quaternion.Euler(0, 0, 0));
                objects[chestTile.tileX, chestTile.tileY] = 1;
                chestIntance.transform.parent = collectablesInstancePool.transform;
            }
            else if (random >= 4 && random < 8) //40% chance small chest
            {
                GameObject chestIntance = Instantiate(smallChest, new Vector3(chestTile.tileX - 0.5f, 0.1f, chestTile.tileY + 0.5f), Quaternion.Euler(0, 0, 0));
                objects[chestTile.tileX, chestTile.tileY] = 1;
                chestIntance.transform.parent = collectablesInstancePool.transform;
            }
        }
    }
    Coord GetRandomTile(CaveRoom room)
    {
        CaveRoom checkRoom = room;
        int randomTile;
        int counter = 0;

        while (true)
        {
            randomTile = UnityEngine.Random.Range(0, checkRoom.tiles.Count);
            Coord potentialStart = checkRoom.tiles[randomTile];

            if (!CheckBlock(potentialStart.tileX, potentialStart.tileY) &&  //not blocking passage
                !room.edgeTiles.Contains(potentialStart)&&                  //not too close to edge
                objects[potentialStart.tileX, potentialStart.tileY] != 1)   //not already taken
            {
                //stopPos = checkRoom.tiles[randomTile];
                objects[potentialStart.tileX, potentialStart.tileY] = 1;
                return potentialStart;
            }
            else
            {                    
                counter++;
                if(counter >= room.tiles.Count) //No more tiles to check
                {
                    //stopPos = checkRoom.tiles[randomTile];
                    objects[potentialStart.tileX, potentialStart.tileY] = 1;
                    return potentialStart;
                }
                checkRoom.tiles.Remove(potentialStart); //Remove from search pool
            }
        }
    }

    
    void ConnectClosestRooms(List<CaveRoom> rooms, bool startAccess)
    {
        
        List<CaveRoom> roomsA = new List<CaveRoom>();
        List<CaveRoom> roomsB = new List<CaveRoom>();

        if (startAccess)
        {
            foreach (CaveRoom room in rooms)
            {
                if (room.startAccessable) //Accessable rooms
                {
                    roomsB.Add(room);
                }
                else    //Non Accessable rooms
                {
                    roomsA.Add(room);                    
                }
            }
        }
        else
        {
            roomsA = rooms;
            roomsB = rooms;
        }
                
        CaveRoom bestRoomA = new CaveRoom();
        CaveRoom bestRoomB = new CaveRoom();
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        float shortest = float.MaxValue;
        bool hasConnection = false;

        foreach (CaveRoom roomA in roomsA) //For all Rooms A
        {           
            if (!startAccess)
            {
                hasConnection = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }
            shortest = float.MaxValue;
            foreach (CaveRoom roomB in roomsB) //Find closest RoomB to RoomA
            {
                if (roomA == roomB || roomA.IsConnected(roomB)) //Is same room or already connected
                {
                    continue;
                }

                //Compare all edge tiles of all rooms to find the closest connections
                foreach (Coord tileA in roomA.edgeTiles)
                {
                    foreach (Coord tileB in roomB.edgeTiles)
                    {
                        float distance = Mathf.Pow(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2), 0.5f);
                        if (distance < shortest || !hasConnection)
                        {
                            hasConnection = true;
                            shortest = distance;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                            bestTileA = tileA;
                            bestTileB = tileB;
                        }                        
                    }
                }                
            }
            if (hasConnection && !startAccess)
            {
                Createpassage(roomA, bestRoomB, bestTileA, bestTileB);
            }
        }
        if (hasConnection && startAccess)
        {
            Createpassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(rooms, true);
        }
        if (!startAccess)
        {
            ConnectClosestRooms(rooms, true);
        }
    }

    void Createpassage(CaveRoom roomA, CaveRoom roomB, Coord tileA, Coord tileB)
    {
        CaveRoom.ConnectRooms(roomA, roomB);

        List<Coord> line = GetLine(tileA, tileB);
        foreach(Coord tile in line)
        {
            FillPassage(tile, 2);
        }
    }

    void FillPassage(Coord tile, int radius)
    {
        for(int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if((x*x) + (y*y) <= (radius * radius))
                {
                    int xTile = tile.tileX + x;
                    int yTile = tile.tileY + y;
                    if (xTile < width && yTile < height
                        && xTile > 0 && yTile > 0)
                    {
                        map[xTile, yTile] = 0;
                    }                    
                }
            }
        }
    }

    List<Coord> GetLine(Coord pos1, Coord pos2){
        List<Coord> line = new List<Coord>();
        int x = pos1.tileX;
        int y = pos1.tileY;

        int dx = pos2.tileX - pos1.tileX;
        int dy = pos2.tileY - pos1.tileY;

        bool inverted = false;
        float step = Math.Sign(dx);
        float gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if(longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign (dy);
            gradientStep = Math.Sign(dx);            
        }

        int gradientAccumulation = longest / 2;
        for(int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += (int)step;
            }
            else
            {
                x += (int)step;
            }
            gradientAccumulation += shortest;
            if( gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += (int)gradientStep;
                }
                else
                {
                    y += (int)gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }
        return line;
        }
    
    struct Coord
    {
        public int tileX;
        public int tileY;
        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }
    class CaveRoom :IComparable<CaveRoom>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<CaveRoom> connectedRooms;
        public bool startAccessable;
        public bool isMain;
        public int roomSize;

        public CaveRoom()
        {
            isMain = false;
        }

        public CaveRoom(List<Coord> roomTiles, int[,] map, int maxWidth,int maxHeight)
        {
            tiles = roomTiles;
            isMain = false;
            roomSize = tiles.Count;
            connectedRooms = new List<CaveRoom>();

            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX-1; x <= tile.tileX+1; x++)
                {
                    for (int y = tile.tileY-1; y <= tile.tileY+1; y++)
                    {
                        
                        if (x > 0 && y > 0 && x < maxWidth && y < maxHeight) { 
                            if (x == tile.tileX || y == tile.tileY)
                            {
                                if (map[x, y] == 1)
                                {
                                    edgeTiles.Add(tile);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AccessableFromStart()
        {
            if (!startAccessable)
            {
                startAccessable = true;
                foreach(CaveRoom room in connectedRooms)
                {
                    room.AccessableFromStart();
                }
            }
        }

        public static void ConnectRooms(CaveRoom roomA, CaveRoom roomB)
        {
            if (roomA.startAccessable)
            {
                roomB.AccessableFromStart();
            }
            else
            {
                roomA.AccessableFromStart();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(CaveRoom otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }
        public int CompareTo(CaveRoom otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }
}

