using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataInstance : MonoBehaviour
{
	private static DataInstance instance;

	public Vector2 playerPosition;

	public int currentHearts;
	public int hp;
	public int coins;

	static string SaveDataKey = "SaveDataKey";
	SaveData saveData;
	SceneData sceneData;

	public static DataInstance Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = new GameObject("DataInstance");
				instance = go.AddComponent<DataInstance>();
				DontDestroyOnLoad(go);
				instance.playerPosition = FindObjectOfType<PlayerMovement>().transform.position;
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public void SetPlayerPosition(Vector2 playerPos)
	{
		playerPosition = playerPos;

		GameManager gm = FindObjectOfType<GameManager>();

		currentHearts = gm.currentHearts;
		hp = gm.hp;
		coins = gm.coins;

		SavePlayerData();
	}

	private void SavePlayerData()
	{
		saveData.currentHearts = currentHearts;
		saveData.hp = hp;
		saveData.coins = coins;

		string json = JsonUtility.ToJson(saveData);
		PlayerPrefs.SetString(SaveDataKey, json);
		PlayerPrefs.Save();
	}

	public void SaveSceneData(string name)
	{
		if (sceneData == null)
		{
			sceneData = new SceneData();
			sceneData.objectsName = new List<string>();
		}

		if (saveData.sceneData.Contains(sceneData)) saveData.sceneData.Remove(sceneData);

		sceneData.objectsName.Add(name);

		saveData.sceneData.Add(sceneData);

		SavePlayerData();
	}

	public void LoadData()
	{
		if (!PlayerPrefs.HasKey(SaveDataKey)) CreateSaveData();

		string json = PlayerPrefs.GetString(SaveDataKey);
		saveData = JsonUtility.FromJson<SaveData>(json);

		currentHearts = saveData.currentHearts;
		hp = saveData.hp;

		foreach (SceneData sceneData in saveData.sceneData)
		{

			this.sceneData = sceneData;

			foreach (string name in sceneData.objectsName)
			{
				GameObject gameObject = GameObject.Find(name);

				//if (gameObject != null)
				//{
				//	if (gameObject.GetComponent<LockedDoor>()) gameObject.GetComponent<LockedDoor>().OpenedDoor();
				//	else if (gameObject.GetComponent<Chest>()) gameObject.GetComponent<Chest>().OpenedChest();
				//	else gameObject.SetActive(false);
				//}
			}

		}
	}

	private void CreateSaveData()
	{
		SaveData saveData = new SaveData();
		saveData.currentHearts = 3;
		saveData.hp = 3;
		saveData.coins = 5;
		saveData.sceneData = new List<SceneData>();

		string json = JsonUtility.ToJson(saveData);
		PlayerPrefs.SetString(SaveDataKey, json);
		PlayerPrefs.Save();
	}

	private void DeleteSaveData()
	{
		if (PlayerPrefs.HasKey(SaveDataKey)) PlayerPrefs.DeleteKey(SaveDataKey);
	}
}

[System.Serializable]
public class SaveData
{
	public int currentHearts;
	public int hp;
	public int coins;
	public List<SceneData> sceneData;
}

[System.Serializable]
public class SceneData
{
	public List<string> objectsName;
}