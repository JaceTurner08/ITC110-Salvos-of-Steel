using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D))]
public class SmokeScreenArea : MonoBehaviour
{
    [Header("Smoke Settings")]
    public float fadeInDuration = 2f;

    [Header("Fade Out Settings")]
    public float fadeOutDuration = 2f;
    public float smokeDuration = 10f;

    private CircleCollider2D smokeCircle;
    private SpriteRenderer smokeRenderer;

    private Color originalColor;
    private bool smokeActive = false;

    private static readonly List<SmokeScreenArea> smokeScreens =
        new List<SmokeScreenArea>();

    void Awake()
    {
        smokeCircle = GetComponent<CircleCollider2D>();
        smokeRenderer = GetComponentInChildren<SpriteRenderer>();

        smokeCircle.isTrigger = true;

        if (smokeRenderer != null)
        {
            originalColor = smokeRenderer.color;

            Color color = originalColor;
            color.a = 0f;
            smokeRenderer.color = color;
        }
        else
        {
            Debug.LogError("Smoke Screen is missing a SpriteRenderer!", this);
        }
    }

    void OnEnable()
    {
        if (!smokeScreens.Contains(this))
            smokeScreens.Add(this);
    }

    void Start()
    {
        StartCoroutine(SmokeLifecycle());
    }

    IEnumerator FadeInSmoke()
    {
        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(
                timer / fadeInDuration
            );

            if (smokeRenderer != null)
            {
                Color color = originalColor;
                color.a = originalColor.a * progress;
                smokeRenderer.color = color;
            }

            yield return null;
        }

        if (smokeRenderer != null)
            smokeRenderer.color = originalColor;

        smokeActive = true;

        Debug.Log("SMOKE SCREEN FULLY ACTIVE!");
    }

    void OnDisable()
    {
        smokeActive = false;
        smokeScreens.Remove(this);
    }

    public bool ContainsPoint(Vector2 position)
    {
        if (!smokeActive || smokeCircle == null)
            return false;

        Vector2 center = smokeCircle.transform.TransformPoint(
            smokeCircle.offset
        );

        Vector3 scale = smokeCircle.transform.lossyScale;

        float worldRadius = smokeCircle.radius *
            Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));

        return Vector2.Distance(center, position) <= worldRadius;
    }

    public static bool IsPointInsideActiveSmoke(Vector2 position)
    {
        for (int i = smokeScreens.Count - 1; i >= 0; i--)
        {
            if (smokeScreens[i] == null)
            {
                smokeScreens.RemoveAt(i);
                continue;
            }

            if (smokeScreens[i].ContainsPoint(position))
                return true;
        }

        return false;
    }

    public static bool AreBothInsideSameSmoke(
        Transform firstShip,
        Transform secondShip)
    {
        if (firstShip == null || secondShip == null)
            return false;

        foreach (SmokeScreenArea smoke in smokeScreens)
        {
            if (smoke == null)
                continue;

            if (smoke.ContainsPoint(firstShip.position) &&
                smoke.ContainsPoint(secondShip.position))
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator SmokeLifecycle()
    {
        // Fade in over 2 seconds.
        yield return StartCoroutine(FadeInSmoke());

        // Keep the smoke fully visible for its duration.
        yield return new WaitForSeconds(smokeDuration);

        // Disable smoke protection immediately when fading out begins.
        smokeActive = false;

        // Gradually fade the smoke away.
        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;

            float alpha = 1f - Mathf.Clamp01(
                timer / fadeOutDuration
            );

            if (smokeRenderer != null)
            {
                Color color = originalColor;
                color.a = originalColor.a * alpha;
                smokeRenderer.color = color;
            }

            yield return null;
        }

        // Destroy the smoke after the fade-out finishes.
        Destroy(gameObject);
    }
}
