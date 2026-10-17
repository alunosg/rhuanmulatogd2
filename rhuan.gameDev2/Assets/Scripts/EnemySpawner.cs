using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public int spawnAmount = 1;
    public int maxAmount = 3;
    public int totalAmount = 6;
    public float spawnInterval = 5;

    public GameObject enemyPrefab;
    public GameObject spawnFX;
    public float spawnRadius = 20;

    public TMP_Text counterText;

    private float counter = 0;
    private float currentAmount = 0;
    private float deadAmount = 0;
    private float availableAmount;

    void Start()
    {
        counterText.text = "0/" + totalAmount;
        availableAmount = totalAmount;
    }

    void Update()
    {
        counter -= Time.deltaTime;
        if(counter < 0)
        {
            counter = spawnInterval;
            if (currentAmount < maxAmount && availableAmount > 0) Spawn();
        }
    }

    void Spawn()
    {
        for(int i = 0; i < spawnAmount; i++)
        {
            if (currentAmount > maxAmount || availableAmount <= 0) break;

            Vector3 p = transform.position + Random.insideUnitSphere * spawnRadius;
            p.y = transform.position.y;

            if (NavMesh.SamplePosition(p, out NavMeshHit hit, spawnRadius * 3, NavMesh.AllAreas))
            {
                if (spawnFX) Instantiate(spawnFX, hit.position, Quaternion.identity);

                EnemyController e = Instantiate(enemyPrefab, hit.position,
                    Quaternion.identity).GetComponent<EnemyController>();
                e.spawner = this;

                availableAmount--;
                currentAmount++;
            }
            else Debug.LogError("Trying to Spawn outside NavMesh");
        }
    }

    public void OnKill()
    {
        currentAmount--;
        deadAmount++;

        if(availableAmount <= 0 && currentAmount <= 0)
        {
            counterText.text = "REGION CLEARED";
            enabled = false;
        }
        else
        {
            counterText.text = deadAmount + "/" + totalAmount;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}