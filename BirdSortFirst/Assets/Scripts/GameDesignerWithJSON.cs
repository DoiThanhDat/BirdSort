using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;
using JetBrains.Annotations;

public class GameDesignerWithJSON : MonoBehaviour
{
    public BaseBranch branches;
    public List<Transform> branchPointRight;
    public List<Transform> branchPointLeft;
    [SerializeField] private List<BaseBranch> branchesOnActive;
    BranchTest m_gc;

    #region Start && Awake
    private void Awake()
    {
        m_gc = FindFirstObjectByType<BranchTest>();
    }
    void Start()
    {
        SpawnLevelFromJSON();
    }
    #endregion

    #region Spawn Level
    public void SpawnLevelFromJSON()
    {
        if (branches == null)
        {
            return;
        }
        int currentLevelIndex = PlayerPrefs.GetInt("currentLevel", 1);
        TextAsset jsonLevelFile = Resources.Load<TextAsset>("Levels/level_" + currentLevelIndex);
        LevelDataJSON levelData = JsonUtility.FromJson<LevelDataJSON>(jsonLevelFile.text);

        // Quet cau hinh tu Level Data from JSON
        for (int i = 0; i < levelData.branchListRight.Count; i++)
        {
            Transform targerPos = branchPointRight[i];
            if (targerPos == null) continue;
            BranchSetUpJSON setUp = levelData.branchListRight[i];

            BaseBranch newBranch = Instantiate(branches, targerPos, false);
            newBranch.transform.localPosition = Vector3.zero;
            branchesOnActive.Add(newBranch);
            RectTransform branchRect = newBranch.GetComponent<RectTransform>();
            if (branchRect != null) branchRect.localPosition = Vector3.zero;
            newBranch.birds.Clear();

            if (setUp.side == 1)
            {
                newBranch.isRightBranch = true;
                newBranch.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                newBranch.isRightBranch = false;
                newBranch.transform.localScale = Vector3.one;
            }

            for (int j = 0; j < setUp.slotID.Count; j++)
            {
                int birdID = setUp.slotID[j];
                if (birdID <= 0) continue;
                BaseBird birdReadyToSPawn = Resources.Load<BaseBird>("Birds/Bird_" + birdID);
                //sinh chim
                BaseBird newBird = Instantiate(birdReadyToSPawn, newBranch.transform, false);
                //gan WorldPos trong Canvas 
                if (j < newBranch.birdPositionInBranch.Count)
                {
                    newBird.transform.position = newBranch.birdPositionInBranch[j].position;
                }
                newBranch.AddBird(newBird);
                newBird.SetFacing(1f);
            }
        }
        for (int i = 0; i < levelData.branchListLeft.Count; i++)
        {
            Transform targerPos = branchPointLeft[i];
            if (targerPos == null) continue;
            BranchSetUpJSON setUp = levelData.branchListLeft[i];

            BaseBranch newBranch = Instantiate(branches, targerPos, false);
            newBranch.transform.localPosition = Vector3.zero;
            branchesOnActive.Add(newBranch);
            RectTransform branchRect = newBranch.GetComponent<RectTransform>();
            if (branchRect != null) branchRect.localPosition = Vector3.zero;
            newBranch.birds.Clear();

            if (setUp.side == 1)
            {
                newBranch.isRightBranch = true;
                newBranch.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                newBranch.isRightBranch = false;
                newBranch.transform.localScale = Vector3.one;
            }

            for (int j = 0; j < setUp.slotID.Count; j++)
            {
                int birdID = setUp.slotID[j];
                if (birdID <= 0) continue;
                BaseBird birdReadyToSPawn = Resources.Load<BaseBird>("Birds/Bird_" + birdID);
                //sinh chim
                BaseBird newBird = Instantiate(birdReadyToSPawn, newBranch.transform, false);
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
        foreach (BaseBranch branches in branchesOnActive)
        {
            if (branches != null && branches.birds.Count == 0)
                return;
        }
        for (int i = 0; i < branchesOnActive.Count; i++)
        {
            for (int j = 0; j < branchesOnActive.Count; j++)
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
