using System;
using LD54.Data;
using TMPro;
using UnityEngine;

namespace LD54 {
	public class GameDataUi : MonoBehaviour {
		[SerializeField] protected TMP_Text _scoreText;
		[SerializeField] protected TMP_Text _creditsText;
		[SerializeField] protected TMP_Text _timeText;

		private void Update() {
			_scoreText.text = $"{GameSessionData.current?.score ?? 0:0}";
			_creditsText.text = $"{GameSessionData.current?.credits ?? 0:0}";
			_timeText.text = $"{GameSessionData.gameTime:0}";
		}
	}
}