using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Point GridPosition;

    public void Setup(int X, int Y)
    {
        GridPosition = new Point(X, Y);
    }

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}
}
