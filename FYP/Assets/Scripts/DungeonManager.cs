 using UnityEngine;
 using System;
 using System.Collections.Generic;
 using Random = UnityEngine.Random;

//Enumerations used to keep track of states
//Enumeration works like a state ID
public enum TileType
{
    essential, random, empty, chest, enemy
}

//Class will create the data the dungeon is made of and pass it to the BoardManager class
public class DungeonManager : MonoBehaviour
{
    [Serializable]
    public class PathTile
    {
        public TileType type;   //Holds a TileType enum value
        public Vector2 position;    //Position of tile
        public List<Vector2> adjacentPathTiles; //List that is used to store the tiles next to the current PathTile

        //Constructor is called to make new PathTile
        public PathTile(TileType t, Vector2 p, int min, int max,
       Dictionary<Vector2, TileType> currentTiles)
        {
            type = t;
            position = p;
            adjacentPathTiles = getAdjacentPath(min, max, currentTiles);    //function that will calculate which tiles are adjacent
                                                                            //to this tile based on the Dungeon Board dimensions and current tiles that have been
                                                                            //laid out.
        }
    
        //Methods are used to check adjacent tiles to the top, right, bottom, and left of the current PathTile
        //ContainsKey() checks whether is within the grid dimensions and if the tile is already part of the dungeon tile list
        public List<Vector2> getAdjacentPath(int minBound, int maxBound,
       Dictionary<Vector2, TileType> currentTiles)
        {
            List<Vector2> pathTiles = new List<Vector2>();
            if (position.y + 1 < maxBound && !currentTiles.ContainsKey(new
           Vector2(position.x, position.y + 1)))
            {
                pathTiles.Add(new Vector2(position.x, position.y + 1));
            }
            if (position.x + 1 < maxBound && !currentTiles.ContainsKey(new
           Vector2(position.x + 1, position.y)))
            {
                pathTiles.Add(new Vector2(position.x + 1, position.y));
            }
            if (position.y - 1 > minBound && !currentTiles.ContainsKey(new
           Vector2(position.x, position.y - 1)))
            {
                pathTiles.Add(new Vector2(position.x, position.y - 1));
            }
            if (position.x - 1 >= minBound && !currentTiles.ContainsKey(new
           Vector2(position.x - 1, position.y)) && type != TileType.essential)
            {
                pathTiles.Add(new Vector2(position.x - 1, position.y));
            }
            return pathTiles;
        }
    }

    public Dictionary<Vector2, TileType> gridPositions = new Dictionary<Vector2, TileType>(); //Used to store structure of dungeon

    public int minBound = 0, maxBound; //Dimensinons of board grid

    public static Vector2 startPos; //Entrance position of dungeon

    public Vector2 endPos; 

    //Driver function
    //Clears dictionary for further generations
    //Randomly chooses dimension of board grid
    //Calls functions that will generate essential and random path
    public void StartDungeon()
    {
        gridPositions.Clear();
        maxBound = Random.Range(50, 101);

        BuildEssentialPath();

        BuildRandomPath();
    }

    private void BuildEssentialPath()
    {
        int randomY = Random.Range(0, maxBound + 1); //Randomly choose y coordinate for our entrance
        //Container for path tile 
        //Initially set to entrance location
        PathTile ePath = new PathTile(TileType.essential, new Vector2(0,randomY), minBound, maxBound, gridPositions);
        startPos = ePath.position; //Entrance location where player is moved to
        int boundTracker = 0; //Tracks how far along grid length we are 

        //Loops through spaces in tile grid and ends when essential path 
        //has reached right side of grid
        while (boundTracker < maxBound)
        {
            gridPositions.Add(ePath.position, TileType.empty);
            int adjacentTileCount = ePath.adjacentPathTiles.Count;
            int randomIndex = Random.Range(0, adjacentTileCount);
            Vector2 nextEPathPos;
            if (adjacentTileCount > 0)
            {
                nextEPathPos = ePath.adjacentPathTiles[randomIndex];
            }
            else
            {
                break;
            }
            PathTile nextEPath = new PathTile(TileType.essential, nextEPathPos,
           minBound, maxBound, gridPositions);
            if (nextEPath.position.x > ePath.position.x || (nextEPath.position.x
           == maxBound - 1 && Random.Range(0, 2) == 1))
            {
                ++boundTracker;
            }
            ePath = nextEPath;
        }

        if (!gridPositions.ContainsKey(ePath.position))
            gridPositions.Add(ePath.position, TileType.empty);

        endPos = new Vector2(ePath.position.x, ePath.position.y);
    }

