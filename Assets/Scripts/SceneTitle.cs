using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 *  CLASS: SceneTitle 
 *  Script to handle a variety of title screen things?
 */
public class SceneTitle : MonoBehaviour
{
    private string[] m_pathScenes =
    {
        "Scene_Title",
        "Scene_Gym"
    };

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Handles clicks in the scene select
    public void OnSceneSelectClick(int i)
    {
        if (i >= 0 && i < m_pathScenes.Length)
        {
            SceneManager.LoadScene(m_pathScenes[i], LoadSceneMode.Single);
        }
    }
}
