using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [HideInInspector]
    public List<StuGrabber> Hands = new List<StuGrabber>();
    public GameObject Zombie;
    public bool MagRelease;
    public List<Transform> SpawnPoints = new List<Transform>();
    public GameObject metalHitEffect;
    public GameObject sandHitEffect;
    public GameObject stoneHitEffect;
    public GameObject waterLeakEffect;
    public GameObject waterLeakExtinguishEffect;
    public GameObject fleshHitEffects;
    public GameObject woodHitEffect;
    private int EnemiesToSpawn;
    private AudioSource AS;
    public AudioClip EndWave, StartWave;
    private float SpawnTimer;
    private int ZombieHealthToAdd;
    private int wave;
    private bool FlipFlop;   
    public int Wave
    {
        get { return wave; }
        set
        {
            wave = value;
            AS.PlayOneShot(StartWave);
            EnemiesToSpawn += 1;
            EnemiesRemaining = EnemiesToSpawn;
            StartCoroutine(SpawnZombies());
        }
    }
    private int enemiesRemaining;
    public int EnemiesRemaining
    {
        get { return enemiesRemaining; }
        set
        {
            enemiesRemaining = value;
            if(EnemiesRemaining == 0)
            {
                WeaponCrate.Instance.EnableCrate();
                FlipFlop = !FlipFlop;
                if(FlipFlop)
                {

                }
                else
                {
                    ZombieHealthToAdd += 1;
                }               
                if(SpawnTimer > 2)
                    SpawnTimer -= 0.2f;
                AS.PlayOneShot(EndWave);
                StartCoroutine(WaveDelay());
            }
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        AS = GetComponent<AudioSource>();
        SpawnTimer = 4;
        XRSettings.eyeTextureResolutionScale = 1.5f;
        EnemiesToSpawn = 3;
        Wave++;
        EnemiesRemaining += 1;// for the preplaced zombie
        //StartCoroutine(testie());
    }
    RaycastHit hit;
    IEnumerator testie()
    {
        yield return new WaitForSeconds(6);
        if (Physics.Raycast(Player.Instance.transform.position + new Vector3(Random.Range(-70f, 70f), 100.0f, Random.Range(-70f, 70f)), Vector3.down, out hit, 200.0f))
        {
            int num = Random.Range(0, 100);
            if(num <= 10)
            {
                for(int i = 0; i < 15; i++)
                {
                    Instantiate(Zombie, hit.point, Quaternion.identity);
                }
            }
            Instantiate(Zombie, hit.point, Quaternion.identity);
        }
        StartCoroutine(testie());
    }
    IEnumerator WaveDelay()
    {
        yield return new WaitForSeconds(10);
        Wave++;
    }
    IEnumerator SpawnZombies()
    {
        
        for(int i = 0; i < EnemiesToSpawn; i++)
        {
            yield return new WaitForSeconds(SpawnTimer);
            Zombie zombie = Instantiate(Zombie, SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity).GetComponent<Zombie>();
            zombie.Health += ZombieHealthToAdd;
        }
    }
    public Vector3 RandomPos()
    {
        RaycastHit Hit;
        bool CanPlace = false;
        while (!CanPlace)
        {
            Vector3 pos = new Vector3(Random.Range(2, 98), 0, Random.Range(2, 98));
            if (Physics.Raycast(pos + new Vector3(0, 10, 0), Vector3.down, out Hit, 12))
            {
                if (Hit.collider.tag == "Floor")
                {
                    CanPlace = true;
                    return pos;
                }
            }
        }
        return new Vector3(0, 0, 0);
    }

    public void HandleRayHit(RaycastHit hit)
    {        
        Renderer MR = hit.collider.GetComponent<Renderer>();
        if (MR == null)
        {
            MR = hit.collider.transform.root.GetComponentInChildren<Renderer>();
        }          
        if(MR != null)
        {
            List<Material> allMaterials = new List<Material>();
            MR.GetSharedMaterials(allMaterials);
            foreach (Material mat in allMaterials)
            {
                switch (mat.name)
                {
                    case "Metal":
                        SpawnRayDecal(hit, metalHitEffect, MR.transform);
                        break;
                    case "Sand":
                        SpawnRayDecal(hit, sandHitEffect, MR.transform);
                        break;
                    case "Stone":
                        SpawnRayDecal(hit, stoneHitEffect, MR.transform);
                        break;
                    case "WaterFilled":
                        SpawnRayDecal(hit, waterLeakEffect, MR.transform);
                        SpawnRayDecal(hit, metalHitEffect, MR.transform);
                        break;
                    case "Wood":
                        SpawnRayDecal(hit, woodHitEffect, MR.transform);
                        break;
                    case "Flesh":
                        SpawnRayDecal(hit, fleshHitEffects, MR.transform);
                        break;
                    case "WaterFilledExtinguish":
                        SpawnRayDecal(hit, waterLeakExtinguishEffect, MR.transform);
                        SpawnRayDecal(hit, metalHitEffect, MR.transform);
                        break;
                }
            }
        }
    }

    void SpawnRayDecal(RaycastHit hit, GameObject prefab, Transform parent)
    {
        GameObject spawnedDecal = Instantiate(prefab, hit.point, Quaternion.LookRotation(hit.normal));
        //spawnedDecal.transform.SetParent(parent);
        spawnedDecal.transform.SetParent(hit.transform);
    }
}
