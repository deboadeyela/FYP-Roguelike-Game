using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows use of C# lists
using Random = UnityEngine.Random; 		//The Unity Engine random number generator.

public class BoardManager : MonoBehaviour
{
    // Serializable allows us to look at class's properties within Unity Editor
            [Serializable]
        public class Count
        {
        //properties Count will keep a track of. Min and max used as random range
            public int minValue;             
            public int maxValue;             
        
            public Count(int min, int max)
            {
                minValue = min;
                maxValue = max;
            }
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
