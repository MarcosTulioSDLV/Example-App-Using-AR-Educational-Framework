using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class QuestionController2E : MonoBehaviour {

    private GameObject canvas;
    private QuestionController2 questionController2;//usa-se para acrescentar automaticamente nos campos do QuestionController2 do mesmo objeto, as instancias dos elementos criados.
    [HideInInspector] public bool firstTime = true;//variavel para executar uma unica ves o codigo, lembrando que está sendo usado [ExecuteInEditMode]

    private void Awake()
    {
        questionController2 = gameObject.GetComponent<QuestionController2>();
    }

    // Use this for initialization
    void Start()
    {
        if (!Application.isPlaying)
        {
            if (firstTime)
            {
                if (questionController2 != null)
                {

                    //ajusta o tamanho do vectores AdditionalElements1 e AdditionalElements3 em 1, para acrescentar alguns objetos
                    questionController2.AdditionalElements1 = new GameObject[1];
                    questionController2.AdditionalElements3 = new GameObject[1];

                    //CRIAR INSTANCIA DO MARCADOR E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER2
                    Object resourceImageTarget1Prefab = Resources.Load("ImageTarget1Prefab");//obtem o prefab do ImageTarget ImageTarget1Prefab, que está num arquivo Resources
                    if (resourceImageTarget1Prefab != null)
                    {
                        //criar uma instancia do prefabs e inicializar ela
                        GameObject marker = (GameObject)GameObject.Instantiate(resourceImageTarget1Prefab);
                        marker.name = "ImageTarget";

                        //CRIAR objeto1 e objeto2 usados como alternativa
                        GameObject[] objects = new GameObject[2];
                        //CRIAR objeto1 usado como alternativa de resposta
                        objects[0] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        objects[0].transform.localPosition = marker.transform.localPosition;
                        //ajustar a posicao em Y do objeto1
                        Vector3 positionT = objects[0].transform.localPosition;
                        positionT.y += 0.65f;
                        objects[0].transform.localPosition = positionT;
                        //colocar objeto1 como filho do marcador
                        objects[0].transform.SetParent(marker.transform);
                        //CRIAR objeto2 usado como alternativa de resposta
                        objects[1] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        objects[1].transform.localPosition = marker.transform.localPosition;
                        //ajustar tamanho do objeto Y do objeto2
                        objects[1].transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                        //ajustar a posicao em Y do objeto2
                        Vector3 positionT2 = objects[1].transform.localPosition;
                        positionT2.y += 0.65f;
                        objects[1].transform.localPosition = positionT2;
                        //colocar objeto2 como filho do marcador
                        objects[1].transform.SetParent(marker.transform);
                        //acrescenta a instancias no campo automaticamente.  
                        questionController2.Marker = marker;
                        questionController2.AnswerAlternatives[0] = objects[0];
                        questionController2.AnswerAlternatives[1] = objects[1];
                    }
                    else
                    {
                        Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ImageTarget1Prefab is not found in the folder Resources.");
                    }
                    //CRIAR INSTANCIA DO CANVAS E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER2     
                    Object resourceCanvasPrefab = Resources.Load("CanvasPrefab");
                    if (resourceCanvasPrefab != null)
                    {
                        canvas = (GameObject)GameObject.Instantiate(resourceCanvasPrefab);
                        canvas.name = "Canvas";

                        //CRIAR O OBJETO EVENTSYSTEM PARA O CANVAS NO CASO DE NAO TER (Um EVENTSYSTEM é criado automaticamente quando se cria um Canvas)
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
                    //CRIAR INSTANCIA DO BOTAO PARA O BOTAO LEFT E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER2
                    Object resource1ButtonPrefab = Resources.Load("ButtonPrefab");//obtem o prefab do botao ButtonPrefab, que está num arquivo Resources
                    if (resource1ButtonPrefab != null)
                    {
                        //criar uma instancia do prefabs e inicializar ela
                        GameObject leftButton = (GameObject)GameObject.Instantiate(resource1ButtonPrefab);
                        leftButton.name = "LeftButton";
                        leftButton.transform.SetParent(canvas.transform);
                        leftButton.transform.localPosition = new Vector3(-80, -95, 0);
                        Sprite image = Resources.Load("ButtonTextures/Left", typeof(Sprite)) as Sprite;//obtem a imagem para o botao
                        if (image != null)
                        {
                            leftButton.GetComponent<Image>().sprite = image;
                            leftButton.GetComponent<Image>().type = Image.Type.Simple;
                        }
                        else
                        {
                            leftButton.transform.GetChild(0).GetComponent<Text>().text = "Left";
                            Debug.LogWarning("AR Educational Framework Message: The Resource: Left is not found in the folder Resources/ButtonTextures.");
                        }
                        //acrescenta a instancia no campo automaticamente.
                        questionController2.LeftButton = leftButton.GetComponent<Button>();
                    }
                    else
                    {
                        Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ButtonPrefab is not found in the folder Resources.");
                    }
                    //CRIAR INSTANCIA DO BOTAO PARA O BOTAO RIGHT E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER2
                    Object resource2ButtonPrefab = Resources.Load("ButtonPrefab");//obtem o prefab do botao ButtonPrefab, que está num arquivo Resources
                    if (resource2ButtonPrefab != null)
                    {
                        //criar uma instancia do prefabs e inicializar ela
                        GameObject rightButton = (GameObject)GameObject.Instantiate(resource2ButtonPrefab);
                        rightButton.name = "RightButton";
                        rightButton.transform.SetParent(canvas.transform);
                        rightButton.transform.localPosition = new Vector3(80, -95, 0);
                        Sprite image = Resources.Load("ButtonTextures/Right", typeof(Sprite)) as Sprite;//obtem a imagem para o botao
                        if (image != null)
                        {
                            rightButton.GetComponent<Image>().sprite = image;
                            rightButton.GetComponent<Image>().type = Image.Type.Simple;
                        }
                        else
                        {
                            rightButton.transform.GetChild(0).GetComponent<Text>().text = "Right";
                            Debug.LogWarning("AR Educational Framework Message: The Resource: Right is not found in the folder Resources/ButtonTextures.");
                        }
                        //acrescenta a instancia no campo automaticamente.
                        questionController2.RightButton = rightButton.GetComponent<Button>();
                    }
                    else
                    {
                        Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ButtonPrefab is not found in the folder Resources.");
                    }
                    //CRIAR INSTANCIA DO BOTAO PARA O BOTAO CHECK PERGUNTA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER2
                    Object resource3ButtonPrefab = Resources.Load("ButtonPrefab");//obtem o prefab do botao ButtonPrefab, que está num arquivo Resources
                    if (resource3ButtonPrefab != null)
                    {
                        //criar uma instancia do prefabs e inicializar ela
                        GameObject checkButton = (GameObject)GameObject.Instantiate(resource3ButtonPrefab);
                        checkButton.name = "CheckButton";
                        checkButton.transform.SetParent(canvas.transform);
                        checkButton.transform.localPosition = new Vector3(0, -125, 0);
                        Sprite image = Resources.Load("ButtonTextures/Ok", typeof(Sprite)) as Sprite;//obtem a imagem para o botao
                        if (image != null)
                        {
                            checkButton.GetComponent<Image>().sprite = image;
                            checkButton.GetComponent<Image>().type = Image.Type.Simple;
                        }
                        else
                        {
                            checkButton.transform.GetChild(0).GetComponent<Text>().text = "OK";
                            Debug.LogWarning("AR Educational Framework Message: The Resource: Ok is not found in the folder Resources/ButtonTextures.");
                        }
                        //acrescenta a instancia no campo automaticamente.
                        questionController2.CheckButton = checkButton.GetComponent<Button>();
                    }
                    else
                    {
                        Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ButtonPrefab is not found in the folder Resources.");
                    }
                    //CRIAR INSTANCIA DO BOTAO PARA O BOTAO RESET PERGUNTA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER2
                    Object resource4ButtonPrefab = Resources.Load("ButtonPrefab");//obtem o prefab do botao ButtonPrefab, que está num arquivo Resources
                    if (resource4ButtonPrefab != null)
                    {
                        //criar uma instancia do prefabs e inicializar ela
                        GameObject resetQuestionButton = (GameObject)GameObject.Instantiate(resource4ButtonPrefab);
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
                        questionController2.ResetButton = resetQuestionButton.GetComponent<Button>();
                    }
                    else
                    {
                        Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ButtonPrefab is not found in the folder Resources.");
                    }
                    //CRIAR INSTANCIA DA IMAGEM PARA A IMAGEM PERGUNTA CERTA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER2
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
                        questionController2.CorrectAnswerImage = correctAnswerImage.GetComponent<Image>();
                    }
                    else
                    {
                        Debug.LogError("AR Educational Framework Message: The Resource: CorrectAnswerImage is not found in the folder Resources.");
                    }
                    //CRIAR INSTANCIA DA IMAGEM PARA IMAGEM PERGUNTA ERRADA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER2
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
                        questionController2.WrongAnswerImage = wrongAnswerImage.GetComponent<Image>();
                    }
                    else
                    {
                        Debug.LogError("AR Educational Framework Message: The Resource: WrongAnswerImage is not found in the folder Resources.");
                    }

                    //CRIAR TEXTO DO TEMPO E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER2
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
                    questionController2.TimeText = timeText.GetComponent<Text>();

                    //CRIAR INSTANCIA DO BOTAO PARA O BOTAO VER RESPOSTA DA PERGUNTA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER2
                    Object resource5ButtonPrefab = Resources.Load("ButtonPrefab");//obtem o prefab do botao ButtonPrefab, que está num arquivo Resources
                    if (resource5ButtonPrefab != null)
                    {
                        //criar uma instancia do prefabs e inicializar ela
                        GameObject answerButton = (GameObject)GameObject.Instantiate(resource5ButtonPrefab);
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
                        questionController2.AnswerButton = answerButton.GetComponent<Button>();
                    }
                    else
                    {
                        Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ButtonPrefab is not found in the folder Resources.");
                    }

                    //CRIAR TEXTO DE PERGUNTA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER2
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
                    questionController2.AdditionalElements1[0] = questionText;

                    //CRIAR TEXTO DE RESPOSTA E ACRESCENTAR NO CAMPO DO QUESTIONCONTROLLER2
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
                    questionController2.AdditionalElements3[0] = answerText;

                }
                else
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject: " + gameObject.name + " doesn't have QuestionController2 component. you need to add it before add this class.");
                }

                firstTime = false;
            }
        }
    }
	
}
