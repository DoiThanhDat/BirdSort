using DG.Tweening;
using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameDesignerWithJSON : MonoBehaviour
{
    public GameObject ghostPrefab;
    public BaseBranch branches;
    public List<Transform> branchPointRight;
    public List<Transform> branchPointLeft;
    public Transform pointRightInfor;
    public Transform pointLeftInfor;
    public Transform pointRight;
    public Transform pointLeft;

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
        m_gc.UndoHistoryClear();
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
        List<BaseBranch> remainingBranches = new List<BaseBranch>();
        for (int i = 0;i < branchesOnActive.Count;i++)
        {
            BaseBranch branchAvailable = branchesOnActive[i];
            if (branchAvailable.gameObject.activeInHierarchy && !branchAvailable.isBreaking)
            {
                branchAvailable.isBreaking = true;
                remainingBranches.Add(branchAvailable);
            }
        }
        if(remainingBranches.Count == 0)
        {
            callback?.Invoke();
            return;
        }
        int completedCount = 0;
        for (int i = 0; i < remainingBranches.Count; i++)
        {
            float delayTime = i * 0.35f;
            BaseBranch branchAvailable = remainingBranches[i];
            DOVirtual.DelayedCall(delayTime, () => 
            {
                branchAvailable.PlayBroken(() => 
                {
                    branchAvailable.ReturnToPool();
                    completedCount++;
                    if (completedCount == remainingBranches.Count)
                    {
                        callback?.Invoke();
                    }
                });
            });
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
        m_gc.UndoHistoryClear();
        if (BranchTest.selectedBranch != null)
        {
            m_gc.TurnHighLight(BranchTest.selectedBranch, false);
            BranchTest.selectedBranch = null;
        }
        foreach (BaseBranch branch in branchesOnActive)
        {
            //bool hasShuffled = false;
            if (branch == null || !branch.gameObject.activeInHierarchy) continue;
            if (branch.isBreaking || branch.birds.Count <= 1 || !branch.IsBranchCanClick()) continue;
            //hasShuffled = true;
            branch.ShuffleBird(() =>
            {
                
            });
        }
    }
    #endregion

    #region Add Branch Button
    public void AddBranchButton()
    {
        //kiem tra dieu kien
        if (m_gc.IsGameOver() || m_gc.SetGameFinishedState()) return;
        foreach (BaseBranch branch in branchesOnActive)
        {
            if (branch == null || !branch.gameObject.activeInHierarchy) continue;
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
        //Dem so luong canh con hoat dong
        int activeRightCount = 0;
        int activeLeftCount = 0;
        foreach (BaseBranch branch in branchesOnActive)
        {
            if (branch != null && branch.gameObject.activeInHierarchy && !branch.isBreaking)
            {
                if (branch.isRightBranch) activeRightCount++;
                else activeLeftCount++;
            }
        }
        if (activeLeftCount >= 11 && activeRightCount >= 11)
        {
            Debug.Log("full canh, stop");
            return;
        }
        bool spawnOnRight;
        if (activeRightCount < activeLeftCount) spawnOnRight = true;
        else if (activeRightCount > activeLeftCount) spawnOnRight = false;
        else
        {
            spawnOnRight = (activeRightCount < 9);
        }

        List<Transform> targetPointList = spawnOnRight ? branchPointRight : branchPointLeft;
        Transform availablePoint = null;
        for (int i = 0; i < targetPointList.Count; i++)
        {
            Transform pt = targetPointList[i];
            if (pt == null) continue;
            if (pt.childCount == 0)
            {
                availablePoint = pt;
                break;
            }
            else
            {
                bool hasActiveChild = false;
                for (int j = 0; j < pt.childCount; j++)
                {
                    if (pt.GetChild(j).gameObject.activeInHierarchy)
                    {
                        hasActiveChild = true;
                        break;
                    }
                }
                if (!hasActiveChild)
                {
                    availablePoint = pt;
                    break;
                }
            }
        }
        if (availablePoint == null)
        {
            Debug.LogWarning("Không tìm thấy điểm neo trống trên Scene!");
            return;
        }
        availablePoint.gameObject.SetActive(true);

        //Sinh bong ma moi va ep cap nhat vi tri
        Transform targetInfor = spawnOnRight ? pointRightInfor : pointLeftInfor;
        GameObject newGhost = PoolingObject.Instance.GetObject(ghostPrefab, targetInfor);
 
        //Lay canh cay va chuan bi spawn 
        BaseBranch newBranch = PoolingObject.Instance.GetObject(branches.gameObject, availablePoint).GetComponent<BaseBranch>();
        newBranch.ghostOfBranches = newGhost;
        branchesOnActive.Add(newBranch);
        newBranch.birds.Clear();

        if (spawnOnRight)
        {
            newBranch.isRightBranch = true;
            newBranch.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            newBranch.isRightBranch = false;
            newBranch.transform.localScale = Vector3.one;
        }

        UpdateBranchScale();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(targetInfor as RectTransform);
        //canh cay chui tu man hinh ra
        float offScreenDistance = 12f;
        Vector3 startPos = newGhost.transform.position + (spawnOnRight? Vector3.right: Vector3.left) * offScreenDistance;
        newBranch.transform.position = startPos;
    }
    #endregion

    #region Update Branch Scale
    public void UpdateBranchScale()
    {
        int activeRightCount = 0;
        int activeLeftCount = 0;
        foreach (BaseBranch branchToCheckScale in branchesOnActive)
        {
            if (branchToCheckScale!=null && branchToCheckScale.gameObject.activeInHierarchy && !branchToCheckScale.isBreaking)
            {
                if (branchToCheckScale.isRightBranch) activeRightCount++;
                else activeLeftCount++;
            }
        }

        float defaultScale = 4f;
        float reduceStep = 0.15f;

        float rightBranchScale = defaultScale;
        if (activeRightCount > 6)
        {
            rightBranchScale = defaultScale - (activeRightCount - 5) * reduceStep;
        }
        Vector3 newRightScale = new Vector3(rightBranchScale, rightBranchScale, rightBranchScale);
        pointRightInfor.localScale = newRightScale;
        pointRight.localScale = newRightScale;

        float leftBranchScale = defaultScale;
        if (activeLeftCount > 6)
        {
            leftBranchScale = defaultScale - (activeLeftCount - 5) * reduceStep;
        }
        Vector3 newLeftScale = new Vector3(leftBranchScale, leftBranchScale, leftBranchScale);
        pointLeftInfor.localScale = newLeftScale;
        pointLeft.localScale = newLeftScale;
    }
    #endregion
}
