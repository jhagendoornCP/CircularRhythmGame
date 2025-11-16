using EventStreams;
using RhythmPlayer;
using TMPro;
using UnityEngine;

public class ExampleSwitchControllers : MonoBehaviour
{
    [SerializeField] TMP_Text howToPlayText;
    [SerializeField] PlayerControllerScheme1 scheme1;
    [SerializeField] PlayerControllerScheme2 scheme2;
    [SerializeField] PlayerControllerScheme3 scheme3;
    [SerializeField] PlayerControllerScheme4 scheme4;
    [SerializeField] IntEventStream startWithNumLanesStream;

    int currentLanes = 4;

    public void Restart(int withLanes = -1)
    {
        if (withLanes != -1) currentLanes = withLanes;
        startWithNumLanesStream.Invoke(currentLanes);
    }

    public void Scheme1()
    {
        howToPlayText.text = "in this control scheme, press WASD to change the direction you are facing. Left click to pop the beats, right click to 'jump' in the direction you are facing.";
        DisableAll();
        scheme1.enabled = true;
        Restart(4);
    }

    public void Scheme2()
    {
        howToPlayText.text = "In this control scheme, you don't use keyboard at all. Left click on the lane to pop the beat, right click on the lane to jump in that direction.";
        DisableAll();
        scheme2.enabled = true;
        Restart(8);
    }
    
    public void Scheme3()
    {
        howToPlayText.text = "in this control scheme, press A for the bottom left lane, W for the top left, D for the top right, and S for the bottom right. That automatically 'clicks' the lane for you. Click on a lane to jump in that direction.";
        DisableAll();
        scheme3.enabled = true;
        Restart(8);
    }

    public void Scheme4()
    {
        howToPlayText.text = "In this control scheme, use A/D to change your facing direction. Press shift to get a burst of speed. Press W to click on the current lane. Press S to add 180 degress to your direction. Press space to jump.";
        DisableAll();
        scheme4.enabled = true;
        Restart(8);
    }

    private void DisableAll()
    {
        scheme1.enabled = false;
        scheme2.enabled = false;
        scheme3.enabled = false;
        scheme4.enabled = false;
    }
}
