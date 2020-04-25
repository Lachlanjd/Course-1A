using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;

    private Player _player;
    private Animator _anim;
    private Collider2D _collider;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _explosionSoundClip;
    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private GameObject _laserPrefab;
    private Vector3 _offset = new Vector3(0, -0.6f, 0);
    private float _nextFire = -1.0f;
    private float _fireRate = 3.0f;
    

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("_player in Enemy.cs is null");
        }
        _anim = this.GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("_anim in Enemy.cs is null");
        }
        _collider = this.GetComponent<Collider2D>();
        if(_collider == null)
        {
            Debug.LogError("collider on enemy.cs is null");
        }
        _audioSource = this.GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("_audioSource on enemy.cs is null");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _nextFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _nextFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);            
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].IsEnemyLaser();
            }
            _audioSource.clip = _laserSoundClip;
            _audioSource.Play();
        }
        
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -10.8f)
        {
            var randomX = Random.Range(-9f, 9f);
            transform.position = new Vector3(randomX, 9.6f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player player = collision.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
            
            _anim.SetTrigger("OnEnemyDeath");
            this._collider.enabled = false;
            _audioSource.clip = _explosionSoundClip;
            _audioSource.Play();
            _speed = 2f;
            Destroy(this.gameObject, 2.6f);
        }
        else if (collision.tag == "Laser")
        {
            Destroy(collision.gameObject);
            
            if(_player != null)
            {
                _player.AddScore(10);                  
            }
            
            _anim.SetTrigger("OnEnemyDeath");
            this._collider.enabled = false;
            _audioSource.clip = _explosionSoundClip;
            _audioSource.Play();
            _speed = 2.0f;
            Destroy(this.gameObject, 2.6f);
        }
    }    
}
