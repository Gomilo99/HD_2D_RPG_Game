using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private string velocityParam = "Velocity";
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator animator;
    [SerializeField] private float flipDeadZone = 0.01f;


    private void Reset()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }
        if (rb == null)
        {
            Debug.LogWarning("PlayerAnimationController: No se encontró Rigidbody en este GameObject.", this);
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        Vector3 velocity = rb.linearVelocity;
        float planarSpeed = new Vector2(velocity.x, velocity.z).magnitude;

        if (animator != null)
        {
            animator.SetFloat(velocityParam, planarSpeed);
        }

        if (sr != null && Mathf.Abs(velocity.x) > flipDeadZone)
        {
            sr.flipX = velocity.x < 0.0f;
        }
    }
}