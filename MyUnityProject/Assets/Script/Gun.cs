using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public enum FireMode {Auto, Burst, Single};
    public FireMode fireMode;
    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShorts = 100;
    public float muzzleVelocity = 35;
    public int burstCount;

    public Transform shell;
    public Transform shellEjection;
    MuzzleFlash muzzleflash;
    float nextShotTime;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;

    void Start(){
        muzzleflash = GetComponent<MuzzleFlash> ();
        shotsRemainingInBurst = burstCount;
    }

    void Shoot(){
        if(Time.time > nextShotTime){

            if (fireMode == FireMode.Burst){
                if (shotsRemainingInBurst == 0){
                    return;
                }
                shotsRemainingInBurst --;
            }
            else if (fireMode == FireMode.Single){
                if (!triggerReleasedSinceLastShot) {
                    return;
                } 
            }

            for (int i = 0; i< projectileSpawn.Length; i++){
                nextShotTime = Time.time + msBetweenShorts / 1000;
                Projectile newProjectile = Instantiate(projectile,projectileSpawn[i].position,projectileSpawn[i].rotation) as Projectile;
                newProjectile.Setspeed(muzzleVelocity);
            }

            Instantiate(shell,shellEjection.position, shellEjection.rotation);
                muzzleflash.Activate();
        }
    }
    public void Aim(Vector3 aimPoint){
        Vector3 direction = (aimPoint - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction); // 자꾸 Y축이 돌아가 총이 거꾸로 발사 되어 이렇게 수정하였습니다.

    lookRotation *= Quaternion.Euler(0, 180, 0);

    transform.rotation = lookRotation;
    }
    public void OnTriggerHold() {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease() {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}
