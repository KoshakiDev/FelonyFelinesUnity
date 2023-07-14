using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This decision will return true if the Brain's current target tag is equal to desired tag, false otherwise
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionIsTargetWithTag")]
    public class AIDecisionIsTargetInLayer : AIDecision
    {

        //public string targetLayer = "";

        //public bool includeSelfInDetection = false;

        /// <summary>
        /// On Decide we check whether the Target is equal to desired tag
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return CheckIfTargetInLayer();
        }

        /// <summary>
        /// Returns true if the Brain's Target is desired tag
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckIfTargetInLayer()
        {
            return false;
            /*
            if (_brain.Target == null)
            {
                return false;
            }

            if (_brain.Target == _brain.Owner && !includeSelfInDetection)
            {
                return false;
            }

            Debug.Log("The brain is " + _brain.Owner + " The target is " + _brain.Target + " who has a tag " + _brain.Target.tag + "and I am looking for " + targetTag);


            if (_brain.Target.layer == targetLayer)
            {
                return true;
            }
            else
            {
                return false;
            }
            */
        }
    }
}