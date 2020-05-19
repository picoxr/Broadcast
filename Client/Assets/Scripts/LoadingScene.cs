using UnityEngine;
using System.Collections;

public class LoadingScene : MonoBehaviour {

	private TextMesh text;

	// Use this for initialization
	void Start ()
    {
		text = transform.Find("Text").GetComponent<TextMesh>();
		StartCoroutine(LoadScene());
	}

    IEnumerator LoadScene()
    {
		AsyncOperation op = Application.LoadLevelAsync(1);
		op.allowSceneActivation = false;
		while (op.progress < 0.9f) {
			text.text = Mathf.FloorToInt(op.progress * 100).ToString();
			yield return new WaitForEndOfFrame();
		}

		text.text = "100";
		yield return new WaitForEndOfFrame();

		op.allowSceneActivation = true;
    }

}
