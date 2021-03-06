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
    [Header("Prefab, material,...")]
    public GameObject PrefabWall;
    public GameObject PrefabEndCeiling;
    public GameObject PrefabCeilingMiddlePart;
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
            _cubeSize = DataManager.instance.CorridorWidth.Value;
            _wallHeight = DataManager.instance.WallHeight.Value;
            _length = DataManager.instance.CorridorLength.Value;
            _turnNumber = DataManager.instance.TurnNumber.Value;
            _imageSize = DataManager.instance.ImageSize.Value;

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

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
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
        Random.InitState(DataManager.instance.Seed.Value);
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
        _playerPath.m_Resolution = 100;
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

        foreach (var valuePair in _maze)
        {
            MazeNode node = valuePair.Value;

            Vector3 baseVector = new Vector3(node.GetPosition().X * _cubeSize, 0, node.GetPosition().Z * _cubeSize);


            #region Spawn Floor

            //Create Floor
            GameObject floor = CreateFloor(i, node);

            #endregion

            #region Spawn Image

            if (node.IsImage())
            {
                //Pick our future sprite
                Sprite selectedSprite;

                //True = Get Random image, False = get the images in alphabetical order
                if (DataManager.instance.RandomizeImage.Value)
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


            WallsToDestroy(node.GetNextNode(), node.GetPreviousNode(), out Direction directionSupprPrevious, out Direction directionSupprNext);


            //Create Wall South
            if (directionSupprPrevious != Direction.South && directionSupprNext != Direction.South)           
                CreateWall(Direction.South, floor.transform, baseVector);
                                           
            //Create Wall North
            if (directionSupprPrevious != Direction.North && directionSupprNext != Direction.North)           
                CreateWall(Direction.North, floor.transform, baseVector);                                        

            //Create Wall East
            if (directionSupprPrevious != Direction.East && directionSupprNext != Direction.East)        
                CreateWall(Direction.East, floor.transform, baseVector);
                                            
            //Create Wall West
            if (directionSupprPrevious != Direction.West && directionSupprNext != Direction.West)           
                CreateWall(Direction.West, floor.transform, baseVector);
               


            //Create Ceiling 
            CreateCeiling(floor.transform, baseVector);

            //Create Columns
            CreateColumns(directionSupprPrevious, directionSupprNext, floor.transform, baseVector);

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

        //Create Rails
        CreateRail();
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

        float midHeightOnWall = ((_wallHeight - 0.5f) / 2.0f) + 0.5f;

        switch (i)
        {
            case 0:
                return new Vector3(np.X * _cubeSize + xDecal, midHeightOnWall - midHeightOnWall/2.0f, np.Z * _cubeSize + zDecal);
            case 1:
                return new Vector3(np.X * _cubeSize + xDecal, midHeightOnWall + midHeightOnWall/2.0f, np.Z * _cubeSize + zDecal);
            case 2:
                return new Vector3(np.X * _cubeSize - xDecal, midHeightOnWall - midHeightOnWall/2.0f, np.Z * _cubeSize - zDecal);
            case 3:
                return new Vector3(np.X * _cubeSize - xDecal, midHeightOnWall + midHeightOnWall/2.0f, np.Z * _cubeSize - zDecal);
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
    private void CreateRail()
    {
        //Create the model with good scale
        GameObject rail = Instantiate(PrefabStraightRail, new Vector3(0,0,0), Quaternion.identity);
        rail.transform.localScale = new Vector3(0.12f, 0.08f, 0.12f);
        rail.transform.rotation = Quaternion.Euler(90, 0, 0);

        //Compute dimension of the rail
        float railLenght = rail.GetComponent<Renderer>().bounds.extents.z;
        float railHeight = rail.GetComponent<Renderer>().bounds.extents.y;

        //Compute the needed number of rail depending the path's lenght
        float p = _playerPath.PathLength / (railLenght*1.7f);
        float offset = _playerPath.MaxPos / p;

        //Add the other rails
        for (int i = 0; i < p; i++)
        {
            GameObject nextrail = Instantiate(PrefabStraightRail, _playerPath.EvaluatePosition(offset * i) + new Vector3(0, railHeight, 0), Quaternion.identity);
            nextrail.transform.localScale = new Vector3(0.12f, 0.08f, 0.12f);
            nextrail.transform.rotation = _playerPath.EvaluateOrientation(offset * i) * Quaternion.Euler(90, 0, 0);      
        }

        //Destroy the rail's model
        Destroy(rail);
    }
    private void CreateWall(Direction d, Transform floorTransform, Vector3 basePosition)
    {
        //Instantiate first wall
        GameObject wall = Instantiate(PrefabWall);
        wall.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        //Compute wall bounds for prefab of wall since they're composed of multiple gameobject
        Bounds bounds = new Bounds();
        bounds.size = Vector3.zero; // reset
        Renderer[] renderers = wall.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        //Declare some usefull variable
        int i = 1;
        float wallHeight = bounds.extents.y * 2;
        float wallLength = bounds.extents.x * 2;

        float currentHeight = wallHeight/2.0f; //Because the first wall is half in the ground (we don't wanna see space between floor and wall)
        float currentLength = wallLength;

        Vector3 startingPosition; 

        switch (d)
        {
            case Direction.North:
                wall.name = "North Wall ";

                //Define the starting position (bottom left corner of the section)
                startingPosition = basePosition + new Vector3((-_cubeSize / 2.0f) + wallLength / 2.0f, 0, (_cubeSize / 2.0f));
                wall.transform.position = startingPosition;
                wall.transform.rotation = Quaternion.Euler(0,-180,0);

                //Add walls to fit he length of the section
                i = 1;
                while(currentLength < _cubeSize)
                {
                    GameObject nextWall = Instantiate(PrefabWall);
                    nextWall.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    nextWall.transform.rotation = Quaternion.Euler(0, -180, 0);
                    nextWall.transform.position = startingPosition + new Vector3(i * (wallLength * (2.7f / 3.0f)), 0, 0);
                    nextWall.transform.parent = wall.transform;
                    ++i;
                    currentLength += wallLength * (2.7f/3.0f);
                }

                //Add walls to fit the height of the section
                i = 1;
                while (currentHeight < _wallHeight)
                {
                    GameObject nextWall = Instantiate(wall);
                    nextWall.transform.position = startingPosition + new Vector3(0, i * (wallHeight/2.0f), 0);
                    nextWall.transform.parent = wall.transform;
                    ++i;
                    currentHeight += wallHeight/2.0f;
                }

                break;
            case Direction.East:
                wall.name = "East Wall ";

                //Define the starting position (bottom left corner of the section)
                startingPosition = basePosition + new Vector3((_cubeSize / 2.0f), 0, (-_cubeSize / 2.0f) + wallLength / 2.0f);
                wall.transform.position = startingPosition;
                wall.transform.rotation = Quaternion.Euler(0, -90, 0);

                //Add walls to fit he length of the section
                i = 1;
                while (currentLength < _cubeSize)
                {
                    GameObject nextWall = Instantiate(PrefabWall);
                    nextWall.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    nextWall.transform.rotation = Quaternion.Euler(0, -90, 0);
                    nextWall.transform.position = startingPosition + new Vector3(0, 0, i * (wallLength * (2.7f / 3.0f)));
                    nextWall.transform.parent = wall.transform;
                    ++i;
                    currentLength += wallLength * (2.7f / 3.0f);
                }

                //Add walls to fit the height of the section
                i = 1;
                while (currentHeight < _wallHeight)
                {
                    GameObject nextWall = Instantiate(wall);
                    nextWall.transform.position = startingPosition + new Vector3(0, i * (wallHeight / 2.0f), 0);
                    nextWall.transform.parent = wall.transform;
                    ++i;
                    currentHeight += wallHeight / 2.0f;
                }


                break;
            case Direction.West:
                wall.name = "West Wall ";

                //Define the starting position (bottom left corner of the section)
                startingPosition = basePosition + new Vector3((-_cubeSize / 2.0f), 0, (-_cubeSize / 2.0f) + wallLength / 2.0f);
                wall.transform.position = startingPosition;
                wall.transform.rotation = Quaternion.Euler(0, 90, 0);

                //Add walls to fit he length of the section
                i = 1;
                while (currentLength < _cubeSize)
                {
                    GameObject nextWall = Instantiate(PrefabWall);
                    nextWall.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    nextWall.transform.rotation = Quaternion.Euler(0, 90, 0);
                    nextWall.transform.position = startingPosition + new Vector3(0, 0, i * (wallLength * (2.7f / 3.0f)));
                    nextWall.transform.parent = wall.transform;
                    ++i;
                    currentLength += wallLength * (2.7f / 3.0f);
                }

                //Add walls to fit the height of the section
                i = 1;
                while (currentHeight < _wallHeight)
                {
                    GameObject nextWall = Instantiate(wall);
                    nextWall.transform.position = startingPosition + new Vector3(0, i * (wallHeight / 2.0f), 0);
                    nextWall.transform.parent = wall.transform;
                    ++i;
                    currentHeight += wallHeight / 2.0f;
                }

                break;
            case Direction.South:
                wall.name = "South Wall ";

                //Define the starting position (bottom left corner of the section)
                startingPosition = basePosition + new Vector3((-_cubeSize / 2.0f) + wallLength / 2.0f, 0, (-_cubeSize / 2.0f));
                wall.transform.position = startingPosition;
                wall.transform.rotation = Quaternion.Euler(0, 0, 0);

                //Add walls to fit he length of the section
                i = 1;
                while (currentLength < _cubeSize)
                {
                    GameObject nextWall = Instantiate(PrefabWall);
                    nextWall.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    nextWall.transform.rotation = Quaternion.Euler(0, 0, 0);
                    nextWall.transform.position = startingPosition + new Vector3(i * (wallLength * (2.7f / 3.0f)), 0, 0);
                    nextWall.transform.parent = wall.transform;
                    ++i;
                    currentLength += wallLength * (2.7f / 3.0f);
                }

                //Add walls to fit the height of the section
                i = 1;
                while (currentHeight < _wallHeight)
                {
                    GameObject nextWall = Instantiate(wall);
                    nextWall.transform.position = startingPosition + new Vector3(0, i * (wallHeight / 2.0f), 0);
                    nextWall.transform.parent = wall.transform;
                    ++i;
                    currentHeight += wallHeight / 2.0f;
                }

                break;
        }

        wall.transform.SetParent(floorTransform);
    }
    private void CreateCeiling(Transform floorTransform, Vector3 basePosition)
    {
        //Create ceiling
        GameObject ceiling = Instantiate(PrefabCeilingMiddlePart);
        ceiling.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        ceiling.transform.rotation = Quaternion.identity;

        //Compute ceiling bounds for prefab of wall since they're composed of multiple gameobject
        Bounds bounds = new Bounds();
        bounds.size = Vector3.zero; // reset
        Renderer[] renderers = ceiling.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        //Usefull variables
        float ceilingLengthX = bounds.extents.x * 2;
        float ceilingLengthY = bounds.extents.y * 2;
        float ceilingLengthZ = bounds.extents.z * 2;

        //print(ceilingLengthX +" "+ ceilingLengthY+" "+ ceilingLengthZ);
        float currentFilledPart;

        Vector3 startingPosition = new Vector3(basePosition.x - (_cubeSize / 2.0f), DataManager.instance.WallHeight.Value + ceilingLengthY/2.0f, basePosition.z - (_cubeSize / 2.0f));
        ceiling.transform.position = startingPosition;

        //Fill X axis
        int i = 1;
        currentFilledPart = ceilingLengthX / 2.0f;
        while (currentFilledPart < _cubeSize)
        {
            GameObject nextCeiling = Instantiate(PrefabCeilingMiddlePart);
            nextCeiling.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            nextCeiling.transform.rotation = Quaternion.identity;

            nextCeiling.transform.position = startingPosition + new Vector3(i * (ceilingLengthX / 2.0f), 0, 0);
            nextCeiling.transform.parent = ceiling.transform;
            currentFilledPart += ceilingLengthX / 2.0f;
            ++i;
        }

        //Fill Z axis
        i = 1;
        currentFilledPart = ceilingLengthZ / 2.0f;
        while (currentFilledPart < _cubeSize)
        {
            GameObject nextCeiling = Instantiate(ceiling);

            nextCeiling.transform.position = startingPosition + new Vector3(0, 0, i * (ceilingLengthZ *(1.7f/ 2.0f)));
            nextCeiling.transform.parent = floorTransform.transform;
            currentFilledPart += ceilingLengthZ * (1.7f / 2.0f);
            ++i;
        }

        ceiling.transform.SetParent(floorTransform);

    }
    private void CreateColumns(Direction previousDirection, Direction nextDirection, Transform floorTransform, Vector3 basePosition)
    {
        GameObject leftColumns = Instantiate(PrefabColumn);        
        leftColumns.transform.localScale = new Vector3(0.25f, 0.25f , 0.3f + (1.0f / 5.0f) * (_wallHeight - 4f)); //We scale z cause prefab has to be rotated
        leftColumns.transform.rotation = Quaternion.Euler(90, 0, 0);

        GameObject rightColumns = Instantiate(leftColumns);
        float columnsHeight = leftColumns.GetComponent<Renderer>().bounds.extents.y * 2;


        switch (nextDirection)
        {
            case Direction.East:
                leftColumns.transform.position = basePosition + new Vector3(_cubeSize / 2.0f, _wallHeight/2.0f + 0.5f, _cubeSize * (0.8f / 2.0f));
                rightColumns.transform.position = basePosition + new Vector3(_cubeSize / 2.0f, _wallHeight / 2.0f + 0.5f, -_cubeSize * (0.8f / 2.0f));
                break;

            case Direction.West:
                leftColumns.transform.position = basePosition + new Vector3(-_cubeSize / 2.0f, _wallHeight / 2.0f + 0.5f, -_cubeSize * (0.8f / 2.0f));
                rightColumns.transform.position = basePosition + new Vector3(-_cubeSize / 2.0f, _wallHeight / 2.0f + 0.5f, _cubeSize * (0.8f / 2.0f));
                break;

            case Direction.North:
                leftColumns.transform.position = basePosition + new Vector3(-_cubeSize * (0.8f / 2.0f), _wallHeight / 2.0f + 0.5f, _cubeSize /2.0f);
                rightColumns.transform.position = basePosition + new Vector3(_cubeSize * (0.8f / 2.0f), _wallHeight / 2.0f + 0.5f, _cubeSize / 2.0f);
                break;

            case Direction.South:
                leftColumns.transform.position = basePosition + new Vector3(-_cubeSize * (0.8f / 2.0f), _wallHeight / 2.0f + 0.5f, -_cubeSize /2.0f);
                rightColumns.transform.position = basePosition + new Vector3(-_cubeSize * (0.8f / 2.0f), _wallHeight / 2.0f + 0.5f, _cubeSize / 2.0f);
                break;

            default:
                Destroy(leftColumns);
                Destroy(rightColumns);
                break;
        }

        leftColumns.transform.parent = floorTransform;
        rightColumns.transform.parent = floorTransform;
    }
    /* --- */
    #endregion

}

