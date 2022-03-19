using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{

    public Rigidbody2D body;

    void Start() {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += Vector3.forward * 90f * Time.deltaTime;
        body.velocity = Vector3.up * 0.05f;
        body.velocity *= 0.95f;
    }
}
