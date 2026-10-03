using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyStates { Idle, Patrol, Chase, Attack, Hit, Death }
    public EnemyStates state = EnemyStates.Patrol;

    public bool patrol = false;

    private float distance;
    private PlayerController player;

    public float chaseDistance = 80;
    public float attackDistance = 30;
    public float attackDuration = 3.0f;
    public float attackDelay = 0.5f;

    public float hitDuration = 0.5f;
    public float deathDuration = 3.0f;

    public Transform turret;
    public Transform cannon;
    private Transform shootTarget;

    public NavMeshAgent nav;
    public Transform bulletPoint;
    public GameObject shootFX;
    public GameObject hitFX;

    public GameObject bulletPrefab;
    public float bulletSpeed = 30;

    public SpriteRenderer hpBar;
    public float hp = 5;
    private float maxHp;

    private bool locked = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        shootTarget = player.transform.Find("Target").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (locked) return;

        distance = Vector3.Distance(player.transform.position, transform.position);

        switch (state)
        {
            case EnemyStates.Idle:
                IdleUpdate();
                break;

            case EnemyStates.Patrol:
                PatrolUpdate();
                break;

            case EnemyStates.Chase:
                ChaseUpdate();
                break;

            case EnemyStates.Attack:
                AttackUpdate();
                break;

            case EnemyStates.Hit:
                HitUpdate();
                break;

            case EnemyStates.Death:
                DeathUpdate();
                break;
        }
    }

    void IdleUpdate()
    {
        if (distance < chaseDistance)
        {
            if (distance < attackDistance)
            {
                EnterAttack();
            }
            else
            {
                state = EnemyStates.Chase;
            }
        }
    }

    void PatrolUpdate()
    {

    }

    void ChaseUpdate()
    {
        if (distance > chaseDistance)
        {
            state = EnemyStates.Idle;
            nav.isStopped = true;
        }
        else if (distance < attackDistance)
        {
            EnterAttack();
        }
        else
        {
            nav.isStopped = false;
            nav.SetDestination(player.transform.position);
        }
    }

    void AttackUpdate()
    {
        if (distance < attackDistance)
        {
            EnterAttack();
        }
        else if (distance < chaseDistance)
        {
            state = EnemyStates.Chase;
        }
        else
        {
            nav.isStopped = true;
            state = EnemyStates.Idle;
        }
    }

    void HitUpdate()
    {

    }

    void DeathUpdate()
    {

    }

    void Shoot()
    {
        if (shootFX) Instantiate(shootFX, bulletPoint.position, bulletPoint.rotation);

        LookAtTarget();

        GameObject bullet = Instantiate(bulletPrefab, bulletPoint.position, bulletPoint.rotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = bulletPoint.forward * bulletSpeed;
    }

    void Unlock() => locked = false;

    public void GetHit(float damage)
    {
        if (hp > 0)
        {
            CancelInvoke("Unlock");
            CancelInvoke("Shoot");
            locked = true;
            nav.isStopped = true;

            if (hitFX) Instantiate(hitFX, transform.position, transform.rotation);

            hp -= damage;
            hpBar.transform.localScale = hp / maxHp * Vector3.one;
            if (hp > 0)
            {
                //Leva hit
                Invoke("Unlock", hitDuration);
            }
            else
            {
                //Morre
                Destroy(gameObject, deathDuration);
            }
        }
    }

    void EnterAttack()
    {
        locked = true;
        CancelInvoke("Unlock");
        Invoke("Unlock", attackDuration);
        CancelInvoke("Shoot");
        Invoke("Shoot", attackDelay);
        state = EnemyStates.Attack;
        nav.isStopped = true;
    }

    void LookAtTarget()
    {
        Vector3 targetPosition = player.transform.position;
        targetPosition.y = transform.position.y;
        turret.LookAt(targetPosition);

        //cannon.LookAt(player.transform);
        //cannon.localEulerAngles = new Vector3(cannon.localEulerAngles.x, 0, 0);

        //bulletPoint.LookAt(player.transform);

        AimCannonAtPlayer();
    }

    void AimCannonAtPlayer()
    {
        // 1. Pegamos a posição do pivot do canhão e do jogador
        Vector3 fromPos = cannon.position;
        Vector3 toPos = shootTarget.position;

        // 2. Calculamos a distância horizontal (XZ) e a altura vertical (Y) pura entre os pivots
        Vector3 diffXZ = new Vector3(toPos.x - fromPos.x, 0, toPos.z - fromPos.z);
        float xPivot = diffXZ.magnitude;
        float yPivot = toPos.y - fromPos.y;

        // 3. Descobrimos o comprimento físico do cano (distância Z local do pivot até a ponta)
        float barrelLength = cannon.InverseTransformPoint(bulletPoint.position).z;

        // 4. CORREÇÃO MATEMÁTICA DEFINITIVA:
        // A gravidade só age na bala DEPOIS que ela sai da ponta do cano.
        // Portanto, a distância real que a parábola precisa percorrer é a distância total menos o cano.
        float x = xPivot - barrelLength;
        float y = yPivot;

        // Proteção para o caso do jogador estar literalmente colado ou "dentro" do canhão
        if (x < 0.1f) x = 0.1f;

        float v = bulletSpeed;
        float g = Physics.gravity.magnitude;

        // 5. Fórmula da Trajetória Balística Pura e Estável
        float discriminant = (v * v * v * v) - g * (g * (x * x) + 2 * y * (v * v));

        if (discriminant >= 0)
        {
            float sqrtRoot = Mathf.Sqrt(discriminant);

            // Ângulo balístico exato em radianos
            float angleRad = Mathf.Atan2((v * v) - sqrtRoot, g * x);
            float angleDeg = angleRad * Mathf.Rad2Deg;

            // Aplica a rotação de forma limpa no eixo X local do canhão
            cannon.localRotation = Quaternion.Euler(-angleDeg, 0, 0);
        }
        else
        {
            // Fora de alcance: Inclina a 45 graus para máxima distância
            cannon.localRotation = Quaternion.Euler(-45f, 0, 0);
        }
    }
}