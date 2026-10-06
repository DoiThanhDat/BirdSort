using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;
using JetBrains.Annotations;

public class GameDesignerWithJSON : MonoBehaviour
{
    public GameObject ghostPrefab;
    public BaseBranch branches;
    public List<Transform> branchPointRight;
    public List<Transform> branchPointLeft;
    public Transform pointRightInfor;
    public Transform pointLeftInfor;
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
        branchesOnActive.Clear();
        int currentLevelIndex = PlayerPrefs.GetInt("currentLevel", 1);
        TextAsset jsonLevelFile = Resources.Load<TextAsset>("Levels/level_" + currentLevelIndex);
        if (jsonLevelFile == null)
        {
            currentLevelIndex = 1;
            PlayerPrefs.SetInt("currentLevel", 1);
            jsonLevelFile = Resources.Load<TextAsset>("Levels/level_1");
            if (jsonLevelFile == null)
                return;
        }
        LevelDataJSON levelData = JsonUtility.FromJson<LevelDataJSON>(jsonLevelFile.text);

        // Quet cau hinh tu Level Data from JSON
        for (int i = 0; i < levelData.branchListRight.Count; i++)
        {
            Transform targerPos = branchPointRight[i];
            if (targerPos == null) continue;

            BranchSetUpJSON setUp = levelData.branchListRight[i];
            GameObject newGhost = PoolingObject.Instance.GetObject(ghostPrefab, pointRightInfor);
            BaseBranch newBranch = PoolingObject.Instance.GetObject(branches.gameObject, branchPointRight[i]).GetComponent<BaseBranch>();
            newBranch.ghostOfBranches = newGhost;
            branchesOnActive.Add(newBranch);
            newBranch.transform.position = newGhost.transform.position;
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
                BaseBird newBird = PoolingObject.Instance.GetObject(birdReadyToSPawn.gameObject, newBranch.transform).GetComponent<BaseBird>();
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
            GameObject newGhost = PoolingObject.Instance.GetObject(ghostPrefab, pointLeftInfor);
            BaseBranch newBranch = PoolingObject.Instance.GetObject(branches.gameObject, branchPointLeft[i]).GetComponent<BaseBranch>();
            newBranch.ghostOfBranches = newGhost;
            branchesOnActive.Add(newBranch);
            newBranch.transform.position = newGhost.transform.position;
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
                BaseBird newBird = PoolingObject.Instance.GetObject(birdReadyToSPawn.gameObject, newBranch.transform).GetComponent<BaseBird>();
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
            if (branches != null && branches.gameObject.activeInHierarchy && branches.birds.Count == 0)
                return;
        }
        for (int i = 0; i < branchesOnActive.Count; i++)
        {
            if (branchesOnActive[i] == null && !branchesOnActive[i].gameObject.activeInHierarchy)
                continue;
            for (int j = 0; j < branchesOnActive.Count; j++)
            {
                if (i == j || branchesOnActive[j] == null || !branchesOnActive[j].gameObject.activeInHierarchy)
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

    #region Check All Bird Cleared
    public bool CheckAllBirdCleared()
    {
        foreach (BaseBranch branch in branchesOnActive)
        {
            if (branch!= null && branch.birds.Count >0)
            {
                return false;
            }
        }
        return true;
    }
    #endregion

    #region End Level
    public void EndLevel(System.Action callback = null)
    {
       for (int i = 0; i< branchesOnActive.Count; i++)
        {
            BaseBranch branchAvailable = branchesOnActive[i];
            if (branchAvailable != null && branchAvailable.gameObject.activeInHierarchy && !branchAvailable.isBreaking)
            {
                branchAvailable.isBreaking = true;
                branchAvailable.PlayBroken(() =>
                {
                    callback?.Invoke();
                    branchAvailable.ReturnToPool();
                });
            }
            else
            {
                branchAvailable.PlayBroken(() => 
                {
                    branchAvailable.ReturnToPool();
                });
            }
        }
       if (callback != null)
        {
            callback.Invoke();
        }
    }
    #endregion

    #region Shuffle Button
    public void ShuffleButton()
    {
        if (m_gc.IsGameOver() || m_gc.SetGameFinishedState()) return;
        foreach (BaseBranch branch in branchesOnActive)
        {
            if (branch != null || !branch.gameObject.activeInHierarchy) continue;
            foreach (BaseBird bird in branch.birds)
            {
                if (bird != null && bird.GetStatus())
                    return;
            }
        }
        if (BranchTest.selectedBranch != null)
        {
            m_gc.TurnHighLight(BranchTest.selectedBranch, false);
            BranchTest.selectedBranch = null;
        }

        bool hasShuffled = false;
        foreach (BaseBranch branch in branchesOnActive)
        {
            if(branch == null || !branch.gameObject.activeInHierarchy) continue;
            if (branch.isBreaking || branch.birds.Count <= 1 || !branch.IsBranchCanClick()) continue;
            hasShuffled = true;
            branch.ShuffleBird(() =>
            {
                CheckGameOver();
                m_gc.CheckIsGameFinished();
            });
        }
    }
    #endregion

   
}
