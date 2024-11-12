using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _InputReader;
    [SerializeField] private Rigidbody2D _PlayerRigidBody;
    [SerializeField] private Animator _WeaponAnim;

    [Header("Settings")]
    [SerializeField] private float _MoveSpeed, _AttackRange, _AttackRate;

    private float _LastAttackTime;

    private int _OwnerID;

    [HideInInspector] public bool me, hasCam;
    [HideInInspector] public Camera myCam;

    private Vector2 _PreviousMovementInput;

    private bool _ShouldAttack;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        me = true;
        hasCam = false;

        myCam = FindObjectOfType<Camera>();

        _OwnerID = Convert.ToInt32(OwnerClientId.ToString());

        _InputReader.MoveEvent += MoveHandle;
        _InputReader.PrimaryFireEvent += AttackHandle;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }

        _InputReader.MoveEvent -= MoveHandle;
        _InputReader.PrimaryFireEvent -= AttackHandle;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        float mouseX = /*(Screen.width / 2) - */ Input.mousePosition.x;

        if (myCam != null)
        {
            float playerPosX = myCam.WorldToScreenPoint(this.transform.position).x;

            if (mouseX > playerPosX)
            {
                _WeaponAnim.transform.parent.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                _WeaponAnim.transform.parent.localScale = new Vector3(-1, 1, 1);
            }
        }
        else
        {
            myCam = FindObjectOfType<Camera>();
        }

        if (_LastAttackTime > _AttackRate && _ShouldAttack)
        {
            _LastAttackTime = 0;

            _WeaponAnim.SetTrigger("Attack");
        }

        if (_LastAttackTime < _AttackRate)
        {
            _LastAttackTime += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        Move();
    }

    private void Move()
    {
        _PlayerRigidBody.velocity = new Vector2(_PreviousMovementInput.x, _PreviousMovementInput.y).normalized * (_MoveSpeed);
    }

    private void MoveHandle(Vector2 movementInput)
    {
        _PreviousMovementInput = movementInput;
    }

    private void AttackHandle(bool attack)
    {
        _ShouldAttack = attack;
    }
}
