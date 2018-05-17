using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Point GridPosition { get; private set; }

    private int width;
    private int height;

    private int levelWidth;
    private int levelHeight;
    void Start ()
    {

    }

    void Update ()
    {
		
	}

    public void Setup(int width, int height)
    {
        this.width = width;
        this.height = height;

        levelWidth = LevelManager.Instance.LevelWidth;
        levelHeight = LevelManager.Instance.LevelHeight;

        MoveRoom(width, height, levelWidth, levelHeight, this);
    }

    private void MoveRoom(int roomWidth, int roomHeight, int levelWidth, int levelHeight, Room room)
    {
        int x = 0;
        int y = 0;

        while (CheckRoomOverlap(roomWidth, roomHeight, room))
        {
            room.transform.position = new Vector3(Random.Range(0, x), Random.Range(0, y));
            x++;
            y++;

            if (y > levelHeight || x > levelWidth)
            {
                Destroy(room);
                break;
            }
        }


        GridPosition = new Point((int)transform.position.x, (int)transform.position.y);
        LevelManager.Instance.Rooms.Add(room.GridPosition, room);
    }

    private bool CheckRoomOverlap(int roomWidth, int roomHeight, Room room)
    {
        BoxCollider2D collider = room.GetComponent<BoxCollider2D>();

        SetupCollider(collider, roomWidth, roomHeight);

        foreach (Room item in LevelManager.Instance.Rooms.Values)
        {
            if (item != room)
            {

                BoxCollider2D colliderToCheck = item.GetComponent<BoxCollider2D>();
                Debug.Log(collider.bounds.Intersects(colliderToCheck.bounds));
                if (colliderToCheck != null && collider.bounds.Intersects(colliderToCheck.bounds) && colliderToCheck != collider)
                {
                    return true;
                }
            }

            for (int i = 0; i < room.transform.childCount; i++)
            {
                if (room.transform.GetChild(i).position.x > levelWidth || room.transform.GetChild(i).position.y > levelHeight)
                {
                    return true;
                }
            }
        }
        return false;
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
}
