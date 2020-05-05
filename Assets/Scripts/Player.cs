using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    private float _speedBoostMultiplier = 1.5f;
    private float _afterburner = 1.2f;
    private int _shieldStrength;
    private int _ammoCount = 15;
    
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShot;
    [SerializeField]
    private GameObject _missilePrefab;
    [SerializeField]
    private GameObject _shield;
    [SerializeField]
    private GameObject[] _playerDamage;
    [SerializeField]
    private GameObject _explosionPrefab;

    private Vector3 offset = new Vector3(0, 1f, 0);
    [SerializeField]
    private float _fireRate = 0.2f;
    private float _nextFire = 0.0f;
    [SerializeField]
    private int _health;
    [SerializeField]
    private int _score;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private CameraController _camera;
    [SerializeField]
    private AudioClip _laserSoundClip;    
    [SerializeField]
    private AudioClip _powerUpCollectionSoundClip;
    [SerializeField]
    private AudioClip _missileSoundClip;
    private AudioSource _audioSource;
    [SerializeField]
    private Vector3 startPos = new Vector3(0, -4.5f, 0);
    [SerializeField]
    private float _afterBurnerFuel;
    
    private bool _firePowerupActive = false;
    private bool _speedBoostActive = false;
    private bool _shieldPowerupActive = false;
    private bool _secondaryFireActive = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _afterBurnerFuel = 20f;
        _health = 3;
        transform.position = new Vector3(0,-4.5f,0);      
        _spawnManager = GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = this.GetComponent<AudioSource>();
        _camera = GameObject.Find("Main Camera").GetComponent<CameraController>();
        if(_camera == null)
        {
            Debug.LogError("camera on Player script is null");
        }
        if(_audioSource == null)
        {
            Debug.LogError("_audiosource on player.cs is null");
        }
        
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
            if(_secondaryFireActive == true)
            {
                HomingMissile();
            }
            else if (_ammoCount > 0)
            {
                FireLaser();
            }            
        }    
    }

    void FireLaser()
    {
        _ammoCount--;
        _uiManager.UpdateAmmoCount(_ammoCount);

        _audioSource.clip = _laserSoundClip;
        _nextFire = Time.time + _fireRate;

        if (_firePowerupActive == true)
        {
            Instantiate(_tripleShot, transform.localPosition, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.localPosition + offset, Quaternion.identity);
        }
        _audioSource.Play();
    }

    void HomingMissile()
    {
        Vector3 StartPos = transform.localPosition + offset;
        _audioSource.clip = _missileSoundClip;
        _nextFire = Time.time + _fireRate;
        Instantiate(_missilePrefab, StartPos, Quaternion.identity);
        _audioSource.Play();        
    }

    void CalculateMovement()
    {        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (_speedBoostActive == false)
        {
            if (Input.GetKey(KeyCode.LeftShift) && _afterBurnerFuel > 1.2)
            {
                _afterBurnerFuel -= 6f * Time.deltaTime;
                transform.Translate(direction * _speed * _afterburner * Time.deltaTime);
                _uiManager.AfterburnerFuel(_afterBurnerFuel);
            }
            if (_afterBurnerFuel < 20 && !Input.GetKey(KeyCode.LeftShift))
            {
                _afterBurnerFuel += 4f * Time.deltaTime;
                _uiManager.AfterburnerFuel(_afterBurnerFuel);
            }
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        else
        {            
            transform.Translate(direction * _speed * _speedBoostMultiplier * Time.deltaTime);            
        }        
                
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4.5f, 4.2f), 0);

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
            _shieldStrength--;
            switch (_shieldStrength)
            {
                case 2:
                    _shield.GetComponent<SpriteRenderer>().material.color = Color.grey;
                    break;
                case 1:
                    _shield.GetComponent<SpriteRenderer>().material.color = Color.green;
                    break;
            }

            if(_shieldStrength == 0)
            {
                _shieldPowerupActive = false;
                _shield.SetActive(false);
            }
            
            return;
        }
        else
        {
            StartCoroutine(_camera.Shake());
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
        _speedBoostActive = true;
        StartCoroutine("SpeedBoostPowerDownRoutine");
    }
    
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(8.0f);
        _speedBoostActive = false;
    }

    public void SecondaryFireActive()
    {
        _secondaryFireActive = true;        
        StartCoroutine("SecondaryFireCoolDownRoutine");
    }

    IEnumerator SecondaryFireCoolDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _secondaryFireActive = false;
    }

    public void ShieldActive()
    {
        _shieldStrength = 3;
        
        _shieldPowerupActive = true;
        _shield.SetActive(true);
        _shield.GetComponent<SpriteRenderer>().material.color = Color.white;
    }

    public void HealthBoost()
    {
        if (_health < 3)
        {
            _health++;
            if (_playerDamage[0].activeSelf)
            {
                _playerDamage[0].SetActive(false);
            }
            else if (_playerDamage[1].activeSelf)
            {
                _playerDamage[1].SetActive(false);
            }
        }
    }

    public void AmmoRefill()
    {
        _ammoCount += 10;
        _uiManager.UpdateAmmoCount(_ammoCount);
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
