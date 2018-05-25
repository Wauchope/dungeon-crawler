using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//http://www.roguebasin.roguelikedevelopment.org/index.php?title=Dungeon-Building_Algorithm
//Currently on step 5
//To do: Check if the new feature will intersect with another feature (hallways are fine)

public class LevelManager : MonoBehaviour
{
    BoardCreator levelGen;

	void Start ()
    {
        levelGen = GetComponent<BoardCreator>();

        levelGen.CreateLevel();
	}

    void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Space))
        {
            levelGen.RemoveLevel();
            levelGen.CreateLevel();
        }
	}
}