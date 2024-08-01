using UnityEngine;
using UnityEngine.UI;
using System;

public class GameController : MonoBehaviour
{
	public double money;
	public double damage;
	public double dps;
	public int stage;
	public double health;
	public double HealthMax
	{
		get
		{
			return 10 * Math.Pow(2, stage - 1) * isBoss;
		}
	}
	public float timer;
	public int kills;
	public int killsMax;
	public int isBoss;
	public int TimerMax;

	public Text MoneyText;
	public Text DamageText;
	public Text StageText;
	public Text HealthText;
	public Text TimerText;

	public Image HealthBar;
	public Image TimerBar;

	public Animator coinAnimation;
	public Animator inventoryAnimations;

	//Offline

	public DateTime cureentDate;
	public DateTime oldTime;
	public int offlineProgressCheck = 0;
	public float idleTime;
	public Text offlineTimeText;
	public float saveTime;
	public GameObject offlineBox;

	// Upgrades

	public Text damagePowerText;
	public Text damageCostText;
	public Image[] weaponImages;
	public Button[] upgradeButtons;
	public Button[] switchWeaponButtons;
	public int switchWeaponButtonId;
	public int upgradeButtonId;



	public string tempPowerText;
	public string tempCostText;
	public double tempPower;
	public int weaponId;

	// base weapon
	public double baseDamageCost
	{
		get
		{
			return 10 * Math.Pow(1.04, baseDamagelevel);
		}
	}
	public int baseDamagelevel;
	public double baseDamagePower
	{
		get
		{
			return 3 * baseDamagelevel;
		}
	}

	//knife

	public double knifeDamageCost
	{
		get
		{
			return 10 * Math.Pow(1.07, knifeDamageLevel);
		}
	}
	public int knifeDamageLevel;
	public double knifeDamagePower
	{
		get
		{
			return 5 * knifeDamageLevel;
		}
	}

	//  scythe

	public double scytheDamageCost
	{
		get
		{
			return 10 * Math.Pow(1.08, scytheDamageLevel);
		}
	}
	public int scytheDamageLevel;
	public double scytheDamagePower
	{
		get
		{
			return 8 * scytheDamageLevel;
		}
	}

	// Items
	// Pocket Watch
	public Text watchPowerText;
	public Text watchCostText;
	public double watchCost
	{
		get
		{
			return 10 * Math.Pow(2.7, watchLevel);
		}
	}
	public int watchLevel;
	public int watchPower
    {
		get
        {
			return 1 * watchLevel;
        }
    }

	// Enemy Sprites

	public Image[] enemyImages;
	public int enemyId;

	// Bosses

	public Image[] bossImages;
	public int bossId;

	// Boss Reward

	public GameObject rewardScreen;
	public int weaponRewardId = 0;
	public int itemRewardId = 0;
	public GameObject[] weaponRewards;
	public GameObject[] itemRewards;
	public Image[] weaponRewardsImage;
	public Image[] itemRewardsImage;

	// Ad Rewards
	public float moneyTimer;
	public int moneyTimerMax;
	public float damageTimer;
	public int damageTimerMax;
	public int moneyMultiplier;
	public int damageMultiplier;
	public Text moneyTimerText;
	public Text damageTimerText;
	public RewardAdsManager rewardAdsManager;
	public void Start()
	{
		offlineBox.gameObject.SetActive(false);
		moneyTimerText.gameObject.SetActive(false);
		damageTimerText.gameObject.SetActive(false);
		Load();
		IsBossChecker();
		health = HealthMax;
		timer = TimerMax;
		moneyTimerMax = 60;
		moneyTimer = moneyTimerMax;
		damageTimerMax = 60;
		damageTimer = damageTimerMax;
		moneyMultiplier = 1;
		damageMultiplier = 1;
		ChangeEnemyImage(enemyId);
		LoadWeapon(switchWeaponButtonId);
		RewardLoad();
	}

