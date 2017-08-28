using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleManager : MonoBehaviour {

	public static ConsoleManager Instance;

	public Text Console;

	private void Awake() {
		Instance = this;
	}

	public void print(object msg) {
		Console.text = msg.ToString();
	}
}
