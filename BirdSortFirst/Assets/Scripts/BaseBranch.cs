using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using DG.Tweening;

public class BaseBranch : MonoBehaviour
{
    public float birdRange;
    public int capacity;
    public List<BaseBird> birds = new List<BaseBird> ();
    public bool isRightBranch;
    BranchTest m_gc;

    #region start && update
    void Awake()
    {
        m_gc = FindAnyObjectByType<BranchTest>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Update Bird Position
    public void UpdateBirdPosition()
    {
        if (birds == null || birds.Count == 0) return;
        for (int i = 0; i < birds.Count; i++)
        {
            if (birds[i] == null) continue;
            RectTransform birdRect = birds[i].GetComponent<RectTransform>();
            birdRect.DOKill();
            birdRect.anchoredPosition = GetSlotPosition(i);
            /*if (isRightBranch == false)
            {
                float yPos = (float)( + 10f);
                float xPos = (float)( - 35f + i * birdRange);
                Vector3 birdPosition = new Vector3(xPos, yPos, 0f);
                birds[i].transform.localPosition = birdPosition;
            }
            else if (isRightBranch == true)
            {
                 float yPos = (float)( + 10f);
                float xPos = (float)(( + (-i) * birdRange));
                Vector3 birdPosition = new Vector3(xPos, yPos, 0f);
                birds[i].transform.localPosition = birdPosition;
            }*/
        }
    }

    #endregion

    #region On Mouse Down
    public void OnMouseDown()
    {
       if (m_gc != null)
        {
            m_gc.OnClickedBranch(this);
        }
    }
    #endregion

    #region Check Point
    public void CheckPoint()
    {
        int demCungMau = 0;
        for (int i = birds.Count - 1; i > 0; i--)
        {
            if (birds[birds.Count - 1].ID == birds[i - 1].ID)
            {
                demCungMau++;
                if (demCungMau == capacity - 1)
                {
                    foreach (BaseBird bird in birds)
                    {
                        Destroy(bird.gameObject);
                    }
                    Destroy(gameObject);
                    m_gc.AddCompletedBranch();
                    m_gc.ScoreIncrement();
                }
            }
            m_gc.CheckIsGameFinished(true);
        }

    }
    #endregion

    // (*)Code mới:
    #region Get Slot Position
    public Vector2 GetSlotPosition(int i)
    {
        float yPos = 10f;
        float xPos;
        if (isRightBranch == false)
        {
            {
                xPos = (float)(- 35f + (i * birdRange));
            }
        }
        else
        {
            {
                xPos = (float)( - (i * birdRange));
            }
        }
        Vector2 localSlot = new Vector3(xPos, yPos);
        return localSlot;
    }
    #endregion

    #region Check Top Bird ID
    public int CheckTopBird()
    {
        if (birds.Count == 0)
            return -1;
        else if (birds.Count > 0 && birds.Count <= capacity)
            return birds[birds.Count-1].ID;
        else return -1;
    }
    #endregion

    #region Check Color
    public List<BaseBird> CheckColor()
    {
        List<BaseBird> BirdsToMove = new List<BaseBird>();
        if (birds.Count == 0)
            return BirdsToMove;
        if (birds.Count == 1)
        {
            BirdsToMove.Add(birds[0]);
        }
        if (birds.Count > 1 && birds.Count <= capacity)
        {
            int targetID = birds[birds.Count - 1].ID;
            for (int i = birds.Count - 1; i >= 0; i--)
            {
                if (birds[i].ID == targetID)
                {
                    BirdsToMove.Add(birds[i]);

                }
                else
                    break;
            }
        }
        return BirdsToMove;
    }
    #endregion

    #region Add, Remove Bird
    public void AddBird(BaseBird bird)
    {
        if (!birds.Contains(bird))
        {
            birds.Add(bird);
            bird.currentBranch = this;
        }
    }

    public void RemoveBird(BaseBird bird)
    {
         if (birds.Contains(bird))
         {
                birds.Remove(bird);
         }
    }
    #endregion

}
