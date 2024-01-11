using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{

    private float lureCooldown = 0.0f;
    private bool isOnCooldown = false;
    private float scatterCooldown = 0.0f;
    private bool isScattering = false;
    [SerializeField] private GameObject fishPrefab;
    [SerializeField] private int numFish = 20;
    [SerializeField] private GameObject lurePrefab;
    public Vector3 flockRadius = new Vector3(5, 5, 5); //area in which the fish can swim
    public GameObject[] allFish;
    public static FlockManager flockManager;
    public Vector3 lurePos = Vector3.zero;
    public float m1 = 1.0f;
    public float m2 = 1.0f;
    public float m3 = 1.0f; //multipliers for scattering -- set to -1 for scatter

    [Header("Fish Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 5.0f)]
    public float maxSpeed;
    [Range(1.0f, 10.0f)]
    public float neighborDist;
    [Range(1.0f, 5.0f)]
    public float rotationSpeed;

    void Start()
    {
        InitializePositions();
        flockManager = this;
        lurePos = this.transform.position;
    }

    void Update()
    {
        CheckForLure();
        CheckForRock();
    }

    void InitializePositions()
    {
        allFish = new GameObject[numFish];
        for (int i = 0; i < numFish; i++)
        {
            //random position from flockmanager
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-flockRadius.x, flockRadius.x),
                                                                Random.Range(-flockRadius.y, flockRadius.y),
                                                                Random.Range(-flockRadius.z, flockRadius.z));
            allFish[i] = Instantiate(fishPrefab, pos, Quaternion.identity); //zero rotation
        }
    }

    void CheckForLure()
    {
        if(Input.GetMouseButtonDown(0))
        {
            float lureDistFromCamera = 10.0f;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = lureDistFromCamera;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            lurePos = mousePos;
            lureCooldown = 10.0f;

            GameObject newLure = Instantiate(lurePrefab, worldPos, Quaternion.identity);
            Destroy(newLure, 10.0f);

            isOnCooldown = true;
        }
        else if(!isOnCooldown)
        {
            if (Random.Range(0, 100) < 10)
            {
                lurePos = this.transform.position + new Vector3(Random.Range(-flockRadius.x, flockRadius.x),
                                                                    Random.Range(-flockRadius.y, flockRadius.y),
                                                                    Random.Range(-flockRadius.z, flockRadius.z));
            }
        }
        if (lureCooldown <= 0.0f)
            isOnCooldown = false;
        else
            lureCooldown -= Time.deltaTime;
    }

    void CheckForRock()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Debug.Log("rock casted");
            m1 = -1.0f; m2 = -1.0f; m3 = -1.0f;
            scatterCooldown = 10.0f;
            isScattering = true;
        }
        else if(!isScattering)
        {
            m1 = 1.0f; m2 = 1.0f; m3 = 1.0f;
        }
        if (scatterCooldown <= 0.0f)
            isScattering = false;
        else
            scatterCooldown -= Time.deltaTime;
    }
}
