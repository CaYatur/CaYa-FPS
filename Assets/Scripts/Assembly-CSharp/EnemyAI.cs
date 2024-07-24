using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
	public float moveSpeed = 5f;

	public float rotationSpeed = 5f;

	public float fireRange = 20f;

	public float attackRange = 2f;

	public Transform firePoint;

	public GameObject bulletPrefab;

	public int damageAmount = 10;

	public float accuracy = 1f;

	public List<Transform> patrolPoints;

	public BotType botType;

	public float jumpForce = 5f;

	public float visionRange = 30f;

	public Difficulty difficulty = Difficulty.Normal;

	public float fireToWait = 5f;

	public AudioClip fireSound;

	private AudioSource audioSource;

	private Transform player;

	private float nextTimeToFire = 0f;

	private int currentPatrolIndex;

	private bool isPatrolling;

	private bool isTakingCover;

	private Vector3 coverPosition;

	private Animator animator;

	private Rigidbody rb;

	private Vector3 wanderPoint;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		currentPatrolIndex = 0;
		isPatrolling = true;
		isTakingCover = false;
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		SetDifficultySettings();
		wanderPoint = GetRandomWanderPoint();
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
		}
		audioSource.clip = fireSound;
	}

	private void Update()
	{
		if (player == null)
		{
			return;
		}
		float num = Vector3.Distance(base.transform.position, player.position);
		if (num <= visionRange)
		{
			switch (botType)
			{
			case BotType.Assault:
				AssaultBehavior();
				break;
			case BotType.Sniper:
				SniperBehavior();
				break;
			case BotType.Defense:
				DefenseBehavior();
				break;
			case BotType.StaticShooter:
				StaticShooterBehavior();
				break;
			}
		}
		else
		{
			StopMoving();
		}
	}

	private void StopMoving()
	{
		base.transform.Translate(Vector3.zero);
	}

	private void SetDifficultySettings()
	{
		switch (difficulty)
		{
		case Difficulty.Easy:
			moveSpeed = 3f;
			fireToWait = 7f;
			accuracy = 0.3f;
			break;
		case Difficulty.Normal:
			moveSpeed = 5f;
			fireToWait = 5f;
			accuracy = 0.6f;
			break;
		case Difficulty.Hard:
			moveSpeed = 7f;
			fireToWait = 3f;
			accuracy = 0.9f;
			break;
		case Difficulty.Custom:
			break;
		}
	}

	private void AssaultBehavior()
	{
		EngagePlayer();
	}

	private void SniperBehavior()
	{
		if (Vector3.Distance(base.transform.position, player.position) <= fireRange)
		{
			TakeCover();
			EngagePlayer();
		}
	}

	private void DefenseBehavior()
	{
		Patrol();
		DefendArea();
	}

	private void StaticShooterBehavior()
	{
		Vector3 normalized = (player.position - base.transform.position).normalized;
		Quaternion b = Quaternion.LookRotation(normalized);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * rotationSpeed);
		if (Time.time >= nextTimeToFire && Vector3.Distance(base.transform.position, player.position) <= fireRange)
		{
			nextTimeToFire = Time.time + fireToWait;
			if (Random.value < accuracy)
			{
				Shoot();
			}
		}
	}

	private void Patrol()
	{
		if (patrolPoints.Count != 0 && isPatrolling)
		{
			Transform transform = patrolPoints[currentPatrolIndex];
			Vector3 normalized = (transform.position - base.transform.position).normalized;
			Quaternion b = Quaternion.LookRotation(normalized);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * rotationSpeed);
			base.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
			if (Vector3.Distance(base.transform.position, transform.position) < 1f)
			{
				currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
			}
		}
	}

	private void EngagePlayer()
	{
		Vector3 normalized = (player.position - base.transform.position).normalized;
		Quaternion b = Quaternion.LookRotation(normalized);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * rotationSpeed);
		if (Vector3.Distance(base.transform.position, player.position) > attackRange)
		{
			MoveTowardsPlayer(normalized);
		}
		if (Time.time >= nextTimeToFire && Vector3.Distance(base.transform.position, player.position) <= fireRange)
		{
			nextTimeToFire = Time.time + fireToWait;
			if (Random.value < accuracy)
			{
				Shoot();
			}
		}
	}

	private void MoveTowardsPlayer(Vector3 direction)
	{
		if (Physics.Raycast(base.transform.position, base.transform.forward, out var hitInfo, 1f))
		{
			if (hitInfo.collider != null && hitInfo.collider.CompareTag("Obstacle"))
			{
				Jump();
			}
			else
			{
				ChangeDirection();
			}
		}
		else
		{
			base.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
		}
	}

	private void ChangeDirection()
	{
		Vector3 forward = base.transform.right + base.transform.forward;
		Quaternion b = Quaternion.LookRotation(forward);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * rotationSpeed);
	}

	private void Jump()
	{
		if (IsGrounded())
		{
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
	}

	private bool IsGrounded()
	{
		return Physics.Raycast(base.transform.position, Vector3.down, 1.1f);
	}

	private void DefendArea()
	{
		if (Vector3.Distance(base.transform.position, player.position) <= attackRange)
		{
			EngagePlayer();
		}
	}

	private void TakeCover()
	{
		if (!isTakingCover)
		{
			coverPosition = FindCoverPosition();
			isTakingCover = true;
		}
		Vector3 normalized = (coverPosition - base.transform.position).normalized;
		Quaternion b = Quaternion.LookRotation(normalized);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * rotationSpeed);
		base.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
		if (Vector3.Distance(base.transform.position, coverPosition) < 1f)
		{
			isTakingCover = false;
		}
	}

	private Vector3 FindCoverPosition()
	{
		Vector3 normalized = (base.transform.position - player.position).normalized;
		return base.transform.position + normalized * 5f;
	}

	private void Shoot()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
		if (gameObject != null)
		{
			if (fireSound != null && audioSource != null && firePoint != null)
			{
				Vector3 normalized = (Camera.main.transform.position - firePoint.position).normalized;
				audioSource.spatialBlend = 1f;
				audioSource.PlayOneShot(fireSound, 1f);
				GameObject gameObject2 = Object.Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
				Rigidbody component = gameObject2.GetComponent<Rigidbody>();
				Vector3 normalized2 = (gameObject.transform.position - firePoint.position).normalized;
				component.velocity = normalized2 * 30f;
				Object.Destroy(gameObject2, 5f);
				if (Physics.Raycast(firePoint.position, normalized2, out var hitInfo, fireRange))
				{
					if (hitInfo.transform.CompareTag("Player"))
					{
						AttackPlayer();
					}
					else
					{
						Object.Destroy(gameObject2);
					}
				}
				else
				{
					Object.Destroy(gameObject2);
				}
			}
			else
			{
				Debug.LogWarning("Null reference detected in Shoot method. Check fireSound, audioSource, and firePoint.");
			}
		}
		else
		{
			Debug.Log("Player object not found. Cannot shoot.");
		}
	}

	private void AttackPlayer()
	{
		PlayerHealth component = player.GetComponent<PlayerHealth>();
		if (component != null)
		{
			component.TakeDamage(damageAmount);
		}
	}

	private Vector3 GetRandomWanderPoint()
	{
		Vector3 sourcePosition = Random.insideUnitSphere * visionRange;
		sourcePosition += base.transform.position;
		NavMesh.SamplePosition(sourcePosition, out var hit, visionRange, -1);
		return hit.position;
	}
}
