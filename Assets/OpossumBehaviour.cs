using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Debug = UnityEngine.Debug;


public enum RampDirection
{
    NONE,
    UP,
    DOWN
}

public class OpossumBehaviour : MonoBehaviour
{
    public float runSpeed;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rigidbody2D;
    public Transform lookAheadPoint;
    public Transform lookInFrontPoint;
    public bool isGroundedAhead;
    public bool isGrounded;
    public LayerMask layerMask;
    public LayerMask collisionWallLayer;
    //public float direction;
    public Vector2 direction;
    public bool onRamp;
    public RampDirection rampD;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        isGroundedAhead = false;
        direction = Vector2.left;
        rampD = RampDirection.NONE;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LookInFront();
        LookAhead();
        Move();
    }

    private void LookInFront()
    {
        var wallHit = Physics2D.Linecast(transform.position, lookInFrontPoint.position, collisionWallLayer);
        if (wallHit)
        {
            if (!wallHit.collider.CompareTag("Ramps"))
            {
                if (!onRamp && transform.rotation.z == 0.0f)
                {
                    FlipX();
                }
                rampD = RampDirection.DOWN;
            }
            else
            {
                rampD = RampDirection.UP;
            }
        }
        
        Debug.DrawLine(transform.position, lookInFrontPoint.position, Color.red);
    }

    private void LookAhead()
    {
        var groundHit = Physics2D.Linecast(transform.position, lookAheadPoint.position, layerMask);
        if (groundHit)
        {
            if (groundHit.collider.CompareTag("Ramps"))
            {
                onRamp = true;
            }
            if (groundHit.collider.CompareTag("Platforms"))
            {
                onRamp = false;
            }


            isGroundedAhead = true;
        }
        else
        {
            isGroundedAhead = false;
        }


        //isGroundedAhead = hit.collider.CompareTag("Platforms") ? true : false;
        Debug.DrawLine(transform.position, lookAheadPoint.position, Color.green);
    }

    private void Move()
    {
        if (isGroundedAhead)
        {
            rigidbody2D.AddForce(Vector2.left * runSpeed * Time.deltaTime * transform.localScale.x);
            
            if (onRamp)
            {

                if (rampD == RampDirection.UP)
                {
                    rigidbody2D.AddForce(Vector2.up * runSpeed * 0.5f * Time.deltaTime);
                }
                else
                {
                    rigidbody2D.AddForce(Vector2.up * runSpeed * 0.25f * Time.deltaTime);
                }
                StartCoroutine(Rotate());
            }
            else
            {
                StartCoroutine(Normalize());
            }

            rigidbody2D.velocity *= 0.90f;
        }
        else
        {
            FlipX();
            //direction *= -1.0f;
        }

        //rigidbody2D.velocity *= 0.93f;
    }

    IEnumerator Rotate()
    {
        yield return new WaitForSeconds(0.05f);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, -26.0f);
    }

    IEnumerator Normalize()
    {
        yield return new WaitForSeconds(0.05f);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }

    private void FlipX()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
    }
}
