using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BranchTest : MonoBehaviour
{
    [SerializeField] Canvas gamePlayCanvas;
    public static BaseBranch selectedBranch = null;
    int m_score;
    bool m_isGameOver;
    bool isGameFinished;
    UIManager m_ui;
    GameDesignerWithJSON m_gdJSON;
    public Transform flyingLayer;

    private void Awake()
    {
        m_gdJSON = FindAnyObjectByType<GameDesignerWithJSON>();
    }
    void Start()
    {
        m_ui = FindAnyObjectByType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
       if (m_isGameOver)
        {
            m_ui.ShowGameOverPanel(true);
            return;
        }
       
       if (isGameFinished)
        {
            m_ui.ShowWinPanel(true);
            return;
        }
    }

    #region On Mouse Down
    public void OnClickedBranch(BaseBranch clickedBranch)
    {
        if (IsGameOver() || SetGameFinishedState())
            return;
        if (!clickedBranch.IsBranchCanClick())
            return;
        if (selectedBranch == null)
        {
            if (clickedBranch.birds.Count > 0)
            {
                selectedBranch = clickedBranch;
                TurnHighLight(selectedBranch, true);
            }
        }
        else if (selectedBranch == clickedBranch)
        {
            TurnHighLight(selectedBranch, false);
            selectedBranch = null;
        }
       else
        {
            TurnHighLight(selectedBranch, false);
            MoveBirdTo(selectedBranch, clickedBranch);
            selectedBranch = null;
        }
    }
    #endregion

    #region Move Bird To
    public void MoveBirdTo(BaseBranch sourceBranch, BaseBranch targetBranch)
    {
        List<BaseBird> MovinBird = sourceBranch.CheckColor(); //gán List BirdsToMove vừa return ở CheckColor(); Done 
        int emptySlots = targetBranch.capacity - targetBranch.birds.Count;
        int birdsToEmptySlot = Mathf.Min(MovinBird.Count, emptySlots);
        bool canMove = emptySlots > 0 && (targetBranch.birds.Count == 0 || (targetBranch.birds[targetBranch.birds.Count - 1].ID == MovinBird[0].ID));
        if (canMove)
        {
            int completedCount = 0;
            for (int i = 0; i < birdsToEmptySlot; i++)
            {
                // Lay chim de di chuyen
                BaseBird birdToMove = sourceBranch.birds[sourceBranch.birds.Count - 1];
                sourceBranch.RemoveBird(birdToMove);
                // Tinh toan vi tri
                int targetSlotIndex = targetBranch.birds.Count;
                Vector3 targetPosToGround = targetBranch.GetSlotPositionToGround(targetSlotIndex);
                Vector3 targetPos = targetBranch.GetSlotPosition(targetSlotIndex);
                // Tinh toan quay mat
                float startPosX = birdToMove.transform.position.x;
                float targetPosX = targetBranch.transform.TransformPoint(targetPos).x;  
                //Chuyen sang canh moi
                targetBranch.AddBird(birdToMove);
                birdToMove.transform.SetParent(targetBranch.transform, true);
                birdToMove.transform.localScale = Vector3.one;
                //Quay mat
                float flyDirection = (targetPosX > startPosX) ? 1f : -1f;
                float branchScaleX = targetBranch.transform.localScale.x;
                birdToMove.SetFacing(flyDirection * branchScaleX);
                birdToMove.MoveTo(targetPosToGround, () =>
                {
                    float landDirection = targetBranch.isRightBranch? -1f : 1f;
                    birdToMove.SetFacing(landDirection * branchScaleX);
                    targetBranch.BranchRotation();
                    birdToMove.transform.SetParent(targetBranch.transform, true);
                    birdToMove.GroundingAfterMoveMent(targetPos,callbak: () =>
                    {
                        birdToMove.ChangeStatus(false);
                        completedCount++;
                        if (completedCount == birdsToEmptySlot)
                        {
                            //targetBranch.AddBird(birdToMove);
                            targetBranch.CheckPoint();
                            m_gdJSON.CheckGameOver();
                            CheckIsGameFinished();
                        }
                    });
                });
            }   
        }
    }
    #endregion

    #region Check Dieu Kien Thang 

    public void CheckIsGameFinished()
    {
        if (m_gdJSON.CheckAllBirdCleared())
        {
            m_gdJSON.EndLevel(callback: () =>
            {
                isGameFinished = true;                          
            });
        }
    }
    public bool SetGameFinishedState()
    {
        return isGameFinished;
    }
    #endregion

    #region Set diem, Set game over
    public void SetScore(int value)
        { m_score = value; }
    public int GetScore()
        { return m_score; }
    public void ScoreIncrement()
    {
        m_score++;
    }
    public void SetGameOverState(bool state)
    {
        m_isGameOver = state;
    }
    public bool IsGameOver()
    {
        return m_isGameOver;
    }
    #endregion

    #region Highlight
    public void TurnHighLight(BaseBranch branch, bool isOn)
    {
        List<BaseBird> topBirds = branch.CheckColor();
        foreach(BaseBird bird in topBirds)
        {
            bird.SetHighlight(isOn);
        }
    }
    #endregion

   
}