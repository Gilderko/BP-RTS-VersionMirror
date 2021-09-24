using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private float mapScale = 20f;
    [SerializeField] private float offset = -6f;

    private Transform playerCameraTransform;

    private void Update()
    {
        if (playerCameraTransform != null)
        {
            return;
        }

       if (NetworkClient.connection.identity  == null)
       {
            return;
       }

        playerCameraTransform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().GetCameraTransform();
    }

    private void MoveCamera()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector2 localPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect,mousePos,null,out localPos))
        {
            return;
        }

        Vector2 linInterp = new Vector2((localPos.x - minimapRect.rect.x) / minimapRect.rect.width,
            (localPos.y - minimapRect.rect.y) / minimapRect.rect.height);

        Vector3 newCameraPos = new Vector3(
            Mathf.Lerp(-mapScale, mapScale, linInterp.x),
            playerCameraTransform.position.y,
            Mathf.Lerp(-mapScale, mapScale, linInterp.y));

        playerCameraTransform.position = newCameraPos + new Vector3(0f, 0f, offset);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("DOWN");
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("DRAG");
        MoveCamera();
    }
}

