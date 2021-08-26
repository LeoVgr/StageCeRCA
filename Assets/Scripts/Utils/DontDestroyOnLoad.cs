using UnityEngine;

namespace Utils
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DontDestroy(gameObject);
        }

        private void DontDestroy(GameObject o)
        {
            var parent = o.transform.parent;
            if(parent == null)
                DontDestroyOnLoad(o);
            else
            {
                transform.SetParent(null);
                DontDestroy(o);
            }
        }
    }
}