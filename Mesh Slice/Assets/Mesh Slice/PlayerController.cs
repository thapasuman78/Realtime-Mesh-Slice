using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public Slicer slicer;
    public Transform Target;
    public CinemachineFreeLook cinemachineFree;
    public MeshSlice slice;

    private Transform mainCam;
    private float xSpeed, ySpeed;

    public List<Transform> MeshCutable { get; set; } = new List<Transform>();

    private bool isMouseDown;
    void Start()
    {
        mainCam = Camera.main.transform;
        xSpeed = cinemachineFree.m_XAxis.m_MaxSpeed;
        ySpeed = cinemachineFree.m_YAxis.m_MaxSpeed;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            slicer.transform.parent.position = Target.position;
            slicer.transform.parent.forward = transform.forward;
            slicer.transform.parent.gameObject.SetActive(true);
            cinemachineFree.m_XAxis.m_MaxSpeed = 0;
            cinemachineFree.m_YAxis.m_MaxSpeed = 0;
        }
        else if(Input.GetMouseButtonUp(0) && isMouseDown)
        {
            isMouseDown = false;
            slicer.transform.parent.gameObject.SetActive(false);
            
            if (MeshCutable.Count > 0)
            {
                Debug.Log(MeshCutable.Count);
                foreach (var item in MeshCutable)
                {
                    slice.Initialize(item, item.GetComponent<MeshFilter>());
                    slice.FindIntersectionPoints();
                }
            }
            MeshCutable.Clear();
            //cinemachineFree.m_XAxis.m_MaxSpeed = xSpeed;
            //cinemachineFree.m_YAxis.m_MaxSpeed = ySpeed;
        }

        if (isMouseDown) return;

        Vector3 inputs = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"));

        Vector3 direction = (transform.forward * inputs.z + transform.right * inputs.x).normalized;

        Vector3 lookTarget = mainCam.forward;
        lookTarget.y = 0;

        transform.rotation = Quaternion.LookRotation(lookTarget);

        transform.position += movementSpeed * direction  * Time.deltaTime;

    }
}
