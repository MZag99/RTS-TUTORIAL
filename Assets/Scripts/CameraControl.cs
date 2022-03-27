using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{


    // CHANGEABLE VARIABLES

    public float cameraSpeed, groundHeight, zoomSpeed;
    public Vector2 cameraHeightMinMax;
    public Vector2 cameraRotationMinMax;

    Vector2 mousePos, mousePosScreen, keyboardInput;

    bool isCursorInScreen;
    Rect selectionRect, boxRect;

    [Range(0, 1)]
    public float zoomLerp = 0.1f;

    [Range(0, 0.2f)]
    public float cursorTreshold;

    // END

    public float mouseScroll;

    RectTransform selectionBox;
    new Camera camera;



    private void Awake()
    {
        selectionBox = GetComponentInChildren<Image>(true).transform as RectTransform;
        camera = GetComponent<Camera>();
        selectionBox.gameObject.SetActive(false);
    }



    private void Update()
    {
        UpdateMovement();
        UpdateClicks();
        UpdateZoom();
    }



    void UpdateInput()
    {

    }



    void UpdateMovement()
    {
        keyboardInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        mousePos = Input.mousePosition;
        mousePosScreen = camera.ScreenToViewportPoint(mousePos);
        isCursorInScreen = mousePosScreen.x >= 0 && mousePosScreen.y >= 0
            && mousePosScreen.x <= 1 && mousePosScreen.y <= 1;

        Vector2 movementDirection = keyboardInput;

        if (isCursorInScreen)
        {
            if (mousePosScreen.x < cursorTreshold) movementDirection.x -= 1 - mousePosScreen.x / cursorTreshold;
            if (mousePosScreen.x > 1 - cursorTreshold) movementDirection.x += 1 - ((1 - mousePosScreen.x) / cursorTreshold);
            if (mousePosScreen.y < cursorTreshold) movementDirection.y -= 1 - mousePosScreen.y / cursorTreshold;
            if (mousePosScreen.y > 1 - cursorTreshold) movementDirection.y += 1 - ((1 - mousePosScreen.y) / cursorTreshold);

        }



        var deltaPosition = new Vector3(movementDirection.x, 0, movementDirection.y);
        deltaPosition *= cameraSpeed * Time.deltaTime;
        transform.position += deltaPosition;
    }



    void UpdateClicks()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionBox.gameObject.SetActive(true);
            selectionRect.position = mousePos;
        }

        else if (Input.GetMouseButtonUp(0))
        {
            selectionBox.gameObject.SetActive(false);
        }

        if (Input.GetMouseButton(0))
        {
            selectionRect.size = mousePos - selectionRect.position;
            boxRect = AbsRect(selectionRect);
            selectionBox.anchoredPosition = boxRect.position;
            selectionBox.sizeDelta = boxRect.size;
        }
    }



    void UpdateZoom()
    {
        mouseScroll = Input.mouseScrollDelta.y;
        float zoomDelta = mouseScroll * zoomSpeed * Time.deltaTime;
        zoomLerp = Mathf.Clamp01(zoomLerp + zoomDelta);

        var position = transform.position;
        position.y = Mathf.Lerp(cameraHeightMinMax.x, cameraHeightMinMax.y, zoomLerp) + groundHeight;
        transform.localPosition = position;

        var rotation = transform.localEulerAngles;
        rotation.x = Mathf.Lerp(cameraRotationMinMax.x, cameraRotationMinMax.y, zoomLerp);
        transform.localEulerAngles = rotation;
    }



    Rect AbsRect(Rect rect)
    {
        if (rect.width < 0)
        {
            rect.x += rect.width;
            rect.width *= -1;
        }

        if (rect.height < 0)
        {
            rect.y += rect.height;
            rect.height *= -1;
        }

        return rect;
    }
}
