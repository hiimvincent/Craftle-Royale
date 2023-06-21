using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VictoryPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI curStreakText;
    [SerializeField] TextMeshProUGUI maxStreakText;

    private void OnEnable()
    {
        GameManager gm = GameManager.GameManagerInstance;

        curStreakText.text = gm.curStreak.ToString("D3");
        maxStreakText.text = gm.maxStreak.ToString("D3");
    }
}
