// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Exploder
{
    /// <summary>
    /// script component for fragment game object
    /// the only logic here is visibility test against main camera and timeout sleeping for rigidbody
    /// </summary>
    public class Fragment : MonoBehaviour
    {
        /// <summary>
        /// if this fragment is cracked and ready for explosion
        /// </summary>
        [NonSerialized]
        public bool Cracked;

        /// <summary>
        /// flag if this fragment is visible from main camera
        /// </summary>
        [NonSerialized]
        public bool Visible;

        /// <summary>
        /// is this fragment active
        /// </summary>
        [NonSerialized]
        public bool IsActive;

        /// <summary>
        /// mesh renderer component for faster access
        /// </summary>
        [NonSerialized]
        public MeshRenderer meshRenderer;

        /// <summary>
        /// mesh collider component for faster access
        /// </summary>
        [NonSerialized]
        public MeshCollider meshCollider;

        /// <summary>
        /// mesh filter component for faster access
        /// </summary>
        [NonSerialized]
        public MeshFilter meshFilter;

        /// <summary>
        /// box collider component for faster access
        /// </summary>
        [NonSerialized]
        public BoxCollider boxCollider;

        /// <summary>
        /// custom 2d collider when exploding 2d objects
        /// </summary>
        [NonSerialized]
        public PolygonCollider2D polygonCollider2D;

        /// <summary>
        /// optional audio source on this fragment
        /// </summary>
        [NonSerialized]
        public AudioSource audioSource;

        private ParticleSystem[] particleSystems;
        private GameObject particleChild;
        private Rigidbody2D rigid2D;
        private Rigidbody rigidBody;
        private ExploderParams settings;
        private Vector3 originalScale;
        private float visibilityCheckTimer;
        private float deactivateTimer;
        private float emmitersTimeout;
        private bool stopEmitOnTimeout;
        private bool collided;
        private static AudioSource activePlayback;

        public bool IsSleeping()
        {
            if (rigid2D)
            {
                return rigid2D.IsSleeping();
            }
            return rigidBody.IsSleeping();
        }

        public void Sleep()
        {
            if (rigid2D)
            {
                rigid2D.Sleep();
            }
            else
            {
                rigidBody.Sleep();
            }
        }

        public void WakeUp()
        {
            if (rigid2D)
            {
                rigid2D.WakeUp();
            }
            else
            {
                rigidBody.WakeUp();
            }
        }

        public void SetConstraints(RigidbodyConstraints constraints)
        {
            if (rigidBody)
            {
                rigidBody.constraints = constraints;
            }
        }

        public void InitSFX(FragmentSFX sfx)
        {
            if (sfx.FragmentEmitter)
            {
                if (!particleChild)
                {
                    var dup = Instantiate(sfx.FragmentEmitter) as GameObject;

                    if (dup)
                    {
                        dup.transform.position = Vector3.zero;
                        particleChild = new GameObject("Particles");
                        particleChild.transform.parent = gameObject.transform;

                        dup.transform.parent = particleChild.transform;
                    }
                }

                if (particleChild)
                {
                    particleSystems = particleChild.GetComponentsInChildren<ParticleSystem>();
                }

                emmitersTimeout = sfx.ParticleTimeout;
                stopEmitOnTimeout = emmitersTimeout > 0.0f;
            }
            else
            {
                if (particleChild)
                {
                    Destroy(particleChild);
                    sfx.ParticleTimeout = -1.0f;
                    stopEmitOnTimeout = false;
                }
            }
        }

        void OnCollisionEnter()
        {
            if (!settings.FragmentSFX.MixMultipleSounds)
            {
                if (activePlayback && activePlayback.isPlaying)
                {
                    return;
                }
            }

            var inHitLimit = !settings.FragmentSFX.PlayOnlyOnce || !collided;
            var chanceToPlay = UnityEngine.Random.Range(0, 100) <= settings.FragmentSFX.ChanceToPlay;

            collided = true;

            if (inHitLimit && chanceToPlay)
            {
                if (audioSource)
                {
                    audioSource.Play();
                    activePlayback = audioSource;
                }
            }
        }

        public void DisableColliders(bool disable, bool meshColliders, bool physics2d)
        {
            if (disable)
            {
                if (physics2d)
                {
                    Object.Destroy(polygonCollider2D);
                }
                else
                {
                    if (meshCollider)
                    {
                        Object.Destroy(meshCollider);
                    }
                    if (boxCollider)
                    {
                        Object.Destroy(boxCollider);
                    }
                }
            }
            else
            {
                if (physics2d)
                {
                    if (!polygonCollider2D)
                    {
                        polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
                    }
                }
                else
                {
                    if (meshColliders)
                    {
                        if (!meshCollider)
                        {
                            meshCollider = gameObject.AddComponent<MeshCollider>();
                        }
                    }
                    else
                    {
                        if (!boxCollider)
                        {
                            boxCollider = gameObject.AddComponent<BoxCollider>();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// apply physical explosion to fragment piece
        /// </summary>
        public void ApplyExplosion(ExploderTransform meshTransform, Vector3 centroid, float force, GameObject original, ExploderParams set)
        {
            this.settings = set;

            if (rigid2D)
            {
                ApplyExplosion2D(meshTransform, centroid, force, original);
                return;
            }

            var rigid = rigidBody;

            // apply fragment mass and velocity properties
            var parentVelocity = Vector3.zero;
            var parentAngularVelocity = Vector3.zero;
            var mass = settings.FragmentOptions.Mass;
            var useGravity = settings.FragmentOptions.UseGravity;

            rigid.maxAngularVelocity = settings.FragmentOptions.MaxAngularVelocity;

            // inherit velocity and mass from original object
            if (settings.FragmentOptions.InheritParentPhysicsProperty)
            {
                if (original && original.GetComponent<Rigidbody>())
                {
                    var parentRigid = original.GetComponent<Rigidbody>();

                    parentVelocity = parentRigid.velocity;
                    parentAngularVelocity = parentRigid.angularVelocity;
                    mass = parentRigid.mass / settings.TargetFragments;
                    useGravity = parentRigid.useGravity;
                }
            }

            var forceVector = (meshTransform.TransformPoint(centroid) - settings.Position).normalized;
            var angularVelocity = settings.FragmentOptions.AngularVelocity * (settings.FragmentOptions.RandomAngularVelocityVector ? Random.onUnitSphere : settings.FragmentOptions.AngularVelocityVector);

            if (settings.UseForceVector)
            {
                forceVector = settings.ForceVector;
            }

            rigid.velocity = forceVector * force + parentVelocity;
            rigid.angularVelocity = angularVelocity + parentAngularVelocity;
            rigid.mass = mass;
            rigid.useGravity = useGravity;
        }

        /// <summary>
        /// apply physical explosion to fragment piece (2D case)
        /// </summary>
        void ApplyExplosion2D(ExploderTransform meshTransform, Vector3 centroid, float force, GameObject original)
        {
            var rigid = rigid2D;

            // apply fragment mass and velocity properties
            var parentVelocity = Vector2.zero;
            var parentAngularVelocity = 0.0f;
            var mass = settings.FragmentOptions.Mass;

            // inherit velocity and mass from original object
            if (settings.FragmentOptions.InheritParentPhysicsProperty)
            {
                if (original && original.GetComponent<Rigidbody2D>())
                {
                    var parentRigid = original.GetComponent<Rigidbody2D>();

                    parentVelocity = parentRigid.velocity;
                    parentAngularVelocity = parentRigid.angularVelocity;
                    mass = parentRigid.mass / settings.TargetFragments;
                }
            }

            Vector2 forceVector = (meshTransform.TransformPoint(centroid) - settings.Position).normalized;
            float angularVelocity = settings.FragmentOptions.AngularVelocity * (settings.FragmentOptions.RandomAngularVelocityVector ? Random.insideUnitCircle.x : settings.FragmentOptions.AngularVelocityVector.y);

            if (settings.UseForceVector)
            {
                forceVector = settings.ForceVector;
            }

            rigid.velocity = forceVector * force + parentVelocity;
            rigid.angularVelocity = angularVelocity + parentAngularVelocity;
            rigid.mass = mass;
        }


        /// <summary>
        /// refresh local members components objects
        /// </summary>
        public void RefreshComponentsCache()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
            boxCollider = GetComponent<BoxCollider>();
            rigidBody = GetComponent<Rigidbody>();
            rigid2D = GetComponent<Rigidbody2D>();
            polygonCollider2D = GetComponent<PolygonCollider2D>();
        }

        /// <summary>
        /// this is called from exploder class to start the explosion
        /// </summary>
        public void Explode(ExploderParams parameters)
        {
            this.settings = parameters;

            IsActive = true;
            ExploderUtils.SetActiveRecursively(gameObject, true);
            visibilityCheckTimer = 0.1f;
            Visible = true;
            Cracked = false;
            collided = false;
            deactivateTimer = settings.FragmentDeactivation.DeactivateTimeout;
            originalScale = transform.localScale;

            if (settings.FragmentOptions.ExplodeFragments)
            {
                tag = ExploderObject.Tag;
            }

            Emit(true);
        }

        public void Emit(bool centerToBound)
        {
            if (particleSystems != null)
            {
                if (centerToBound)
                {
                    if (particleChild && meshRenderer)
                    {
                        particleChild.transform.position = meshRenderer.bounds.center;
                    }
                }

                foreach (var psystem in particleSystems)
                {
                    psystem.Clear();
                    psystem.Play();
                }
            }
        }

        /// <summary>
        /// deactivate this fragment piece
        /// </summary>
        public void Deactivate()
        {
            if (activePlayback == audioSource)
            {
                activePlayback = null;
            }

            Sleep();
            ExploderUtils.SetActiveRecursively(gameObject, false);
            Visible = false;
            IsActive = false;

            if (meshFilter && meshFilter.sharedMesh)
            {
                DestroyImmediate(meshFilter.sharedMesh, true);
            }

            // turn off particles
            if (particleSystems != null)
            {
                foreach (var psystem in particleSystems)
                {
                    psystem.Clear();
                }
            }
        }

        public void AssignMesh(UnityEngine.Mesh mesh)
        {
            //
            // destroy mesh if still exists
            //
            if (meshFilter.sharedMesh)
            {
                DestroyImmediate(meshFilter.sharedMesh, true);
            }

            meshFilter.sharedMesh = mesh;
        }

        void Start()
        {
            visibilityCheckTimer = 1.0f;
            RefreshComponentsCache();
            Visible = false;
        }

        void Update()
        {
            if (IsActive)
            {
                var maxVelocity = settings.FragmentOptions.MaxVelocity;

                //
                // clamp velocity
                //
                if (rigidBody)
                {
                    if (rigidBody.velocity.sqrMagnitude > maxVelocity * maxVelocity)
                    {
                        var vel = rigidBody.velocity.normalized;
                        rigidBody.velocity = vel * maxVelocity;
                    }
                }
                else if (rigid2D)
                {
                    if (rigid2D.velocity.sqrMagnitude > maxVelocity * maxVelocity)
                    {
                        var vel = rigid2D.velocity.normalized;
                        rigid2D.velocity = vel * maxVelocity;
                    }
                }

                if (settings.FragmentDeactivation.DeactivateOptions == DeactivateOptions.Timeout)
                {
                    deactivateTimer -= Time.deltaTime;

                    if (deactivateTimer < 0.0f)
                    {
                        Deactivate();

                        // return fragment to previous fadout state
                        switch (settings.FragmentDeactivation.FadeoutOptions)
                        {
                            case FadeoutOptions.FadeoutAlpha:
                                break;
                        }
                    }
                    else
                    {
                        var t = deactivateTimer/settings.FragmentDeactivation.DeactivateTimeout;

                        switch (settings.FragmentDeactivation.FadeoutOptions)
                        {
                            case FadeoutOptions.FadeoutAlpha:
                                if (meshRenderer.material && meshRenderer.material.HasProperty("_Color"))
                                {
                                    var color = meshRenderer.material.color;
                                    color.a = t;
                                    meshRenderer.material.color = color;
                                }
                                break;

                            case FadeoutOptions.ScaleDown:
                                gameObject.transform.localScale = originalScale*t;
                                break;
                        }
                    }
                }

                // emmiter timeout
                if (stopEmitOnTimeout)
                {
                    emmitersTimeout -= Time.deltaTime;

                    if (emmitersTimeout < 0.0f)
                    {
                        // turn off particles
                        if (particleChild != null)
                        {
                            var psystem = particleChild.GetComponentInChildren<ParticleSystem>();

                            if (psystem)
                            {
                                psystem.Stop();
                            }
                        }

                        stopEmitOnTimeout = false;
                    }
                }

                visibilityCheckTimer -= Time.deltaTime;

                if (visibilityCheckTimer < 0.0f && UnityEngine.Camera.main)
                {
                    var viewportPoint = UnityEngine.Camera.main.WorldToViewportPoint(transform.position);

                    if (viewportPoint.z < 0 || viewportPoint.x < 0 || viewportPoint.y < 0 ||
                        viewportPoint.x > 1 || viewportPoint.y > 1)
                    {
                        if (settings.FragmentDeactivation.DeactivateOptions == DeactivateOptions.OutsideOfCamera)
                        {
                            Deactivate();
                        }

                        Visible = false;
                    }
                    else
                    {
                        Visible = true;
                    }

                    visibilityCheckTimer = Random.Range(0.1f, 0.3f);
                }
            }
        }
    }
}
