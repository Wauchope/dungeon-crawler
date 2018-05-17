using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] tilePrefabs;

    [SerializeField]
    private GameObject roomObject;

    private List<GameObject> rooms = new List<GameObject>();

    [SerializeField]
    private Transform map;

    private bool levelGenerated = false;

    private GameObject newTile;

    //[SerializeField]
    private int numberOfRooms = 200;

    private int levelWidth = 128;
    private int levelHeight = 128;

    private bool haltLevelGen = false;

    void Start ()
    {
        if (!levelGenerated)
        {
            GenerateLevel(numberOfRooms, levelWidth, levelHeight);
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

            if (!haltLevelGen)
            {
                GenerateRoom(roomWidth, roomHeight, room);
                MoveRoom(roomWidth, roomHeight, levelWidth, levelHeight, rooms[room]);
            }
        }
    }

    private void GenerateRoom(int desiredWidth, int desiredHeight, int roomNumber)
    {
        GameObject room = Instantiate(roomObject, map);
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

        rooms.Add(room);
        Debug.Log("Generated room number: " + (roomNumber + 1) + ". Width: " + desiredWidth + ", Height: " + desiredHeight);
    }

    private bool CheckRoomOverlap(int roomWidth, int roomHeight, GameObject room)
    {
        BoxCollider2D collider = room.GetComponent<BoxCollider2D>();

        SetupCollider(collider, roomWidth, roomHeight);

        foreach (GameObject item in rooms)
        {
            if (item != room)
            {
                BoxCollider2D colliderToCheck = item.GetComponent<BoxCollider2D>();
                if (colliderToCheck != null && collider.bounds.Intersects(colliderToCheck.bounds) && colliderToCheck != collider)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void MoveRoom(int roomWidth, int roomHeight, int levelWidth, int levelHeight, GameObject room)
    {
        int x = 0;
        int y = 0;

        while (CheckRoomOverlap(roomWidth, roomHeight, room))
        {
            room.transform.position = new Vector3(Random.Range(-x, x), Random.Range(-y, y));
            x++;
            y++;

            if(y > levelHeight / 2 || x > levelWidth / 2)
            {
                Debug.Log("Warning: Too many rooms for the current level size");
                rooms.Remove(room);
                Destroy(room);
                haltLevelGen = true;
            }
        }
    }

    private void SetupCollider(BoxCollider2D collider, int roomWidth, int roomHeight)
    {
        collider.size = new Vector2(roomWidth, roomHeight);

        if (roomWidth % 2 == 0)
        {
            collider.offset = new Vector2((roomWidth / 2) - 0.5f, collider.offset.y);
        }
        else
        {
            collider.offset = new Vector2(roomWidth / 2, collider.offset.y);
        }

        if (roomHeight % 2 == 0)
        {
            collider.offset = new Vector2(collider.offset.x, (roomHeight / 2) - 0.5f);
        }
        else
        {
            collider.offset = new Vector2(collider.offset.x, roomHeight / 2);
        }
    }

    private void RemoveLevel(Transform map)
    {
        for (int i = 0; i < map.childCount - 1; i++)
        {
            Destroy(map.GetChild(0).gameObject);
        }

        rooms = new List<GameObject>();
        haltLevelGen = false;
    }

    void Update ()
    {
		
	}
}