	public void Update()
	{
		if (money >= 1000)
		{
			MoneyText.text = "Money: " + (money / 1000).ToString("F2") + "k";
			if (money >= 1000000)
			{
				MoneyText.text = "Money: " + (money / 1000000).ToString("F2") + "m";
			}
		}
		else
		{
			MoneyText.text = "Money: " + money.ToString("F2");
		}
		if (damage >= 1000)
		{
			DamageText.text = "Damage: " + (damage / 1000).ToString("F1") + "k";
			if (damage >= 1000000)
			{
				DamageText.text = "Damage: " + (damage / 1000000).ToString("F1") + "m";
			}
		}
		else
		{
			DamageText.text = "Damage: " + damage;
		}
		HealthText.text = health + "/" + HealthMax + " HP";

		HealthBar.fillAmount = (float)(health / HealthMax);
		IsBossChecker();
		Upgrades();
		MoneyReward();
		DamageReward();

		saveTime += Time.deltaTime;
		if (saveTime >= 5)
		{
			saveTime = 0;
			Save();
		}
	}

	void ChangeEnemyImage(int id)
	{
		enemyImages[id].gameObject.SetActive(true);
	}
	void ChangeBossImage(int id)
	{
		bossImages[id].gameObject.SetActive(true);
	}

	public void IsBossChecker()
	{
		if (stage % 5 == 0)
		{
			isBoss = 10;
			StageText.text = "Stage: BOSS";
			timer -= Time.deltaTime;
			if (timer <= 0)
			{
				stage -= 1;
				health = HealthMax / 10;
				timer = TimerMax;
				killsMax = 10;
				bossImages[bossId].gameObject.SetActive(false);
				enemyImages[enemyId].gameObject.SetActive(true);
			}
			TimerText.text = "" + timer.ToString("F2");
			TimerBar.gameObject.SetActive(true);
			TimerBar.fillAmount = timer / TimerMax;
		}
		else
		{
			isBoss = 1;
			StageText.text = "Stage: " + stage;
			TimerText.text = "";
			TimerBar.gameObject.SetActive(false);
		}
	}

	public void Hit()
	{
		health -= damage;
		if (health <= 0)
		{
			Kill();
			if (kills >= killsMax)
			{
				kills = 0;
				stage += 1;
			}
			IsBossChecker();
			health = HealthMax;
			if (stage % 5 == 0)
			{
				timer = TimerMax;
				killsMax = 1;
				enemyImages[enemyId].gameObject.SetActive(false);
				bossId = UnityEngine.Random.Range(0, bossImages.Length);
				ChangeBossImage(bossId);
			}
			else
			{
				killsMax = 10;
				bossImages[bossId].gameObject.SetActive(false);
			}
				
				
		}
	}

	public void Kill()
	{
		money += Math.Ceiling(HealthMax / 14) * moneyMultiplier;

		kills += 1;
		coinAnimation.Play("CoinAnimation", 0, 0);
		enemyImages[enemyId].gameObject.SetActive(false);
		enemyId = UnityEngine.Random.Range(0, enemyImages.Length);
		ChangeEnemyImage(enemyId);

		if (stage % 5 == 0)
		{
			BossReward();
		}
		
	}

