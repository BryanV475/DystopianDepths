using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public int healthPerItem = 20;

    public float restartLevelDelay = 1f;

    public Text healthText;

    private Animator animator;
    
    private int health;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        health = GameManager.instance.health;
    
        healthText.text = "Health: " + health;

        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.health = health;
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        CheckHealth();
        GameManager.instance.playersTurn = false;
    }
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerAttack");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        } else if (collision.tag == "Collectable")
        {
            health += healthPerItem;
            healthText.text = "Health: " + health;
            collision.gameObject.SetActive(false);
        }
    }
    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    public void TakeDamage(int damage)
    {
        animator.SetTrigger("playerHit");
        health -= damage;
        healthText.text = "Health: " + health;
        CheckHealth();
    }
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
        if (!GameManager.instance.playersTurn)
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0) { 
           AttemptMove<Wall>(horizontal, vertical);
        }
    }
}
