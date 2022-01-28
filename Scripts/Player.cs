using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour, IDamagable
{
    public static Player Instance;
    public TextMeshProUGUI PointsText;
    private int health;
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            print(Health);
            if (Health <= 0)
                HasDied();
        }
    }
    [SerializeField] int points;
    public int Points
    {
        get { return points; }
        set
        {
            points = value;
            if(PointsText != null)
                PointsText.text = "Points: "+ Points;
        }
    }

    private void Start()
    {
        Instance = this;
        Health = 100;
        Points = 1000;
        StartCoroutine(HealthRegen());
    }
    IEnumerator HealthRegen()
    {
        yield return new WaitForSeconds(1);
        if(Health < 100)
        Health++;
        StartCoroutine(HealthRegen());
    }
    private void HasDied()
    {
        SceneManager.LoadScene("VrScene");
    }

    public void TakeDamage(int damage, Vector3 direction, Player player)
    {
        Health -= damage;
    }
}
