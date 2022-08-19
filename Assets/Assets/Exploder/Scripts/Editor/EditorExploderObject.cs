// Version 1.7.4
// ©2016 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using Exploder;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (ExploderObject)), CanEditMultipleObjects]
public class EditorExploderObject : UnityEditor.Editor
{
    SerializedProperty radius;
    SerializedProperty cubeRadius;
    SerializedProperty useCubeRadius;
    SerializedProperty force;
    SerializedProperty targetFragments;
    SerializedProperty frameBudget;
    SerializedProperty useForceVector;
    SerializedProperty forceVector;
    SerializedProperty ignoreTag;
    SerializedProperty explodeSelf;
    SerializedProperty disableRadiusScan;
    SerializedProperty hideSelf;
    SerializedProperty deleteOriginalObjects;
    SerializedProperty uniformDistribution;
    SerializedProperty splitMeshIslands;
    SerializedProperty disableTriangulation;
    SerializedProperty user2dPhysics;
    SerializedProperty poolSize;
    SerializedProperty threadingOptions;
    SerializedProperty cuttingStyle;

    SerializedProperty meshColliders;
    SerializedProperty deactivateOn;
    SerializedProperty deactivateTimeout;
    SerializedProperty fadeout;
    SerializedProperty fragmentParticles;
    SerializedProperty fragmentParticlesTimeout;
    SerializedProperty fragmentParticlesMax;
    SerializedProperty fragmentMaterial;

    SerializedProperty layer;
    SerializedProperty maxVelocity;
    SerializedProperty maxAngularVelocity;
    SerializedProperty inheritParentPhysics;
    SerializedProperty mass;
    SerializedProperty useGravity;
    SerializedProperty disableColliders;
    SerializedProperty angularVelocity;
    SerializedProperty randomAngularVelocity;
    SerializedProperty angularVelocityVector;
    SerializedProperty freezePositionX;
    SerializedProperty freezePositionY;
    SerializedProperty freezePositionZ;
    SerializedProperty freezeRotationX;
    SerializedProperty freezeRotationY;
    SerializedProperty freezeRotationZ;
    SerializedProperty explodableFragments;
    SerializedProperty fragmentHitSoundChance;
    SerializedProperty fragmentHitSoundOnce;
    SerializedProperty fragmentMixMultipleSounds;

