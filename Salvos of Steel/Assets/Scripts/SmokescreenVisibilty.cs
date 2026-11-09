using UnityEngine;
using System.Collections.Generic;

public class SmokeScreenVisibility : MonoBehaviour
{
    [Header("Ship Visuals")]
    public GameObject shipVisuals;

    private HashSet<SmokeScreenEffect> activeSmoke =
        new HashSet<SmokeScreenEffect>();

    public void EnterSmoke(SmokeScreenEffect smoke)
    {
        activeSmoke.Add(smoke);
        UpdateVisibility();
    }

    public void ExitSmoke(SmokeScreenEffect smoke)
    {
        activeSmoke.Remove(smoke);
        UpdateVisibility();
    }

    void UpdateVisibility()
    {
        bool invisible = activeSmoke.Count > 0;

        if (shipVisuals != null)
        {
            shipVisuals.SetActive(!invisible);
        }
    }
}
