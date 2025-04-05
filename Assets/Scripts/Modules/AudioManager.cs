using System.Collections.Generic;
using UnityEngine;
public static class AudioManager
{
	const string MusicPath = "Music/";
	const string SFXPath = "SFX/";
	private static AudioSource musicSource;
	private static Dictionary<string, AudioSource> sfxSources = new Dictionary<string, AudioSource>();

	private static GameObject audioManagerObject;

	static AudioManager()
	{
		InitializeAudioManager();
	}

	private static void InitializeAudioManager()
	{
		audioManagerObject = new GameObject("AudioManager");
		Object.DontDestroyOnLoad(audioManagerObject);

		musicSource = audioManagerObject.AddComponent<AudioSource>();
		musicSource.loop = true;
	}

	public static void PlayMusic(string musicClipName, float volume = 1.0f)
	{
		AudioClip clip = Resources.Load<AudioClip>(MusicPath + musicClipName);
		if (clip != null)
		{
			musicSource.clip = clip;
			musicSource.volume = volume;
			musicSource.Play();
		}
		else
		{
			Debug.LogError("Music clip not found: " + musicClipName);
		}
	}

	public static void StopMusic()
	{
		musicSource.Stop();
	}

	public static void PlaySFX(string sfxClipName, float volume = 1.0f, bool loop = false)
	{
		if (!sfxSources.ContainsKey(sfxClipName))
		{
			AudioSource source1 = audioManagerObject.AddComponent<AudioSource>();
			sfxSources[sfxClipName] = source1;
		}

		AudioSource source = sfxSources[sfxClipName];
		AudioClip clip = Resources.Load<AudioClip>(SFXPath + sfxClipName);
		if (clip != null)
		{
			source.clip = clip;
			source.volume = volume;
			source.loop = loop;
			source.Play();
		}
		else
		{
			Debug.LogError("SFX clip not found: " + sfxClipName);
		}
	}

	public static void StopSFX(string sfxClipName)
	{
		if (sfxSources.ContainsKey(sfxClipName))
		{
			AudioSource source = sfxSources[sfxClipName];
			source.Stop();
		}
		else
		{
			Debug.LogWarning("SFX clip not found or not playing: " + sfxClipName);
		}
	}

	public static void StopAllSFX()
	{
		foreach (var source in sfxSources.Values)
		{
			source.Stop();
		}
	}

	public static void SetSFXLoop(string sfxClipName, bool loop)
	{
		if (sfxSources.ContainsKey(sfxClipName))
		{
			AudioSource source = sfxSources[sfxClipName];
			source.loop = loop;
		}
		else
		{
			Debug.LogWarning("SFX clip not found or not initialized: " + sfxClipName);
		}
	}
}