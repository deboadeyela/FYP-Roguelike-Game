using System.Collections;
using UnityEngine;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
   
    //public int foodPoints = 10;                 //Points gained from getting food
    //public int drinkPoints = 20;                //Points gained from getting drink
    public int wallDamage = 1;                  //Damage that is done to wall by player hit
  //  public Text healthText;						//UI Text display of player health
    private Animator animator;					//Used to store a reference to the Player's animator component.
    private int health;                         //Player health
    public static Vector2 position;             //Current coordinates of the player

    // Start is called before the first frame update
    protected override void Start()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();

        //Player's Health
        health = GameManager.instance.playerHealth;
        //  healthText.text = "Health: " + health;

        position.x = position.y = 2;    //Player begins game in position (2,2)
        base.Start();   //The Start function of the MovingObject base class is called.
    }

    // Update is called once per frame
    private void Update()
    {
        //If it's not the player's turn, exit the function.
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;     //Used to store the horizontal move direction.
        int vertical = 0;       //Used to store the vertical move direction.
        bool canMove = false;   //Used to tell whether plaayer can move or not

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
            //Position of player is updated by adding values for horizontal/vertical to the player's x/y movement
            canMove = AttemptMove<Wall>(horizontal, vertical);
            if (canMove)
            {
                position.x += horizontal;
                position.y += vertical;
                GameManager.instance.updateBoard(horizontal, vertical);
            }
        }
    }

    //Overrides the AttemptMove function in the MovingObject class 
    protected override bool AttemptMove<T>(int xDir, int yDir) {
        //Every time player moves, subtract from food points total.
        //health--;

        //Update food text display to reflect current score.
        // healthText.text = "Health: " + health;

        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        bool hit = base.AttemptMove<T>(xDir, yDir);
        GameManager.instance.playersTurn = false;
        return hit;
        

        //Since the player has moved and lost health, check if the game has ended.
        CheckIfGameOver();

        //Set the playersTurn boolean of GameManager to false now that players turn is over.
        //GameManager.instance.playersTurn = false;
    }

    //Overrides the OnCantMove function in the MovingObject class
    protected override void OnCantMove<T>(T component) {
        //Set hitWall to equal the component passed in as a parameter.
        Wall hitWall = component as Wall;

        //Call the DamageWall function of the Wall we are hitting.
        hitWall.WallDamaged(wallDamage);

        //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
        animator.SetTrigger("playerChop");
    }

    //Manage player's health
    public void LoseHealth(int loss) {
        //Set the trigger for the player animator to transition to the playerHit animation.
        animator.SetTrigger("playerHit");

        //Subtract lost food points from the players total.
        health -= loss;

        //Update the food display with the new total.
      //  healthText.text = "-" + loss + " Health: " + health;

        //Check to see if game has ended.
        CheckIfGameOver();
    }

    private void CheckIfGameOver() {
        //Check if player has no health
        if (health <= 0) {
            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver();
        }
    }
}
