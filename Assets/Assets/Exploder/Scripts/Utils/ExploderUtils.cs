// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace Exploder
{
    public static class ExploderUtils
    {
        /// <summary>
        /// just assert ...
        /// </summary>
        [Conditional("UNITY_EDITOR_NO_DEBUG")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                UnityEngine.Debug.LogError("Assert! " + message);
                UnityEngine.Debug.Break();
            }
        }

        /// <summary>
        /// just warning ...
        /// </summary>
        [Conditional("UNITY_EDITOR_NO_DEBUG")]
        public static void Warning(bool condition, string message)
        {
            if (!condition)
            {
                UnityEngine.Debug.LogWarning("Warning! " + message);
            }
        }

        /// <summary>
        /// unity log
        /// </summary>
        [Conditional("UNITY_EDITOR_NO_DEBUG")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// get centroid of the object (based on render bounds)
        /// </summary>
        public static Vector3 GetCentroid(GameObject obj)
        {
            var meshRenderer = obj.GetComponentsInChildren<MeshRenderer>();

            var centroid = Vector3.zero;

            if (meshRenderer == null || meshRenderer.Length == 0)
            {
                var skinnedMeshRenderer = obj.GetComponentInChildren<SkinnedMeshRenderer>();

                if (skinnedMeshRenderer)
                {
                    return skinnedMeshRenderer.bounds.center;
                }

                return obj.transform.position;
            }

            foreach (var meshRend in meshRenderer)
            {
                centroid += meshRend.bounds.center;
            }

            return centroid/meshRenderer.Length;
        }

        /// <summary>
        /// set this object visible to render
        /// </summary>
        public static void SetVisible(GameObject obj, bool status)
        {
            if (obj)
            {
                var renderers = obj.GetComponentsInChildren<MeshRenderer>();
                foreach (var meshRenderer in renderers)
                {
                    meshRenderer.enabled = status;
                }
            }
        }

#if UNITY_EDITOR_
        /// <summary>
        /// clear console
        /// </summary>
        public static void ClearLog()
        {
            Assembly assembly = Assembly.GetAssembly(typeof (UnityEditor.SceneView));

            Type type = assembly.GetType("UnityEditorInternal.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
#else
        public static void ClearLog() {}
#endif

        /// <summary>
        /// unity version specific isActive (to suppress warnings)
        /// </summary>
        public static bool IsActive(GameObject obj)
        {
            return obj && obj.activeSelf;
        }

        /// <summary>
        /// unity version specific SetActive (to suppress warnings)
        /// </summary>
        public static void SetActive(GameObject obj, bool status)
        {
            if (obj)
            {
                obj.SetActive(status);
            }
        }

        /// <summary>
        /// unity version specific SetActiveRecursively (to suppress warnings)
        /// </summary>
        public static void SetActiveRecursively(GameObject obj, bool status)
        {
            if (obj)
            {
                var childCount = obj.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    SetActiveRecursively(obj.transform.GetChild(i).gameObject, status);
                }
                obj.SetActive(status);
            }
        }

        /// <summary>
        /// enable colliders in object hiearchy
        /// </summary>
        public static void EnableCollider(GameObject obj, bool status)
        {
            if (obj)
            {
                var colliders = obj.GetComponentsInChildren<Collider>();

                foreach (var collider in colliders)
                {
                    collider.enabled = status;
                }
            }
        }

        /// <summary>
        /// returns true if the obj is valid explodable object
        /// </summary>
        public static bool IsExplodable(GameObject obj)
        {
            var explodable = obj.GetComponent<Explodable>() != null;

            if (!explodable)
            {
                explodable = obj.CompareTag(ExploderObject.Tag);
            }

            return explodable;
        }

        public static void CopyAudioSource(AudioSource src, AudioSource dst)
        {
            dst.bypassEffects = src.bypassEffects;
            dst.bypassListenerEffects = src.bypassListenerEffects;
            dst.bypassReverbZones = src.bypassReverbZones;
            dst.clip = src.clip;
            dst.dopplerLevel = src.dopplerLevel;
            dst.enabled = src.enabled;
            dst.ignoreListenerPause = src.ignoreListenerPause;
            dst.ignoreListenerVolume = src.ignoreListenerVolume;
            dst.loop = src.loop;
            dst.maxDistance = src.maxDistance;
            dst.minDistance = src.minDistance;
            dst.mute = src.mute;
            dst.outputAudioMixerGroup = src.outputAudioMixerGroup;
            dst.panStereo = src.panStereo;
            dst.pitch = src.pitch;
            dst.playOnAwake = src.playOnAwake;
            dst.priority = src.priority;
            dst.reverbZoneMix = src.reverbZoneMix;
            dst.rolloffMode = src.rolloffMode;
            dst.spatialBlend = src.spatialBlend;
            dst.spatialize = src.spatialize;
            dst.spread = src.spread;
            dst.time = src.time;
            dst.timeSamples = src.timeSamples;
            dst.velocityUpdateMode = src.velocityUpdateMode;
            dst.volume = src.volume;
        }
    }
}
