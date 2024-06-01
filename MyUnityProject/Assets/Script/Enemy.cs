using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    NavMeshAgent pathfinder;
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag ("Player").transform;
        pathfinder.baseOffset = 1.0f; // position 의 Y값이 계속 0으로 바뀌어서 추가하였습니다.

        StartCoroutine (UpdatePath ());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator UpdatePath(){
        float refreshRate = .25f;

        while(target != null){
            Vector3 targetPosition = new Vector3(target.position.x,0,target.position.z);
            pathfinder.SetDestination (targetPosition);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
