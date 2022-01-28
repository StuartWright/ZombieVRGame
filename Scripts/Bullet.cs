using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage;
    public Rigidbody RB;
    public LayerMask layer;
    private float Timer = 2;
    RaycastHit hit;
    private Vector3 velocity, lastPosition;
    [HideInInspector]
    public Player Player;
    private void Start()
    {
        lastPosition = transform.position;
        RB.AddForce(transform.forward * 20000);
    }
  
    private void FixedUpdate()
    {
        velocity = (transform.position - lastPosition);
        lastPosition = transform.position;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3f))
        {
            if (((1 << hit.collider.gameObject.layer) & layer) != 0)
            {
                if (hit.collider.TryGetComponent(out IDamagable _hit))
                    _hit.TakeDamage(Damage, velocity, Player);
                GameManager.Instance.HandleRayHit(hit);
                Destroy(gameObject);
            }
            
        }        
    }
    private void LateUpdate()
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0)
            Destroy(gameObject);
    }
}
