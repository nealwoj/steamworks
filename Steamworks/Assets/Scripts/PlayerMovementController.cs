using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovementController : NetworkBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isOwned)
        {
            if (Input.GetKey(KeyCode.D))
                transform.position += new Vector3(Time.deltaTime * speed, 0f,0f);
            else if (Input.GetKey(KeyCode.A))
                transform.position += new Vector3(Time.deltaTime * -speed, 0f, 0f);
            else if (Input.GetKey(KeyCode.W))
                transform.position += new Vector3(0f, 0f, Time.deltaTime * speed);
            else if (Input.GetKey(KeyCode.S))
                transform.position += new Vector3(0f, 0f, Time.deltaTime * -speed);
        }
    }
}
