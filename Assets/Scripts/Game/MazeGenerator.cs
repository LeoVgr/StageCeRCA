using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cinemachine;
using DG.Tweening;
using Player;
using UnityAtoms.BaseAtoms ;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
// ReSharper disable PossibleLossOfFraction

/**
* @author : Samuel BUSSON
* @brief : Class MazeGenerator is used to generate the corridor based on the parameters
* @date : 07/2020
*/
public struct NodePosition
{
    public int X;
    public int Z;
    public int ImagePositionInt;

    public NodePosition(int xPos, int zPos)
    {
        X = xPos;
        Z = zPos;
        ImagePositionInt = -1;
    }
}
public enum Direction
{
    North,
    East,
    South,
    West,
    Undefined
}

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze size")] 
    public IntReference mazeLength;
    public FloatReference cubeSize;
    public FloatReference wallHeight;   
    
    [Header("Maze properties")]
    public IntReference turnNumber;
    public FloatReference imageSize;

    [Header("Random")]
    public IntReference seed;
    public BoolVariable randomizeImage;

    [Header("Prefab, material,...")] 
    public Material wallMaterial;
    public Material floorMaterial;
    public GameObject imagePrefab;
    public GameObject emptyTarget;
    public GameObject raceEnd;
    
    
    private List<Sprite> _baseSpriteList;
    private List<Sprite> _spriteList;
    private List<int> _turnValue;
    private List<int> _imageValue;

    //Set length of the 1st corridor after generate the true one
    private readonly int _startLength = 4;

    private Dictionary<NodePosition, MazeNode> _maze;
    
    private CinemachineSmoothPath _playerPath;
    private CinemachineSmoothPath.Waypoint[] _wayPointsPath;

    //_master is the old generated maze
    private GameObject _master;
    private bool _canCreate;
    
    // Maze properties
    private float _cubeSizeX = 1.0f;
    private float _cubeSizeZ = 1.0f;
    private float _wallHeight = 5.0f;
    private int _length = 10;
    private int _turnNumber;
    private float _imageSize = 1.0f;
    

    public void Start()
    {  
        //Init List
        _baseSpriteList = new List<Sprite>();
        _maze = new Dictionary<NodePosition, MazeNode>();
        _turnValue = new List<int>();
        _imageValue = new List<int>();
        
        //Load images in _baseSpriteList
        ReadImageFolderThenCreateMaze();
    }

    /**
     * @brief Set values bases on AtomsVariables then create corridor
     */
    public bool GenerateMaze()
    {
        //False if the all image hasn't been loaded
        if (_canCreate)
        {
            //Destroy old maze (can be removed since we reload the scene when we want to restart a game)
            if (_master)
            {
                Destroy(_master);
            }
            
            //Create new maze
            _master = new GameObject();
            
            //Clear all list
            _maze.Clear();
            _turnValue.Clear();
            _imageValue.Clear();
        
            //Set values
            _cubeSizeX = cubeSize.Value;
            _cubeSizeZ = cubeSize.Value;
            _wallHeight = wallHeight.Value;
            _length = mazeLength.Value;
            _turnNumber = turnNumber.Value;
            _imageSize = imageSize.Value;

            //Copy _baseSpriteList into _spriteList
            if(_length < _baseSpriteList.Count)
                _spriteList = _baseSpriteList.GetRange(0, _length - 1);
            else
                _spriteList = new List<Sprite>(_baseSpriteList);

            CreateMaze();
            return true;
        }
        return false;
    }


    /**
     * @brief Fill _baseSpriteList with Images located in StreamingAssets/Images, taking few times to load all images
     */
    private void ReadImageFolderThenCreateMaze()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
        FileInfo[] allFiles = directoryInfo.GetFiles("Images/*.*");

        StartCoroutine(nameof(LoadImages), allFiles);
    }
    
    /**
     * @brief Load image
     */
    IEnumerator LoadImages(FileInfo[] allFiles)
    {
    
        foreach (FileInfo file in allFiles)
        {
            if ((file.Name.Contains("png")  || file.Name.Contains("jpg")) && !file.Name.Contains("meta"))
            {
                string playerFileWithoutExtension = Path.GetFileNameWithoutExtension(file.ToString());
                string[] playerNameData = playerFileWithoutExtension.Split(" "[0]);
                //3
                string tempPlayerName = "";
                int i = 0;
                foreach (string stringFromFileName in playerNameData)
                {
                    if (i != 0)
                    {
                        tempPlayerName = tempPlayerName + stringFromFileName + " ";
                    }
                    i++;
                }
                //4
                string wwwImageFilePath = "file://" + file.FullName;

                UnityWebRequest www = UnityWebRequestTexture.GetTexture(wwwImageFilePath);
                yield return www.SendWebRequest();
                
                if(www.isNetworkError || www.isHttpError) {
                    Debug.Log(www.error);
                }
                else 
                {
                    Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height),
                        new Vector2(0.5f, 0.5f));
                    sprite.name = file.Name;
                    
                    _baseSpriteList.Add(sprite);
                }
            }
        }

        _canCreate = true;
    }
    
    /**
     * @brief Set the seed and generate the corridor
     */
    private void CreateMaze()
    {
        Random.InitState(seed.Value);
        InitMaze();
    }

    /**
     * Initialize the corridor
     */
    private void InitMaze()
    {
        InitGameObject();
        InitRandomTurnAndImages();

        CorridorGeneration();

        CheckCorner();
        EndInit();

        BuildMaze();
    }
    
    /**
     * @brief Creating object used to move player (CinemachineSmoothPath)
     */
    private void InitGameObject()
    {
        transform.position = Vector3.zero;
        
        _master.AddComponent<CinemachineSmoothPath>();
        _master.name = "Corridor Maze";

        _playerPath = _master.GetComponent<CinemachineSmoothPath>();
        _wayPointsPath = new CinemachineSmoothPath.Waypoint[_length + _startLength];
        
    }

    /**
     * @brief Generates random values, index, used to know when to place a turn and an image.
     */
    private void InitRandomTurnAndImages()
    {
        //Setup Turn place
        for (int i = 0; i < _turnNumber; ++i)
        {
            float randomA =  i * _length / _turnNumber;
            float randomB =   (i + 1) * _length / _turnNumber - 3;
            int j = Mathf.RoundToInt(Random.Range(randomA, randomB));
            
            _turnValue.Add(j);
        }

        int maxImages = _spriteList.Count > _length ? _length : _spriteList.Count;
        
        //Setup image place
        for (int i = 0; i < maxImages; ++i)
        {
            float randomA =  i * _length / maxImages;
            float randomB =   (i + 1) * _length / maxImages;
            --randomB;
            
            int j = Mathf.RoundToInt(Random.Range(randomA, randomB));

            //Debug.Log(randomA + " et " + randomB + " = " + j );
            
            //Move images if it next to a turn
            if (_turnValue.Contains(j) && j != maxImages - 1)
                ++j;

            //Never have two images in a row
            if (_imageValue.Contains(j - 1))
                ++j;

            _imageValue.Add(j);
        }
    }


    /**
     * @brief Main function that generates nodes that allow the creation of the corridor
     */
    private void CorridorGeneration()
    {
        float pathHeight = 0.5f;
        
        Direction previousDirection = Direction.North;
        Direction newDirection = Direction.North;
        NodePosition position = new NodePosition(0,0);

        ImagePos oldImagePosition = ImagePos.none;
        ImagePos currentImagePosition = ImagePos.none;
        

        // Base Node 
        _maze[position] = new MazeNode(position.X, position.Z, newDirection);

        //Base waypoint 
        CinemachineSmoothPath.Waypoint waypoint = new CinemachineSmoothPath.Waypoint
        {
            position = new Vector3(position.X, pathHeight, position.Z)
        };
        _wayPointsPath[0] = waypoint;
        
        //Save previous node, all nodes know previous and next node
        MazeNode previousNode = _maze[position];

        for (int i = 0; i < _startLength - 1; i++)
        {
            position = GetPosition(newDirection, position);
            _maze[position] = new MazeNode(position.X, position.Z, newDirection);
            
            waypoint =  new CinemachineSmoothPath.Waypoint();
            waypoint.position = new Vector3(position.X * _cubeSizeX, pathHeight , position.Z * _cubeSizeZ);
            _wayPointsPath[i + 1] = waypoint;
            
            previousNode.SetNextNode(_maze[position]);
            _maze[position].SetPreviousNode(previousNode);
            previousNode = _maze[position];
        }

        
        //Main loop
        for (int i = 0; i < _length; i++)
        {
            bool isTurner = false;

            //Check if random generated index equal current length, if true generates turn
            if (_turnValue.Contains(i))
            {
                isTurner = true;
                //Get Random direction different from previous one
                while (previousDirection == newDirection)
                {
                    NodePosition tempPosition = position;
                    Direction tempDirection = GetRandomDirection();
                    tempPosition = GetPosition(tempDirection, tempPosition);

                    if (CheckDistance(tempPosition))
                        newDirection = tempDirection;
                    
                }
                //Special case if it's the first block
                /*if (i == 0)
                    _maze[position].SetDirection(newDirection);*/
            }

            previousDirection = newDirection;
            //Set Node position
            position = GetPosition(newDirection, position);
            
            //New waypoint
            waypoint =  new CinemachineSmoothPath.Waypoint();
            waypoint.position = new Vector3(position.X * _cubeSizeX, pathHeight , position.Z * _cubeSizeZ);
            _wayPointsPath[i + _startLength] = waypoint;

            //Add new node to _maze
            _maze[position] = new MazeNode(position.X, position.Z, newDirection);

            
            
            //Check if random generated index equal current length index , if true generates image
            if (_imageValue.Contains(i))
            {
                Vector3 imagePosition = CalculateRandomImagePosition( _maze[position], out currentImagePosition);
                /*Debug.Log("Crrent " + currentImagePosition + " old " + oldImagePosition);
                while (currentImagePosition == oldImagePosition)
                {
                    Debug.Log("Bug : Current " + currentImagePosition + " old " + oldImagePosition);
                    imagePosition = CalculateRandomImagePosition( _maze[position], out currentImagePosition);
                }

                Debug.Log("Result" + currentImagePosition);

                Debug.Log("================");
                oldImagePosition = currentImagePosition;*/
                
                
                _maze[position].SetImagePositon(imagePosition);
                _maze[position].SetNegateImage( waypoint.position);
            }

            //Set previous and next node
            _maze[position].SetPreviousNode(previousNode);
            previousNode.SetNextNode(_maze[position]);
            previousNode.SetIsTurner(isTurner);
            
            previousNode = _maze[position];
            
        }
    }     

    /**
     * @brief If image is generated in turner move it
     */
    private void CheckCorner()
    {
        foreach (var mazeNode in _maze)
        {
            //If it's a turn and has image on it
            if (mazeNode.Value.IsCorner() && mazeNode.Value.GetImagePosition() != Vector3.zero)
            {
                switch (mazeNode.Value.GetNextNode().GetDirection())
                {
                    case Direction.North:
                        if(mazeNode.Value.GetPreviousNode().GetDirection() == Direction.East)
                            mazeNode.Value.SetNegateImage(true);
                        if(mazeNode.Value.GetPreviousNode().GetDirection() == Direction.West)
                            mazeNode.Value.SetNegateImage(false);
                        mazeNode.Value.SetImagePositon(ImagePosition(Random.Range(2, 4),  mazeNode.Value));
                        break;
                    case Direction.East:
                        if(mazeNode.Value.GetPreviousNode().GetDirection() == Direction.North || mazeNode.Value.GetPreviousNode().GetDirection() == Direction.East )
                            mazeNode.Value.SetNegateImage(false);
                        mazeNode.Value.SetImagePositon(ImagePosition(Random.Range(2, 4),  mazeNode.Value));
                        break;
                    case Direction.West:
                        if(mazeNode.Value.GetPreviousNode().GetDirection() == Direction.North)
                            mazeNode.Value.SetNegateImage(true);
                        mazeNode.Value.SetImagePositon(ImagePosition(Random.Range(0, 2),  mazeNode.Value));
                        break;
                }
            }
        }
    }
    
    /**
     * @brief Set the player path
     */
    private void EndInit()
    {
        _playerPath.m_Waypoints = _wayPointsPath;
        
        foreach (PlayerMovement playerMovement in FindObjectsOfType<PlayerMovement>())
        {
            playerMovement.SetPath(_playerPath);
        }
    }
    

    private bool CheckDistance(NodePosition tempPosition)
    {
        return !_maze.ContainsKey(tempPosition);
    }
    
    /**
     * @brief Generate wall, set material, floor, images,...
     */
    private void BuildMaze()
    {
        int i = 0;


        float decal = 0.5f;
        

        foreach (var valuePair in _maze)
        {
            MazeNode node = valuePair.Value;
            
            float wallPosition = _wallHeight / 2.0f + 0.5f;
            Vector3 baseVector = new Vector3(node.GetPosition().X * _cubeSizeX, wallPosition ,node.GetPosition().Z * _cubeSizeZ);


            #region Spawn Floor

            //Create Floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.transform.position =  new Vector3(node.GetPosition().X * _cubeSizeX, 0 ,node.GetPosition().Z * _cubeSizeZ);
            floor.transform.localScale = new Vector3(_cubeSizeX, 1, _cubeSizeZ);
            floor.name = node.GetDirection() + " Corridor floor " + i;
            floor.GetComponent<MeshRenderer>().material = floorMaterial;
            
            floor.transform.SetParent(_master.transform);
            #endregion

            #region Spawn Image
            
            if (node.IsImage())
            {
                GameObject image = Instantiate(imagePrefab, node.GetImagePosition(), Quaternion.identity);
                Target target = image.GetComponent<Target>();
                image.transform.SetParent(floor.transform);
                
                //If there is image on this node
                if (target)
                {
                    //Set target direction
                    target.Direction = node.GetDirection();

                    //True = Get Random image, False = get the images in alphabetical order
                    if (randomizeImage.Value)
                    {
                        int random = Random.Range(0, _spriteList.Count);
                        target.Sprite = _spriteList[random];
                        _spriteList.RemoveAt(random);
                    }
                    else
                    {
                        target.Sprite = _spriteList[0];
                        _spriteList.RemoveAt(0);
                    }
                    
                    //Depending the target side left or right 
                    target.IsNegateImage = node.IsNegateImagePosition();
                    target.WayPointIndex = i;
                    target.TargetPosition = node.GetIntImagePosition();

                    var transform1 = target.transform;
                    transform1.localScale = _imageSize *  transform1.localScale;

                    if (node.IsNegateImagePosition())
                    {
                        SpriteRenderer spireRenderer = target.GetComponentInChildren<SpriteRenderer>();
                        if (spireRenderer)
                        {
                            var transform2 = spireRenderer.transform;
                            transform2.localPosition = -transform2.localPosition;
                        }
                    }
                                
                }
            }
            
            #endregion

            #region Spawn Wall


            /*if (node.GetPreviousNode() != null && node.GetNextNode() != null)
                Debug.Log("Number : " + i + " next " + node.GetNextNode().GetDirection() + " previous " + node.GetPreviousNode().GetDirection());*/
            
            
            WallsToDestroy(node.GetNextNode(), node.GetPreviousNode(), out Direction directionSupprPrevious, out Direction directionSupprNext);

            
            //Create Wall South
            if (directionSupprPrevious != Direction.South && directionSupprNext != Direction.South)
                CreateWall(Direction.South, floor.transform, baseVector, decal, node);

            //Create Wall North
            if (directionSupprPrevious != Direction.North && directionSupprNext != Direction.North)
                CreateWall(Direction.North, floor.transform, baseVector, decal, node);

            //Create Wall East
            if (directionSupprPrevious != Direction.East && directionSupprNext != Direction.East)
                CreateWall(Direction.East, floor.transform, baseVector, decal, node);

            //Create Wall West
            if (directionSupprPrevious != Direction.West && directionSupprNext != Direction.West)
                CreateWall(Direction.West, floor.transform, baseVector, decal, node);
            
            
            #endregion

            // Poop code, check without
            DOVirtual.DelayedCall(Time.deltaTime, () => CreateFakeTarget(node, floor));


            i++;
            
            if (i == _maze.Count)
            {
                
                Vector3 pos =  new Vector3(node.GetPosition().X * _cubeSizeX, 3.0f ,node.GetPosition().Z * _cubeSizeZ);
                Instantiate(raceEnd, pos, Quaternion.identity);
            }
        }
    }

    private void CreateFakeTarget(MazeNode node, GameObject floor)
    {
        for (int j = 0; j < 4; ++j)
        {
            if (node.GetIntImagePosition() != j)
            {
                Vector3 positon = ImagePosition(j, node, false);

                bool removeLeft = false;
                bool removeRight = false;
                bool create = true;

                if (node.IsCorner())
                {
                    switch (node.GetNextNode().GetDirection())
                    {
                        case Direction.North:
                            if (node.GetDirection() == Direction.East)
                                removeLeft = true;
                            if (node.GetDirection() == Direction.West)
                                removeLeft = true;
                            break;
                        case Direction.East:
                            if (node.GetDirection() == Direction.North)
                                removeLeft = true;
                            if (node.GetDirection() == Direction.West)
                                removeRight = true;
                            break;
                       
                        case Direction.West:             
                            if (node.GetDirection() == Direction.East)
                                removeLeft = true;
                            if (node.GetDirection() == Direction.North)
                                removeRight = true;
                            break;
                    }
                    if (removeLeft && j < 2)
                    {
                        create = false;
                    }
                
                    if (removeRight && j >= 2)
                    {
                        create = false;
                    }
                }

                if (create)
                {
                    GameObject emptyTargetObject = Instantiate(emptyTarget, positon, Quaternion.identity);
                    emptyTargetObject.transform.SetParent(floor.transform);
                    emptyTargetObject.transform.localScale = _imageSize *  emptyTargetObject.transform.localScale;
                }
            }
        }
    }

    private void CreateWall(Direction d, Transform floorTransform, Vector3 basePosition, float decal, MazeNode node)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        switch (d)
        {
            case Direction.North:
                wall.name = "North Wall ";
                wall.transform.position = basePosition + new Vector3(0, 0,  _cubeSizeZ / 2.0f - decal);
                wall.transform.localScale = new Vector3(_cubeSizeX, _wallHeight, 1);
                break;
            case Direction.East:
                wall.name = "East Wall ";
                wall.transform.position = basePosition + new Vector3(_cubeSizeX / 2.0f - decal, 0,0);
                wall.transform.localScale = new Vector3(1, _wallHeight, _cubeSizeZ);
                break;
            case Direction.West:
                wall.name = "West Wall ";
                wall.transform.position = basePosition + new Vector3( -_cubeSizeX / 2.0f + decal , 0, 0);
                wall.transform.localScale = new Vector3(1, _wallHeight, _cubeSizeZ);
                break;
            case Direction.South:
                wall.name = "South Wall ";
                wall.transform.position =  basePosition + new Vector3(0, 0 ,  -_cubeSizeZ/2.0f + decal);
                wall.transform.localScale = new Vector3(_cubeSizeX, _wallHeight, 1);
                break;
        }
        
        wall.transform.SetParent(floorTransform);
        wall.GetComponent<MeshRenderer>().material = wallMaterial;

        //Create ceiling
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.transform.position = new Vector3(basePosition.x, wallHeight + 0.5f, basePosition.z);
        ceiling.transform.localScale = new Vector3(_cubeSizeX, 0.1f, _cubeSizeZ);
        ceiling.transform.SetParent(floorTransform);
        ceiling.GetComponent<MeshRenderer>().material = wallMaterial;
    }

    private NodePosition GetPosition(Direction testDirection, NodePosition tempPosition)
    {
        switch (testDirection)
        {
            case Direction.North:
                tempPosition.Z++;
                break;
            case Direction.East:
                tempPosition.X++;
                break;
            case Direction.West:
                tempPosition.X--;
                break;
        }
        return tempPosition;
    }

    private Vector3 CalculateRandomImagePosition(MazeNode n, out ImagePos imagepos)
    {
      
        float r = Random.Range(0, 101);

        if (r < 25)
        {
            imagepos = ImagePos.BotRight;
            return ImagePosition(0, n);
        }
        if (r >= 25 && r < 50)
        {
            imagepos = ImagePos.BotLeft;
            return ImagePosition(1, n);
        }
        if (r >= 50 && r < 75)
        {
            imagepos = ImagePos.TopLeft;
            return ImagePosition(2, n);
        }
        if (r >= 75)
        {
            imagepos = ImagePos.TopRight;
            return ImagePosition(3, n);
        }
        
        imagepos = ImagePos.none;
        
        return Vector3.zero;
    }
    
    public enum ImagePos
    {
        TopLeft,
        TopRight,
        BotLeft,
        BotRight,
        none
    }

    private Vector3 ImagePosition(int i, MazeNode n, bool useToSetImagePosition = true)
    {
        float xDecal = 0;
        float zDecal = 0;

        var np = n.GetPosition();
        var d = n.GetDirection();

        switch (d)
        {
            case Direction.North:
            case Direction.South:
                xDecal = _cubeSizeX / 2.0f - 1.0f;
                break;
            case Direction.East:
            case Direction.West:
                zDecal = _cubeSizeZ/2.0f - 1.0f;
                break;
        }
        
        if(useToSetImagePosition)
            n.SetIntImagePositon(i);
        
        switch (i)
        {
            case 0:
                return new Vector3(np.X * _cubeSizeX + xDecal, _imageSize, np.Z * _cubeSizeZ + zDecal);
            case 1:
                return new Vector3(np.X * _cubeSizeX + xDecal, _wallHeight + (1 - _imageSize), np.Z * _cubeSizeZ + zDecal);
            case 2:
                return new Vector3(np.X * _cubeSizeX - xDecal, _imageSize, np.Z * _cubeSizeZ - zDecal);
            case 3:
                return new Vector3(np.X * _cubeSizeX - xDecal, _wallHeight + (1 - _imageSize), np.Z * _cubeSizeZ - zDecal);
        }
        
        
        return Vector3.zero;
    }
    

    private void WallsToDestroy(MazeNode nextNode, MazeNode previousNode, out Direction wallToDestroyPrevious, out Direction wallToDestroyNext)
    {
        wallToDestroyPrevious = Direction.Undefined;
        wallToDestroyNext = Direction.Undefined;
        
        if(previousNode != null)
        {
            wallToDestroyPrevious = WhereIsNode(previousNode, previousNode.GetNextNode());
        }

        if (nextNode != null)
        {
            wallToDestroyNext =  nextNode.GetDirection();
        }
    }
    
    private Direction WhereIsNode(MazeNode n, MazeNode n2)
    {
        if (n.GetPosition().X < n2.GetPosition().X)
            return Direction.West;
        if (n.GetPosition().X > n2.GetPosition().X)
            return Direction.East;
        if (n.GetPosition().Z < n2.GetPosition().Z)
            return Direction.South;
        if (n.GetPosition().Z > n2.GetPosition().Z)
            return Direction.North;

        return Direction.Undefined;
    }


    private Direction GetRandomDirection()
    {
        float r = Random.Range(0.0f, 100f);

      /*  if (r < 25.0f)
            return Direction.East;
        if(r >= 25.0f && r < 50.0f)
            return Direction.North;
        if(r >= 50.0f && r < 75.0f)
            return Direction.South;
        if(r >= 75.0f)
            return Direction.West;*/
      
          if (r < 33.0f)
              return Direction.East;
          if(r >= 33.0f && r < 66.0f)
              return Direction.North;
          if(r >= 66.0f)
              return Direction.West;
      
      

        return Direction.East;
    }
}

public class MazeNode
{
    private NodePosition _nodePosition;
    private MazeNode _previousNode;
    private MazeNode _nextNode;
    
    private Vector3 _imagePosition;
    private Direction _direction;
    private bool _negateImagePosition;
    private bool _isTurner;

    public MazeNode(int x, int z, Direction d)
    {
        _nodePosition.X = x;
        _nodePosition.Z = z;
        _nodePosition.ImagePositionInt = -1;
        _imagePosition = Vector3.zero;
        _direction = d;
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

    public void SetNegateImage(Vector3 nodePosition)
    {
        if (_imagePosition.x > nodePosition.x && (_direction == Direction.North || _direction == Direction.South))
            _negateImagePosition = true;
        if (_imagePosition.z < nodePosition.z && _direction == Direction.East)
            _negateImagePosition = true;
        if (_imagePosition.z > nodePosition.z && _direction == Direction.West)
            _negateImagePosition = true;
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
}
