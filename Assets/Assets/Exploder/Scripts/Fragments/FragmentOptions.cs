// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using UnityEngine;

namespace Exploder
{
    [Serializable]
    public class FragmentOption
    {
        public GameObject FragmentPrefab;

        public bool FreezePositionX;
        public bool FreezePositionY;
        public bool FreezePositionZ;
        public bool FreezeRotationX;
        public bool FreezeRotationY;
        public bool FreezeRotationZ;

        public string Layer;

        /// <summary>
        /// flag for destroying already destroyed fragments
        /// if this is true you can destroy object and all the new created fragments
        /// you can keep destroying fragments until they are small enough (see Fragment.cs)
        /// </summary>
        public bool ExplodeFragments = true;

        /// <summary>
        /// maximal velocity the fragment can fly
        /// </summary>
        public float MaxVelocity;

        /// <summary>
        /// if set to true, mass, velocity and angular velocity will be inherited from original game object
        /// </summary>
        public bool InheritParentPhysicsProperty;

        /// <summary>
        /// mass property which will apply to fragments
        /// NOTE: if the parent object object has rigidbody and InheritParentPhysicsProperty is true
        /// the mass property for fragments will be calculated based on this equation (fragmentMass = parentMass / settings.TargetFragments)
        /// </summary>
        public float Mass;

        /// <summary>
        /// gravity settings
        /// </summary>
        public bool UseGravity;

        /// <summary>
        /// disable collider on fragments
        /// </summary>
        public bool DisableColliders;

        /// <summary>
        /// using mesh colliders for fragments
        /// NOTE: don't use it unless you have to, mesh colliders are very slow
        /// </summary>
        public bool MeshColliders = false;

        /// <summary>
        /// angular velocity of fragments
        /// </summary>
        public float AngularVelocity;

        /// <summary>
        /// maximal angular velocity of fragment
        /// </summary>
        public float MaxAngularVelocity;

        /// <summary>
        /// direction of angular velocity
        /// </summary>
        public Vector3 AngularVelocityVector;

        /// <summary>
        /// set this to true if you want to have randomly rotated fragments
        /// </summary>
        public bool RandomAngularVelocityVector;

        /// <summary>
        /// optional parameter to use different material for fragment pieces
        /// if not set the default (first) material is chosen from the original object
        /// </summary>
        public Material FragmentMaterial;

        public FragmentOption Clone()
        {
            return new FragmentOption
            {
                ExplodeFragments = ExplodeFragments,
                FreezePositionX = FreezePositionX,
                FreezePositionY = FreezePositionY,
                FreezePositionZ = FreezePositionZ,
                FreezeRotationX = FreezeRotationX,
                FreezeRotationY = FreezeRotationY,
                FreezeRotationZ = FreezeRotationZ,
                Layer = Layer,
                Mass = Mass,
                DisableColliders = DisableColliders,
                MeshColliders = MeshColliders,
                UseGravity = UseGravity,
                MaxVelocity = MaxVelocity,
                MaxAngularVelocity = MaxAngularVelocity,
                InheritParentPhysicsProperty = InheritParentPhysicsProperty,
                AngularVelocity = AngularVelocity,
                AngularVelocityVector = AngularVelocityVector,
                RandomAngularVelocityVector = RandomAngularVelocityVector,
                FragmentMaterial = FragmentMaterial,
            };
        }
    }
}
