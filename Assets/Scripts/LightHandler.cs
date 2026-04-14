using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform thisTransform;
    [SerializeField] private GameObject thisGameObject;
    [SerializeField] private InputAction action;

    [SerializeField] private List<GameObject> sphereObj;

    private bool isLightOn = false;
    void Start()
    {
        print("Hello World");
        Debug.Log("Hello World");
        Debug.LogWarning("Hello World");
        Debug.LogError("Hello World");

        //thisTransform = GetComponent<Transform>();
        //Instantiate(thisGameObject, thisTransform.position + new Vector3(0,0,1.5f) , Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        //thisTransform.Rotate(1f * Time.deltaTime, 0f, 1f * Time.deltaTime);
        if (action.triggered)
        {
            isLightOn = !isLightOn;
            Debug.Log("Input Action Triggered: " + isLightOn);
            //sphereObj.GetComponent<Renderer>().enabled = isLightOn;
            foreach (var item in sphereObj)
            {
                item.SetActive(isLightOn);
            }
        }
    }

    public void OnEnable()
    {
        action.Enable();
    }

    public void OnDisable()
    {
        action.Disable();
    }
}