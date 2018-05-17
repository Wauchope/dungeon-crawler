using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int tileType { get; private set; }

    public Point GridPosition;

    public void Setup(int X, int Y, int tileType)
    {
        this.tileType = tileType;
        GridPosition = new Point(X, Y);
    }

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}
}
