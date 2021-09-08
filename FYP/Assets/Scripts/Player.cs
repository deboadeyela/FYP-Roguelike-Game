using System.Collections;
using UnityEngine;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;
using System.Collections.Generic; 		//Allows use of C# lists
using Random = UnityEngine.Random; 		//The Unity Engine random number generator.
using System;

public class Player : MovingObject
{
    // public float restartLevelDelay = 1f;        //Delay time in seconds to restart level
    //public int foodPoints = 10;                 //Points gained from getting food
    //public int drinkPoints = 20;                //Points gained from getting drink
    public int wallDamage = 1; //Damage that is done to wall by player hit
    public Text healthText; //UI Text display of player health
    private Animator animator; //Used to store a reference to the Player's animator component.
    public int health; //Player health
    public static Vector2 position; //Current coordinates of the player
    public bool onWorldBoard; //Determines if player is on world board in order to then track player's position 
    public bool dungeonTransition; //Switches off movement, to transition dungeon entrance
    
    //Image References
    public Image glove;
    public Image boot;
    //Text refercences for attack/defense ratings
    public Text attackText;
    public Text defenceText;

    //Sword Sprites
    public Sprite steelWeapon;
    public Sprite goldWeapon;
    public Sprite diamondWeapon;
    // public Text gloveText;
    public int attack = 0, defense = 0; //attack/defense ratings
    //public static Player instance;
   // private Armour armourScript;
    private Dictionary<String, Armour> inventory;  //Inventory for armour
    
    private Weapon weapon; //Weapon object
    public Image weaponComp1; //Image for weapon component
    public static bool isFacingRight;
    public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
    public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
    public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
    public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
    public AudioClip drinkSound1;               //1 of 2 Audio clips to play when player collects a soda object.
    public AudioClip drinkSound2;               //2 of 2 Audio clips to play when player collects a soda object.
    public AudioClip gameOverSound;				//Audio clip to play when player dies
    public int c = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();
        //   armourScript = GameObject.FindGameObjectWithTag("Item").GetComponent<Armour>();
        //      instance = this;
        attackText = GameObject.Find("AText").GetComponent<Text>();
        defenceText = GameObject.Find("DText").GetComponent<Text>();
        //Player's Health
        health = GameManager.instance.playerHealth;
        healthText.text = "Health: " + health;
        // gloveText.text = "Gloves: x" + gloveCount;
        attack = GameManager.instance.attackRating;
        defense = GameManager.instance.defenceRating;
        //attackText.text = "Attack: " + attackMod;

        onWorldBoard = true;
        position.x = position.y = 2;    //Player begins game in position (2,2)
        dungeonTransition = false;

