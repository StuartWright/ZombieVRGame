using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHolster : MonoBehaviour
{
    public Transform Camera;
    private float RotationSpeed = 200;
    private float RotToMove, RotToMoveMinus;
    private bool Move;
    float target;
    void Start()
    {

    }
    float step;
    float oldrot;
    void Update()
    {
        transform.position = new Vector3(Camera.position.x, Camera.position.y - 0.65f, Camera.position.z);
        float rotationDifference = Mathf.Abs(Camera.eulerAngles.y);
        float FinalRotationSpeed = RotationSpeed;
        //print(rotationDifference); 
        if((int)rotationDifference % 45 == 0)
        {
            Move = true;
        }
        if(Move)
        {
            step = FinalRotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0), step);
            if((int)transform.eulerAngles.y == (int)Camera.transform.eulerAngles.y)
            {
                Move = false;
            }
        }
        
        /*
        transform.position = new Vector3(Camera.position.x, Camera.position.y / 2, Camera.position.z);

        float rotationDifference = Mathf.Abs(Camera.position.y - transform.eulerAngles.y);
        float FinalRotationSpeed = RotationSpeed;
        print(rotationDifference);
        if(rotationDifference > 60)
        {
            FinalRotationSpeed = RotationSpeed * 2;
        }
        else if(rotationDifference > 40 && rotationDifference < 60)
        {
            FinalRotationSpeed = RotationSpeed;
        }
        else if(rotationDifference < 40 && rotationDifference > 20)
        {
            FinalRotationSpeed = RotationSpeed / 2;
        }
        else if(rotationDifference < 20 && rotationDifference > 0)
        {
            FinalRotationSpeed = RotationSpeed / 4;
        }
        if ((int)transform.eulerAngles.y == (int)Camera.transform.eulerAngles.y)
        {
            step = 0;
        }
        else
            step = FinalRotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0), step);
        */
    }
}
