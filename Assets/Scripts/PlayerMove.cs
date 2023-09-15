using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float speed;
    private Animator anim;
    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        speed = 1f;
        canMove = true;
        anim = transform.GetChild(1).transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (Input.GetKey(KeyCode.W))
            {
                if(transform.localRotation == Quaternion.Euler(0, 180, 0))
                {
                    anim.SetBool("MoveBackward", true);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                    anim.SetBool("MoveForward", true);
                }
                
                transform.Translate(Vector3.forward * Time.deltaTime);
            }
            else
            {
                anim.SetBool("MoveBackward", false);
                anim.SetBool("MoveForward", false);
            }
            if (Input.GetKey(KeyCode.S))
            {
                if(transform.localRotation == Quaternion.Euler(0, 0, 0))
                {
                    anim.SetBool("MoveBackward", true);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0, 180, 0);
                    anim.SetBool("MoveForward", true);
                }
                
                transform.Translate(-Vector3.forward * Time.deltaTime);
                
            }
            else
            {
                anim.SetBool("MoveBackward", false);
                anim.SetBool("MoveForward", false);
            }

            if (Input.GetKey(KeyCode.D))
            {

                if (transform.localRotation == Quaternion.Euler(0, 270, 0))
                {
                    anim.SetBool("MoveBackward", true);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0, 90, 0);
                    anim.SetBool("MoveForward", true);
                }

                transform.position += Vector3.right * speed * Time.deltaTime;
            }
            else
            {
                anim.SetBool("MoveBackward", false);
                anim.SetBool("MoveForward", false);
            }

            if (Input.GetKey(KeyCode.A))
            {
                if (transform.localRotation == Quaternion.Euler(0, 90, 0))
                {
                    anim.SetBool("MoveBackward", true);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0, 270, 0);
                    anim.SetBool("MoveForward", true);
                }
                transform.position += Vector3.left * speed * Time.deltaTime;
            }
            else
            {
                anim.SetBool("MoveBackward", false);
                anim.SetBool("MoveForward", false);
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {                
                anim.SetBool("Sprinting", true);
                speed = 1.3f;
            }
            else
            {
                speed = 1;
                anim.SetBool("Sprinting", false);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                anim.SetTrigger("Roll");
            }
        }
        
    }
}
