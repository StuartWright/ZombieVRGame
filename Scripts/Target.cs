using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, IDamagable
{
    private Animator Anim;
    private float Timer;
    private bool StartTimer;
    private Collider Col;
    [SerializeField]
    private int health;
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (Health <= 0)
            {
                Dead();
                //transform.position = GameManager.Instance.RandomPos();
                //transform.position = new Vector3(transform.position.x , 1, transform.position.z);
            }
        }
    }
    private void Dead()
    {
        Col.enabled = false;
        Health = 1;
        Anim.SetTrigger("Shot");
        Timer = 5;
        StartTimer = true;
    }
    private void Update()
    {
        if(StartTimer)
        {
            Timer -= Time.deltaTime;
            if(Timer <= 0)
            {
                StartTimer = false;
                Anim.SetTrigger("Reset");
                Col.enabled = true;
            }
        }
    }
    public void TakeDamage(int damage, Vector3 direction, Player player)
    {
        Health -= damage;
    }

    void Start()
    {
        Anim = GetComponentInChildren<Animator>();
        Col = GetComponent<Collider>();
    }

    
}
