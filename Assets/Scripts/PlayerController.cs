using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public Rigidbody2D PlayerRigidBody;
    public Animator weaponAnim;

    public float moveSpeed;
    public float attackRange;
    public float attackRate;

    private float _LastAttackTime;

    private int _OwnerID;

    [HideInInspector]
    public bool me, hasCam;

    // Start is called before the first frame update
    void Start()
    {
        if(IsOwner)
        {
            me = true;
            hasCam = false;
        }
        else
        {
            me = false;
        }

        _OwnerID = Convert.ToInt32(OwnerClientId.ToString());

        Invoke("SetCamera", 0.5f);
    }

    private void SetCamera()
    {
        if(!hasCam)
        {
            MatchMaker.instance.CameraAttach(this.gameObject, _OwnerID);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;

        if(hasCam)
        {
            MatchMaker.instance.UpdateCamera(_OwnerID);
        }

        Move();

        if(Input.GetMouseButtonDown(0) && Time.time - _LastAttackTime > attackRate)
        {
            Attack();
        }

        float mouseX = (Screen.width/2) - Input.mousePosition.x;

        if(mouseX < 0)
        {
            weaponAnim.transform.parent.localScale = new Vector3(1,1,1);
        }
        else
        {
            weaponAnim.transform.parent.localScale = new Vector3(-1,1,1);
        }
    }

    void Move()
    {
        PlayerRigidBody.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * (moveSpeed);
    }

    void Attack()
    {
        _LastAttackTime = Time.time;

        Vector3 dir = (Input.mousePosition - Camera.main.ScreenToWorldPoint(transform.position)).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + dir, dir, attackRange);

        if(hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {

        }

        weaponAnim.SetTrigger("Attack");
    }
}
