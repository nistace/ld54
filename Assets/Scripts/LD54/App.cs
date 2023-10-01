using NiUtils.Libraries;
using UnityEngine;

namespace LD54 {
	public class App : MonoBehaviour {
		private static bool initialized { get; set; }

		[SerializeField] protected AudioClipLibrary audioClipLibrary;

		private void Awake() {
			if (initialized) Destroy(gameObject);
			else {
				AudioClips.LoadLibrary(audioClipLibrary);

				DontDestroyOnLoad(gameObject);
				initialized = true;
			}
		}
	}
}