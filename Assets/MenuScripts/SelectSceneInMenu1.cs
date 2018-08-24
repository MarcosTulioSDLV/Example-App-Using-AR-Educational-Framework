using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectSceneInMenu1 : MonoBehaviour {

    [SerializeField] private Button scene1Button;
    [SerializeField] private Button scene2Button;


	// Use this for initialization
	void Start () {

        scene1Button.onClick.AddListener(Scene1ButtonMethod);
        scene2Button.onClick.AddListener(Scene2ButtonMethod);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Scene1ButtonMethod()
    {
        SceneManager.LoadScene("SC1Menu");
    }
    private void Scene2ButtonMethod()
    {
        SceneManager.LoadScene("SC2Menu0");
    }
}
