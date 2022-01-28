using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
public class BaseVRGun : XRGrabInteractable
{
    public Animator Anim;
    public GameObject BulletCase, Bullet;
    public Transform ShellSpawn, FirePoint;
    public AudioSource AS;
    public List<ParticleSystem> Flash;
    public float BulletCasePower;
    public AudioClip FireSound, ReloadClip, NoBulletsClip;
    public Magazine CurrentMagazine;
    private XRBaseInteractor SocketInteractor;
    private Sliderr GunSlider;
    protected bool HasSlide = true, OneInChamber, Equipped;
    private Rigidbody RB, SliderRB;
    public float FireRate;
    protected float NextFire;
    public bool Automatic;
    private bool TriggerDown;
    public int Damage;
    public GameObject FirstHandGO, SecondHandGO;
    public List<XRSimpleInteractable> SecondHandgrabPoints = new List<XRSimpleInteractable>();
    protected XRBaseInteractor SecondInteractor;
    protected Quaternion AttachInitialRotation;
    //public Transform MainGrabPoint;
    public enum TwoHandRotationType { None, First, Second };
    public TwoHandRotationType RotationType;
    public bool SnapToSecondHand = true, RotateAroundSecondHand;
    private Quaternion InitialRotationOffset;
    public bool MustSlide;
    private Transform Rig;
    void Start()
    {
        Rig = GameObject.Find("VrRig").transform;
        if(Anim == null)
        Anim = GetComponent<Animator>();
        AS = GetComponent<AudioSource>();
        RB = GetComponent<Rigidbody>();
        SocketInteractor = GetComponentInChildren<XRSocketInteractor>();
        if(SocketInteractor != null)
        {
            SocketInteractor.onSelectEntered.AddListener(AddMagazine);
            SocketInteractor.onSelectExited.AddListener(RemoveMagazine);
        }
        GunSlider = GetComponentInChildren<Sliderr>();
        if(GunSlider != null)
        {
            SliderRB = GunSlider.gameObject.GetComponent<Rigidbody>();
        }
        foreach (XRSimpleInteractable item in SecondHandgrabPoints)
        {
            item.onSelectEntered.AddListener(OnSecondHandGrab);
            item.onSelectExited.AddListener(OnSecondHandRelease);
            //item.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
    
    public Quaternion GetTwoHandRotation()
    {
        Quaternion targetRotation;
        if (RotationType == TwoHandRotationType.None)
        {
            targetRotation = Quaternion.LookRotation(SecondInteractor.attachTransform.position - CurrentInteractor.attachTransform.position);
        }
        else if (RotationType == TwoHandRotationType.First)
        {
            targetRotation = Quaternion.LookRotation(SecondInteractor.attachTransform.position - CurrentInteractor.attachTransform.position, CurrentInteractor.transform.up);
        }
        else if (RotationType == TwoHandRotationType.Second)
        {
            targetRotation = Quaternion.LookRotation(SecondInteractor.attachTransform.position - CurrentInteractor.attachTransform.position, SecondInteractor.transform.up);           
        }
        else
            targetRotation = Quaternion.identity;
        return targetRotation;
    }
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        /*
        if (SecondInteractor && CurrentInteractor)
        {
            Vector3 target = SecondInteractor.transform.position - CurrentInteractor.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(target);

            Vector3 gripRotation = Vector3.zero;
            gripRotation.z = CurrentInteractor.transform.eulerAngles.z;

            lookRotation *= Quaternion.Euler(gripRotation);
            CurrentInteractor.attachTransform.rotation = lookRotation;
        }
        base.ProcessInteractable(updatePhase);
        */
        if (SecondInteractor && CurrentInteractor)
        {
            if (RotateAroundSecondHand)
            {
                if (SnapToSecondHand)
                    CurrentInteractor.attachTransform.rotation = GetTwoHandRotation();
                else
                    CurrentInteractor.attachTransform.rotation = GetTwoHandRotation() * InitialRotationOffset;
                SecondInteractor.transform.position = SecondHandgrabPoints[0].transform.position;
                SecondInteractor.transform.rotation = SecondHandgrabPoints[0].transform.rotation;
                //CurrentInteractor.transform.rotation = CurrentInteractor.attachTransform.rotation;
            }
            else
            {
                SecondInteractor.transform.position = SecondHandgrabPoints[0].transform.position;
                SecondInteractor.transform.rotation = SecondHandgrabPoints[0].transform.rotation;
            }
        }
        //else if (CurrentInteractor && SecondInteractor == null)
              //CurrentInteractor.attachTransform.localRotation = AttachInitialRotation;
            base.ProcessInteractable(updatePhase);
        
    }
    public void FireAnim()
    {
        Anim.SetTrigger("Fire");
    }
    public void Fire()
    {
        TriggerDown = true;
        
    }
    protected void CasingRelease(GameObject go)
    {
        GameObject tempCasing;
        tempCasing = Instantiate(go, ShellSpawn.position, ShellSpawn.rotation);
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(BulletCasePower * 0.7f, BulletCasePower), (ShellSpawn.position - ShellSpawn.right * 0.3f - ShellSpawn.up * 0.6f), 1f);
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);
        Destroy(tempCasing, 3);
    }

    private InputDevice TargetDevice;
    private HandPrecence FirstHand, SecondHand;
    private void LateUpdate()
    {
        if(TriggerDown)
        {
            if(Time.time > NextFire)
            {
                FireFunction();
            }            
        }
        if(Equipped)
        {
            //if (TargetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool PrimaryButtonValue) && PrimaryButtonValue)
            if (TargetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool PrimaryButtonValue) && PrimaryButtonValue)
            {
                GameManager.Instance.MagRelease = true;
            }
            else
            {
                GameManager.Instance.MagRelease = false;
            }
        }        
    }
    public virtual void FireFunction()
    {
        if (OneInChamber || (!MustSlide && CurrentMagazine && CurrentMagazine.Bullets > 0))
        {
            NextFire = Time.time + FireRate;
            OneInChamber = false;
            if (Anim != null)
                Anim.SetTrigger("Fire");
            CasingRelease(BulletCase);
            AS.PlayOneShot(FireSound);
            foreach (ParticleSystem flash in Flash)
                flash.Play();

            RaycastHit hit;
            if (Physics.Raycast(FirePoint.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<IDamagable>() != null)
                    hit.collider.GetComponent<IDamagable>().TakeDamage(Damage, Vector3.zero, null);
            }
            //RB.AddRelativeForce(Vector3.back * 1000, ForceMode.Impulse);//REQUIRED VELOCITY TRACKED

            if (CurrentMagazine && CurrentMagazine.Bullets > 0)
            {
                OneInChamber = true;
                CurrentMagazine.Bullets--;
            }
        }
        else
        {
            AS.PlayOneShot(NoBulletsClip);
            NextFire = Time.time + FireRate;
        }
    }
    public bool HasMag;

    public void AddMagazine(XRBaseInteractable interactable)
    {
        HasMag = true;
        CurrentMagazine = interactable.GetComponent<Magazine>();
        //CurrentMagazine.transform.SetParent(selectingInteractor.transform);
        CurrentMagazine.transform.SetParent(transform);
        AS.PlayOneShot(ReloadClip);
        HasSlide = false;
        CurrentMagazine.interactionLayerMask = LayerMask.GetMask("UnGrabbable");
    }
    public void RemoveMagazine(XRBaseInteractable interactable)
    {
        HasMag = false;
        if (GameManager.Instance.MagRelease)
        {
            if (CurrentMagazine)
            {
                CurrentMagazine.interactionLayerMask = LayerMask.GetMask("Default");
                CurrentMagazine.transform.SetParent(null);
            }               
            CurrentMagazine = null;
            AS.PlayOneShot(ReloadClip);
            
        }
        
    }
    public void Slide()
    {
        HasSlide = true;
        if(CurrentMagazine && CurrentMagazine.Bullets > 0)
        {
            if(OneInChamber)
            {
                CurrentMagazine.Bullets--;
                CasingRelease(Bullet);
            }
            else
            {
                OneInChamber = true;
                CurrentMagazine.Bullets--;
            }           
        }
        AS.PlayOneShot(ReloadClip);
    }

    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        if (SliderRB != null)
            SliderRB.isKinematic = true;
        if (GunSlider != null)
            GunSlider.SetEverything();
        
        Equipped = true;
        FirstHand = interactor.GetComponentInChildren<HandPrecence>();
        TargetDevice = FirstHand.TargetDevice;
        if(FirstHandGO != null)
        {
            FirstHand.SpawnedHandModel.SetActive(false);
            FirstHandGO.SetActive(true);
        }                
        transform.SetParent(CurrentInteractor.transform);
        //transform.SetParent(Rig);
        base.OnSelectEntered(interactor);
        AttachInitialRotation = interactor.attachTransform.localRotation;
        //foreach (XRSimpleInteractable item in SecondHandgrabPoints)
          //  item.gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        
        if (SliderRB != null)
            SliderRB.isKinematic = false;
        if (GunSlider != null)
            GunSlider.SetNothing();
        //foreach (XRSimpleInteractable item in SecondHandgrabPoints)
          //  item.gameObject.GetComponent<BoxCollider>().enabled = false;
        Equipped = false;
        if (FirstHandGO != null)
        {
            FirstHandGO.SetActive(false);
            FirstHand.SpawnedHandModel.SetActive(true);
            FirstHand = null;
        }           
        transform.SetParent(null);
        if(CurrentMagazine)
            CurrentMagazine.transform.SetParent(null);
        base.OnSelectExited(interactor);
        SecondInteractor = null;
        interactor.attachTransform.localRotation = AttachInitialRotation;
    }
    public void OnSecondHandGrab(XRBaseInteractor interactor)
    {
        if (SecondHandGO != null)
        {
            SecondHand = interactor.GetComponentInChildren<HandPrecence>();
            SecondHand.SpawnedHandModel.SetActive(false);
            SecondHandGO.SetActive(true);
        }       
        print("second grab enter");
        SecondInteractor = interactor;
        InitialRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * CurrentInteractor.attachTransform.rotation;
    }
    public void OnSecondHandRelease(XRBaseInteractor interactor)
    {
        if (SecondHandGO != null)
        {
            SecondHand.SpawnedHandModel.SetActive(true);
            SecondHand = null;
            SecondHandGO.SetActive(false);
        }            
        print("second grab exit");
        SecondInteractor = null;
        CurrentInteractor.attachTransform.localRotation = AttachInitialRotation;
    }
    private XRBaseInteractor CurrentInteractor;
    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        if (!isSelected)
        {
            CurrentInteractor = interactor;
        }
        bool iscurrent = interactor.Equals(CurrentInteractor);
        if (!iscurrent)
            print("gsr");
        return base.IsSelectableBy(interactor) && iscurrent;
    }
    public void TriggerUp()
    {
        TriggerDown = false;
    }
    public void ChangeMovementTypeKin()
    {
        movementType = MovementType.Kinematic;
    }
    public void ChangeMovementTypeInst()
    {
        movementType = MovementType.Instantaneous;
    }
    protected override void OnActivate(XRBaseInteractor interactor)
    {
        Fire();
        base.OnActivate(interactor);
    }
    protected override void OnDeactivate(XRBaseInteractor interactor)
    {
        TriggerUp();
        base.OnDeactivate(interactor);
    }
}
