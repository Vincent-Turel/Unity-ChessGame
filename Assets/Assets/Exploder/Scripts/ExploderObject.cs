// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using UnityEngine;
using System.Collections.Generic;

namespace Exploder
{
    /// <summary>
    /// main Exploder interface class
    /// </summary>
    public class ExploderObject : MonoBehaviour
    {
        /// <summary>
        /// name of the tag for destroyable objects
        /// only objects with this tag can be destroyed, other objects are ignored
        /// </summary>
        public static string Tag = "Exploder";

        /// <summary>
        /// flag for not tagging Explodable objects
        /// if you set this to TRUE you will have to assign "Explodable" to your GameObject instead of Tagging it
        /// this is useful if you already have tagged GameObject and you don't want to re-tag it to "Exploder"
        /// </summary>
        public bool DontUseTag = false;

        /// <summary>
        /// radius of explosion
        /// see red wire-frame sphere inside scene view
        /// </summary>
        public float Radius = 10;

        /// <summary>
        /// cubic radius of explosion
        /// see red wire-frame cube inside scene view
        /// </summary>
        public Vector3 CubeRadius = Vector3.zero;

        /// <summary>
        /// flag for using cubic radius
        /// </summary>
        public bool UseCubeRadius = false;

        /// <summary>
        /// vector of explosion force
        /// NOTE: this parameter is used only if "settings.UseForceVector == true"
        /// ex.: with Vector(0, 0, 1) exploding fragments will fly in "UP" direction
        /// </summary>
        public Vector3 ForceVector = Vector3.up;

        /// <summary>
        /// flag for using "ForceVector"
        /// if this flag is false explosion force is distributed randomly on unit sphere (from sphere center to all directions)
        /// </summary>
        public bool UseForceVector;

        /// <summary>
        /// force of explosion
        /// more means higher velocity of exploding fragments
        /// </summary>
        public float Force = 30;

        /// <summary>
        /// time budget in [ms] for processing explosion calculation in one frame
        /// if the calculation takes more time it is stopped and resumed in next frame
        /// recommended settings: 15 - 30 (30 frame-per-second game takes approximately 33ms in one frame)
        /// for example:
        /// if your game is running 30 fps in average this value should be lower than 30 (~ 15 can be ok)
        /// if your game is running 60 fps in average this value can be 30 and more
        /// in other words, higher the value is faster the calculation is finished but more time in one frame can take
        /// </summary>
        public float FrameBudget = 15;

        /// <summary>
        /// number of target fragments that will be created by cutting the exploding objects
        /// more fragments means more calculation and more PhysX overhead
        /// </summary>
        public int TargetFragments = 30;

        /// <summary>
        /// option for using multi-threading
        /// </summary>
        public enum ThreadOptions
        {
            /// <summary>
            /// used 1 extra worker thread
            /// </summary>
            WorkerThread1x,

            /// <summary>
            /// used 2 extra worker threads
            /// </summary>
            WorkerThread2x,

            /// <summary>
            /// used 3 extra worker threads
            /// </summary>
            WorkerThread3x,

            /// <summary>
            /// multithreading disabled, using only Unity main thread
            /// </summary>
            Disabled,
        }

        /// <summary>
        /// maximum number of threads used for cutting, minimum is 1 (main thread)
        /// </summary>
        public ThreadOptions ThreadOption = ThreadOptions.WorkerThread3x;

        /// <summary>
        /// flag for destroying this GameObject if there is any mesh
        /// </summary>
        public bool ExplodeSelf = true;

        /// <summary>
        /// disable scanning for explodable objects in radius
        /// this options is valid only if ExplodeSelf is true
        /// </summary>
        public bool DisableRadiusScan = false;

        /// <summary>
        /// flag for hiding this game object after explosion
        /// </summary>
        public bool HideSelf = true;

        /// <summary>
        /// flag for destroying game object after explosion
        /// </summary>
        public bool DestroyOriginalObject = false;

        /// <summary>
        /// by enabling this Exploder will handle all objets in radius equally
        /// they will have the same number of fragments
        /// </summary>
        public bool UniformFragmentDistribution = false;

        /// <summary>
        /// option for separating not-connecting parts of the same mesh
        /// if this option is enabled all exploding fragments are searched for not connecting 
        /// parts of the same mesh and these parts are separated into new fragments
        /// example:
        /// if you explode a "chair" model, mesh cutter cut it into pieces however it is likely
        /// possible that one of the fragments will contain not-connecting "chair legs" (no sitting part) 
        /// and it will look not very realistic, by enabling this all not connecting "chair legs" are found 
        /// and split into different meshes
        /// 
        /// IMPORTANT: by enabling this you can achieve better visual quality but it will take more CPU power
        /// (more frames to process the explosion)
        /// </summary>
        public bool SplitMeshIslands = false;

