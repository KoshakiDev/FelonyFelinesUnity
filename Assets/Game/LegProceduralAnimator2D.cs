using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections.Generic;



public class LegProceduralAnimator2D : MonoBehaviour
{
    
    [Header("Bindings")]
    /// The transform to use as a target for the left hand
    [Tooltip("The transform to use as a target for the right arm")]
    public Transform RightArmTarget = null;

    [Tooltip("The transform to use as a target for the left arm")]
    public Transform LeftArmTarget = null;

    [Tooltip("The transform to use as a target for the right shoulder")]
    public Transform RightShoulder = null;

    [Tooltip("The transform to use as a target for the left shoulder")]
    public Transform LeftShoulder = null;




    [Tooltip("The transform to use as a target for the right leg")]
    public Transform RightLegTarget = null;

    [Tooltip("The transform to use as a target for the left leg")]
    public Transform LeftLegTarget = null;
    /// The transform to use as a target for the right hand
    
    [Tooltip("The transform to use for the hip")]
    public Transform HipTransform = null;

    [Tooltip("The transform to use for as a target for posture")]
    public Transform PostureTarget = null;


    [Header("Stride Settings")]

    [Tooltip("How far the leaning should be")]
    public float LeanSize = 0.25f;

    [Tooltip("The angle to lean the hips at")]
    public float LeanHipAngleDegrees = 5f;

    [Tooltip("How high the bobbing should be")]
    public float BobSize = 0.25f;

    [Tooltip("How big the crouch should be")]
    public float CrouchSize = 0.2f;

    [Tooltip("How big the step would be")]
    public float GaitSize = 0.4f;

    [Tooltip("The scale of the circular motion of legs")]
    public Vector2 scale = new Vector2(Mathf.Clamp(0.5f, 0f, 1f), Mathf.Clamp(0.5f, 0f, 1f)).normalized;

    [Header("Arm Settings")]

    public float ArmLength = 1f;
    public Vector2 ArmScale = new Vector2(Mathf.Clamp(0.5f, 0f, 1f), Mathf.Clamp(0.5f, 0f, 1f)).normalized;





    [HideInInspector]
    public float TravelSpeed = 1f;

    

    Vector3 InitialRightLegPosition;
    Vector3 InitialLeftLegPosition;

    Vector3 InitialRightArmPosition;
    Vector3 InitialLeftArmPosition;

    Vector3 InitialHipPosition;
    float InitialHipRotation;
    Vector3 InitialPosturePosition;
    float angle = 0f;



    /*
    [Header("Attachments")]
    /// whether or not to attach the left hand to its target
    [Tooltip("whether or not to attach the left hand to its target")]
    public bool AttachLeftHand = true;
    /// whether or not to attach the right hand to its target
    [Tooltip("whether or not to attach the right hand to its target")]
    public bool AttachRightHand = true;
    */

    protected TopDownController2D _topDownController2D;

    protected CharacterOrientation2D _characterOrientation2D;

    protected CharacterHandleWeapon _characterHandleWeapon;
    

    protected virtual void Start()
    {
        _topDownController2D = GetComponent<TopDownController2D>();
        _characterHandleWeapon = GetComponent<CharacterHandleWeapon>();


        _characterOrientation2D = GetComponent<CharacterOrientation2D>();

        InitialRightLegPosition = RightLegTarget.localPosition;
        InitialLeftLegPosition = LeftLegTarget.localPosition;       
       
        InitialRightArmPosition = RightArmTarget.localPosition;
        InitialLeftArmPosition = LeftArmTarget.localPosition;

        InitialHipPosition = HipTransform.localPosition;
        InitialHipRotation = 90;

        InitialPosturePosition = PostureTarget.localPosition;
    }


    // Update is called once per frame
    void Update()
    {
        if (_topDownController2D == null)
        {
            return;
        }


        
        if (_topDownController2D.CurrentMovement.magnitude > 0)
        {
            
            TravelSpeed = _topDownController2D.CurrentMovement.magnitude;
            if(
                (_topDownController2D.CurrentMovement.x < 0 && _characterOrientation2D.IsFacingRight) ||
                (_topDownController2D.CurrentMovement.x > 0 && !_characterOrientation2D.IsFacingRight)
                )
            {
                TravelSpeed *= -1;
            }

            
            IncrementAngle();

            
            MoveLeg(LeftLegTarget, angle + 180f, InitialLeftLegPosition);
            MoveLeg(RightLegTarget, angle, InitialRightLegPosition);

            MovePosture();
            LeanHip();
            MoveHip(angle);

            
            



        }
        else
        {
            TravelSpeed = 0;

            ResetHipToInitialPosition(angle);
            ResetLegToInitialPosition(LeftLegTarget, angle + 180f, InitialLeftLegPosition);
            ResetLegToInitialPosition(RightLegTarget, angle, InitialRightLegPosition);
        }

        if (_characterHandleWeapon.CurrentWeapon == null)
        {
            MoveArm(RightArmTarget, angle + 180f, InitialRightArmPosition);
            MoveArm(LeftArmTarget, angle, InitialLeftArmPosition);

        }

    }

