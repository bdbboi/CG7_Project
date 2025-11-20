using UnityEngine;
using UnityEngine.InputSystem; // New Input System을 사용하는 경우 필요
//제미나이사. 변수수정
public class CameraMove : MonoBehaviour
{
    // 이동 속도 (값이 클수록 목표에 빨리 도달)
    public float moveSpeed = 5f; 
    
    // 카메라가 멈춰야 할 목표 Y 위치
    private const float targetYPosition = 7.0f; 

    void LateUpdate()
    {
        return;
        // New Input System의 Keyboard.current.ctrlKey.isPressed를 사용합니다.
        if (Keyboard.current != null && Keyboard.current.ctrlKey.isPressed)
        {
            // 현재 카메라 위치
            Vector3 currentPos = transform.position;
            
            // 목표 위치 (X, Z는 현재 위치를 유지하고 Y만 목표 Y로 설정)
            Vector3 targetPos = new Vector3(currentPos.x, targetYPosition, currentPos.z);

            // Lerp를 사용하여 목표 위치로 부드럽게 이동합니다.
            // moveSpeed * Time.deltaTime 비율로 이동하며, 이 값이 1에 가까워질수록 빠르게 목표에 도달합니다.
            transform.position = Vector3.Lerp(
                currentPos, 
                targetPos, 
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            // Ctrl 키를 떼면 카메라를 위로 올릴 수 있도록 추가 로직을 구현합니다.
            // 예시: 원래의 Y 위치 (5.0f)로 돌아가기
            
            Vector3 currentPos = transform.position;
            Vector3 originalTargetPos = new Vector3(currentPos.x, 11.0f, currentPos.z);
            
            transform.position = Vector3.Lerp(
                currentPos,
                originalTargetPos,
                moveSpeed * Time.deltaTime
            );
            
            
        }

        if (transform.position.y < targetYPosition)
        {
            transform.position = new Vector3(transform.position.x, targetYPosition, transform.position.z);
        }
    }
}

