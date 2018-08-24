using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class QuestionController1E : MonoBehaviour {

    private GameObject canvas;
    private QuestionController1 questionController1;//usa-se para acrescentar automaticamente nos campos do QuestionController1 do mesmo objeto, as instancias dos elementos criados.
    [HideInInspector] public bool firstTime = true;//variavel para executar uma unica ves o codigo, lembrando que está sendo usado [ExecuteInEditMode]

    private void Awake()
    {
        questionController1 = gameObject.GetComponent<QuestionController1>();
    }

    // Use this for initialization
    void Start ()
    {
        if (!Application.isPlaying)
        {
            if (firstTime)
            {
                if (questionController1 != null)
                {

                        //ajusta o tamanho dos vectores AdditionalElements1 e AdditionalElements3 a 2 para acrescentar alguns objetos
                        questionController1.AdditionalElements1 = new GameObject[1];
                        questionController1.AdditionalElements3 = new GameObject[1];

                        //CRIAR INSTANCIA DO MARCADOR E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER1 
                        Object resourceImageTarget1Prefab = Resources.Load("ImageTarget1Prefab");//obtem o prefab do ImageTarget ImageTarget1Prefab, que está num arquivo Resources
                        if (resourceImageTarget1Prefab != null)
                        {
                            //criar uma instancia do prefabs e inicializar ela
                            GameObject marker = (GameObject)GameObject.Instantiate(resourceImageTarget1Prefab);
                            marker.name = "ImageTarget";

                            //criar um objeto para teste
                            GameObject objectCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            objectCube.transform.localPosition = marker.transform.localPosition;
                            //ajustar a posicao em Y do objeto
                            Vector3 positionT = objectCube.transform.localPosition;
                            positionT.y += 0.65f;
                            objectCube.transform.localPosition = positionT;
                            //colocar objeto como filho do marcador
                            objectCube.transform.SetParent(marker.transform);
                            // acrescenta a instancias no campos automaticamente.                 
                            questionController1.Marker = marker;
                            questionController1.QuestionObjects[0]=objectCube;
                        }
                        else
                        {
                            Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ImageTarget1Prefab is not found in the folder Resources.");
                        }
                        //CRIAR INSTANCIA DO CANVAS E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER1               
                        Object resourceCanvasPrefab = Resources.Load("CanvasPrefab");//obtem o prefab do canvas CanvasPrefab, que está num arquivo Resources
                        if (resourceCanvasPrefab != null)
                        {
                            //criar a instancia do canvas
                            canvas = (GameObject)GameObject.Instantiate(resourceCanvasPrefab);
                            canvas.name = "Canvas";

                            //CRIAR ISNTANCIA DE EVENTSYSTEM PARA O CANVAS NO CASO DE NAO TER (Um EVENTSYSTEM é criado automaticamente quando se cria um Canvas)
                            if (GameObject.FindObjectOfType<EventSystem>() == null)
                            {
                                //criar eventsystem só com codigo (sem prefab)
                                GameObject eventSystem = new GameObject("EventSystem");
                                eventSystem.AddComponent<EventSystem>();
                                eventSystem.AddComponent<StandaloneInputModule>();
                            }
                        }
                        else
                        {
                            Debug.LogError("AR Educational Framework Message: The GameObject Prefab: CanvasPrefab is not found in the folder Resources.");
                        }                       
                        //CRIAR TEXTOS DE ALTERNATIVA E ACRESCENTAR NOS CAMPOS DO QUESTIONCONTROLLER1
                        GameObject[] alternativeTexts = new GameObject[2];
                        for (int i = 0; i < 2; i++)
                        {
                            alternativeTexts[i]= new GameObject("Alternative " + (i + 1));
                            alternativeTexts[i].AddComponent<Text>();
                            alternativeTexts[i].transform.SetParent(canvas.transform);
                            alternativeTexts[i].GetComponent<RectTransform>().sizeDelta = new Vector2(180, 30);
                            alternativeTexts[i].GetComponent<Text>().text = "Alternative " + (i + 1);
                            alternativeTexts[i].GetComponent<Text>().fontStyle = FontStyle.Bold;
                            alternativeTexts[i].GetComponent<Text>().fontSize = 30;
                            alternativeTexts[i].GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                            alternativeTexts[i].GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
                            alternativeTexts[i].GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
                            alternativeTexts[i].GetComponent<Text>().color = Color.white;

                            if (i == 0)//para o texto de alternativa1, posicionar ele
                            {
                                    alternativeTexts[i].transform.localPosition = new Vector3(320, 30, 0);
                            }
                            else if (i == 1)//para o texto de alternativa2, posicionar ele
                            {
                                    alternativeTexts[i].transform.localPosition = new Vector3(320, -10, 0);
                            }

                            //acrescenta os textos nos campos automaticamente.
                            questionController1.Texts[i] = alternativeTexts[i].GetComponent<Text>();
                        }
                        //CRIAR INSTANCIA DO BOTAO PARA O BOTAO RESET PERGUNTA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER1
                        Object resource1ButtonPrefab = Resources.Load("ButtonPrefab");//obtem o prefab do botao ButtonPrefab, que está num arquivo Resources
                        if (resource1ButtonPrefab != null)
                        {
                            //criar uma instancia do prefabs e inicializar ela
                            GameObject resetQuestionButton = (GameObject)GameObject.Instantiate(resource1ButtonPrefab);
                            resetQuestionButton.name = "ResetQuestionButton";
                            resetQuestionButton.transform.SetParent(canvas.transform);
                            resetQuestionButton.transform.localPosition = new Vector3(0, -95, 0);
                            Sprite image = Resources.Load("ButtonTextures/TryAgain", typeof(Sprite)) as Sprite;//obtem a imagem para o botao
                            if (image != null)
                            {
                                resetQuestionButton.GetComponent<Image>().sprite = image;
                                resetQuestionButton.GetComponent<Image>().type = Image.Type.Simple;
                            }
                            else
                            {
                                resetQuestionButton.transform.GetChild(0).GetComponent<Text>().text = "Try Again";
                                Debug.LogWarning("AR Educational Framework Message: The Resource: TryAgain is not found in the folder Resources/ButtonTextures.");
                            }
                            //acrescenta a instancia no campo automaticamente.
                            questionController1.ResetButton = resetQuestionButton.GetComponent<Button>();
                        }
                        else
                        {
                            Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ButtonPrefab is not found in the folder Resources.");
                        }
                        //CRIAR INSTANCIA DA IMAGEM PARA A IMAGEM PERGUNTA CERTA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER1
                        Sprite resource1CorrectAnswerImage = Resources.Load("CorrectAnswerImage", typeof(Sprite)) as Sprite;//obtem a imagem CorrectAnswerImage,  que está num arquivo Resources
                        if (resource1CorrectAnswerImage != null)
                        {
                            //criar uma elemento Image só com codigo, e apartir da imagem CorrectAnswerImage inicializando ela
                            GameObject correctAnswerImage = new GameObject("CorrectAnswerImage");
                            correctAnswerImage.AddComponent<Image>();
                            correctAnswerImage.transform.SetParent(canvas.transform);
                            correctAnswerImage.transform.localPosition = new Vector3(0, 25, 0);
                            correctAnswerImage.GetComponent<Image>().sprite = resource1CorrectAnswerImage;
                            correctAnswerImage.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 140);
                            //acrescenta a imagem no campo automaticamente. 
                            questionController1.CorrectAnswerImage = correctAnswerImage.GetComponent<Image>();
                        }
                        else
                        {
                            Debug.LogError("AR Educational Framework Message: The Resource: CorrectAnswerImage is not found in the folder Resources.");
                        }
                        //CRIAR INSTANCIA DA IMAGEM PARA IMAGEM PERGUNTA ERRADA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER1
                        Sprite resourceWrongAnswerImage = Resources.Load("WrongAnswerImage", typeof(Sprite)) as Sprite;//obtem a imagem WrongAnswerImage,  que está num arquivo Resources
                        if (resourceWrongAnswerImage != null)
                        {
                            //criar uma elemento Image só com codigo, e apartir da imagem WrongAnswerImage inicializando ela
                            GameObject wrongAnswerImage = new GameObject("WrongAnswerImage");
                            wrongAnswerImage.AddComponent<Image>();
                            wrongAnswerImage.transform.SetParent(canvas.transform);
                            wrongAnswerImage.transform.localPosition = new Vector3(0, 25, 0);
                            wrongAnswerImage.GetComponent<Image>().sprite = resourceWrongAnswerImage;
                            wrongAnswerImage.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 140);
                            //acrescenta a imagem no campo automaticamente.
                            questionController1.WrongAnswerImage = wrongAnswerImage.GetComponent<Image>();
                        }
                        else
                        {
                            Debug.LogError("AR Educational Framework Message: The Resource: WrongAnswerImage is not found in the folder Resources.");
                        }

                        //CRIAR TEXTO DO TEMPO E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER1
                        GameObject timeText = new GameObject("TimeText");
                        timeText.AddComponent<Text>();
                        timeText.transform.SetParent(canvas.transform);
                        timeText.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 30);
                        timeText.GetComponent<Text>().text = "Time Text";
                        timeText.GetComponent<Text>().fontStyle = FontStyle.Bold;
                        timeText.GetComponent<Text>().fontSize = 30;
                        timeText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        timeText.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
                        timeText.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
                        timeText.transform.localPosition = new Vector3(320, 135, 0);
                        timeText.GetComponent<Text>().color = Color.yellow;
                        //acrescenta o texto no campo automaticamente.
                        questionController1.TimeText = timeText.GetComponent<Text>();

                        //CRIAR INSTANCIA DO BOTAO PARA O BOTAO VER RESPOSTA DA PERGUNTA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER1
                        Object resource2ButtonPrefab = Resources.Load("ButtonPrefab");//obtem o prefab do botao ButtonPrefab, que está num arquivo Resources
                        if (resource2ButtonPrefab != null)
                        {
                            //criar uma instancia do prefabs e inicializar ela
                            GameObject answerButton = (GameObject)GameObject.Instantiate(resource2ButtonPrefab);
                            answerButton.name = "AnswerButton";
                            answerButton.transform.SetParent(canvas.transform);
                            answerButton.transform.localPosition = new Vector3(0, -65, 0);
                            Sprite image = Resources.Load("ButtonTextures/SeeAnswer", typeof(Sprite)) as Sprite;//obtem a imagem para o botao
                            if (image != null)
                            {
                                answerButton.GetComponent<Image>().sprite = image;
                                answerButton.GetComponent<Image>().type = Image.Type.Simple;
                            }
                            else
                            {
                                answerButton.transform.GetChild(0).GetComponent<Text>().text = "See Answer";
                                Debug.LogWarning("AR Educational Framework Message: The Resource: SeeAnswer is not found in the folder Resources/ButtonTextures.");
                            }
                            //acrescenta a instancia no campo automaticamente.
                            questionController1.AnswerButton = answerButton.GetComponent<Button>();
                        }
                        else
                        {
                            Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ButtonPrefab is not found in the folder Resources.");
                        }
                        //CRIAR INSTANCIA DA IMAGEM PARA A IMAGEM PEQUENA QUE INDICA A ALTERNATIVA CERTA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER1
                        Sprite resource2CorrectAnswerImage = Resources.Load("CorrectAnswerImage", typeof(Sprite)) as Sprite;//obtem a imagem CorrectAnswerImage,  que está num arquivo Resources
                        if (resource2CorrectAnswerImage != null)
                        {
                            //criar uma elemento Image só com codigo, e apartir da imagem CorrectAnswerImage inicializando ela
                            GameObject correctAnswerImage2 = new GameObject("CorrectAnswerImage2");
                            correctAnswerImage2.AddComponent<Image>();
                            correctAnswerImage2.transform.SetParent(canvas.transform);
                            correctAnswerImage2.transform.localPosition = new Vector3(320, 30, 0);//mesma posicao do texto de alternativa 1
                            correctAnswerImage2.GetComponent<Image>().sprite = resource2CorrectAnswerImage;
                            correctAnswerImage2.GetComponent<RectTransform>().sizeDelta = new Vector2(32, 30);
                            //acrescenta a instancia no campo automaticamente.
                            questionController1.CorrectAnswerImage2 = correctAnswerImage2.GetComponent<Image>();
                        }
                        else
                        {
                            Debug.LogError("AR Educational Framework Message: The Resource: CorrectAnswerImage is not found in the folder Resources.");
                        }

                        //CRIAR TEXTO DE PERGUNTA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER1
                        GameObject questionText = new GameObject("QuestionText");
                        questionText.AddComponent<Text>();
                        questionText.transform.SetParent(canvas.transform);
                        questionText.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 30);
                        questionText.GetComponent<Text>().text = "Question Text";
                        questionText.GetComponent<Text>().fontStyle = FontStyle.Bold;
                        questionText.GetComponent<Text>().fontSize = 30;
                        questionText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        questionText.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
                        questionText.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
                        questionText.transform.localPosition = new Vector3(0, 135, 0);
                        questionText.GetComponent<Text>().color = Color.blue;
                        //acrescenta o texto no campo automaticamente.
                        questionController1.AdditionalElements1[0] = questionText;

                        //CRIAR TEXTO DE RESPOSTA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER1
                        GameObject answerText = new GameObject("AnswerText");
                        answerText.AddComponent<Text>();
                        answerText.transform.SetParent(canvas.transform);
                        answerText.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 30);
                        answerText.GetComponent<Text>().text = "Answer Text";
                        answerText.GetComponent<Text>().fontStyle = FontStyle.Bold;
                        answerText.GetComponent<Text>().fontSize = 30;
                        answerText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                        answerText.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
                        answerText.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
                        answerText.transform.localPosition = new Vector3(0, 135, 0);
                        answerText.GetComponent<Text>().color = Color.blue;
                        //acrescenta o texto no campo automaticamente.
                        questionController1.AdditionalElements3[0] = answerText;

                }
                else
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject: " + gameObject.name + " doesn't have QuestionController1 component. you need to add it before add this class.");
                }

                firstTime = false;
            }
        }
    }
	
}
