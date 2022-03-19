using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : Block {


    #region Unity
    protected override void Release() {
        body.constraints = RigidbodyConstraints2D.None;
        body.gravityScale = 0f;
    }

    protected override void Freeze() {
        body.constraints = RigidbodyConstraints2D.None;
        body.gravityScale = 1f;
    }
    #endregion
}
