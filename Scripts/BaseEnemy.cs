using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BaseEnemy : MonoBehaviour
{
    protected Animator Anim;
    protected Transform Target;
    protected NavMeshAgent Agent;
    public bool IsDead;
    public CapsuleCollider Collider;
    [SerializeField]
    private int health;
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (Health <= 0)
                HasDied();
        }
    }

    public virtual void Start()
    {
        Anim = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
    }   
    protected virtual void HasDied()
    {
        IsDead = true;
        //Collider.enabled = false;        
        Agent.isStopped = true;
        Anim.SetBool("Dead", true);
        //Destroy(gameObject, 5);
    }
}
