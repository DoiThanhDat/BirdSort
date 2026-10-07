using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BaseBranch : MonoBehaviour, IPointerDownHandler
{
    public int side;
    public int capacity;
    public List<BaseBird> birds = new List<BaseBird> ();
    public bool isRightBranch;
    BranchTest m_gc;
    [SerializeField] protected SkeletonGraphic body;
    public SkeletonGraphic Body => body;
    public const string BRANCHBROKEN = "Brach1";
    public List<Transform> birdPositionInBranch = new List<Transform> ();
    public List<Transform> slotToGround = new List<Transform>();
    public bool isBreaking = false;
    public GameObject ghostOfBranches;


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
    void LateUpdate()
    {
        if (ghostOfBranches != null && ghostOfBranches.activeInHierarchy)
        {
            transform.position = Vector3.Lerp(transform.position, ghostOfBranches.transform.position, Time.deltaTime * 20f);
        }
    }
    #endregion

    #region Rotate Branch
    public void BranchRotation(System.Action callback = null)
    {
        transform.DOKill();
        transform.DOShakeRotation(duration: 1f, strength: new Vector3(0, 0, 1f), vibrato: 4, randomness: 90, fadeOut: false).OnComplete(() =>
        {
            transform.localRotation = Quaternion.identity;
            callback?.Invoke();
        });
    }
    #endregion

    #region Play Anim
    public void PlayBroken(System.Action callback = null)
    {
        if (body != null && !string.IsNullOrEmpty(BRANCHBROKEN))
        {
            var trackEntry = body.AnimationState.SetAnimation(0, BRANCHBROKEN, false);
            trackEntry.Complete += (track) =>
            {
                callback?.Invoke();
            };
        }
        else
        {
            callback?.Invoke();
        }
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
            birdRect.localPosition = GetSlotPosition(i);
        }
    }

    #endregion

    #region On Pointer Down
    public void OnPointerDown(PointerEventData evenData)
    {
        if (m_gc != null)
        {
            m_gc.OnClickedBranch(this);
        }
    }
    #endregion

    #region Escape Position
    public Vector3 EscapePos()
    {
        float randomPos = UnityEngine.Random.Range(-9f, 9f);
        float xPos = randomPos;
        float yPos = 6f;
        Vector3 EscapePoint = new Vector3(xPos, yPos, 0f);
        return EscapePoint;
    }
    #endregion

    #region Check Point 
    public void CheckPoint()
    {
        if (birds.Count < capacity)
            return;
        foreach (BaseBird bird in birds)
        {
            if (bird.GetStatus() == true) return;
        }    

        int demCungMau = 0;
        for (int i = birds.Count - 1; i > 0; i--)
        {
            if (birds[birds.Count - 1].ID == birds[i - 1].ID)
            {
                GameObject parentPoint = transform.parent.gameObject;
                demCungMau++;
                if (demCungMau == capacity - 1)
                {
                    isBreaking = true;
                    PlayBroken();
                    foreach (BaseBird bird in birds)
                    {
                        bird.Escape(EscapePos(), () =>
                        {
                            bird.transform.SetParent(null);
                            parentPoint.SetActive(false);
                            //PoolingObject.Instance.ReturnObject(gameObject);
                            ReturnToPool();
                        });
                    }
                    birds.Clear();
                    m_gc.ScoreIncrement();
                }
            }
            else
                break;
        }

    }
    #endregion

    #region Get Slot Position
    public Vector3 GetSlotPosition(int i)
    {
        if (i >= 0 && i <capacity )
        {
            Transform slotTransform = birdPositionInBranch[i];
            if (slotTransform != null)
            {
                return transform.InverseTransformPoint(slotTransform.position);
            }
        }
        return Vector3.zero;
    }
    #endregion

    //slot de dap dat
    #region Get Slot Position To Ground
    public Vector3 GetSlotPositionToGround(int i)
    {
        if (i >= 0 && i < capacity)
        {
           RectTransform rt = slotToGround[i] as RectTransform;
            if (rt != null) return rt.anchoredPosition;
        }
        return Vector3.zero;
    }
    #endregion

    #region Slot To Ground World
    public Vector3 GetGhostSlotToGroundWorld(int i)
    {
        if (i >= 0 && i < capacity)
        {
            Canvas.ForceUpdateCanvases();

            Transform slot = slotToGround[i];
            if (ghostOfBranches != null && ghostOfBranches.activeInHierarchy)
            {
                Vector3 offSet = slot.position - transform.position;

                return ghostOfBranches.transform.position + offSet;
            }
            return slot.position;

        }
        return Vector3.zero;
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

    #region On Enable
    void OnEnable()
    {
        birds.Clear();
        transform.localRotation= Quaternion.identity;
        isBreaking = false;

        //reset animation
        body.AnimationState.ClearTracks();
        body.Skeleton.SetToSetupPose();
    }
    #endregion

    #region Return To Pool
    public void ReturnToPool()
    {
        if (ghostOfBranches != null)
        {
            PoolingObject.Instance.ReturnObject(ghostOfBranches);
            ghostOfBranches = null;
        }
        PoolingObject.Instance.ReturnObject(gameObject);
    }
    #endregion

    #region Is Branch Can CLick
    public bool IsBranchCanClick()
    {
        if (this.birds.Count == capacity)
        {
            int demCungMau = 0;
            for (int i = birds.Count - 1; i > 0; i--)
            {
                if (birds[birds.Count - 1].ID == birds[i - 1].ID)
                {
                    demCungMau++;
                    if (demCungMau == capacity - 1)
                    {
                        return false;
                    }
                }
            }
        }
         return true;
    }
    #endregion

    #region Shuffle Bird
    public void ShuffleBird(System.Action callback = null)
    {
        if (birds == null || birds.Count <= 1 || isBreaking)
        {
            callback?.Invoke();
            return;
        }

        for (int i = birds.Count - 1; i > 0; i-- )
        {
            int randomSlot = UnityEngine.Random.Range(0, i+ 1);
            BaseBird temp = birds[i];
            birds[i]= birds[randomSlot];
            birds[randomSlot]= temp;
        }
        int totalBirds = birds.Count;
        for (int i = 0; i < totalBirds; i++)
        {
            BaseBird bird = birds[i];
            RectTransform birdRect = bird.GetComponent<RectTransform>();
            birdRect.DOKill();
            Vector3 newPosToGround = GetSlotPositionToGround(i);
            Vector3 newPosInBranch = GetSlotPosition(i);
            float branchScaleX = this.transform.localScale.x;
            float startPosX = bird.transform.position.x;
            float targetPosX = transform.TransformPoint(newPosInBranch).x;
            float flyDirection = (targetPosX >= startPosX) ? 1f : -1f;
            bird.SetFacing(flyDirection * branchScaleX);
            bird.BoosterMovement(newPosToGround, () =>
            {
                float landDirection = isRightBranch ? -1f : 1f;
                bird.SetFacing(landDirection * branchScaleX);
                bird.GroundingAfterMoveMent(newPosInBranch, callbak: () =>
                {
                    BranchRotation();
                });
            });
        }
    }
    #endregion
}
