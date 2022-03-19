using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : Controller {

    public Spritesheet spritesheet;

    /* --- Parameters --- */
    [SerializeField, ReadOnly] protected bool alive = true;
    public bool Alive => alive;

    [SerializeField] protected float floatSpeed = 1f;

    public Corpse corpse;
    public BoxCollider2D ghostbox;

    /* --- Overridden Methods --- */
    // Runs the thinking logic.
    protected override void Think() {
        base.Think(); // Runs the base think.

        if (GameRules.MainPlayer != null) {
            alive = GameRules.MainPlayer.Alive;
            GetComponent<BoxCollider2D>().isTrigger = !alive;
            box.SetActive(!alive);
        }
    }

    // Moves this controller based on it's input.
    protected override void Move(float deltaTime) {
        if (alive) {
            
        }
        else {
            Vector2 direction = (GameRules.MainPlayer.transform.position - transform.position);
            body.velocity += floatSpeed * direction.normalized * deltaTime;
            if (body.velocity.sqrMagnitude > moveSpeed * moveSpeed / 1.5f / 1.5f) {
                body.velocity = moveSpeed / 1.5f * body.velocity.normalized;
            }
            if (direction == Vector2.zero) {
                body.velocity *= 0.925f;
                if (body.velocity.sqrMagnitude <= GameRules.MovementPrecision * GameRules.MovementPrecision) {
                    body.velocity = Vector2.zero;
                }
            }
            directionFlag = direction.x < 0 ? Controller.Direction.Left : Controller.Direction.Right;
        }

    }

    void OnTriggerEnter2D(Collider2D collider) {

        if (collider.GetComponent<Player>() != null) {
            if (!collider.GetComponent<Player>().Alive && !GameRules.Resetting) {
                print("touching dead player");
                // GameRules.SoulBeenEaten = true;
                collider.GetComponent<Player>().Respawn();
            }
        }

    }

    public GameObject box;

}
