using UnityEngine;
//chat gpt사용
public class ZogetanPickup : MonoBehaviour
{
    // 레이어 체크를 위해 레이어 이름 저장
    public string targetLayerName = "Zogetan";

    void Update()
    {
        // 마우스 좌클릭
        if (Input.GetMouseButtonDown(0))
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 레이캐스트 발사
        if (Physics.Raycast(ray, out hit, 100f))
        {
            // 클릭한 게 본인인지 확인
            if (hit.transform == transform)
            {
                // 플레이어 인벤토리 가져오기
                PlayerInventory inv = FindObjectOfType<PlayerInventory>();

                if (inv != null)
                {
                    inv.AddZogetan();
                }

                // 화면에서 제거 (파괴 또는 비활성화)
                gameObject.SetActive(false);
                // Destroy(gameObject);  // 파괴하고 싶으면 이걸로
            }
        }
    }
}
