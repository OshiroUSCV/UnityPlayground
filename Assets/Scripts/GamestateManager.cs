using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamestateManager : MonoBehaviour
{

    // Static instance
    public static GamestateManager gamestate = null;

    private void Awake()
    {
        // Case: Static instance not set; make this the persistent object
        if (gamestate == null)
        {
            DontDestroyOnLoad(gameObject);
            gamestate = this;
        }
        // Case: Static instance already exists, destroy this duplicate
        else
        if (gamestate != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
