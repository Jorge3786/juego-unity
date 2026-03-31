using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public LayerMask groundMask; // asignar en el Inspector

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    bool IsGrounded()
    {
        // Lanza un rayo desde los pies del jugador hacia abajo
        Vector3 feetPosition = transform.position + Vector3.down * (controller.height / 2f);
        return Physics.Raycast(feetPosition, Vector3.down, 0.15f, groundMask);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // -- MOVIMIENTO --
        float x = 0f;
        float z = 0f;

        if (Keyboard.current.dKey.isPressed) x = 1f;
        if (Keyboard.current.aKey.isPressed) x = -1f;
        if (Keyboard.current.wKey.isPressed) z = 1f;
        if (Keyboard.current.sKey.isPressed) z = -1f;

        Vector3 movement = transform.right * x + transform.forward * z;
        controller.Move(movement * speed * Time.deltaTime);

        // -- GRAVEDAD Y SALTO --
        bool isGrounded = IsGrounded();

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}