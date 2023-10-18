using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine_Camera : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rb;
    private Animator animator;
    public GameObject subm;
    public int speed = 1;
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        animator = subm.GetComponent<Animator>();
    
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.eulerAngles[0] < 180){
            transform.Rotate( -10.0f * Time.deltaTime, 0f ,0f );
        }else if(this.transform.eulerAngles[0] > 180){
            transform.Rotate( 10.0f * Time.deltaTime, 0f ,0f );
        }

        if(this.transform.eulerAngles[2] < 180){
            transform.Rotate( 0f, 0f , -10.0f * Time.deltaTime );
        }else if(this.transform.eulerAngles[2] > 180){
            transform.Rotate( 0f, 0f , 10.0f * Time.deltaTime );
        }

        Vector3 vel_accel = Vector3.zero;
        Vector3 angvel_accel = Vector3.zero;

        // Wキー（前方移動）
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("isForward", true);
            vel_accel = vel_accel + transform.forward.normalized * speed * speed * -1 * 0.5f;
        }else{
            animator.SetBool("isForward", false);
        }
 
        // Sキー（後方移動）
        if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("isBackward", true);
            vel_accel = vel_accel + transform.forward * speed * speed  * 0.5f;
        }else{
            animator.SetBool("isBackward", false);
        }
 
        // Dキー（右移動）
        if (Input.GetKey(KeyCode.D))
        {
            angvel_accel = angvel_accel + transform.up * speed;
        }
 
        // Aキー（左移動）
        if (Input.GetKey(KeyCode.A))
        {
            angvel_accel = angvel_accel + transform.up * speed * -1;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            vel_accel = vel_accel + transform.up * speed * 2;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            vel_accel = vel_accel + transform.up * speed * -1 * 2;
        }

        rb.velocity = 0.999f * rb.velocity + 0.001f * vel_accel * Time.deltaTime;
        rb.angularVelocity = 0.999f * rb.angularVelocity + 0.001f * angvel_accel * Time.deltaTime;
    }
}
