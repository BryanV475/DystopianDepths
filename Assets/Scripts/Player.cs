using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Player : MovingObject
{
    // Attributes
    public int wallDamage = 1;
    public int healthPerItem = 20;
    public float restartLevelDelay = 1f;
    public Text healthText;

    private Animator animator;
    private int health;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Get the Animator component attached to the player object
        animator = GetComponent<Animator>();

        // Get the initial health from the GameManager
        health = GameManager.instance.health;

        // Update the health text
        healthText.text = "Health: " + health;

        // Call the base class Start method
        base.Start();
    }

    // Called when the script is being disabled
    private void OnDisable()
    {
        // Store the current health in the GameManager
        GameManager.instance.health = health;
    }

    // Attempt to move the player in the specified direction
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // Call the base class AttemptMove method
        base.AttemptMove<T>(xDir, yDir);

        // Check the player's health
        CheckHealth();

        // Set playersTurn to false in the GameManager
        GameManager.instance.playersTurn = false;
    }

    // Called when the player cannot move into a position occupied by a blocking object
    protected override void OnCantMove<T>(T component)
    {
        // Cast the component to Wall type
        Wall hitWall = component as Wall;

        // Damage the wall
        hitWall.DamageWall(wallDamage);

        // Trigger the "playerAttack" animation in the Animator
        animator.SetTrigger("playerAttack");
    }

    // Called when the player collides with a trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Exit")
        {
            // Invoke the Restart method after a delay
            Invoke("Restart", restartLevelDelay);

            // Disable the Player script
            enabled = false;
        }
        else if (collision.tag == "Collectable")
        {
            // Increase health by healthPerItem
            health += healthPerItem;

            // Update the health text
            healthText.text = "Health: " + health;

            // Deactivate the collectible object
            collision.gameObject.SetActive(false);
        }
    }

    // Restart the current level
    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    // Inflict damage to the player and check health status
    public void TakeDamage(int damage)
    {
        // Trigger the "playerHit" animation in the Animator
        animator.SetTrigger("playerHit");

        // Decrease health by damage
        health -= damage;

        // Update the health text
        healthText.text = "Health: " + health;

        // Check the player's health
        CheckHealth();
    }

    // Check if the player's health is depleted and trigger game over if true
    private void CheckHealth()
    {
        if (health <= 0)
        {
            GameManager.instance.GameOver();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If it's not the player's turn, return
        if (!GameManager.instance.playersTurn)
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        // Get the input from the horizontal and vertical axes
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        // Restrict diagonal movement
        if (horizontal != 0)
        {
            vertical = 0;
        }

        // If there is movement input, attempt to move the player
        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }
}