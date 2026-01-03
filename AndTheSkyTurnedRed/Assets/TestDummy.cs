using System.Collections;
using Features.DamageableModule.Scripts;
using UnityEngine;
using UnityEngine.AI;

public class TestDummy : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private MockDamageable _mockDamageable;
    [SerializeField] private float _searchRange;
    [SerializeField] private Rigidbody _rigidbody;
    private GameObject _target;
    [SerializeField] private float _impulseStrenth;
    [SerializeField] private float _impulseDuration;
    [SerializeField] private float _getUpDuration;
    private bool _isHited;

    private void OnEnable()
    {
        _mockDamageable.OnDamage += ProcessHit;
    }

    private void OnDisable()
    {
        _mockDamageable.OnDamage += ProcessHit;
    }

    private void ProcessHit(DamageData damageData)
    {
        if(_isHited)
            return;
        
        _isHited = true;
        Vector3 impulseDirection = transform.position - damageData.DamageDealer.position;
        impulseDirection.y = 0;
        StartCoroutine(Knockback(impulseDirection.normalized * _impulseStrenth, damageData.DamageDealer));
    }
    
    private IEnumerator Knockback(Vector3 force, Transform target)
    {
        _agent.enabled = false;
        _animator.SetTrigger("Hit");
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(force, ForceMode.Impulse);
        _rigidbody.transform.LookAt(target, Vector3.up);
        yield return new WaitForSeconds(_impulseDuration);
        _animator.SetTrigger("GetUp");
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.isKinematic = true;
        yield return new WaitForSeconds(_getUpDuration);
        _agent.enabled = true;
        _isHited = false;
    }

    private void Update()
    {
        if(_target && _agent.enabled)
            _agent.SetDestination(_target.transform.position);
        
        _animator.SetTrigger(_agent.velocity.sqrMagnitude > 0.1f ? "Run" : "Idle");

        if(!_target)
            _target = GetTargetsInRange(transform.position, _searchRange);
    }

    public GameObject GetTargetsInRange(Vector3 initialPosition, float range) {
        Collider[] colliders = Physics.OverlapSphere(initialPosition, range, _playerLayerMask);
        if(colliders.Length == 0)
            return null;
        
        return colliders[0].gameObject;
    }
}
