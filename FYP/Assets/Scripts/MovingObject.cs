using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour
{

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

   // protected bool Move(int xDir, int yDir, out RaycastHit2D hit) {
     //   hit = Physics2D.Linecast(start, end, blockingLayer);
  //  }

  //  protected IEnumerator SmoothMovement(Vector3 end) {
        
 //   }

    protected virtual bool AttemptMove<T>(int xDir, int yDir) where T : Component
    {
        return true;
    }

    protected abstract void OnCantMove<T>(T component)
            where T : Component;
    // Update is called once per frame
    void Update()
    {
        
    }
}
