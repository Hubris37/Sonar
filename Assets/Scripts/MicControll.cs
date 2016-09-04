using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/** Created by Staffan 4/9-2016 
 * Sound recording and playback
 * https://docs.unity3d.com/ScriptReference/AudioSource.html */
public class MicControll : MonoBehaviour
{
    public int recLen = 3;
    public Button rec, play;

    AudioSource aud;
    int minFreq, maxFreq;

    void Start()
    {
        aud = GetComponent<AudioSource>();

        //List all available microphones 
        foreach (string device in Microphone.devices)
            Debug.Log("Name: " + device);

            //Check frequency capabilities of device.
            //deviceName = "" is the default microphone
            Microphone.GetDeviceCaps("", out minFreq, out maxFreq);
        Debug.Log("Min: " + minFreq);
        Debug.Log("Max: " + maxFreq);
    }

    /** Used in Button "On Click" function */
    public void RecordSound()
    {
        //Check if default microphone is recording
        if (Microphone.IsRecording(""))
            return;
        aud.clip = Microphone.Start("", false, recLen, maxFreq);
    }

    /** Used in Button "On Click" function */
    public void PlaySound()
    {
        if (Microphone.IsRecording(""))
            return;
        aud.Play();
    }

    void Update()
    {
        if (Microphone.IsRecording(""))
            rec.GetComponent<Image>().color = Color.red;
        else
            rec.GetComponent<Image>().color = Color.green;

        //Sound spectrum
        //https://docs.unity3d.com/ScriptReference/AudioSource.GetSpectrumData.html
        //http://forum.unity3d.com/threads/what-is-spectrum-data-audio-getspectrumdata.204060/
        float[] spectrum = new float[64];

        //Get spectrum data from recording
        aud.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);

        //Only shown in editor
        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        }
    }
}


