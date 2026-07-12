using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ComputeCPU computeCPU;
    Camera camera;
    InputAction MoveMouse;
    InputAction ScrollMouse;
    public float BoundsScaleX;
    void Start()
    {
        camera = GetComponent<Camera>();
        camera.orthographicSize = Screen.width/2.0f * BoundsScaleX;
        MoveMouse = InputSystem.actions.FindAction("MoveMouse");
        ScrollMouse = InputSystem.actions.FindAction("ScrollMouse");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 Movement = MoveMouse.ReadValue<Vector2>();
        Vector2 Scroll = ScrollMouse.ReadValue<Vector2>();
        Debug.Log((Scroll,EventSystem.current.IsPointerOverGameObject()));
        if(!EventSystem.current.IsPointerOverGameObject()) camera.orthographicSize += camera.orthographicSize*(Scroll.y/10);
    
    }
}