	public void Save()
	{
		offlineProgressCheck = 1;
		PlayerPrefs.SetString("money", money.ToString());
		PlayerPrefs.SetString("damage", damage.ToString());
		PlayerPrefs.SetString("dps", dps.ToString());
		PlayerPrefs.SetString("tempPowerText", tempPowerText);
		PlayerPrefs.SetString("tempCostText", tempCostText);
		PlayerPrefs.SetString("tempPower", tempPower.ToString());
		PlayerPrefs.SetInt("stage", stage);
		PlayerPrefs.SetInt("kills", kills);
		PlayerPrefs.SetInt("killsMax", killsMax);
		PlayerPrefs.SetInt("isBoss", isBoss);
		PlayerPrefs.SetInt("TimerMax", TimerMax);
		PlayerPrefs.SetInt("offlineProgressCheck", offlineProgressCheck);
		PlayerPrefs.SetInt("baseDamagelevel", baseDamagelevel);
		PlayerPrefs.SetInt("knifeDamageLevel", knifeDamageLevel);
		PlayerPrefs.SetInt("scytheDamageLevel", scytheDamageLevel);
		PlayerPrefs.SetInt("watchLevel", watchLevel);
		PlayerPrefs.SetInt("enemyId", enemyId);
		PlayerPrefs.SetInt("weaponId", weaponId);
		PlayerPrefs.SetInt("weaponRewardId", weaponRewardId);
		PlayerPrefs.SetInt("itemRewardId", itemRewardId);
		PlayerPrefs.SetInt("upgradeButtonId", upgradeButtonId);
		PlayerPrefs.SetInt("switchWeaponButtonId", switchWeaponButtonId);

		PlayerPrefs.SetString("offlineTime", DateTime.Now.ToBinary().ToString());
	}

	public void Load()
	{
		money = double.Parse(PlayerPrefs.GetString("money", "0"));
		damage = double.Parse(PlayerPrefs.GetString("damage", "1"));
		dps = double.Parse(PlayerPrefs.GetString("dps", "1"));
		tempPowerText = PlayerPrefs.GetString("tempPowerText", "Your Sword\n+0 Damage");
		tempCostText = PlayerPrefs.GetString("tempCostText", "10\nUpgrade");
		tempPower = double.Parse(PlayerPrefs.GetString("tempPower", "0"));
		stage = PlayerPrefs.GetInt("stage", 1);
		kills = PlayerPrefs.GetInt("kills", 0);
		killsMax = PlayerPrefs.GetInt("killsMax", 10);
		isBoss = PlayerPrefs.GetInt("isBoss", 1);
		TimerMax = PlayerPrefs.GetInt("TimerMax", 30);
		offlineProgressCheck = PlayerPrefs.GetInt("offlineProgressCheck", 0);
		baseDamagelevel = PlayerPrefs.GetInt("baseDamagelevel", 0);
		knifeDamageLevel = PlayerPrefs.GetInt("knifeDamageLevel", 0);
		scytheDamageLevel = PlayerPrefs.GetInt("scytheDamageLevel", 0);
		watchLevel = PlayerPrefs.GetInt("watchLevel", 0);
		enemyId = PlayerPrefs.GetInt("enemyId", 0);
		weaponId = PlayerPrefs.GetInt("weaponId", 0);
		weaponRewardId = PlayerPrefs.GetInt("weaponRewardId", 0);
		itemRewardId = PlayerPrefs.GetInt("itemRewardId", 0);
		upgradeButtonId = PlayerPrefs.GetInt("upgradeButtonId", 0);
		switchWeaponButtonId = PlayerPrefs.GetInt("switchWeaponButtonId", 0);
		LoadOfflineProduction();
	}

	public void LoadOfflineProduction()
	{
		if (offlineProgressCheck == 1)
		{
            offlineBox.gameObject.SetActive(true);
			long prevoiusTime = Convert.ToInt64(PlayerPrefs.GetString("offlineTime"));
			oldTime = DateTime.FromBinary(prevoiusTime);
			cureentDate = DateTime.Now;
			TimeSpan difference = cureentDate.Subtract(oldTime);
			idleTime = (float)difference.TotalSeconds;

			var moneyEarned = Math.Ceiling(HealthMax / 14) / HealthMax * (dps / 5) * idleTime;
			money += moneyEarned;
			TimeSpan timer = TimeSpan.FromSeconds(idleTime);

			offlineTimeText.text = "You were gone for: " + timer.ToString(@"hh\:mm\:ss") + "\n\nYou earned: " + moneyEarned.ToString("F2");
		}
	}

	public void CloseOfflineBox()
	{
		offlineBox.gameObject.SetActive(false);
	}

	public void InventoryOpen()
	{
		inventoryAnimations.Play("InventoryOpenAnim");
	}