    //Checks if there are open adjacent path tiles
    //that can branch off into alternate path
    //and then chooses whether to add "chamber"
    //at end of path
    private void BuildRandomPath()
    {
        
        List<PathTile> pathQueue = new List<PathTile>(); //List is used for queue
        List<PathTile> tempQueue = new List<PathTile>();
        var tempArray = pathQueue.ToArray(); //temporary array is used for list
        int i ;

        //Copy essential path to path queue
        foreach (KeyValuePair<Vector2, TileType> tile in gridPositions)
        {
            Vector2 tilePos = new Vector2(tile.Key.x, tile.Key.y);
      
            tempQueue.Add(new PathTile(TileType.random, tilePos, minBound, maxBound, gridPositions));
            pathQueue.Add(new PathTile(TileType.random, tilePos, minBound,maxBound, gridPositions));
        }

        tempQueue.ForEach(delegate (PathTile tile)
        //Used to process tiles
        //foreach (PathTile tile in tempArray)
        {

            int adjacentTileCount = tile.adjacentPathTiles.Count; //Checks for adjacent tiles
            if (adjacentTileCount != 0)
            {
                //1 in 5 chance path will become chamber
                if (Random.Range(0, 5) == 1)
                {
                    BuildRandomChamber(tile);
                }
                else if (Random.Range(0, 5) == 1 || (tile.type == TileType.random
               && adjacentTileCount > 1))
                {
                    int randomIndex = Random.Range(0, adjacentTileCount);

                    Vector2 newRPathPos = tile.adjacentPathTiles[randomIndex];
                    //Checks path tile isnt already part of dungeon
                    if (!gridPositions.ContainsKey(newRPathPos))
                    {
                        if (Random.Range(0, 20) == 1)
                        {
                            gridPositions.Add(newRPathPos, TileType.enemy);
                        }
                        else
                        {
                            gridPositions.Add(newRPathPos, TileType.empty);
                        }

                        PathTile newRPath = new PathTile(TileType.random, newRPathPos,
                       minBound, maxBound, gridPositions);
                        pathQueue.Add(newRPath);

                        //tempArray[i] = newRPath;
                        //i++;
                        //  var tempArray = pathQueue.ToArray();
                    }
                }
            }
        });
    }

    //Adds chamber to random path
    private void BuildRandomChamber(PathTile tile)
    {
        int chamberSize = 3, adjacentTileCount = tile.adjacentPathTiles.Count, randomIndex = Random.Range(0, adjacentTileCount); //Chamber is 3x3 //Choose adjacent tile
        Vector2 chamberOrigin = tile.adjacentPathTiles[randomIndex]; //Origin of chamber

        //Loop through tiles we need to add
        for (int x = (int)chamberOrigin.x; x < chamberOrigin.x + chamberSize; x++)
        {
            for (int y = (int)chamberOrigin.y; y < chamberOrigin.y + chamberSize; y++)
            {
                Vector2 chamberTilePos = new Vector2(x, y);
                if (!gridPositions.ContainsKey(chamberTilePos) && chamberTilePos.x < maxBound && chamberTilePos.x > 0 && chamberTilePos.y < maxBound && chamberTilePos.y > 0)

                    if (Random.Range(0, 70) == 1)
                    {
                        gridPositions.Add(chamberTilePos, TileType.chest);
                    }
                    else
                    {
                        gridPositions.Add(chamberTilePos, TileType.empty);
                    }

                // gridPositions.Add(chamberTilePos, TileType.empty); //Add new tiles to dictionary
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
