using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySettings : LevelSettings
{
    
    // Player start position on this gameplay portion.
    public Vector2Int playerPosition = new Vector2Int(0, 3);
    // Exit position on this gameplay portion.
    public Vector2Int exitPosition = new Vector2Int(5, 0);
    // If the level exit should start locked;
    public bool locked = true;

    // Used as developer interface because Unity doesn't serializes 2D arrays.
    public GameObject[] gridLine0;
    public GameObject[] gridLine1;
    public GameObject[] gridLine2;
    public GameObject[] gridLine3;
    
    // Used to store the above variables in abetter way to use inside the code.
    public GameObject[,] levelGrid;

    private void Start() {

        // Creates an array representing this gameplay portion grid.
        levelGrid = new GameObject[6,4];
        for(int i = 0; i < 4; i++) {
            for(int j = 0; j < 6; j++) {

                if(i == 0) {
                    levelGrid[j, i] = gridLine0[j];
                }else if (i == 1)
                    levelGrid[j, i] = gridLine1[j];
                else if (i == 2)
                    levelGrid[j, i] = gridLine2[j];
                else
                    levelGrid[j, i] = gridLine3[j];

            }   
        }
    }
}
