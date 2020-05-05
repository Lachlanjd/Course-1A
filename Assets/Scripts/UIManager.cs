using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _ammoCount;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private bool _isGameOver;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]    
    public Slider _slider;

    
    // Start is called before the first frame update
    void Start()
    {
        _slider = GameObject.Find("Afterburner_Bar").GetComponent<Slider>();
        if(_slider == null)
        {
            Debug.LogError("_slider on UIManager is null");
        }
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _restartText.gameObject.SetActive(false);
        _gameOverText.gameObject.SetActive(false);
        _scoreText.text = "Score: " + 0;
        _ammoCount.text = "Ammo: " + 15;
    }        

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _livesSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();        
        }
    }

    void GameOverSequence()
    {
        _isGameOver = true;
        _restartText.gameObject.SetActive(true);

        StartCoroutine(GameOverFlickerWaitTimeRoutine());

        if(_gameManager != null)
        {
            _gameManager.GameOver();
        }
    }
    
    IEnumerator GameOverFlickerWaitTimeRoutine()
    {
        while (_isGameOver == true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.4f);
        }        
    }

    public void UpdateAmmoCount(int ammoCount)
    {
        _ammoCount.text = "Ammo: " + ammoCount;
    }

    public void AfterburnerFuel(float value)
    {
        _slider.value = value;
    }

        
}
