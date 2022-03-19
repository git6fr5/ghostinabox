using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : Block {

    public bool fall;

    #region Unity
    protected override void Release() {
        body.constraints = RigidbodyConstraints2D.None;
        body.gravityScale = 0f;
    }

    protected override void Freeze() {
        if (!fall) {
            body.constraints = RigidbodyConstraints2D.FreezeAll;
            body.gravityScale = 0f;
        }
        else {
            body.constraints = RigidbodyConstraints2D.FreezePositionX;
            body.gravityScale = 0.75f;
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.GetComponent<Player>() != null) {
            if (collider.GetComponent<Player>().Alive) {
                StartCoroutine(IEFall(collider));

            }
        }
    }

    private IEnumerator IEFall(Collider2D collider) {
        yield return new WaitForSeconds(0.5f);
        fall = true;
        Freeze();
    }
    #endregion


}
