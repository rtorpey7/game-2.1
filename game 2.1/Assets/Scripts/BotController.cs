using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    public Rigidbody botRoot;
    Vector3 positionOffset;
    private GameObject _stabilizerGameobject;
    private Rigidbody _stabilizerRigidbody;
    private ConfigurableJoint _stabilizerJoint;
    private void Awake()
    {
        positionOffset = transform.position - botRoot.position;

        _stabilizerGameobject = new GameObject("Stabilizer", typeof(Rigidbody), typeof(ConfigurableJoint));
        _stabilizerGameobject.transform.parent = botRoot.transform.parent;
        _stabilizerGameobject.transform.rotation = botRoot.rotation;

        _stabilizerJoint = _stabilizerGameobject.GetComponent<ConfigurableJoint>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _stabilizerRigidbody = _stabilizerGameobject.GetComponent<Rigidbody>();
        _stabilizerRigidbody.isKinematic = true;

        _stabilizerJoint.connectedBody = botRoot;

        JointDrive drive = new JointDrive();
        drive.positionSpring = 100000;
        drive.positionDamper = 500;
        drive.maximumForce = Mathf.Infinity;
        _stabilizerJoint.angularXDrive = drive;
        _stabilizerJoint.angularYZDrive = drive;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //botRoot.MovePosition(positionOffset + transform.position);
        _stabilizerRigidbody.MovePosition(_stabilizerRigidbody.position);
        _stabilizerRigidbody.MoveRotation(transform.rotation);

        botRoot.transform.position = -positionOffset + transform.position;
        //botRoot.MoveRotation(transform.rotation);
    }

    public ConfigurableJoint GetStabilizerJoint()
    {
        return _stabilizerJoint;
    }
}
