using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
public class BoardManager : MonoBehaviour
{
    // Serializable class for defining minimum and maximum counts
    [Serializable]
    public class Count
    {
        public int minimum; // Minimum count value
        public int maximum; // Maximum count value

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // Size of the board
    public int columns = 8; // Number of columns on the board
    public int rows = 8; // Number of rows on the board

    // Count of how many prefabs to generate
    public Count wallCount = new Count(5, 9); // Count range for walls
    public Count itemsCount = new Count(1, 5); // Count range for items

    // Prefabs store
    public GameObject exit; // Exit prefab
    public GameObject[] floorTiles; // Array of floor tile prefabs
    public GameObject[] wallTiles; // Array of wall tile prefabs
    public GameObject outerWall; // Outer wall prefab
    public GameObject[] enemyTiles; // Array of enemy prefabs
    public GameObject[] itemTiles; // Array of item prefabs

    private Transform boardHolder; // Parent object for all board tiles
    private List<Vector3> gridPositions = new List<Vector3>(); // List of available grid positions

    // Initialize the list of grid positions
    void InitializeList()
    {
        gridPositions.Clear();
        for (int x = 1; x < columns; x++) // Iterate over columns
        {
            for (int y = 1; y < rows; y++) // Iterate over rows
            {
                gridPositions.Add(new Vector3(x, y, 0f)); // Add grid position to the list
            }
        }
    }

    // Setup the game board
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform; // Create a new parent object for the board
        for (int x = -1; x < columns + 1; x++) // Iterate over columns with outer walls
        {
            for (int y = -1; y < rows + 1; y++) // Iterate over rows with outer walls
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)]; // Choose a random floor tile

                if (x == -1 || x == columns || y == -1 || y == rows) // Check if the position is on the edge
                {
                    toInstantiate = outerWall; // Use outer wall prefab instead
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; // Instantiate the chosen tile
                instance.transform.SetParent(boardHolder); // Set the parent object
            }
        }
    }

    // Get a random position from the grid positions list and remove it
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count); // Choose a random index from the grid positions list
        Vector3 randomPosition = gridPositions[randomIndex]; // Get the corresponding random position
        gridPositions.RemoveAt(randomIndex); // Remove the position from the list
        return randomPosition; // Return the random position
    }

    // Randomly layout objects from the tile array
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1); // Choose a random count of objects to instantiate
        for (int i = 0; i < objectCount; i++) // Iterate over the count
        {
            Vector3 randomPosition = RandomPosition(); // Get a random position
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)]; // Choose a random tile from the tile array
            Instantiate(tileChoice, randomPosition, Quaternion.identity); // Instantiate the chosen tile at the random position
        }
    }

    // Setup the scene for the given level
    public void SetupScene(int level)
    {
        BoardSetup(); // Setup the game board
        InitializeList(); // Initialize the grid positions list
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum); // Layout random wall tiles
        LayoutObjectAtRandom(itemTiles, itemsCount.minimum, itemsCount.maximum); // Layout random item tiles
        int enemyCount = (int)Mathf.Log(level, 2f); // Calculate the number of enemies based on the level
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount); // Layout random enemy tiles
        Instantiate(exit, new Vector3(columns - 1f, rows - 1f), Quaternion.identity); // Instantiate the exit tile
    }
}