	public void InventoryClose()
	{
		inventoryAnimations.Play("InventoryCloseAnim");
	}

	public void BuyUpgrade(string id)
	{
		switch (id)
		{
			case "baseDamage":
				if (money >= baseDamageCost)
				{
					UpgradeDefaults(ref baseDamagelevel, baseDamageCost);
				}
				break;
			case "knifeDamage":
				if (money >= knifeDamageCost)
				{
					UpgradeDefaults(ref knifeDamageLevel, knifeDamageCost);
				}
				break;
			case "scytheDamage":
				if (money >= scytheDamageCost)
				{
					UpgradeDefaults(ref scytheDamageLevel, scytheDamageCost);
				}
				break;
			case "watch":
				if (money >= watchCost)
				{
					UpgradeDefaults(ref watchLevel, watchCost);
					TimerMax = 30 + watchPower;
				}
				break;
		}
	}

	public void UpgradeDefaults(ref int level, double cost)
	{
		money -= cost;
		level++;
	}

	public void Upgrades()
	{
		switch (weaponId)
		{
			case 0:
				tempCostText = baseDamageCost.ToString("F2") + "\nUpgrade";
				tempPowerText = "Your Sword\n+" + baseDamagePower + " Damage";
				tempPower = baseDamagePower;
				break;
			case 1:
				tempCostText = knifeDamageCost.ToString("F2") + "\nUpgrade";
				tempPowerText = "Knife\n+" + knifeDamagePower + " Damage";
				tempPower = knifeDamagePower;
				break;
			case 2:
				tempCostText = scytheDamageCost.ToString("F2") + "\nUpgrade";
				tempPowerText = "Scythe\n+" + scytheDamagePower + " Damage";
				tempPower = scytheDamagePower;
				break;
		}
		weaponImages[weaponId].gameObject.SetActive(true);
		upgradeButtons[upgradeButtonId].gameObject.SetActive(true);
		damageCostText.text = tempCostText;
		damagePowerText.text = tempPowerText;
		damage = (1 + tempPower) * damageMultiplier;

		//Items
		//Watch

		watchCostText.text = watchCost.ToString("F2") + "\nUpgrade";
		watchPowerText.text = "Pocket Watch\n+" + watchPower + " to Boss Timer";
	}

	public void SwitchWeapon(string id)
	{
		switch (id)
		{
			case "base":
				weaponImages[weaponId].gameObject.SetActive(false);
				weaponId = 0;
				weaponImages[weaponId].gameObject.SetActive(true);
				upgradeButtons[upgradeButtonId].gameObject.SetActive(false);
				upgradeButtonId = 0;
				upgradeButtons[upgradeButtonId].gameObject.SetActive(true);
				switchWeaponButtons[switchWeaponButtonId].gameObject.SetActive(true);
				switchWeaponButtonId = 0;
				switchWeaponButtons[switchWeaponButtonId].gameObject.SetActive(false);
				break;
			case "knife":
				weaponImages[weaponId].gameObject.SetActive(false);
				weaponId = 1;
				weaponImages[weaponId].gameObject.SetActive(true);
				upgradeButtons[upgradeButtonId].gameObject.SetActive(false);
				upgradeButtonId = 1;
				upgradeButtons[upgradeButtonId].gameObject.SetActive(true);
				switchWeaponButtons[switchWeaponButtonId].gameObject.SetActive(true);
				switchWeaponButtonId = 1;
				switchWeaponButtons[switchWeaponButtonId].gameObject.SetActive(false);
				break;
			case "scythe":
				weaponImages[weaponId].gameObject.SetActive(false);
				weaponId = 2;
				weaponImages[weaponId].gameObject.SetActive(true);
				upgradeButtons[upgradeButtonId].gameObject.SetActive(false);
				upgradeButtonId = 2;
				upgradeButtons[upgradeButtonId].gameObject.SetActive(true);
				switchWeaponButtons[switchWeaponButtonId].gameObject.SetActive(true);
				switchWeaponButtonId = 2;
				switchWeaponButtons[switchWeaponButtonId].gameObject.SetActive(false);
				break;
		}
	}

