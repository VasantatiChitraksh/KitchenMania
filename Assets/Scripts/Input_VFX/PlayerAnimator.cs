using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";

    private Animator animator;
    [SerializeField] private Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (player == null)
        {
            if (player == null)
            {
                Debug.LogError("Player script not found! Assign it in the Inspector.");
            }
        }
    }

    private void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is not assigned!");
            return;
        }

        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
