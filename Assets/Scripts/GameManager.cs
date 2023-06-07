using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    // Attributes
    public float levelStartDelay = 2f;
    public float turnDelay = 1f;
    public int health = 100;
    [HideInInspector] public bool playersTurn = true;
    public static GameManager instance = null;
    public BoardManager boardManager;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool settingUp;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Check if an instance of GameManager already exists
        if (instance == null)
        {
            // If not, set the instance to this GameManager
            instance = this;
        }
        else if (instance != this)
        {
            // If another instance already exists, destroy this GameManager
            Destroy(gameObject);
        }

        // Don't destroy this object when loading new scenes
        DontDestroyOnLoad(gameObject);

        // Initialize the list of enemies
        enemies = new List<Enemy>();

        // Get the BoardManager component
        boardManager = GetComponent<BoardManager>();

        // Call the InitGame method
        InitGame();
    }

    // Called when a new level is loaded
    private void OnLevelWasLoaded(int index)
    {
        // Increase the level count
        level++;

        // Call the InitGame method
        InitGame();
    }

    // Initialize the game
    void InitGame()
    {
        // Set the settingUp flag to true
        settingUp = true;

        // Find and activate the levelImage GameObject
        levelImage = GameObject.Find("LevelImage");
        levelImage.SetActive(true);

        // Find the levelText component and set its text
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Level: " + level;

        // Hide the levelImage after a delay
        Invoke("HideLevelImage", levelStartDelay);

        // Clear the list of enemies
        enemies.Clear();

        // Call the boardManager's SetupScene method to generate the level
        boardManager.SetupScene(level);
    }

    // Hide the levelImage GameObject
    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        settingUp = false;
    }

    // Coroutine for moving the enemies
    IEnumerator MoveEnemies()
    {
        // Set the enemiesMoving flag to true
        enemiesMoving = true;

        // Wait for the turn delay
        yield return new WaitForSeconds(turnDelay);

        // If there are no enemies, wait for another turn delay
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        // Loop through each enemy and move them
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        // Set the playersTurn flag to true and the enemiesMoving flag to false
        playersTurn = true;
        enemiesMoving = false;
    }

    // Called when the player's health reaches zero
    public void GameOver()
    {
        // Set the levelText to display the game over message
        levelText.text = "You died after " + level + " levels";

        // Show the levelImage GameObject
        levelImage.SetActive(true);

        // Disable this GameManager script
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If it's the player's turn, enemies are moving, or setting up the level, return
        if (playersTurn || enemiesMoving || settingUp)
        {
            return;
        }

        // Start the coroutine for moving the enemies
        StartCoroutine(MoveEnemies());
    }

    // Add an enemy to the list of enemies
    public void AddEnemyToList(Enemy enemy)
    {
        enemies.Add(enemy);
    }
}
