using UnityEngine;
//참고 소스코드 https://www.youtube.com/watch?v=QAydinBwkp8&list=LL를 바탕으로 chatgpt로 수정
// 이 스크립트가 부착된 게임 오브젝트에는 Rigidbody 컴포넌트가 필수입니다.
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 15f;  // Walk Speed 15.0f 
    [SerializeField] float runSpeed = 25f;   // Run Speed 25.0f 
    [SerializeField] float jumpForce = 25f;  // <-- [조절 포인트] 이 값을 25f로 높여 초기 상승 속도(스피드)를 확보합니다.
    [SerializeField] float fallMultiplier = 4.0f; // 더 빠른 하강 속도 적용
    [SerializeField] float jumpHoldMultiplier = 2.0f; // 점프 키를 짧게 눌렀을 때 상승 가속을 줄이기 위한 값

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.2f; // 0.4f에서 0.2f로 줄여 지면 감지 정확도 및 연속 점프 방지
    [SerializeField] LayerMask groundMask = ~0; // 모든 레이어에서 점프 가능

    [Header("Look Settings")]
    [SerializeField] float mouseSpeed = 2000f; // 수정: 10000f에서 2000f로 낮춰 멈춤 현상 방지

    float xRot = 0f;
    Transform camTr;
    Rigidbody rb;
    float currentSpeed;
    bool isGrounded; // 인스펙터 Gizmo와 OnGUI에서 사용하기 위해 전역 변수로 유지

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 중요: Rigidbody 회전 고정은 에디터에서 설정해야 하지만, 코드로도 강제합니다.
        rb.freezeRotation = true; 

        Cursor.lockState = CursorLockMode.Locked;

        if (Camera.main != null)
        {
            camTr = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Main Camera not found! Please tag a camera as 'MainCamera'.");
        }
    }

    void Update()
    {


        // 2. 입력 감지
        HandleInput();
    }
    void LateUpdate()
{
    // 2. 카메라 시선 처리 (Look)는 LateUpdate에서 처리
    // -> FixedUpdate에서 처리되는 물리 이동(캐릭터 회전)이 완료된 후 실행하여 흔들림 방지
    Look();
}

    // 물리 기반 움직임은 FixedUpdate에서 처리
    void FixedUpdate()
    {
        CheckGroundStatus();
        
        MoveLogic();
        ApplyFasterFall();
    }

    void ApplyFasterFall()
    {
        // 1. 하강 중일 때 (rb.velocity.y < 0)
        if (rb.linearVelocity.y < 0)
        {
            // fallMultiplier를 사용하여 추가 중력 가속도를 적용합니다.
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // 2. 상승 중일 때 (rb.velocity.y > 0)
        else if (rb.linearVelocity.y > 0)
        {
            // 점프 키를 놓았을 때 (빠른 Apex)
            if (!Input.GetButton("Jump"))
            {
                // jumpHoldMultiplier를 사용하여 상승 속도를 빠르게 상쇄시킵니다.
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (jumpHoldMultiplier - 1) * Time.deltaTime;
            }
            // 점프 키를 누르고 있을 때 (높이 제한 로직 추가 - 속도는 유지, 높이만 낮춤)
            else
            {
                // jumpHoldMultiplier의 절반 정도를 추가 중력으로 적용하여, 
                // 추가로 작성한 코드.
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (jumpHoldMultiplier / 2.0f) * Time.deltaTime;
            }
        }
    }
//추가로 작성
    void CheckGroundStatus()
    {
        // GroundCheck 위치에서 groundDistance만큼의 구를 생성하여 지면 마스크와 충돌하는지 확인
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void HandleInput()
    {
        // Shift 키를 누르면 달리기 속도 적용
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        //CheckGroundStatus();
        // 점프 입력 처리
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }
    
    void Jump()
    {
        // 수직 속도를 초기화하고 점프 힘을 Impulse 모드로 가함
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); 
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void MoveLogic()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 이동 방향 계산 및 정규화
        Vector3 movement = (transform.right * h + transform.forward * v).normalized * currentSpeed;

        // Rigidbody의 속도를 직접 설정 (y축은 물리 엔진이 관리하도록 유지)
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }

    void Look()
    {
        if (camTr == null) return;
        
        // Time.deltaTime을 곱하여 프레임 속도에 독립적인 회전 속도를 보장합니다.
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        // 카메라 상하 회전
        camTr.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        // 플레이어 좌우 회전
        transform.Rotate(Vector3.up * mouseX);
    }
    
    // 유니티 에디터에서 지면 감지 구체를 시각적으로 확인하기 위한 Gizmo
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
    //ui적용후 삭제
   /* void OnGUI()
    {
        // 1. 크로스헤어
        float size = 8f;
        float x = (Screen.width - size) / 2f;
        float y = (Screen.height - size) / 2f;
        GUI.color = Color.white;
        GUI.DrawTexture(new Rect(x, y, size, size), Texture2D.whiteTexture);

        // 2. 지면 감지 디버그 텍스트 
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = isGrounded ? Color.green : Color.red;

        string debugText = isGrounded ? "IS GROUNDED: TRUE (점프 가능)" : "IS GROUNDED: FALSE (점프 불가능)";
        GUI.Label(new Rect(10, 10, 300, 50), debugText, style);
    }*/
}

