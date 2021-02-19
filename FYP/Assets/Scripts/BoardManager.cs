using UnityEngine;
using System;
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
    public int columns = 7;                                         //# columns in game
    public int rows = 7;                                            //# rows in game
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
                gridPositions.Add(new Vector2(x, y), new Vector2(x, y));                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)]; //Randomly instantiates a floor tile                 GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; //Randomly choose co-ordinates for floor tile
                instance.transform.SetParent(boardHolder); //Floor tile set to child of board holder
            }
            //A list of possible locations to place tiles.

            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}
