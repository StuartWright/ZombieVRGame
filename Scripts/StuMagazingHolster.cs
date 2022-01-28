using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class StuMagazingHolster : StuBaseGrabbable
{
    public static StuMagazingHolster Instance;
    public List<GunMags> AllMags = new List<GunMags>();
    public GunMags CurrentMagInfo;
    public new void Awake()
    {
        Instance = this;
        base.Awake();
    }

    public override void OnSelectEnter(StuGrabber interactor)
    {
        if (CurrentMagInfo == null) return;
        GameObject go = Instantiate(CurrentMagInfo.MagGO, transform.position, transform.rotation);
        StuBaseGrabbable obj = go.GetComponent<StuBaseGrabbable>();
        interactor.GrabbedObject = obj;
        interactor.GrabbedObject.OnSelectEnter(interactor);
        //base.OnSelectEnter(interactor);
    }
    public override void OnSelectExit(StuGrabber interactor)
    {
        //base.OnSelectExit(interactor);
    }
    public void GetCurrentMagInfo(string name)
    {
        foreach(GunMags gun in AllMags)
        {
            if(name == gun.GunName)
            {
                CurrentMagInfo = gun;
            }
        }
    }
}

[Serializable]
public class GunMags
{
    public GunMags(string name, int amount, GameObject magGO)
    {
        GunName = name;
        MagCount = amount;
        MagGO = magGO;
    }
    public string GunName;
    public int MagCount;
    public GameObject MagGO;
}