    private void OnEnable()
    {
        radius = serializedObject.FindProperty("Radius");
        cubeRadius = serializedObject.FindProperty("CubeRadius");
        useCubeRadius = serializedObject.FindProperty("UseCubeRadius");
        force = serializedObject.FindProperty("Force");
        targetFragments = serializedObject.FindProperty("TargetFragments");
        frameBudget = serializedObject.FindProperty("FrameBudget");
        useForceVector = serializedObject.FindProperty("UseForceVector");
        forceVector = serializedObject.FindProperty("ForceVector");
        ignoreTag = serializedObject.FindProperty("DontUseTag");
        explodeSelf = serializedObject.FindProperty("ExplodeSelf");
        disableRadiusScan = serializedObject.FindProperty("DisableRadiusScan");
        hideSelf = serializedObject.FindProperty("HideSelf");
        deleteOriginalObjects = serializedObject.FindProperty("DestroyOriginalObject");
        uniformDistribution = serializedObject.FindProperty("UniformFragmentDistribution");
        splitMeshIslands = serializedObject.FindProperty("SplitMeshIslands");
        disableTriangulation = serializedObject.FindProperty("DisableTriangulation");
        user2dPhysics = serializedObject.FindProperty("Use2DCollision");
        poolSize = serializedObject.FindProperty("FragmentPoolSize");
        threadingOptions = serializedObject.FindProperty("ThreadOption");
        cuttingStyle = serializedObject.FindProperty("CuttingStyle");

        //
        // fragments
        //
        deactivateOn = serializedObject.FindProperty("FragmentDeactivation.DeactivateOptions");
        deactivateTimeout = serializedObject.FindProperty("FragmentDeactivation.DeactivateTimeout");
        fadeout = serializedObject.FindProperty("FragmentDeactivation.FadeoutOptions");

        fragmentParticles = serializedObject.FindProperty("FragmentSFX.FragmentEmitter");
        fragmentParticlesMax = serializedObject.FindProperty("FragmentSFX.EmitersMax");
        fragmentParticlesTimeout = serializedObject.FindProperty("FragmentSFX.ParticleTimeout");

        layer = serializedObject.FindProperty("FragmentOptions.Layer");
        maxVelocity = serializedObject.FindProperty("FragmentOptions.MaxVelocity");
        maxAngularVelocity = serializedObject.FindProperty("FragmentOptions.MaxAngularVelocity");
        inheritParentPhysics = serializedObject.FindProperty("FragmentOptions.InheritParentPhysicsProperty");
        mass = serializedObject.FindProperty("FragmentOptions.Mass");
        useGravity = serializedObject.FindProperty("FragmentOptions.UseGravity");
        angularVelocity = serializedObject.FindProperty("FragmentOptions.AngularVelocity");
        randomAngularVelocity = serializedObject.FindProperty("FragmentOptions.RandomAngularVelocityVector");
        angularVelocityVector = serializedObject.FindProperty("FragmentOptions.AngularVelocityVector");
        freezePositionX = serializedObject.FindProperty("FragmentOptions.FreezePositionX");
        freezePositionY = serializedObject.FindProperty("FragmentOptions.FreezePositionY");
        freezePositionZ = serializedObject.FindProperty("FragmentOptions.FreezePositionZ");
        freezeRotationX = serializedObject.FindProperty("FragmentOptions.FreezeRotationX");
        freezeRotationY = serializedObject.FindProperty("FragmentOptions.FreezeRotationY");
        freezeRotationZ = serializedObject.FindProperty("FragmentOptions.FreezeRotationZ");
        fragmentMaterial = serializedObject.FindProperty("FragmentOptions.FragmentMaterial");

        explodableFragments = serializedObject.FindProperty("FragmentOptions.ExplodeFragments");
        meshColliders = serializedObject.FindProperty("FragmentOptions.MeshColliders");
        disableColliders = serializedObject.FindProperty("FragmentOptions.DisableColliders");

        fragmentHitSoundChance = serializedObject.FindProperty("FragmentSFX.ChanceToPlay");
        fragmentHitSoundOnce = serializedObject.FindProperty("FragmentSFX.PlayOnlyOnce");
        fragmentMixMultipleSounds = serializedObject.FindProperty("FragmentSFX.MixMultipleSounds");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var exploder = this.target as ExploderObject;

        if (exploder)
        {
            var change = false;

            EditorExploderUtils.Separator("Main Settings", 20);
            EditorGUILayout.Space();

            GUI.enabled = !(exploder.ExplodeSelf && exploder.DisableRadiusScan && useCubeRadius.boolValue);

			change |= EditorExploderUtils.SliderEdit("Radius", "Explosion radius.", 0.0f, 100, radius);

            GUI.enabled = true;

            change |= EditorExploderUtils.SliderEdit("Force", "Force of explosion.", 0.0f, 100, force);
            change |= EditorExploderUtils.SliderEdit("Target Fragments", "Number of target fragments.", 0, 500, targetFragments);

            if (exploder.ThreadOption == ExploderObject.ThreadOptions.Disabled)
            {
                change |= EditorExploderUtils.SliderEdit("Frame Budget [ms]", "Time budget in [ms] for processing explosion calculation in one frame.", 0.0f, 60.0f, frameBudget);
            }

            change |= EditorExploderUtils.Toggle("Use Force Vector", "Use this vector as a direction of explosion.", useForceVector);

            if (exploder.UseForceVector)
            {
                change |= EditorExploderUtils.Vector3("", "Use this vector as a direction of explosion.", forceVector);
            }

            change |= EditorExploderUtils.Toggle("Ignore Tag", "Ignore Exploder tag on object, use Explodable component instead.", ignoreTag);

            change |= EditorExploderUtils.Toggle("Explode self", "Explode this game object.", explodeSelf);

            if (exploder.ExplodeSelf)
            {
                change |= EditorExploderUtils.Toggle("Disable radius", "Disable scanning for objects in radius.", disableRadiusScan);
            }

            change |= EditorExploderUtils.Toggle("Hide self", "Hide this game object after explosion.", hideSelf);
            change |= EditorExploderUtils.Toggle("Delete original object", "Delete original game object after explosion.", deleteOriginalObjects);
            change |= EditorExploderUtils.Toggle("Uniform distribution", "Uniformly distribute fragments inside the radius.", uniformDistribution);
            change |= EditorExploderUtils.Toggle("Split mesh islands", "Split non-connecting part of the mesh into separate fragments.", splitMeshIslands);
            change |= EditorExploderUtils.Toggle("Disable triangulation", "Disable triangulation of fragments.", disableTriangulation);
            change |= EditorExploderUtils.Toggle("Use 2D physics", "Enable 2D explosion.", user2dPhysics);

			change |= EditorExploderUtils.Toggle("Use Cube Radius", "Explosion cubic radius.", useCubeRadius);

			if (useCubeRadius.boolValue)
			{
				change |= EditorExploderUtils.Vector3("", "Explosion cubic radius.", cubeRadius);
			}

            EditorExploderUtils.EnumSelection("Multi-threading", "Options for multi-threaded calculations.", exploder.ThreadOption, threadingOptions, ref change);

            EditorExploderUtils.EnumSelection("Cutting plane angle", "Style of cutting plane.", exploder.CuttingStyle, cuttingStyle, ref change);

//            EditorGUILayout.Space();
//            EditorExploderUtils.Separator("Partial explosion", 20);
//            EditorGUILayout.Space();

            EditorGUILayout.Space();
            EditorExploderUtils.Separator("Audio", 20);
            EditorGUILayout.Space();

            var hasAudioSource = exploder.gameObject.GetComponent<AudioSource>();

            if (!hasAudioSource && EditorExploderUtils.Button("Add Explosion Audio Source"))
            {
                var audioSource = exploder.gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }

            EditorGUILayout.Space();
            EditorExploderUtils.Separator("Fragment options", 20);
            EditorGUILayout.Space();

            change |= EditorExploderUtils.SliderEdit("Pool Size", "Size of the fragment pool, this value should be higher than TargetFragments.", 0, 1000, poolSize);

            change |= EditorExploderUtils.Toggle("Explodable fragments", "Enable fragments to be exploded again.", explodableFragments);
            change |= EditorExploderUtils.String("Layer", "Layer of the fragment game object.", layer);

            change |= EditorExploderUtils.Toggle("Mesh colliders", "Use mesh colliders for all fragments.", meshColliders);
            change |= EditorExploderUtils.Toggle("Disable colliders", "Disable colliders of all fragments.", disableColliders);

            change |= EditorExploderUtils.SliderEdit("MaxVelocity", "Maximal velocity that fragment can have.", 0.0f, 100.0f, maxVelocity);
            change |= EditorExploderUtils.SliderEdit("MaxAngularVelocity", "Maximal angular velocity that fragment can have.", 0.0f, 30.0f, maxAngularVelocity);
            change |= EditorExploderUtils.Toggle("Inherit parent physics", "Use the same physics settings as in original game object.", inheritParentPhysics);
            change |= EditorExploderUtils.SliderEdit("Mass", "Mass property of every fragment.", 0.0f, 100.0f, mass);
            change |= EditorExploderUtils.Toggle("Use gravity", "Apply gravity to fragment.", useGravity);

            change |= EditorExploderUtils.SliderEdit("Angular velocity", "Angular velocity of fragments.", 0.0f, 100.0f, angularVelocity);
            change |= EditorExploderUtils.Toggle("Random angular vector", "Randomize rotation of fragments.", randomAngularVelocity);

            if (!exploder.FragmentOptions.RandomAngularVelocityVector)
            {
                change |= EditorExploderUtils.Vector3("", "Use this vector as a angular velocity vector.", angularVelocityVector);
            }

            change |= EditorExploderUtils.Toggle3("Freeze Position", "Freeze position of the fragment in selected axis.",
                "x", "y", "z", freezePositionX,
                freezePositionY,
                freezePositionZ);

            change |= EditorExploderUtils.Toggle3("Freeze Rotation", "Freeze rotation of the fragment in selected axis.",
                "x", "y", "z", freezeRotationX,
                freezeRotationY,
                freezeRotationZ);

            change |= EditorExploderUtils.ObjectSelection<Material>("Material", "Optional material for fragments.",
                fragmentMaterial);

            EditorGUILayout.Space();
            EditorExploderUtils.Separator("Fragment Deactivation", 20);
            EditorGUILayout.Space();

            EditorExploderUtils.EnumSelection("Deactivate on", "Options for fragment deactivation.", exploder.FragmentDeactivation.DeactivateOptions, deactivateOn, ref change);

            if (exploder.FragmentDeactivation.DeactivateOptions == DeactivateOptions.Timeout)
            {
                change |= EditorExploderUtils.SliderEdit("Deactivate Timeout [s]", "Time in [s] to deactivate fragments.", 0.0f, 60.0f, deactivateTimeout);
                EditorExploderUtils.EnumSelection("FadeOut", "Option for fragments to fadeout during deactivation timeout.", exploder.FragmentDeactivation.FadeoutOptions, fadeout, ref change);

                if (exploder.FragmentDeactivation.FadeoutOptions == FadeoutOptions.FadeoutAlpha)
                {
                    EditorExploderUtils.WarningBox("Requires transparent shaders", 20);
                }
            }

            EditorGUILayout.Space();
            EditorExploderUtils.Separator("Fragment Particles", 20);
            EditorGUILayout.Space();

            change |= EditorExploderUtils.ObjectSelection<GameObject>("Particles prefab", "Particle effect that will start to emit from each fragment after explosion.", fragmentParticles);

            if (exploder.FragmentSFX.FragmentEmitter)
            {
                change |= EditorExploderUtils.IntEdit("Maximum emitters", "Maximumal number of emmiters.", 0, 1000, fragmentParticlesMax);

                bool isTimeout = fragmentParticlesTimeout.floatValue > 0.0f;
                var changeTimeout = EditorExploderUtils.ToggleBool("Particle timeout", "Timeout to deactivate partices.", ref isTimeout);
                change |= changeTimeout;

                if (isTimeout)
                {
                    if (changeTimeout)
                    {
                        fragmentParticlesTimeout.floatValue = 1.0f;
                    }

                    change |= EditorExploderUtils.SliderEdit("", "Timeout to deactivate partices.", 0.001f, 100.0f, fragmentParticlesTimeout);
                }
                else
                {
                    fragmentParticlesTimeout.floatValue = -1.0f;
                }
            }

            EditorGUILayout.Space();
            EditorExploderUtils.Separator("Fragment Audio", 20);
            EditorGUILayout.Space();

            var fragment = GameObject.FindObjectOfType<Fragment>();

            if (fragment && fragment.GetComponent<AudioSource>())
            {
                change |= EditorExploderUtils.SliderEdit("Hit sound chance %", "Chance to play the hit sound.", 0, 100, fragmentHitSoundChance);
                change |= EditorExploderUtils.Toggle("Play only once", "Play only one time per fragment.", fragmentHitSoundOnce);
                change |= EditorExploderUtils.Toggle("Mix multiple hit sounds", "Allow playing multiple hit sounds.", fragmentMixMultipleSounds);
            }
            else
            {
                EditorExploderUtils.WarningBox("Audio Source on Fragment Prefab required.", 20);
            }

            EditorGUILayout.Space();
            EditorExploderUtils.Separator("Fragment Prefab", 20);
            EditorGUILayout.Space();

            if (EditorExploderUtils.Button("Edit Fragment Prefab"))
            {
                GameObject fragmentObject = null;

                if (fragment)
                {
                    fragmentObject = fragment.gameObject;
                }
                else
                {
                    var fragmentPrefab = Resources.Load("ExploderFragment");

                    if (fragmentPrefab)
                    {
                        fragmentObject = Instantiate(fragmentPrefab) as GameObject;

                        if (fragmentObject)
                        {
                            fragmentObject.name = "ExploderFragment";
                        }
                    }
                }

                if (fragmentObject)
                {
                    Selection.activeGameObject = fragmentObject;
                }
            }

            if (change)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(exploder);
            }

            EditorGUILayout.Separator();
        }
    }
}
