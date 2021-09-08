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

    private Transform holderBoard; //A variable to store a reference to the transform of our Board object.
    public int columns = 5; //# columns in game
    public int rows = 5; //# rows in game
    public GameObject[] tileFloor; //GameObeject for holding floor prefabs
    private Dictionary<Vector2, Vector2> gridPositions = new Dictionary<Vector2, Vector2>(); //Dictionary holds list of game references for game tiles
    public GameObject dungeonExit; //Used to hold door sprite for entrance/exit
    public GameObject[] dungeonWallTiles; //Used to enclose dungeon
    private Transform holderDungeonBoard; //Transform for dictionary
    private Dictionary<Vector2, Vector2> dungeonGridPositions; //Dictionary for dungeon
    public GameObject[] tileWall;
    public GameObject tileChest;
    public GameObject[] tileFood;
    // public GameObject[] enemy;
    public GameObject[] enemy;

    //Create GameBoard and add tile references dictionary.
    public void BoardSetup()
    {
        //Instantiate Board and set boardHolder to its transform.
        holderBoard = new GameObject("Board").transform;

        //Nested for-loop l to iterate over every cell in 5x5 grid.
        for (int x = 0; x < columns; x++)
        {
            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
            for (int y = 0; y < rows; y++)
            {
                gridPositions.Add(new Vector2(x, y), new Vector2(x, y)); //Add to dictionary
                GameObject objectInstantiate = tileFloor[Random.Range(0, tileFloor.Length)]; //Randomly instantiates a floor tile from array
                GameObject instance = Instantiate(objectInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; //Randomly choose co-ordinates for floor tile
                instance.transform.SetParent(holderBoard); //Floor tile set to child of board holder

            }
        }
    }

    //Gets Postion of an enemy and check to see whether that position is in the dictionary of visible floor tiles
    public bool checkValidTile(Vector2 pos)
    {
        if (gridPositions.ContainsKey(pos))
        {
            return true;
        }
        return false;
    }

    //Entry point from GameManager class
    public void addToBoard(int horizontal, int vertical)
    {
        //IF statement for when player is moving a right
        if (horizontal == 1)
        {
            //Nested for-loop used to iterate over player's line of sight which is a 2x3 grid
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
        //ELSE-IF statements used to check other directions player moves in 
        //And establish line of sight for that direction
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
        //Checks line of sight tiles and add tiles if they're not there
        if (!gridPositions.ContainsKey(tileToAdd))
        {
            gridPositions.Add(tileToAdd, tileToAdd);
            GameObject objectInstantiate = tileFloor[Random.Range(0,
           tileFloor.Length)];
            GameObject instance = Instantiate(objectInstantiate, new Vector3
           (tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;
            instance.transform.SetParent(holderBoard);

            /*if (Random.Range(0, 3) == 1)
            {
                toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
                instance = Instantiate(toInstantiate, new Vector3(tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }*/

            //1 in 40 probability an food tile is spawned
           if (Random.Range(0, 40) == 1)
            {
                objectInstantiate = tileFood[Random.Range(0, tileFood.Length)];
                instance = Instantiate(objectInstantiate, new Vector3(tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(holderBoard);
            }
            //1 in 100 probability that an exit tile is spawned 
            else if (Random.Range(0, 100) == 1)
            {
                objectInstantiate = dungeonExit;
                instance = Instantiate(objectInstantiate, new Vector3(tileToAdd.x,
               tileToAdd.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(holderBoard);
            }
            //1 in enemySpawnRatio probability that an enemy tile is spawned
            else if (Random.Range(0, GameManager.instance.enemySpawnRatio) == 0)
            {
                objectInstantiate = enemy[Random.Range(0, enemy.Length)];
                instance = Instantiate(objectInstantiate, new Vector3(tileToAdd.x,
               tileToAdd.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(holderBoard);
            }
        }
    }

    //Applies graphics to dungeon data
    public void SetDungeonBoard(Dictionary<Vector2, TileType> dungeonTiles, int bound, Vector2 endPos)
    {
        holderBoard.gameObject.SetActive(false); //Remove world board by setting it to inactive
        holderDungeonBoard = new GameObject("Dungeon").transform;
        GameObject objectInstantiate, instance;

        //Loops through coordinates and place sprite
        foreach (KeyValuePair<Vector2, TileType> tile in dungeonTiles)
        {
            objectInstantiate = tileFloor[Random.Range(0, tileFloor.Length)];
            instance = Instantiate(objectInstantiate, new Vector3(tile.Key.x,
           tile.Key.y, 0f), Quaternion.identity) as GameObject;
            instance.transform.SetParent(holderDungeonBoard);


            if (tile.Value == TileType.chest)
            {
                objectInstantiate = tileChest;
                instance = Instantiate(objectInstantiate, new Vector3(tile.Key.x, tile.Key.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(holderDungeonBoard);
            }

            else if (tile.Value == TileType.enemy)
            {
                objectInstantiate = enemy[Random.Range(0, enemy.Length)];
                instance = Instantiate(objectInstantiate, new Vector3(tile.Key.x,
               tile.Key.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(holderDungeonBoard);
            }
        }

        //Places outer wall tiles to enclose dungeon
        for (int x = -1; x < bound + 1; x++)
        {
            for (int y = -1; y < bound + 1; y++)
            {
                if (!dungeonTiles.ContainsKey(new Vector2(x, y)))
                {
                    objectInstantiate = dungeonWallTiles[Random.Range(0,
                   dungeonWallTiles.Length)];
                    instance = Instantiate(objectInstantiate, new Vector3(x, y, 0f),
                   Quaternion.identity) as GameObject;
                    instance.transform.SetParent(holderDungeonBoard);
                }
            }
        }

        //Sets exit tiles
        objectInstantiate = dungeonExit;
        instance = Instantiate(objectInstantiate, new Vector3(endPos.x,
       endPos.y, 0f), Quaternion.identity) as GameObject;
        instance.transform.SetParent(holderDungeonBoard);
    }

    //Reactivate World board after exiting dungeon
    public void SetWorldBoard()
    {
        Destroy(holderDungeonBoard.gameObject);
        holderBoard.gameObject.SetActive(true);
    }



}


