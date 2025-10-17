using UnityEngine;

namespace Core
{
    public class CoreInitializer : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Debug.Log("[CoreInitializer] Core systems ready.");
        }
    }
}