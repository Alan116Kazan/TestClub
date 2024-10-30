using UnityEngine;
using UnityEngine.AI;

public class SecurityMovement : MonoBehaviour
{
    [SerializeField] private GameObject[] _visitors; // ������ �����������

    private NavMeshAgent _security; // ��������� NavMeshAgent
    private GameObject _currentVisitor; // ������� ����������

    private void Start()
    {
        _security = GetComponent<NavMeshAgent>();
        ChooseNextVisitor();
    }

    private void Update()
    {
        // ���� ���� ������� ����������, ������� �� ���
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
        // ���� �������� ������ �������� ����������, �������� ����������
        if (_security.remainingDistance <= _security.stoppingDistance && !_security.pathPending)
        {
            ChooseNextVisitor();
        }
    }

    private void ChooseNextVisitor()
    {
        if (_visitors.Length == 0) return;

        // �������� ���������� ����������
        _currentVisitor = _visitors[Random.Range(0, _visitors.Length)];
    }
}