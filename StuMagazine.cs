using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuMagazine : StuBaseGrabbable
{
    public GameObject BulletGO;
    [SerializeField]
    private int bullets;
    private bool IsEmpty, LetGo;
    private float Timer = 10;
    public int Bullets
    {
        get { return bullets; }
        set
        {
            bullets = value;
            if (Bullets == 0)
            {
                if(BulletGO != null)
                    BulletGO.SetActive(false);
                IsEmpty = true;
            }
                
        }
    }
    public override void OnSelectEnter(StuGrabber interactor)
    {
        LetGo = false;        
        base.OnSelectEnter(interactor);
        col.enabled = true;
    }
    public override void OnSelectExit(StuGrabber interactor)
    {
        LetGo = true;
        if (IsEmpty)
            Timer = 10;
        else
            Timer = 20;
        base.OnSelectExit(interactor);
    }
    public override void Update()
    {
        if(LetGo)
        {
            Timer -= Time.deltaTime;
            if(Timer <= 0)
            {
                Destroy(gameObject);
            }
        }
        base.Update();
    }
}
