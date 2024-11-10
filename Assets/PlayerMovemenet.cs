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
        // ī�޶� ������ �������� �̵��� ���� ����
        Vector3 cameraForward = camMovemenet.transform.forward;
        Vector3 cameraRight = camMovemenet.transform.right;

        cameraForward.y = 0; // ���� �������θ� �̵��ϵ��� y ���� 0���� ����
        cameraRight.y = 0;

        cameraForward.Normalize();   //�밢�� �̵��� ���ӵǴ� ������ �����ϰ��� ���� ����ȭ ��Ų��.
        cameraRight.Normalize();

        // �Է°��� ī�޶� ���⿡ ���� ��ȯ
        dir = cameraForward * Input.GetAxis("Vertical") + cameraRight * Input.GetAxis("Horizontal");
        dir.Normalize();
        animator.SetFloat("Speed", dir.magnitude);  // �ִϸ��̼� Speed �Ķ���Ϳ� �̵� ������ ũ�⸦ ����
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
        //Ű�Է��� �޾��� ��
        if (dir != Vector3.zero)
        {
            //���� �ٶ󺸴� ������ ��ȣ != ���ư� ���� ��ȣ -> �̸� ȸ�� ��Ű�� (�̸� ȸ����Ű�� ���� ��� ���� ȸ���ؾ��� �� �𸣱� ������ ������ �߻��Ѵ�.)
            if(Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) || Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))  //������� �������� �Ǵ� Sign
            {
                transform.Rotate(0, 1, 0);
            }

            //ĳ������ �չ����� dir
            transform.forward = Vector3.Lerp(transform.forward, dir, rotSpeed * Time.deltaTime);
        }
        //������ġ���� ���ư� ������͸� ��������Ѵ�.
        rb.MovePosition(gameObject.transform.position + dir * speed * Time.deltaTime);
    }
    private void CheckGround()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + (Vector3.up * 0.2f);  // ���� ������
        Vector3 rayDirection = Vector3.down;  // ���� ����

        // Raycast �õ�
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 0.2f, layer))
        {
            isground = true;
        }
        else
        {
            isground = false;
        }

        // Ray �ð������� ǥ�� (������)
        Debug.DrawRay(rayOrigin, rayDirection * 0.2f, Color.red);
    }
    public int AttackCount
    {
        get => animator.GetInteger(hashAttackCnt);
        set => animator.SetInteger(hashAttackCnt, value);
    }

}
