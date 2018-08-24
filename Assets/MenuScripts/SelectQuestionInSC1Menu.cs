using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectQuestionInSC1Menu : MonoBehaviour {

    [SerializeField] private Button q1Button;
    [SerializeField] private Button q2Button;
    [SerializeField] private Button q3Button;
    [SerializeField] private Button q4Button;
    private static byte scene1questionIndex = 0; // indica pergunta escolhida da cena1, 0 significa pergunta 1, 1 significa pergunta 2, 2 signifca pergunta 3, e 3 significa pergunta 4
    public static byte Scene1questionIndex{ get {return scene1questionIndex;} set{scene1questionIndex = value;}}

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
        Scene1questionIndex = 0;//coloca a variavel em 0 para indicar que foi escolhida a pergunta 1
        SceneManager.LoadScene("Scene1");//passa a Scene1  de perguntas
    }
    private void Q2ButtonMethod()//é pressionado o botao da pergunta 2
    {
        Scene1questionIndex = 1;//coloca a variavel em 1 para indicar que foi escolhida a pergunta 2
        SceneManager.LoadScene("Scene1");//passa a Scene1  de perguntas
    }
    private void Q3ButtonMethod()//é pressionado o botao da pergunta 3
    {
        Scene1questionIndex = 2;//coloca a variavel em 2 para indicar que foi escolhida a pergunta 3
        SceneManager.LoadScene("Scene1");//passa a Scene1  de perguntas
    }
    private void Q4ButtonMethod()//é pressionado o botao da pergunta 4
    {
        Scene1questionIndex = 3;//coloca a variavel em 3 para indicar que foi escolhida a pergunta 4
        SceneManager.LoadScene("Scene1");//passa a Scene1  de perguntas
    }


}
