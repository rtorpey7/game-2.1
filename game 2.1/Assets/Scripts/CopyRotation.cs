using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    Quaternion initialRotation;
    ConfigurableJoint joint;
    public Transform targetJoint;
    public Vector3 offset;

    void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();
        initialRotation = joint.transform.localRotation * Quaternion.Euler(offset);
        //targetJoint = GameObject.Find(joint.name).transform;
    }

    void Start()
    {
        ConfigurableJointExtensions.SetTargetRotation(joint, targetJoint.localRotation, initialRotation);
    }
    void Update()
    {
        ConfigurableJointExtensions.SetTargetRotationLocal(joint, targetJoint.localRotation, initialRotation);
    }

}