	public void LoadWeapon(int id)
	{
		for (int i = 0; i < switchWeaponButtons.Length; i++)
		{
			if (i == id)
			{
				switchWeaponButtons[i].gameObject.SetActive(false);
			}
			else
			{
				switchWeaponButtons[i].gameObject.SetActive(true);
			}
		}
	}

	public void BossReward()
	{
		// 1 = weapon  2 = item
		int rand = UnityEngine.Random.Range(1, 10);
		if (rand <= 5)
		{
			if (weaponRewardId < weaponRewards.Length)
			{
				if (weaponRewardId == 0)
				{
					weaponRewards[weaponRewardId].gameObject.SetActive(true);
					weaponRewardsImage[weaponRewardId].gameObject.SetActive(true);
					weaponRewardId++;
				}
				else
				{
					weaponRewardsImage[weaponRewardId - 1].gameObject.SetActive(false);
					weaponRewards[weaponRewardId].gameObject.SetActive(true);
					weaponRewardsImage[weaponRewardId].gameObject.SetActive(true);
					weaponRewardId++;
				}
				rewardScreen.gameObject.SetActive(true);
			}
		}
		if (rand > 5)
		{
			if (itemRewardId < itemRewards.Length)
			{
				if (itemRewardId == 0)
				{
					itemRewards[itemRewardId].gameObject.SetActive(true);
					itemRewardsImage[itemRewardId].gameObject.SetActive(true);
					itemRewardId++;
				}
				else
				{
					itemRewardsImage[itemRewardId - 1].gameObject.SetActive(false);
					itemRewards[itemRewardId].gameObject.SetActive(true);
					itemRewardsImage[itemRewardId].gameObject.SetActive(true);
					itemRewardId++;
				}
				rewardScreen.gameObject.SetActive(true);
			}
		}
	}
	public void RewardLoad()
	{
		for (int i = 0; i < weaponRewardId; i++)
		{
			weaponRewards[i].gameObject.SetActive(true);
		}
		for (int i = 0; i < itemRewardId; i++)
		{
			itemRewards[i].gameObject.SetActive(true);
		}
	}

    public void Reset()
    {
		PlayerPrefs.DeleteAll();
		Load();
		IsBossChecker();
		health = HealthMax;
		timer = TimerMax;
		LoadWeapon(switchWeaponButtonId);
		SwitchWeapon("base");
		for (int i = 0; i < weaponRewards.Length; i++)
		{
			weaponRewards[i].gameObject.SetActive(false);
		}
		for (int i = 0; i < itemRewards.Length; i++)
		{
			itemRewards[i].gameObject.SetActive(false);
		}
	}

	public void MoneyReward()
	{
		int check = rewardAdsManager.moneyRewardCheck;
		if (check == 1)
		{
			moneyTimerText.gameObject.SetActive(true);
			moneyTimer -= Time.deltaTime;
			moneyMultiplier = 2;
			if (moneyTimer <= 0)
			{
				moneyMultiplier = 1;
				rewardAdsManager.moneyRewardCheck = 0;
				moneyTimer = moneyTimerMax;
				moneyTimerText.gameObject.SetActive(false);
			}
			moneyTimerText.text = "" + moneyTimer.ToString("F2");

		}
	}

	public void DamageReward()
	{
		int check = rewardAdsManager.damageRewardCheck;
		if (check == 1)
		{
			damageTimerText.gameObject.SetActive(true);
			damageTimer -= Time.deltaTime;
			damageMultiplier = 2;
			if(damageTimer <= 0)
            {
				damageMultiplier = 1;
				rewardAdsManager.damageRewardCheck = 0;
				damageTimer = damageTimerMax;
				damageTimerText.gameObject.SetActive(false);
            }
			damageTimerText.text = "" + damageTimer.ToString("F2");
		}
	}
}
