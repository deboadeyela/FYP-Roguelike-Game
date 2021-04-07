using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Wall : MonoBehaviour
{
    public Sprite damaged; //Reference of sprite that shows wall has taken damage
    public int wallHealth = 1; //Hits required to break wall
    private SpriteRenderer sRenderer;
    public GameObject[] foodTiles; //Array used to hold food tiles
    public GameObject[] floorT;
    public AudioClip chopSound1;                //1 of 2 audio clips that play when the wall is attacked by the player.
    public AudioClip chopSound2;				//2 of 2 audio clips that play when the wall is attacked by the player.

    void Awake()
    {
        //Store reference to the SpriteRenderer.
        sRenderer = GetComponent<SpriteRenderer>();
    }

    //Track times wall is hit
    public void WallDamaged(int broken)
    {
        //Call the RandomizeSfx function of SoundManager to play one of two chop sounds.
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);

        sRenderer.sprite = damaged;

        //Subtract loss from hit point total.
        wallHealth = wallHealth-broken;

        //If hit points are less than or equal to zero:
        if (wallHealth <= 0)
        {
            GameObject toSet = floorT[Random.Range(0, floorT.Length)];
            GameObject instant = Instantiate(toSet, new Vector3(transform.position.x, transform.position.y, 0f), Quaternion.identity) as GameObject;
            //Food tile is generated in place of food tile based on random probability value
            if (Random.Range(0, 5) == 1)
            {
             
                GameObject toInstantiate = foodTiles[Random.Range(0,foodTiles.Length)];
                GameObject instance = Instantiate(toInstantiate, new Vector3(transform.position.x, transform.position.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(transform.parent);
            }
            //Disable the gameObject.
            gameObject.SetActive(false);
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
