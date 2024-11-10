using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class CameraMovemenet : MonoBehaviour
{
    public Transform objectToFollow;
    public float followSpeed = 10f; //이동 속도
    public float sensitivity = 10f;    //감도
    public float clampAngle = 70f;  //각도 제한

    private float rotX;
    private float rotY;

    public Transform realCamera;
    public Vector3 dirNormalized;   //백터를 제외한 방향값
    public Vector3 finalDir;    //최종적으로 정해진 방향 저장

    //카메라 사이에 방해물이 있을 때 카메라의 위치를 정해주는 변수들
    public float minDistance;   //카메라의 최소거리
    public float maxDistance;   //카메라의 최대거리
    public float finalDistance; //최종적으로 결정된 거리
    public float Smoothness = 10;   //보간시 카메라의 부드러움 정도를 지정

    // Start is called before the first frame update
    void Start()
    {
        //카메라 인풋 초기화
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        //백터값 초기화
        dirNormalized = realCamera.localPosition.normalized;    //normallized를 통해 크기를 0으로 초기화 해 방향만 저장
        finalDistance = realCamera.localPosition.magnitude; //magnitude를 통해 카메라가 얼마나 떨어져 있는지 확인할 수 있다. 

        //원신이나 명조 등 오픈월드에서 사용하는 방식
        Cursor.lockState = CursorLockMode.Locked;   //마우스 커서를 화면 중앙에 고정하고 움직이지 못하게 만드는 기능
        Cursor.visible = false; //커서 숨기기

        /*if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.None; //커서를 화면에서 자유롭게 움직일 수 있고, 화면 밖으로도 나갈 수 있습니다.
            Cursor.visible = false; //커서 숨기기
        }
        */
        //다른 기능으로는 CursorLockMode.Confined: 커서가 화면 안에만 머물며, 화면 밖으로 나갈 수 없습니다. (커서는 보입니다.)
    }

    // Update is called once per frame
    void Update()
    {

        //매 프레임 마다 input을 받기
        rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;    //회전 시 상하로 움직이기 때문에 Y축을 받는다.
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;    //회전 시 좌우로 움직이기 때문에 X축을 받는다.

        //제한각 설정
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);  //최소값 -70도, 최대값 70도

        Quaternion rot = quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }
    //카메라 움직임은 LateUpdate에서 실시
    private void LateUpdate()
    {
        //목표를 따라가야한다. 현재위치, 따라갈 오븢게트 위치, 따라갈 속력
        transform.position = Vector3.MoveTowards(transform.position, objectToFollow.position, followSpeed * Time.deltaTime);

        //최종적으로 정해진 방향 설정
        //캐릭터와 카메라 사이에 방해물이 존재할 경우 rayCast를 통해 감지
        finalDir = transform.TransformPoint(dirNormalized * maxDistance);   // LocalSpace에서 WorldSpace로 변경, 방향 * 최대거리

        //방해물 오브젝트의 정보를 저장하는 변수
        RaycastHit hit;
        //캐릭터와 카메라 사이에 방해물이 있을 경우
        if(Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            //없으면 최대 거리
            finalDistance = maxDistance;
        }
        //선형 보간(두 점 사이를 부드럽게 해준다.) (카메라의 위치 ,방향 * finalDistance, 부드러움 정도)
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * Smoothness);
    }
    private void OnDrawGizmos()
    {
        
    }
}
