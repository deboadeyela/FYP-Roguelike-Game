using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{

    public Sprite openSprite;
    public Armour randomArmour;
    public Weapon weapon;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

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

        gameObject.layer = 10;
        spriteRenderer.sortingLayerName = "Items";
    }
}
