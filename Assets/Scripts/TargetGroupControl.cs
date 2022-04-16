using System;
using Cinemachine;
using UnityEngine;

public class TargetGroupControl : MonoBehaviour
{
    private CinemachineTargetGroup _group;

    private CinemachineTargetGroup _targetGroup
    {
        get
        {
            if (_group == null) _group = GetComponent<CinemachineTargetGroup>();
            return _group;
        }
    }

    private void Start()
    {
        for (var index = 0; index < _targetGroup.m_Targets.Length; index++)
        {
            _targetGroup.m_Targets[index].weight = _targetGroup.m_Targets[index].target.gameObject.activeInHierarchy ? 1 : 0;
        }
    }
}