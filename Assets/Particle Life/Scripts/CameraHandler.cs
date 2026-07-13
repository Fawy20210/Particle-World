using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;

public class CameraHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ComputeCPU computeCPU;
    Camera camera;
    InputAction MoveMouse;
    InputAction ScrollMouse;
    public float BoundsScaleX;
    public float speed;
    float startSize;
    float Zoom = 1;
    void Start()
    {
        camera = GetComponent<Camera>();
        startSize = camera.orthographicSize = Screen.width/2.0f * BoundsScaleX;
        MoveMouse = InputSystem.actions.FindAction("MoveMouse");
        ScrollMouse = InputSystem.actions.FindAction("ScrollMouse");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 Movement = MoveMouse.ReadValue<Vector2>();
        Vector2 Scroll = ScrollMouse.ReadValue<Vector2>();
        bool IsClicked = Mouse.current.leftButton.isPressed;
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            Zoom -= Zoom * Scroll.y/10;
            camera.orthographicSize = startSize * Zoom;

            float screenHeight = camera.orthographicSize * 2;
            float screenWidth = screenHeight * Screen.width/Screen.height;

            Vector3 newpos = new Vector3(Movement.x/screenWidth,Movement.y/screenHeight,0);
            if(IsClicked) camera.transform.position -= newpos*Zoom*speed*Zoom;//(camera.orthographicSize/startSize) 

        }

        //Vector3 newpos = new Vector3(Movement.x,Movement.y,0);
        //Debug.Log((Movement,newpos,IsClicked,Zoom,screenWidth,screenHeight));

    
    }
}
