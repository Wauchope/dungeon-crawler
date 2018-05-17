using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDataGenerator
{
    public float placementThreshold;    // chance of empty space

    public MazeDataGenerator()
    {
        placementThreshold = .1f;                               // 1
    }

    public int[,] FromDimensions(int width, int height)    // 2
    {
        int[,] maze = new int[width, height];

        int xMax = maze.GetUpperBound(0);
        int yMax = maze.GetUpperBound(1);

        for (int x = 0; x <= xMax; x++)
        {
            for (int y = 0; y <= yMax; y++)
            {
                if (x == 0 || y == 0 || x == xMax || y == yMax)
                {
                    maze[x, y] = 1;
                }

                else if (x % 2 == 0 && y % 2 == 0)
                {
                    if (Random.value > placementThreshold)
                    {
                        maze[x, y] = 1;

                        int a = Random.value < .5 ? 0 : (Random.value < .5 ? -1 : 1);
                        int b = a != 0 ? 0 : (Random.value < .5 ? -1 : 1);
                        maze[x + a, y + b] = 1;
                    }
                }
            }
        }

        return maze;
    }
}
