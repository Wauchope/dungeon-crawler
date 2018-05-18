using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    public GameObject[] tilePrefabs;

    [SerializeField]
    private GameObject roomObject;

    private MazeGenerator mazeGen;

    public Dictionary<Point, Room> Rooms = new Dictionary<Point, Room>();

    public Dictionary<Point, Tile> Tiles = new Dictionary<Point, Tile>();

    public Dictionary<int, string> tileTypes = new Dictionary<int, string>();

    [SerializeField]
    private Transform map;

    private bool levelGenerated = false;

    [SerializeField]
    private int numberOfRooms;

    //Must be a multiple of 2
    [SerializeField]
    private int levelHeight;

    [SerializeField]
    private int levelWidth;

    public int LevelHeight
    {
        get
        {
            return levelHeight;
        }
    }

    public int LevelWidth
    {
        get
        {
            return levelWidth;
        }

    }

    void Start ()
    {
        SetupTileTypes();
        mazeGen = GetComponent<MazeGenerator>();
        NewLevel();
    }

    private void SetupTileTypes()
    {
        tileTypes.Add(0, "Wall");
        tileTypes.Add(1, "Floor");
        tileTypes.Add(2, "Door");
    }

    private void NewLevel()
    {
        if (!levelGenerated)
        {
            GenerateLevel(numberOfRooms, LevelWidth, LevelHeight);
            levelGenerated = true;
        }
    }

    private void GenerateLevel(int maxRooms, int levelWidth, int levelHeight)
    {
        for (int room = 0; room < maxRooms; room++)
        {
            //Randomly decides on the size of the room (+2 to include walls)
            int roomWidth = Random.Range(5, 15) + 2;
            int roomHeight = Random.Range(5, 15) + 2;

            GenerateRoom(roomWidth, roomHeight, room);
        }

        GenerateMaze(LevelWidth + 2, LevelHeight + 2);
    }

    private void GenerateRoom(int desiredWidth, int desiredHeight, int roomNumber)
    {
        Room room = Instantiate(roomObject, map).GetComponent<Room>();
        room.name = "Room_" + roomNumber;        

        int tileType = 0;

        for (int currentY = 1; currentY < desiredHeight+1; currentY++)
        {
            for (int currentX = 1; currentX < desiredWidth+1; currentX++)
            {

                if (currentX == 1 || currentX == desiredWidth || currentY == 1 || currentY == desiredHeight)
                {
                    //Place walls
                    tileType = 0;
                }
                else
                {
                    //Place all other objects here (TO ADD RANDOMNESS AND MORE TILES)
                    tileType = 1;
                }

                GameObject newTile = Instantiate(tilePrefabs[tileType], new Vector3(currentX, currentY, 0), Quaternion.identity, room.transform);
                newTile.name = tileTypes[tileType];
            }
        }

        room.Setup(desiredWidth, desiredHeight);
    }

    private void GenerateMaze(int width, int height)
    {
        mazeGen.GenerateNewMaze(width, height);
    }

    private void RemoveLevel(Transform map)
    {
        for (int i = 0; i < map.childCount - 1; i++)
        {
            Destroy(map.GetChild(0).gameObject);
        }

        Rooms = new Dictionary<Point, Room>();
    }

    void Update ()
    {
		
	}
}
