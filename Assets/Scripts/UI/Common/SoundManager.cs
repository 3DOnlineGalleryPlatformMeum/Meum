using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���带 �����ϴ� Ŭ����
/// </summary>
public class SoundManager : MonoBehaviour
{

	private static SoundManager instance;
	public static SoundManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<SoundManager>();

				if (instance == null)
				{
					instance = new GameObject("SoundManager").AddComponent<SoundManager>();
					GameObject.DontDestroyOnLoad(instance.gameObject);
				}
			}
			return instance;
		}
	}

	private AudioSource bgmAudio;
	private List<AudioSource> seAudioList = new List<AudioSource>();

	public float bgmValue = 0;
	public float seValue = 0;

	BGMEnum currentPlaybgmEnum = BGMEnum.None;

	MeumSaveData meumSaveData = null;

	void Awake()
	{
		Init();
	}

	public void Start()
	{
		//PlayBGM("Main");
	}

	/// <summary>
	/// �ʱ�ȭ
	/// </summary>
	private void Init()
	{
		bgmValue = PlayerPrefs.GetFloat("bgmValue", 0.5f);
		BgmVolumeReset();

		seValue = PlayerPrefs.GetFloat("seValue", 0.5f);
		SeVolumeReset();

		meumSaveData = Resources.Load<MeumSaveData>("MeumSaveData");
	}

	/// <summary>
	/// BGM�� ��°� ����
	/// </summary>
	/// <param name="value"></param>
	public void BgmValueReset(float value)
	{
		bgmValue = value;
		PlayerPrefs.SetFloat("bgmValue", bgmValue);
		BgmVolumeReset();
	}

	/// <summary>
	/// bgmAudio�� ������ ����
	/// </summary>
	void BgmVolumeReset()
	{
		if (bgmAudio == null)
			bgmAudio = gameObject.AddComponent<AudioSource>();

		bgmAudio.volume = bgmValue;
	}

	/// <summary>
	/// SE�� ��°� ����
	/// </summary>
	/// <param name="value"></param>
	public void SeValueReset(float value)
	{
		seValue = value;
		PlayerPrefs.SetFloat("seValue", seValue);
		SeVolumeReset();
	}

	/// <summary>
	/// seAudio�� ������ ���� 
	/// </summary>
	void SeVolumeReset()
	{
		foreach (var seAudio in seAudioList)
		{
			seAudio.volume = seValue;
		}
	}

	/// <summary>
	/// BGM ����
	/// </summary>
	/// <param name="path"></param>
	public void PlayBGM(string path)
	{
		bgmAudio.clip = Resources.Load<AudioClip>(path);
		bgmAudio.loop = true;
		bgmAudio.Play();
	}

	public void PlayBGM(BGMEnum bgmEnum)
	{
		if (currentPlaybgmEnum == bgmEnum)
			return;

		currentPlaybgmEnum = bgmEnum;

		BGMSaveData bGMSaveData = meumSaveData.GetBGMData(bgmEnum);

        if (bGMSaveData == null)
			return;
		
		bgmAudio.clip = bGMSaveData.audioClip;
		bgmAudio.loop = true;
		bgmAudio.Play();
	}


	/// <summary>
	/// SE ����
	/// </summary>
	/// <param name="path"></param>
	public void PlaySe(string path)
	{
		if (bgmAudio == null)
			bgmAudio = gameObject.AddComponent<AudioSource>();

		AudioSource audioSource = null;

		foreach (var seAudio in seAudioList)
		{
			if (!seAudio.isPlaying)
			{
				audioSource = seAudio;
				break;
			}
		}

		if (audioSource == null)
		{
			audioSource = gameObject.AddComponent<AudioSource>();
			seAudioList.Add(audioSource);
			SeVolumeReset();
		}

		audioSource.clip = Resources.Load<AudioClip>("Se/" + path);
		audioSource.Play();
	}


}


