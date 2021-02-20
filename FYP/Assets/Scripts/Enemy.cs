using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    protected override void AttemptMove<T>(int xDir, int yDir) {
        //Call the AttemptMove function from MovingObject.
        base.AttemptMove<T>(xDir, yDir);

        
        //return true;
    }

    //Check if enemy has hit wall
    protected override void OnCantMove<T>(T component) {
    }

    public void MoveEnemy() { }
// Start is called before the first frame update
//void Start()
//  {

    // }

    // Update is called once per frame
    void Update()
    {
        
    }
}
