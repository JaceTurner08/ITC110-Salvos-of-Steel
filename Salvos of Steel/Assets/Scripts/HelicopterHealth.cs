using UnityEngine;

public class HelicopterHealth : MonoBehaviour
{
    [Header("Helicopter Durability")]
    public int maxMissileHits = 5;

    private int currentMissileHits;

    void Start()
    {
        currentMissileHits = 0;
    }

    public void TakeSmallMissileHit()
    {
        currentMissileHits++;

        Debug.Log(
            "Helicopter hit! Damage received: " +
            currentMissileHits + "/" + maxMissileHits
        );

        if (currentMissileHits >= maxMissileHits)
        {
            DestroyHelicopter();
        }
    }

    void DestroyHelicopter()
    {
        Debug.Log("HELICOPTER DESTROYED!");

        Destroy(gameObject);
    }
}
