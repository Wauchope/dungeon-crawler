using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    //Created using https://www.raywenderlich.com/177695/procedural-generation-mazes

    private MazeDataGenerator dataGenerator;

    [SerializeField]
    private Transform maze;

    private GameObject[] tilePrefabs;

    public int[,] Data
    {
        get; private set;
    }

    void Awake()
    {
        tilePrefabs = LevelManager.Instance.tilePrefabs;
        dataGenerator = new MazeDataGenerator();

        Data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
    }

    public void GenerateNewMaze(int width, int height)
    {
        Data = dataGenerator.FromDimensions(width, height);
        int data = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                try
                {
                    Tile testTile = LevelManager.Instance.Tiles[new Point(x, y)];
                }
                catch (KeyNotFoundException e)
                {
                    if(Data[x, y] == 1)
                    {
                        data = 0;
                    }
                    else
                    {
                        data = 1;
                    }
                    Tile newTile = Instantiate(tilePrefabs[data], new Vector3(x, y, 0), Quaternion.identity, maze.transform).GetComponent<Tile>();
                    newTile.name = LevelManager.Instance.tileTypes[data];
                    newTile.Setup(x, y);
                }
            }
        }
    }
}