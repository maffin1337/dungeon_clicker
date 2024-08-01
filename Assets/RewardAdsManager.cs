using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;
using UnityEngine.UI;

public class RewardAdsManager : MonoBehaviour
{
    public int moneyRewardCheck;
    public int damageRewardCheck;


    private void OnEnable() => YandexGame.RewardVideoEvent += Rewarded;

    private void OnDisable() => YandexGame.RewardVideoEvent -= Rewarded;

    void Rewarded(int id)
    {
        if (id == 1)
        {
            DoubleMoney();
        }
        else if (id == 2)
        {
            DoubleDamage();
        }
    }

    void DoubleMoney()
    {
        moneyRewardCheck = 1;
    }

    void DoubleDamage()
    {
        damageRewardCheck = 1;
    }
}
