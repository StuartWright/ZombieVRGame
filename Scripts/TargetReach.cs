using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
public class TargetReach : MonoBehaviour
{
    public float Threashold;
    public Transform target;
    public UnityEvent OnReached;
    private bool Reached;
    public BaseVRGun Parent;
    private void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        print(distance);
        if (distance < Threashold)
        {
            if (!Reached)
            {
                Reached = true;
                OnReached.Invoke();
            }
        }
        else
            Reached = false;
    }
    private void OnEnable()
    {
       // Parent.movementType = 
    }
}
