using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class ProceduralAnimation2D : MonoBehaviour
{

    const float PIXEL_UNITS = (1f / 32f);

    public GameObject Parent = null;

    [Header("Arm Targets")]
    /// The transform to use as a target for the left hand
    [Tooltip("The transform to use as a target for the right arm")]
    public Transform RightArmTarget = null;
    Vector3 InitialRightArmTargetPosition;
    Vector3 right_arm_delta_position = new Vector3(0, 0);



    public bool is_right_arm_ik_enabled = false;


    [Tooltip("The transform to use as a target for the left arm")]
    public Transform LeftArmTarget = null;
    Vector3 InitialLeftArmTargetPosition;
    Vector3 left_arm_delta_position = new Vector3(0, 0);

    public bool is_left_arm_ik_enabled = false;


    [Header("Leg Targets")]

    [Tooltip("The transform to use as a target for the right leg")]
    public Transform RightLegTarget = null;
    Vector3 InitialRightLegTargetPosition;
    Vector3 right_leg_delta_position = new Vector3(0, 0);


    [Tooltip("The transform to use as a target for the left leg")]
    public Transform LeftLegTarget = null;
    Vector3 InitialLeftLegTargetPosition;
    Vector3 left_leg_delta_position = new Vector3(0, 0);



    [Header("Body Targets")]

    [Tooltip("The transform to use for the hip")]
    public Transform HipTransform = null;
    Vector3 InitialHipPosition;
    float InitialHipRotation = 90;
    Vector3 hip_delta_position = new Vector3(0, 0);



    [Tooltip("The transform to use for the torso")]
    public Transform TorsoTransform = null;
    Vector3 InitialTorsoPosition;
    Vector3 torso_delta_position = new Vector3(0, 0);


    [Tooltip("The transform to use for as a target for posture")]
    public Transform PostureTarget = null;
    Vector3 InitialPostureTargetPosition;
    Vector3 posture_delta_position = new Vector3(0, 0);


    [Header("Standing Settings")]

    [Tooltip("How fast the character should breathe")]
    public int breathingSpeed = 1;

    [Tooltip("The angle to lean the hips at")]
    public float IdleHipAngleDegrees = 5;

    [Tooltip("How big in pixels the crouch should be")]
    public int CrouchSize = 1;



    [Header("Walking/Running Settings")]

    [Tooltip("How far in pixels the leaning should be")]
    public int LeanSize = 3;

    [Tooltip("The angle to lean the hips at")]
    public float LeanHipAngleDegrees = 5;

    [Tooltip("How high in pixels the bobbing should be")]
    public int BobSize = 1;

    [Tooltip("How big in pixels the step would be")]
    public int GaitSize = 10;

    [Tooltip("The scale of the circular motion of legs")]
    public Vector2 LegScale = new Vector2(1f, 1f);

    public Vector2 ArmScale = new Vector2(1f, 1f);

    public float armSwingAngleDegrees = 30;

    float armLength;


    [HideInInspector]
    public float TravelSpeed = 0f;


    protected TopDownController2D _topDownController2D;

    protected CharacterOrientation2D _characterOrientation2D;

    protected CharacterHandleWeapon _characterHandleWeapon;


    void Awake()
    {
        AutoConnect();
    }

    void Reset()
    {
        AutoConnect();
    }


    // Start is called before the first frame update
    void Start()
    {
        SetInitialPositions();
        if (Parent != null)
        {
            _characterHandleWeapon = Parent.GetComponent<CharacterHandleWeapon>();
            _topDownController2D = Parent.GetComponent<TopDownController2D>();
            _characterOrientation2D = Parent.GetComponent<CharacterOrientation2D>();
        }
        else
        {
            Debug.LogError("Error: Parent not found for ProceduralAnimation2D");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_topDownController2D == null)
        {
            return;
        }

        Breathe();


        left_leg_delta_position += GetLegPosition(movementAngleUpdate + 180f);
        right_leg_delta_position += GetLegPosition(movementAngleUpdate);

        posture_delta_position += GetPosturePosition();


        hip_delta_position += GetHipPosition(movementAngleUpdate);


        if (_topDownController2D.CurrentMovement.magnitude > 0)
        {
            Move();
        }
        else
        {
            Idle();
        }
        UpdateIKHands();
        UpdateBodyPositions();
    }


    void UpdateIKHands()
    {
        if (_characterHandleWeapon == null)
        {
            return;
        }

        if (_characterHandleWeapon.CurrentWeapon == null)
        {
            is_left_arm_ik_enabled = false;
            is_right_arm_ik_enabled = false;
            return;
        }
        else
        {
            is_left_arm_ik_enabled = true;
            is_right_arm_ik_enabled = true;
        }


        Transform LeftHandHandle = _characterHandleWeapon.CurrentWeapon.LeftHandHandle;
        Transform RightHandHandle = _characterHandleWeapon.CurrentWeapon.RightHandHandle;


        //LeanWithAim();

        //if the IK is active, set the position and rotation directly to the goal. 

        if (LeftHandHandle != null)
        {
            LeftArmTarget.position = LeftHandHandle.position;
        }

        if (RightHandHandle != null)
        {
            RightArmTarget.position = RightHandHandle.position;
        }
    }

    

    int breathingUpdate = 0;

    void Breathe()
    {

        breathingUpdate = (breathingUpdate + breathingSpeed) % 360;
        
        float torso_radians = breathingUpdate * Mathf.Deg2Rad;
        float hip_radians = ((breathingUpdate + 32) % 360)  * Mathf.Deg2Rad;

        Vector3 vertical_scale;
        Vector3 horizontal_scale;

        // TODO: turn the vertical and horizontal scale into adjustable by user variables

        vertical_scale = new Vector3(0, 1 * PIXEL_UNITS);
        horizontal_scale = new Vector3(1 * PIXEL_UNITS, 0);

        Vector3 torso;
        Vector3 hip;
        Vector3 posture;

        torso = new Vector3(0, Mathf.Sin(torso_radians));

        hip = new Vector3(0, Mathf.Sin(hip_radians));

        posture = new Vector3(Mathf.Sin(hip_radians), 0);


        posture.Scale(horizontal_scale);

        torso.Scale(vertical_scale / 2);
        hip.Scale(vertical_scale / 2);

        torso -= vertical_scale / 2;
        hip -= vertical_scale / 2;


        torso_delta_position += torso;
        hip_delta_position += hip;
        posture_delta_position += posture;
    }

    float movementAngleUpdate = 0;

    void Move()
    {
        
        TravelSpeed = _topDownController2D.CurrentMovement.magnitude;

        
        if (
            (_topDownController2D.CurrentMovement.x < 0 && _characterOrientation2D.IsFacingRight) ||
            (_topDownController2D.CurrentMovement.x > 0 && !_characterOrientation2D.IsFacingRight)
            )
        {
            TravelSpeed *= -1;
        }

        LeanHip(LeanHipAngleDegrees);


        movementAngleUpdate = (movementAngleUpdate + TravelSpeed) % 360f;

        if (!is_right_arm_ik_enabled)
        {
            right_arm_delta_position += GetArmPosition(movementAngleUpdate + 180f);
        }
        if (!is_left_arm_ik_enabled)
        {
            left_arm_delta_position += GetArmPosition(movementAngleUpdate);
        }
        
    }

    void Idle()
    {
        TravelSpeed = 0;
        LeanHip(IdleHipAngleDegrees);


        //TODO: make the idle for arms adjustable in some way
        if (!is_right_arm_ik_enabled)
        {
            right_arm_delta_position += GetArmPosition(breathingUpdate);
        }
        if (!is_left_arm_ik_enabled)
        {
            left_arm_delta_position += GetArmPosition(breathingUpdate);
        }
    }

    

    protected virtual Vector3 GetPosturePosition()
    {
        return new Vector3(Mathf.Sign(TravelSpeed) * LeanSize * PIXEL_UNITS * ((TravelSpeed == 0) ? 0.0f : 1.0f), 0);
    }

    protected virtual void LeanHip(float passed_angle)
    {
        Quaternion new_rotation = Quaternion.Euler(0, 0, InitialHipRotation);

        new_rotation = Quaternion.Euler(0, 0, InitialHipRotation + passed_angle * Mathf.Sign(-TravelSpeed));

        HipTransform.localRotation = new_rotation;

        /*
        Quaternion new_rotation = Quaternion.Euler(0, 0, InitialHipRotation + LeanHipAngleDegrees);
        HipTransform.localRotation = new_rotation;
        */
    }

    protected virtual Vector3 GetHipPosition(float passed_angle)
    {
        float angle_in_radians = passed_angle * Mathf.Deg2Rad;

        Vector3 new_position;

        new_position = new Vector3(0, Mathf.Sin(angle_in_radians) * BobSize * PIXEL_UNITS - CrouchSize * PIXEL_UNITS);

        /*
        Adding and subtracting the initial position to clamp it
        */
        new_position += InitialHipPosition;
        new_position = new Vector3(
            new_position.x,
            Mathf.Clamp(new_position.y, 0, InitialHipPosition.y - CrouchSize / 2 * PIXEL_UNITS * ((TravelSpeed == 0) ? 0.0f : 1.0f)));
        new_position -= InitialHipPosition;

        return new_position;
    }


    protected virtual Vector3 GetLegPosition(float passed_angle)
    {
        float angle_in_radians = passed_angle * Mathf.Deg2Rad;

        Vector3 new_position;

        new_position = new Vector3(
            Mathf.Sin(angle_in_radians) - Mathf.Sign(TravelSpeed) * LeanSize / 2 * PIXEL_UNITS * ((TravelSpeed != 0) ? 0.0f : 1.0f),
            Mathf.Cos(angle_in_radians) * ((TravelSpeed == 0) ? 0.0f : 1.0f)
        );
        new_position.Scale(LegScale);

        new_position += InitialRightLegTargetPosition;

        new_position *= GaitSize * PIXEL_UNITS * ((TravelSpeed == 0) ? 0.5f : 1.0f);

      
        return new_position;
    }


    protected virtual Vector3 GetArmPosition(float passed_angle)
    {
        float angle_in_radians = passed_angle * Mathf.Deg2Rad;

        Vector3 new_position;

        float half_arm_swing = Mathf.Cos((Mathf.PI - armSwingAngleDegrees * Mathf.Deg2Rad) / 2f);
        float arm_swing_height = Mathf.Sin((Mathf.PI - armSwingAngleDegrees * Mathf.Deg2Rad) / 2f);


        new_position = new Vector3(
            Mathf.Sin(angle_in_radians) * half_arm_swing,
            Mathf.Clamp(-Mathf.Abs(Mathf.Cos(angle_in_radians)), -1, -arm_swing_height)
            );

        new_position.Scale(ArmScale);

        return new_position;
    }




    //################

    void UpdateBodyPositions()
    {
        if (!is_right_arm_ik_enabled)
        {
            RightArmTarget.localPosition = right_arm_delta_position + InitialRightArmTargetPosition + new Vector3(armLength, 0);
        }
        if (!is_left_arm_ik_enabled)
        {
            LeftArmTarget.localPosition = left_arm_delta_position + InitialLeftArmTargetPosition - new Vector3(armLength, 0);
        }


        right_leg_delta_position = new Vector3(
            right_leg_delta_position.x,
            Mathf.Clamp(right_leg_delta_position.y, InitialRightLegTargetPosition.y, right_leg_delta_position.y)
        );

        left_leg_delta_position = new Vector3(
            left_leg_delta_position.x,
            Mathf.Clamp(left_leg_delta_position.y, InitialLeftLegTargetPosition.y, left_leg_delta_position.y)
        );
        
        RightLegTarget.localPosition = right_leg_delta_position + InitialRightLegTargetPosition;
        LeftLegTarget.localPosition = left_leg_delta_position + InitialLeftLegTargetPosition;


        /*
        The torso is rotated, therefore it is the x axis, not the y axis for moving vertically
        */

        TorsoTransform.localPosition = new Vector3(torso_delta_position.y, torso_delta_position.x) + InitialTorsoPosition;
        HipTransform.localPosition = hip_delta_position + InitialHipPosition;
        PostureTarget.localPosition = posture_delta_position + InitialPostureTargetPosition;



        // Resetting the positions to zero

        right_arm_delta_position = new Vector3(0, 0);
        left_arm_delta_position = new Vector3(0, 0);

        right_leg_delta_position = new Vector3(0, 0);
        left_leg_delta_position = new Vector3(0, 0);

        torso_delta_position = new Vector3(0, 0);
        hip_delta_position = new Vector3(0, 0);
        posture_delta_position = new Vector3(0, 0);
    }

    void AutoConnect()
    {
        RightArmTarget = transform.Find("Targets/AHand_Target");
        LeftArmTarget = transform.Find("Targets/BHand_Target");
        RightLegTarget = transform.Find("Targets/ALeg_Target");
        LeftLegTarget = transform.Find("Targets/BLeg_Target");
        TorsoTransform = transform.Find("Hip/Torso");
        HipTransform = transform.Find("Hip");
        PostureTarget = transform.Find("Targets/Posture_Target");


        Vector2 upper_arm_vector = transform.Find("Hip/Torso/AHand1").position - transform.Find("Hip/Torso/AHand1/AHand2").position;
        Vector2 lower_arm_vector = transform.Find("Hip/Torso/AHand1/AHand2").position - transform.Find("Hip/Torso/AHand1/AHand2/AHand3").position;

        float upper_arm_length = upper_arm_vector.magnitude;
        float lower_arm_length = lower_arm_vector.magnitude;

        armLength = upper_arm_length + lower_arm_length;
    }
    void SetInitialPositions()
    {
        InitialRightArmTargetPosition = RightArmTarget.localPosition;
        InitialLeftArmTargetPosition = LeftArmTarget.localPosition;
        InitialRightLegTargetPosition = RightLegTarget.localPosition;
        InitialLeftLegTargetPosition = LeftLegTarget.localPosition;
        InitialTorsoPosition = TorsoTransform.localPosition;
        InitialHipPosition = HipTransform.localPosition;
        InitialPostureTargetPosition = PostureTarget.localPosition;
    }
}
