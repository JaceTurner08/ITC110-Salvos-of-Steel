using UnityEngine;

public class Menumoving : MonoBehaviour
{
    void Update()
    {
        transform.position += new Vector3(0.01f, 0, 0);
    }
}
