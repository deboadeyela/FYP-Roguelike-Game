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

    void Awake()
    {
        //Store reference to the SpriteRenderer.
        sRenderer = GetComponent<SpriteRenderer>();
    }

    //Track times wall is hit
    public void WallDamaged(int broken)
    {
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
