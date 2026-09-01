using UnityEngine;
using DG.Tweening;

public class BaseBird : MonoBehaviour
{
    public BaseBranch currentBranch;
    public int ID;
    public float moveSpeed;
    public void MoveTo(Vector3 targetPosition, System.Action onMoveCompleted = null)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        float moveDuration = distance / moveSpeed;
        transform.DOMove(targetPosition, moveDuration).OnComplete(() =>
        {
            onMoveCompleted?.Invoke();
        });
    }
    

}
