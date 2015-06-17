using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;
using Random = UnityEngine.Random;

public class ScreenshotTaker : EditorWindow
{
	private const float LengthZMultiplier = 1.25f;
	private const string Sep = "-";

	private static readonly WaitForEndOfFrame WaitForFrame = new WaitForEndOfFrame();

	[MenuItem("MenuItems/ThumbnailMaker")]
	static void CreateBuilder()
	{
		ScreenshotTaker builder = GetWindow<ScreenshotTaker>();
		builder.Show();
	}

	void OnGUI()
	{
		EditorGUILayout.BeginVertical();

		EditorGUILayout.TextField("Save Thumbs");

		if (GUILayout.Button("Take Now!"))
		{
			DateTime n = DateTime.Now;

			string dateVer = StrUtils.Add(
				n.Year.ToString(), Sep,
				n.Month.ToString(), Sep,
				n.Day.ToString(), Sep,
				n.Hour.ToString(), Sep,
				n.Minute.ToString(), Sep,
				n.Second.ToString(),
				"/");

			string savePath = Application.dataPath + "/Screenshots/" + dateVer;

			Debug.LogWarning(savePath);

			EditorCoroutine.start(CaptureRoutine(savePath));
		}

		EditorGUILayout.EndVertical();
	}

	static IEnumerator CaptureRoutine(string savePath)
	{
		ToggleAllCamsButCapture(true);

		var gos = FindObjectsOfType(typeof(GameObject));

		Camera captureCamera = GameObject.Find("CaptureCamera").GetComponent<Camera>();

		foreach (var go in gos)
		{
			GameObject g = go as GameObject;
			if (g != null && g.GetComponent<MeshRenderer>() != null)
			{
				var oldPos = g.transform.position;
				int oldLayer = g.layer;

				g.transform.position = GameObject.Find("CapturePoint").transform.position;
				g.layer = 31;

				var bounds = g.GetComponent<Renderer>().bounds.size;

				float[] arr = {bounds.x, bounds.y, bounds.z};
				Array.Sort(arr);

				var capCamPos = captureCamera.transform.position;
				captureCamera.transform.position = new Vector3(capCamPos.x, capCamPos.y, -arr[2] * LengthZMultiplier);

				string fileName = g.name + ".png";

				Directory.CreateDirectory(savePath);

				string fullPath = Path.Combine(savePath, fileName);

				Debug.LogWarning(fullPath);

				Application.CaptureScreenshot(fullPath);

				yield return WaitForFrame;

				g.transform.position = oldPos;
				g.layer = oldLayer;

				Debug.LogWarning(g.name);
			}
		}
		ToggleAllCamsButCapture(false);
	}

	public static void ToggleAllCamsButCapture(bool setCaptureCamActive)
	{
		foreach (var cam in FindObjectsOfType(typeof(Camera)))
		{
			Camera c = cam as Camera;
			if (c.name != "CaptureCamera")
			{
				c.gameObject.SetActive(!setCaptureCamActive);
			}
		}
	}
}
