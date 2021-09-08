using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float gameStartDelay = 15f;                      //Time to wait before starting level, in seconds.
    public GameObject levelImage;
    private bool doingSetup = false;
    public float turnDelay = 0.05f;                          //Delay between each Player turn.
    public int playerHealth = 100;                          //Starting value for Player food points.
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.
    private DungeonManager dungeonScript; //Reference to Dungeon
    private Player playerScript; //Reference to Playeer
    private Loader loaderScript; //Reference to Playeer
    public int enemyHealth = 70;
    //public Text gameText;   //Text to display current level number.
    private Text subText;
    private Text levelText;
    private Text pressText;
    //  private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.
    private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
    public int attackRating = 0;                                                  // private int level = 1;                                  //Current level number, expressed in game as "Day 1".
    private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
    private bool enemiesMoving;                             //Boolean to check if enemies are moving.
    public int defenceRating = 0;                                             // private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.
    private bool playerInDungeon;
    public bool enemiesFaster = false;
    public bool enemiesSmarter = false;
    public int enemySpawnRatio = 100;
    public int exploredDungeons = 0;
    public int enemiesKilled = 0;
    public int playerScore = 0;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Assign enemies to a new List of Enemy objects.
        enemies = new List<Enemy>();
        loaderScript = GetComponent<Loader>();
        //Get a component reference to the attached BoardManager/Dungeon/Player script
        boardScript = GetComponent<BoardManager>();
        dungeonScript = GetComponent<DungeonManager>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    //this is called only once, and the paramter tell it to be called only after the scene was loaded
    //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //instance.level++;
        instance.InitGame();
    }


    //Initializes the game for each level.
    void InitGame()
    {
      //  Screen.SetResolution(640, 480, true);
        //While doingSetup is true the player can't move, prevent player from moving while title card is up.
        doingSetup = true;

        //Get a reference to our image LevelImage by finding it by name.
         levelImage = GameObject.Find("LevelImage");

        //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        subText = GameObject.Find("Itext").GetComponent<Text>();
        pressText = GameObject.Find("PressText").GetComponent<Text>();

        //Set the text of levelText to the string "Day" and append the current level number.
        levelText.text = "FYP Game";
        subText.text = "\n\n\n- Traverse the world, battle enemies and explore dungeons!\n- Health items can be found by exloring world\n -Armour can be found in chests\n- Armour is ranked in strength by blue = low, green =medium,\nyellow = high, red = very high\n - Steel, gold and diamond swords can be also found in chests\n -Swords are ranked in strength by steel = low, gold = medium, diamond = high.\n -Your player's max Attack & Defense rating is 30\n-It's much easier to fight enemies with a sword!\n-The stronger you gets the stronger the enemy gets!\n- Enjoy !";
        pressText.text = "";
        //Set levelImage to active blocking player's view of the game board during setup.
        levelImage.SetActive(true);

        //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
        Invoke("HideLevelImage", gameStartDelay);

        //Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();

        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.BoardSetup();
        playerInDungeon = false;

    }

    
    //Hides black image used between levels
    void HideLevelImage()
    {
        //Disable the levelImage gameObject.
        levelImage.SetActive(false);

        //Set doingSetup to false allowing player to move again.
        doingSetup = false;
    }

    //Update is called every frame.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            {
             Restart();
            }

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameFinished();
        }
        //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if (playersTurn || enemiesMoving || doingSetup)

            //If any of these are true, return and do not start MoveEnemies.
            return;

        //Start moving enemies.
        StartCoroutine(MoveEnemies());
    }

    //Call this to add the passed in Enemy to the List of Enemy objects.
    public void AddEnemyToList(Enemy script)
    {
        //Add Enemy to List enemies.
        enemies.Add(script);
    }

    //Removes enemy from list once they are killed
     public void RemoveEnemyFromList(Enemy script)
     {
         enemies.Remove(script);
       // enemiesKilled++;
     }

    

    //GameOver is called when the player reaches 0 food points
    public void GameOver()
    {
        //Set levelText to display number of levels passed and game over message

        playerScore = 56 * (exploredDungeons + enemiesKilled);
        //Enable black background image gameObject.
          levelImage.SetActive(true);
        levelText.text = "Game Over";
        subText.text = "Dungeons Completed: " + exploredDungeons + "\nEnemies Killed: " + enemiesKilled + "\nScore: " + playerScore;
        pressText.text = "\n\n-Press 'R' to restart from last checkpoint\n-Press 'P' to end game";
        doingSetup = true;
        //  SoundManager.instance.efxSource.();
        //Disable this GameManager.
        // enabled = false;
        // yield return new WaitForSeconds(gameStartDelay);
        //Restart();
        // Destroy(gameObject);
        //   instance = null;
        //  Awake();
        //  Destroy(gameObject);
        // loaderScript.Awake();
    }

    // public void updateBoard(int horizantal, int vertical) { }
    //Restart game for player
    public void Restart()
    {
        /* if (Input.GetKeyDown("space"))
         {
             //   yield return new WaitForSeconds(7);
           //  Invoke("HideLevelImage", 0.1f);
             enabled = false;
         }*/
        // else if (Input.GetKeyDown(KeyCode.R)) {
        //  yield return new WaitForSeconds(5f);
        doingSetup = false;
        playerScript.health = 100;
        exploredDungeons = 0;
        enemiesKilled = 0;
        playerScore = 0;
        //playerScript.attackMod= 0;
        //playerScript.defenseMod = 0;
        SoundManager.instance.musicSource.Play();
        playerHealth = 100;
      //  attackRating = 0;
        //defenceRating = 0;
        Invoke("HideLevelImage", 0.1f);
        playerScript.healthText.text = "Health: " + playerHealth;
      //  playerScript.attackText.text = "Attack: " + attackRating;
      //  playerScript.defenceText.text = "Defence: " + defenceRating;
        //   }
        //  if (Input.GetKeyDown(KeyCode.R))
        //{
        // enabled = false;
        //}
    }

    //Permanentely ends game
    public void GameFinished()
    {
        levelImage.SetActive(true);
        levelText.text = "\n\n\n\n\nGame Ended";
        subText.text = "";
        pressText.text = "";
        enabled = false;
    }
        //Coroutine to move enemies in sequence.
        IEnumerator MoveEnemies()
    {
        //While enemiesMoving is true player is unable to move.
        enemiesMoving = true;

        //Wait for turnDelay seconds, defaults to .1 (100 ms).
        yield return new WaitForSeconds(turnDelay);

        //If there are no enemies spawned (IE in first level):
        if (enemies.Count == 0)
        {
            //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(turnDelay);
        }
        List<Enemy> enemiesToDestroy = new List<Enemy>();
        //Loop through List of Enemy objects.
        for (int i = 0; i < enemies.Count; i++)
        {
            //Halt enemy movement until Player is on screen
            if (playerInDungeon)
            {
                if ((!enemies[i].GetComponent<SpriteRenderer>().isVisible))
                {
                    if (i == enemies.Count - 1) //if in the dungeon and all enemies are off screen i.e. usually when Player first enters dungeon
                        yield return new WaitForSeconds(enemies[i].moveTime); //Add time delay for last enemy to avoid jitter affect
                    continue;
                }
            }
            else {
                //Checks whether enemy is on camera and if they are standing on a valid tile
                //Enemy is destroyed if it does not meet conditons
                //If conditions are met they moves a single space and waits for turn delay
                            if ((!enemies[i].GetComponent<SpriteRenderer>().isVisible) || (!boardScript.checkValidTile(enemies[i].transform.position)))
                {
                    enemiesToDestroy.Add(enemies[i]);
                    continue;

                }
            }
            //Call the MoveEnemy function of Enemy at index i in the enemies List.
            enemies[i].MoveEnemy();

            //Wait for Enemy's moveTime before moving next Enemy, 
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        //Once Enemies are done moving, set playersTurn to true so player can move.
        playersTurn = true;

        //Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;

        //Destroy and clear enemies from List
        for (int i = 0; i < enemiesToDestroy.Count; i++)
        {
            enemies.Remove(enemiesToDestroy[i]);
            Destroy(enemiesToDestroy[i].gameObject);
        }
        enemiesToDestroy.Clear();

    }

    public void updateBoard(int horizantal, int vertical)
    {
        boardScript.addToBoard(horizantal, vertical);

    }

    //Driver for dungeon generation
    //Generate the dungeon data then pass it to the BoardManager class to place the dungeon onscreen
    //Notify the Player class that the dungeon transition has occurred
    public void enterDungeon()
    {
        dungeonScript.StartDungeon();
        boardScript.SetDungeonBoard(dungeonScript.gridPositions, dungeonScript.maxBound, dungeonScript.endPos);
        playerScript.dungeonTransition = false;
        playerInDungeon = true;

        //Destroy all enemies from world board
        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i].gameObject);
        }
        enemies.Clear(); 
    }

    //Driver for returning to world board
    public void exitDungeon()
    {
        boardScript.SetWorldBoard();
        playerScript.dungeonTransition = false;
        playerInDungeon = false;
        enemies.Clear(); //Clear enemies from list when exiting dungeon
        exploredDungeons++;
    }

    
}

