using UnityEngine;

public class MenuMovingY : MonoBehaviour
{
    void Update()
    {
        transform.position += new Vector3(0, 0.001f, 0);
    }
}
