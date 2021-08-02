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
public enum ImagePos
{
    TopLeft,
    TopRight,
    BotLeft,
    BotRight,
    none
}

public class MazeGenerator : MonoBehaviour
{
    #region "Attributs"
    [Header("Maze size")] 
    public IntReference CorridorLenght;
    public FloatReference CorridorCubeSegmentSize;
    public FloatReference WallHeight;   
    
    [Header("Maze properties")]
    public IntReference TurnNumber;
    public FloatReference ImageSize;

    [Header("Random")]
    public IntReference Seed;
    public BoolVariable RandomizeImage;

    [Header("Prefab, material,...")]
    public GameObject PrefabWall;
    public GameObject PrefabCeiling;
    public GameObject PrefabColumn;
    public GameObject PrefabStraightRail;
    public GameObject PrefabRotatingRail;
    
    public Material FloorMaterial;
    public GameObject ImagePrefab;
    public GameObject TurretPrefab;
    public GameObject EmptyTarget;
    public GameObject RaceEnd;
    
    
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
    private float _cubeSize = 1.0f;
    private float _wallHeight = 5.0f;
    private int _length = 10;
    private int _turnNumber;
    private float _imageSize = 1.0f;
    #endregion

    #region "Events"
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
    #endregion

    #region "Methods"
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
            _cubeSize = CorridorCubeSegmentSize.Value;
            _wallHeight = WallHeight.Value;
            _length = CorridorLenght.Value;
            _turnNumber = TurnNumber.Value;
            _imageSize = ImageSize.Value;

            //Copy _baseSpriteList into _spriteList
            if (_length < _baseSpriteList.Count)
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
            if ((file.Name.Contains("png") || file.Name.Contains("jpg")) && !file.Name.Contains("meta"))
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

                if (www.isNetworkError || www.isHttpError)
                {
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
        Random.InitState(Seed.Value);
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
            float randomA = i * _length / _turnNumber;
            float randomB = (i + 1) * _length / _turnNumber - 3;
            int j = Mathf.RoundToInt(Random.Range(randomA, randomB));

            _turnValue.Add(j);
        }

        int maxImages = _spriteList.Count > _length ? _length : _spriteList.Count;

