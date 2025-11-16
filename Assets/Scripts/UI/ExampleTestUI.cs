using EventStreams;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExampleTestUI : MonoBehaviour
{
    [Header("Streams")]
    [SerializeField] IntEventStream scoreStream;
    [SerializeField] IntEventStream comboStream;
    [SerializeField] IntEventStream maxComboStream;
    [SerializeField] IntEventStream multiplierStream;
    [SerializeField] FloatEventStream multiplierProgressStream;
    [SerializeField] BasicEventStream playerDeathStream;
    [SerializeField] BasicEventStream levelStartsStream;

    [Header("UI components")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text comboText;
    [SerializeField] TMP_Text maxComboText;
    [SerializeField] TMP_Text multiplierText;
    [SerializeField] Image multiplierProgressBar;
    [SerializeField] TMP_Text deathText;
    [SerializeField] GameObject restartButton;
    void OnEnable()
    {
        scoreStream.Sub(UpdateScore);
        comboStream.Sub(UpdateCombo);
        maxComboStream.Sub(UpdateMaxCombo);
        multiplierStream.Sub(UpdateMultiplier);
        multiplierProgressStream.Sub(UpdateMultProgress);
        playerDeathStream.Sub(ShowLostText);
        levelStartsStream.Sub(HideLostText);
    }

    void OnDisable()
    {
        scoreStream.Unsub(UpdateScore);
        comboStream.Unsub(UpdateCombo);
        maxComboStream.Unsub(UpdateMaxCombo);
        multiplierStream.Unsub(UpdateMultiplier);
        multiplierProgressStream.Unsub(UpdateMultProgress);
        playerDeathStream.Unsub(ShowLostText);
        levelStartsStream.Unsub(HideLostText);
    }

    void UpdateScore(int to)
    {
        scoreText.text = "score: " + to;
    }

    void UpdateCombo(int to)
    {
        comboText.text = "combo: " + to;
    }

    void UpdateMaxCombo(int to)
    {
        maxComboText.text = "max combo: " + to;
    }

    void UpdateMultiplier(int to)
    {
        multiplierText.text = "x" + to;
    }

    void UpdateMultProgress(float to)
    {
        multiplierProgressBar.fillAmount = to;
    }

    void ShowLostText()
    {
        deathText.gameObject.SetActive(true);
        restartButton.SetActive(true);
    }
    
    void HideLostText()
    {
        deathText.gameObject.SetActive(false);
        restartButton.SetActive(false);
    }
}
