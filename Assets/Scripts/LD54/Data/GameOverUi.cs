using System.Collections;
using LD54.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverUi : MonoBehaviour {
	public static GameOverUi current { get; private set; }

	[SerializeField] protected GameObject _rootObject;
	[SerializeField] protected TMP_Text _body;
	[SerializeField] protected string _bodyTemplate;
	[SerializeField] protected AnimationCurve _appearAnimationCurve;
	[SerializeField] protected float _appearDuration = .5f;
	[SerializeField] protected Button _newGameButton;
	[SerializeField] protected Button _backToMenuButton;
	[SerializeField] protected Button _quitButton;

	public UnityEvent onNewGameClicked => _newGameButton.onClick;
	public UnityEvent onBackToMenuClicked => _backToMenuButton.onClick;
	public UnityEvent onQuitClicked => _quitButton.onClick;

	private void Awake() {
		current = this;
	}

	private void Start() {
		_rootObject.gameObject.SetActive(false);
		_rootObject.transform.localScale = Vector3.zero;
	}

	public void Show() {
		_body.text = _bodyTemplate.Replace("[score]", $"{GameSessionData.current.score:0}").Replace("[time]", $"{GameSessionData.gameTime:0}");
#if UNITY_WEBGL || UNITY_EDITOR
		_quitButton.gameObject.SetActive(false);
#endif
		StartCoroutine(PlayAppearAnimation());
		_rootObject.gameObject.SetActive(true);
	}

	private IEnumerator PlayAppearAnimation() {
		for (var lerp = 0f; lerp < 1; lerp += Time.deltaTime / _appearDuration) {
			_rootObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, _appearAnimationCurve.Evaluate(lerp));
			yield return null;
		}
		_rootObject.transform.localScale = Vector3.one;
	}
}