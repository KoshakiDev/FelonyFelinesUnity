using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Random = UnityEngine.Random;

namespace JonGearCustomScript
{
    public class BulletYPosition : MonoBehaviour
    {
        public void SetYPosition(float new_y)
        {
            foreach (Transform visual in transform)
            {
                if (visual.name == "DEBUG")
                    return;
                visual.transform.position = transform.position + new Vector3(0, new_y, 0);
            }
        }
    }
}
