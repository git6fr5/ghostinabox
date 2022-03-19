using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Block : MonoBehaviour {

    [HideInInspector] public BoxCollider2D box;
    [HideInInspector] public Rigidbody2D body;

    #region Unity
    void Start() {
        box = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        body.angularDrag = 0.05f;
        Freeze();
    }

    void Update() {
        if (!GameRules.MainPlayer.Alive) {
            Release();
        }
        else {
            Freeze();
        }
        body.velocity *= 0.99f;
    }

    protected virtual void Release() {
        body.constraints = RigidbodyConstraints2D.None;
        body.gravityScale = 0f;
    }

    protected virtual void Freeze() {
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0f;
    }
    #endregion
}
