// Copyright (c) Meta Platforms, Inc. and affiliates.

using Meta.XR.Samples;
using System;
using UnityEngine;

/// <summary>
/// Hand-based grabber that replaces controller input with hand pinch input.
/// Subclasses OVRGrabber to reuse grab volume, candidate selection, and object movement logic.
/// Overrides Update entirely to read from OVRHand instead of OVRInput controllers.
/// </summary>
[MetaCodeSample("StarterSample.Core-HandsInteraction")]
public class OVRHandGrabber : OVRGrabber
{
    [Header("Hand Tracking")]
    [SerializeField]
    private OVRHand m_hand;

    [SerializeField]
    private OVRSkeleton m_skeleton;

    private const int VelocityHistorySize = 5;
    private Vector3[] m_velocityHistory = new Vector3[VelocityHistorySize];
    private int m_velocityIndex = 0;
    private Vector3 m_lastWristPos;
    private bool m_wasTracked;

    public OVRHand Hand => m_hand;

    protected override void Awake()
    {
        // Initialize anchor offsets like the base class, but skip OVRCameraRig
        // callback registration to prevent the base from reading controller input
        // via the private OnUpdatedAnchors method.
        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;

        if (m_hand == null)
        {
            m_hand = GetComponentInParent<OVRHand>();
        }

        if (m_skeleton == null)
        {
            m_skeleton = GetComponentInParent<OVRSkeleton>();
        }
    }

    protected override void Start()
    {
        base.Start();
        m_lastWristPos = transform.position;
    }

    /// <summary>
    /// Replaces the base class Update entirely. Reads pinch strength from OVRHand
    /// instead of controller trigger, and computes throw velocity from skeleton bone
    /// position deltas instead of OVRInput controller velocity.
    /// </summary>
    public override void Update()
    {
        if (m_hand == null || !m_hand.IsTracked)
        {
            if (m_wasTracked && m_grabbedObj != null)
            {
                HandGrabEnd();
            }

            m_wasTracked = false;
            return;
        }

        m_wasTracked = true;
        TrackHandVelocity();

        // Replicate base OnUpdatedAnchors logic with hand input
        Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition);
        Quaternion destRot = m_parentTransform.rotation * m_anchorOffsetRotation;

        if (m_moveHandPosition)
        {
            GetComponent<Rigidbody>().MovePosition(destPos);
            GetComponent<Rigidbody>().MoveRotation(destRot);
        }

        if (!m_parentHeldObject)
        {
            MoveGrabbedObject(destPos, destRot);
        }

        m_lastPos = transform.position;
        m_lastRot = transform.rotation;

        float prevFlex = m_prevFlex;
        m_prevFlex = m_hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin))
        {
            GrabBegin();
        }
        else if ((m_prevFlex <= grabEnd) && (prevFlex > grabEnd))
        {
            HandGrabEnd();
        }
    }

    /// <summary>
    /// Releases the grabbed object with velocity computed from hand bone tracking
    /// instead of controller velocity.
    /// </summary>
    private void HandGrabEnd()
    {
        if (m_grabbedObj != null)
        {
            Vector3 linearVelocity = GetAverageVelocity();
            GrabbableRelease(linearVelocity, Vector3.zero);
        }

        GrabVolumeEnable(true);
    }

    private void TrackHandVelocity()
    {
        Vector3 currentPos = GetWristPosition();
        if (Time.deltaTime > 0f)
        {
            m_velocityHistory[m_velocityIndex] = (currentPos - m_lastWristPos) / Time.deltaTime;
            m_velocityIndex = (m_velocityIndex + 1) % VelocityHistorySize;
        }

        m_lastWristPos = currentPos;
    }

    private Vector3 GetWristPosition()
    {
        if (m_skeleton != null && m_skeleton.IsInitialized && m_skeleton.Bones != null)
        {
            foreach (var bone in m_skeleton.Bones)
            {
                if (bone.Id == OVRSkeleton.BoneId.Hand_WristRoot)
                {
                    return bone.Transform.position;
                }
            }
        }

        return transform.position;
    }

    private Vector3 GetAverageVelocity()
    {
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < VelocityHistorySize; i++)
        {
            sum += m_velocityHistory[i];
        }

        return sum / VelocityHistorySize;
    }
}
