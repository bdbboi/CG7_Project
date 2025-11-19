using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TeacherAI : MonoBehaviour
{
    [Header("참조")]
    public Transform player;      // 플레이어 Transform
    public NavMeshAgent agent;    // NavMeshAgent

    [Header("속도 설정")]
    public float walkSpeed = 5f;   // 걷기 속도 (유저보다 1.3배 정도로 맞춰도 됨)
    public float runSpeed = 8f;    // 뛰기 속도 (걷기의 ~2배)
    public float dashSpeed = 12f;  // 대쉬 속도 (뛰기의 ~1.5배~2배)

    [Header("시야 / 공격")]
    public float viewDistance = 4.68f; // 시야 거리
    public float viewAngle = 45f;      // 시야 각도
    public float attackDistance = 1.8f; // 공격 거리
    public float attackAngle = 120f;    // 공격 각도

    [Header("시간 / 확률")]
    public float firstRunDuration = 3f;   // 처음 발견시 뛰기 3초
    public float chaseStepDuration = 2f;  // 걷기/뛰기 전환 간격
    public float dashDuration = 2f;       // 대쉬 2초
    [Range(0f, 1f)]
    public float dashProbability = 0.2f;  // 대쉬 확률 20%

    private enum MoveMode { Idle, Walk, Run, Dash }
    private MoveMode currentMode = MoveMode.Walk;

    private bool isChasing = false;
    private Coroutine chaseRoutine;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.speed = walkSpeed; // 기본은 걷기 속도
    }

    void Update()
    {
        if (player == null || agent == null) return;

        // 1. 공격 범위에 들어왔는지 확인
        if (IsPlayerInAttackRange())
        {
            Debug.Log("Teacher: 플레이어 공격!");
            // 나중에 여기서 HP 깎기, 리스폰 처리 넣으면 됨
        }

        // 2. 시야 범위에 들어왔는지 확인
        if (IsPlayerInView())
        {
            if (!isChasing)
            {
                StartChase();
            }
        }

        // 3. 추격 중이면 항상 플레이어를 향해 이동
        if (isChasing)
        {
            agent.SetDestination(player.position);
        }

        // 4. 이동 모드에 맞게 속도 적용
        ApplyMoveMode();
    }

    // ====== 시야 / 공격 판정 ======

    bool IsPlayerInView()
    {
        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;
        if (distance > viewDistance) return false;

        float angle = Vector3.Angle(transform.forward, toPlayer);
        if (angle > viewAngle * 0.5f) return false;

        // 여기서는 일단 Raycast(벽 막힘)는 생략, 나중에 필요하면 추가하자
        return true;
    }

    bool IsPlayerInAttackRange()
    {
        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;
        if (distance > attackDistance) return false;

        float angle = Vector3.Angle(transform.forward, toPlayer);
        return angle <= attackAngle * 0.5f;
    }

    // ====== 추격 패턴 ======

    void StartChase()
    {
        isChasing = true;

        if (chaseRoutine != null)
            StopCoroutine(chaseRoutine);

        chaseRoutine = StartCoroutine(ChasePattern());
    }

    IEnumerator ChasePattern()
    {
        // 유저 처음 발견시: 3초간 뛰기
        currentMode = MoveMode.Run;
        yield return new WaitForSeconds(firstRunDuration);

        // 이후: 걷기/뛰기 반복하면서 가끔 대쉬
        while (isChasing)
        {
            if (Random.value < dashProbability)
            {
                currentMode = MoveMode.Dash;
                yield return new WaitForSeconds(dashDuration);
            }
            else
            {
                currentMode = (currentMode == MoveMode.Run) ? MoveMode.Walk : MoveMode.Run;
                yield return new WaitForSeconds(chaseStepDuration);
            }
        }

        currentMode = MoveMode.Walk;
    }

    void ApplyMoveMode()
    {
        switch (currentMode)
        {
            case MoveMode.Idle:
                agent.speed = 0f;
                break;
            case MoveMode.Walk:
                agent.speed = walkSpeed;
                break;
            case MoveMode.Run:
                agent.speed = runSpeed;
                break;
            case MoveMode.Dash:
                agent.speed = dashSpeed;
                break;
        }
    }
}
