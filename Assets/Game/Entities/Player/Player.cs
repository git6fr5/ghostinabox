/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Player : Controller {

    public Spritesheet spritesheet;

    [Space(2), Header("Jumping")]
    /* --- Parameters --- */
    [SerializeField, ReadOnly] private KeyCode jumpKey = KeyCode.Space; // The key used to jump.
    [SerializeField, ReadOnly] public bool canJump;

    [SerializeField, ReadOnly] protected bool alive = true;
    public bool Alive => alive;

    [SerializeField, Range(1f, 100f)] protected float respawnTimer = 1f;
    [SerializeField, ReadOnly] protected float respawnTicks = 0f;
    public float RespawnRatio => respawnTicks / respawnTimer;

    [SerializeField] protected float floatSpeed = 1f;

    public Corpse corpse;
    public BoxCollider2D ghostbox;

    /* --- Overridden Methods --- */
    // Runs the thinking logic.
    protected override void Think() {
        base.Think(); // Runs the base think.

        // Get the movement.
        moveDirection = Input.GetAxisRaw("Horizontal");

        // Check if the action is currently being performed.
        CheckJump();
        GetJump();

        CheckDeath();
    }

    // Moves this controller based on it's input.
    protected override void Move(float deltaTime) {
        if (alive) {
            base.Move(deltaTime);
            if (body.velocity.y < 0f && airborneFlag == Airborne.Grounded) {
                body.velocity = new Vector2(body.velocity.x, 0f);
            }

            if (body.velocity.y < -20f) {
                body.velocity = new Vector2(body.velocity.x, -20f);
            }
        }
        else {
            Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            body.velocity += floatSpeed * direction.normalized * deltaTime;
            if (body.velocity.sqrMagnitude > moveSpeed * moveSpeed / 1.5f / 1.5f) {
                body.velocity = moveSpeed / 1.5f  * body.velocity.normalized;
            }
            if (direction == Vector2.zero) {
                body.velocity *= 0.925f;
                if (body.velocity.sqrMagnitude <= GameRules.MovementPrecision * GameRules.MovementPrecision) {
                    body.velocity = Vector2.zero;
                }
            }
        }

    }

    /* --- Methods --- */
    private void GetJump() {
        if (Input.GetKeyDown(jumpKey) && canJump) {
            Jump();
        }
    }

    private void CheckJump() {
        // Check that we're on the ground.
        if (airborneFlag == Airborne.Grounded) {
            canJump = true;
        }
        else {
            canJump = false;
        }
    }

    private void CheckDeath() {
        corpse.transform.parent = null;

        if (!alive) {
            respawnTicks += Time.deltaTime;
            if (respawnTicks > respawnTimer) {
                think = false;
                StartCoroutine(IERespawn());
            }
        }
        if (alive) {
            respawnTicks = 0f;
        }
    }

    public void Kill(Vector3 origin, float force = 5f) {
        alive = false;
        GameRules.GravityScale = 0f;
        body.velocity = (transform.position - origin).normalized * force;

        corpse.transform.position = transform.position;
        transform.position += 1.25f * (Vector3)body.velocity.normalized;

        corpse.gameObject.SetActive(true);
        // corpse.body.velocity = -body.velocity.normalized * 0.5f;
        ghostbox.enabled = true;
    }

    private IEnumerator IERespawn() {
        yield return new WaitForSeconds(0.5f);
        Respawn();
    }

    public void Respawn() {
        alive = true;
        think = true;
        GameRules.GravityScale = 1f;
        transform.position = corpse.transform.position;
        corpse.gameObject.SetActive(false);

        ghostbox.enabled = false;
    }

    /* --- Debug --- */
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawWireSphere(transform.position - Vector3.up * height, GameRules.GroundCheckRadius);
    }

}
