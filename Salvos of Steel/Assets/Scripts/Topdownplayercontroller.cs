using UnityEngine;
using UnityEngine.InputSystem;
public class Topdownplayercontroller : MonoBehaviour
{
    
    public float moveSpeed = 10.0f;
    public InputActionReference playerMove;
    
    // Update is called once per frame
    void Update()
    {
        // get input from Input System
        transform.Translate(playerMove.action.ReadValue<Vector2>() * moveSpeed * Time.deltaTime);
    }
}
