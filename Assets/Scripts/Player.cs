using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    private float _speedBoostMultiplier = 2;
    
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShot;
    [SerializeField]
    private GameObject _shield;
    [SerializeField]
    private GameObject[] _playerDamage;
    [SerializeField]
    private GameObject _explosionPrefab;

    private Vector3 offset = new Vector3(0, 0.86f, 0);
    [SerializeField]
    private float _fireRate = 0.2f;
    private float _nextFire = 0.0f;
    [SerializeField]
    private int _health;
    [SerializeField]
    private int _score;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    [SerializeField]
    private AudioClip _laserSoundClip;    
    [SerializeField]
    private AudioClip _powerUpCollectionSoundClip;
    private AudioSource _audioSource;
    
    private Vector3 startPos = new Vector3(0, 0, 0);
    
    private bool _firePowerupActive = false;
    private bool _speedBoostActive = false;
    private bool _shieldPowerupActive = false;
    
    // Start is called before the first frame update
    void Start()
    {        
        _health = 3;
        transform.position = startPos;      
        _spawnManager = GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = this.GetComponent<AudioSource>();
        if(_audioSource == null)
        {
            Debug.LogError("_audiosource on player.cs is null");
        }
        /*else
        {
            _audioSource.clip = _laserSoundClip;
        }*/

        if(_uiManager == null)
        {
            Debug.Log("UIManager not found!");
        }       
    }

    // Update is called once per frame
    void Update()
    {        
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            FireLaser();
        }    
    }

    void FireLaser()
    {
        _audioSource.clip = _laserSoundClip;
        _nextFire = Time.time + _fireRate;

        if (_firePowerupActive == true)
        {            
            Instantiate(_tripleShot, transform.position, Quaternion.identity);            
        }
        else
        {           
            Instantiate(_laserPrefab, transform.position + offset, Quaternion.identity);
        }
        _audioSource.Play();
    }

    void CalculateMovement()
    {        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (_speedBoostActive == false)
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        else
        {            
            transform.Translate(direction * _speed * _speedBoostMultiplier * Time.deltaTime);            
        }        
                
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -1.4f, 6.5f), 0);

        if (transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }         
    }    

    public void Damage()
    {
        if (_shieldPowerupActive == true)
        {
            _shieldPowerupActive = false;
            _shield.SetActive(false);
            return;
        }
        else
        {
            _health--;
            _uiManager.UpdateLives(_health);

            if (_health > 1)
            {                
                _playerDamage[Random.Range(0, 2)].SetActive(true);                               
            }

            else if (_health == 1)
            {
                if (_playerDamage[0].activeSelf)
                {
                    _playerDamage[1].SetActive(true);
                }
                else
                {
                    _playerDamage[0].SetActive(true);
                }
            }

            else if (_health < 1)
            {                
                GameObject _explosionClone = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                
                this.gameObject.SetActive(false);

                Destroy(this.gameObject, 3.0f);                

                if (_spawnManager != null)
                {
                    _spawnManager.OnPlayerDeath();
                }
            }
        }        
    }
        
    public void TripleShotActive()
    {
        //_audioSource.clip = _powerUpCollectionSoundClip;
        //_audioSource.Play();

        _firePowerupActive = true;
        StartCoroutine("TripleShotPowerDownRoutine");        
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _firePowerupActive = false;
    }

    public void SpeedBoostActive()
    {
        //_audioSource.clip = _powerUpCollectionSoundClip;
        //_audioSource.Play();

        _speedBoostActive = true;
        StartCoroutine("SpeedBoostPowerDownRoutine");
    }
    
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(8.0f);
        _speedBoostActive = false;
    }

    public void ShieldActive()
    {
        //_audioSource.clip = _powerUpCollectionSoundClip;
        //_audioSource.Play();

        _shieldPowerupActive = true;
        _shield.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy_Laser")
        {
            Damage();
            Destroy(other.gameObject);
        }
    }
}
