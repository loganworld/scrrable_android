using System.Globalization;
using TMPro;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;

public class PlayerTimer : MonoBehaviour
{
    public float DigitsSpacing;

    private TextMeshProUGUI timerText;
    private float time;
    private int seconds;
    private int minutes;
    private int hours;

    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        
        if (!Global.isSingle)
        {
            time = Global.limitedMinutes * 60;
        }
        else
        {
            time = 0;
        }
    }

    private void Update()
    {
        if (!Global.isSingle)
        {
            if (time < 0.0001)
            {
                return;
            }
            if (Global.myTurn == GameController.data.currentPlayer)
            {
                time -= Time.deltaTime;
                if (time <= 0f)
                {
                    time = 0f;

                    GameController.data.GiveUp();

                }
            }
        }
        else
        {
            time += Time.deltaTime;
        }
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        UpdateTimerValues();
        SetTimerText();
    }

    private void UpdateTimerValues()
    {
        seconds = (int) (time % 60);
        minutes = (int) (time / 60) % 60;
        hours = (int) (time / 60 / 60) % 24;
    }

    private void SetTimerText()
    {
        timerText.text = GetFormatedTimerValue(minutes) + ':' + GetFormatedTimerValue(seconds);
        if (hours > 0)
            timerText.text = GetFormatedTimerValue(hours) + ':' + timerText.text;
    }

    private string GetFormatedTimerValue(int timeValue)
    {
        return "<mspace=" + DigitsSpacing.ToString("F2", CultureInfo.CreateSpecificCulture("en-US")) + "em>" +
               timeValue.ToString("00") + "</mspace>";
    }
}