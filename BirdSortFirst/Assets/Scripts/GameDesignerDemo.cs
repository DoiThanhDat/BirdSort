using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class GameDesignerDemo : MonoBehaviour
{
 
    public BaseBird birdsSpawned0;
    public BaseBird birdsSpawned1;
    public BaseBird birdsSpawned2;
    public BaseBranch branchSpawned0;
    BranchTest m_gc;
    List<BaseBird> spawnedObject = new List<BaseBird>();
    public List<BaseBranch> branchListRight = new List<BaseBranch>();
    public List<BaseBranch> branchListLeft = new List<BaseBranch>();
    [SerializeField] Transform canvasTransform;
    public List<Transform> branchSpawnPointRight = new List<Transform>();
    public List<Transform> branchSpawnPointLeft = new List<Transform>();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_gc = FindAnyObjectByType<BranchTest>();
        SpawnBranch();
        SpawnBirds();
        DistributeBirds();
       
    }

    #region spawn branch
    public void SpawnBranch()
    {
        for (int i = 0; i < branchSpawnPointRight.Count; i++)
        {
            BaseBranch branchSpawnNew = Instantiate(branchSpawned0, branchSpawnPointRight[i]);
            branchSpawnNew.transform.localPosition = Vector3.zero;
            branchSpawnNew.transform.rotation = Quaternion.identity;
            branchSpawnNew.transform.localScale = new Vector3(-1f, 1f, 1f);
            branchSpawnNew.isRightBranch = true;
            branchListRight.Add(branchSpawnNew);
        }
        for (int i = 0; i < branchSpawnPointLeft.Count; i++)
        {
            BaseBranch branchSpawnNew = Instantiate(branchSpawned0, branchSpawnPointLeft[i]);
            branchSpawnNew.transform.localPosition = Vector3.zero;
            branchSpawnNew.transform.rotation = Quaternion.identity;
            branchSpawnNew.transform.localScale = new Vector3(1f, 1f, 1f);
            branchSpawnNew.isRightBranch = false;
            branchListLeft.Add(branchSpawnNew);
        }
    }
    #endregion

    #region Spawn && Distribute Birds
    public void SpawnBirds(int totalTypes = 3)
    {
        spawnedObject.Clear();
        for (int currentID = 1; currentID <= totalTypes; currentID++) 
        {
            if (currentID == 1)
            {
                for (int i = 3; i >= 0; i--)
                {
                    BaseBird birdSpawnNew = Instantiate(birdsSpawned0, canvasTransform);
                    birdSpawnNew.ID= currentID;
                    birdSpawnNew.SetSkinByID(currentID);
                    spawnedObject.Add(birdSpawnNew);
                }
            }
            if (currentID == 2)
            {
                for (int i = 3; i >= 0; i--)
                {
                    BaseBird birdSpawnNew = Instantiate(birdsSpawned1,canvasTransform);
                    birdSpawnNew.ID = currentID;
                    birdSpawnNew.SetSkinByID(currentID);
                    spawnedObject.Add(birdSpawnNew);
                }
            }
            if (currentID == 3)
            {
                for (int i = 3; i >= 0; i--)
                {
                    BaseBird birdSpawnNew = Instantiate(birdsSpawned2,canvasTransform);
                    birdSpawnNew.ID = currentID;
                    birdSpawnNew.SetSkinByID(currentID);
                    spawnedObject.Add(birdSpawnNew);
                }
            }
        }
    }
    public void DistributeBirds()
    {       
        for (int i = 0; i < branchListRight.Count; i++)
        {
            for (int j = 0; j < 4; j ++)
            {
                if (spawnedObject.Count == 0)
                    break;
                int randomMau = UnityEngine.Random.Range(0, spawnedObject.Count);
                BaseBird SelectedBird = spawnedObject[randomMau];
                spawnedObject.RemoveAt(randomMau);
                SelectedBird.transform.SetParent(branchListRight[i].transform, false);
                branchListRight[i].AddBird(SelectedBird);
            }
            branchListRight[i].UpdateBirdPosition();
        }
        for (int i = 0; i < branchListLeft.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (spawnedObject.Count == 0)
                    break;
                int randomMau = UnityEngine.Random.Range(0, spawnedObject.Count);
                BaseBird SelectedBird = spawnedObject[randomMau];
                spawnedObject.RemoveAt(randomMau);
                SelectedBird.transform.SetParent(branchListLeft[i].transform, false);
                branchListLeft[i].AddBird(SelectedBird);
            }
            branchListLeft[i].UpdateBirdPosition();
        }
    }
    #endregion

    #region Check Game Over
    public void CheckGameOver()
    {
        List<BaseBranch> allBranches = new List<BaseBranch>();
        allBranches.AddRange(branchListLeft);
        allBranches.AddRange(branchListRight);
        foreach (BaseBranch branches in allBranches)
        {
            if (branches != null && branches.birds.Count == 0)
            {
                return;
            }
        }
        for (int i = 0; i < allBranches.Count; i++)
        {
            for (int j = 0; j < allBranches.Count; j++)
            {
                if (i == j)
                    continue;
                else if (allBranches[j].birds.Count >= allBranches[j].capacity)
                    continue;
                else if (allBranches[i].CheckTopBird() == allBranches[j].CheckTopBird())
                    return;

            }
        }
        m_gc.SetGameOverState(true);
    }
    #endregion

}
