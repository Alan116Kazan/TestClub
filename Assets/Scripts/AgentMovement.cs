using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class AgentMovement : MonoBehaviour
{
    public Transform[] danceFloorTargets; // ���� �� ��������
    public Transform[] barTargets; // ���� � ����
    public Transform[] loungeTargets; // ���� � ������

    private NavMeshAgent agent;
    //private Animator animator;
    private Transform currentTarget;
    private bool isWaiting = false;

    private static List<Transform> occupiedTargets = new List<Transform>(); // ������ ������� �����

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
        GoToNextTarget();
    }

    void Update()
    {
        if (!isWaiting && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            StartCoroutine(WaitAtTarget());
        }
    }

    private void GoToNextTarget()
    {
        int randomZone = Random.Range(0, 3); // 0 - �������, 1 - ���, 2 - ������
        Transform[] targetArray;

        switch (randomZone)
        {
            case 0:
                targetArray = danceFloorTargets;
                break;
            case 1:
                targetArray = barTargets;
                break;
            case 2:
                targetArray = loungeTargets;
                break;
            default:
                targetArray = danceFloorTargets;
                break;
        }

        currentTarget = GetFreeTarget(targetArray);
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
            occupiedTargets.Add(currentTarget); // ��������� ������� ����� � �������
        }
        else
        {
            GoToNextTarget(); // ���� ��� ��������� �����, ������� �����
        }
    }

    private Transform GetFreeTarget(Transform[] targets)
    {
        List<Transform> freeTargets = new List<Transform>();

        foreach (var target in targets)
        {
            if (!occupiedTargets.Contains(target)) // �������� �� ���������
            {
                freeTargets.Add(target);
            }
        }

        if (freeTargets.Count > 0)
        {
            return freeTargets[Random.Range(0, freeTargets.Count)]; // ���������� ��������� ��������� �����
        }
        return null; // ���� ��� ����� ������
    }

    private IEnumerator WaitAtTarget()
    {
        isWaiting = true;
        float waitTime;

        // ���������� �������� � �������� � ����������� �� ����
        if (System.Array.Exists(danceFloorTargets, target => target == currentTarget))
        {
            waitTime = Random.Range(30f, 60f);
            //animator.SetBool("IsDancing", true); // �������� �������� ��� ��������
        }
        else if (System.Array.Exists(barTargets, target => target == currentTarget))
        {
            waitTime = Random.Range(5f, 15f);
            //animator.SetBool("IsDrinking", true); // �������� �������� ��� ����
        }
        else if (System.Array.Exists(loungeTargets, target => target == currentTarget))
        {
            waitTime = Random.Range(15f, 30f);
            //animator.SetBool("IsRelaxing", true); // �������� �������� ��� �������
        }
        else
        {
            waitTime = 10f; // �������� �� ���������
        }

        //Debug.Log(waitTime);

        yield return new WaitForSeconds(waitTime);

        // ���������� ��� �������� ����� ��������
        //animator.SetBool("IsDancing", false);
        //animator.SetBool("IsDrinking", false);
        //animator.SetBool("IsRelaxing", false);

        occupiedTargets.Remove(currentTarget); // ������� ������� ����� �� �������
        GoToNextTarget(); // ������� � ��������� ���� ����� ��������
        isWaiting = false;
    }
}