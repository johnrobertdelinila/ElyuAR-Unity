using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement1 : MonoBehaviour
{
    public float speed;

    private Vector3 movement;

    public Transform Player;

    private Rigidbody rb;

    private Animator anim;

    public GameObject bullet;
    public Transform bulletPos;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bulletPos.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    public void OnMovement(InputValue val)
    {
        movement = val.Get<Vector2>();
        movement.z = movement.y;
        movement.y = 0;
    }

    public void OnShoot()
    {
        Debug.Log("Shoot");
        //rb.velocity = new Vector3(movement.x, 6f, movement.z);
        Instantiate(bullet, bulletPos);
        //anim.SetTrigger("Jump");
    }
}
