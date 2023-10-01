using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LD54.Data {
	public class OrderPackageImageUi : MonoBehaviour {
		[SerializeField] protected Image _packageIcon;
		[SerializeField] protected Image _presentInStorageMarker;
		[SerializeField] protected TMP_Text _counterText;

		public Sprite icon {
			get => _packageIcon.sprite;
			set => _packageIcon.sprite = value;
		}

		public string counterText {
			get => _counterText.text;
			set => _counterText.text = value;
		}

		public Color counterTextColor {
			get => _counterText.color;
			set => _counterText.color = value;
		}

		public bool presentInStorage {
			get => _presentInStorageMarker.enabled;
			set => _presentInStorageMarker.enabled = value;
		}
	}
}