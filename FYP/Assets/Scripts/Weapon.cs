using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{

    public bool inPlayerInventory = false; //Identifies whether weapon was picked up by the player

    private Player player; //Refernece to Player character
    public WeaponComponents[] weaponsComps; //Array of references to child weapon components
    private bool weaponUsed = false; //Boolean flag is uses to trigger the swing animation


    //Adds Weapon to player Inventory
    //Initialize connection between Player, Weapon, and WeaponComponents
    public void AcquireWeapon() {
        player = GetComponentInParent<Player>();
        weaponsComps = GetComponentsInChildren<WeaponComponents>();
    }

    //Updates player animation
    void Update()
    {
        if (inPlayerInventory) //Checks if item is in player inventory
        {
            transform.position = player.transform.position; //Weapon has same coordinates as player
            if (weaponUsed == true)
            {
                //Adjust arc animations
                float degreeY = 0, degreeZ = -90f, degreeZMax = 275f;
                Vector3 returnVecter = Vector3.zero;
                
                //Makes sword face right if player is 
                if (Player.isFacingRight)
                {
                    degreeY = 0;
                    returnVecter = Vector3.zero;
                }
                else if (!Player.isFacingRight)
                {
                    degreeY = 180;
                    returnVecter = new Vector3(0, 180, 0);
                }

                //Rotates Sword
                transform.rotation = Quaternion.Slerp(transform.rotation,
               Quaternion.Euler(0, degreeY, degreeZ), Time.deltaTime * 20f);

                //Disappers sword once angle has been exceeded 
                //Resets angle of sword
                if (transform.eulerAngles.z <= degreeZMax)
                {
                    transform.eulerAngles = returnVecter;
                    weaponUsed = false;
                    enableSpriteRender(false);
                }
            }
        }
    }

    //function that is called to start weapon swing animation
    public void useWeapon() {
         enableSpriteRender(true);
         weaponUsed = true;

    }

    //Disable the Sprite Renderers of the weapon components to keep them invisible till they are used
    public void enableSpriteRender(bool isEnabled) {
        foreach (WeaponComponents comp in weaponsComps)
        {
             comp.getSpriteRenderer().enabled = isEnabled;
             }
    }

    //Gets the image of item that is to be displayed on screen
    public Sprite getComponentImage(int index)
    {
        return weaponsComps[index].getSpriteRenderer().sprite;

    }
}