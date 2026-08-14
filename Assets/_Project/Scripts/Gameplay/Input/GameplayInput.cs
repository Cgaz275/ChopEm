using UnityEngine;

public class GameplayInput : MonoBehaviour
{
    [SerializeField] private TreeController treeController;

    private void Awake()
    {
        Debug.Assert(treeController != null, "GameplayInput requires a TreeController reference.", this);
    }

    public void ChopLeft()
    {
        TryChop(ChopSide.Left);
    }

    public void ChopRight()
    {
        TryChop(ChopSide.Right);
    }

    private void TryChop(ChopSide side)
    {
        if (GameManager.Instance.CurrentState != GameState.Gameplay) return;

        if (treeController.Chop(side))
        {
            GameManager.Instance.AddScore();
            return;
        }

        GameManager.Instance.TriggerGameOver();
    }
}
