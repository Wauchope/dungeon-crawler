using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//http://www.roguebasin.roguelikedevelopment.org/index.php?title=Dungeon-Building_Algorithm
//Currently on step 5
//To do: Check if the new feature will intersect with another feature (hallways are fine)

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private int levelWidth, levelHeight;

    [SerializeField]
    private GameObject[] tilePrefabs;

    [SerializeField]
    private int maxArea;

    [SerializeField]
    private int maxFeatures;

	void Start ()
    {
        CreateMap(levelWidth, levelHeight);
	}

    private void CreateMap(int levelWidth, int levelHeight)
    {
        int[,] mapData = GenerateMapData(levelWidth, levelHeight);

        CreateTiles(mapData);
    }

    private void CreateTiles(int[,] map)
    {
        int xMax = map.GetUpperBound(0);
        int yMax = map.GetUpperBound(1);

        for (int x = 0; x <= xMax; x++)
        {
            for (int y = 0; y <= yMax; y++)
            {
                Instantiate(tilePrefabs[map[x, y]], new Vector3(x, y), Quaternion.identity);
            }
        }
    }

    //0 is wall, 1 is floor
    private int[,] GenerateMapData(int width, int height)
    {
        int[,] map = new int[width, height];

        int xMax = map.GetUpperBound(0);
        int yMax = map.GetUpperBound(1);

        for (int x = 0; x <= xMax; x++)
        {
            for (int y = 0; y <= yMax; y++)
            {
                map[x, y] = 0;
            }
        }

        int maxArea = width * height;

        CreateRoom(map, xMax, yMax, true);
        CreateHallway(map, xMax, yMax);
        for (int features = 0; features <= maxFeatures; features++)
        {
            CreateFeature(map, xMax, yMax);
        }

        return map;
    }

    private void CreateFeature(int[,] map, int levelWidth, int levelHeight)
    {
        int[] wallPos = FindWall(map, levelWidth, levelHeight);

        int rand = UnityEngine.Random.Range(0, 4);

        switch (rand)
        {
            case 0:
                CreateRoom(map, levelWidth, levelHeight, false);
                break;
            default:
                CreateHallway(map, levelWidth, levelHeight);
                break;
        }

    }

    private int[,] CreateHallway(int[,] map, int levelWidth, int levelHeight)
    {
        int length = UnityEngine.Random.Range(0, 10);

        int[] wallpos = FindWall(map, levelWidth, levelHeight);
        int x = wallpos[0];
        int y = wallpos[1];

        int[] target = new int[] { 0, 0 };

        int rand = 0;

        while (!TileInBounds(target[0], target[1], levelWidth, levelHeight))
        {
            rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    if (TileInBounds(x, y + length, levelWidth, levelHeight))
                    {
                        target = new int[] { x, y + length };
                    }
                    break;
                case 1:
                    if (TileInBounds(x, y - length, levelWidth, levelHeight))
                    {
                        target = new int[] { x, y - length };
                    }
                    break;
                case 2:
                    if (TileInBounds(x + length, y, levelWidth, levelHeight))
                    {
                        target = new int[] { x + length, y };
                    }
                    break;
                case 3:
                    if (TileInBounds(x - length, y, levelWidth, levelHeight))
                    {
                        target = new int[] { x - length, y };
                    }
                    break;
            }
        }

        switch (rand)
        {
            case 0:
                if (TileInBounds(x, y + length, levelWidth, levelHeight))
                {
                    for (int offset = 0; offset <= length; offset++)
                    {
                        map[x, y + offset] = 1;
                    }
                }
                break;
            case 1:
                if (TileInBounds(x, y - length, levelWidth, levelHeight))
                {
                    for (int offset = 0; offset <= length; offset++)
                    {
                        map[x, y - offset] = 1;
                    }
                }
                break;
            case 2:
                if (TileInBounds(x + length, y, levelWidth, levelHeight))
                {
                    for (int offset = 0; offset <= length; offset++)
                    {
                        map[x + offset, y] = 1;
                    }
                }
                break;
            case 3:
                if (TileInBounds(x - length, y, levelWidth, levelHeight))
                {
                    for (int offset = 0; offset <= length; offset++)
                    {
                        map[x - offset, y] = 1;
                    }
                }
                break;
        }

        return map;
    }

    private int[] FindWall(int[,] map, int levelWidth, int levelHeight)
    {
        int x = 0;
        int y = 0;

        while (!IsWall(map, x, y, levelWidth, levelHeight))
        {
            x = UnityEngine.Random.Range(0, levelWidth);
            y = UnityEngine.Random.Range(0, levelHeight);
        }

        return new int[] { x, y };
    }

    private bool IsWall(int[,] map, int x, int y, int levelWidth, int levelHeight)
    {
        //Check if the position is on the edge of the map
        if (TileInBounds(x, y, levelWidth, levelHeight) && map[x, y] != 1)
        {
            //Randomizes the direction it will check (for even more randomness!)
            int rand = UnityEngine.Random.Range(0, 4);

            switch (rand)
            {
                case 0:
                    if (map[x, y + 1] == 1)
                    {
                        return true;
                    }
                    break;
                case 1:
                    if (map[x + 1, y] == 1)
                    {
                        return true;
                    }
                    break;
                case 2:
                    if (map[x, y - 1] == 1)
                    {
                        return true;
                    }
                    break;
                case 3:
                    if (map[x - 1, y] == 1)
                    {
                        return true;
                    }
                    break;
            }
        }
        return false;
    }

    private int[,] CreateRoom(int[,] map, int levelWidth, int levelHeight, bool firstRoom)
    {
        int roomWidth = 0;
        int roomHeight = 0;

        int area = 0;

        int xPos = 0;
        int yPos = 0;

        if (firstRoom)
        {
            xPos = UnityEngine.Random.Range(1, levelWidth);
            yPos = UnityEngine.Random.Range(1, levelHeight);
        }
        else
        {
            int[] wallPos = FindWall(map, levelWidth, levelHeight);
            xPos = wallPos[0];
            yPos = wallPos[1];
        }
        bool proceed = false;

        while (!proceed)
        {
            while (area == 0 || area > maxArea)
            {
                roomWidth = UnityEngine.Random.Range(levelWidth / 16, levelWidth / 8);
                roomHeight = UnityEngine.Random.Range(levelHeight / 16, levelHeight / 8);

                area = roomWidth * roomHeight;
            }

            if (TileInBounds(xPos + roomWidth, yPos + roomHeight, levelWidth, levelHeight))
            {
                //Create the room in the map array
                for (int x = xPos; x <= xPos + roomWidth; x++)
                {
                    for (int y = yPos; y <= yPos + roomHeight; y++)
                    {
                        map[x, y] = 1;
                    }
                }

                proceed = true;
            }
        }

        return map;
    }

    private bool TileInBounds(int xPos, int yPos, int levelWidth, int levelHeight)
    {
        if(xPos < levelWidth && xPos > 0 && yPos < levelHeight && yPos > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