        //Setup image place
        for (int i = 0; i < maxImages; ++i)
        {
            float randomA = i * _length / maxImages;
            float randomB = (i + 1) * _length / maxImages;
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
        NodePosition position = new NodePosition(0, 0);

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

            waypoint = new CinemachineSmoothPath.Waypoint();
            waypoint.position = new Vector3(position.X * _cubeSize, pathHeight, position.Z * _cubeSize);
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
            waypoint = new CinemachineSmoothPath.Waypoint();
            waypoint.position = new Vector3(position.X * _cubeSize, pathHeight, position.Z * _cubeSize);
            _wayPointsPath[i + _startLength] = waypoint;

            //Add new node to _maze
            _maze[position] = new MazeNode(position.X, position.Z, newDirection);



            //Check if random generated index equal current length index , if true generates image
            if (_imageValue.Contains(i))
            {
                Vector3 imagePosition = CalculateRandomImagePosition(_maze[position], out currentImagePosition);
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
                _maze[position].SetNegateImage(waypoint.position);
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
                        if (mazeNode.Value.GetPreviousNode().GetDirection() == Direction.East)
                            mazeNode.Value.SetNegateImage(true);
                        if (mazeNode.Value.GetPreviousNode().GetDirection() == Direction.West)
                            mazeNode.Value.SetNegateImage(false);
                        mazeNode.Value.SetImagePositon(ImagePosition(Random.Range(2, 4), mazeNode.Value));
                        break;
                    case Direction.East:
                        if (mazeNode.Value.GetPreviousNode().GetDirection() == Direction.North || mazeNode.Value.GetPreviousNode().GetDirection() == Direction.East)
                            mazeNode.Value.SetNegateImage(false);
                        mazeNode.Value.SetImagePositon(ImagePosition(Random.Range(2, 4), mazeNode.Value));
                        break;
                    case Direction.West:
                        if (mazeNode.Value.GetPreviousNode().GetDirection() == Direction.North)
                            mazeNode.Value.SetNegateImage(true);
                        mazeNode.Value.SetImagePositon(ImagePosition(Random.Range(0, 2), mazeNode.Value));
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
        float decal = -0.5f;


        foreach (var valuePair in _maze)
        {
            MazeNode node = valuePair.Value;

            float wallPosition = _wallHeight / 2.0f + 0.5f;
            Vector3 baseVector = new Vector3(node.GetPosition().X * _cubeSize, wallPosition, node.GetPosition().Z * _cubeSize);


            #region Spawn Floor

            //Create Floor
            GameObject floor = CreateFloor(i, node);

            //GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //floor.transform.position =  new Vector3(node.GetPosition().X * _cubeSizeX, 0 ,node.GetPosition().Z * _cubeSizeZ);
            //floor.transform.localScale = new Vector3(_cubeSizeX, 1, _cubeSizeZ);
            //floor.name = node.GetDirection() + " Corridor floor " + i;
            //floor.GetComponent<MeshRenderer>().material = floorMaterial;

            //floor.transform.SetParent(_master.transform);
            #endregion

            #region Spawn Image

            if (node.IsImage())
            {
                //Pick our future sprite
                Sprite selectedSprite;

                //True = Get Random image, False = get the images in alphabetical order
                if (RandomizeImage.Value)
                {
                    int random = Random.Range(0, _spriteList.Count);
                    selectedSprite = _spriteList[random];
                    _spriteList.RemoveAt(random);
                }
                else
                {
                    selectedSprite = _spriteList[0];
                    _spriteList.RemoveAt(0);
                }

                //Check his name to see if it's a target or a turret
                if (selectedSprite.name.Split('_').Length >= 2 && selectedSprite.name.Split('_')[1].Contains("T"))
                {
                    //Create turret
                    GameObject turret = Instantiate(TurretPrefab, node.GetImagePosition(), Quaternion.identity);
                    Turret turretTarget = turret.GetComponent<Turret>();
                    turret.transform.SetParent(floor.transform);

                    //If there is image on this node
                    if (turretTarget)
                    {
                        //Set shoot rate 
                        turretTarget.TimeBetweenShoot = float.Parse(selectedSprite.name.Split('_')[1].Remove(0, 1));

                        //Set target direction
                        turretTarget.Direction = node.GetDirection();
                        turretTarget.Sprite = selectedSprite;

                        //Depending the target side left or right 
                        turretTarget.IsNegateImage = node.IsNegateImagePosition();
                        turretTarget.WayPointIndex = i;
                        turretTarget.TargetPosition = node.GetIntImagePosition();

                        var transform1 = turretTarget.transform;
                        transform1.localScale = _imageSize * transform1.localScale;

                        if (node.IsNegateImagePosition())
                        {
                            SpriteRenderer spireRenderer = turretTarget.GetComponentInChildren<SpriteRenderer>();
                            if (spireRenderer)
                            {
                                var transform2 = spireRenderer.transform;
                                transform2.localPosition = -transform2.localPosition;
                            }
                        }

                    }
                }
                else
                {
                    //Create simple target
                    GameObject image = Instantiate(ImagePrefab, node.GetImagePosition(), Quaternion.identity);
                    Target target = image.GetComponent<Target>();
                    image.transform.SetParent(floor.transform);

                    //If there is image on this node
                    if (target)
                    {
                        //Set target direction
                        target.Direction = node.GetDirection();
                        target.Sprite = selectedSprite;

                        //Depending the target side left or right 
                        target.IsNegateImage = node.IsNegateImagePosition();
                        target.WayPointIndex = i;
                        target.TargetPosition = node.GetIntImagePosition();

                        var transform1 = target.transform;
                        transform1.localScale = _imageSize * transform1.localScale;

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

            //Create Ceiling 
            CreateCeiling(floor.transform, baseVector);

            //Create Columns 
            CreateCeiling(floor.transform, baseVector);

            //Create Rails
            CreateRail(node.GetNextNode(), node.GetPreviousNode(),node, floor.transform, baseVector);

            #endregion

            // Poop code, check without
            DOVirtual.DelayedCall(Time.deltaTime, () => CreateFakeTarget(node, floor));


            i++;

            if (i == _maze.Count)
            {

                Vector3 pos = new Vector3(node.GetPosition().X * _cubeSize, 3.0f, node.GetPosition().Z * _cubeSize);
                Instantiate(RaceEnd, pos, Quaternion.identity);
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
                    GameObject emptyTargetObject = Instantiate(EmptyTarget, positon, Quaternion.identity);
                    emptyTargetObject.transform.SetParent(floor.transform);
                    emptyTargetObject.transform.localScale = _imageSize * emptyTargetObject.transform.localScale;
                }
            }
        }
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
                xDecal = _cubeSize / 2.0f - 1.0f;
                break;
            case Direction.East:
            case Direction.West:
                zDecal = _cubeSize / 2.0f - 1.0f;
                break;
        }

        if (useToSetImagePosition)
            n.SetIntImagePositon(i);

        switch (i)
        {
            case 0:
                return new Vector3(np.X * _cubeSize + xDecal, _imageSize, np.Z * _cubeSize + zDecal);
            case 1:
                return new Vector3(np.X * _cubeSize + xDecal, _wallHeight + (1 - _imageSize), np.Z * _cubeSize + zDecal);
            case 2:
                return new Vector3(np.X * _cubeSize - xDecal, _imageSize, np.Z * _cubeSize - zDecal);
            case 3:
                return new Vector3(np.X * _cubeSize - xDecal, _wallHeight + (1 - _imageSize), np.Z * _cubeSize - zDecal);
        }


        return Vector3.zero;
    }
    private void WallsToDestroy(MazeNode nextNode, MazeNode previousNode, out Direction wallToDestroyPrevious, out Direction wallToDestroyNext)
    {
        wallToDestroyPrevious = Direction.Undefined;
        wallToDestroyNext = Direction.Undefined;

        if (previousNode != null)
        {
            wallToDestroyPrevious = WhereIsNode(previousNode, previousNode.GetNextNode());
        }

        if (nextNode != null)
        {
            wallToDestroyNext = nextNode.GetDirection();
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
        if (r >= 33.0f && r < 66.0f)
            return Direction.North;
        if (r >= 66.0f)
            return Direction.West;



        return Direction.East;
    }


    /* Build physical parts of the mine*/
    private GameObject CreateFloor(int index, MazeNode node)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.transform.position = new Vector3(node.GetPosition().X * _cubeSize, 0, node.GetPosition().Z * _cubeSize);
        floor.transform.localScale = new Vector3(_cubeSize, 1, _cubeSize);
        floor.name = node.GetDirection() + " Corridor floor " + index;
        floor.GetComponent<MeshRenderer>().material = FloorMaterial;

        floor.transform.SetParent(_master.transform);

        return floor;
    }
    private void CreateRail(MazeNode nextNode, MazeNode previousNode, MazeNode currentNode, Transform floorTransform, Vector3 basePosition)
    {
        //Create rail
        GameObject rail;

        //Select a straight rail or rotating rail
        if (nextNode != null && nextNode.GetDirection() != currentNode.GetDirection() && 
            nextNode.GetDirection() != Direction.Undefined && currentNode.GetDirection() != Direction.Undefined)
        {
            rail = Instantiate(PrefabRotatingRail);

            //Apply first the scale on rails
            rail.transform.localScale = new Vector3(0.12f, 0.12f, 0.2f);
        }
        else
        {
            rail = Instantiate(PrefabStraightRail);

            //Apply first the scale on rails
            rail.transform.localScale = new Vector3(0.12f, 0.12f, 0.2f);
        }

        //Compute lenghts
        float railLenght = rail.GetComponent<Renderer>().bounds.extents.z;
        float railHeight = rail.GetComponent<Renderer>().bounds.extents.y;
        float floorLenght = floorTransform.GetComponent<Renderer>().bounds.extents.z;
        float floorHeight = floorTransform.GetComponent<Renderer>().bounds.extents.y;

        //Find the direction of the current cube
        Vector3 railPosition;
        Quaternion railRotation;

        if (currentNode.GetDirection() == Direction.East || currentNode.GetDirection() == Direction.West)
        {
            railPosition = new Vector3(basePosition.x - (floorLenght) + railLenght, floorHeight + railHeight, basePosition.z);
            railRotation = Quaternion.Euler(-90, 90, 0);
        }
        else
        {
            railPosition = new Vector3(basePosition.x, floorHeight + railHeight, basePosition.z - (floorLenght) + railLenght);
            railRotation = Quaternion.Euler(-90, 0, 0);
        }            

        //Compute how many rails we need to fill the fragment
        float neededRails = ((floorLenght * 2) / (railLenght * 2))-1;

        rail.transform.position = railPosition;
        rail.transform.rotation = railRotation;
        rail.transform.SetParent(floorTransform);

        //Create rails until they fill the corridor segment
        for (int i = 0; i < neededRails; i++)
        {
            GameObject nextRail = Instantiate(PrefabStraightRail);
            nextRail.transform.localScale = new Vector3(0.12f, 0.12f, 0.2f);

            //Find the direction of the current cube
            Vector3 nextRailPosition;
            Quaternion nextRailRotation;

            if (currentNode.GetDirection() == Direction.East || currentNode.GetDirection() == Direction.West)
            {
                nextRailPosition = railPosition + new Vector3(railLenght * 2 * i, 0, 0);
                nextRailRotation = Quaternion.Euler(-90, 90, 0);
            }
            else
            {
                nextRailPosition = railPosition + new Vector3(0, 0, railLenght * 2 * i);
                nextRailRotation = Quaternion.Euler(-90, 0, 0);
            }

            nextRail.transform.position = nextRailPosition;
            nextRail.transform.rotation = nextRailRotation;
            nextRail.transform.SetParent(floorTransform);
        }
    }
    private void CreateWall(Direction d, Transform floorTransform, Vector3 basePosition, float decal, MazeNode node)
    {
        GameObject wall = Instantiate(PrefabWall);/*GameObject.CreatePrimitive(PrimitiveType.Cube);*/
        wall.transform.localScale = new Vector3(0.06f * _cubeSize, 0.15f * _wallHeight, 0.4f);

        switch (d)
        {
            case Direction.North:
                wall.name = "North Wall ";
                wall.transform.position = basePosition + new Vector3(0, 0, _cubeSize / 2.0f - decal);
                wall.transform.rotation = Quaternion.Euler(0, -180, 0);/* = new Vector3(0.05f * _cubeSizeX, 0.5f *_wallHeight, 0.05f);*/
                break;
            case Direction.East:
                wall.name = "East Wall ";
                wall.transform.position = basePosition + new Vector3(_cubeSize / 2.0f - decal, 0, 0);
                wall.transform.rotation = Quaternion.Euler(0, -90, 0);
                break;
            case Direction.West:
                wall.name = "West Wall ";
                wall.transform.position = basePosition + new Vector3(-_cubeSize / 2.0f + decal, 0, 0);
                wall.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case Direction.South:
                wall.name = "South Wall ";
                wall.transform.position = basePosition + new Vector3(0, 0, -_cubeSize / 2.0f + decal);
                wall.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
        }

        wall.transform.SetParent(floorTransform);
    }
    private void CreateCeiling(Transform floorTransform, Vector3 basePosition)
    {
        //Create ceiling
        GameObject ceiling = Instantiate(PrefabCeiling);

        ceiling.transform.position = new Vector3(basePosition.x, WallHeight + 3f, basePosition.z);
        ceiling.transform.rotation = Quaternion.Euler(90, 90 * Random.Range(0, 3), 0);
        ceiling.transform.localScale = new Vector3(0.09f * _cubeSize, 0.09f * _cubeSize, 0.6f);
        ceiling.transform.SetParent(floorTransform);

    }
    /* --- */
    #endregion

}

