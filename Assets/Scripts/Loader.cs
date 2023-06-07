using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    // Reference to the GameManager prefab
    public GameObject gameManager;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Check if an instance of the GameManager already exists
        if (GameManager.instance == null)
        {
            // Instantiate the GameManager prefab
            Instantiate(gameManager);
        }
    }
}