    protected void IncrementAngle()
    {
        angle = (angle + TravelSpeed) % 360f;
    }

    protected virtual void MovePosture()
    {
        Vector3 new_position;

        new_position = new Vector3(TravelSpeed * LeanSize, 0);

        new_position.Scale(scale);

        new_position += InitialPosturePosition;

        PostureTarget.localPosition = new_position;
    }

    protected virtual void LeanHip()
    {
        Quaternion new_rotation = Quaternion.Euler(0, 0, InitialHipRotation);


        if (TravelSpeed < 0)
        {
            new_rotation = Quaternion.Euler(0, 0, InitialHipRotation + LeanHipAngleDegrees);

        }
        else if (TravelSpeed > 0)
        {
            new_rotation = Quaternion.Euler(0, 0, InitialHipRotation - LeanHipAngleDegrees);
        }
        HipTransform.localRotation = new_rotation;
    }

    protected virtual void MoveHip(float passed_angle)
    {
        float angle_in_radians = passed_angle * Mathf.Deg2Rad;

        Vector3 new_position;

        new_position = new Vector3(0, Mathf.Sin(angle_in_radians) * BobSize - CrouchSize);

        new_position.Scale(scale);

        new_position += InitialHipPosition;


        new_position = new Vector3(new_position.x, Mathf.Clamp(new_position.y, 0, InitialHipPosition.y));


        HipTransform.localPosition = new_position;
    }

    protected virtual void ResetHipToInitialPosition(float passed_angle)
    {
        float angle_in_radians = passed_angle * Mathf.Deg2Rad;

        Vector3 new_position;

        new_position = new Vector3(0, Mathf.Sin(angle_in_radians) * BobSize - CrouchSize);

        new_position.Scale(scale);

        new_position += InitialHipPosition;


        new_position = new Vector3(new_position.x, Mathf.Clamp(new_position.y, 0, InitialHipPosition.y - CrouchSize / 2));


        HipTransform.localPosition = new_position;
    }


    protected virtual void MoveLeg(Transform leg, float passed_angle, Vector3 initial_position)
    {

        float angle_in_radians = passed_angle * Mathf.Deg2Rad;

        Vector3 new_position;


        new_position = new Vector3(Mathf.Sin(angle_in_radians) - TravelSpeed * LeanSize / 2, Mathf.Cos(angle_in_radians));

        new_position.Scale(scale);

        new_position += initial_position;

        new_position *= GaitSize;


        new_position = new Vector3(new_position.x, Mathf.Clamp(new_position.y, initial_position.y, new_position.y));

        leg.localPosition = new_position;
    }

    protected virtual void ResetLegToInitialPosition(Transform leg, float passed_angle, Vector3 initial_position)
    {
        float angle_in_radians = passed_angle * Mathf.Deg2Rad;

        Vector3 new_position;


        new_position = new Vector3(Mathf.Sin(angle_in_radians), 0);

        new_position.Scale(scale);

        new_position += initial_position;

        new_position *= GaitSize / 2;

        leg.localPosition = new_position;

    }

    protected virtual void MoveArm(Transform arm, float passed_angle, Vector3 initial_position)
    {
        float angle_in_radians = passed_angle * Mathf.Deg2Rad;

        Vector3 new_position;


        new_position = new Vector3(Mathf.Sin(angle_in_radians), -Mathf.Abs(Mathf.Cos(angle_in_radians)));

        
        /**/
        if(arm == RightArmTarget)
        {
            new_position += initial_position + new Vector3(ArmLength, 0);
        }
        else if (arm == LeftArmTarget)
        {
            new_position += initial_position - new Vector3(ArmLength, 0);
        }
        new_position.Scale(ArmScale);



        //new_position = new Vector3(new_position.x, Mathf.Clamp(new_position.y, initial_position.y, new_position.y));

        arm.localPosition = new_position;
    }

}
