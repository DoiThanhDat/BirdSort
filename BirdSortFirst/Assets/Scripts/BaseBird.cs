using UnityEngine;
using DG.Tweening;
using Spine.Unity;

public class BaseBird : MonoBehaviour
{
    public BaseBranch currentBranch;
    public int ID;
    public float moveSpeed;
    public const string FLY = "fly";
    public const string GROUNDING = "ground";
    public const string IDLE = "idle";

    [SerializeField] protected SkeletonGraphic body;
    public SkeletonGraphic Body => body;

    private void Start()
    {
        PlayIdle();
    }
    #region Idle, Fly && Grounding
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
        transform.DOKill();
        if (body != null && body.Skeleton != null)
        {
            body.Skeleton.ScaleX = (targetPosition.x < transform.position.x) ? -1f : 1f;
        }    
        PlayFly();
        float distance = Vector3.Distance(transform.position, targetPosition);
        float moveDuration = distance / moveSpeed;
        transform.DOMove(targetPosition, moveDuration).OnComplete(() =>
        {
            PlayGrounding();
            onMoveCompleted?.Invoke();
        });
    }
    #endregion

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
}
