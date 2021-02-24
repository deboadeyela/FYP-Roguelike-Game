using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic; 		//Allows use of C# lists
using Random = UnityEngine.Random; 		//The Unity Engine random number generator.

public class BoardManager : MonoBehaviour
{
    // Serializable allows us to look at class's properties within Unity Editor
    [Serializable]
    public class Count
    {
        //properties Count will keep a track of. Min and max used as random range
        public int minValue;
        public int maxValue;

        public Count(int min, int max)
        {
            minValue = min;
            maxValue = max;
        }
    }

    private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
    public int columns = 5;                                         //# columns in game
    public int rows = 5;                                            //# rows in game
    public GameObject[] floorTiles;
    private Dictionary<Vector2, Vector2> gridPositions = new Dictionary<Vector2, Vector2>(); //Dictionary holds list of game references for game tiles
   

    //Create GameBoard and add tile references dictionary.
    public void BoardSetup()
    {
        //Instantiate Board and set boardHolder to its transform.
        boardHolder = new GameObject("Board").transform;

        //Nested for-loop l to iterate over every cell in 7x7 grid.

        for (int x = 0; x < columns; x++)
        {
            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
            for (int y = 0; y < rows; y++)
            {
                gridPositions.Add(new Vector2(x, y), new Vector2(x, y));
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)]; //Randomly instantiates a floor tile 
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; //Randomly choose co-ordinates for floor tile
                instance.transform.SetParent(boardHolder); //Floor tile set to child of board holder

            }
        }
    }
    

    //SetupScene initializes our level and calls the previous functions to lay out the game board
    public void addToBoard(int horizontal, int vertical)
    {
        if (horizontal == 1)
        {
            //Check if tiles exist
            int x = (int)Player.position.x;
            int sightX = x + 2;
            for (x += 1; x <= sightX; x++)
            {
                int y = (int)Player.position.y;
                int sightY = y + 1;
                for (y -= 1; y <= sightY; y++)
                {
                    addTiles(new Vector2(x, y));
                }
            }
        }
        else if (horizontal == -1)
        {
            int x = (int)Player.position.x;
            int sightX = x - 2;
            for (x -= 1; x >= sightX; x--)
            {
                int y = (int)Player.position.y;
                int sightY = y + 1;
                for (y -= 1; y <= sightY; y++)
                {
                    addTiles(new Vector2(x, y));
                }
            }
        }
        else if (vertical == 1)
        {
            int y = (int)Player.position.y;
            int sightY = y + 2;
            for (y += 1; y <= sightY; y++)
            {
                int x = (int)Player.position.x;
                int sightX = x + 1;
                for (x -= 1; x <= sightX; x++)
                {
                    addTiles(new Vector2(x, y));
                }
            }
        }
        else if (vertical == -1)
        {
            int y = (int)Player.position.y;
            int sighty = y - 2;
            for (y -= 1; y >= sighty; y--)
            {
                int x = (int)Player.position.x;
                int sightx = x + 1;
                for (x -= 1; x <= sightx; x++)
                {
                    addTiles(new Vector2(x, y));
                }
            }
        }
    }



    private void addTiles(Vector2 tileToAdd)
    {
        if (!gridPositions.ContainsKey(tileToAdd))
        {
            gridPositions.Add(tileToAdd, tileToAdd);
            GameObject toInstantiate = floorTiles[Random.Range(0,
           floorTiles.Length)];
            GameObject instance = Instantiate(toInstantiate, new Vector3
           (tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;

            instance.transform.SetParent(boardHolder);
        }
    }
}


