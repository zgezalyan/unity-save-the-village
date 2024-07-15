using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioSource _buttonClickSound;
    [SerializeField] private AudioSource _bgMusic;
    [SerializeField] private AudioSource _foodSound;
    [SerializeField] private AudioSource _harvestSound;
    [SerializeField] private AudioSource _enemySound;
    [SerializeField] private AudioSource _peasantHireSound;
    [SerializeField] private AudioSource _wariorHireSound;
    [SerializeField] private AudioSource _victorySound;
    [SerializeField] private AudioSource _defeatSound;
    [SerializeField] private Sprite _musicOnSprite;
    [SerializeField] private Sprite _musicOffSprite;
    [SerializeField] private Sprite _soundOnSprite;
    [SerializeField] private Sprite _soundOffSprite;
    [SerializeField] private Sprite _pauseOnSprite;
    [SerializeField] private Sprite _pauseOffSprite;
    [SerializeField] private Button _musicButton;
    [SerializeField] private Button _soundButton;
    [SerializeField] private GameObject _pauseButton;
    [SerializeField] private GameObject _pauseImage;
    [SerializeField] private GameObject _harvestTimerImage;
    [SerializeField] private GameObject _foodTimerImage;
    [SerializeField] private GameObject _enemiesTimerImage;
    [SerializeField] private GameObject _foodTimerText;
    [SerializeField] private GameObject _harvestTimerText;
    [SerializeField] private GameObject _enemiesTimerText;
    [SerializeField] private GameObject _peasantHireButton;
    [SerializeField] private GameObject _wariorHireButton;
    [SerializeField] private GameObject _wariorTimerImage;
    [SerializeField] private GameObject _peasantTimerImage;
    [SerializeField] private GameObject _foodAmountText;
    [SerializeField] private GameObject _peasantAmountText;
    [SerializeField] private GameObject _wariorAmountText;
    [SerializeField] private GameObject _enemiesAmountText;
    [SerializeField] private GameObject _waveToWinText;
    [SerializeField] private GameObject _foodToWinText;
    [SerializeField] private GameObject _peasantToWinText;
    [SerializeField] private GameObject _startGameButton;
    [SerializeField] private GameObject _gameNameText;

    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Text _statusText;
    [SerializeField] private Text _statusReasonText;
    [SerializeField] private Text _wavesSurvivedText;
    [SerializeField] private Text _foodGainedText;
    [SerializeField] private Text _foodEatenText;
    [SerializeField] private Text _peasantsHiredText;
    [SerializeField] private Text _wariorsHiredText;
    [SerializeField] private Text _wariorsDiedText;
    [SerializeField] private Text _enemiesKilledText;

    [SerializeField] private int _foodCycleLength;
    [SerializeField] private int _harvestCycleLength;
    [SerializeField] private int _enemiesCycleLength;
    [SerializeField] private int _peasantHireTimeLength;
    [SerializeField] private int _wariorHireTimeLength;
    [SerializeField] private int _foodAmountStartValue;
    [SerializeField] private int _peasantAmountStartValue;
    [SerializeField] private int _wariorAmountStartValue;
    [SerializeField] private int _enemiesAmountStartValue;
    [SerializeField] private int _wavesToWinStartValue;
    [SerializeField] private int _foodToWinStartValue;
    [SerializeField] private int _peasantToWinStartValue;
    [SerializeField] private int _wavesWithZeroEnemies;
    [SerializeField] private int _addedEnemiesNumber;
    [SerializeField] private int _peasantPrice;
    [SerializeField] private int _wariorPrice;

    private bool isMusicOn;
    private bool isSoundOn;
    private bool isGameOn;
    private bool isPaused;
    private bool isPeasantHireOn;
    private bool isWariorHireOn;

    private int foodAmount;
    private int peasantAmount;
    private int wariorAmount;
    private int enemiesAmount;
    private int waveCount;
    private int wavesToWin;
    private int foodToWin;
    private int peasantToWin;

    private int foodGained;
    private int foodEaten;
    private int wariorsHired;
    private int wariorsDied;
    private int enemiesKilled;
    
    private float currentFoodTime;
    private float currentHarvestTime;
    private float currentEnemiesTime;
    private float currentPeasantHireTime;
    private float currentWariorHireTime;

    void Start()
    {
        isMusicOn = true;
        isSoundOn = true;
        isGameOn = false;
        isPaused = false;

        ShowHideMenuInterface(true);
        ShowHideGameInterface(false);
        _gameOverPanel.SetActive(false);
        _pauseImage.SetActive(false);
    }

    void Update()
    {
        if (isGameOn && !isPaused)
        {
            currentFoodTime += Time.deltaTime;
            currentHarvestTime += Time.deltaTime;
            currentEnemiesTime += Time.deltaTime;
            
            // Processing of food cycle
            if (currentFoodTime > _foodCycleLength)
            {                
                currentFoodTime = 0;
                foodAmount -= wariorAmount;
                foodEaten += wariorAmount;
                if (foodAmount < 0)
                    GameOverHandle(false, true);
                foodToWin = _foodToWinStartValue - foodAmount >= 0 ? _foodToWinStartValue - foodAmount : 0;
                PlaySoundOnCondition(_foodSound, isSoundOn && isGameOn);
                CheckHirePossibility();
            }

            //Processing of harvest cycle
            if (currentHarvestTime > _harvestCycleLength)
            {               
                currentHarvestTime = 0;
                foodAmount += peasantAmount;
                foodGained += peasantAmount;
                foodToWin = _foodToWinStartValue - foodAmount >= 0 ? _foodToWinStartValue - foodAmount : 0;
                PlaySoundOnCondition(_harvestSound, isSoundOn);
                CheckHirePossibility();
            }

            //Processing of enemies attack cycle
            if (currentEnemiesTime > _enemiesCycleLength)
            {                
                currentEnemiesTime = 0;
                wariorsDied = enemiesAmount <= wariorAmount ? wariorsDied + enemiesAmount : wariorsDied + wariorAmount;
                enemiesKilled = enemiesAmount <= wariorAmount ? enemiesKilled + enemiesAmount : enemiesKilled + wariorAmount;
                wariorAmount -= enemiesAmount;                
                if (wariorAmount < 0)
                    GameOverHandle(false, false);
                waveCount += 1;
                if (waveCount >= _wavesWithZeroEnemies)
                    enemiesAmount += _addedEnemiesNumber;
                wavesToWin = _wavesToWinStartValue - waveCount >= 0 ? _wavesToWinStartValue - waveCount : 0;
                PlaySoundOnCondition(_enemySound, isSoundOn && isGameOn);
            }

            //Processing of peasant hiring
            if (isPeasantHireOn)
            {
                currentPeasantHireTime += Time.deltaTime;

                if (currentPeasantHireTime > _peasantHireTimeLength)
                {
                    isPeasantHireOn = false;
                    currentPeasantHireTime = 0;
                    peasantAmount += 1;
                    peasantToWin = _peasantToWinStartValue - peasantAmount >= 0 ? _peasantToWinStartValue - peasantAmount : 0;
                    CheckHirePossibility();
                    PlaySoundOnCondition(_peasantHireSound, isSoundOn);
                }
            }

            //Processing of warior hiring
            if (isWariorHireOn)
            {
                currentWariorHireTime += Time.deltaTime;

                if (currentWariorHireTime > _wariorHireTimeLength)
                {
                    isWariorHireOn = false;
                    currentWariorHireTime = 0;
                    wariorAmount += 1;
                    wariorsHired += 1;
                    CheckHirePossibility();
                    PlaySoundOnCondition(_wariorHireSound, isSoundOn);
                }
            }

            //Updating values on screen and checking for game over
            ShowUpdatedValues();
            if (foodToWin == 0 && wavesToWin == 0 && peasantToWin == 0)
                GameOverHandle(true, false);
        }
    }

    public void OnPlayButtonClick()
    {
        PlaySoundOnCondition(_buttonClickSound, isSoundOn);

        ShowHideMenuInterface(false);
        ShowHideGameInterface(true);
        _gameOverPanel.SetActive(false);

        currentFoodTime = 0;
        currentHarvestTime = 0;
        currentEnemiesTime = 0;
        currentPeasantHireTime = 0;
        currentWariorHireTime = 0;
        foodAmount = _foodAmountStartValue;
        peasantAmount = _peasantAmountStartValue;
        wariorAmount = _wariorAmountStartValue;
        enemiesAmount = _enemiesAmountStartValue;
        waveCount = 0;
        wavesToWin = _wavesToWinStartValue;
        foodToWin = _foodToWinStartValue - foodAmount;
        peasantToWin = _peasantToWinStartValue - peasantAmount;
        isPeasantHireOn = false;
        isWariorHireOn = false;

        foodGained = 0;
        foodEaten = 0;
        wariorsHired = wariorAmount;
        wariorsDied = 0;
        enemiesKilled = 0;

        ShowUpdatedValues();
        CheckHirePossibility();

        isGameOn = true;
    }

    public void OnMusicButtonClick()
    {
        if (isMusicOn)
        {
            isMusicOn = false;
            _musicButton.GetComponent<Image>().sprite = _musicOffSprite;
            _bgMusic.Stop();
        }
        else
        {
            isMusicOn = true;
            _musicButton.GetComponent<Image>().sprite = _musicOnSprite;
            _bgMusic.Play();
        }
    }

    public void OnSoundButtonClick()
    {
        if (isSoundOn)
        {
            isSoundOn = false;
            _soundButton.GetComponent<Image>().sprite = _soundOffSprite;
        }
        else
        {
            isSoundOn = true;
            _soundButton.GetComponent<Image>().sprite = _soundOnSprite;
        }
    }

    public void OnPauseButtonClick()
    {
        PlaySoundOnCondition(_buttonClickSound, isSoundOn);
        if (isPaused)
        {
            isPaused = false;
            _pauseImage.SetActive(false);
            _pauseButton.GetComponent<Image>().sprite = _pauseOffSprite;
        }
        else
        {
            isPaused = true;
            _pauseImage.SetActive(true);
            _pauseButton.GetComponent<Image>().sprite = _pauseOnSprite;
        }
    }

    public void PlaySoundOnCondition(AudioSource soundToPlay, bool condition)
    {
        if (condition)
            soundToPlay.Play();
    }

    public void ShowHideGameInterface(bool setActive)
    {
        //_harvestTimerImage.SetActive(setActive);
        //_foodTimerImage.SetActive(setActive);
        //_enemiesTimerImage.SetActive(setActive);
        _foodTimerText.SetActive(setActive);
        _harvestTimerText.SetActive(setActive);
        _enemiesTimerText.SetActive(setActive);
        _peasantHireButton.SetActive(setActive);
        _wariorHireButton.SetActive(setActive);
        //_wariorTimerImage.SetActive(setActive);
        //_peasantTimerImage.SetActive(setActive);
        _foodAmountText.SetActive(setActive);
        _peasantAmountText.SetActive(setActive);
        _wariorAmountText.SetActive(setActive);
        _enemiesAmountText.SetActive(setActive);
        _waveToWinText.SetActive(setActive);
        _foodToWinText.SetActive(setActive);
        _peasantToWinText.SetActive(setActive);
        _pauseButton.SetActive(setActive);        
    }

    public void ShowHideMenuInterface(bool setActive)
    {
        _gameNameText.SetActive(setActive);
        _startGameButton.SetActive(setActive);
    }

    public void ShowUpdatedValues()
    {
        _foodAmountText.GetComponent<Text>().text = "Количество еды: " + foodAmount.ToString();
        _peasantAmountText.GetComponent<Text>().text = "Количество крестьян: " + peasantAmount.ToString();
        _wariorAmountText.GetComponent<Text>().text = "Количество воинов: " + wariorAmount.ToString();
        _enemiesAmountText.GetComponent<Text>().text = "Врагов в следующем набеге: " + enemiesAmount.ToString();
        _waveToWinText.GetComponent<Text>().text = "Набегов до победы: " + wavesToWin.ToString();
        _foodToWinText.GetComponent<Text>().text = "Еды до победы: " + foodToWin.ToString();
        _peasantToWinText.GetComponent<Text>().text = "Крестьян до победы: " + peasantToWin.ToString();
    }

    public int GetTimeLength(string typeOfTime)
    {
        if (typeOfTime == "food")
            return _foodCycleLength;
        if (typeOfTime == "harvest")
            return _harvestCycleLength;
        if (typeOfTime == "enemies")
            return _enemiesCycleLength;
        if (typeOfTime == "peasant")
            return _peasantHireTimeLength;
        if (typeOfTime == "warior")
            return _wariorHireTimeLength;
        return 0;
    }

    public bool GetGameStatus()
    {
        return isGameOn;
    }

    public bool GetPausedStatus()
    {
        return isPaused;
    }

    public bool GetPeasantHireStatus()
    {
        return isPeasantHireOn;
    }

    public bool GetWariorHireStatus()
    {
        return isWariorHireOn;
    }

    public void OnPeasantHireButtonClick()
    {
        PlaySoundOnCondition(_buttonClickSound, isSoundOn);

        isPeasantHireOn = true;
        foodAmount -= _peasantPrice;
        foodEaten += _peasantPrice;
        foodToWin = _foodToWinStartValue - foodAmount >= 0 ? _foodToWinStartValue - foodAmount : 0;

        CheckHirePossibility();
    }

    public void OnWariorHireButtonClick()
    {
        PlaySoundOnCondition(_buttonClickSound, isSoundOn);

        isWariorHireOn = true;
        foodAmount -= _wariorPrice;
        foodEaten += _wariorPrice;
        foodToWin = _foodToWinStartValue - foodAmount >= 0 ? _foodToWinStartValue - foodAmount : 0;

        CheckHirePossibility();
    }

    public void CheckHirePossibility()
    {
        if (foodAmount < _peasantPrice || isPeasantHireOn)
            _peasantHireButton.GetComponent<Button>().interactable = false;
        else
            _peasantHireButton.GetComponent<Button>().interactable = true;
        if (foodAmount < _wariorPrice || isWariorHireOn)
            _wariorHireButton.GetComponent<Button>().interactable = false;
        else
            _wariorHireButton.GetComponent<Button>().interactable = true;
    }

    public void GameOverHandle(bool isWin, bool isZeroFood)
    {
        isGameOn = false; 
        isPeasantHireOn = false;
        isWariorHireOn = false;
        ShowHideGameInterface(false);
        _gameOverPanel.SetActive(true);
        if (isWin)
        {
            PlaySoundOnCondition(_victorySound, isSoundOn);
            _statusText.text = "Победа";
            _statusReasonText.text = "Деревня процветает!";
        }
        else 
        {
            PlaySoundOnCondition(_defeatSound, isSoundOn);
            _statusText.text = "Поражение";
            if (isZeroFood)
                _statusReasonText.text = "В деревне кончилась еда";
            else
                _statusReasonText.text = "Деревню разграбили враги";
        }

        _wavesSurvivedText.text = "Набегов отражено: " + waveCount.ToString();
        _foodGainedText.text = "Еды собрано: " + foodGained.ToString();
        _foodEatenText.text = "Еды съедено: " + foodEaten.ToString();
        _peasantsHiredText.text = "Крестьян нанято: " + peasantAmount.ToString();
        _wariorsHiredText.text = "Воинов нанято: " + wariorsHired.ToString();
        _wariorsDiedText.text = "Воинов погибло: " + wariorsDied.ToString();
        _enemiesKilledText.text = "Врагов убито: " + enemiesKilled.ToString();
    }
}
