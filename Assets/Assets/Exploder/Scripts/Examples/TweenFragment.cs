using UnityEngine;

//
// 1) Create an empty game object and assign it this sample script
//
// 2) Set input to the sample script: TargetPos and LerpTime in the inspector
//
// 3) Drag & drop it to "Exploder prefab" in the Exploder inspector
//
// 4) Disable colliders & Angular velocity & set Force to 0
//
// 5) Explode/Crack object as usual
//
public class TweenFragment : MonoBehaviour
{
    public Transform TargetPos;
    public float LerpTime;

    private Vector3 initPos;
    private float time;

	void Start()
	{
	    initPos = transform.position;
	    time = 0.0f;
	}

    void Update()
    {
        time += Time.deltaTime * Random.value * 2;

        transform.position = Vector3.Lerp(initPos, TargetPos.position, time / LerpTime);
    }
}
