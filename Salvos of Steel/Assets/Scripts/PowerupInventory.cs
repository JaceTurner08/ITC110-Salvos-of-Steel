using UnityEngine;

public class PowerupInventory : MonoBehaviour
{
    [Header("Powerup Inventory")]
    public int emergencyFuelCount = 0;
    public int airstrikeCount = 0;

    [Header("Smoke Screen Inventory")]
    public int smokeScreenCount = 0;

    [Header("Interception Inventory")]
    public int interceptionCount = 0;

    public void AddSmokeScreen()
    {
        smokeScreenCount++;
        Debug.Log("Smoke Screen: " + smokeScreenCount);
    }

    public void AddEmergencyFuel()
    {
        emergencyFuelCount++;
        Debug.Log("Emergency Fuel collected! Total: " + emergencyFuelCount);
    }

    public void AddAirstrike()
    {
        airstrikeCount++;
        Debug.Log("Airstrike collected! Total: " + airstrikeCount);
    }

    public void AddInterception()
    {
        interceptionCount++;
        Debug.Log("Interception System collected! Total: " + interceptionCount);
    }

    public bool UseEmergencyFuel()
    {
        if (emergencyFuelCount <= 0)
            return false;

        emergencyFuelCount--;
        return true;
    }

    public bool UseAirstrike()
    {
        if (airstrikeCount <= 0)
            return false;

        airstrikeCount--;
        return true;
    }

    public bool UseSmokeScreen()
    {
        if (smokeScreenCount <= 0)
            return false;

        smokeScreenCount--;
        return true;
    }

    public bool UseInterception()
    {
        if (interceptionCount <= 0)
            return false;

        interceptionCount--;
        return true;
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 18;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;

        GUI.Label(
            new Rect(Screen.width - 230, 20, 220, 30),
            "POWERUPS",
            style
        );

        GUI.Label(
            new Rect(Screen.width - 230, 50, 220, 30),
            "1 - Emergency Fuel: " + emergencyFuelCount,
            style
        );

        GUI.Label(
            new Rect(Screen.width - 230, 80, 220, 30),
            "2 - Airstrike: " + airstrikeCount,
            style
        );

        GUI.Label(
            new Rect(Screen.width - 230, 110, 250, 30),
            "3 - Smoke Screen: " + smokeScreenCount,
            style
        );

        GUI.Label(
            new Rect(Screen.width - 230, 140, 250, 30),
            "4 - Interception: " + interceptionCount,
            style
        );
    }
}
