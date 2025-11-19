using UnityEngine;
using UnityEngine.InputSystem;
//chat gpt사용
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))] 
public class PlayerCrouch : MonoBehaviour
{
    [Header("Crouch Settings")]
    [Tooltip("?낇겕?몄쓣 ??Capsule Collider??Y ?믪씠")]
    public float crouchHeight = 1.0f; // ?몄뒪?숉꽣?먯꽌 Stand Height蹂대떎 ?묒? 媛믪쑝濡??ㅼ젙?댁빞 ?⑸땲?? (?? 1.0)
    
    [Tooltip("???덉쓣 ??Capsule Collider??Y ?믪씠")]
    public float standHeight = 2.0f; // ?몄뒪?숉꽣?먯꽌 Crouch Height蹂대떎 ??媛믪쑝濡??ㅼ젙?댁빞 ?⑸땲?? (?? 2.0)
    
    [Tooltip("?낇겕?몄쓣 ??移대찓?쇱? ?붿씠 ?대젮媛?濡쒖뺄 Y ?꾩튂 (?? 0.5f)")]
    public float crouchCameraY = 1.1f; 
    
    [Tooltip("???덉쓣 ??移대찓?쇱? ?붿쓽 濡쒖뺄 Y ?꾩튂 (?? 1.7f)")]
    public float standCameraY = 1.7f;
    
    [Tooltip("?먯꽭媛 蹂?섎뒗 ?띾룄 (媛믪씠 ?댁닔濡?鍮좊쫫)")]
    public float crouchSpeed = 15f; 

    [Header("Component References")]
    [Tooltip("硫붿씤 移대찓?쇱쓽 Transform???곌껐?섏꽭??")]
    public Transform cameraTransform;

    [Tooltip("????紐⑤뜽??Transform???곌껐?섏꽭??")]
    public Transform armsTransform_right;
    public Transform armsTransform_left; 

    public Transform playerTransform; // ?ъ슜?섏? ?딆쑝誘濡?臾댁떆?대룄 ?⑸땲??

    [Header("Visual Scale")]
    public Vector3 standScale = Vector3.one;
    public Vector3 crouchScale = new Vector3(1f, 0.7f, 1f);
    private CapsuleCollider playerCollider;
    private Rigidbody rb;
    
    void Start()
    {
        // 罹≪뒓 肄쒕씪?대뜑? Rigidbody瑜?媛?몄샃?덈떎.
        playerCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>(); 

        // 珥덇린 ?ㅼ젙
        playerCollider.height = standHeight;
        /*playerCollider.center = new Vector3(
            playerCollider.center.x, standHeight / 2f, playerCollider.center.z);
        */
        // 移대찓?쇱? ??珥덇린 ?꾩튂 ?ㅼ젙
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
        // 紐⑺몴 ?믪씠? 紐⑺몴 濡쒖뺄 Y ?꾩튂瑜?寃곗젙?⑸땲??
        float targetHeight;
        float targetCameraY;

        // FixedUpdate ?쒖옉 遺遺꾩뿉??lerpT瑜??좎뼵?섏뿬 ?꾩껜 ?⑥닔?먯꽌 ?ъ슜 媛?ν븯?꾨줉 ?⑸땲??
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
        
        // ----------------------------------------------------
        // 1. Capsule Collider ?믪씠 諛?Rigidbody ?꾩튂 議곗젅 (臾쇰━ 異⑸룎泥?
        // ----------------------------------------------------
        
        // ?믪씠 蹂寃??꾩쓽 媛????
        float oldHeight = playerCollider.height; 
        
        // 1-1. Capsule Collider ?믪씠瑜?Lerp濡?遺?쒕읇寃?議곗젅
        playerCollider.height = Mathf.Lerp(
            playerCollider.height, 
            targetHeight, 
            lerpT
        );
        
        // 1-2. 肄쒕씪?대뜑 ?믪씠媛 蹂?덉쓣 ?? ?뚮젅?댁뼱???붾뱶 ?꾩튂瑜?蹂댁젙?⑸땲?? (諛붾떏???낆뿉 遺숈뼱?덈룄濡?
        // 바닥 고정: center = height/2 유지하면 추가 위치 보정 불필요
        //아래 수정함.
        // 1-3. 以묒떖(Center) ?꾩튂??議곗젅 (??긽 ?믪씠???덈컲?쇰줈 ?ㅼ젙)
        /*playerCollider.center = new Vector3(
            playerCollider.center.x, 
            playerCollider.height / 2f, 
            playerCollider.center.z
        );
        */
        // 臾쇰━ ?쒖뒪?쒖뿉 蹂寃??ы빆??利됱떆 ?숆린??
        Physics.SyncTransforms();

        // ----------------------------------------------------
        // 2. 移대찓?쇱? ?붿쓽 濡쒖뺄 Y ?꾩튂瑜?Lerp濡?遺?쒕읇寃?議곗젅 (?쒖젏)
        // ----------------------------------------------------
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




