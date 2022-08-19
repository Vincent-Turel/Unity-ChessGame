using UnityEngine;

namespace Exploder.Examples
{
    /// <summary>
    /// example how to explode every explodable (tagged) objects in the scene at once
    /// </summary>
    public class ExplodeAllObjects : MonoBehaviour
    {
        private ExploderObject Exploder;
        private GameObject[] DestroyableObjects;

        private void Start()
        {
            DestroyableObjects = GameObject.FindGameObjectsWithTag("Exploder");
            Exploder = Utils.ExploderSingleton.Instance;
        }

        private void Update()
        {
            // press enter to start explosions
            if (Input.GetKeyDown(KeyCode.Return))
            {
				Exploder.ExplodeObjects(DestroyableObjects);
            }
        }

        private void ExplodeObject(GameObject gameObject)
        {
            ExploderUtils.SetActive(Exploder.gameObject, true);
            Exploder.transform.position = ExploderUtils.GetCentroid(gameObject);
            Exploder.Radius = 1.0f;
            Exploder.ExplodeRadius();
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(200, 10, 300, 30), "Hit enter to explode everything!");
        }
    }
}
