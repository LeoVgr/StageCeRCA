using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cinemachine;
using DG.Tweening;
using Player;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class MazeNode
{
    #region "Atttributs"
    private NodePosition _nodePosition;
    private MazeNode _previousNode;
    private MazeNode _nextNode;

    private Vector3 _imagePosition;
    private Direction _direction;
    private bool _negateImagePosition;
    private bool _isTurner;
    #endregion

    #region "Methods"
    public MazeNode(int x, int z, Direction d)
    {
        _nodePosition.X = x;
        _nodePosition.Z = z;
        _nodePosition.ImagePositionInt = -1;
        _imagePosition = Vector3.zero;
        _direction = d;
    }

    public void SetNegateImage(Vector3 nodePosition)
    {
        if (_imagePosition.x > nodePosition.x && (_direction == Direction.North || _direction == Direction.South))
            _negateImagePosition = true;
        if (_imagePosition.z < nodePosition.z && _direction == Direction.East)
            _negateImagePosition = true;
        if (_imagePosition.z > nodePosition.z && _direction == Direction.West)
            _negateImagePosition = true;
    }
    public bool IsCorner()
    {
        return _isTurner;
    }
    public void SetIsTurner(bool isTurn)
    {
        _isTurner = isTurn;
    }
    public NodePosition GetPosition()
    {
        return _nodePosition;
    }
    public void SetIntImagePositon(int pos)
    {
        _nodePosition.ImagePositionInt = pos;
    }
    public MazeNode GetPreviousNode()
    {
        return _previousNode;
    }
    public MazeNode GetNextNode()
    {
        return _nextNode;
    }
    public void SetPreviousNode(MazeNode previousNode)
    {
        _previousNode = previousNode;
    }
    public void SetNextNode(MazeNode nextNode)
    {
        _nextNode = nextNode;
    }
    public void SetImagePositon(Vector3 pos)
    {
        _imagePosition = pos;
    }
    public bool IsImage()
    {
        return Vector3.Distance(_imagePosition, Vector3.zero) > 0.1f;
    }
    public Vector3 GetImagePosition()
    {
        return _imagePosition;
    }
    public Direction GetDirection()
    {
        return _direction;
    }
    public bool IsNegateImagePosition()
    {
        return _negateImagePosition;
    }
    public void SetNegateImage(bool setter)
    {
        _negateImagePosition = setter;
    }
    public void SetDirection(Direction newDirection)
    {
        _direction = newDirection;
    }
    public int GetIntImagePosition()
    {
        return _nodePosition.ImagePositionInt;
    }
    #endregion



}
