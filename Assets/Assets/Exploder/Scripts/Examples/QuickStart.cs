using Exploder.Utils;
using UnityEngine;

public class QuickStart : MonoBehaviour
{
    void Start()
    {
        ExploderSingleton.Instance.ExplodeObject(gameObject);
    }
}
