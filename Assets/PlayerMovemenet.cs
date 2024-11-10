using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovemenet : MonoBehaviour
{
    [SerializeField] private CameraMovemenet camMovemenet;
    private Rigidbody rb;
    private Animator animator;
    private Collider playerCollider;

    public float speed = 10f;
    public float jumpPower = 3f;
    public int maxJumpCnt;
    public int curJumpCnt;
    public float dash = 5f;
    public float rotSpeed = 3f;
    public int hashAttackCnt = Animator.StringToHash("AttackCount");
    private Vector3 dir = Vector3.zero;

    [SerializeField]private bool isground = false;
    public LayerMask layer;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 카메라 방향을 기준으로 이동할 방향 설정
        Vector3 cameraForward = camMovemenet.transform.forward;
        Vector3 cameraRight = camMovemenet.transform.right;

        cameraForward.y = 0; // 수평 방향으로만 이동하도록 y 축을 0으로 설정
        cameraRight.y = 0;

        cameraForward.Normalize();   //대각선 이동시 가속되는 현상을 방지하고자 값을 정규화 시킨다.
        cameraRight.Normalize();

        // 입력값을 카메라 방향에 맞춰 변환
        dir = cameraForward * Input.GetAxis("Vertical") + cameraRight * Input.GetAxis("Horizontal");
        dir.Normalize();
        animator.SetFloat("Speed", dir.magnitude);  // 애니메이션 Speed 파라미터에 이동 방향의 크기를 전달
        animator.SetFloat("velocityY", rb.velocity.y);

        CheckGround();

        if(Input.GetButtonDown("Jump") && isground)
        {
            Vector3 JumpPower = Vector3.up * jumpPower; 
            rb.AddForce(JumpPower, ForceMode.VelocityChange);
        }

        if(Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Attack");
        }

    }

    private void FixedUpdate()
    {
        //키입력을 받았을 때
        if (dir != Vector3.zero)
        {
            //지금 바라보는 방향의 부호 != 나아갈 방향 부호 -> 미리 회전 시키기 (미리 회전시키지 않을 경우 어디로 회전해야할 지 모르기 때문에 문제가 발생한다.)
            if(Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) || Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))  //양수인지 음수인지 판단 Sign
            {
                transform.Rotate(0, 1, 0);
            }

            //캐릭터의 앞방향은 dir
            transform.forward = Vector3.Lerp(transform.forward, dir, rotSpeed * Time.deltaTime);
        }
        //현재위치에서 나아갈 방향백터를 더해줘야한다.
        rb.MovePosition(gameObject.transform.position + dir * speed * Time.deltaTime);
    }
    private void CheckGround()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + (Vector3.up * 0.2f);  // 레이 시작점
        Vector3 rayDirection = Vector3.down;  // 레이 방향

        // Raycast 시도
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 0.2f, layer))
        {
            isground = true;
        }
        else
        {
            isground = false;
        }

        // Ray 시각적으로 표시 (빨간색)
        Debug.DrawRay(rayOrigin, rayDirection * 0.2f, Color.red);
    }
    public int AttackCount
    {
        get => animator.GetInteger(hashAttackCnt);
        set => animator.SetInteger(hashAttackCnt, value);
    }

}
