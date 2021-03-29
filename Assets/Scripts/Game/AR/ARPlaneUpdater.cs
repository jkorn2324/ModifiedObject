﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ModifiedObject.Scripts.Game
{

    [RequireComponent(typeof(ARPlane))]

    public class ARPlaneUpdater : Utils.EventContainerComponent
    {

        [SerializeField]
        private Utils.References.Vector2Reference maxSize;
        [SerializeField]
        private Utils.References.Vector3Reference centerPos;
        [SerializeField]
        private Utils.References.BooleanReference foundTarget;
        
        private ARPlane _arPlane;

        /// <summary>
        /// The start function.
        /// </summary>
        protected override void OnStart()
        {
            this._arPlane = this.GetComponent<ARPlane>();
        }

        protected override void HookEvents()
        {
            this.foundTarget.ChangedValueEvent += this.OnFoundPlaneTarget;
        }

        protected override void UnHookEvents()
        {
            this.foundTarget.ChangedValueEvent -= this.OnFoundPlaneTarget;
        }

        private void OnFoundPlaneTarget(bool foundTarget)
        {
            if(foundTarget)
            {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Called when the AR Plane is updated.
        /// </summary>
        private void Update()
        {
            Vector2 currentSize = this._arPlane.size;
            if(currentSize.x >= this.maxSize.Value.x
                && currentSize.y >= this.maxSize.Value.y)
            {
                this.centerPos.Value = this._arPlane.center;
                this.foundTarget.Value = true;
            }
        }
    }
}

