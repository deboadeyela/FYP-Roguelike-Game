using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class WeaponComponents : MonoBehaviour
{

    public Sprite[] modules; //Array of different modules that make up a weapon.

    private Weapon parent; //Stores a reference to parent Weapon object 
    private SpriteRenderer spriteRenderer; //Store a reference to the Sprite Renderer component to turn it off and on

    //Set up our parent and Sprite Renderer references
    //Randomly select the module from the array of modules to render
    void Start()
    {
        parent = GetComponentInParent<Weapon>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = modules[Random.Range(0, modules.Length)];
    }

    //Continuously poll the parent weapon object for it’s angle in order 
    //to turn the weapon as the player turns
    void Update()
    {
        transform.eulerAngles = parent.transform.eulerAngles;
    }

    //Used to manipulate when the Sprite Renderer is enabled or disabled in the Weapon class
    public SpriteRenderer getSpriteRenderer()
    {
        return spriteRenderer;
    }
}
