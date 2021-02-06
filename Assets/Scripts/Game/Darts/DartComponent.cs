﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModifiedObject.Scripts.Game
{

    [System.Serializable]
    public struct DartReferences
    {
        [SerializeField]
        public Utils.References.Vector3Reference originalPosition;
        [SerializeField]
        public Utils.References.Vector3Reference originalDirection;
        [SerializeField]
        public Utils.References.FloatReference originalForce;

        [SerializeField]
        public Utils.References.Vector3Reference targetPosition;
        [SerializeField]
        public Utils.References.Vector3Reference targetRotation;
        [SerializeField]
        public Utils.References.Vector3Reference targetFacing;
        [SerializeField]
        public Utils.References.FloatReference despawnCooldownTime;
    }

    [RequireComponent(typeof(Rigidbody))]
    public class DartComponent : Utils.EventContainerComponent, IDestroyable
    {
        [SerializeField]
        private DartReferences references;
        [SerializeField]
        private GameObject pointToTrack;

        private Rigidbody _rigidbody;

        private Vector3 _prevTargetPosition = Vector3.zero;
        private Vector3 _prevTargetEulers = Vector3.zero;

        private float _despawnCooldownTime = 0.0f;

        private bool _hitTarget = false;

        public Vector3 DartPosition
            => this.pointToTrack != null ? this.pointToTrack.transform.position
                : this.transform.position;

        /// <summary>
        /// Called when the dart is started.
        /// </summary>
        protected override void OnStart()
        {
            // Sets the start position.
            this.transform.position = this.references.originalPosition.Value;

            // Sets the look rotation.
            Quaternion quat = this.transform.rotation;
            quat.eulerAngles = this.references.originalDirection.Value;
            this.transform.rotation = quat;

            this._rigidbody = this.GetComponent<Rigidbody>();
            this._rigidbody.AddForce(
                this.transform.forward * this.references.originalForce.Value,
                ForceMode.Impulse);
        }

        protected override void HookEvents()
        {
            this.references.targetPosition.ChangedValueEvent += this.OnTargetChangedPosition;
            this.references.targetRotation.ChangedValueEvent += this.OnTargetChangedRotation;
        }

        protected override void UnHookEvents()
        {
            this.references.targetPosition.ChangedValueEvent -= this.OnTargetChangedPosition;
            this.references.targetRotation.ChangedValueEvent -= this.OnTargetChangedRotation;
        }

        private void Update()
        {
            if(this._hitTarget && this._despawnCooldownTime > 0.0f)
            {
                this._despawnCooldownTime -= Time.deltaTime;

                if(this._despawnCooldownTime <= 0.0f)
                {
                    Destroy(this.gameObject);
                }
            }

            Vector3 projection = Vector3.ProjectOnPlane(
                this.transform.position, this.references.targetFacing.Value);
            Debug.DrawLine(projection, this.transform.position);

        }

        /// <summary>
        /// Called when the dart collides with the target.
        /// </summary>
        /// <param name="collision">Gets the collision.</param>
        private void OnCollisionEnter(Collision collision)
        {
            TargetComponent target = collision.gameObject.GetComponent<TargetComponent>()
                ?? collision.gameObject.GetComponentInParent<TargetComponent>();
            if(target != null)
            {
                this._rigidbody.velocity = Vector3.zero;
                this._rigidbody.useGravity = false;
                this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;

                this._hitTarget = true;
                this._despawnCooldownTime = this.references.despawnCooldownTime.Value;

                this._prevTargetPosition = target.transform.position;
                this._prevTargetEulers = target.transform.eulerAngles;

                target.OnDartCollide(this);
            }
        }

        /// <summary>
        /// Called when this object has been his by the object destoyer.
        /// </summary>
        /// <param name="destroyer">The object destroyer.</param>
        public void OnDestroyerHit(ObjectDestroyer destroyer)
        {
            Destroy(this.gameObject);
        }

        private void OnTargetChangedPosition(Vector3 targetPosition)
        {
            if(!this._hitTarget)
            {
                this._prevTargetPosition = targetPosition;
                return;
            }

            Vector3 newTargetDifference = targetPosition - this._prevTargetPosition;
            this.transform.position += newTargetDifference;
            this._prevTargetPosition = targetPosition;
        }

        private void OnTargetChangedRotation(Vector3 targetRotation)
        {
            if(!this._hitTarget)
            {
                this._prevTargetEulers = targetRotation;
                return;
            }

            Vector3 newTargetDifference = targetRotation - this._prevTargetEulers;
            Quaternion quat = this.transform.rotation;
            quat.eulerAngles += newTargetDifference;
            this.transform.rotation = quat;
            this._prevTargetEulers = targetRotation;
        }
    }
}
