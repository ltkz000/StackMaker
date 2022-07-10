using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Transform playerObj;
    [SerializeField] private float height;
    [SerializeField] private float distance;

    private void Awake()
    {
        if(instance == null){
            instance = this;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 cameraoffset = new Vector3(0, height, distance);
        transform.position = playerObj.position + cameraoffset;
    }
}
