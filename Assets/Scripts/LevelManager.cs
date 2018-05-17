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

    void Start ()
    {
		if (!levelGenerated)
        {
            GenerateLevel(4);
            levelGenerated = true;
        }
	}

    private void GenerateLevel(int maxRooms)
    {
        for (int room = 0; room < maxRooms; room++)
        {
            //Randomly decides on the size of the room (+2 to include walls)
            int roomWidth = Random.Range(5, 15) + 2;
            int roomHeight = Random.Range(5, 15) + 2;

            GenerateRoom(roomWidth, roomHeight, room);
            CheckRoomOverlap(roomWidth, roomHeight, room, rooms[room]);
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
                    Instantiate(tilePrefabs[0], new Vector3(currentX, currentY, 0), Quaternion.identity, room.transform);
                }
                else
                {
                    Instantiate(tilePrefabs[1], new Vector3(currentX, currentY, 0), Quaternion.identity, room.transform);
                }
            }
        }

        rooms.Add(room);
        Debug.Log("Generated room number: " + (roomNumber + 1) + ". Width: " + desiredWidth + ", Height: " + desiredHeight);
    }

    private void CheckRoomOverlap(int roomWidth, int roomHeight, int roomNumber, GameObject room)
    {
        BoxCollider2D collider = room.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(roomWidth, roomHeight);

        if (roomWidth % 2 == 1)
        {
            collider.offset = new Vector2((roomWidth + 1) / 2, collider.offset.y);
        }
        else
        {
            collider.offset = new Vector2(roomWidth / 2, collider.offset.y);
        }

        if (roomHeight % 2 == 1)
        {
            collider.offset = new Vector2(collider.offset.x, (roomHeight + 1) / 2);
        }
        else
        {
            collider.offset = new Vector2(collider.offset.x, roomHeight / 2);
        }

        foreach (GameObject item in rooms)
        {
            BoxCollider2D colliderToCheck = item.GetComponent<BoxCollider2D>();
            Debug.Log(collider.IsTouching(colliderToCheck));
            if (colliderToCheck != null && collider.IsTouching(colliderToCheck) && colliderToCheck != collider)
            {
                Debug.Log("Room_" + roomNumber + " must be moved");
            }
        }
    }

    void Update ()
    {
		
	}
}
