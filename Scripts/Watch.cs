using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Watch : MonoBehaviour
{
    public TextMeshProUGUI PointsText;
    void Start()
    {
        GameObject.Find("VrRig").GetComponent<Player>().PointsText = PointsText;
    }
}
