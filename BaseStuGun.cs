using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class BaseStuGun : StuBaseGrabbable
{
    public delegate void GrabbedEvents();
    public event GrabbedEvents GunGrabbed, GunReleased, NoAmmo;
    public delegate void MagEvents();
    public event MagEvents MagInserted;
    public Animator ShootAnim;
    public GameObject BulletCase, Bullet;
    public Transform ShellSpawn, FirePoint;
    [HideInInspector]
    public AudioSource AS;
    public List<ParticleSystem> FlashParticles;
    public float BulletCasePower;
    public AudioClip FireSound, ReloadClip, NoBulletsClip;
    [HideInInspector]
    public StuMagazine CurrentMagazine;
    protected StuGunMagazine SocketInteractor;
    protected StuGunSlide GunSlider;
    protected bool HasSlide = true, Equipped;
    [HideInInspector]
    public bool OneInChamber;
    private Rigidbody SliderRB;
    public float FireRate;
    protected float NextFire;
    public bool Automatic;
    private bool TriggerDown;
    public int Damage;
    protected Quaternion AttachInitialRotation;
    [HideInInspector]
    public bool RotateAroundSecondHand;
    protected float RecoilRotation, RecoilPushBack;
    protected StuRecoil Recoil;
    public float FollowHandSpeed;
    private StuWeaponSecondHandGrip SecondHandGrip;
    [HideInInspector]
    public bool CanFire;
    public string GunName;
    public GameObject GunMag;
    public GameObject Projectile;
    private float DestroyTimer = 10;
    public bool StartWithMagazine;
    public Transform HolsterOffsetPoint;
    public WeaponHolsterTypes HolsterType;
    public override void Awake()
    {
        DestroyTimer = 60;
        CanFire = true;
        Recoil = GetComponentInChildren<StuRecoil>();
        SecondHandGrip = GetComponentInChildren<StuWeaponSecondHandGrip>();
        if (ShootAnim == null)
            ShootAnim = GetComponent<Animator>();
        AS = GetComponent<AudioSource>();     
        SocketInteractor = GetComponentInChildren<StuGunMagazine>();
        if (SocketInteractor != null)
        {
            SocketInteractor.OnSelectedEnterAction += AddMagazine;
            SocketInteractor.OnSelectedExitAction += RemoveMagazine;
        }        
        GunSlider = GetComponentInChildren<StuGunSlide>();
        if(Recoil != null)
        {
            RecoilRotation = Recoil.OneHandedRecoilAmount;
            RecoilPushBack = Recoil.OneHandedPushBackAmount;
            Recoil.enabled = false;
        }
        
        
        base.Awake();
    }
    private void Start()
    {
        AS.enabled = false;
        bool hasMag = false;
        foreach (GunMags mag in StuMagazingHolster.Instance.AllMags)
        {
            if (mag.GunName == gameObject.tag)
            {
                hasMag = true;
            }
        }
        if (!hasMag)
        {
            GunMags mag = new GunMags(GunName, 0, GunMag);
            StuMagazingHolster.Instance.AllMags.Add(mag);
        }
        if(StartWithMagazine)
        {
            StartWithMag();
        }
        AS.enabled = true;
    }
    protected virtual void StartWithMag()
    {
        StuBaseGrabbable mag = Instantiate(GunMag, SocketInteractor.transform.position, transform.rotation).GetComponent<StuBaseGrabbable>();
        SocketInteractor.ForceSelect(mag);
        Slide();
    }
    public override void Update()
    {
        if (SecondInteractor && CurrentInteractor)
        {
            if(RotateAroundSecondHand)
            {
                Vector3 target = SecondInteractor.transform.position - CurrentInteractor.transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(target);

                Vector3 gripRotation = Vector3.zero;
                gripRotation.z = CurrentInteractor.transform.eulerAngles.z;

                lookRotation *= Quaternion.Euler(gripRotation.x, gripRotation.y, gripRotation.z);
                //transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, FollowHandSpeed * 3 * Time.deltaTime);
                transform.rotation = lookRotation;
            }
        }        
        base.Update();
    }
    public void CockCallback()
    {
        ShootAnim.enabled = true;
        AS.PlayOneShot(ReloadClip);
    }
    public void FireAnim()
    {
        ShootAnim.SetTrigger("Fire");
    }
    public void Fire()
    {
        TriggerDown = true;

    }
    protected void NoAmmoEvent() => NoAmmo?.Invoke();
    protected void CasingRelease(GameObject go)
    {
        GameObject tempCasing;
        tempCasing = Instantiate(go, ShellSpawn.position, ShellSpawn.rotation);
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(BulletCasePower * 0.7f, BulletCasePower), (ShellSpawn.position - ShellSpawn.right * 0.3f - ShellSpawn.up * 0.6f), 1f);
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);
        Destroy(tempCasing, 3);
    }   
    private void LateUpdate()
    {
        if (TriggerDown && CanFire)
        {
            if (Time.time > NextFire)
            {
                FireFunction();
                if (!Automatic)
                    TriggerDown = false;
            }
        }
        if (Equipped)
        {
            MagButton(); 
        }
        /*
        else if(!IsInHolster)
        {
            DestroyTimer -= Time.deltaTime;
            if (DestroyTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
        */
    }
    public virtual void MagButton()
    {
        if (CurrentMagazine != null)
        {
            if (TargetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool PrimaryButtonValue) && PrimaryButtonValue)
            {
                SocketInteractor.EjectMag();
            }
        }
    }
    protected override void AttachToHand()
    {
        if (GripPoint != null)
        {
            Vector3 Offset = transform.position - GripPoint.position;
            //transform.position = Vector3.Lerp(transform.position, CurrentInteractor.transform.position + Offset, FollowHandSpeed * Time.deltaTime);
            transform.position = CurrentInteractor.transform.position + Offset;
            if (!RotateAroundSecondHand)
                transform.localRotation = GripPoint.localRotation;
                //transform.localRotation = Quaternion.Lerp(transform.localRotation, GripPoint.localRotation, FollowHandSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = CurrentInteractor.transform.position;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, 0), FollowHandSpeed * Time.deltaTime);
        }

    }
    public virtual void FireFunction()
    {
        if(OneInChamber)
        {
            NextFire = Time.time + FireRate;
            OneInChamber = false;
            if (ShootAnim != null)
                ShootAnim.SetTrigger("Fire");
            if(BulletCase != null && ShellSpawn != null)
                CasingRelease(BulletCase);
            AS.PlayOneShot(FireSound);
            foreach (ParticleSystem flash in FlashParticles)
                flash.Play();
            /*
            RaycastHit hit;
            if (Physics.Raycast(FirePoint.position, FirePoint.forward, out hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<IDamagable>() != null)
                    hit.collider.GetComponent<IDamagable>().TakeDamage(Damage);
                GameManager.Instance.HandleHit(hit);
            }
            */
            Bullet bullet = Instantiate(Projectile, FirePoint.position, FirePoint.transform.rotation).GetComponent<Bullet>();
            bullet.Damage = Damage;
            bullet.Player = CurrentInteractor.Player;
            if (Recoil != null)
            {
                Recoil.AddRecoil(RecoilRotation);
                Recoil.AddPushBack(RecoilPushBack);
            }
            if (CurrentMagazine && CurrentMagazine.Bullets > 0)
            {
                
                OneInChamber = true;
                CurrentMagazine.Bullets--;               
            }
            else
            {
                if (CurrentMagazine.Bullets == 0)
                {
                    if (ShootAnim != null)
                        ShootAnim.ResetTrigger("Fire");
                    NoAmmoEvent();
                }
            }
        }
        else
        {           
            AS.PlayOneShot(NoBulletsClip);
            NextFire = Time.time + FireRate;
        }
    }
    public virtual void AddMagazine(StuBaseGrabbable interactable)
    {
        CurrentMagazine = interactable.GetComponent<StuMagazine>();
        MagInserted?.Invoke();
        AS.PlayOneShot(ReloadClip);
        HasSlide = false;
    }
    public void RemoveMagazine(StuBaseGrabbable interactable)
    {
        CurrentMagazine = null;
        AS.PlayOneShot(ReloadClip);
    }    
    public virtual bool Slide()
    {
        if (CurrentMagazine && CurrentMagazine.Bullets > 0)
        {
            HasSlide = true;
            if (OneInChamber)
            {
                CurrentMagazine.Bullets--;
                CasingRelease(Bullet);
            }
            else
            {
                OneInChamber = true;
                CurrentMagazine.Bullets--;
            }
            AS.PlayOneShot(ReloadClip);
            return true;
        }
        else
        {
            if(OneInChamber)
            {
                OneInChamber = false;
                CasingRelease(Bullet);
                AS.PlayOneShot(ReloadClip);
            }
            return false;
        }            
    }

    public override void OnSelectEnter(StuGrabber interactor)
    {
        StuMagazingHolster.Instance.GetCurrentMagInfo(GunName);
        if(IsInHolster)
        {
            Holster.OnSelectExit(this);
        }
        if (Recoil != null)
        {
            Recoil.enabled = true;
            RecoilRotation = Recoil.OneHandedRecoilAmount;
            RecoilPushBack = Recoil.OneHandedPushBackAmount;
        }       
        GunGrabbed?.Invoke();
        if (SliderRB != null)
            SliderRB.isKinematic = true;

        Equipped = true;       
        base.OnSelectEnter(interactor);
        AttachInitialRotation = interactor.transform.localRotation;       
    }

    public override void OnSelectExit(StuGrabber interactor)
    {
        DestroyTimer = 60;
        StuMagazingHolster.Instance.CurrentMagInfo = null;
        //print("First Hand Released");
        if (Recoil != null)
            Recoil.enabled = false;
        GunReleased?.Invoke();
        if (SliderRB != null)
            SliderRB.isKinematic = false;
        
        Equipped = false;       
        if (CurrentMagazine)
            CurrentMagazine.transform.SetParent(null);
        base.OnSelectExit(interactor);
        
        interactor.transform.localRotation = AttachInitialRotation;
        if (SecondInteractor != null)
        {
            StuGrabber hand = SecondInteractor;
            SecondHandGrip.OnSelectExit(SecondInteractor);
            hand.GrabbedObject = this;
            OnSelectEnter(hand);
        }
        SecondInteractor = null;
        foreach(StuGrabber hand in GameManager.Instance.Hands)
        {
            if(hand.GrabbedObject != null)
            {                
                if (hand.GrabbedObject.GetComponent<BaseStuGun>())
                {
                    BaseStuGun gun = hand.GrabbedObject.GetComponent<BaseStuGun>();
                    if(gun.Equipped)
                        StuMagazingHolster.Instance.GetCurrentMagInfo(gun.GunName);
                }
            }
        }
    }
    public void OnSecondHandGrab(StuGrabber interactor, bool RotateBetweenHands)
    {
        //print("Second Hand Grab");
        if (Recoil != null)
        {
            RecoilRotation = Recoil.TwoHandedRecoilAmount;
            RecoilPushBack = Recoil.TwoHandedPushBackAmount;
        }        
        RotateAroundSecondHand = RotateBetweenHands;
        SecondInteractor = interactor;
    }
    public void OnSecondHandRelease(StuGrabber interactor)
    {
        //print("Second Hand Released");
        RotateAroundSecondHand = false;
        if (Recoil != null)
        {
            RecoilRotation = Recoil.OneHandedRecoilAmount;
            RecoilPushBack = Recoil.OneHandedPushBackAmount;
        }
        SecondInteractor = null;
    }
    public void TriggerUp()
    {
        TriggerDown = false;
    }
    protected override void OnActivate()
    {
        Fire();
        base.OnActivate();
    }
    protected override void OnDeactivate()
    {
        TriggerUp();
        base.OnDeactivate();
    }
    protected override void OnSecondaryButtonActivate()
    {
        if(!HasSlide)
        {
            GunSlider.ManualCockGun();
            Slide();
        }        
        base.OnSecondaryButtonActivate();
    }
    protected override void OnSecondaryButtonDeactivate()
    {
        base.OnSecondaryButtonDeactivate();
    }   
}
