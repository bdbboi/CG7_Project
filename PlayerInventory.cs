using UnityEngine;
//chat gpt사용
public class PlayerInventory : MonoBehaviour
{
    // 조개탄을 들고 있는 상태
    public bool hasZogetan = false;

    // 여러 개 수집할 경우
    public int zogetanCount = 0;

    // 조개탄을 획득하는 함수
    public void AddZogetan()
    {
        hasZogetan = true;
        zogetanCount++;
        Debug.Log("조개탄 획득! 현재 개수: " + zogetanCount);
    }
}
