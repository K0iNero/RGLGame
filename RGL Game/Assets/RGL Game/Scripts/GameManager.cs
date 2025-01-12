using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image[] playerHearts;
    public Sprite[] heartStatus;
    public int currentHearts;
	public int hp;

	static int minHearts = 1;
	static int maxHearts = 14;

	public int coins;
	public TextMeshProUGUI coinsText;

	public GameObject dialogBox;
	public TextMeshProUGUI dialogText;

	public GameObject npcDialogBox;
	public TextMeshProUGUI npcDialogText;
	public TextMeshProUGUI npcName;
	public Image npcImage;

	public GameObject[] spawnItems;
	public float[] spawnItemsChance;

	private void Awake()
	{
		DataInstance.Instance.LoadData();
		currentHearts = DataInstance.Instance.currentHearts;
		hp = DataInstance.Instance.hp;
		coins = DataInstance.Instance.coins;
	}

	void Start()
	{
		currentHearts = Mathf.Clamp(currentHearts, minHearts, maxHearts);
		hp = Mathf.Clamp(hp, minHearts, currentHearts);
		UpdateCurrentHearts();
		UpdateCoins(0);
	}

	public bool CanHeal()
	{
		return hp < currentHearts;
	}

	public void IncreaseMaxHP()
	{
		currentHearts++;
		currentHearts = Mathf.Clamp(currentHearts, minHearts, maxHearts);
		hp = currentHearts;
		UpdateCurrentHearts();
	}

	public void UpdateCurrentHP(int x)
	{
		hp += x;
		hp = Mathf.Clamp(hp, 0, currentHearts);
		UpdateCurrentHearts();
	}

	public void UpdateCurrentHearts()
	{
		int aux = hp;

		for (int i = 0; i < maxHearts; i++)
		{
			if (i < currentHearts)
			{
				playerHearts[i].enabled = true;
				playerHearts[i].sprite = GetHeartStatus(aux);
				aux -= 1;
			}
			else 
			{
				playerHearts[i].enabled = false;
			}
		}
	}


	private Sprite GetHeartStatus(int x)
	{
		switch (x)
		{
			case >=1: return heartStatus[1];
			default: return heartStatus[0];
		}
	}

	public void ShowText(string text)
	{
		dialogBox.SetActive(true);
		dialogText.text = text;
		Time.timeScale = 0;
	}

	public void HideText()
	{
		dialogBox.SetActive(false);
		dialogText.text = "";
		Time.timeScale = 1;
	}

	public void NPCShowText(string text, string name, Sprite image)
	{
		npcDialogBox.SetActive(true);
		npcDialogText.text = text;
		npcName.text = name;
		npcImage.sprite = image;
		Time.timeScale = 0;
	}

	public void NPCHideText() 
	{
		npcDialogBox.SetActive(false);
		npcDialogText.text = "";
		npcName.text = "";
		npcImage.sprite = null;
		Time.timeScale = 1;
	}

	public void SpawnItem(Vector2 enemyPos)
	{
		float x = Random.Range(0f, 100f);
		float sum = 0f;

		for (int i = 0; i < spawnItemsChance.Length; i++)
		{
			sum += spawnItemsChance[i];

			if (x < sum)
			{
				Instantiate(spawnItems[i], enemyPos, spawnItems[i].transform.rotation);
				break;
			}
		}
	}

	public void UpdateCoins(int amount)
	{
		coins = Mathf.Clamp(coins + amount, 0, 999);
		coinsText.text = coins.ToString();
	}
}