        inventory = new Dictionary<String, Armour>();
        base.Start();   //The Start function of the MovingObject base class is called.
    }

    // Update is called once per frame
    private void Update()
    {
      //  attackText.text = "Attack: " + attackMod;
      //  attackText.text = "Attack: " + attackMod;
        //If it's not the player's turn, exit the function.
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;     //Used to store the horizontal move direction.
        int vertical = 0;       //Used to store the vertical move direction.
        bool canMove = false;   //Used to tell whether plaayer can move or not
        bool camMove = false;
        bool cafMove = false;
        //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER
			
			//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			
			//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
			vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
			//Check if moving horizontally, if so set vertical to zero.
			if(horizontal != 0)
			{
				vertical = 0;
			}

#endif      //Check if we have a non-zero value for horizontal or vertical
        if (horizontal != 0 || vertical != 0)
        {
            if (!dungeonTransition)
            {
                Vector2 start = transform.position;
                Vector2 end = start + new Vector2(horizontal, vertical);
                base.boxCollider.enabled = false;
                RaycastHit2D hit = Physics2D.Linecast(start, end, base.blockingLayer);
                base.boxCollider.enabled = true;
                /* if (onWorldBoard)
                 {
                     canMove = AttemptMove<Wall>(horizontal, vertical);
                 }*/
                // else
                //{
                //camMove = AttemptMove<Chest>(horizontal, vertical);
                //}
                if (hit.transform != null)
                {
                    switch (hit.transform.gameObject.tag)
                    {
                        case "Wall":
                            canMove = AttemptMove<Wall>(horizontal, vertical);
                            break;
                        case "Chest":
                            camMove = AttemptMove<Chest>(horizontal, vertical);
                            break;
                        case "Enemy":
                            cafMove = AttemptMove<Enemy>(horizontal, vertical);
                            break;
                    }
                }
                else
                {
                    canMove = AttemptMove<Wall>(horizontal, vertical);
                }
                //Switches off movement, to transition dungeon entrance
                //Determines if player is on world board in order to then track player's position 
                //Position of player is updated by adding values for horizontal/vertical to the player's x/y movement
                //  canMove = AttemptMove<Wall>(horizontal, vertical);
                if (canMove && onWorldBoard)
                {
                    position.x += horizontal;
                    position.y += vertical;
                    GameManager.instance.updateBoard(horizontal, vertical);
                }
            }
        }
    }

    //Overrides the AttemptMove function in the MovingObject class 
    protected override bool AttemptMove<T>(int xDir, int yDir)
    {
        if (xDir == 1 && !isFacingRight)
        {
             isFacingRight = true;
             }
        else if (xDir == -1 && isFacingRight)
        {
             isFacingRight = false;
             }

        //Every time player moves, subtract from food points total.
        //health--;

        //Update food text display to reflect current score.
        // healthText.text = "Health: " + health;

        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        bool hit = base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D trust;
        if (Move(xDir, yDir, out trust))
        {
            //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        GameManager.instance.playersTurn = false;
        return hit;
        //Hit allows us to reference the result of the Linecast done in Move.
        //RaycastHit2D hit;

        //Since the player has moved and lost food points, check if the game has ended.
        //        CheckIfGameOver();

        //Set the playersTurn boolean of GameManager to false now that players turn is over.
        //GameManager.instance.playersTurn = false;
    }

    //Overrides the OnCantMove function in the MovingObject class
    protected override void OnCantMove<T>(T component)
    {
        //Set hitWall to equal the component passed in as a parameter.
        //        Wall hitWall = component as Wall;

        //Call the DamageWall function of the Wall we are hitting.
        //      hitWall.WallDamaged(wallDamage);

        //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
        //animator.SetTrigger("playerChop");

        if (typeof(T) == typeof(Wall))
        {
            Wall blockingO = component as Wall;
            blockingO.WallDamaged(wallDamage);
        }
        else if (typeof(T) == typeof(Chest))
        {
            Chest blockingObj = component as Chest;
            blockingObj.Open();
        }

        else if (typeof(T) == typeof(Enemy))
        {
            Enemy blockingObj = component as Enemy;
            blockingObj.DamageEnemy(wallDamage);
        }

        animator.SetTrigger("playerChop");
         if (weapon)
        {
             weapon.useWeapon();
             }

    }

    //Manage player's health
    public void LoseHealth(int loss)
    {
        //Set the trigger for the player animator to transition to the playerHit animation.
        animator.SetTrigger("playerHit");

        //Subtract lost food points from the players total.
        health -= loss;

        //Update the food display with the new total.
          healthText.text = "Damage:-" + loss + " Health: " + health;

        //Check to see if game has ended.
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        //Check if player has no health
        if (health <= 0)
        {
            //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
            SoundManager.instance.PlaySingle(gameOverSound);

            //Stop the background music.
            SoundManager.instance.musicSource.Stop();
            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver();
        }
    }

    private void GoDungeonPortal()
    {
        //If player is on the world board then they enter a dungeon
        if (onWorldBoard)
        {
            onWorldBoard = false;
            GameManager.instance.enterDungeon();
            transform.position = DungeonManager.startPos;
        }
        //If the player dungeon then they are sent back to the world board
        else
        {
            onWorldBoard = true;
            GameManager.instance.exitDungeon();
            transform.position = position;
        }
    }

    //Update health of player
    private void UpdateHealth(Collider2D item)
    {
        if (health < 100) //Checks if health is less than 100
        {
            //Value of food and soda based on probability values
            if (item.tag == "Food")
            {
                health += Random.Range(1, 4);
                health = Mathf.Clamp(health, 1, 100);
            }
            else
            {
                health += Random.Range(4, 11); //Value of soda is higher than food 
                health = Mathf.Clamp(health, 1, 100);
            }
            GameManager.instance.playerHealth = health;
            healthText.text = "Health: " + health;
        }
    }

    //Called when the player picks up a new Armour
    //Only allowed one item of each type in our inventory
    //New armour is put into the empty slot in our inventory
    //If we already have item, we replace it with the new item that was just picked up
    public void UpdateInventory(Collider2D armour)
    {
        //attackMod = 12; 
        //GameManager.instance.attackRating = attackMod;
        //attackText.text = "Attack: " + attackMod;
        Armour armourData = armour.GetComponent<Armour>();
        switch (armourData.type)
        {
            case armourType.glove:
                if (!inventory.ContainsKey("glove"))
                    inventory.Add("glove", armourData);
                else
                    inventory["glove"] = armourData;

                glove.color = armourData.strength;
                break;
            case armourType.boot:
                if (!inventory.ContainsKey("boot"))
                    inventory.Add("boot", armourData);
                else
                    inventory["boot"] = armourData;

                boot.color = armourData.strength;
                break;
        }

        attack = 0;
        defense = 0;

        foreach (KeyValuePair<String, Armour> gear in inventory)
        {
            attack += gear.Value.attack;
            defense += gear.Value.defense;
            attack = Mathf.Clamp(attack, 0, 30);
            defense = Mathf.Clamp(defense, 0, 30);
            wallDamage += attack;
            wallDamage = Mathf.Clamp(wallDamage, 0, 30);
            // GameManager.instance.attackRating = attackMod;
            //attackText.text = "Attack: " + attackMod;

        }
        GameManager.instance.attackRating += attack;
        GameManager.instance.attackRating = Mathf.Clamp(GameManager.instance.attackRating, 0, 30);
        attackText.text = "Attack: " + GameManager.instance.attackRating;

        GameManager.instance.defenceRating += defense;
        GameManager.instance.defenceRating = Mathf.Clamp(GameManager.instance.defenceRating, 0, 30);
        defenceText.text = "Defence: " + GameManager.instance.defenceRating;
        
        //  attackText.text = "Attack: " + attackMod;
        /*  if (weapon)
          {
              attackMod = attackMod + 200;
             // attackMod += 5;
          }*/
        //c = attackMod;
        //   attackText.text = "Attack: " + c; 
    }

    //Update attack/defence rating based on inventory image sprite
    private void UpdateWeaponScore() {
        attack = 0;
        defense = 0;

        if (weaponComp1.sprite == steelWeapon) {
            Debug.Log("Hello");
            attack += 1;
            attack = Mathf.Clamp(attack, 0, 30);
            GameManager.instance.attackRating += attack;
            GameManager.instance.attackRating = Mathf.Clamp(GameManager.instance.attackRating, 0, 30);
            attackText.text = "Attack: " + GameManager.instance.attackRating;
           

            defense += 1;
            defense = Mathf.Clamp(defense, 0, 30);
            GameManager.instance.defenceRating += defense;
            GameManager.instance.defenceRating = Mathf.Clamp(GameManager.instance.defenceRating, 0, 30);
            defenceText.text = "Defence: " + GameManager.instance.defenceRating;

            wallDamage += attack;
            wallDamage = Mathf.Clamp(wallDamage, 0, 30);
        }

        else if (weaponComp1.sprite == goldWeapon)
        {
            Debug.Log("Hello");
            attack += 2;
            attack = Mathf.Clamp(attack, 0, 30);
            GameManager.instance.attackRating += attack;
            GameManager.instance.attackRating = Mathf.Clamp(GameManager.instance.attackRating, 0, 30);
            attackText.text = "Attack: " + GameManager.instance.attackRating;
            

            defense += 2;
            defense = Mathf.Clamp(defense, 0, 30);
            GameManager.instance.defenceRating += defense;
            GameManager.instance.defenceRating = Mathf.Clamp(GameManager.instance.defenceRating, 0, 30);
            defenceText.text = "Defence: " + GameManager.instance.defenceRating;

            wallDamage += attack;
            wallDamage = Mathf.Clamp(wallDamage, 0, 30);
        }

        else if (weaponComp1.sprite == diamondWeapon)
        {
            Debug.Log("Hello");
            attack += 3;
            attack = Mathf.Clamp(attack, 0, 30);
            GameManager.instance.attackRating += attack;
            GameManager.instance.attackRating = Mathf.Clamp(GameManager.instance.attackRating, 0, 30);
            attackText.text = "Attack: " + GameManager.instance.attackRating;
         

            defense += 3;
            defense = Mathf.Clamp(defense, 0, 30);
            GameManager.instance.defenceRating += defense;
            GameManager.instance.defenceRating = Mathf.Clamp(GameManager.instance.defenceRating, 0, 30);
            defenceText.text = "Defence: " + GameManager.instance.defenceRating;

            wallDamage += attack;
            wallDamage = Mathf.Clamp(wallDamage, 0, 30);
        }

    }

    //Adjusts enemy Ai based off strength of player
    private void AdaptDifficulty()
    {
        if (wallDamage >= 10)
            GameManager.instance.enemiesSmarter = true;
        if (wallDamage >= 20)
            GameManager.instance.enemiesFaster = true;
        if (wallDamage >= 28)
            GameManager.instance.enemySpawnRatio = 40;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Checks if players has triggered exit
        if (other.tag == "Exit")
        {
            dungeonTransition = true;
            Invoke("GoDungeonPortal", 0.5f);
            Destroy(other.gameObject);
        }
        //Checks if the player collides with health items   
        else if (other.tag == "Food")
        {
            UpdateHealth(other);
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            Destroy(other.gameObject);
        }
        else if (other.tag == "Soda") {
            UpdateHealth(other);
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            Destroy(other.gameObject);
        }

        //Checks if the player collides with armour
        else if (other.tag == "Item")
        {
            UpdateInventory(other);
            attackText.text = "Attack: " + GameManager.instance.attackRating;
            defenceText.text = "Defence: " + GameManager.instance.defenceRating;
            Destroy(other.gameObject);
            AdaptDifficulty();
        }

        //Checks if the player collides with weapon
        //Update score 
        //Adjust difficulty
        else if (other.tag == "Weapon")
        {
          
            if (weapon)
            {
                Destroy(transform.GetChild(0).gameObject);
            }
            //attackText.text = "Attack: " + GameManager.instance.attackRating;
            other.enabled = false; //disable box collider of weapon
            other.transform.parent = transform; //set player as parent of weapon
            weapon = other.GetComponent<Weapon>();
            weapon.AcquireWeapon();
            weapon.weaponInInventory = true;
            weapon.enableSpriteRender(false);
           // wallDamage = attackMod + 3;
            weaponComp1.sprite = weapon.getComponentImage(0);
            UpdateWeaponScore();
            AdaptDifficulty();
        }

    }
}
