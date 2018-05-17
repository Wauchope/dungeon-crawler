using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    //Created using https://www.raywenderlich.com/177695/procedural-generation-mazes

    private MazeDataGenerator dataGenerator;

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

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Check if tile exists
                //Instantiate the game object
            }
        }
    }
}