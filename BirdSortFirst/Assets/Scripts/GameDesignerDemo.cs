using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GameDesignerDemo : MonoBehaviour
{
 
    public BaseBird birdsSpawned0;
    public BaseBird birdsSpawned1;
    public BaseBird birdsSpawned2;
    List<BaseBird> spawnedObject = new List<BaseBird>();
    public List<BaseBranch> branchList = new List<BaseBranch>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        SpawnBirds();
        DistributeBirds();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void SpawnBirds(int totalTypes = 3)
    {
        for (int currentID = 0; currentID < totalTypes; currentID++) 
        {
            if (currentID == 0)
            {
                for (int i = 3; i >= 0; i--)
                {
                    BaseBird birdSpawnNew = Instantiate(birdsSpawned0);
                    birdSpawnNew.ID = currentID;
                    spawnedObject.Add(birdSpawnNew);
                }
            }
            if (currentID == 1)
            {
                for (int i = 3; i >= 0; i--)
                {
                    BaseBird birdSpawnNew = Instantiate(birdsSpawned1);
                    birdSpawnNew.ID = currentID;
                    spawnedObject.Add(birdSpawnNew);
                }
            }
            if (currentID == 2)
            {
                for (int i = 3; i >= 0; i--)
                {
                    BaseBird birdSpawnNew = Instantiate(birdsSpawned2);
                    birdSpawnNew.ID = currentID;
                    spawnedObject.Add(birdSpawnNew);
                }
            }
        }
    }
   
    public void DistributeBirds()
    {
        
        for (int i = 0; i < branchList.Count; i++)
        {
            
            for (int j = 0; j < 4; j ++)
            {
                if (spawnedObject.Count == 0)
                    break;
                int randomMau = UnityEngine.Random.Range(0, spawnedObject.Count);
                BaseBird SelectedBird = spawnedObject[randomMau];
                spawnedObject.RemoveAt(randomMau);
                branchList[i].AddBird(SelectedBird);
                branchList[i].UpdateBirdPosition();
            }
        }
    }
}
