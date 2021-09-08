using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{

    public Sprite chestOpen; //Reference to sprite that shows that chest has been opened
    public Armour randomArmour; //Random Item that chest spawns
    public Weapon weapon; //Weapon object
    private SpriteRenderer sRend; //Renderer to change sprite

    //Set the spriteRenderer variable to chest’s Sprite Renderer component
    void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
    }

    //Switches to "open" sprite and spawns item
    public void Open()
    {
        sRend.sprite = chestOpen;
        GameObject InstantiaeObject;

        if (Random.Range(0, 2) == 1)
        {

            randomArmour.RandomArmourInit();
            InstantiaeObject = randomArmour.gameObject;
        }
        else
        {
            InstantiaeObject = weapon.gameObject;
        }
         GameObject instance = Instantiate (InstantiaeObject, new Vector3(transform.position.x, transform.position.y, 0f), Quaternion.identity) as GameObject;
         instance.transform.SetParent (transform.parent);

        //Set layer so that player can walk over chest
        gameObject.layer = 10; 
        sRend.sortingLayerName = "Items";
    }
}