        /// <summary>
        /// by enabling this option exploder will also cut non-closed meshes, for example vehicle with windows (mesh with holes)
        /// however it might not triangulate properly the mesh because of the original hole
        /// use this option with models that doesn't explode or only explode in very few fragments
        /// </summary>
        public bool DisableTriangulation;

        /// <summary>
        /// maximum number of all available fragments
        /// this number should be higher than TargetFragments
        /// </summary>
        public int FragmentPoolSize = 200;

        /// <summary>
        /// if enabled this will use 2d collision rigid bodies, valid only from Unity 4.3
        /// </summary>
        public bool Use2DCollision;

        /// <summary>
        /// option for angle of cutting plane
        /// </summary>
        public enum CuttingStyleOption
        {
            /// <summary>
            /// default for most realistic results
            /// </summary>
            Random,

            /// <summary>
            /// fragments will have rectangular shape, rectangular axis is chosen randomly
            /// </summary>
            RectangularRandom,

            /// <summary>
            /// fragments will have rectangular shape always cut with longest side
            /// </summary>
            RectangularRegular,
        }

        /// <summary>
        /// style of plane cutting
        /// </summary>
        public CuttingStyleOption CuttingStyle;

        /// <summary>
        /// deactivation of fragments
        /// </summary>
        public FragmentDeactivation FragmentDeactivation = new FragmentDeactivation
        {
            DeactivateOptions = DeactivateOptions.OutsideOfCamera,
            DeactivateTimeout = 10.0f,
            FadeoutOptions = FadeoutOptions.None,
        };

        /// <summary>
        /// particles and sound options
        /// </summary>
        public FragmentSFX FragmentSFX = new FragmentSFX
        {
            ChanceToPlay = 100,
            PlayOnlyOnce = false,
            MixMultipleSounds = false,
            EmitersMax = 1000,
            ParticleTimeout = 5.0f,
        };

        /// <summary>
        /// global settings for fragment options
        /// constrains for rigid bodies and name of the layer
        /// </summary>
        public FragmentOption FragmentOptions = new FragmentOption
        {
            FreezePositionX = false,
            FreezePositionY = false,
            FreezePositionZ = false,
            FreezeRotationX = false,
            FreezeRotationY = false,
            FreezeRotationZ = false,
            Layer = "Default",
            Mass = 20,
            MaxVelocity = 1000,
            DisableColliders = false,
            MeshColliders = false,
            UseGravity = true,
            InheritParentPhysicsProperty = true,
            AngularVelocity = 1.0f,
            AngularVelocityVector = Vector3.up,
            MaxAngularVelocity = 7,
            RandomAngularVelocityVector = true,
            FragmentMaterial = null,
        };

        /// <summary>
        /// explosion callback
        /// this callback is called when the calculation is finished and the physics explosion is started
        /// this is useful for playing explosion sound effect, particles etc.
        /// </summary>
        public delegate void OnExplosion(float timeMS, ExplosionState state);

        /// <summary>
        /// state of explosion, this enum used as parameter in callback
        /// </summary>
        public enum ExplosionState
        {
            /// <summary>
            /// explosion just started to show flying fragment pieces, but it can take several frames to
            /// start all pieces (activate rigid bodies, etc...)
            /// this is a good place to play explosion soundeffects
            /// </summary>
            ExplosionStarted,

            /// <summary>
            /// explosion process is finally completed, all fragment pieces are generated and visible
            /// this is a good place to get all active fragments and do watever necessery (particles, FX, ...)
            /// </summary>
            ExplosionFinished,

            /// <summary>
            /// cracking of the object has finished
            /// </summary>
            ObjectCracked,
        }

        /// <summary>
        /// search and explode objects in radius
        /// </summary>
        public void ExplodeRadius()
        {
            ExplodeRadius(null);
        }

        /// <summary>
        /// search and explode objects in radius
        /// </summary>
        /// <param name="callback">callback to be called when explosion calculation is finished
        /// play your sound effects or particles on this callback
        /// </param>
        public void ExplodeRadius(OnExplosion callback)
        {
            core.Enqueue(this, callback, false, null);
        }

        /// <summary>
        /// explode single object
        /// </summary>
        /// <param name="obj">game object to be exploded</param>
        public void ExplodeObject(GameObject obj)
        {
            ExplodeObject(obj, null);
        }

        /// <summary>
        /// explode single object with callback
        /// </summary>
        /// <param name="obj">game object to be exploded</param>
        /// <param name="callback">callback to be called when explosion calculation is finished
        /// play your sound effects or particles on this callback</param>
        public void ExplodeObject(GameObject obj, OnExplosion callback)
        {
            core.Enqueue(this, callback, false, obj);
        }

