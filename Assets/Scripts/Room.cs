using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Point GridPosition { get; private set; }

    private int width;
    private int height;

    [SerializeField]
    private int padding;

    private int levelWidth;
    private int levelHeight;

    [SerializeField]
    private int maxAttempts;

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

        MoveRoom(width, height, levelWidth, levelHeight);
        CreateDoor(width, height);
    }

    private void CreateDoor(int width, int height)
    {
        //A list of walls which can be converted to doors
        List<Tile> walls = new List<Tile>();

        foreach (Transform child in transform)
        {
            Tile tile = child.GetComponent<Tile>();

            if (child.name == "Wall")
            {
                int currentX = tile.GridPosition.x - (int)transform.position.x;
                int currentY = tile.GridPosition.y - (int)transform.position.y;

                if (currentX == 0 && currentY == 0 ||
                    currentY == 0 && currentX == width - 1 ||
                    currentY == height - 1 && currentX == 0 ||
                    currentX == width - 1 && currentY == height - 1)
                {
                    continue;
                }
                else
                {
                    walls.Add(child.GetComponent<Tile>());
                }
            }
        }


        int rand = Random.Range(0, walls.Count);
        Tile wall = walls[rand];
        LevelManager.Instance.Tiles.Remove(wall.GridPosition);

        int tempX = wall.GridPosition.x;
        int tempY = wall.GridPosition.y;

        Destroy(wall.gameObject);

        wall = Instantiate(LevelManager.Instance.tilePrefabs[2], new Vector3(tempX, tempY, 0), Quaternion.identity, transform).GetComponent<Tile>();
        wall.name = "Door";
        wall.Setup(tempX, tempY);
        LevelManager.Instance.Tiles.Add(wall.GridPosition, wall);
    }

    private void MoveRoom(int roomWidth, int roomHeight, int levelWidth, int levelHeight)
    {
        int attemptNo = 0;

        while (CheckRoomOverlap(roomWidth, roomHeight, this))
        {
            transform.position = new Vector3(Random.Range(1, levelWidth), Random.Range(1, levelHeight));

            attemptNo++;

            if (attemptNo >= maxAttempts)
            {
                foreach (Transform child in transform)
                {
                    Tile tile = child.GetComponent<Tile>();
                    LevelManager.Instance.Tiles.Remove(tile.GridPosition);
                }
                Destroy(gameObject);
                break;
            }
        }


        //Unknown cause for ArgumentException 
        try
        {
            GridPosition = new Point((int)transform.position.x + (int)System.Math.Round((double)(roomWidth / 2), 0), (int)transform.position.y + (int)System.Math.Round((double)(roomHeight / 2), 0));
            LevelManager.Instance.Rooms.Add(GridPosition, this);

            foreach (Transform child in transform)
            {
                Tile tile = child.GetComponent<Tile>();
                tile.Setup((int) child.transform.position.x, (int) child.transform.position.y);
                LevelManager.Instance.Tiles.Add(tile.GridPosition, tile);
            }
        }
        catch (System.ArgumentException e)
        {
            foreach (Transform child in transform)
            {
                Tile tile = child.GetComponent<Tile>();
                LevelManager.Instance.Tiles.Remove(tile.GridPosition);
            }
            Destroy(gameObject);
        }
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
        collider.size = new Vector2(roomWidth + padding, roomHeight + padding);

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
            collider.offset = new Vector2(collider.offset.x, (roomHeight/ 2) - 0.5f);
        }
        else
        {
            collider.offset = new Vector2(collider.offset.x, roomHeight / 2);
        }
    }
}
