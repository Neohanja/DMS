using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMovement : MonoBehaviour
{
    public static PMovement Player;
    
    [SerializeField] protected Transform cam;
    public float moveSpeed;
    public TaskManager playerTasks;


    private void Awake()
    {
        if(Player != null && Player != this)
        {
            Destroy(gameObject);
        }

        Player = this;
        cam = GetComponentInChildren<Camera>().transform;
        playerTasks = gameObject.AddComponent<TaskManager>();
    }

    public Camera GetPlayerCam()
    {
        return cam.GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float sideMove = Input.GetAxis("Horizontal");
        float forwardMove = Input.GetAxis("Vertical");
        float swivel = 0f;

        if(Input.GetMouseButton(1))
        {
            swivel = Input.GetAxis("Mouse X");
        }
        else if(Input.GetKey(KeyCode.Comma))
        {
            swivel = -0.5f;
        }
        else if(Input.GetKey(KeyCode.Period))
        {
            swivel = 0.5f;
        }

        transform.Rotate(new Vector3(0, swivel, 0));
        transform.position += ((transform.forward * forwardMove) + (transform.right * sideMove)) * moveSpeed * Time.deltaTime;
    }
}
