using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Wall : MonoBehaviour
{
    // Attributes
    public Sprite dmgSprite;
    public int hp = 4;

    private SpriteRenderer spriteRenderer;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Get the SpriteRenderer component attached to the wall object
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Damage the wall by the specified amount
    public void DamageWall(int loss)
    {
        // Set the sprite to the damaged sprite
        spriteRenderer.sprite = dmgSprite;

        // Decrease the wall's hit points
        hp -= loss;

        // If the wall's hit points reach zero or below, deactivate the wall object
        if (hp <= 0)
            gameObject.SetActive(false);
    }
}