using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Data
{
    public class SetGameObjectData : MonoBehaviour
    {

        public GameObjectVariable gameObjectToSet;

        private void Awake()
        {
            gameObjectToSet.SetValue(gameObject);
        }
    }
}
