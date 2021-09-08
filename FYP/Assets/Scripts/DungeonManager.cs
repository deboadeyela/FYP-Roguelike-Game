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
        public TileType typeOfTile;   //Holds a TileType enum value
        public Vector2 tilePosition;    //Position of tile
        public List<Vector2> adjacentTiles; //List that is used to store the tiles next to the current PathTile

        //Constructor is called to make new PathTile
        public PathTile(TileType typeTile, Vector2 positionTile, int min, int max,
       Dictionary<Vector2, TileType> currentTiles)
        {
            typeOfTile = typeTile;
            tilePosition = positionTile;
            adjacentTiles = getAdjacentPath(min, max, currentTiles);    //function that will calculate which tiles are adjacent
                                                                            //to this tile based on the Dungeon Board dimensions and current tiles that have been
                                                                            //laid out.
        }
    
        //Methods are used to check adjacent tiles to the top, right, bottom, and left of the current PathTile
        //ContainsKey() checks whether is within the grid dimensions and if the tile is already part of the dungeon tile list
        public List<Vector2> getAdjacentPath(int minimum, int maximum,
       Dictionary<Vector2, TileType> currentTiles)
        {
            List<Vector2> pathTiles = new List<Vector2>();
            if (tilePosition.y + 1 < maximum && !currentTiles.ContainsKey(new
           Vector2(tilePosition.x, tilePosition.y + 1)))
            {
                pathTiles.Add(new Vector2(tilePosition.x, tilePosition.y + 1));
            }
            if (tilePosition.x + 1 < maximum && !currentTiles.ContainsKey(new
           Vector2(tilePosition.x + 1, tilePosition.y)))
            {
                pathTiles.Add(new Vector2(tilePosition.x + 1, tilePosition.y));
            }
            if (tilePosition.y - 1 > minimum && !currentTiles.ContainsKey(new
           Vector2(tilePosition.x, tilePosition.y - 1)))
            {
                pathTiles.Add(new Vector2(tilePosition.x, tilePosition.y - 1));
            }
            if (tilePosition.x - 1 >= minimum && !currentTiles.ContainsKey(new
           Vector2(tilePosition.x - 1, tilePosition.y)) && typeOfTile != TileType.essential)
            {
                pathTiles.Add(new Vector2(tilePosition.x - 1, tilePosition.y));
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

        BuildEssential();

        BuildRandom();
    }

    private void BuildEssential()
    {
        int randomYCoordinate = Random.Range(0, maxBound + 1); //Randomly choose y coordinate for our entrance
        //Container for path tile 
        //Initially set to entrance location
        PathTile essentialPath = new PathTile(TileType.essential, new Vector2(0,randomYCoordinate), minBound, maxBound, gridPositions);
        startPos = essentialPath.tilePosition; //Entrance location where player is moved to
        int gridLength = 0; //Tracks how far along grid length we are 

        //Loops through spaces in tile grid and ends when essential path 
        //has reached right side of grid
        while (gridLength < maxBound)
        {
            gridPositions.Add(essentialPath.tilePosition, TileType.empty); //Add current tile to dictionary
            int adjacentTileCount = essentialPath.adjacentTiles.Count; //Count Adjacent tiles
            int randomNum = Random.Range(0, adjacentTileCount); //Use PRN to choose random adjacent tile to follow
            Vector2 nextEssentialPathPos; //reference to next random adjacent tile that algorithm will follow
            if (adjacentTileCount > 0) //check if there are adjacent tiles
            {
                nextEssentialPathPos = essentialPath.adjacentTiles[randomNum]; //next random adjacent tile is chosen to be followed
            }
            else
            {
                break; //if there are no adjacent tiles, then end of the grid reached and loop can be broken
            }
            PathTile nextEssentialPath = new PathTile(TileType.essential, nextEssentialPathPos,
           minBound, maxBound, gridPositions); //next random adjacent tile is chosen to be followed and is part of essential path 
            if (nextEssentialPath.tilePosition.x > essentialPath.tilePosition.x || (nextEssentialPath.tilePosition.x
           == maxBound - 1 && Random.Range(0, 2) == 1)) //if loop checks if path is moving right
            {
                ++gridLength;
            }
            essentialPath = nextEssentialPath; //tile is then set to current after all the above has been completed
        }
        //IF loop checks if FINAL tile was added to list as loop may have have broken earlier
        //This final tile is then set to end position of dungeon
        if (!gridPositions.ContainsKey(essentialPath.tilePosition))
            gridPositions.Add(essentialPath.tilePosition, TileType.empty);

        endPos = new Vector2(essentialPath.tilePosition.x, essentialPath.tilePosition.y);
    }

    //Checks if there are open adjacent path tiles
    //that can branch off into alternate path
    //and then chooses whether to add "chamber"
    //at end of path
    private void BuildRandom()
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

            int adjacentTileCount = tile.adjacentTiles.Count; //Checks for adjacent tiles
            if (adjacentTileCount != 0)
            {
                //1 in 5 chance path will become chamber
                if (Random.Range(0, 5) == 1)
                {
                    BuildRandomChamber(tile);
                }
                else if (Random.Range(0, 5) == 1 || (tile.typeOfTile == TileType.random
               && adjacentTileCount > 1)) //1 in 5 chance that current random tile will continue to develop path 
                {
                    int randomNum = Random.Range(0, adjacentTileCount);

                    Vector2 newRPathPos = tile.adjacentTiles[randomNum];
                    //Checks randpm path tile isnt already part of dungeon
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
                        pathQueue.Add(newRPath); //add new tile to dictionary

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
        int chamberDimension = 3, adjacentTileCount = tile.adjacentTiles.Count, randomIndex = Random.Range(0, adjacentTileCount); //Chamber is 3x3 //Choose adjacent tile
        Vector2 chamberStart = tile.adjacentTiles[randomIndex]; //Origin of chamber

        //Loop through tiles we need to add
        for (int x = (int)chamberStart.x; x < chamberStart.x + chamberDimension; x++)
        {
            for (int y = (int)chamberStart.y; y < chamberStart.y + chamberDimension; y++)
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
