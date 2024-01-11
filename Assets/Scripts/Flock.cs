using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    float speed;
    bool isTurning = false;

    void Start()
    {
        //randomly set speed from static pointer
        speed = Random.Range(FlockManager.flockManager.minSpeed, FlockManager.flockManager.maxSpeed);
    }

    void Update()
    {
        //bounding box
        Bounds b = new Bounds(FlockManager.flockManager.transform.position, FlockManager.flockManager.flockRadius * 2);
        if(!b.Contains(transform.position))
            isTurning = true;
        else
            isTurning = false;
        if(isTurning) //turn the fish to stay within the area
        {
            Vector3 dir = FlockManager.flockManager.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir),
                                                    FlockManager.flockManager.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 100) < 50)
            {
                speed = Random.Range(FlockManager.flockManager.minSpeed, FlockManager.flockManager.maxSpeed);
            }
            if (Random.Range(0, 100) < 70) //so that not updating fish each time
            {
                MoveAllFishToNewPositions();
            }
        }


        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void MoveAllFishToNewPositions()
    {
        GameObject[] allFish = FlockManager.flockManager.allFish;
        Vector3 flockCenter = Vector3.zero;
        Vector3 vectorAvoid = Vector3.zero;
        float flockSpeed = 0.01f;
        float neighborDist;
        int flockSize = 0;
        float m1 = FlockManager.flockManager.m1;
        float m2 = FlockManager.flockManager.m2;
        float m3 = FlockManager.flockManager.m3;

        foreach (GameObject fish in allFish) //find all neighbors
        {
            if(fish != this.gameObject)
            {
                neighborDist = Vector3.Distance(fish.transform.position, this.transform.position);
                if(neighborDist <= FlockManager.flockManager.neighborDist)
                {
                    flockCenter += fish.transform.position; //rule 1
                    flockSize++;

                    if(neighborDist < 1.0f) //to avoid collisions -- rule 2
                    {
                        vectorAvoid += this.transform.position - fish.transform.position;
                    }

                    Flock otherFlock = fish.GetComponent<Flock>();
                    flockSpeed += otherFlock.speed; //rule 3 -- try to match velocity of nearby fish
                }
            }
        }

        if(flockSize > 0)
        {
            flockCenter = flockCenter / flockSize + (FlockManager.flockManager.lurePos - this.transform.position);
            flockCenter *= m1;
            //vectorAvoid *= m2;
            //flockSpeed *= m3;
            speed = flockSpeed / flockSize; //avg speed of flock

            if(speed > FlockManager.flockManager.maxSpeed)
            {
                speed = FlockManager.flockManager.maxSpeed;
            }

            Vector3 dir = (flockCenter + vectorAvoid) - transform.position;
            if(dir != Vector3.zero)
            {
                //interpolate the rotation of the fish
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                        Quaternion.LookRotation(dir),
                                                        FlockManager.flockManager.rotationSpeed * Time.deltaTime);
            }
        }

    }
}
