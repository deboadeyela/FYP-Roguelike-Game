﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class Wall : MonoBehaviour
{
    public Sprite damaged; //Reference of sprite that shows wall has taken damage
    {
        //Store reference to the SpriteRenderer.
        sRenderer = GetComponent<SpriteRenderer>();
    }
    {
        sRenderer.sprite = damaged;

        //Subtract loss from hit point total.
        wallHealth = wallHealth-broken;

        //If hit points are less than or equal to zero:
        if (wallHealth <= 0)
            //Disable the gameObject.
            gameObject.SetActive(false);
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