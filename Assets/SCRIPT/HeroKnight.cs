using UnityEngine;

public class HeroKnight : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TriggerHurt()
    {
        if (animator != null) animator.SetTrigger("Hurt");
    }

    public void TriggerDeath()
    {
        if (animator != null) animator.SetTrigger("Death");
    }
}
