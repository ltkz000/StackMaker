using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;  

    //gameObj 
    [SerializeField] private GameObject StackHolder;
    [SerializeField] private GameObject plusStack;
    [SerializeField] private GameObject passedYellowStack;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerTranform;

    //Gameplay Variables
    public float moveSpeed;
    private int stackCount;
    public enum Direction {Idle, Forward, Backward, Left, Right}
    private Direction direction;
    private Direction nextDirection;
    private Vector3 targetPosition;
    private float size;
    private bool win;
    public GameObject winParticlesObject;
    public GameObject closedArk;
    public GameObject openArk;

    // Start is called before the first frame update
    void Start()
    {
        stackCount = 0;
        direction = Direction.Idle;
        win = false;
        size = plusStack.GetComponent<MeshRenderer>().bounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(stackCount.ToString() + playerTranform.position + "-" + targetPosition + direction);
        if(direction == Direction.Idle && !win){
            getPlayerInput();
        }else{
            Move();
            if(Vector3.Distance(playerTranform.position, targetPosition) < 0.1f)
            {
                checkNextDirection();
            }
        }
    }

    //Moving
    private void Move()
    {
        playerTranform.position = Vector3.MoveTowards(playerTranform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void checkNextDirection()
    {
        playerTranform.position = targetPosition;
        direction = nextDirection;
        nextDirection = Direction.Idle;

        switch(direction)
        {
            case Direction.Forward:
                getTargetPosition(Vector3.forward);
                break;
            case Direction.Backward:
                getTargetPosition(Vector3.back);
                break;
            case Direction.Left:
                getTargetPosition(Vector3.left);
                break;
            case Direction.Right:
                getTargetPosition(Vector3.right);
                break;
        }
    }

    private bool getTargetPosition(Vector3 directionVector)
    {
        int layer_mask = LayerMask.GetMask("Wall");
        if(Physics.Raycast(playerTranform.position - Vector3.up * 0.5f, directionVector, out RaycastHit hit, Mathf.Infinity, layer_mask))
        {
            targetPosition = playerTranform.position + directionVector * (hit.distance - 0.5f);
            return true;
        }
        return false;
    }

    private void getPlayerInput()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            if(getTargetPosition(Vector3.forward))
            {
                direction = Direction.Forward;
            }
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            if(getTargetPosition(Vector3.back))
            {
                direction = Direction.Backward;
            }
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            if(getTargetPosition(Vector3.left))
            {
                direction = Direction.Left;
            }
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            if(getTargetPosition(Vector3.right))
            {
                direction = Direction.Right;
            }
        }
    }

    private void AddStack(GameObject stack)
    {
        if(stackCount != 0)
        {
            playerBody.position += Vector3.up * size;

            StackHolder.transform.position += Vector3.up *size;
        }

        stackCount += 1;
        stack.tag = "Untagged";
        stack.transform.SetParent(StackHolder.transform);
        stack.transform.position = playerBody.position - (Vector3.up * stackCount * size);
    }

    private void RemoveStack(GameObject connectObj)
    {
        playerBody.position -= Vector3.up * size;
        
        stackCount--;

        GameObject topStack = StackHolder.transform.GetChild(0).gameObject;
        Destroy(topStack);

        //Leave passedYellowStack
        GameObject passedYellow = Instantiate(passedYellowStack) as GameObject;
        passedYellow.transform.SetParent(connectObj.transform);
        passedYellow.transform.position = connectObj.transform.position + new Vector3(0, 0.04f, 0);
        connectObj.tag = "Untagged";
    }

    private void OnTriggerEnter(Collider other) {
        GameObject colliderObject = other.gameObject;
        switch(colliderObject.tag)
        {
            case "Stackpickup":
                AddStack(colliderObject);
                break;
            case "Stackremove":
                RemoveStack(colliderObject);
                break;
            case "Win":
                Win(colliderObject);
                break;
        }
    }

    private void Win(GameObject winPos)
    {
        win = true;
        winPos.tag = "Untagged";
        UIManager.Ins.SetScoreValue(stackCount*10);

        foreach(Transform child in StackHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        playerBody.position += Vector3.down * size * stackCount;
        playerBody.eulerAngles = Vector3.up * 0;

        closedArk.SetActive(false);
        openArk.SetActive(true);
        foreach (Transform child in winParticlesObject.transform)
        {
            child.GetComponent<ParticleSystem>().Play();
        }

        playerBody.gameObject.GetComponent<Animator>().Play("player_Win");
        Invoke("CompleteGame", 2.5f);
    }

    private void CompleteGame()
    {
        Time.timeScale = 0;
        UIManager.Ins.ActiveMenu();
    }
}
