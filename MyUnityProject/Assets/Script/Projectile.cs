using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    public Color trailColour;
    float speed = 10;
    float damage = 1;

    float lifetime = 3;

    void Start(){
        Destroy (gameObject, lifetime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0){
            OnHitObject(initialCollisions[0], transform.position);
        }

        GetComponent<TrailRenderer>().startColor = trailColour;
    }

    public void Setspeed(float newSpeed){ 
        speed = newSpeed;
    }
    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions (moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckCollisions(float moveDistance){
        Ray ray = new Ray(transform.position,transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide)){
            OnHitObject(hit.collider, hit.point);                    
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint){
         IDamageable damageableObject = c.GetComponent<IDamageable>();
        if(damageableObject != null){
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }
        GameObject.Destroy (gameObject);
    }
}
