using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PetController : MonoBehaviour
{
    public float moveSpeed = 1.5f;          // 이동 속도
    public float minMoveTime = 1f;          // 최소 이동 시간
    public float maxMoveTime = 3f;          // 최대 이동 시간
    public float idleTime = 2f;             //  대기 시간
    public float MaxMoveRange = 8.2f;        // 최대 이동 거리
    private float moveTimer;
    private Vector2 moveDirection;

    public bool facingRight = true;     // 방향 플립 상태

    private Animator animator;
    private Rigidbody2D rb;

    private enum PetState { Idle, Walk, Sit, Laying, Run, Itch, Meow }
    private PetState currentState = PetState.Idle;
    private PetState previousState = PetState.Idle;

    private readonly string[] actionNames = { "Idle", "Walk", "Sit", "Laying", "Run", "Itch", "Meow" };
    private readonly int[] actionWeights = { 30, 50, 20, 20, 30, 10, 10 };
    private int totalActionWeight;

    private HashSet<PetState> playedStates = new HashSet<PetState>() {PetState.Walk, PetState.Run };    // 좌표 이동이 있는 상태들
    private bool HasPlayed(PetState state)  // 좌표 이동이 있는지 확인
    {
        if (playedStates.Contains(state))
            return true;
        else
            return false;
    }

    private string currentAnimation = "";

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        totalActionWeight = 0;
        for (int i = 0; i < actionWeights.Length; i++)
            totalActionWeight += actionWeights[i];

        RandomAction();
    }

    void Update()
    {
        moveTimer -= Time.deltaTime;

        if(HasPlayed(currentState))
        {
            HandleWalkingState();
        }
        else
        {
            if (moveTimer <= 0f)
                RandomAction();
        }

        if (currentState != previousState)
        {
            OnStateEnter(currentState);
            previousState = currentState;
        }
    }

    void FixedUpdate()
    {

        if (HasPlayed(currentState))
        {
            if(currentState == PetState.Run)
                rb.linearVelocity = moveDirection * moveSpeed * 2;
            else
                rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

    }


    public string ChooseAction()
    {
        float r = UnityEngine.Random.value * totalActionWeight;
        for (int i = 0; i < actionNames.Length; i++)
        {
            if (r < actionWeights[i])
                return actionNames[i];
            r -= actionWeights[i];
        }

        return actionNames[0];
    }

    void RandomAction()
    {
        string nextAction = ChooseAction();

        switch (nextAction)
        {
            case "Idle":
                currentState = PetState.Idle;
                moveTimer = idleTime;
                break;

            case "Walk":
                currentState = PetState.Walk;
                moveTimer = UnityEngine.Random.Range(minMoveTime, maxMoveTime);
                moveDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), 0).normalized;
                break;

            case "Sit":
                currentState = PetState.Sit;
                moveTimer = UnityEngine.Random.Range(minMoveTime, maxMoveTime);
                break;

            case "Laying":
                currentState = PetState.Laying;
                moveTimer = UnityEngine.Random.Range(minMoveTime, maxMoveTime);
                break;

            case "Run":
                currentState = PetState.Run;
                moveTimer = UnityEngine.Random.Range(minMoveTime, maxMoveTime);
                moveDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), 0).normalized;
                break;

            case "Itch":
                currentState = PetState.Itch;
                moveTimer = UnityEngine.Random.Range(minMoveTime, maxMoveTime);
                break;

            case "Meow":
                currentState = PetState.Meow;
                moveTimer = 1f;
                break;

            default:
                currentState = PetState.Idle;
                moveTimer = idleTime;
                break;
        }
    }

    void HandleWalkingState()
    {
        if ((moveDirection.x > 0 && !facingRight) || (moveDirection.x < 0 && facingRight))
        {
            Flip();
        }

        float positionX = transform.position.x;

        if (positionX*moveDirection.x >= MaxMoveRange)
        {
            currentState = PetState.Idle;
            moveTimer = idleTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (moveTimer <= 0f)
        {
            currentState = PetState.Idle;
            moveTimer = idleTime;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnStateEnter(PetState state)
    {
        string stateName = state.ToString();

        if (HasPlayed(state))
        {
            SetAnimation(stateName);
        }
        else
        {
            SetAnimation(stateName);
            rb.linearVelocity = Vector2.zero;
        }

    }

    private void SetAnimation(string anim)
    {
        if (animator == null)
            return;

        if (currentAnimation == anim)
            return;

        animator.Play(anim);
        currentAnimation = anim;
    }

    float SignedDistanceX(Vector2 a, Vector2 b)     // 부호가 있는 X축 거리 계산
    {
        return b.x - a.x;  // 부호 포함
    }
}
