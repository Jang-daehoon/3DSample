using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class CameraMovemenet : MonoBehaviour
{
    public Transform objectToFollow;
    public float followSpeed = 10f; //�̵� �ӵ�
    public float sensitivity = 10f;    //����
    public float clampAngle = 70f;  //���� ����

    private float rotX;
    private float rotY;

    public Transform realCamera;
    public Vector3 dirNormalized;   //���͸� ������ ���Ⱚ
    public Vector3 finalDir;    //���������� ������ ���� ����

    //ī�޶� ���̿� ���ع��� ���� �� ī�޶��� ��ġ�� �����ִ� ������
    public float minDistance;   //ī�޶��� �ּҰŸ�
    public float maxDistance;   //ī�޶��� �ִ�Ÿ�
    public float finalDistance; //���������� ������ �Ÿ�
    public float Smoothness = 10;   //������ ī�޶��� �ε巯�� ������ ����

    // Start is called before the first frame update
    void Start()
    {
        //ī�޶� ��ǲ �ʱ�ȭ
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        //���Ͱ� �ʱ�ȭ
        dirNormalized = realCamera.localPosition.normalized;    //normallized�� ���� ũ�⸦ 0���� �ʱ�ȭ �� ���⸸ ����
        finalDistance = realCamera.localPosition.magnitude; //magnitude�� ���� ī�޶� �󸶳� ������ �ִ��� Ȯ���� �� �ִ�. 

        //�����̳� ���� �� ���¿��忡�� ����ϴ� ���
        Cursor.lockState = CursorLockMode.Locked;   //���콺 Ŀ���� ȭ�� �߾ӿ� �����ϰ� �������� ���ϰ� ����� ���
        Cursor.visible = false; //Ŀ�� �����

        /*if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.None; //Ŀ���� ȭ�鿡�� �����Ӱ� ������ �� �ְ�, ȭ�� �����ε� ���� �� �ֽ��ϴ�.
            Cursor.visible = false; //Ŀ�� �����
        }
        */
        //�ٸ� ������δ� CursorLockMode.Confined: Ŀ���� ȭ�� �ȿ��� �ӹ���, ȭ�� ������ ���� �� �����ϴ�. (Ŀ���� ���Դϴ�.)
    }

    // Update is called once per frame
    void Update()
    {

        //�� ������ ���� input�� �ޱ�
        rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;    //ȸ�� �� ���Ϸ� �����̱� ������ Y���� �޴´�.
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;    //ȸ�� �� �¿�� �����̱� ������ X���� �޴´�.

        //���Ѱ� ����
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);  //�ּҰ� -70��, �ִ밪 70��

        Quaternion rot = quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }
    //ī�޶� �������� LateUpdate���� �ǽ�
    private void LateUpdate()
    {
        //��ǥ�� ���󰡾��Ѵ�. ������ġ, ���� ������Ʈ ��ġ, ���� �ӷ�
        transform.position = Vector3.MoveTowards(transform.position, objectToFollow.position, followSpeed * Time.deltaTime);

        //���������� ������ ���� ����
        //ĳ���Ϳ� ī�޶� ���̿� ���ع��� ������ ��� rayCast�� ���� ����
        finalDir = transform.TransformPoint(dirNormalized * maxDistance);   // LocalSpace���� WorldSpace�� ����, ���� * �ִ�Ÿ�

        //���ع� ������Ʈ�� ������ �����ϴ� ����
        RaycastHit hit;
        //ĳ���Ϳ� ī�޶� ���̿� ���ع��� ���� ���
        if(Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            //������ �ִ� �Ÿ�
            finalDistance = maxDistance;
        }
        //���� ����(�� �� ���̸� �ε巴�� ���ش�.) (ī�޶��� ��ġ ,���� * finalDistance, �ε巯�� ����)
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * Smoothness);
    }
    private void OnDrawGizmos()
    {
        
    }
}
