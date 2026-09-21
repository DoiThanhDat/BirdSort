using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.Port;

public class BranchTest : MonoBehaviour
{
    [SerializeField] Canvas gamePlayCanvas;
    public static BaseBranch selectedBranch = null;
    public bool isMoving = false;
    int m_score;
    bool m_isGameOver;
    bool isGameFinished;
    UIManager m_ui;
    GameDesignerWithJSON m_gdJSON;

    private void Awake()
    {
        m_gdJSON = FindAnyObjectByType<GameDesignerWithJSON>();
    }
    void Start()
    {
        m_ui = FindAnyObjectByType<UIManager>();
        m_ui.SetScoreText("Score: " + m_score);
        
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
        if (IsGameOver() || SetGameFinishedState() || isMoving)
            return;
        if (selectedBranch == null)
        {
            if (clickedBranch.birds.Count > 0)
            {
                selectedBranch = clickedBranch;
            }
        }
        else if (selectedBranch == clickedBranch)
        {
            selectedBranch = null;
        }
       else
        {
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
            //isMoving = true;
            int completedCount = 0;
            Sequence masterSequence = DOTween.Sequence();
            for (int i = 0; i < birdsToEmptySlot; i++)
            {
                BaseBird birdToMove = sourceBranch.birds[sourceBranch.birds.Count - 1];
                sourceBranch.RemoveBird(birdToMove);
                int targetSlotIndex = targetBranch.birds.Count ;
                Vector3 targetPosToGround = targetBranch.GetSlotPositionToGround(targetSlotIndex);

                Vector3 targetPos = targetBranch.GetSlotPosition(targetSlotIndex); // gan xuong dat
                Vector3 targetWolrdPos = targetBranch.transform.TransformPoint(targetPos);

                birdToMove.transform.SetParent(targetBranch.transform, true);
                birdToMove.transform.localScale = Vector3.one;
                targetBranch.AddBird(birdToMove);

                if (birdToMove.transform.position.x <= targetWolrdPos.x)
                {
                    if(targetBranch.isRightBranch)
                    {
                        birdToMove.SetFacing(-1f);
                    }
                    if(targetBranch.isRightBranch == false)
                    {
                        birdToMove.SetFacing(1f);
                    }
                }
                if (birdToMove.transform.position.x > targetWolrdPos.x)
                {
                    if (targetBranch.isRightBranch)
                    {
                        birdToMove.SetFacing(1f);
                    }
                    if (targetBranch.isRightBranch == false)
                    {
                        birdToMove.SetFacing(-1f);
                    }
                }
                birdToMove.MoveTo(targetPosToGround, () =>
                {
                    //birdToMove.SetFacing(1f);
                    birdToMove.GroundingAfterMoveMent(targetPos,callbak: () =>
                    {
                        birdToMove.SetFacing(1f);
                        completedCount++;
                        if (completedCount == birdsToEmptySlot)
                        {
                            targetBranch.BranchRotation(callback: () =>
                            {
                                targetBranch.CheckPoint();
                                m_gdJSON.CheckGameOver();
                                CheckIsGameFinished();  
                            }
                            );
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
    /*
    public void DestroyAllBranches()
    {
        if (isGameFinished == true)
        {
            m_gdJSON.EndLevel();
        }
    }*/

    #region Set diem, Set game over
    public void SetScore(int value)
        { m_score = value; }
    public int GetScore()
        { return m_score; }
    public void ScoreIncrement()
    {
        m_score++;
        m_ui.SetScoreText("Score: " + m_score);
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
}