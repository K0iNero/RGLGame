using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDialog : MonoBehaviour
{
    public string text = "";
    GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			gameManager.ShowText(text);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			gameManager.HideText();
		}
	}
}
