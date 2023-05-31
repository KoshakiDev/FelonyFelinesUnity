using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Random = UnityEngine.Random;

namespace MoreMountains.TopDownEngine
{
    public class BulletYPosition : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject model;

        public void SetYPosition(float new_y)
        {
            if (model == null)
            {
                return;
            }
            model.transform.position = new Vector2(model.transform.position.x, new_y);
        }
    }
}
