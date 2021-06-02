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
    public Color level; //Color used to show how powerful armour is

    //Attack/Defense Ratings for Player
    public int attackMod, defenseMod;
   
    //Sprite Reference
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer sRend;

    public void RandomArmourInit()
    {
        sRend = GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SelectArmour();
    }

    //Generate the random armour
    private void SelectArmour()
    {
        var itemCount = Enum.GetValues(typeof(armourType)).Length;
        type = (armourType)Random.Range(0, itemCount);

        //Dictates the base value for armour
        switch (type)
        {
            case armourType.glove:
                attackMod = Random.Range(1, 4);
                defenseMod = 0;
                spriteRenderer.sprite = glove;
                //           c++;
                //         Player.instance.gloveText.text = " Gloves: x" + c;
                  //       Player.instance.attackText.text = "Attack: " + attackMod;

                break;
            case armourType.boot:
                attackMod = 0;
                defenseMod = Random.Range(1, 4);
                spriteRenderer.sprite = boot;
                break;
        }

        //Changes color of item and adjust value of attack/defense Ratings for Player based off probability
        int randomLevel = Random.Range(0, 100);
        if (randomLevel >= 0 && randomLevel < 50)
        {
            spriteRenderer.color = level = Color.blue;
            attackMod += Random.Range(1, 4);
            defenseMod += Random.Range(1, 4);
        }
        else if (randomLevel >= 50 && randomLevel < 75)
        {
            spriteRenderer.color = level = Color.green;
            attackMod += Random.Range(4, 10);
            defenseMod += Random.Range(4, 10);
        }
        else if (randomLevel >= 75 && randomLevel < 90)
        {
            spriteRenderer.color = level = new Color(1.0F, 0.64F, 0.0F);
            attackMod += Random.Range(15, 25);
            defenseMod += Random.Range(15, 25);
        }
        else
        {
            spriteRenderer.color = level = Color.magenta;
            attackMod += Random.Range(40, 55);
            defenseMod += Random.Range(40, 55);
        }

        //attackText.text = "Attack: " + attackMod;
    }
}
