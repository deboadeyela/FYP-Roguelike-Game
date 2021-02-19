using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    void Awake() {
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization() {
    }
   // static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) { }

    void InitGame() { }
    public void AddEnemyToList(Enemy script) { }
    public void GameOver() { }
   // IEnumerator MoveEnemies() { }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
