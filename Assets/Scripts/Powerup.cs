using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3;
    [SerializeField] //0 = Triple Shot 1 = Speed 2 = Shields
    private int _powerUpID;
    [SerializeField]
    private AudioClip _powerupSoundClip;
    
    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -8f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(_powerupSoundClip, new Vector3(0, 3, -8));
            Player player = collision.GetComponent<Player>();

            if(player != null)
            {                
                switch (_powerUpID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.HealthBoost();
                        break;
                    case 4:
                        player.AmmoRefill();
                        break;
                    case 5:                        
                        player.SecondaryFireActive();                                               
                        break;                        
                    default:
                        Debug.Log("Default value");
                        break;
                }
            }

            Destroy(this.gameObject);
        }
    }
}
