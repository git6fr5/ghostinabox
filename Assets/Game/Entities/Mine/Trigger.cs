using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{

    public bool active = false;

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.GetComponent<Player>() != null && collider.GetComponent<Player>().Alive) {
            active = true;
        }
    }

}
