using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 5f;
	private float speed2;
	Vector2 direction;

	Rigidbody2D rigidBody;
	Animator animator;
	SpriteRenderer spriteRenderer;

	bool isAttacking;

	bool invencible;
	bool uncontrollable;
	float invencibilityTime = 1.2f;

	float blinkTime = 0.1f;
	public float knockbackStrength = 4f;
	float knockbackTime = 0.35f;

	GameManager gameManager;
	Vector2 spawnPostion;

	List<BasicInteraction> basicInteractionList = new List<BasicInteraction>();

	private void Start()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		gameManager = FindObjectOfType<GameManager>();
		speed2 = speed;
		spawnPostion = new Vector2(0.3f, 1);
	}

	private void FixedUpdate()
	{
		if (!uncontrollable)
		{
			rigidBody.velocity = direction * speed;
		}
	}

	private void Update()
	{
		Inputs();
		Animations();
	}

	private void Inputs()
	{
		if (isAttacking || uncontrollable) return;

		direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (basicInteractionList != null)
			{
				Vector2 playerFacing = new Vector2(animator.GetFloat("Horizontal"), animator.GetFloat("Vertical"));
				bool interactionSuccess = false;

				foreach (BasicInteraction basicInteraction in basicInteractionList)
				{
					if (interactionSuccess) return;
					if (basicInteraction.Interact(playerFacing, transform.position))
					{
						interactionSuccess = true;
					}
				}

				if (!interactionSuccess)

				{
					Attack();
				}
			}
			else
			{
				Attack();
			}
		}
		else
		{
			speed = speed2;
		}
	}

	private void Attack()
	{
		animator.Play("Attack_Sword");
		isAttacking = true;
		speed = 1;
		AttackAnimDirection();
	}

	private void Animations()
	{
		if (isAttacking || Time.timeScale == 0) return;

		if (direction.magnitude != 0)
		{
			animator.SetFloat("Horizontal", direction.x);
			animator.SetFloat("Vertical", direction.y);
			animator.Play("Run");
		}
		else animator.Play("Idle");
	}

	private void AttackAnimDirection()
	{
		direction.x = animator.GetFloat("Horizontal");
		direction.y = animator.GetFloat("Vertical");

		if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
		{
			direction.x = 0;
		}
		else
		{
			direction.y = 0;
		}
		direction = direction.normalized;

		animator.SetFloat("Horizontal", direction.x);
		animator.SetFloat("Vertical", direction.y);

		direction = Vector2.zero;
	}

	private void EndAttack()
	{
		isAttacking = false;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("MaxHpUp"))
		{
			Destroy(collision.gameObject);
			gameManager.IncreaseMaxHP();
			DataInstance.Instance.SaveSceneData(name);
		}
		else if (collision.CompareTag("Heal") && gameManager.CanHeal())
		{
			Destroy(collision.gameObject);
			gameManager.UpdateCurrentHP(1);
		}
		else if (collision.CompareTag("Interaction"))
		{
			basicInteractionList.Add(collision.GetComponent<BasicInteraction>());
		}
		else if (collision.CompareTag("Coin"))
		{
			gameManager.UpdateCoins(1);
			Destroy(collision.gameObject);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Interaction"))
		{
			basicInteractionList.Remove(collision.GetComponent<BasicInteraction>());
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.CompareTag("Enemy") && !invencible)
		{

			gameManager.UpdateCurrentHP(-1);
			StartCoroutine(Invincibility());
			if (gameManager.hp == 0)
			{
				Respawn();
			}
			else
			{
				StartCoroutine(Knockback(collision.transform.position));
			}
		}
	}

	private void Respawn()
	{
		transform.position = spawnPostion;
		gameManager.hp = 2;
		gameManager.currentHearts = 3;
		gameManager.coins = 0;
		gameManager.UpdateCurrentHearts();
		gameManager.UpdateCoins(0);
	}

	IEnumerator Invincibility()
	{
		invencible = true;
		float auxTime = invencibilityTime;

		while (auxTime > 0)
		{
			yield return new WaitForSeconds(blinkTime);
			auxTime -= blinkTime;
			spriteRenderer.enabled = !spriteRenderer.enabled;
		}

		spriteRenderer.enabled = true;
		invencible = false;
	}

	IEnumerator Knockback(Vector3 hitPosition)
	{
		uncontrollable = true;
		direction = Vector2.zero;
		rigidBody.velocity = (transform.position - hitPosition).normalized * knockbackStrength;
		yield return new WaitForSeconds(knockbackTime);
		rigidBody.velocity = Vector3.zero;
		uncontrollable = false;
	}

}
