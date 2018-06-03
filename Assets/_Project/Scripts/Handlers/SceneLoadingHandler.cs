using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CardboardVRProto
{
	public class SceneLoadingHandler : MonoBehaviour
	{
		public delegate void OnSceneStart();
		public event OnSceneStart SceneStartEvent;

		private const float SceneLoadingProgressThreshold = 0.9f;
		private const int SceneIndex = 0;

		[Header("Fade In/Out Variables")]
		[SerializeField] private Image _background = null;
		[SerializeField] private Ease _fadeInEase = Ease.Linear;
		[SerializeField] private Ease _fadeOutEase = Ease.Linear;
		[SerializeField] [Range(0, 3)] private float _fadeInDuration = 0;
		[SerializeField] [Range(0, 3)] private float _fadeOutDuration = 0;

		private AsyncOperation _sceneLoading;

		private bool _fadeInIsComplete = false;

		void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		public void LoadScene()
		{
			_fadeInIsComplete = false;
			_background
				.DOFade(1, _fadeInDuration)
				.SetEase(_fadeInEase)
				.OnComplete(() => _fadeInIsComplete = true);

			StartCoroutine(LoadLevelAsync(SceneIndex));
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			_background.DOFade(1, 0);
			_background
				.DOFade(0, _fadeOutDuration)
				.SetEase(_fadeOutEase)
				.OnComplete(() =>
				{
					if (SceneStartEvent != null) SceneStartEvent();
				});
		}

		private IEnumerator LoadLevelAsync(int sceneIndex,
			LoadSceneMode mode = LoadSceneMode.Single)
		{
			if (_sceneLoading != null) yield break;

			_sceneLoading = SceneManager.LoadSceneAsync(sceneIndex, mode);
			_sceneLoading.allowSceneActivation = false;

			while (!_sceneLoading.allowSceneActivation)
			{
				if (_sceneLoading.progress >= SceneLoadingProgressThreshold &&
				    _fadeInIsComplete)
				{
					_sceneLoading.allowSceneActivation = true;
				}

				yield return null;
			}

			_sceneLoading = null;
		}
	}
}
