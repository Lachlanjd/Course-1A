using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private AudioClip _explosionSoundClip;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource.PlayClipAtPoint(_explosionSoundClip, new Vector3(0, 3, -10));

        Destroy(this.gameObject, 3.5f);
    }    
}
