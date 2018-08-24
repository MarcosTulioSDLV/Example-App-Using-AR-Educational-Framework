using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectModeInSC2Menu0 : MonoBehaviour {

    [SerializeField] private Button trainingButton;
    [SerializeField] private Button questionsButton;


	// Use this for initialization
	void Start () {
        trainingButton.onClick.AddListener(TrainingButtonMethod);
        questionsButton.onClick.AddListener(QuestionsButtonMethod);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void TrainingButtonMethod()
    {
        SceneManager.LoadScene("Scene2OfTraining");
    }
    private void QuestionsButtonMethod()
    {
        SceneManager.LoadScene("SC2Menu1");
    }


}
