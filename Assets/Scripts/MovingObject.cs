using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class MovingObject : MonoBehaviour
{
    // Time it takes for the object to move from one tile to another
    public float moveTime = 0.1f;
    // Layer mask for blocking objects
    public LayerMask blockLayer;
    // Components
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Get the required components and calculate the inverse move time
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    // Move the object in the specified direction and check for collisions
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        // Disable the collider temporarily to prevent self-collision
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            // Move smoothly to the target position
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        // Object cannot move if there's a blocking object in the way
        return false;
    }

    // Smoothly move the object to the target position
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    // Attempt to move the object in the specified direction and handle collisions
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
        {
            // No collision, so the move is successful
            return;
        }

        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent != null)
        {
            // Call the appropriate method when the object can't move due to collision
            OnCantMove(hitComponent);
        }
    }

    // Method to handle what happens when the object can't move due to collision
    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}