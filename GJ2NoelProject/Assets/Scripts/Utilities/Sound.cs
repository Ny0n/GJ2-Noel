using UnityEngine;

[System.Serializable]
public class Sound {
	
	public enum TypeSound { Music, SFX }
	public enum TypeBlend { None_2D, Full_3D }

	// Parameters
	
	public string Name;									// name of the sound
	public TypeSound Type;								// type of the sound (Music or SFX)
	public TypeBlend Blend;								// type of the sound (Music or SFX)

	public AudioClip Clip;								// clip to play
	public bool Loop;									// will it be looping or not?
	[Range(0f, 1f)] public float Volume = 0.75f;		// volume for this sound
	
	// the actual source component that will play the sound,
	// every transfer parameters above will be applied to that in the audio manager
	[HideInInspector] public AudioSource Source;
	
}
