using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject chicken;

    int actualEnemies;
    int maxEnemies;

    int actualChicken;
    int maxChicken;

    void Start()
    {
        actualEnemies = 9;
        maxEnemies = 10;

        actualChicken = 1;
        maxChicken = 15;

        InvokeRepeating("SpawnEnemy",0,10);
        InvokeRepeating("SpawnChicken", 0, 5);
    }

    private void SpawnEnemy()
    {
        if (actualEnemies < maxEnemies)
        {
            Instantiate(enemy, transform.position, Quaternion.identity);
            actualEnemies++;
        }
    }

    private void SpawnChicken()
    {
        if (actualChicken < maxChicken)
        {
            Instantiate(chicken, transform.position, Quaternion.identity);
            actualChicken++;
        }
    }

    public void SubstractEnemy()
    {
        actualEnemies--;
    }

    public void SubstractChicken()
    {
        actualChicken--;
    }
}
