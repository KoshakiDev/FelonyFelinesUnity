using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class WeaponIK2D : MonoBehaviour
{



    [Header("Bindings")]
    /// The transform to use as a target for the left hand
    [Tooltip("The transform to use as a target for the left hand")]
    public Transform LeftHandTarget = null;
    /// The transform to use as a target for the right hand
    [Tooltip("The transform to use as a target for the right hand")]
    public Transform RightHandTarget = null;

    [Header("Attachments")]
    /// whether or not to attach the left hand to its target
    [Tooltip("whether or not to attach the left hand to its target")]
    public bool AttachLeftHand = true;
    /// whether or not to attach the right hand to its target
    [Tooltip("whether or not to attach the right hand to its target")]
    public bool AttachRightHand = true;


    protected CharacterHandleWeapon _characterHandleWeapon;

    protected virtual void Start()
    {
        _characterHandleWeapon = GetComponent<CharacterHandleWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_characterHandleWeapon == null)
        {
            return;
        }

        if (_characterHandleWeapon.CurrentWeapon == null)
        {
            return;
        }


        Transform LeftHandHandle = _characterHandleWeapon.CurrentWeapon.LeftHandHandle;
        Transform RightHandHandle = _characterHandleWeapon.CurrentWeapon.RightHandHandle;

        


        //if the IK is active, set the position and rotation directly to the goal. 

        if (AttachLeftHand)
        {
            if (LeftHandHandle != null)
            {
                AttachHandToHandle(LeftHandTarget, LeftHandHandle);

            }
            else
            {
                DetachHandFromHandle(LeftHandTarget);
            }
        }

        if (AttachRightHand)
        {
            if (RightHandHandle != null)
            {
                AttachHandToHandle(RightHandTarget, RightHandHandle);
            }
            else
            {
                DetachHandFromHandle(RightHandTarget);
            }
        }
    }

    /// <summary>
    /// Attaches the hands to the handles
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="handle"></param>
    protected virtual void AttachHandToHandle(Transform hand, Transform handle)
    {
        
        hand.position = handle.position;
    
    }

    /// <summary>
    /// Detachs the hand from handle, if the IK is not active, set the position and rotation of the hand and head back to the original position
    /// </summary>
    /// <param name="hand">Hand.</param>
    protected virtual void DetachHandFromHandle(Transform hand)
    {

    }

    /// <summary>
    /// Binds the character hands to the handles targets
    /// </summary>
    /// <param name="leftHand">Left hand.</param>
    /// <param name="rightHand">Right hand.</param>
    public virtual void SetHandles(Transform leftHand, Transform rightHand)
    {
        LeftHandTarget = leftHand;
        RightHandTarget = rightHand;
    }



}
