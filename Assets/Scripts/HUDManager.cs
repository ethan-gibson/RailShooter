using System;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [SerializeField] private TMP_Text ammoCount;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateAmmoCount(string _message)
    {
        ammoCount.text = _message;
    }

    public void UpdateScoreText(int _score)
    {
        scoreText.text = _score.ToString();
    }

    public void UpdateTimeText(float _time)
    {
        timeText.text = $"{_time : F2}";
    }
}
