using UnityEngine;
using UnityEngine.AI;

public class SecurityMovement : MonoBehaviour
{
    [SerializeField] private GameObject[] _visitors; // Массив посетителей

    private NavMeshAgent _security; // Компонент NavMeshAgent
    private GameObject _currentVisitor; // Текущий посетитель

    private void Start()
    {
        _security = GetComponent<NavMeshAgent>();
        ChooseNextVisitor();
    }

    private void Update()
    {
        // Если есть текущий посетитель, следуем за ним
        if (_currentVisitor != null)
        {
            MoveToCurrentVisitor();
            CheckArrivalAtVisitor();
        }
    }

    private void MoveToCurrentVisitor()
    {
        _security.SetDestination(_currentVisitor.transform.position);
    }

    private void CheckArrivalAtVisitor()
    {
        // Если охранник достиг текущего посетителя, выбираем следующего
        if (_security.remainingDistance <= _security.stoppingDistance && !_security.pathPending)
        {
            ChooseNextVisitor();
        }
    }

    private void ChooseNextVisitor()
    {
        if (_visitors.Length == 0) return;

        // Выбираем случайного посетителя
        _currentVisitor = _visitors[Random.Range(0, _visitors.Length)];
    }
}