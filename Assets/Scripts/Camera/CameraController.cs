using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Basic camera contoller works with keyboard and mouse. Works offline and instantiated with player to move the cinemachine camera.
/// </summary>
public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float speedY = 3f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;
    [SerializeField] private Vector2 screenYLimits = Vector2.zero;

    private Vector2 prevInput = Vector2.zero;
    private float prevScrollInput = 0f;

    private Controls controls;


    #region Client

#if !UNITY_SERVER

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority || !Application.isFocused)
        {
            return;
        }

        UpdateCameraPosition();
    }

#endif


    [Client]
    private void UpdateCameraPosition()
    {
        Vector3 pos = playerCameraTransform.position;

        if (prevInput == Vector2.zero)
        {
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            if (cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;
            }
            else if (cursorPosition.y < screenBorderThickness)
            {
                cursorMovement.z -= 1;
            }
            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPosition.x < screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            pos += cursorMovement.normalized * speed * Time.deltaTime;
        }
        else
        {
            pos += new Vector3(prevInput.x, 0f, prevInput.y) * speed * Time.deltaTime;
        }

        pos.y += -1 * prevScrollInput * speedY * Time.deltaTime;

        pos.y = Mathf.Clamp(pos.y, screenYLimits.x, screenYLimits.y);
        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

        playerCameraTransform.position = pos;
    }

    [Client]
    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);

        controls = new Controls();

        controls.Player.ZoomCamera.performed += SetPrevScrollInput;
        controls.Player.ZoomCamera.canceled += SetPrevScrollInput;

        controls.Player.MoveCamera.performed += SetPrevInput;
        controls.Player.MoveCamera.canceled += SetPrevInput;

        controls.Enable();
    }

    [Client]
    private void SetPrevScrollInput(InputAction.CallbackContext ctx)
    {
        prevScrollInput = ctx.ReadValue<float>();
    }

    [Client]
    private void SetPrevInput(InputAction.CallbackContext ctx)
    {
        prevInput = ctx.ReadValue<Vector2>();
    }

    #endregion
}
