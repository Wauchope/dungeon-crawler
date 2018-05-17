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

        bool flagForDoor = false;
        bool isDoorPlaced = false;

        int xStart = (int) transform.position.x;
        int yStart = (int)transform.position.y;


        for (int currentY = yStart; currentY < yStart + height; currentY++)
        {
            for (int currentX = xStart; currentX < width; currentX++)
            {
                flagForDoor = false;

                if (currentX == 0 || currentX == width - 1 || currentY == 0 || currentY == height - 1)
                {
                    if (!(currentX == 0 && currentY == 0 ||
                        currentY == 0 && currentX == width - 1 ||
                        currentY == height - 1 && currentX == 0 ||
                        currentX == width - 1 && currentY == height - 1))
                    {
                        flagForDoor = true;
                    }
                }

                //Adds non-corner wall tiles to a list
                if (flagForDoor == true)
                {
                    walls.Add(LevelManager.Instance.Tiles[new Point(currentX, currentY)]);
                }
            }
        }

        while (!isDoorPlaced)
        {
            int tempX = 0;
            int tempY = 0;

            foreach (Tile wall in walls)
            {
                tempX = wall.GridPosition.x;
                tempY = wall.GridPosition.y;
                if (Random.Range(0, 25) == 24)
                {
                    //Destroy the old tile
                    Tile tile = LevelManager.Instance.Tiles[wall.GridPosition];
                    Destroy(tile);
                    LevelManager.Instance.Tiles.Remove(wall.GridPosition);

                    //Create the door
                    tile = Instantiate(LevelManager.Instance.tilePrefabs[2], new Vector3(tempX, tempY, 0), Quaternion.identity, transform).GetComponent<Tile>();
                    tile.name = "Door";
                    tile.Setup(tempX, tempY);
                    isDoorPlaced = true;
                    break;
                }
            }
        }

    }

    private void MoveRoom(int roomWidth, int roomHeight, int levelWidth, int levelHeight)
    {
        int attemptNo = 0;

        while (CheckRoomOverlap(roomWidth, roomHeight, this))
        {
            transform.position = new Vector3(Random.Range(0, levelWidth), Random.Range(0, levelHeight));

            attemptNo++;

            if (attemptNo >= maxAttempts)
            {
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
                LevelManager.Instance.Tiles.Add(new Point((int)child.transform.position.x, (int)child.transform.position.y), tile);
            }
        }
        catch (System.ArgumentException e)
        {
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
