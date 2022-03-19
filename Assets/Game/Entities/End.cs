using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D collider) {

        if (collider.GetComponent<Player>() != null && collider.GetComponent<Player>().Alive) {
            print("End");
            GameRules.NextLevel();
        }

    }

}
