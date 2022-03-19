/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Pole : MonoBehaviour {


    /* --- Components --- */
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D hitbox;
    private Rigidbody2D body;
    public Rope rope;

    /* --- Properties --- */
    public Pole targetPoint;
    public Vector2 tension;
    [Range(0.05f, 1f)] public float resistance; // either make this static - or make it so that if the interactable component is moving, then this has 0 or less value.

    public bool released = true;

    /* --- Unity --- */
    private void Start() {
        // Cache these components
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitbox = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();

        // Set up the components
        CheckRope();

        Freeze();
    }

    private void CheckRope() {
        if (targetPoint != null) {
            if (rope != null) {
                rope.gameObject.SetActive(true);
                rope.startpoint = transform;
                rope.endpoint = targetPoint.transform;
                rope.ropeLength = 7f;
                rope.ropeWidth = 0.1f;
            }
        }
        else if (rope != null) {
            rope.gameObject.SetActive(false);
        }
    }

    private void Freeze() {
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0f;
        body.angularDrag = 5f;
        released = false;
    }

    private void Release() {
        body.constraints = RigidbodyConstraints2D.None;
        body.gravityScale = 0f;
        body.angularDrag = 5f;
        released = true;
    }

    private void Update() {
        if (released) {
            Move();
            Connect();
        }

        if (GameRules.MainPlayer.Alive) {
            Freeze();
            // rope.gameObject.SetActive(false);
            released = false;
        }
        else {
            Release();
            //if (rope != null && targetPoint != null) {
            //    rope.gameObject.SetActive(true);
            //}
            released = true;
        }
    }

    private void Move() {
        if (targetPoint != null) {
            TargetBounds();
        }
        body.velocity *= resistance;
    }

    /* --- Methods --- */
    private void TargetBounds() {
        float distance = (targetPoint.transform.position - transform.position).magnitude;
        if (distance > rope.ropeLength) {
            tension = (Vector2)(targetPoint.transform.position - transform.position).normalized * (distance / rope.ropeLength);
            body.velocity = tension;
        }
    }

    private void Connect() {
        if (targetPoint == null) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, rope.ropeLength);
            for (int i = 0; i < colliders.Length; i++) {
                if (colliders[i].GetComponent<Pole>() && colliders[i].GetComponent<Pole>() != this) {
                    targetPoint = colliders[i].GetComponent<Pole>();
                }
            }
        }
        CheckRope();
    }

}
