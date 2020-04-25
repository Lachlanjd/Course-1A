using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemy;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    
    private bool _stopSpawning = false;

    private float _enemyWaitTime = 2f;
       

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {           
            Vector3 enemyStartPos = new Vector3(Random.Range(-9f, 9f), 9.6f, 0);
            GameObject newEnemy = Instantiate(_enemy, enemyStartPos, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemyWaitTime);
        }        
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while(_stopSpawning ==false)
        {
            Vector3 powerUpStartPos = new Vector3(Random.Range(-9f, 9f), 9.6f, 0);
            int randomPowerup = Random.Range(0, 3);
            Instantiate(_powerups[randomPowerup], powerUpStartPos, Quaternion.identity);            
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;        
    }

    public void OnRestartLevel()
    {
        _stopSpawning = false;        
    }
}
