using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Loader script is responsible for instanitating GameManager class
public class Loader : MonoBehaviour
{
    public GameObject gameManager;
  //  public static Loader instance = this;

    //Awake function creates a new GameManager if one doesn't already exist 
    public void Awake()
    {
        
        if (GameManager.instance == null)

            //Instantiate gameManager prefab
            Instantiate(gameManager);
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
