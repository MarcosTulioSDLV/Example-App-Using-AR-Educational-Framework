using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectQuestionInScene1 : MonoBehaviour {

    [SerializeField] private QuestionController1 questionController1Q1;//QuestionController1 da pergunta1
    [SerializeField] private QuestionController1 questionController1Q2;//QuestionController1 da pergunta2
    [SerializeField] private QuestionController1 questionController1Q3;//QuestionController1 da pergunta3
    [SerializeField] private QuestionController1 questionController1Q4;//QuestionController1 da pergunta 4

    // Use this for initialization
    void Start () {

        if (SelectQuestionInSC1Menu.Scene1questionIndex == 0)//se foi escolhida a pergunta 1 no menu
        {
            questionController1Q1.Active = true;//ativar a pergunta 1
        }
        else if (SelectQuestionInSC1Menu.Scene1questionIndex == 1)//se foi escolhida a pergunta 2 no menu
        {
            questionController1Q2.Active = true;//ativar a pergunta 2
        }
        else if(SelectQuestionInSC1Menu.Scene1questionIndex==2)//se foi escolhida a pergunta 3 no menu
        {
            questionController1Q3.Active = true;//ativar a pergunta 3
        }
        else if(SelectQuestionInSC1Menu.Scene1questionIndex == 3)//se foi escolhida a pergunta 4 no menu
        {
            questionController1Q4.Active = true;
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}


}
