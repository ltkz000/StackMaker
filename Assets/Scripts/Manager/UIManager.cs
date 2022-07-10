using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public GameObject Menu;
    public TextMeshProUGUI scoreValue;

    public void ActiveMenu()
    {
        Menu.SetActive(true);
    }

    public void DeactivateCompleteMenu()
    {
        Menu.SetActive(true);
    }

    public void SetScoreValue(int score)
    {
        scoreValue.text = score.ToString();
    }
}