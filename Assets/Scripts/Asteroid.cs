using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 18.5f;
    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;
    


    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
        {
            Debug.LogError("_spawnManager on Asteroid.cs is null");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward *_rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {        
        if (other.tag == "Laser")
        {            
            GameObject _explosionClone = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            
            Destroy(other.gameObject);

            _spawnManager.StartSpawning();
            this.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(this.gameObject, 2.0f);
        }

        
    }
}
