using UnityEngine;

public class AudioMeasure : MonoBehaviour {
	
	public float RmsValue;
	public float DbValue;
	public float PitchValue;

	private const int QSamples = 1024;
	private const float RefValue = 0.1f;
	private const float Threshold = 0.02f;

	float[] _samples;
	private float[] _spectrum;
	private float _fSample;

	AudioSource aud;
	int minFreq, maxFreq;

	void Start() {
		_samples = new float[QSamples];
		_spectrum = new float[QSamples];
		_fSample = AudioSettings.outputSampleRate;

		aud = GetComponent<AudioSource>();

		if(Microphone.devices.Length <= 0) {  
            Debug.LogWarning("Microphone not connected!");  
        }

		//Check frequency capabilities of device.
		//deviceName = "" is the default microphone
		Microphone.GetDeviceCaps("", out minFreq, out maxFreq);
		Debug.Log("Min: " + minFreq);
		Debug.Log("Max: " + maxFreq);

		// According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency
		if(minFreq == 0 && maxFreq == 0) {
			maxFreq = 44100;
		}

		aud.clip = Microphone.Start("", true, 10, maxFreq);
		aud.loop = true;

		//don't do anything until the microphone started up
		while (!(Microphone.GetPosition("") > 0)) {

		}

		aud.Play();
	}

	void Update() {
		AnalyzeSound();
	}

	void OnApplicationFocus( bool focusStatus )
	{
        // To not run before Start()
        if (maxFreq > 0) {
            if (focusStatus == false) {
                Microphone.End("");
                aud.Stop();
            }
            else {
                aud.clip = Microphone.Start("", true, 10, maxFreq);
                aud.Play();
            }
        }
    }

	void OnApplicationPause( bool pauseStatus )
	{
		// To not run before Start()
		if(maxFreq > 0) {
            if (pauseStatus == false) {
                Microphone.End("");
                aud.Stop();
            }
            else {
                aud.clip = Microphone.Start("", true, 10, maxFreq);
                aud.Play();
            }
		}
	}

	void AnalyzeSound() {
		aud.GetOutputData(_samples, 0); // fill array with samples
		int i;
		float sum = 0;
		for (i = 0; i < QSamples; i++) {
			sum += _samples[i] * _samples[i]; // sum squared samples
		}

		RmsValue = Mathf.Sqrt(sum / QSamples); // rms = square root of average

		DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
		if (DbValue < -160) DbValue = -160; // clamp it to -160dB min

		// get sound spectrum
		aud.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);

		float maxV = 0;
		var maxN = 0;

		for (i = 0; i < QSamples; i++) { // find max 
			if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
				continue;

			maxV = _spectrum[i];
			maxN = i; // maxN is the index of max
		}

		float freqN = maxN; // pass the index to a float variable

		if (maxN > 0 && maxN < QSamples - 1) { // interpolate index using neighbours
			var dL = _spectrum[maxN - 1] / _spectrum[maxN];
			var dR = _spectrum[maxN + 1] / _spectrum[maxN];
			freqN += 0.5f * (dR * dR - dL * dL);
		}

		PitchValue = freqN * (_fSample / 2) / QSamples; // convert index to frequency
	}
}
