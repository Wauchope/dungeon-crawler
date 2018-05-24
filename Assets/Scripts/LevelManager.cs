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

        CreateRoom(map, true);
        CreateHallway(map);
        for (int features = 0; features <= maxFeatures; features++)
        {
            //CreateFeature(map);
        }

        return map;
    }

    private void CreateFeature(int[,] map)
    {
        int[] wallPos = FindWall(map);

        int rand = UnityEngine.Random.Range(0, 4);

        switch (rand)
        {
            case 0:
                CreateRoom(map, false);
                break;
            default:
                CreateHallway(map);
                break;
        }

    }

    private int[,] CreateHallway(int[,] map)
    {
        int[,] mapO = map;

        int levelWidth = map.GetUpperBound(0);
        int levelHeight = map.GetUpperBound(1);

        Debug.Log(levelWidth + ", " + levelHeight);

        int length = UnityEngine.Random.Range(5, 10);

        int[] wallPos = FindWall(map);
        int x = wallPos[0];
        int y = wallPos[1];

        int[] target = new int[] { 0, 0 };

        int rand = 0;

        while (!TileInBounds(map, target[0], target[1]))
        {
            rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    y += length;
                    break;
                case 1:
                    y -= length;
                    break;
                case 2:
                    x += length;
                    break;
                case 3:
                    x -= length;
                    break;
            }

            if (TileInBounds(map, x, y))
            {
                target = new int[] { x, y };
            }
            else
            {
                x = wallPos[0];
                y = wallPos[1];
                length--;

                if (length < 5)
                {
                    return mapO;
                }
            }
        }

        if (target[0] == wallPos[0])
        {
            //Vertical hallway
            int displacement = target[1] - wallPos[1];

            if (displacement > 0)
            {
                //Build hallway in positive y direction
                for (int yPos = wallPos[1]; yPos < target[1]; yPos++)
                {
                    if (!IntersectsFeature(map, wallPos[0], yPos))
                    {
                        map[wallPos[0], yPos] = 1;
                    }
                    else return mapO;
                }
            }
            else
            {
                for (int yPos = wallPos[1]; yPos > target[1]; yPos++)
                {
                    if (!IntersectsFeature(map, wallPos[0], yPos))
                    {
                        map[wallPos[0], yPos] = 1;
                    }
                    else return mapO;
                }
            }
        }
        else
        {
            //Horizontal hallway
            int displacement = target[0] - wallPos[0];

            if (displacement > 0)
            {
                //Build hallway in positive x direction
                for (int xPos = wallPos[0]; xPos < target[0]; xPos++)
                {
                    if (!IntersectsFeature(map, xPos, wallPos[1]))
                    {
                        map[xPos, wallPos[1]] = 1;
                    }
                    else return mapO;
                }
            }
            else
            {
                for (int xPos = wallPos[0]; xPos > target[0]; xPos++)
                {
                    if (!IntersectsFeature(map, xPos, wallPos[1]))
                    {
                        map[xPos, wallPos[1]] = 1;
                    }
                    else return mapO;
                }
            }
        }
        return map;
    }

    private int[] FindWall(int[,] map)
    {
        int x = 0;
        int y = 0;

        int levelWidth = map.GetUpperBound(0);
        int levelHeight = map.GetUpperBound(1);

        while (!IsWall(map, x, y))
        {
            x = UnityEngine.Random.Range(0, levelWidth);
            y = UnityEngine.Random.Range(0, levelHeight);
        }

        return new int[] { x, y };
    }

    private bool IsWall(int[,] map, int x, int y)
    {
        int levelWidth = map.GetUpperBound(0);
        int levelHeight = map.GetUpperBound(1);

        //Check if the position is on the edge of the map
        if (TileInBounds(map, x, y) && map[x, y] != 1)
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

    private int[,] CreateRoom(int[,] map, bool firstRoom)
    {
        int[,] mapO = map;

        int levelWidth = map.GetUpperBound(0);
        int levelHeight = map.GetUpperBound(1);

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
            int[] wallPos = FindWall(map);
            xPos = wallPos[0];
            yPos = wallPos[1];
        }

        while (area == 0 || area > maxArea)
        {
            roomWidth = UnityEngine.Random.Range(levelWidth / 16, levelWidth / 8);
            roomHeight = UnityEngine.Random.Range(levelHeight / 16, levelHeight / 8);

            area = roomWidth * roomHeight;
        }

        if (TileInBounds(map, xPos + roomWidth, yPos + roomHeight))
        {
            //Create the room in the map array
            for (int x = xPos; x <= xPos + roomWidth; x++)
            {
                for (int y = yPos; y <= yPos + roomHeight; y++)
                {
                    if (!IntersectsFeature(map, x, y))
                    {
                        map[x, y] = 1;
                    }
                    else return mapO;
                }
            }

        }
        else return mapO;

        return map;
    }

    private bool TileInBounds(int[,] map, int xPos, int yPos)
    {
        int levelWidth = map.GetUpperBound(0);
        int levelHeight = map.GetUpperBound(1);

        if (xPos < levelWidth && xPos > 0 && yPos < levelHeight && yPos > 0)
        {
            return true;
        }
        else return false;
    }

    private bool IntersectsFeature (int[,] map, int xPos, int yPos)
    {
        if (map[xPos, yPos] == 1)
        {
            return true;
        }
        else return false;
    }

    void Update () {
		
	}
}