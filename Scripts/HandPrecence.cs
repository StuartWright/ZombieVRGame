using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class HandPrecence : MonoBehaviour
{
    public bool ShowControllers;
    public InputDeviceCharacteristics ControllerCharacteristics;
    public InputDevice TargetDevice;
    public GameObject HandModel;
    public List<GameObject> ControllerPrefabs;
    private GameObject SpawnedController;
    public GameObject SpawnedHandModel;
    public Animator HandAnim;
    public bool RightHand;
    private Vector3 StartPos;
    private Quaternion StartRot;
    private void Start()
    {
        TryInit();
    }
    private void TryInit()
    {
        List<InputDevice> Devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(ControllerCharacteristics, Devices);

        if (Devices.Count > 0)
        {
            TargetDevice = Devices[0];
            if (ShowControllers)
            {
                GameObject prefab = ControllerPrefabs.Find(Controller => Controller.name == TargetDevice.name);
                if (prefab)
                {
                    SpawnedController = Instantiate(prefab, transform);
                }
                else
                {
                    print("no controller found");
                    SpawnedController = Instantiate(ControllerPrefabs[0], transform);
                }
            }
            else
            {
                SpawnedHandModel = Instantiate(HandModel, transform);
                StartPos = SpawnedHandModel.transform.localPosition;
                StartRot = SpawnedHandModel.transform.localRotation;
                HandAnim = SpawnedHandModel.GetComponent<Animator>();
            }
        }
        GetComponentInParent<StuGrabber>().SetUp(this); 
    }
    public void ResetHandPos()
    {
        SpawnedHandModel.transform.SetParent(transform);
        SpawnedHandModel.transform.localPosition = StartPos;
        SpawnedHandModel.transform.localRotation = StartRot;
    }
    private void UpdateHandAnims()
    {
        if(TargetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            HandAnim.SetFloat("Trigger", triggerValue);
        else
            HandAnim.SetFloat("Trigger", 0);
        if (TargetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            HandAnim.SetFloat("Grip", gripValue);
        else
            HandAnim.SetFloat("Grip", 0);
    }
    private void Update()
    {
        if (!TargetDevice.isValid)
            TryInit();
        else
        UpdateHandAnims();
    }
}
