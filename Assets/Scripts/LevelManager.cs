using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private GameObject[] tilePrefabs;

    [SerializeField]
    private GameObject roomObject;

    public Dictionary<Point, Room> Rooms = new Dictionary<Point, Room>();

    [SerializeField]
    private Transform map;

    private bool levelGenerated = false;

    private GameObject newTile;

    //[SerializeField]
    private int numberOfRooms = 100;

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
        NewLevel();
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
        for (int x = -1; x < levelWidth+1; x++)
        {
            for (int y = -1; y < levelHeight+1; y++)
            {
                if (x == -1 || x == levelWidth || y == -1 || y == levelHeight)
                {
                    Instantiate(tilePrefabs[0], new Vector3(x, y, 0), Quaternion.identity, map.transform).name = "Map_Border";
                }
            }
        }

        for (int room = 0; room < maxRooms; room++)
        {
            //Randomly decides on the size of the room (+2 to include walls)
            int roomWidth = Random.Range(5, 15) + 2;
            int roomHeight = Random.Range(5, 15) + 2;

            GenerateRoom(roomWidth, roomHeight, room);
        }
    }

    private void GenerateRoom(int desiredWidth, int desiredHeight, int roomNumber)
    {
        Room room = Instantiate(roomObject, map).GetComponent<Room>();
        room.name = "Room_" + roomNumber;

        for (int currentY = 0; currentY < desiredHeight; currentY++)
        {
            for (int currentX = 0; currentX < desiredWidth; currentX++)
            {
                if (currentX == 0 || currentX == desiredWidth - 1 || currentY == 0 || currentY == desiredHeight - 1)
                {
                    //Places the walls
                    Instantiate(tilePrefabs[0], new Vector3(currentX, currentY, 0), Quaternion.identity, room.transform);
                }
                else
                {
                    //Place all other objects here (TO ADD RANDOMNESS AND MORE TILES)
                    Instantiate(tilePrefabs[1], new Vector3(currentX, currentY, 0), Quaternion.identity, room.transform);
                }
            }
        }

        room.Setup(desiredWidth, desiredHeight);
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
