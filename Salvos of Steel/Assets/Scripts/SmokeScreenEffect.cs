
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmokeScreenEffect : MonoBehaviour
{
    [Header("Smoke Duration")]
    public float smokeDuration = 10f;
    public float fadeInDuration = 1f;

    [Header("Smoke Appearance")]
    public SpriteRenderer smokeRenderer;
    [Range(0f, 1f)]
    public float smokeOpacity = 0.5f;

    [Header("Deployment Sound")]
    public AudioClip poppingSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private HashSet<SmokeScreenVisibility> affectedShips =
        new HashSet<SmokeScreenVisibility>();

    private void Start()
    {
        // Begin with invisible smoke.
        if (smokeRenderer != null)
        {
            Color color = smokeRenderer.color;
            color.a = 0f;
            smokeRenderer.color = color;
        }

        // Play deployment sound.
        if (poppingSound != null)
        {
            AudioSource.PlayClipAtPoint(
                poppingSound,
                transform.position,
                soundVolume
            );
        }

        StartCoroutine(FadeInSmoke());

        Destroy(gameObject, smokeDuration);
    }

    IEnumerator FadeInSmoke()
    {
        if (smokeRenderer == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(
                0f,
                smokeOpacity,
                elapsed / fadeInDuration
            );

            Color color = smokeRenderer.color;
            color.a = alpha;
            smokeRenderer.color = color;

            yield return null;
        }

        Color finalColor = smokeRenderer.color;
        finalColor.a = smokeOpacity;
        smokeRenderer.color = finalColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SmokeScreenVisibility ship =
            other.GetComponentInParent<SmokeScreenVisibility>();

        if (ship != null && affectedShips.Add(ship))
        {
            ship.EnterSmoke(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SmokeScreenVisibility ship =
            other.GetComponentInParent<SmokeScreenVisibility>();

        if (ship != null && affectedShips.Remove(ship))
        {
            ship.ExitSmoke(this);
        }
    }

    private void OnDisable()
    {
        foreach (SmokeScreenVisibility ship in affectedShips)
        {
            if (ship != null)
                ship.ExitSmoke(this);
        }

        affectedShips.Clear();
    }
}
