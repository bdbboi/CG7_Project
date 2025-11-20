using UnityEngine;
using UnityEngine.InputSystem;
//chat gpt사용
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))] 
public class PlayerCrouch : MonoBehaviour
{
    [Header("Crouch Settings")]
    public float crouchHeight = 1.0f; // ?몄뒪?숉꽣?먯꽌 Stand Height蹂대떎 ?묒? 媛믪쑝濡??ㅼ젙?댁빞 ?⑸땲?? (?? 1.0)
    
    public float standHeight = 2.0f; // ?몄뒪?숉꽣?먯꽌 Crouch Height蹂대떎 ??媛믪쑝濡??ㅼ젙?댁빞 ?⑸땲?? (?? 2.0)
    
    public float crouchCameraY = 1.1f; 
    
    public float standCameraY = 1.7f;
    
    public float crouchSpeed = 15f; 

    [Header("Component References")]
    public Transform cameraTransform;

    public Transform armsTransform_right;
    public Transform armsTransform_left; 

    public Transform playerTransform; 
    [Header("Visual Scale")]
    public Vector3 standScale = Vector3.one;
    public Vector3 crouchScale = new Vector3(1f, 0.7f, 1f);
    private CapsuleCollider playerCollider;
    private Rigidbody rb;
    
    void Start()
    {
        playerCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>(); 

        playerCollider.height = standHeight;
        //주석처리로 삭제
        /*playerCollider.center = new Vector3(
            playerCollider.center.x, standHeight / 2f, playerCollider.center.z);
        */
        if (cameraTransform != null)
        {
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x, standCameraY, cameraTransform.localPosition.z);
        }
        if (armsTransform_right != null)
        {
            armsTransform_right.localPosition = new Vector3(
               armsTransform_right.localPosition.x, standCameraY, armsTransform_right.localPosition.z);
        }
        if (armsTransform_left != null)
        {
            armsTransform_left.localPosition = new Vector3(
               armsTransform_left.localPosition.x, standCameraY, armsTransform_left.localPosition.z);
                }
        // Visual model scale init
        if (playerTransform == null) playerTransform = transform;
        playerTransform.localScale = standScale;    }

    void FixedUpdate()
    {
        float targetHeight;
        float targetCameraY;

        float lerpT = crouchSpeed * Time.fixedDeltaTime; 
        
        bool isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || (Keyboard.current != null && Keyboard.current.ctrlKey.isPressed);

        if (isCrouching)
        {
            targetHeight = crouchHeight;
            targetCameraY = crouchCameraY;
        }
        else
        {
            targetHeight = standHeight;
            targetCameraY = standCameraY;
        }
?
        float oldHeight = playerCollider.height; 
        
        playerCollider.height = Mathf.Lerp(
            playerCollider.height, 
            targetHeight, 
            lerpT
        );
        
        // 바닥 고정: center = height/2 유지하면 추가 위치 보정 불필요
        //아래 수정함.
        /*playerCollider.center = new Vector3(
            playerCollider.center.x, 
            playerCollider.height / 2f, 
            playerCollider.center.z
        );
        */
        // 臾쇰━ ?쒖뒪?쒖뿉 蹂寃??ы빆??利됱떆 ?숆린??
        Physics.SyncTransforms();

        if (cameraTransform != null)
        {
            Vector3 targetCamLocalPos = new Vector3(
                cameraTransform.localPosition.x, targetCameraY, cameraTransform.localPosition.z);

            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                targetCamLocalPos,
                lerpT
            );
        }

        if (armsTransform_right != null)
        {
            Vector3 targetArmsLocalPos = new Vector3(
               armsTransform_right.localPosition.x, targetCameraY, armsTransform_right.localPosition.z);

            armsTransform_right.localPosition = Vector3.Lerp(
                armsTransform_right.localPosition,
                targetArmsLocalPos,
                lerpT
            );
        }
        if (armsTransform_left != null)
        {
            Vector3 targetArmsLocalPos = new Vector3(
               armsTransform_left.localPosition.x, targetCameraY, armsTransform_left.localPosition.z);

            armsTransform_left.localPosition = Vector3.Lerp(
                armsTransform_left.localPosition,
                targetArmsLocalPos,
                lerpT
            );
                }
        // 3. Visual model scale blend
        if (playerTransform != null)
        {
            Vector3 targetScale = isCrouching ? crouchScale : standScale;
            playerTransform.localScale = Vector3.Lerp(playerTransform.localScale, targetScale, lerpT);
        }    }
}





