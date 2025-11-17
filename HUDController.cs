using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("HP (초록 5칸)")]
    public Image[] hpIcons;                 // HP1~HP5
    public Color hpOnColor = new Color(0.3f, 0.9f, 0.4f, 1f);
    public Color hpOffColor = Color.gray;

    [Header("스킬 포인트 (파란 동그라미 5개)")]
    public Image[] skillIcons;              // Skill1~Skill5
    public Color skillAvailableColor = new Color(0.4f, 0.8f, 1f, 1f);
    public Color skillUsedColor = Color.gray;

    [Header("승리 포인트 (별 3개)")]
    public Image[] scoreStars;              // Star1~Star3
    public Color scoreOnColor = Color.yellow;
    public Color scoreOffColor = Color.gray;

    [Header("인벤토리 슬롯 1개")]
    public Image inventorySlotBorder;       // InventorySlot 이미지
    public Color inventoryNormalColor = Color.white;
    public Color inventorySelectedColor = Color.red;

    private int maxHP;
    private int currentHP;

    private int maxSkillPoints = 5;
    private int usedSkillPoints = 0;

    private int maxScore = 3;
    private int currentScore = 0;

    private void Awake()
    {
        maxHP = hpIcons != null ? hpIcons.Length : 0;
        currentHP = maxHP;

        RefreshAll();
    }

    private void RefreshAll()
    {
        UpdateHP(currentHP);
        UpdateSkill(usedSkillPoints);
        UpdateScore(currentScore);
        SetInventorySelected(false);
    }

    // ====== HP ======
    public void UpdateHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);

        for (int i = 0; i < hpIcons.Length; i++)
        {
            bool alive = i < currentHP;
            if (hpIcons[i] != null)
            {
                hpIcons[i].color = alive ? hpOnColor : hpOffColor;
            }
        }
    }

    // ====== 스킬 포인트 ======
    // usedCount = 사용한 개수 (회색으로 바뀌는 아이콘 개수)
    public void UpdateSkill(int usedCount)
    {
        usedSkillPoints = Mathf.Clamp(usedCount, 0, maxSkillPoints);

        for (int i = 0; i < skillIcons.Length; i++)
        {
            bool available = i >= usedSkillPoints;
            if (skillIcons[i] != null)
            {
                skillIcons[i].color = available ? skillAvailableColor : skillUsedColor;
            }
        }
    }

    // ====== 별 점수 ======
    public void UpdateScore(int score)
    {
        currentScore = Mathf.Clamp(score, 0, maxScore);

        for (int i = 0; i < scoreStars.Length; i++)
        {
            bool on = i < currentScore;
            if (scoreStars[i] != null)
            {
                scoreStars[i].color = on ? scoreOnColor : scoreOffColor;
            }
        }
    }

    // ====== 인벤토리 슬롯 선택 효과 ======
    public void SetInventorySelected(bool selected)
    {
        if (inventorySlotBorder == null) return;

        inventorySlotBorder.color = selected ? inventorySelectedColor : inventoryNormalColor;
    }

    // 외부에서 상태 읽고 싶을 때용 getter
    public int GetCurrentHP() => currentHP;
    public int GetCurrentScore() => currentScore;
    public int GetUsedSkillPoints() => usedSkillPoints;
}
