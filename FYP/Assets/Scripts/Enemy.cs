using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MovingObject
{
    public int playerDamage; //Variable for Player Damage

    private Animator animator; //Variable for Enemy animations
    private Transform target; //Target for enemy to hunt
    private bool skipMove; //used to slow down enemy
    public AudioClip attackSound1; //First of two audio clips to play when attacking the player.
    public AudioClip attackSound2;	//Second of two audio clips to play when attacking the player.
    //private SpriteRenderer spriteRenderer;
    // public Sprite enem;
    private int eHealth; //Enemy Health

    //
    protected override void Start()
    {
        eHealth = GameManager.instance.enemyHealth;
        GameManager.instance.AddEnemyToList(this); //Add enemies to list in order to track them

        animator = GetComponent<Animator>();

        /* spriteRenderer = GetComponent<SpriteRenderer>();
         spriteRenderer.sprite = enem;*/


        //Lets enemies to know position of player so that the enemies can chase the player
        target = GameObject.FindGameObjectWithTag("Player").transform; 

        base.Start();
    }

    public void DamageEnemy(int lack)
    {
        eHealth -= lack;
        if (eHealth <= 0)
        {
            gameObject.SetActive(false);
            GameManager.instance.enemiesKilled++;
        }
    }

    //Ends enemies turn without movement 
    protected override bool AttemptMove<T>(int xDir, int yDir) {
        if (skipMove)
        {
            skipMove = false;
            return false;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;

        return true;

    }

    //Check if enemy has hit wall
    protected override void OnCantMove<T>(T component) {
        Player hitPlayer = component as Player;

        hitPlayer.LoseHealth(playerDamage);

        animator.SetTrigger("enemyAttack");
        //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
        SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);
    }

    //Function to move enemies 
    //AI logic for Enemies
    public void MoveEnemy() {
        //Records direction of player
        int xDir = 0;
        int yDir = 0;

   
        if (GameManager.instance.enemiesSmarter) //Checks if flag has been set
        {
            // Calculate the distance from the player to enemy on the x and y axis
            int xHeading = (int)target.position.x - (int)transform.position.x;
            int yHeading = (int)target.position.y - (int)transform.position.y;
            bool moveOnX = false; //Dictates wheher to move horizontal(true) or vertical(false)

            //Check the size of the x and y distances
            if (Mathf.Abs(xHeading) >= Mathf.Abs(yHeading))
            {
                moveOnX = true;
            }
            for (int attempt = 0; attempt < 2; attempt++) //Allows for two attempts to find optimal path
            {
                //if-else block determines which direction the enemy will move in
                if (moveOnX == true && xHeading < 0)
                {
                    xDir = -1; yDir = 0;
                }
                else if (moveOnX == true && xHeading > 0)
                {
                    xDir = 1; yDir = 0;
                }
                else if (moveOnX == false && yHeading < 0)
                {
                    yDir = -1; xDir = 0;
                }
                else if (moveOnX == false && yHeading > 0)
                {
                    yDir = 1; xDir = 0;
                }
                //Checks if player is hit 
                Vector2 start = transform.position;
                Vector2 end = start + new Vector2(xDir, yDir);
                base.boxCollider.enabled = false;
                RaycastHit2D hit = Physics2D.Linecast(start, end,
               base.blockingLayer);
                base.boxCollider.enabled = true;

                //Checks what player has hit
                if (hit.transform != null)
                {
                    if (hit.transform.gameObject.tag == "Wall" ||
                   hit.transform.gameObject.tag == "Chest")
                    {
                        if (moveOnX == true)
                            moveOnX = false;
                        else
                            moveOnX = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }

        }
        else
        {
            //Base AI logic
            //Enemy moves in the x direction towards the player
            //If player and enemy are on the same x coordinate, then enemy moves towards the player in the y direction
            if (Mathf.Abs(target.position.x - transform.position.x) <
           float.Epsilon)
                yDir = target.position.y > transform.position.y ? 1 : -1;
            else
                xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        AttemptMove<Player>(xDir, yDir);





    }

    /*public SpriteRenderer getSpriteRenderer()
    {
        return spriteRenderer;
    }*/
    // Update is called once per frame
    //    void Update()
    //  {

    //}
}
