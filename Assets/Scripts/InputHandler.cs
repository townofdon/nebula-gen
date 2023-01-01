using UnityEngine;

public class InputHandler : MonoBehaviour
{
    // public float scrollSpeed = 10.0f;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key was pressed.");
        }

        // float vertical = Input.GetAxisRaw("Vertical");
        // float horizontal = Input.GetAxisRaw("Horizontal");
        // transform.position = transform.position + new Vector3(horizontal, vertical) * scrollSpeed * Time.deltaTime;
    }
}