using System;
using UnityEngine;
using UnityEngine.UI;

public class Tim : MonoBehaviour
{
    public Text timerText;
    private float timeRemaining = 600f;

    private void Update()
    {
        if(timeRemaining>0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerText();
        }
        else
        {
            timeRemaining = 0;
            //end game
        }
    }

    private void UpdateTimerText()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeRemaining);
        timerText.text = string.Format("{0:D2}:{1:D2}",timeSpan.Minutes,timeSpan.Seconds);
    }
}
