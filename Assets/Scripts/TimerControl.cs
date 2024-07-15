using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerControl : MonoBehaviour
{

    [SerializeField] private GameObject _mainEngine;
    [SerializeField] private string _nameOfTimeLength;
    private Image timerImage;
    private int maxTime;
    private float currentTime;
    
    void Start()
    {
        maxTime = _mainEngine.GetComponent<GameManager>().GetTimeLength(_nameOfTimeLength);
        timerImage = GetComponent<Image>();
        currentTime = 0;
    }

    void Update()
    {
        bool needToUpdate;
        bool isPaused;

        if (_nameOfTimeLength == "peasant")
            needToUpdate = _mainEngine.GetComponent<GameManager>().GetPeasantHireStatus();
        else if (_nameOfTimeLength == "warior")
            needToUpdate = _mainEngine.GetComponent<GameManager>().GetWariorHireStatus();
        else
            needToUpdate = _mainEngine.GetComponent<GameManager>().GetGameStatus();

        isPaused = _mainEngine.GetComponent<GameManager>().GetPausedStatus();

        if (needToUpdate)
        {
            if (!isPaused)
                currentTime += Time.deltaTime;
            if (currentTime > maxTime)
                currentTime = 0;

            timerImage.fillAmount = currentTime / maxTime;
        }
        else
        {
            timerImage.fillAmount = 0;
            currentTime = 0;
        }
        //Debug.Log(needToUpdate);
    }
}
