using DG.Tweening;
using Spine.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BaseBird : MonoBehaviour
{
    public BaseBranch currentBranch;
    RectTransform recttr;
    public int ID;
    public float moveSpeed;
    public const string FLY = "fly";
    public const string GROUNDING = "grounding";
    public const string IDLE = "idle";

    [SerializeField] protected SkeletonGraphic body;
    public SkeletonGraphic Body => body;

    private void Awake()
    {
        recttr = GetComponent<RectTransform>();
    }
    private void Start()
    {
        PlayIdle();
    }

    #region PLay Anim
    public void PlayIdle()
    {
        if (body != null && !string.IsNullOrEmpty(IDLE))
        {
            body.AnimationState.SetAnimation(0, IDLE, true);
        }
    }

    public void PlayFly()
    {
        if (body != null && !string.IsNullOrEmpty(FLY))
        {
            body.AnimationState.SetAnimation(0, FLY, true);
        }
    }

    public void PlayGrounding()
    {
        if (body != null && !string.IsNullOrEmpty(FLY))
        {
            body.AnimationState.AddAnimation(0, GROUNDING, false, 0f);
            body.AnimationState.AddAnimation(0, IDLE, true, 0f);
        }
        else
        {
            PlayIdle();
        }
    }
    #endregion

    #region Move To
   public void MoveTo(Vector3 targetPosition, System.Action onMoveCompleted = null)
    {
        if (recttr == null) recttr = GetComponent<RectTransform>();
        recttr.DOKill();
        PlayFly();
        float distance = Vector3.Distance(recttr.anchoredPosition, targetPosition);
        float moveDuration = distance / moveSpeed;
        recttr.DOAnchorPos(targetPosition, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            onMoveCompleted?.Invoke();
        });
    }
    #endregion

   public void GroundingAfterMoveMent(Vector3 pos, System.Action callbak = null)
    {
        recttr.DOKill();
        if (recttr == null) recttr = GetComponent<RectTransform>();
        PlayGrounding();
        recttr.DOLocalMove(pos, 1.167f).OnComplete(() =>
        {
            callbak?.Invoke();
        });
    }

    #region Escape
    public void Escape(Vector3 targetPosition, System.Action onMoveCompleted = null)
    {
        if (recttr == null) recttr = GetComponent<RectTransform>();
        recttr.DOKill();
        PlayFly();
        float distance = Vector3.Distance(transform.position, targetPosition);
        float moveDuration = distance / moveSpeed;
        recttr.DOMove(targetPosition, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            onMoveCompleted?.Invoke();
            Destroy(gameObject);
        });
    }
    #endregion

    #region Look at target branch
    public void SetFacing(float direct)
    {
        if (body != null && body.Skeleton != null)
        {
            body.Skeleton.ScaleX = direct;
        }
    }
    public void FlipFacing()
    {
        if (body !=null && body.Skeleton != null)
        {
            body.Skeleton.ScaleX *= -1f;
        }
    }
    #endregion

    #region Change & Set Skin
    public void ChangeSkin(string skinName)
    {
        if (body == null || body.Skeleton == null) //Tranh loi
            return;
        body.Skeleton.SetSkin(skinName);
        body.Skeleton.SetSlotsToSetupPose();
        body.LateUpdate();
    }
    public void SetSkinByID(int index)
    {
        this.ID = index;
        string birdSkin = $"skin-{index}";
        ChangeSkin(birdSkin);
    }
    #endregion
}
