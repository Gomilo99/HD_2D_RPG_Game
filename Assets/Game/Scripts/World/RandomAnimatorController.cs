using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string randomParam = "RandomParam";
    [Range(0, 100)]
    [SerializeField] private int seed = 0;
    [SerializeField] private int minValue = 0;
    [SerializeField] private int maxValue = 99;
    [SerializeField] private float minInterval = 0.5f;
    [SerializeField] private float maxInterval = 1.5f;

    private Coroutine randomRoutine;
    private System.Random rng;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        rng = new System.Random(seed);
    }

    private void OnValidate()
    {
        if (minValue > maxValue)
        {
            int temp = minValue;
            minValue = maxValue;
            maxValue = temp;
        }

        if (minInterval < 0.0f)
        {
            minInterval = 0.0f;
        }

        if (maxInterval < minInterval)
        {
            maxInterval = minInterval;
        }
    }

    private void OnEnable()
    {
        if (randomRoutine == null)
        {
            randomRoutine = StartCoroutine(RandomizeLoop());
        }
    }

    private void OnDisable()
    {
        if (randomRoutine != null)
        {
            StopCoroutine(randomRoutine);
            randomRoutine = null;
        }
    }

    private IEnumerator RandomizeLoop()
    {
        while (true)
        {
            if (animator != null)
            {
                int value = NextIntInclusive(minValue, maxValue);
                animator.SetFloat(randomParam, value);
            }

            float wait = NextFloat(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);
        }
    }

    private int NextIntInclusive(int minInclusive, int maxInclusive)
    {
        if (rng == null)
        {
            rng = new System.Random(seed);
        }

        return rng.Next(minInclusive, maxInclusive + 1);
    }

    private float NextFloat(float minInclusive, float maxInclusive)
    {
        if (rng == null)
        {
            rng = new System.Random(seed);
        }

        double sample = rng.NextDouble();
        return minInclusive + (float)sample * (maxInclusive - minInclusive);
    }
}
