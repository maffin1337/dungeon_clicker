using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;
using UnityEngine.UI;

public class DamageAds : MonoBehaviour
{
    public YandexGame sdk;
    public int damageRewardCheck;

    public void AdButtonDamage()
    {
        sdk._RewardedShow(2);
    }

    public void AdButtonDamageReward()
    {
        damageRewardCheck = 1;
    }
}
