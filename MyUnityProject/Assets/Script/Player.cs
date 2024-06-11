using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;

    public Crosshairs crosshairs;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // 이동을 입력받는곳
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        // 바라보는 방향
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if(groundPlane.Raycast(ray,out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
           // Debug.DrawLine(ray.origin, point, Color.red);
           controller.LootAt(point);
           crosshairs.transform.position = point;
           crosshairs.DetectTargets(ray);
           if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1) {
				gunController.Aim(point);
           }
    }

        if(Input.GetMouseButton(0)){
            gunController.OnTriggerHold();
        }
        if(Input.GetMouseButtonUp(0)){
            gunController.OnTriggerRelease();
        }
    }
}

