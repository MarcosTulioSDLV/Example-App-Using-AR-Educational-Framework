using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectQuestionInScene2OfQuestions : MonoBehaviour {

    [SerializeField] private QuestionController2 questionControllerQ1;
    [SerializeField] private QuestionController2 questionControllerQ2;
    [SerializeField] private QuestionController2 questionControllerQ3;
    [SerializeField] private QuestionController2 questionControllerQ4;

	// Use this for initialization
	void Start () {

        if (SelectQuestionInSC2Menu1.Scene2questionIndex == 0)//se foi escolhida a pergunta 1 no menu
        {
            questionControllerQ1.Active = true;//ativar a pergunta 1
        }
        else if (SelectQuestionInSC2Menu1.Scene2questionIndex == 1)//se foi escolhida a pergunta 2 no menu
        {
            questionControllerQ2.Active = true;//ativar a pergunta 2
        }
        else if (SelectQuestionInSC2Menu1.Scene2questionIndex == 2)//se foi escolhida a pergunta 3 no menu
        {
            questionControllerQ3.Active = true;//ativar a pergunta 3
        }
        else if(SelectQuestionInSC2Menu1.Scene2questionIndex == 3)//se foi escolhida a pergunta 4 no menu
        {
            questionControllerQ4.Active = true;//ativar a perguna 4
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}



}
