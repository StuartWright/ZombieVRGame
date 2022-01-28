using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuRecoil : MonoBehaviour
{
    public float OneHandedRecoilAmount, TwoHandedRecoilAmount, OneHandedPushBackAmount, TwoHandedPushBackAmount;
    public float PullDownForce, PullBackForce;
    public bool CustomPivotPoint;
    private Quaternion StartRot;
    private Vector3 StartPos;
    private bool PullUp, PullBack;
    private Quaternion DesiredRotation;
    private Vector3 DesiredPosition;
    private void Start()
    {
        StartRot = transform.localRotation;
        StartPos = transform.localPosition;
    }
    public void AddRecoil(float Amount)
    {
        PullUp = true;
        //transform.localRotation = Quaternion.Euler(transform.localRotation.x - Amount, transform.localRotation.y, transform.localRotation.z);
        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x - Amount, transform.localEulerAngles.y, transform.localEulerAngles.z);
        DesiredRotation = Quaternion.Euler(transform.localRotation.x - Amount, transform.localRotation.y, transform.localRotation.z);
    }
    public void AddPushBack(float Amount)
    {
        PullBack = true;
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - Amount * Time.deltaTime);
        //transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - Amount), 2 * Time.deltaTime);
        DesiredPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - Amount);
    }
    void Update()
    {
        if(!CustomPivotPoint)
        {
            if(PullUp)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, DesiredRotation, PullDownForce * Time.deltaTime * 20);
                if (transform.localRotation.x <= DesiredRotation.x)
                {
                    PullUp = false;
                }                   
            }
            else
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(0, 0, 0), PullDownForce * Time.deltaTime * 20);

            if (PullBack)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, DesiredPosition, PullBackForce * Time.deltaTime);
                if (transform.localPosition.z >= DesiredPosition.z)
                {
                    PullBack = false;
                }
            }
            else
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, PullBackForce * Time.deltaTime);
        }
        else
        {
            //transform.localPosition = Vector3.Lerp(transform.localPosition, StartPos, PullBackForce * Time.deltaTime);
            //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, StartRot, PullDownForce * Time.deltaTime * 20);

            if (PullUp)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, DesiredRotation, PullDownForce * Time.deltaTime * 20);
                if (transform.localRotation.x <= DesiredRotation.x)
                {
                    PullUp = false;
                }
            }
            else
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, StartRot, PullDownForce * Time.deltaTime * 20);

            if (PullBack)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, DesiredPosition, PullBackForce * Time.deltaTime);
                if (transform.localPosition.z >= DesiredPosition.z)
                {
                    PullBack = false;
                }
            }
            else
                transform.localPosition = Vector3.Lerp(transform.localPosition, StartPos, PullBackForce * Time.deltaTime);
        }
    }
}
