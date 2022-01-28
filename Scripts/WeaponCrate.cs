using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCrate : StuBaseGrabbable
{
    public static WeaponCrate Instance;
    public Transform WeaponPoint;
    private Vector3 LidStartPoint;
    private Quaternion LidStartRot;
    public GameObject Lid;
    private Rigidbody LidRB;
    private bool IsOpen;
    public int CrateCost = 1000;
    public List<GameObject> LowTier = new List<GameObject>();
    public List<GameObject> MidTier = new List<GameObject>();
    public List<GameObject> TopTier = new List<GameObject>();
    private void Start()
    {
        Instance = this;
        LidRB = Lid.GetComponent<Rigidbody>();
        LidStartPoint = Lid.transform.position;
        LidStartRot = Lid.transform.rotation;
    }
    public override void OnSelectEnter(StuGrabber interactor)
    {
        if(IsOpen)
        {
            if (interactor.Player.Points >= CrateCost)
            {
                interactor.Player.Points -= CrateCost;
                EnableCrate();
            }
        }
        else
        {
            //col.enabled = false;
            List<GameObject> TempList = null;
            IsOpen = true;
            int randomNum = Random.Range(0, 100);
            if (randomNum <= 20)
                TempList = TopTier;
            else if (randomNum <= 30)
                TempList = MidTier;
            else
                TempList = LowTier;
            print(randomNum);
            Instantiate(TempList[Random.Range(0, TempList.Count)], WeaponPoint.position, WeaponPoint.rotation);
            LidRB.isKinematic = false;
            LidRB.AddExplosionForce(7f, new Vector3(1f, 0f, -1f), 5f, 0f, ForceMode.Impulse);
        }               
    }
    public override void OnSelectExit(StuGrabber interactor)
    {
        
    }
    public void EnableCrate()
    {
        //col.enabled = true;
        IsOpen = false;
        LidRB.isKinematic = true;
        Lid.transform.position = LidStartPoint;
        Lid.transform.rotation = LidStartRot;
    }
}
