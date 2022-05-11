using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer : MonoBehaviour
{
    public PlayerController PlayerTransform;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

        Vector3 direction = worldPos.normalized;

        float angle = Vector3.SignedAngle(direction, PlayerTransform.transform.right, Vector3.up);

        transform.localRotation = Quaternion.Euler(0, 0, -angle * 8);
    }

    //private void OnDrawGizmos()
    //{
    //    Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

    //    Vector3 direction = worldPos.normalized;

    //    Gizmos.DrawRay(Vector3.zero, direction);
    //    Gizmos.DrawRay(Vector3.zero, PlayerTransform.right);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (!PlayerTransform.MeshCutable.Contains(other.transform))
            PlayerTransform.MeshCutable.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (PlayerTransform.MeshCutable.Contains(other.transform))
            PlayerTransform.MeshCutable.Remove(other.transform);
    }
}
