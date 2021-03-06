﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ModifiedObject.Scripts.Game.Misc
{

    [RequireComponent(typeof(ARTrackedImageManager), typeof(ARPlaneManager))]
    public class TargetPlacerManager : Utils.EventContainerComponent
    {

        [SerializeField]    
        private TargetPlacerReference targetPlacerReference; 
        [SerializeField]
        private Utils.References.BooleanReference foundTarget;

        private ARTrackedImageManager _trackedImageManager;
        private ARPlaneManager _planeManager;

        protected override void OnStart()
        {
            this._trackedImageManager = this.GetComponent<ARTrackedImageManager>();
            this._planeManager = this.GetComponent<ARPlaneManager>();       
            this.OnTargetPlacerChanged(this.targetPlacerReference.Value);     
        }

        protected override void HookEvents()
        {
            this.targetPlacerReference.TargetPlacerChanged += this.OnTargetPlacerChanged;
            this.foundTarget.ChangedValueEvent += this.OnFoundTarget;
        }

        protected override void UnHookEvents()
        {
            this.targetPlacerReference.TargetPlacerChanged -= this.OnTargetPlacerChanged;
            this.foundTarget.ChangedValueEvent -= this.OnFoundTarget;
        }

        private void OnTargetPlacerChanged(TargetPlacerType targetPlacer)
        {
            switch(targetPlacer)
            {
                case TargetPlacerType.TYPE_IMAGE_TRACKING:
                    this._trackedImageManager.enabled = true;
                    this._planeManager.enabled = false;
                    break;
                case TargetPlacerType.TYPE_PLANE_TRACKING:
                    this._trackedImageManager.enabled = false;
                    this._planeManager.enabled = true;
                    break;
            }
        }

        private void OnFoundTarget(bool foundTarget)
        {
            this._planeManager.enabled = !foundTarget;        
        }
    }
}

