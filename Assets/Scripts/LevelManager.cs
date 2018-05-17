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
        /*//Creates the map borders
        for (int x = -1; x < levelWidth+1; x++)
        {
            for (int y = -1; y < levelHeight+1; y++)
            {
                if (x == -1 || x == levelWidth || y == -1 || y == levelHeight)
                {
                    Instantiate(tilePrefabs[0], new Vector3(x, y, 0), Quaternion.identity, map.Find("Border").transform).name = "Map_Border";
                }
            }
        }*/

        for (int room = 0; room < maxRooms; room++)
        {
            //Randomly decides on the size of the room (+2 to include walls)
            int roomWidth = Random.Range(5, 15) + 2;
            int roomHeight = Random.Range(5, 15) + 2;

            //GenerateRoom(roomWidth, roomHeight, room);
        }

        GenerateMaze(LevelWidth, LevelHeight);
    }

    private void GenerateRoom(int desiredWidth, int desiredHeight, int roomNumber)
    {
        Room room = Instantiate(roomObject, map).GetComponent<Room>();
        room.name = "Room_" + roomNumber;

        bool isDoorPlaced = false;

        //A list of walls which can be converted to doors
        List<GameObject> walls = new List<GameObject>();

        for (int currentY = 0; currentY < desiredHeight; currentY++)
        {
            for (int currentX = 0; currentX < desiredWidth; currentX++)
            {
                if (currentX == 0 || currentX == desiredWidth - 1 || currentY == 0 || currentY == desiredHeight - 1)
                {
                    //Ensures the corner tiles are solid wall
                    if (currentX == 0 && currentY == 0 || currentY == 0 && currentX == desiredWidth - 1 || currentY == desiredHeight - 1 && currentX == 0 || currentX == desiredWidth - 1 && currentY == desiredHeight - 1)
                    {
                        Instantiate(tilePrefabs[0], new Vector3(currentX, currentY, 0), Quaternion.identity, room.transform);
                    }
                    else
                    {
                        //Places the walls and adds it to the special list! (okay maybe not so special)
                        walls.Add(Instantiate(tilePrefabs[0], new Vector3(currentX, currentY, 0), Quaternion.identity, room.transform));
                    }
                }
                else
                {
                    //Place all other objects here (TO ADD RANDOMNESS AND MORE TILES)
                    Instantiate(tilePrefabs[1], new Vector3(currentX, currentY, 0), Quaternion.identity, room.transform);
                }
            }
        }

        while (!isDoorPlaced)
        {
            int tempX = 0;
            int tempY = 0;

            foreach (GameObject wall in walls)
            {
                tempX = (int) wall.transform.position.x;
                tempY = (int) wall.transform.position.y;

                if (Random.Range(0, 25) == 24)
                {
                    Instantiate(tilePrefabs[2], new Vector3(tempX, tempY, 0), Quaternion.identity, room.transform).name = "Door";
                    isDoorPlaced = true;
                    break;
                }
            }
        }

        room.Setup(desiredWidth, desiredHeight);
    }

    private void GenerateMaze(int width, int height)
    {

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
