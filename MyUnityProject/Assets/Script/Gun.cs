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
    public int projectilesPerMag;
    public float reloadTime = .3f;

    [Header("반동")]
    public Vector2 kickMinMax = new Vector2(.05f,.2f);
    public Vector2 recoilAngleMinMax = new Vector2(3,5);
    public float recoilMoveSettleTime = 0.1f;
    public float recoilRotationSettleTime = .1f;

    [Header("효과")]
    public Transform shell;
    public Transform shellEjection;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;
    MuzzleFlash muzzleflash;
    float nextShotTime;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;
    int projectilsRemainingInMag;
    bool isReloading;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    void Start(){
        muzzleflash = GetComponent<MuzzleFlash> ();
        shotsRemainingInBurst = burstCount;
        projectilsRemainingInMag = projectilesPerMag;
    }

    void LateUpdate(){
        // 총기반동
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * -recoilAngle;

        if (!isReloading && projectilsRemainingInMag == 0){
            Reload();
        }
    }

    void Shoot(){
        if(!isReloading && Time.time > nextShotTime && projectilsRemainingInMag > 0){

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
                if (projectilsRemainingInMag == 0){
                    break;
                }
                projectilsRemainingInMag --;
                nextShotTime = Time.time + msBetweenShorts / 1000;
                Projectile newProjectile = Instantiate(projectile,projectileSpawn[i].position,projectileSpawn[i].rotation) as Projectile;
                newProjectile.Setspeed(muzzleVelocity);
            }

            Instantiate(shell,shellEjection.position, shellEjection.rotation);
                muzzleflash.Activate();
                transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
                recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
                recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

                AudioManager.instance.PlaySound(shootAudio, transform.position);
        }
    }

    public void Reload(){
        if (!isReloading && projectilsRemainingInMag != projectilesPerMag){
        StartCoroutine(AnimateReload ());
        AudioManager.instance.PlaySound(reloadAudio, transform.position);
        }
    }

    IEnumerator AnimateReload(){
        isReloading = true;
        yield return new WaitForSeconds (.2f);

        float reloadSpeed =  1f / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1){
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;


            yield return null;
        }

        isReloading = false;
        projectilsRemainingInMag = projectilesPerMag;

    }
    public void Aim(Vector3 aimPoint){
        if (!isReloading){
        Vector3 direction = (aimPoint - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction); // 자꾸 Y축이 돌아가 총이 거꾸로 발사 되어 이렇게 수정하였습니다.

    lookRotation *= Quaternion.Euler(0, 180, 0);

    transform.rotation = lookRotation;
        }
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
