using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectQuestionInSC2Menu1 : MonoBehaviour {

    [SerializeField] private Button q1Button;
    [SerializeField] private Button q2Button;
    [SerializeField] private Button q3Button;
    [SerializeField] private Button q4Button;
    private static byte scene2questionIndex = 0;//indica pergunta escolhida da cena2, 0 significa pergunta 1, 1 significa pergunta 2, e 2 signifca pergunta 3 
    public static byte Scene2questionIndex{ get {return scene2questionIndex;} set{scene2questionIndex = value;}}

    // Use this for initialization
    void Start () {
        q1Button.onClick.AddListener(Q1ButtonMethod);
        q2Button.onClick.AddListener(Q2ButtonMethod);
        q3Button.onClick.AddListener(Q3ButtonMethod);
        q4Button.onClick.AddListener(Q4ButtonMethod);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Q1ButtonMethod()//é pressionado o botao da pergunta 1
    {
        Scene2questionIndex = 0;//coloca a variavel em 0 para indicar que foi escolhida a pergunta 1
        SceneManager.LoadScene("Scene2OfQuestions");//passa a Scene2  de perguntas
    }
    private void Q2ButtonMethod()//é pressionado o botao da pergunta 2
    {
        Scene2questionIndex = 1;//coloca a variavel em 1 para indicar que foi escolhida a pergunta 2
        SceneManager.LoadScene("Scene2OfQuestions");//passa a Scene2  de perguntas
    }
    private void Q3ButtonMethod()//é pressionado o botao da pergunta 3
    {
        Scene2questionIndex = 2;//coloca a variavel em 2 para indicar que foi escolhida a pergunta 3
        SceneManager.LoadScene("Scene2OfQuestions");//passa a Scene2  de perguntas
    }
    private void Q4ButtonMethod()//é pressionado o botao da pergunta 4
    {
        Scene2questionIndex = 3;//coloca a variavel em 3 para indicar que foi escolhida a pergunta 4
        SceneManager.LoadScene("Scene2OfQuestions");//passa a Scene2  de perguntas
    }
}
