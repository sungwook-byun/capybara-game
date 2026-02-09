using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scFishMove : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 3f;

    private float startPosX;
    //public float startRotX;
    private bool movingRight;
    private float startSpeed;

    // Start is called before the first frame update
    void Start()
    {
        startSpeed = speed;
        speed = startSpeed * Random.Range(0.9f, 1.3f);

        startPosX = transform.position.x;
        if (transform.eulerAngles.y > 90f)
        {
            movingRight = false;
 
        }
        else
        {

            movingRight = true;

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(transform.localEulerAngles.z);

        if (movingRight)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
            if (transform.position.x >= distance)
            {
                movingRight = false;
                transform.rotation = Quaternion.Euler(90f, 0f, 180f);

                speed = startSpeed * Random.Range(0.9f, 1.3f);

            }
        }
        else
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
            if (transform.position.x <= -distance)
            {
                movingRight = true;
                transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                speed = startSpeed * Random.Range(0.9f, 1.3f);

            }
        }

    }
}
