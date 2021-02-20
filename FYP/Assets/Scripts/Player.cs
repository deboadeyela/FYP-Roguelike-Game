using System.Collections;
using UnityEngine;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public float restartLevelDelay = 1f;        //Delay time in seconds to restart level
    public int foodPoints = 10;                 //Points gained from getting food
    public int drinkPoints = 20;                //Points gained from getting drink
    public int wallDamage = 1;                  //Damage that is done to wall by player hit
    public Text healthText;						//UI Text display of player health
    private Animator animator;					//Used to store a reference to the Player's animator component.
    private int health;                         //Player health

    // Start is called before the first frame update
    void Start()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();

        //Player's Health
        health = GameManager.instance.playerHealth;
        healthText.text = "Health: " + health;
        
        base.Start();   //The Start function of the MovingObject base class is called.
    }

    // Update is called once per frame
    void Update()
    {
        //If it's not the player's turn, exit the function.
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;     //Used to store the horizontal move direction.
        int vertical = 0;       //Used to store the vertical move direction.
                                //Check if we have a non-zero value for horizontal or vertical
        if (horizontal != 0 || vertical != 0)
        {
            //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
            //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir) {
        //Every time player moves, subtract from food points total.
        health--;

        //Update food text display to reflect current score.
        healthText.text = "Health: " + health;

        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        base.AttemptMove<T>(xDir, yDir);

        //Hit allows us to reference the result of the Linecast done in Move.
        RaycastHit2D hit;

        //Since the player has moved and lost food points, check if the game has ended.
        CheckIfGameOver();

        //Set the playersTurn boolean of GameManager to false now that players turn is over.
        GameManager.instance.playersTurn = false;
    }
    protected override void OnCantMove<T>(T component) {
        //Set hitWall to equal the component passed in as a parameter.
        Wall hitWall = component as Wall;

        //Call the DamageWall function of the Wall we are hitting.
        hitWall.WallDamaged(wallDamage);

        //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
        animator.SetTrigger("playerChop");
    }
    public void LoseHealth(int loss) {
        //Set the trigger for the player animator to transition to the playerHit animation.
        animator.SetTrigger("playerHurt");

        //Subtract lost food points from the players total.
        health -= loss;

        //Update the food display with the new total.
        healthText.text = "-" + loss + " Health: " + health;

        //Check to see if game has ended.
        CheckIfGameOver();
    }
    private void CheckIfGameOver() {
        //Check if food point total is less than or equal to zero.
        if (health <= 0) {
            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver();
        }
    }
}
