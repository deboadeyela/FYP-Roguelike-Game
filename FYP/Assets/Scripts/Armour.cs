using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;

//  Enumeration that dictates the armour type
public enum armourType
{
    glove, boot
}

public class Armour : MonoBehaviour
{
    //Item references
    public Sprite glove;
    public Sprite boot;


    public armourType type; //Reference to armour type
    public Color strength; //Color used to show how powerful armour is

    //Attack/Defense Ratings for Player
    public int attack, defense;
   
    //Sprite Reference
    private SpriteRenderer sRend;
    
    public void RandomArmourInit()
    {
        sRend = GetComponent<SpriteRenderer>();
        SelectArmour();
    }

    //Generate the random armour
    private void SelectArmour()
    {
        var armourCount = Enum.GetValues(typeof(armourType)).Length;
        type = (armourType)Random.Range(0, armourCount);

        //Dictates the base value for armour
        switch (type)
        {
            case armourType.glove:
                attack = Random.Range(1, 4);
                defense = 0;
                sRend.sprite = glove;
                //           c++;
                //         Player.instance.gloveText.text = " Gloves: x" + c;
                  //       Player.instance.attackText.text = "Attack: " + attackMod;

                break;
            case armourType.boot:
                attack = 0;
                defense = Random.Range(1, 4);
                sRend.sprite = boot;
                break;
        }

        //Changes color of item and adjust value of attack/defense Ratings for Player based off probability
        int randomStrength = Random.Range(0, 100);
        if (randomStrength >= 0 && randomStrength < 50)
        {
            sRend.color = strength = Color.blue;
            attack += Random.Range(1, 4);
            defense += Random.Range(1, 4);
        }
        else if (randomStrength >= 50 && randomStrength < 75)
        {
            sRend.color = strength = Color.green;
            attack += Random.Range(4, 8);
            defense += Random.Range(4, 8);
        }
        else if (randomStrength >= 75 && randomStrength < 90)
        {
            sRend.color = strength = new Color(1.0F, 0.64F, 0.0F);
            attack += Random.Range(8, 12);
            defense += Random.Range(8, 12);
        }
        else
        {
            sRend.color = strength = Color.magenta;
            attack += Random.Range(12, 16);
            defense += Random.Range(12, 16);
        }

        //attackText.text = "Attack: " + attackMod;
    }
}
