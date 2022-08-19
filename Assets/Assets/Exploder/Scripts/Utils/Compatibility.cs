// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace Exploder.Utils
{
    public static class Compatibility
    {
        /// <summary>
        /// set this object visible to render
        /// </summary>
        public static void SetVisible(GameObject obj, bool status, bool includeInactive)
        {
            if (obj)
            {
                var renderers = obj.GetComponentsInChildren<MeshRenderer>(includeInactive);
                foreach (var meshRenderer in renderers)
                {
                    meshRenderer.enabled = status;
                }

                var skinnedMeshes = obj.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive);
                foreach (var meshRenderer in skinnedMeshes)
                {
                    meshRenderer.enabled = status;
                }
            }
        }

        /// <summary>
        /// unity version specific isActive (to suppress warnings)
        /// </summary>
        public static bool IsActive(GameObject obj)
        {
#if !(UNITY_2_6	|| UNITY_2_6_1 || UNITY_3_0	|| UNITY_3_0_0 || UNITY_3_1	|| UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
            return obj && obj.activeSelf;
#else
        return obj && obj.active;
#endif
        }

        /// <summary>
        /// unity version specific SetActive (to suppress warnings)
        /// </summary>
        public static void SetActive(GameObject obj, bool status)
        {
#if !(UNITY_2_6	|| UNITY_2_6_1 || UNITY_3_0	|| UNITY_3_0_0 || UNITY_3_1	|| UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
            if (obj)
            {
                obj.SetActive(status);
            }
#else
        if (obj)
        {
            obj.active = status;
        }
#endif
        }

        /// <summary>
        /// unity version specific SetActiveRecursively (to suppress warnings)
        /// </summary>
        public static void SetActiveRecursively(GameObject obj, bool status)
        {
#if !(UNITY_2_6	|| UNITY_2_6_1 || UNITY_3_0	|| UNITY_3_0_0 || UNITY_3_1	|| UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
            if (obj)
            {
                var childCount = obj.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    SetActiveRecursively(obj.transform.GetChild(i).gameObject, status);
                }
                obj.SetActive(status);
            }
#else
        if (obj)
        {
            obj.SetActiveRecursively(status);
            obj.active = status;
        }
#endif
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

        public static void Destroy(UnityEngine.Object obj, bool allowDestroyingAssets)
        {
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(obj);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(obj, allowDestroyingAssets);
            }
        }

        public static void SetCursorVisible(bool status)
        {
#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
            Cursor.visible = status;
#else
            Screen.showCursor = status;
#endif
        }

        public static void LockCursor(bool status)
        {
#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
            Cursor.lockState = status?CursorLockMode.Locked : CursorLockMode.Confined;
#else
            Screen.lockCursor = status;
#endif
        }

        public static bool IsCursorLocked()
        {
#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
            return Cursor.lockState == CursorLockMode.Locked;
#else
            return Screen.lockCursor;
#endif
        }
    }
}
