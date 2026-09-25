using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	public static AudioManager me;

	public AudioMixer mixer;

	public AudioClip BattleLoop;

	public AudioClip SpiritLoop;

	private AudioSource ambienceSource;

	public List<AudioClip> CancelSadness;

	public List<AudioClip> Click;

	public List<AudioClip> SpiritTransitionExit;

	public List<AudioClip> SpiritTransitionEnter;

	public List<AudioClip> CitiesTransitionEnter;

	public List<AudioClip> CitiesTransitionExit;

	public List<AudioClip> Eat;

	public List<AudioClip> GetSick;

	public List<AudioClip> BecomeTeenager;

	public List<AudioClip> BecomeAdult;

	public List<AudioClip> BecomeOld;

	public List<AudioClip> ConsumeHappiness;

	public List<AudioClip> Unhappy;

	public List<AudioClip> Angry;

	public AudioMixerGroup SfxGroup;

	public AudioMixerGroup MusicGroup;

	public List<AudioClip> DropOnStack;

	public List<AudioClip> CardPickup;

	public List<AudioClip> CardDrop;

	public List<AudioClip> CardDestroy;

	public AudioClip QuestComplete;

	public List<AudioClip> MediumPickup;

	public List<AudioClip> HeavyPickup;

	public AudioClip EndOfMoon;

	public List<AudioClip> CardCreate;

	public List<AudioClip> OpenBooster;

	public List<AudioClip> Coin;

	public List<AudioClip> Dollar;

	public List<AudioClip> AnimalMove;

	public List<AudioClip> Miss;

	public List<AudioClip> Block;

	public List<AudioClip> HitMelee;

	public List<AudioClip> HitRanged;

	public List<AudioClip> HitMagic;

	public List<AudioClip> HitFoot;

	public List<AudioClip> HitArmour;

	public List<AudioClip> HitAir;

	public List<AudioClip> MissCities;

	public List<AudioClip> Buff;

	public List<AudioClip> MagicRelease;

	public List<AudioClip> MagicCharge;

	public List<AudioClip> RangedRelease;

	public List<AudioClip> Crit;

	public List<AudioClip> ShamanSpawn;

	public List<AudioClip> Bleed;

	public List<AudioClip> Poison;

	[Header("Cities")]
	public AudioClip EnergyConnected;

	public AudioClip EnergyStrech;

	public AudioClip EnergyStart;

	public AudioClip SewerConnected;

	public AudioClip SewerStrech;

	public AudioClip SewerStart;

	public AudioClip TransportConnected;

	public AudioClip TransportStrech;

	public AudioClip TransportStart;

	public AudioClip AddWellbeing;

	public AudioClip LostWellbeing;

	public AudioClip WellbeingCounter;

	public AudioClip PowerOutage;

	public AudioClip LandmarkBuild;

	public AudioClip ExtinguishCardSound;

	public AudioClip RepairCardSound;

	public AudioClip DamagedCardSound;

	public AudioClip OnFireCardSound;

	public AudioClip DroughtStart;

	public AudioClip DroughtSolved;

	public AudioClip IndustrialRevolutionCreate;

	public AudioClip PositiveEventSpawn;

	public AudioClip NegativeEventSpawn;

	public AudioClip ColliderRunningSound;

	public AudioClip AdvisorAppears;

	public AudioClip AdvisorWarning;

	public AudioClip AdvisorTalking;

	public AudioClip ClearPollution;

	public AudioClip SpawnPollution;

	public AudioClip LandfillOverflow;

	private AudioSource songSource;

	private AudioSource battleSource;

	private AudioSource spiritSource;

	private List<AudioClip> playedSongs = new List<AudioClip>();

	private float currentSongTargetVolume;

	private float newSongTimer;

	private bool muteSong;

	private List<AudioSource> sources = new List<AudioSource>();

	private List<Transform> targets = new List<Transform>();

	private void Awake()
	{
		me = this;
		InitializeAudioSources();
	}

	private void Start()
	{
		ambienceSource = GetSource(null, claim: true);
		ambienceSource.spatialBlend = 0f;
		ambienceSource.clip = DetermineCurrentAmbience();
		ambienceSource.volume = DetermineCurrentAmbienceVolume();
		ambienceSource.reverbZoneMix = 0f;
		ambienceSource.outputAudioMixerGroup = MusicGroup;
		ambienceSource.loop = true;
		ambienceSource.Play();
		songSource = GetSource(null, claim: true);
		songSource.spatialBlend = 0f;
		songSource.reverbZoneMix = 0f;
		songSource.outputAudioMixerGroup = MusicGroup;
		songSource.volume = 0f;
		songSource.time = 0f;
		AudioClipWithVolume audioClipWithVolume = DetermineCurrentSong();
		songSource.clip = audioClipWithVolume.Clip;
		currentSongTargetVolume = audioClipWithVolume.Volume;
		songSource.Play();
		battleSource = GetSource(null, claim: true);
		battleSource.spatialBlend = 0f;
		battleSource.reverbZoneMix = 0f;
		battleSource.outputAudioMixerGroup = SfxGroup;
		battleSource.volume = 0f;
		battleSource.clip = BattleLoop;
		battleSource.loop = true;
		battleSource.Play();
		spiritSource = GetSource(null, claim: true);
		spiritSource.spatialBlend = 0f;
		spiritSource.reverbZoneMix = 0f;
		spiritSource.outputAudioMixerGroup = SfxGroup;
		spiritSource.volume = 0f;
		spiritSource.clip = SpiritLoop;
		spiritSource.loop = true;
		spiritSource.Play();
	}

	public void SkipSong()
	{
		songSource.time = songSource.clip.length - 5.1f;
	}

	private AudioClipWithVolume DetermineCurrentSong()
	{
		GameBoard curBoard = WorldManager.instance.GetCurrentBoardSafe();
		if (curBoard.BoardOptions.Songs.Count == 1)
		{
			return curBoard.BoardOptions.Songs.FirstOrDefault();
		}
		List<AudioClipWithVolume> list = curBoard.BoardOptions.Songs.FindAll((AudioClipWithVolume x) => !playedSongs.Contains(x.Clip));
		if (list.Count < 1)
		{
			playedSongs.RemoveAll((AudioClip x) => curBoard.BoardOptions.Songs.Any((AudioClipWithVolume v) => (Object)(object)v.Clip == (Object)(object)x) && (Object)(object)x != (Object)(object)songSource.clip);
			list = curBoard.BoardOptions.Songs.FindAll((AudioClipWithVolume x) => !playedSongs.Contains(x.Clip));
		}
		AudioClipWithVolume audioClipWithVolume = ((list.Count > 0) ? list.Choose() : null);
		if (audioClipWithVolume != null && !playedSongs.Contains(audioClipWithVolume.Clip))
		{
			playedSongs.Add(audioClipWithVolume.Clip);
		}
		return audioClipWithVolume;
	}

	private AudioClip DetermineCurrentAmbience()
	{
		return WorldManager.instance.GetCurrentBoardSafe().BoardOptions.Ambience;
	}

	private float DetermineCurrentAmbienceVolume()
	{
		return WorldManager.instance.GetCurrentBoardSafe().BoardOptions.AmbienceVolume;
	}

	private bool CurrentSongShouldStop()
	{
		AudioClip currentSong = songSource.clip;
		if ((Object)(object)WorldManager.instance.CurrentBoard != (Object)null && !WorldManager.instance.CurrentBoard.BoardOptions.Songs.Any((AudioClipWithVolume x) => (Object)(object)x.Clip == (Object)(object)currentSong))
		{
			return true;
		}
		return false;
	}

	private bool AnyCardInConflict()
	{
		foreach (GameCard allCard in WorldManager.instance.AllCards)
		{
			if (allCard.MyBoard.IsCurrent && (Object)(object)allCard.Combatable != (Object)null && allCard.InConflict)
			{
				return true;
			}
		}
		return false;
	}

	private bool SpiritOnBoard()
	{
		return (Object)(object)WorldManager.instance.GetCard<Spirit>() != (Object)null;
	}

	private void LateUpdate()
	{
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		MusicGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log(OptionsScreen.MusicVol) * 20f);
		SfxGroup.audioMixer.SetFloat("SfxVolume", Mathf.Log(OptionsScreen.SfxVol) * 20f);
		float num = currentSongTargetVolume;
		if ((Object)(object)songSource.clip != (Object)null)
		{
			if (songSource.time >= songSource.clip.length - 5f || CurrentSongShouldStop())
			{
				num = 0f;
				newSongTimer += Time.deltaTime;
			}
			if (WorldManager.instance.GetCardCount<Spirit>() > 0)
			{
				num = 0f;
			}
		}
		if (WorldManager.instance.CurrentGameState == WorldManager.GameState.GameOver)
		{
			num = 0f;
		}
		if (muteSong)
		{
			num = 0f;
		}
		if (!songSource.isPlaying)
		{
			newSongTimer += Time.deltaTime;
		}
		battleSource.volume = Mathf.Lerp(battleSource.volume, AnyCardInConflict() ? 0.01f : 0f, Time.unscaledDeltaTime);
		spiritSource.volume = Mathf.Lerp(spiritSource.volume, SpiritOnBoard() ? 0.5f : 0f, Time.unscaledDeltaTime);
		float num2 = 1f;
		if (WorldManager.instance.SpeedUp == 0f)
		{
			num2 = 0.8f;
		}
		else if (WorldManager.instance.SpeedUp == 5f)
		{
			num2 = 1.2f;
		}
		songSource.pitch = Mathf.Lerp(songSource.pitch, num2, Time.deltaTime * 12f);
		if (newSongTimer >= 4f)
		{
			newSongTimer = 0f;
			AudioClipWithVolume audioClipWithVolume = DetermineCurrentSong();
			songSource.clip = audioClipWithVolume.Clip;
			currentSongTargetVolume = audioClipWithVolume.Volume;
			songSource.time = 0f;
			songSource.Play();
		}
		songSource.volume = Mathf.Lerp(songSource.volume, num, Time.unscaledDeltaTime);
		UpdateAmbience();
		for (int i = 0; i < sources.Count; i++)
		{
			if ((Object)(object)targets[i] != (Object)null)
			{
				((Component)sources[i]).transform.position = ((Component)targets[i]).transform.position;
			}
		}
	}

	private void UpdateAmbience()
	{
		AudioClip val = DetermineCurrentAmbience();
		if ((Object)(object)ambienceSource.clip != (Object)(object)val)
		{
			ambienceSource.volume = Mathf.Lerp(ambienceSource.volume, 0f, Time.unscaledDeltaTime);
			if (ambienceSource.volume < 0.001f)
			{
				ambienceSource.clip = val;
				ambienceSource.Play();
			}
		}
		else
		{
			ambienceSource.volume = Mathf.Lerp(ambienceSource.volume, DetermineCurrentAmbienceVolume(), Time.unscaledDeltaTime);
		}
	}

	private void InitializeAudioSources()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 100; i++)
		{
			GameObject val = new GameObject("Audio " + i);
			val.transform.SetParent(((Component)this).transform);
			val.transform.localPosition = Vector3.zero;
			val.transform.localRotation = Quaternion.identity;
			val.transform.localScale = Vector3.zero;
			AudioSource val2 = val.AddComponent<AudioSource>();
			val2.outputAudioMixerGroup = SfxGroup;
			val2.loop = false;
			val2.playOnAwake = false;
			val2.spatialBlend = 0f;
			val2.minDistance = 20f;
			val2.priority = 10;
			sources.Add(val2);
			targets.Add(null);
		}
	}

	public AudioSource GetSource(Transform target = null, bool claim = false)
	{
		AudioSource val = null;
		for (int i = 0; i < sources.Count; i++)
		{
			if (!sources[i].isPlaying)
			{
				val = sources[i];
				targets[i] = target;
				break;
			}
		}
		if (claim)
		{
			sources.Remove(val);
		}
		return val;
	}

	public void PlaySound(List<AudioClip> clips, Vector3 pos, float pitch = 1f, float vol = 1f)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		PlaySound(clips[Random.Range(0, clips.Count)], pos, pitch, vol);
	}

	public void PlaySound(AudioClip clip, Vector3 pos, float pitch = 1f, float vol = 1f)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		AudioSource source = GetSource();
		if (!((Object)(object)source == (Object)null))
		{
			source.pitch = pitch;
			source.clip = clip;
			source.volume = vol;
			((Component)source).transform.position = pos;
			source.spatialBlend = 1f;
			source.reverbZoneMix = 1f;
			source.bypassListenerEffects = false;
			source.Play();
		}
	}

	public void PlaySound(AudioClip clip, Transform t, float pitch = 1f, float vol = 1f)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		AudioSource source = GetSource(t);
		if (!((Object)(object)source == (Object)null))
		{
			source.pitch = pitch;
			source.clip = clip;
			source.volume = vol;
			source.reverbZoneMix = 0f;
			((Component)source).transform.localPosition = Vector3.zero;
			source.spatialBlend = 1f;
			source.bypassListenerEffects = false;
			source.Play();
		}
	}

	public void PlaySound2D(List<AudioClip> clips, float pitch = 1f, float vol = 1f)
	{
		PlaySound2D(clips[Random.Range(0, clips.Count)], pitch, vol);
	}

	public void PlaySound2D(AudioClip clip, float pitch, float vol)
	{
		AudioSource source = GetSource();
		if (!((Object)(object)source == (Object)null))
		{
			source.pitch = pitch;
			source.clip = clip;
			source.volume = vol;
			source.reverbZoneMix = 0f;
			source.spatialBlend = 0f;
			source.bypassListenerEffects = false;
			source.Play();
		}
	}

	public AudioSource PlayLoop2D(AudioClip clip, float pitch, float vol)
	{
		AudioSource source = GetSource();
		if ((Object)(object)source == (Object)null)
		{
			return null;
		}
		source.pitch = pitch;
		source.clip = clip;
		source.volume = vol;
		source.reverbZoneMix = 0f;
		source.spatialBlend = 0f;
		source.bypassListenerEffects = false;
		source.loop = true;
		source.Play();
		return source;
	}

	public void ReleaseLoopedSource(AudioSource source)
	{
		if ((Object)(object)source != (Object)null)
		{
			source.loop = false;
			source.Stop();
		}
	}

	public List<AudioClip> GetSoundForPickupSoundGroup(PickupSoundGroup group)
	{
		return group switch
		{
			PickupSoundGroup.Medium => MediumPickup, 
			PickupSoundGroup.Heavy => HeavyPickup, 
			_ => CardPickup, 
		};
	}
}
