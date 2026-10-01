using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    [SerializeField] private GameObject winUIPanel;
    [SerializeField] private StringChannelEventSO OnWin;
    [SerializeField] private TextMeshProUGUI winText;

    [SerializeField] private Button restartButton;
    private void Awake()
    {
        restartButton?.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }
    private void OnEnable()
    {
        OnWin.OnRised += OnWin_OnRised;
    }
    private void OnWin_OnRised(string winDragon)
    {
        winUIPanel.SetActive(true);
        UpdateUI(winDragon);
    }

    private void UpdateUI(string winDragon)
    {
        winText.text = winDragon + " Dragon Won.";

    }
    private void OnDisable()
    {
        OnWin.OnRised -= OnWin_OnRised;
    }
}
