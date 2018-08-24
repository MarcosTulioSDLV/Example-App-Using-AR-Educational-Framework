using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour {

    //public enum ActualScene {Scene1,Scene2};
    //[SerializeField] private ActualScene actualScene;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ExitButtonMethodToMenu1()//metodo para o botao Sair da cena Menu1
    {
        Application.Quit();
    }
    public void ExitButtonMethodToSC1Menu()//metodo para o botao Sair da cena SC1Menu  (menu de perguntas da Scene1)
    {
        SceneManager.LoadScene("Menu1");
    }
    public void ExitButtonMethodToScene1()//metodo para o botao Sair da cena Scene1
    {
        SceneManager.LoadScene("SC1Menu");
    }
    public void ExitButtonMethodToSC2Menu0()//metodo para o botao Sair da cena SC2Menu0 (menu de modo da Scene2)
    {
        SceneManager.LoadScene("Menu1");
    }
    public void ExitButtonMethodToSC2Menu1()//metodo para o botao Sair da cena SC2Menu1 (menu de perguntas da Scene2)
    {
        SceneManager.LoadScene("SC2Menu0");
    }
    public void ExitButtonMethodToScene2OfTraining()//metodo para o botao Sair da cena Scene2OfTraining (treinamento da Scene2)
    {
        SceneManager.LoadScene("SC2Menu0");
    }
    public void ExitButtonMethodToScene2OfQuestions()//metodo para o botao Sair da cena Scene2OfQuestions (execucaco de perguntas da Scene2)
    {
        SceneManager.LoadScene("SC2Menu1");
    }


}
