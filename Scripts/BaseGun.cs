using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGun : MonoBehaviour
{
    public Animator Anim;
    public GameObject BulletCase;
    public Transform ShellSpawn, FirePoint;
    public AudioSource AS;
    public List<ParticleSystem> Flash;
    public float BulletCasePower;
    public AudioClip FireSound;
    void Start()
    {
        if (Anim == null)
            Anim = GetComponent<Animator>();
        AS = GetComponent<AudioSource>();
    }

    public void Fire()
    {
        Anim.SetTrigger("Fire");
        CasingRelease();
        AS.PlayOneShot(FireSound);
        foreach (ParticleSystem flash in Flash)
            flash.Play();

        RaycastHit hit;
        if (Physics.Raycast(FirePoint.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            if (hit.collider.GetComponent<IDamagable>() != null)
                hit.collider.GetComponent<IDamagable>().TakeDamage(1, Vector3.zero, null);
        }
    }
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        //if (!casingExitLocation || !casingPrefab)
        //{ return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(BulletCase, ShellSpawn.position, ShellSpawn.rotation);
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(BulletCasePower * 0.7f, BulletCasePower), (ShellSpawn.position - ShellSpawn.right * 0.3f - ShellSpawn.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, 3);
    }
    private void LateUpdate()
    {
        //Debug.DrawRay(FirePoint.position, transform.TransformDirection(Vector3.forward) * 100, Color.red);             
    }
}
