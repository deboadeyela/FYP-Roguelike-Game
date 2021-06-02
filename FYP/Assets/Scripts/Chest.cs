using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{

    public Sprite openSprite; //Reference to sprite that shows that chest has been opened
    public Armour randomArmour; //Random Item that chest spawns
    public Weapon weapon; //Weapon object
    private SpriteRenderer spriteRenderer; //Renderer to change sprite

    //Set the spriteRenderer variable to chest’s Sprite Renderer component
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //Switches to "open" sprite and spawns item
    public void Open()
    {
        spriteRenderer.sprite = openSprite;
        GameObject toInstantiate;

        if (Random.Range(0, 2) == 1)
        {

            randomArmour.RandomArmourInit();
            toInstantiate = randomArmour.gameObject;
        }
        else
        {
            toInstantiate = weapon.gameObject;
        }
         GameObject instance = Instantiate (toInstantiate, new Vector3(transform.position.x, transform.position.y, 0f), Quaternion.identity) as GameObject;
         instance.transform.SetParent (transform.parent);

        //Set layer so that player can walk over chest
        gameObject.layer = 10; 
        spriteRenderer.sortingLayerName = "Items";
    }
}
