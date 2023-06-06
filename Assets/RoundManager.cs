using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] TextAsset roundDataJson;

    RoundData rd;

    private void Awake()
    {
        string jsonString = roundDataJson.ToString();
        rd = JsonUtility.FromJson<RoundData>(jsonString);


    }

    public void OnNewRound()
    {
        ItemPoolItem target = SelectNewTarget();

    }

    private ItemPoolItem SelectNewTarget()
    {
        return rd.itemPool[Random.Range(0, rd.itemPool.Count)];
    }
}
