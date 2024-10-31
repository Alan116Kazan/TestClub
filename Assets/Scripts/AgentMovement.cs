using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class AgentMovement : MonoBehaviour
{
    public Transform[] danceFloorTargets; // Цели на танцполе
    public Transform[] barTargets; // Цели в баре
    public Transform[] loungeTargets; // Цели в лаунже

    private NavMeshAgent agent;
    private Animator animator;
    private Transform currentTarget;
    private bool isWaiting = false;

    private static List<Transform> occupiedTargets = new List<Transform>(); // Список занятых целей

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
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
        int randomZone = Random.Range(0, 3); // 0 - танцпол, 1 - бар, 2 - лаундж
        Debug.Log(randomZone);
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
            occupiedTargets.Add(currentTarget); // Добавляем целевую точку в занятые
            animator.SetBool("IsWalking", true); // Включаем анимацию ходьбы
        }
        else
        {
            GoToNextTarget(); // Если нет свободной точки, пробуем снова
        }
    }

    private Transform GetFreeTarget(Transform[] targets)
    {
        List<Transform> freeTargets = new List<Transform>();

        foreach (var target in targets)
        {
            if (!occupiedTargets.Contains(target)) // Проверка на занятость
            {
                freeTargets.Add(target);
            }
        }

        if (freeTargets.Count > 0)
        {
            return freeTargets[Random.Range(0, freeTargets.Count)]; // Возвращаем случайную свободную точку
        }
        return null; // Если все точки заняты
    }

    private IEnumerator WaitAtTarget()
    {
        isWaiting = true;
        animator.SetBool("IsWalking", false); // Отключаем анимацию ходьбы
        float waitTime;

        // Определяем задержку и анимацию в зависимости от зоны
        if (System.Array.Exists(danceFloorTargets, target => target == currentTarget))
        {
            waitTime = Random.Range(25f, 40f);
            animator.SetBool("IsDancing", true); // Включаем анимацию для танцпола
        }
        else if (System.Array.Exists(barTargets, target => target == currentTarget))
        {
            waitTime = Random.Range(5f, 10f);
            animator.SetBool("IsDrinking", true); // Включаем анимацию для бара
        }
        else if (System.Array.Exists(loungeTargets, target => target == currentTarget))
        {
            waitTime = Random.Range(15f, 30f);
            animator.SetBool("IsRelaxing", true); // Включаем анимацию для лаунджа
            StartCoroutine(SetIsSittingAfterDelay(1f)); // Запускаем корутину с задержкой 1 секунда
        }
        else
        {
            waitTime = 10f; // Задержка по умолчанию
        }

        yield return new WaitForSeconds(waitTime);

        // Сбрасываем все анимации после ожидания
        animator.SetBool("IsDancing", false);
        animator.SetBool("IsDrinking", false);
        animator.SetBool("IsRelaxing", false);
        animator.SetBool("IsSitting", false);

        occupiedTargets.Remove(currentTarget); // Убираем целевую точку из занятых
        GoToNextTarget(); // Переход к следующей цели после задержки
        isWaiting = false;
    }

    // Вынесенная корутина для установки IsSitting в true через заданную задержку
    private IEnumerator SetIsSittingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Ждем указанную задержку
        animator.SetBool("IsSitting", true); // Устанавливаем IsSitting в true
    }
}