using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State{Idle, Chasing, Attacking};
    State currentState;

    public ParticleSystem deathEffect;
    public static event System.Action OnDeathStatic;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;

    Color originalColour;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1;
    float damage = 1;
    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    void Awake(){
                pathfinder = GetComponent<NavMeshAgent>();


        if (GameObject.FindGameObjectWithTag ("Player") != null){
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag ("Player").transform;
            targetEntity = target.GetComponent<LivingEntity> ();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;

        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();


        if (hasTarget){
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetDeath;
            pathfinder.baseOffset = 1.0f; // position 의 Y값이 계속 0으로 바뀌어서 추가하였습니다.

            StartCoroutine (UpdatePath ());
        }
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor){
        pathfinder.speed = moveSpeed;
        
        if (hasTarget) {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;

        var mainModule = deathEffect.main;
        mainModule.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);
        skinMaterial = GetComponent<Renderer> ().material;
        skinMaterial.color = skinColor;
        originalColour = skinMaterial.color;
    }

    public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection)
	{
        AudioManager.instance.PlaySound("Impact", transform.position);
		if (damage >= health) {
            if (OnDeathStatic != null){
                OnDeathStatic();
            }
            AudioManager.instance.PlaySound("Enemy Death ", transform.position);
            var main = deathEffect.main; // 원래 없던 코드이지만 주의 경고문이 발생하여 deathEffect.startLifetime이라 써야하지만 main.startLifetime.constant라고 수정하였습니다.
			Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, main.startLifetime.constant);
		}
		base.TakeHit (damage, hitPoint, hitDirection);
	}

    void OnTargetDeath() {
        hasTarget = false;
        currentState = State.Idle;
    }
 void Update() {
    if (hasTarget) {
        if (target == null) {
            hasTarget = false;
            return; // Player 객체가 파괴되었을때 오류가 발생하여 수정하였습니다.
        }

        if (Time.time > nextAttackTime) {
            float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
            if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2)) {
                nextAttackTime = Time.time + timeBetweenAttacks;
                AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                StartCoroutine(Attack());
            }
        }
    }
}
    
    IEnumerator Attack(){

        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while (percent <= 1 ){
            
            if(percent >= 0.5f && !hasAppliedDamage) {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
        
            yield return null;
        }
        skinMaterial.color = originalColour;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath(){
        float refreshRate = .25f;

        while(hasTarget){
            if(currentState == State.Chasing){
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
            if (!dead){
                 pathfinder.SetDestination (targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
