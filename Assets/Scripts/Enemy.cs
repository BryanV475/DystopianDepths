using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class Enemy : MovingObject
{
    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Call the base class Start method
        base.Start();

        // Add the enemy to the GameManager's enemy list
        GameManager.instance.AddEnemyToList(this);

        // Get the Animator component attached to the enemy object
        animator = GetComponent<Animator>();

        // Find the GameObject with the "Player" tag and get its Transform component
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Attempt to move the enemy
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // If skipMove is true, don't move and reset skipMove to false
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        // Call the base class AttemptMove method
        base.AttemptMove<T>(xDir, yDir);

        // Set skipMove to true to skip the next move
        skipMove = true;
    }

    // Move the enemy towards the player
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // Check if the player is on the same vertical line
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            // If player is above the enemy, move up; otherwise, move down
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            // If player is to the right of the enemy, move right; otherwise, move left
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        // Attempt to move the enemy towards the player
        AttemptMove<Player>(xDir, yDir);
    }

    // Called when the enemy cannot move because there is a blocking object in the way
    protected override void OnCantMove<T>(T component)
    {
        // Cast the component to Player type
        Player hitPlayer = component as Player;

        // Trigger the "enemyAttack" animation in the Animator
        animator.SetTrigger("enemyAttack");

        // Deal damage to the player
        hitPlayer.TakeDamage(playerDamage);
    }
}