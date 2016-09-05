# Sonar

# Intressanta länkar
  * http://www.shaderslab.com/index.php?post/Echolocation-effect
  * https://support.unity3d.com/hc/en-us/articles/206485253-How-do-I-get-Unity-to-playback-a-Microphone-input-in-real-time-
  * http://forum.unity3d.com/threads/blow-detection-using-ios-microphone.118215/#post-801969
  * http://answers.unity3d.com/questions/157940/getoutputdata-and-getspectrumdata-they-represent-t.html
  * https://docs.unity3d.com/Manual/SL-DepthTextures.html

# Basic shader tutorials
  * https://www.youtube.com/watch?v=T-HXmQAMhG0
  * https://www.youtube.com/watch?v=Tjl8jP5Nuvc


# Hur Unity använder sin mic

```javascript
//detect the default microphone
audio.clip = Microphone.Start(selectedDevice, true, 10, 44100);

//loop the playing of the recording so it will be realtime
audio.loop = true;

//if you only need the data stream values  check Mute, if you want to hear yourself ingame don't check Mute. 
audio.mute = Mute;

//don't do anything until the microphone started up
while (!(Microphone.GetPosition(selectedDevice) > 0)){

}

//Put the clip on play so the data stream gets ingame on realtime
audio.Play();

//apply the mic input data stream to a float;
function Update () {
	
	//set timer for refreshing memory.
	mTimer += Time.deltaTime;
	
	//refresh the memory
	if (micSelected == true){
		if (mTimer >= mRefTime) {
			StopMicrophone();
			
			StartMicrophone();
			
			mTimer = 0;
		}
	}
	
	if(Microphone.IsRecording(selectedDevice)){
	
		loudness = GetDataStream()*sensitivity*(sourceVolume/10);
	
		if(debug){
			Debug.Log(loudness);
		}
	}
	
	//the source volume
	if (sourceVolume > 100){
		sourceVolume = 100;
	}
	
	if (sourceVolume < 0){
		sourceVolume = 0;
	}
	audio.volume = (sourceVolume/100);
}
```

