using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Zombie : BaseEnemy
{
    public Rigidbody[] bodies;
    public float DistanceToGround;
    public LayerMask Layer;
    private bool IKFeet;
    public bool HasStartAnim;
    public AudioSource AS;
    protected Collider ZombieCollider;
    private Vector3 velocity, lastPosition;
    private Vector3 Targetvelocity, targetLastPos;
    private void LateUpdate()
    {
        if (IsDead || Target == null) return;
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
        if (Agent.remainingDistance > 10)
        {
            Targetvelocity = (Target.position - targetLastPos) / Time.deltaTime;
            targetLastPos = Target.position;
            Agent.destination = Target.position + Targetvelocity * 2;
        }
        else
            Agent.destination = Target.position;

        Anim.SetFloat("Speed", Agent.velocity.magnitude / Agent.speed);
        if (Agent.remainingDistance <= Agent.stoppingDistance)
        {
            //float distance = Vector3.Distance(transform.position, Target.position);
            //if (distance <= 1)
             //   Anim.SetTrigger("Attack");
            Agent.velocity /= 2;
        }
    }
    IEnumerator t()
    {
        yield return new WaitForSeconds(5);
        HasDied();
    }
    protected new  void Start()
    {
        //StartCoroutine(t());
        bodies = GetComponentsInChildren<Rigidbody>();
        ZombieCollider = GetComponent<Collider>();
        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = true;
            rb.mass = 0.1f;
            rb.angularDrag = 0.1f;
        }
        base.Start();
        if(!HasStartAnim)
        HasStoodUp();
    }
    public void DealDamage()
    {
        float distance = Vector3.Distance(transform.position, Target.position);
        if(distance <= 1.3f)
        Target.GetComponent<IDamagable>().TakeDamage(30, Vector3.zero, null);
        //print(distance);
    }
    public void SetMove()
    {
        Agent.isStopped = !Agent.isStopped;
    }
    public void HasStoodUp()
    {
        Anim.SetTrigger("HasStoodUp");
        Target = GameObject.Find("VrRig").transform;
        //IKFeet = true;
    }
    protected override void HasDied()
    {
        if (IsDead) return;
        ZombieCollider.enabled = false;
        if (HasStartAnim)
            GameManager.Instance.Wave++;
        AS.Stop();
        if(!HasStartAnim)
        GameManager.Instance.EnemiesRemaining--;
        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = false;
            rb.AddForce(velocity * 10);
        }
        //foreach (Rigidbody rb in bodies)
        //{
        //    rb.AddForce(velocity * 150);
        //}
        //GameManager.Instance.SpawnZombie();
        IsDead = true;
        //Agent.velocity = Vector3.zero;        
        Anim.enabled = false;        
        Agent.enabled = false;
        this.enabled = false;
        Destroy(gameObject, 5);
    }
    /*
    void OnAnimatorIK(int layerIndex)
    {
        if(Anim)
        {
            Anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            Anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

            RaycastHit Hit;
            Ray ray = new Ray(Anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out Hit, DistanceToGround + 1.5f, Layer))
            {
                if(Hit.transform.tag == "Floor")
                {
                    Vector3 footPos = Hit.point;
                    footPos.y += DistanceToGround;
                    Anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);
                    Anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, Hit.normal));
                }
            }

            Anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            Anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
            ray = new Ray(Anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out Hit, DistanceToGround + 1.5f, Layer))
            {
                if (Hit.transform.tag == "Floor")
                {
                    Vector3 footPos = Hit.point;
                    footPos.y += DistanceToGround;
                    Anim.SetIKPosition(AvatarIKGoal.RightFoot, footPos);
                    Anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, Hit.normal));
                }
            }
        }
    }
    */
}
