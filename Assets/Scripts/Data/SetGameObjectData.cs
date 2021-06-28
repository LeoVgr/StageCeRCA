using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class SetGameObjectData : MonoBehaviour
{

    public GameObjectVariable gameObjectToSet;

    private void Awake()
    {
        gameObjectToSet.SetValue(gameObject);
    }
}
