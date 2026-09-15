using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;
using JetBrains.Annotations;

public class GameDesignerDemo2 : MonoBehaviour
{
    public LevelData currentLevel;
    public BirdCatalog catalog;
    public BaseBranch branches;
    public List<Transform> branchPoint;
    [SerializeField] private List<BaseBranch> branchesOnActive;
    BranchTest m_gc;

    #region Start && Awake
    private void Awake()
    {
        m_gc = FindFirstObjectByType<BranchTest>();
    }
    void Start()
    {
        SpawnLevel();
    }
    #endregion

    #region Spawn Level
    public void SpawnLevel()
    {
        if (currentLevel == null || catalog == null || branches == null)
        {
            return;
        }
        // Quet cau hinh tu Level Data
        for (int i = 0;i < currentLevel.branchLists.Count;i++)
        {
            if (i >= branchPoint.Count) break;
            Transform targerPos = branchPoint[i];
            if (targerPos == null) continue;
            BranchSetUp setUp = currentLevel.branchLists[i];

            //Spawn canh cay:
            BaseBranch newBranch = Instantiate(branches, targerPos, false);
            newBranch.transform.localPosition = Vector3.zero;
            branchesOnActive.Add(newBranch);

            //Ep canh UI nam giua diem neo
            RectTransform branchRect = newBranch.GetComponent<RectTransform>();
            if (branchRect != null) branchRect.localPosition = Vector3.zero;
            newBranch.birds.Clear();
            //thiet lap quay
            if (setUp.side == 1)
            {
                newBranch.isRightBranch = true;
                newBranch.transform.localScale = new Vector3 (-1,1,1);
            }
            else
            {
                newBranch.isRightBranch= false;
                newBranch.transform.localScale = Vector3.one;
            }

            //Spawn chim
            for (int j = 0;j < setUp.slotID.Count;j++)
            {
               int birdID = setUp.slotID[j];
                if (birdID <= 0) continue;
                BaseBird birdReadyToSPawn = catalog.GetBirdsByID(birdID);
                //sinh chim
                BaseBird newBird = Instantiate(birdReadyToSPawn, newBranch.transform, false) ;
                //gan WorldPos trong Canvas 
                if (j < newBranch.birdPositionInBranch.Count)
                {
                    newBird.transform.position = newBranch.birdPositionInBranch[j].position;
                }
                newBranch.AddBird(newBird);
                newBird.SetFacing(1f);
            }
        }
    }
    #endregion

    #region Check Game Over
    public void CheckGameOver()
    {
        foreach(BaseBranch branches in branchesOnActive)
        {
            if (branches != null && branches.birds.Count == 0)
                return;
        }
        for (int i = 0; i < branchesOnActive.Count;i++)
        {
            for (int j = 0;j < branchesOnActive.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }
                if (branchesOnActive[i].CheckTopBird() == branchesOnActive[j].CheckTopBird())
                {
                    return;
                }
            }
        }
        m_gc.SetGameOverState(true);
    }
    #endregion

    #region End Level
    public void EndLevel()
    {
        foreach (BaseBranch branch in branchesOnActive)
        {
            branch.PlayBroken();
        }
    }
    #endregion

}