		public void ExplodeObjects(params GameObject[] objects)
		{
			ExplodeObjects(null, objects);
		}

		public void ExplodeObjects(OnExplosion callback, params GameObject[] objects)
		{
			core.Enqueue (this, callback, false, objects);
		}

        public void ExplodePartial(GameObject obj, Vector3 shotDir, Vector3 hitPosition, float bulletSize)
        {
            ExplodePartial(obj, shotDir, hitPosition, bulletSize, null);
        }

        public void ExplodePartial(GameObject obj, Vector3 shotDir, Vector3 hitPosition, float bulletSize, OnExplosion callback)
        {
            core.ExplodePartial(obj, shotDir, hitPosition, bulletSize, callback);
        }

        /// <summary>
        /// crack will calculate fragments and prepare object for explosion
        /// Use this method in combination with ExplodeCracked()
        /// Purpose of this method is to get higher performance of explosion, Crack() will 
        /// calculate the explosion and prepare all fragments. Calling ExplodeCracked() will 
        /// then start the explosion (flying fragments...) immediately
        /// </summary>
        public void CrackRadius()
        {
            CrackRadius(null);
        }

        /// <summary>
        /// crack will calculate fragments and prepare object for explosion
        /// Use this method in combination with ExplodeCracked()
        /// Purpose of this method is to get higher performance of explosion, Crack() will 
        /// calculate the explosion and prepare all fragments. Calling ExplodeCracked() will 
        /// then start the explosion (flying fragments...) immediately
        /// </summary>
        public void CrackRadius(OnExplosion callback)
        {
            core.Enqueue(this, callback, true, null);
        }

        /// <summary>
        /// crack single object, use in combination with ExplodeCracked(...)
        /// </summary>
        /// <param name="obj">object to be cracked</param>
        public void CrackObject(GameObject obj)
        {
            CrackObject(obj, null);
        }

        /// <summary>
        /// crack single object, use in combination with ExplodeCracked(...)
        /// </summary>
        /// <param name="obj">object to be cracked</param>
        /// <param name="callback"></param>
        public void CrackObject(GameObject obj, OnExplosion callback)
        {
            core.Enqueue(this, callback, true, obj);
        }

        /// <summary>
        /// returns true if the object can be cracked with the current settings
        /// </summary>
        public bool CanCrack()
        {
            return TargetFragments < FragmentPool.Instance.GetAvailableCrackFragmentsCount();
        }

        public bool IsCracked(GameObject gm)
        {
            return core.IsCracked(gm);
        }

        /// <summary>
        /// explode cracked objects
        /// Use this method in combination with Crack()
        /// Purpose of this method is to get higher performance of explosion, Crack() will
        /// calculate the explosion and prepare all fragments. Calling ExplodeCracked() will
        /// then start the explosion (flying fragments...) immediately
        /// </summary>
        public void ExplodeCracked(GameObject obj, OnExplosion callback)
        {
            core.ExplodeCracked(obj, callback);
        }

        public void ExplodeCracked(GameObject obj)
        {
            core.ExplodeCracked(obj, null);
        }

        public void ExplodeCracked(OnExplosion callback)
        {
            ExplodeCracked(null, callback);
        }

        /// <summary>
        /// explode cracked objects
        /// Use this method in combination with Crack()
        /// Purpose of this method is to get higher performance of explosion, Crack() will 
        /// calculate the explosion and prepare all fragments. Calling ExplodeCracked() will 
        /// run the explosion immediately.
        /// </summary>
        public void ExplodeCracked()
        {
            ExplodeCracked(null, null);
        }

        public int ProcessingFrames { get { return core.processingFrames; } }

        private Core core;

        private void Awake()
        {
            core = Core.Instance;
            core.Initialize(this);
        }

        void OnDrawGizmos()
        {
            if (enabled && !(ExplodeSelf && DisableRadiusScan))
            {
                Gizmos.color = Color.red;

                if (UseCubeRadius)
                {
					var pos = ExploderUtils.GetCentroid(gameObject);
					Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(transform.InverseTransformPoint(pos), CubeRadius);
                }
                else
                {
                    Gizmos.DrawWireSphere(ExploderUtils.GetCentroid(gameObject), Radius);
                }
            }
        }

        bool IsExplodable(GameObject obj)
        {
            if (core.parameters.DontUseTag)
            {
                return obj.GetComponent<Explodable>() != null;
            }
            else
            {
                return obj.CompareTag(ExploderObject.Tag);
            }
        }

        public ExploderParams Params => core.parameters;
    }
}
