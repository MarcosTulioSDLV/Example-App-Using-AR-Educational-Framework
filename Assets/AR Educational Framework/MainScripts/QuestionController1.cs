using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class QuestionController1 : MonoBehaviour {

    [SerializeField] private bool active=true;
    public bool Active{ get {return active;} set {active = value;}}
    [SerializeField] private bool useMarker = true;
    [SerializeField] private GameObject marker;
    public GameObject Marker{get{return marker;}set{marker = value;}}
    [SerializeField] private bool useQuestionObjects=true;
    [SerializeField] private List<GameObject> questionObjects = new List<GameObject>();//objetos pergunta (NOTA:sao objetos filhos do marcador e sobre os quais vai ser feita a pergunta)
    public List<GameObject> QuestionObjects { get { return questionObjects; } }
    private bool firstFrameActive=true;
    private bool firstFrameDesactive = true;
    public enum TypeOfAnswerAlternatives {Texts,Images};
    [SerializeField] private TypeOfAnswerAlternatives typeOfAnswerAlternatives;
    [SerializeField] private List<Text> texts=new List<Text>();
    public List<Text> Texts{get{return texts;}}
    [SerializeField] private List<UnityEngine.UI.Image> images = new List<UnityEngine.UI.Image>();
    [SerializeField] private int correctAnswerIndex = 0;
    [SerializeField] private bool useTextBackGround = false;
    [SerializeField] private UnityEngine.UI.Image textBackGroundImage;
    [SerializeField] private bool alternativesDependingOfMarker=true;
    [SerializeField] private Button resetButton;
    public Button ResetButton{get{return resetButton;}set{resetButton = value;}}
    [SerializeField] private bool useNextQuestionButton = false;
    [SerializeField] private Button nextQuestionButton;
    [SerializeField] private GameObject nextQuestion;
    [SerializeField] private bool usePreviousQuestionButton = false;
    [SerializeField] private Button previousQuestionButton;
    [SerializeField] private GameObject previousQuestion;
    [SerializeField] private bool randomOptions = true;
    [SerializeField] private UnityEngine.UI.Image correctAnswerImage;
    public UnityEngine.UI.Image CorrectAnswerImage { get { return correctAnswerImage; } set { correctAnswerImage = value; } }
    [SerializeField] private UnityEngine.UI.Image wrongAnswerImage;
    public UnityEngine.UI.Image WrongAnswerImage { get { return wrongAnswerImage; } set { wrongAnswerImage = value; } }
    [SerializeField] private bool useTime = true;
    [SerializeField] private bool timeDependingOfMarker = false;//permite definir se quer que o tempo esteja vinculado com o marcador
    [SerializeField] private int totalTime = 15;//tempo maximo em segundos
    private float guiTime;
    private float starTime = 0f;
    private float countDown;
    private Color originalColor;
    private bool stopTime = false;
    [SerializeField] private Text timeText;
    public Text TimeText {get{return timeText;}set{timeText = value;}}
    [SerializeField] private bool useAlertTime = true;
    [SerializeField] private int alertTime = 7;//tempo no qual vai se trocar a cor o texto de tempo para alertar fim do tempo
    [SerializeField] private Color newTextColor = Color.red;
    [SerializeField] private GameObject[] additionalElements0 = new GameObject[0];//elementos adicionais estado 0, ativos enquanto a pergunta esteja ativa (ou controller ativo), ou seja ativo em todos os estados.
    [SerializeField] private GameObject[] additionalElements1 = new GameObject[0];//elementos adicionais estado1, pergunta está sendo respondida
    public GameObject[] AdditionalElements1{get{return additionalElements1;}set{additionalElements1 = value;}}
    [SerializeField] private GameObject[] additionalElements2 = new GameObject[0];//elementos adicionais estado2, após responder a pergunta
    [SerializeField] private bool useAnswerButton = true;
    [SerializeField] private Button answerButton;
    public Button AnswerButton{get{return answerButton;}set{answerButton = value;}}
    [SerializeField] private bool useDefaultAnswer = true;
    [SerializeField] private UnityEngine.UI.Image correctAnswerImage2;//image colocado sobre o texto ou imagem alternativa para indicar a resposta certa
    public UnityEngine.UI.Image CorrectAnswerImage2{get{return correctAnswerImage2;}set{correctAnswerImage2 = value;}}
    [SerializeField] private GameObject[] additionalElements3 = new GameObject[0];//elementos adicionais estado3, após responder a pergunta e quando se ve resposta certa  
    public GameObject[] AdditionalElements3{get{return additionalElements3;}set{additionalElements3 = value;}}
    private Graphic correctAnswerObj;
    private bool questionAnswered = false;//variavel usada para indicar se a pergunta foi respondida ou nao
    private float stopTimeValue = 0;//stopTimeValue é uma variavel usada para que após pausar o tempo, entao ao ser despausado, possa continuar a contagem no valor no qual tinha sido pausado
    private bool paused = false;
    private bool firstTime = true;
    private int originalTotalTime;
    private bool inscresedTotalTime = false;
    //ATRIBUTOS RELACIONADOS COM RESET TRANSFORM OF THE ELEMENTS
    private Vector3[] questionObjectsOriginalPosition;
    private Quaternion[] questionObjectsOriginalRotation;
    private Vector3[] questionObjectsOriginalScale;
    private Vector3[] additionalElements0OriginalPosition;
    private Quaternion[] additionalElements0OriginalRotation;
    private Vector3[] additionalElements0OriginalScale;
    private Vector3[] additionalElements1OriginalPosition;
    private Quaternion[] additionalElements1OriginalRotation;
    private Vector3[] additionalElements1OriginalScale;
    private Vector3[] additionalElements2OriginalPosition;
    private Quaternion[] additionalElements2OriginalRotation;
    private Vector3[] additionalElements2OriginalScale;
    private Vector3[] additionalElements3OriginalPosition;
    private Quaternion[] additionalElements3OriginalRotation;
    private Vector3[] additionalElements3OriginalScale;
    //ATRIBUTOS RELACIONADOS COM RESET TRANSFORM SUBSTITUTES OBJECTS
    [SerializeField] private List<GameObject> substituteObjects= new List<GameObject>();
    private Vector3[] substituteObjectsOriginalPosition;
    private Quaternion[] substituteObjectsOriginalRotation;
    private Vector3[] substituteObjectsOriginalScale;
    [SerializeField] private List<GameObject> substituteObjectsOfQuestionObjects = new List<GameObject>();
    private Vector3[] substituteObjectsOfQuestionObjectsOriginalPosition;
    private Quaternion[] substituteObjectsOfQuestionObjectsOriginalRotation;
    private Vector3[] substituteObjectsOfQuestionObjectsOriginalScale;
    //ATRIBUTOS RELACIONADOS COM RESET SCALE de emty objects
    private List<GameObject> originalObjects = new List<GameObject>();
    private List<GameObject> emtyObjects = new List<GameObject>();
    //---
    [SerializeField] private ActivateInformation[] activateInformationComponents;//armazena os componentes ActivateInformation, usados para indicar-lhes quando esta se mudando de uma pergunta a outra. isto é necessario para que no frame no qual se está mudando de pergunta, nao se execute o codigo de ativar linhas e info, evitando ativacoes em ordem random delas que podem ocorrer. portanto nesse frame se modifica a variavel isChangingQuestion de cada ActivateInformation a true.
    [SerializeField] private ChangeElement[] changeElementComponents;//RELACIONADO COM:RESET VALORES DE COMPONENTES ChangeElement. ajustar valores de ChangeElement para permitir que após fazer reset na pergunta, e quando marcadores seja isolados, entao o objeto possa voltar ao seu valor original devido a que já foi feito reset (e nao o ultimo valor guardado obtido quando os marcadores tinham ficado juntos).
    //ATRIBUTOS RELACIONADOS COM RESET COR
    private List<GameObject> allObjects = new List<GameObject>();//lista que vai armazenar o total de objetos sem repetir (unicamente adicional elements sem repetir)
    [SerializeField] private List<GameObject> allObjectsWithRenderer = new List<GameObject>();//lista com objetos que tem componentes Renderer, ou seja objetos que tem materiais.
    [SerializeField] private List<ObjectContainerWithRenderer> objectsContainerWithRenderer = new List<ObjectContainerWithRenderer>();//estrutura para guardar objeto, seu componente renderer (necessario para buscá), e materiais. 
    //---
    private NewTrackableEventHandler markerNewTrackableEventHandlerComponent;
    //ATRIBUTOS RELACIONADOS COM MENSAGEM DE ERRO SEVERAL QUESTIONCONTROLLER ACTIVE
    private QuestionController1[] questionController1Components;
    private QuestionController2[] questionController2Components;

    private void SaveObjectsOfArrayInAllObjects(GameObject[] vect)//CODIGO RELACIONADO COM: RESET COR, armazernar no vector allObjects todos os objetos sem repetir (para esta classe, objetos pergunta + adicional elements sem repetir)
    {
        foreach (GameObject obj in vect)
        {
            if (!allObjects.Contains(obj))//se objeto nao esta na lista.
            {
                allObjects.Add(obj);
            }
        }
    }
    private void InitializeVectObjectsContainerWithRenderer()//CODIGO RELACIONADO COM: RESET COR, inicializar objectsContainerWithRenderer, e enche o vector allObjectsWithRenderer com os objetos de allObject que tenham componente Renderer
    {
        for (int i = 0; i < allObjects.Count; i++)
        {
            Renderer[] componentsRenderer = allObjects[i].GetComponentsInChildren<Renderer>();
            if (componentsRenderer.Length > 0)//objeto tem componente Renderer
            {
                allObjectsWithRenderer.Add(allObjects[i]);//acrescenta os elementos com Renderer numa lista

                foreach (Renderer rendererT in componentsRenderer)
                {
                    Material[] materialsT = rendererT.materials;
                    objectsContainerWithRenderer.Add(new ObjectContainerWithRenderer(allObjects[i], rendererT, materialsT));//armazena para cada objeto, na estrura objectsContainerWithRenderer, o mesmo objeto associado usado como identificador, seu componente Renderer(usado para buscá) e uma copia do vector dos materiais do componente. 
                }
            }
        }
    }
    private void ResetObjectsColor(List<GameObject> objectList)//CODIGO RELACIONADO COM: RESET COR, fazer reset nos materias dos objetos duma lista (NOTA: materias sao elementos que controlam a cor dos objetos)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            for (int y = 0; y < objectsContainerWithRenderer.Count; y++)
            {
                if (UnityEngine.Object.ReferenceEquals(objectList[i],objectsContainerWithRenderer[y].ObjectWithRenderer))//se tem o mesmo indentificador, ou seja o ObjectContainerWithRenderer é do objeto atual.
                {
                    Renderer actualComponentRenderer = objectsContainerWithRenderer[y].ComponentRenderer;//obtem o componente renderer original,que sera usado para buscá
                    Material[] originalMaterials = objectsContainerWithRenderer[y].MaterialsVector;//obtem as copias armazenados do vector materials original

                    Renderer[] componentsRenderer = objectList[i].GetComponentsInChildren<Renderer>();
                    foreach (Renderer rendererT in componentsRenderer)
                    {
                        if (UnityEngine.Object.ReferenceEquals(rendererT,actualComponentRenderer))//se os componentes Renderer sao o mesmo
                        {
                            rendererT.materials = originalMaterials;//reset materiais ao original guardado
                            continue;
                        }
                    }
                }
            }
        }
    }

    private void Awake()
    {
        //MENSAGEM DE ERRO SEVERAL QUESTIONCONTROLLER ACTIVE
        questionController1Components = GameObject.FindObjectsOfType<QuestionController1>();
        questionController2Components = GameObject.FindObjectsOfType<QuestionController2>();
        bool severalQuestionControllersActive = false;
        if (Active)
        {
            foreach (QuestionController1 questionController1 in questionController1Components)
            {
                if (!UnityEngine.Object.ReferenceEquals(questionController1,this) && (questionController1.Active == true))
                {
                    Debug.LogError("AR Educational Framework Message: Several QuestionController component have Active in true. The QuestionControllers in the objects: " + this.gameObject.name + " and " + questionController1.gameObject.name + ", have Active in true. only one QuestionController must have Active in true.");
                    severalQuestionControllersActive = true;
                }
               
            }
            foreach (QuestionController2 questionController2 in questionController2Components)
            {
                if (questionController2.Active == true)
                {
                    Debug.LogError("AR Educational Framework Message: Several QuestionController component have Active in true. The QuestionControllers in the objects: " + this.gameObject.name + " and " + questionController2.gameObject.name + ", have Active in true. only one QuestionController must have Active in true.");
                    severalQuestionControllersActive = true;
                }
            }

        }
        //MENSAGEM DE ERRO ESTAO SENDO REPETIDAS ALTERNATIVAS DE RESPOSTA EM VARIOS QuestionController1(no caso, texts ou images)
        bool reusingAnyAnswerAlternative = false;
        foreach (QuestionController1 questionController1 in questionController1Components)
        {
            if (!UnityEngine.Object.ReferenceEquals(questionController1,this))
            {
                if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Texts)
                {
                    if (questionController1.typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Texts)
                        foreach (Text text in Texts)
                        {
                            if (questionController1.Texts.Contains(text))//confere se algum texto de alternativa deste QuestionController1 for usado em outros QuestionController1 que também usam textos de alternativa
                            {
                                Debug.LogError("AR Educational Framework Message: Several QuestionController components are reusing same Texts of alternatives. The QuestionControllers in the objects: " + this.gameObject.name + " and " + questionController1.gameObject.name + ", are reusing the Text: " + text.name + ". reusing Texts of alternatives is not supported, you must criate new Texts for each Questioncontroller.");
                                reusingAnyAnswerAlternative = true;
                            }
                        }
                }
                else if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Images)
                {
                    if (questionController1.typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Images)
                        foreach (UnityEngine.UI.Image image in images)
                        {
                            if (questionController1.images.Contains(image))//confere se alguma imagem de alternativa deste QuestionController1 for usada em outros QuestionController2 que também usam imagens de alternativa
                            {
                                Debug.LogError("AR Educational Framework Message: Several QuestionController components are reusing same Images of alternatives. The QuestionControllers in the objects: " + this.gameObject.name + " and " + questionController1.gameObject.name + ", are reusing the Image: " + image.name + ". reusing Images of alternatives is not supported, you must criate new Images for each Questioncontroller.");
                                reusingAnyAnswerAlternative = true;
                            }
                        }
                }
            }
        }
        //---
        if (severalQuestionControllersActive || reusingAnyAnswerAlternative)
        {
            this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
            return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
        }

        activateInformationComponents = GameObject.FindObjectsOfType<ActivateInformation>(); 
        changeElementComponents = GameObject.FindObjectsOfType<ChangeElement>();//CODIGO RELACIONADO COM: RESET VALORES DE ChangeElement

        //MENSAGEM ERRO MARKER SEM COMPONENTE NewTrackableEventHandler
        if (useMarker)
        {
            markerNewTrackableEventHandlerComponent = Marker.GetComponent<NewTrackableEventHandler>();

            if (markerNewTrackableEventHandlerComponent == null)
            {
                Debug.LogError("AR Educational Framework Message: The ImageTarget: " + Marker.name + " doesn't have NewTrackableEventHandler component. you need to substitute DefaultTrackableEventHandler component for it.");
                this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
                return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
            }                            
        }

        //MENSAGEM ERRO OBJECT IS NOT CHILD OF MARKER
        if (useMarker && useQuestionObjects)
        {
            bool anyQuestionObjectIsNotChildOfMarker = false;
            for (int i = 0; i < QuestionObjects.Count; i++)
            {
                if (!QuestionObjects[i].transform.IsChildOf(Marker.transform))
                {
                    Debug.LogError("AR Educational Framework Message: The ImageTarget: " + Marker.name + " doesn't have child " + QuestionObjects[i].name + " in the Unity hierarchy. you need to make the object: " + QuestionObjects[i].name + ", child of marker: " + Marker.name + ".");
                    anyQuestionObjectIsNotChildOfMarker = true;
                }
            }
            if (anyQuestionObjectIsNotChildOfMarker)
            {
                this.enabled = false;// desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
                return;// nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.           
            }
        }

        //ACRESCENTA COMPONENTES OnClickElement NOS ELEMENTOS Texts ou Images, NOTA: este componente é necessario para permitir reconhecer o evento On click nos elementos, o qual sera utilizado nesta classe.
        if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Texts)
        {
            foreach (Text text in Texts)
            {
                if (text.GetComponent<OnClickElement>() == null)//só se o elemento text nao tiver o componente OnClickElement, entao acrescenta ele. (nota-se que normalemte nao deveria ter esse componente e seria acrescentado aqui, porém se já for acrescentado manualmente pelo usuario, nao terá problema pois nao será mais acrescentado)
                {
                    text.gameObject.AddComponent<OnClickElement>();
                }
            }
        }
        else if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Images)
        {
            foreach(UnityEngine.UI.Image image in images)
            {
                if (image.GetComponent<OnClickElement>() == null)//só se o elemento image nao tiver o componente OnClickElement, entao acrescenta ele. (nota-se que normalemte nao deveria ter esse componente e seria acrescentado aqui, porém se já for acrescentado manualmente pelo usuario, nao terá problema pois nao será mais acrescentado)
                {
                    image.gameObject.AddComponent<OnClickElement>();
                }          
            }
        }

        //CODIGO RELACIONADO COM: RESET COR
        if (useMarker && useQuestionObjects)
        {
            SaveObjectsOfArrayInAllObjects(QuestionObjects.ToArray());
        }
        SaveObjectsOfArrayInAllObjects(additionalElements0);
        SaveObjectsOfArrayInAllObjects(AdditionalElements1);
        SaveObjectsOfArrayInAllObjects(additionalElements2);
        SaveObjectsOfArrayInAllObjects(AdditionalElements3);

        //CODIGO RELACIONADO COM: RESET TRANSFORM SUBSTITUTES OBJECTS, obtem todos os elementos substitutos(para esta pergunta) sem repetir na lista substituteObjects, fazendo buscá apartir da lista allObjects
        ChangeElement[] changeElementsT = GameObject.FindObjectsOfType<ChangeElement>();
        foreach (GameObject obj in allObjects)
        {
            foreach (ChangeElement changeElement in changeElementsT)
            {
                if (changeElement.TypeOfChange == ChangeElement.TypeOfChanges.ChangeObject)//componente changElement esta trocando o objeto atual por outro
                {
                    if (UnityEngine.Object.ReferenceEquals(obj,changeElement.MyObject))
                    {
                        if (!substituteObjects.Contains(changeElement.NewObject))
                            substituteObjects.Add(changeElement.NewObject);//encher lista substituteObjects de todos os objetos subtitutos

                        if (useMarker && useQuestionObjects)
                        {
                            if (QuestionObjects.Contains(obj))//se o objeto atual é um objeto pergunta
                            {
                                if (!substituteObjectsOfQuestionObjects.Contains(changeElement.NewObject))
                                    substituteObjectsOfQuestionObjects.Add(changeElement.NewObject);//encher lista substituteObjectsOfQuestionObjects dos objetos substitutos que substituem objetos pergunta.
                            }
                        }
                    }
                }
            }
        }

        //CODIGO RELACIONADO COM: RESET COR SUBSTITUTES OBJECTS
        SaveObjectsOfArrayInAllObjects(substituteObjects.ToArray());//acrescenta também os elementos substitutos no vector allObjects
    }

    // Use this for initialization
    void Start() {

        //CODIGO RELACIONADO COM: RESET SCALE emty objects
        foreach (GeralObjectsContainer geralObj in ButtonOperations.GeralObjectsContainerList)//obtem em listas os objetos gerais e seus objetos emptys respetivos que vao ser escalados por botoes ou por marcadores de controle
        {
            originalObjects.Add(geralObj.OriginalObject);
            emtyObjects.Add(geralObj.EmtyObject);
        }

        //CODIGO RELACIONADO COM: RESET COR
        InitializeVectObjectsContainerWithRenderer();

        //CODIGO RELACIONADO COM: RESET TRANSFORM OF THE ELEMENTS
        //objetos pergunta
        if (useMarker && useQuestionObjects)
        {
            questionObjectsOriginalPosition = new Vector3[QuestionObjects.Count];
            questionObjectsOriginalRotation = new Quaternion[QuestionObjects.Count];
            questionObjectsOriginalScale = new Vector3[QuestionObjects.Count];
            SaveOriginalTransformOfElements(QuestionObjects.ToArray(), questionObjectsOriginalPosition, questionObjectsOriginalRotation, questionObjectsOriginalScale);
        }
        //elementos adicionais
        additionalElements0OriginalPosition = new Vector3[additionalElements0.Length];
        additionalElements0OriginalRotation = new Quaternion[additionalElements0.Length];
        additionalElements0OriginalScale = new Vector3[additionalElements0.Length];
        SaveOriginalTransformOfElements(additionalElements0, additionalElements0OriginalPosition, additionalElements0OriginalRotation, additionalElements0OriginalScale);

        additionalElements1OriginalPosition = new Vector3[AdditionalElements1.Length];
        additionalElements1OriginalRotation = new Quaternion[AdditionalElements1.Length];
        additionalElements1OriginalScale = new Vector3[AdditionalElements1.Length];
        SaveOriginalTransformOfElements(AdditionalElements1, additionalElements1OriginalPosition, additionalElements1OriginalRotation, additionalElements1OriginalScale);

        additionalElements2OriginalPosition = new Vector3[additionalElements2.Length];
        additionalElements2OriginalRotation = new Quaternion[additionalElements2.Length];
        additionalElements2OriginalScale = new Vector3[additionalElements2.Length];
        SaveOriginalTransformOfElements(additionalElements2, additionalElements2OriginalPosition, additionalElements2OriginalRotation, additionalElements2OriginalScale);

        additionalElements3OriginalPosition = new Vector3[AdditionalElements3.Length];
        additionalElements3OriginalRotation = new Quaternion[AdditionalElements3.Length];
        additionalElements3OriginalScale = new Vector3[AdditionalElements3.Length];
        SaveOriginalTransformOfElements(AdditionalElements3, additionalElements3OriginalPosition, additionalElements3OriginalRotation, additionalElements3OriginalScale);

        //CODIGO RELACIONADO COM: RESET TRANSFORM SUBSTITUTES OBJECTS
        if (substituteObjects.Count > 0)//se tem objetos substituindo algum dos objetos vinculados com a pergunta
        {
            substituteObjectsOriginalPosition = new Vector3[substituteObjects.Count];
            substituteObjectsOriginalRotation = new Quaternion[substituteObjects.Count];
            substituteObjectsOriginalScale = new Vector3[substituteObjects.Count];
            SaveOriginalTransformOfElements(substituteObjects.ToArray(), substituteObjectsOriginalPosition, substituteObjectsOriginalRotation, substituteObjectsOriginalScale);
        }
        if (useMarker && useQuestionObjects)
        {
            if (substituteObjectsOfQuestionObjects.Count > 0)//se tem objetos substituindo algum objeto pergunta (objeto sobre o qual vai ser feita a pergunta)
            {
                substituteObjectsOfQuestionObjectsOriginalPosition = new Vector3[substituteObjectsOfQuestionObjects.Count];
                substituteObjectsOfQuestionObjectsOriginalRotation = new Quaternion[substituteObjectsOfQuestionObjects.Count];
                substituteObjectsOfQuestionObjectsOriginalScale = new Vector3[substituteObjectsOfQuestionObjects.Count];
                SaveOriginalTransformOfElements(substituteObjectsOfQuestionObjects.ToArray(), substituteObjectsOfQuestionObjectsOriginalPosition, substituteObjectsOfQuestionObjectsOriginalRotation, substituteObjectsOfQuestionObjectsOriginalScale);
            }
        }

        if (useTime)
        {
            if (useAlertTime) originalColor = TimeText.color; 
            if(useMarker && timeDependingOfMarker) originalTotalTime = totalTime;//usado para ajustar totalTime dependendo de se inicia o markador vissivel ou nao vissivel
        }

        if (randomOptions)
        {
            if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Texts)
            {
                correctAnswerObj = Texts[correctAnswerIndex];//obtem o objeto resposta antes de reajustar a lista random
            }
            else if(typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Images)
            {
                correctAnswerObj = images[correctAnswerIndex];//obtem o objeto resposta antes de reajustar a lista random
            }         
        }

            ResetButton.onClick.AddListener(ResetButtonMethod); 

        if (useNextQuestionButton)
        {

            nextQuestionButton.onClick.AddListener(delegate { StartCoroutine(ChangeQuestion(nextQuestion));});
        }

        if (usePreviousQuestionButton)
        {
            previousQuestionButton.onClick.AddListener(delegate { StartCoroutine(ChangeQuestion(previousQuestion)); });
        }
        if (useAnswerButton)
        {
            AnswerButton.onClick.AddListener(AnswerButtonMethod);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //NOTA: diferente de QuestionController2, nesta abordagem nao se precisa a condicao if(Active) para a execuacao dos texts3D ou images pois neste caso devem ser sempre diferentes para cada pergunta (para cada QuestionController1)
        if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Texts)
        {
            if (!questionAnswered)
            {
                //DESATIVAR E ATIVAR OS TEXTS ALTERNATIVA DEPENDENDO DE SE MARCADOR ESTÁ VISSIVEL OU NAO. NOTA: JEITO DE GERENCIAR OS TEXTOS ALTERNATIVAS QUANDO SE USA MARCADOR E alternativesDependingOfMarker É TRUE, NAO PRECISANDO ATIVAR ATRAVES DO METODO ResetButtonMethod
                if (Active && useMarker && alternativesDependingOfMarker)
                {    
                        if (markerNewTrackableEventHandlerComponent.MarkerFound)//marcador vissivel
                        {
                            EnableListElements<Text>(Texts);

                            if(useTextBackGround)
                            textBackGroundImage.gameObject.SetActive(true);
                        }
                        else//marcador nao vissivel
                        {
                            DisableListElements<Text>(Texts);

                            if(useTextBackGround)
                            textBackGroundImage.gameObject.SetActive(false);
                        }
                }
                //CONFERIR RESPOSTA 
                for (int i = 0; i < Texts.Count; i++)
                {
                    if (Texts[i].GetComponent<OnClickElement>().OnClick)//conferir se resposta certa ou errada, quando algum dos Texts foi hundido e portanto o valor onClick do componente OnClickElement do Text esta true.
                    {
                        CheckMethod<Text>(Texts[i], Texts);//verificar se o Text é a alternativa certa 
                        Texts[i].GetComponent<OnClickElement>().OnClick = false;//reset seu valor onclick a seu estado original
                    }
                }         
            }
        }
        else if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Images)
        {
            if (!questionAnswered)
            {
                //DESATIVAR E ATIVAR AS IMAGES ALTERNATIVA DEPENDENDO DE SI MARCADOR ESTÁ VISSIVEL OU NAO. NOTA: JEITO DE GERENCIAR AS IMAGENS ALTERNATIVAS QUANDO SE USA MARCADOR E alternativesDependingOfMarker É TRUE, NAO PRECISANDO ATIVAR ATRAVES DO METODO ResetButtonMethod
                if (Active && useMarker && alternativesDependingOfMarker)
                {
                    if (markerNewTrackableEventHandlerComponent.MarkerFound)//marcador vissivel
                    {
                        EnableListElements<UnityEngine.UI.Image>(images);
                    }
                    else//marcador nao vissivel
                    {
                        DisableListElements<UnityEngine.UI.Image>(images);
                    }
                }
                //CONFERIR RESPOSTA 
                for (int i = 0; i < images.Count; i++)
                {
                    if (images[i].GetComponent<OnClickElement>().OnClick)//conferir se resposta certa ou errada, quando alguma das images foi hundida e portanto o valor onClick do componente OnClickElement da image esta true
                    {
                        CheckMethod<UnityEngine.UI.Image>(images[i], images);//verificar se a image é a alternativa certa 
                        images[i].GetComponent<OnClickElement>().OnClick = false;//reset seu valor valor onclick a seu estado original
                    }
                }
            }
        }
            
        if (!Active)
        {
             if (firstFrameDesactive)
             {
                //DISABLE ELEMENTS
                 DisableElements();
                 firstFrameDesactive = false;
                 firstFrameActive = true;
             }
        }
     
    }

    private void LateUpdate()
    {
        if (Active)
        {
            if (firstFrameActive)
            {
                //ENABLE ELEMENTS
                ResetButtonMethod();
                firstFrameActive = false;
                firstFrameDesactive = true;
            }

            if (useTime)
            {
                //CODIGO RELACIONADO COM: PAUSAR TEMPO
                //usado para ajustar o valor original do totalTime em +1 ou deixar seu valor original, dependendo de se inicia o markador nao vissivel ou vissivel. usa-se para consertar defeito de relogio em nao mostrar o valor inicial certinho com o qual comeca.
                if (useMarker && timeDependingOfMarker)
                {
                    if (!markerNewTrackableEventHandlerComponent.MarkerFound && firstTime)//se o marcador nao está vissivel no primeiro frame, nesse caso já nao deve testar a proxima condicao até ser reseteado o tempo, e o totalTime fica no valor original sem nenhum aumento. este caso é usado quando precisa-se mostrar o tempo maximo exato pois vai ficar pausado no comeco.
                    {
                        firstTime = false;
                    }
                    else if (markerNewTrackableEventHandlerComponent.MarkerFound && firstTime)//marcador está vissivel no primeiro frame, nesse caso precisa-se aumentar o totalTime para garantir mostrar o valor maximo do tempo. este caso é usado quando precisa-se mostrar o tempo maximo +1 , pois se for mostrado o tempo sem o aumento, entao nunca seria visto o tempo maximo no relogio. 
                    {
                        totalTime++;//aumentar +1 o tempo total
                        firstTime = false;
                    }
                }
                else if (!inscresedTotalTime)//nao se está usando marcador e nao tem executado este codigo ainda. o valor aumenta em 1 para fazer sempre vissivel o maximo valor de totalTime
                {
                    totalTime++;
                    inscresedTotalTime = true;
                }

                CountDown();

                //CODIGO RELACIONADO COM: PAUSAR TEMPO
                //usado para pausar e despausar o tempo dependendo se o marcador for vissivel ou nao
                if (useMarker && timeDependingOfMarker)//vai ser usado marcador
                {
                    if (!questionAnswered)
                    {
                        if (!markerNewTrackableEventHandlerComponent.MarkerFound && !paused)//se marcador nao for vissivel, e tempo nao esta pausado ainda, entao PAUSAR TEMPO
                        {
                            //PAUSAR TEMPO
                            stopTime = true;
                            paused = true;
                        }
                        else if (markerNewTrackableEventHandlerComponent.MarkerFound && paused)//se marcador esta vissivel, e tempo esta pausado, entao DESPAUSAR TEMPO
                        {
                            //DESPAUSAR TEMPO
                            starTime = Time.timeSinceLevelLoad;
                            stopTimeValue = guiTime;//stopTimeValue se coloca em guiTime para permitir que o contagem possa continuar desde o valor onde foi pausado 
                            stopTime = false;
                            paused = false;
                        }
                    }
                }
            }
        }
    }

    private void SaveOriginalTransformOfElements(GameObject[] elements, Vector3[] elementsOriginalPosition, Quaternion[] elementsOriginalRotation, Vector3[] elementsOriginalScale)//armazenar os valores inicias de transform(localposition,localrotation,localscale) dos elementos para fazer o reset quando for necesserio
    {
        for (int i = 0; i < elements.Length; i++)
        {
            elementsOriginalPosition[i] = elements[i].transform.localPosition;
            elementsOriginalRotation[i] = elements[i].transform.localRotation;
            elementsOriginalScale[i] = elements[i].transform.localScale;
        }
    }

    private void ResetElementsTransform(GameObject[] elements, Vector3[] elementsOriginalPosition,Quaternion[] elementsOriginalRotation,Vector3[] elementsOriginalScale)//usado para reset tranform de elementos
    {
        //CODIGO RELACIONADO COM: RESET TRANSFORM OF THE ELEMENTS
        for (int i = 0; i < elements.Length; i++)
        {
                elements[i].transform.localPosition = elementsOriginalPosition[i];
                elements[i].transform.localRotation = elementsOriginalRotation[i];
                elements[i].transform.localScale = elementsOriginalScale[i];

                //CODIGO RELACIONADO COM: RESET SCALE emty objects
                //reset a escala do objeto empty intermediario usado para escalar no caso de existir, NOTA: Lembre-se que quando se escala um objeto atraves das funcionalidades do framework(botoes escalar, ou marcadores de controler), o objeto vai ser escalado atraves dum objeto empty criado e colocado como pai do objeto ao inves de escalar o objeto diretamente, isso foi feito para permitir escalar objeto de jeito proporcional sem se sobrepor ao marcador.
                for (int y = 0; y < originalObjects.Count; y++)
                {
                    if (UnityEngine.Object.ReferenceEquals(elements[i],originalObjects[y]))//se o objeto atual está no vector geral de objetos a serem escalados pelas funcionalidades framework (ou seja pelos botoes de escalar ou marcadores de controle), e portanto deve ter associado um objeto empty intermediario o qual é escalado ao inves do proprio objeto
                    {
                        emtyObjects[y].transform.localScale = Vector3.one;//reset escala do objeto empty intermediario associado
                    }
                }
        }
    }

    private void ResetButtonMethod()//botao de reset pergunta, se usa quando a pergunta é inicializada ou quando se pressiona o botao de reset questao  
    {
        if (Active)//NOTA:esta condicao é necessaria aqui unicamente divido a que o metodo também pode ser executado atraves do botao de reset questao que pode ser um botao reusado em varios QuestionController, e nesse caso deve ser testada a condicao
        {

            //CODIGO RELACIONADO COM: RESET TRANSFORM OF THE ELEMENTS
            ResetElementsTransform(additionalElements0, additionalElements0OriginalPosition, additionalElements0OriginalRotation, additionalElements0OriginalScale);//reset os elementos additionalElements0 a seus valores iniciais, isto pois gameobjects 3D podem ter mudado estes valores.
            ResetElementsTransform(AdditionalElements1, additionalElements1OriginalPosition, additionalElements1OriginalRotation, additionalElements1OriginalScale);//reset os elementos additionalElements1 a seus valores iniciais, isto pois gameobjects 3D podem ter mudado estes valores.
            ResetElementsTransform(additionalElements2, additionalElements2OriginalPosition, additionalElements2OriginalRotation, additionalElements2OriginalScale);//reset os elementos additionalElements2 a seus valores iniciais, isto pois gameobjects 3D podem ter mudado estes valores.
            ResetElementsTransform(AdditionalElements3, additionalElements3OriginalPosition, additionalElements3OriginalRotation, additionalElements3OriginalScale);//reset os elementos additionalElements3 a seus valores iniciais, isto pois gameobjects 3D podem ter mudado estes valores.

            //desativar elementos estado2 e 3, NOTA: em alguns casos este metodo ira desativar elementos que puderam ter sido já desativados (por exemplo no caso de estar no estado 3, já foram desativados: answerButton, elementos adicionais2 e imagens feedback de resposta certa e errada etc)
            ResetButton.gameObject.SetActive(false);
            if (useNextQuestionButton) nextQuestionButton.gameObject.SetActive(false);
            if (usePreviousQuestionButton) previousQuestionButton.gameObject.SetActive(false);
            //desativar elementos estado2
            if (useAnswerButton) AnswerButton.gameObject.SetActive(false);
            foreach (GameObject obj in additionalElements2)
            {
                obj.SetActive(false);
            }
            //desativar images de feedback resposta certa e errada estado2 (alguma das duas deveria estar ativada)
            CorrectAnswerImage.gameObject.SetActive(false);
            WrongAnswerImage.gameObject.SetActive(false);
            //desativa elementos estado3
            if (useAnswerButton)
            {
                foreach (GameObject obj in AdditionalElements3)
                {
                    obj.SetActive(false);
                }
                if (useDefaultAnswer)
                    CorrectAnswerImage2.gameObject.SetActive(false);
            }

            //ativa elementos adicionais estado 0
            foreach (GameObject obj in additionalElements0)
            {
                obj.SetActive(true);
            }

            //ativar elementos estado1 
            foreach (GameObject obj in AdditionalElements1)//ativa elementos adicionais estado1
            {
                obj.SetActive(true);
            }

            questionAnswered = false;

            if (useTime)
            {
                //reset o tempo
                starTime = Time.timeSinceLevelLoad;//starTime inicia em 0, mas é usado para fazer reset a conta do tempo
                stopTime = false;
                if (useMarker && timeDependingOfMarker)
                {
                    //CODIGO RELACIONADO COM: PAUSAR TEMPO
                    stopTimeValue = 0;//reset esta variavel a 0 permite que ainda tendo pausado, e portanto mudado este valor, possa voltar a seu valor inicial quando se faz reset ao tempo.
                    paused = false;
                    firstTime = true;
                    totalTime = originalTotalTime;//necessario quando se usa codigo de pausar tempo, nesse caso o totalTime poderia ter sido mudado (AUMENTAR ++1)
                }
                if (useAlertTime) TimeText.color = originalColor;
                TimeText.gameObject.SetActive(true);
            }

            //CODIGO RELACIONADO COM: RESET TRANSFORM OF THE ELEMENTS (objetos pergunta)
            if(useMarker && useQuestionObjects)
            ResetElementsTransform(QuestionObjects.ToArray(), questionObjectsOriginalPosition, questionObjectsOriginalRotation, questionObjectsOriginalScale);

            //CODIGO RELACIONADO COM: RESET TRANSFORM SUBSTITUTES OBJECTS.NOTA: nota-se que substituteObjects já está incluindo substituteObjectsOfQuestionObjects, e portanto é suficiente este codigo para reset transform de todos os objetos substitutos.
            if (substituteObjects.Count > 0)
                ResetElementsTransform(substituteObjects.ToArray(), substituteObjectsOriginalPosition, substituteObjectsOriginalRotation, substituteObjectsOriginalScale);

            //CODIGO RELACIONADO COM: RESET COR
            ResetObjectsColor(allObjectsWithRenderer);//neste caso reset cor todos os elementos com Renderer

            //CODIGO RELACIONADO COM: RESET VALORES DE ChangeElement
            changeElementComponents = GameObject.FindObjectsOfType<ChangeElement>();//obtem todos os componentes ChangeElement
            foreach (GameObject obj in allObjects)
            {
                foreach (ChangeElement changeElement in changeElementComponents)
                {
                    if (UnityEngine.Object.ReferenceEquals(obj,changeElement.MyObject))//se algum objeto de allObjects esta sendo mudado pelo atual componente ChangeElement
                    {
                        if (changeElement.TypeOfChange == ChangeElement.TypeOfChanges.ChangeObjectProperties)//se o componente ChangeElement está mudando propriedades do objeto
                        {
                            if (changeElement.ChangeRotation)//se o componente ChangeElement está mudando rotacao
                            {
                                GameObject myObject = changeElement.MyObject;
                                changeElement.OriginalRotation = myObject.transform.localRotation;//mudar o valor originalRotation ao valor original (lembrar que neste frame, nas linhas anteriores foi feito reset no rotacao),isto, para quando isolar os marcadores, o objeto possa voltar a este valor original devido a que já foi feito o reset, e nao ao valor inicial armazendo quando os marcadores estiveram juntos por primeira vez.
                            }
                            if (changeElement.ChangeScale)//se o componente ChangeElement está mudando escala
                            {
                                GameObject emtyObj = changeElement.EmtyObj;
                                changeElement.OriginalScaleObject = emtyObj.transform.localScale;//mudar o valor originalScaleObject ao valor original (lembrar que neste frame, nas linhas anteriores foi feito reset na escala),isto, para quando isolar os marcadores, o objeto possa voltar a este valor original devido a que já foi feito reset, e nao ao valor inicial armazenado quando os marcadores estiveram juntos por primeira vez.
                            }
                            if (changeElement.ChangeMaterials)//se o componente ChangeElement está mudando a cor (ou seja materiais)
                            {
                                changeElement.GetOriginalMaterials();//mudar os materias originais usados para fazer reset aos valores originais (lembrar que neste frame, nas linhas anteriores foi feito reset nos cores),isto, para quando isolar os marcadores, o objeto possa voltar a estes materiais originais devido a que já foi feito reset, e nao aos materiais iniciais armazenados quando os marcadores estiveram juntos por primeira vez. 
                            }
                        }
                    }
                }
            }

            //ATIVAR TEXTOS ou IMAGENS alternativa, e reset a ordem de jeito random 
            if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Texts)
            {
                if (randomOptions) ReshuffleList<Text>(Texts);//reset a ordem de jeito random
                //ATIVAR TEXTOS ALTERNATIVA
                if (!useMarker || (useMarker && !alternativesDependingOfMarker))//JEITO DE GERENCIAR TEXTOS ALTERNATIVAS QUANDO NAO SE TEM MARCADOR, OU QUANDO SE TEM PORÉM alternativesDependingOfMarker É FALSE, PRECISANDO ATIVAR ATRAVES DESTE METODO ResetButtonMethod. LEMBRE-SE QUE NESTE CASO, NAO SE EXECUTA O CODIGO QUE CONTINUAMENTE ESTA ATIVANDO E DESATIVANDO O TEXTO DEPENDENDO DO MARCADOR (cogido em update)
                {
                    EnableListElements<Text>(Texts);
                    if (useTextBackGround)
                        textBackGroundImage.gameObject.SetActive(true);
                }

                if (useDefaultAnswer)
                    for (int i = 0; i < Texts.Count; i++)
                    {
                        Texts[i].GetComponent<OnClickElement>().OnClick = false;//reset valor onclick de cada texto a seu estado original.NOTA:NECESARIO UNICAMENTE USANDO ButtonSeeAnswerd, pois como volta-se a ativar os textos após responder, entao se for hundido, seu valor onClick mudaria novamente precisando ser reiniciado (lembrar que a classe OnclickElement sempre continua ativa).
                    }
            }
            else if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Images)
            {
                if (randomOptions) ReshuffleList<UnityEngine.UI.Image>(images);//reset a ordem de jeito random
                //ATIVAR IMAGENS ALTERNATIVA
                if (!useMarker || (useMarker && !alternativesDependingOfMarker))//JEITO DE GERENCIAR IMAGENS ALTERNATIVAS QUANDO NAO SE TEM MARCADOR, OU QUANDO SE TEM PORÉM alternativesDependingOfMarker É FALSE, PRECISANDO ATIVAR ATRAVES DESTE METODO ResetButtonMethod. LEMBRE-SE QUE NESTE CASO, NAO SE EXECUTA O CODIGO QUE CONTINUAMENTE ESTA ATIVANDO E DESATIVANDO IMAGENS DEPENDENDO DO MARCADOR (cogido em update)
                {
                    EnableListElements<UnityEngine.UI.Image>(images);
                }

                if (useDefaultAnswer)
                    for (int i = 0; i < images.Count; i++)
                    {
                        images[i].GetComponent<OnClickElement>().OnClick = false;//reset valor onclick de cada imagem a seu estado original.NOTA:NECESARIO UNICAMENTE USANDO ButtonSeeAnswerd, pois como volta-se a ativar as imagens após responder, entao se for hundido, seu valor onClick mudaria novamente precisando ser reiniciado (lembrar que a classe OnclickElement sempre continua ativa).
                    }
            }

            if(useMarker && useQuestionObjects)
            foreach (GameObject obj in QuestionObjects)//ativa objetos pergunta inicialmente
            {
                obj.gameObject.SetActive(true);
            }

        }
    }

    private void EnableListElements<T>(List<T> list) where T : Graphic
    {
        foreach (T element in list)
        {
            element.gameObject.SetActive(true);
        }
    }
    private void DisableListElements<T>(List<T> list)where T:Graphic
    {
        foreach(T element in list)
        {
            element.gameObject.SetActive(false);
        }
    }

    private IEnumerator ChangeQuestion(GameObject newQuestionController)
    {
        if (Active)
        {
            yield return new WaitForEndOfFrame(); //NOTA: esta linha é necessaria por dois fatos: 1:executar unicamente o codigo do QuestionController Active em true e nao de outros quando for precionado o botao. o codigo quando este botao for precionado muda o estado de Active dos dois QuestionController, assim o segundo QuestionController poderia ser executado também quando nao é seu turno. com esta espera, se garante que primeiro sejam lidos todos o codigos do QuestionController enquanto tem seu valor Active original, finalizar esse frame, e após isso mudar os estados. 
                                                  //2: como o codigo a seguir se executa em um novo frame, entao já seu ordem de execucao fica como está estabelecido normalmente, ou seja primeiro desactivar objetos (codigo que esta em update)- e depois ativar objetos(codigo que esta em lateupdate)), nao é necessario colocar outro delay, e o comportamento será igual a como se executa por default inicialmente, quando um unico QuestionController está ativo (este se executa de ultimo para ativar objetos que podem ser reusados em QuestionController) e o resto de controller desativos (se executam primeiro para que objetos que podem ser reusados, possam ser ativados depois) 

            //AVISAR ActivateInformation, muda a variavel isChangingQuestion a true de cada componente ActivateInformation para fazer o codigo de ativar e desativar texts, images e linhas que nao se execute neste frame.
            activateInformationComponents = GameObject.FindObjectsOfType<ActivateInformation>();
            foreach (ActivateInformation activateInformationComponent in activateInformationComponents)
            {
                 activateInformationComponent.IsChangingQuestion = true;
            }

            Active = false;//desativar o QuestionController atual (da pergunta atual)

            if (newQuestionController.GetComponent<QuestionController2>() != null)//se a proxima pergunta tem um componente tipo QuestionControlle2
            {
                newQuestionController.GetComponent<QuestionController2>().Active = true;//ativar o proximo QuestionController (da proxima pergunta)
            }
            else if (newQuestionController.GetComponent<QuestionController1>() != null)//se a proxima pergunta tem também um componente tipo QuestionControlle1 (é uma pergunta do mesmo tipo do que a atual)
            {
                newQuestionController.GetComponent<QuestionController1>().Active = true;//ativar o proximo QuestionController (da proxima pergunta)
            }
        }
    }

    private void DisableElements()//desativa todos os elementos pois nao vai ser usado este QuestionController no momento 
    {
        if(useMarker && useQuestionObjects)
        foreach (GameObject obj in QuestionObjects)//desativar objetos pergunta
        {
            obj.SetActive(false);
        }
        //desativar Texts ou images alternativas
        if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Texts)
        {
            DisableListElements<Text>(Texts);
            if (useTextBackGround)
            {
                textBackGroundImage.gameObject.SetActive(false);
            }
        }
        else if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Images)
        {
            DisableListElements<UnityEngine.UI.Image>(images);
        }
        //---
        //desativar botoes
        ResetButton.gameObject.SetActive(false);
        if (useNextQuestionButton) nextQuestionButton.gameObject.SetActive(false);
        if (usePreviousQuestionButton) previousQuestionButton.gameObject.SetActive(false);
        if (useAnswerButton) AnswerButton.gameObject.SetActive(false);
        //desativar images de feedback resposta certa e errada, para o caso de estarem ativos inicialmente
        CorrectAnswerImage.gameObject.SetActive(false);
        WrongAnswerImage.gameObject.SetActive(false);

        if (useTime)//desativar texto do tempo
        {
            TimeText.gameObject.SetActive(false);
        }

        foreach (GameObject obj in additionalElements0)//desativar elementos adicionais estado 0
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in AdditionalElements1)//desativar elementos adicionais estado1 
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in additionalElements2)//desastivar elementos adicionais estado2
        {
            obj.SetActive(false);
        }
        if (useAnswerButton)
        {
            foreach (GameObject obj in AdditionalElements3)//desastivar elementos adicionais estado3
            {
                obj.SetActive(false);
            }
            if (useDefaultAnswer)
                CorrectAnswerImage2.gameObject.SetActive(false);
        }
    }

    private void AnswerButtonMethod()//pressionado botao de ver resposta certa, pasar a estado 3
    {
        if (Active)//NOTA: as condicoes if(active) antes do codigo dos botoes, garante que ainda tendo botoes reusados para varias perguntas numa mesma cena,só seja executado o codigo para a pergunta ativa nesse momento. 
        {

            //desativar elementos estado2 necessarios( todos do estado2 menos o resetQuestion,nextQuestionButton e  previousQuestionButton que sao tanto do estado2 quanto do estado3)
            AnswerButton.gameObject.SetActive(false);
            foreach (GameObject obj in additionalElements2)//desativa elementos adicionais estado2
            {
                obj.SetActive(false);
            }
            //desativar images de feedback resposta certa e errada estado2 (alguma das duas deveria estar ativada)
            CorrectAnswerImage.gameObject.SetActive(false);
            WrongAnswerImage.gameObject.SetActive(false);

            //ativar elementos estado3
            foreach (GameObject obj in AdditionalElements3)//ativar elementos adicionais estado3
            {
                obj.SetActive(true);
            }

            if (useDefaultAnswer)//vai ser usado o jeito default para mostrar a resposta
            {
                if (useMarker && useQuestionObjects)
                {
                    //CODIGO RELACIONADO COM: RESET TRANSFORM OF THE ELEMENTS (objetos pergunta)
                    ResetElementsTransform(QuestionObjects.ToArray(), questionObjectsOriginalPosition, questionObjectsOriginalRotation, questionObjectsOriginalScale);

                    //CODIGO RELACIONADO COM: RESET TRANSFORM OF substituteObjectsOfQuestionObjects (objetos sustitutos dos objetos pergunta)
                    if (substituteObjectsOfQuestionObjects.Count > 0)
                        ResetElementsTransform(substituteObjectsOfQuestionObjects.ToArray(), substituteObjectsOfQuestionObjectsOriginalPosition, substituteObjectsOfQuestionObjectsOriginalRotation, substituteObjectsOfQuestionObjectsOriginalScale);

                    //CODIGO RELACIONADO COM: RESET COR, NOTA: neste caso reset cor unicamente dos objetos pergunta
                    ResetObjectsColor(QuestionObjects);

                    //CODIGO RELACIONADO COM: RESET COR, RESET COR OF substituteObjectsOfQuestionObjects (objetos sustitutos dos objetos pergunta)
                    if (substituteObjectsOfQuestionObjects.Count > 0)
                        ResetObjectsColor(substituteObjectsOfQuestionObjects);

                    //CODIGO RELACIONADO COM: RESET VALORES DE ChangeElement (para objetos pergunta e objetos substitutos de objetos pergunta)
                    changeElementComponents = GameObject.FindObjectsOfType<ChangeElement>();//obtem todos os componentes ChangeElement 
                    List<GameObject> questionObjectsAndSubstituteObjectsOfQuestionObjects = new List<GameObject>();//lista com todos os elementos de QuestionObjects e substituteObjectsOfQuestionObjects juntos
                    foreach (GameObject obj in QuestionObjects)
                    {
                        if (!questionObjectsAndSubstituteObjectsOfQuestionObjects.Contains(obj))
                        {
                            questionObjectsAndSubstituteObjectsOfQuestionObjects.Add(obj);//acrescentamentos os objetos pergunta (QuestionObjects) na lista
                        }
                    }
                    foreach (GameObject obj in substituteObjectsOfQuestionObjects)
                    {
                        if (!questionObjectsAndSubstituteObjectsOfQuestionObjects.Contains(obj))
                        {
                            questionObjectsAndSubstituteObjectsOfQuestionObjects.Add(obj);//acrescentamos os objetos substitutos dos objetos pergunta (substituteObjectsOfQuestionObjects) na lista
                        }
                    }
                    foreach (GameObject obj in questionObjectsAndSubstituteObjectsOfQuestionObjects)
                    {
                        foreach (ChangeElement changeElement in changeElementComponents)
                        {
                            if (UnityEngine.Object.ReferenceEquals(obj,changeElement.MyObject))//se o objeto esta sendo mudado pelo atual componente ChangeElement
                            {
                                if (changeElement.TypeOfChange == ChangeElement.TypeOfChanges.ChangeObjectProperties)//se o componente ChangeElement está mudando propriedades do objeto
                                {
                                    if (changeElement.ChangeRotation)//se o componente ChangeElement está mudando rotacao
                                    {
                                        changeElement.OriginalRotation = changeElement.MyObject.transform.localRotation;//mudar o valor originalRotation ao valor original (lembrar que neste frame, nas linhas anteriores foi feito reset na rotacao),isto é feito, para quando isolar os marcadores, o objeto possa voltar a este valor original devido a que já foi feito o reset, e nao ao valor inicial armazendo quando os marcadores estiveram juntos por primeira vez.
                                    }
                                    if (changeElement.ChangeScale)//se o componente ChangeElement está mudando escala
                                    {
                                        changeElement.OriginalScaleObject = changeElement.EmtyObj.transform.localScale;//mudar o valor originalScaleObject ao valor original (lembrar que neste frame, nas linhas anteriores foi feito reset na escala),isto é feito, para quando isolar os marcadores, o objeto possa voltar a este valor original devido a que já foi feito reset, e nao ao valor inicial armazenado quando os marcadores estiveram juntos por primeira vez.
                                    }
                                    if (changeElement.ChangeMaterials)//se o componente ChangeElement está mudando cor (ou seja materiais)
                                    {
                                        changeElement.GetOriginalMaterials();//mudar os materias originais usados para fazer reset aos valores originais (lembrar que neste frame, nas linhas anteriores foi feito reset nos cores),isto é feito, para quando isolar os marcadores, o objeto possa voltar a estes materiais originais devido a que já foi feito reset, e nao aos materiais iniciais armazenados quando os marcadores estiveram juntos por primeira vez. 
                                    }
                                }
                            }
                        }
                    }

                    foreach (GameObject obj in QuestionObjects)//ativar objetos pergunta
                    {
                        obj.SetActive(true);
                    }
                }

                //ATIVAR texts ou images alternativa
                if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Texts)
                {
                    EnableListElements<Text>(Texts);
                    if (useTextBackGround)
                    {
                        textBackGroundImage.gameObject.SetActive(true);
                    }
                }
                else if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Images)
                {
                    EnableListElements<UnityEngine.UI.Image>(images);
                }
                //ativar image para indicar qual é o elemento certo, sobrepondo ele.
                CorrectAnswerImage2.gameObject.SetActive(true);
                //posicionar a imagem no mesmo lugar que a alternativa certa
                if (randomOptions)
                {
                    CorrectAnswerImage2.GetComponent<RectTransform>().anchoredPosition = correctAnswerObj.GetComponent<RectTransform>().anchoredPosition;
                }
                else
                {
                    if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Texts)
                    {
                        CorrectAnswerImage2.GetComponent<RectTransform>().anchoredPosition = Texts[correctAnswerIndex].GetComponent<RectTransform>().anchoredPosition;
                    }
                    else if(typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Images)
                    {
                        CorrectAnswerImage2.GetComponent<RectTransform>().anchoredPosition = images[correctAnswerIndex].GetComponent<RectTransform>().anchoredPosition;
                    }
                }
            }

        }
    }

    private void CheckMethod<T>(T OnClickElement,List<T> list) where T:Graphic//confirmar opcao (escolher opcao)
    {
        if (!randomOptions)//se as opcoes nao sao random
        {
            if (UnityEngine.Object.ReferenceEquals(OnClickElement, list[correctAnswerIndex]))
            {
                CorrectAnswer();
            }
            else
            {
                WrongAnswer();
            }
        }
        else //if(randomOptions)//se as opcoes sao ramdom
        {
            if (UnityEngine.Object.ReferenceEquals(OnClickElement, correctAnswerObj))
            {
                CorrectAnswer();
            }
            else
            {
                WrongAnswer();
            }
        }
        StateAnsweredQuestion();
    }

    private void StateAnsweredQuestion()//mudar ao estado pergunta respondida , estado2
    {
        questionAnswered = true;

        //desativar elementos estado1
        foreach (GameObject obj in AdditionalElements1)//desativa elementos adicionais estado1
        {
            obj.SetActive(false);
        }
        if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Texts)
        {
            DisableListElements<Text>(Texts);
            if (useTextBackGround)
            {
                textBackGroundImage.gameObject.SetActive(false);
            }
        }
        else if (typeOfAnswerAlternatives == TypeOfAnswerAlternatives.Images)
        {
            DisableListElements<UnityEngine.UI.Image>(images);
        }

        if(useMarker && useQuestionObjects)
        foreach(GameObject obj in QuestionObjects)//desativar objetos pergunta
        {
            obj.SetActive(false);
        }

        //ativar elementos estado2
        ResetButton.gameObject.SetActive(true);
        if (useNextQuestionButton) nextQuestionButton.gameObject.SetActive(true);
        if (usePreviousQuestionButton) previousQuestionButton.gameObject.SetActive(true);
        if (useAnswerButton) AnswerButton.gameObject.SetActive(true);
        foreach (GameObject obj in additionalElements2)//ativa elementos adicionais estado2
        {
            obj.SetActive(true);
        }

        if (useTime)
        {   //pausar codigo do tempo e desativar texto do tempo
            stopTime = true;
            TimeText.gameObject.SetActive(false);
        }
    }

    private void ReshuffleList<T>(List<T> list) where T : Graphic//reset de jeito random objetos duma lista, ver mais:https://forum.unity.com/threads/random-shuffle-array.86737/ 
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            Vector2 anchoredPositionTemp = temp.GetComponent<RectTransform>().anchoredPosition;
            int index = UnityEngine.Random.Range(i, list.Count);
            T other = list[index];
            Vector2 anchoredPositionOther = other.GetComponent<RectTransform>().anchoredPosition;
            list[i] = other;
            other.GetComponent<RectTransform>().anchoredPosition = anchoredPositionTemp; 
            list[index] = temp;
            temp.GetComponent<RectTransform>().anchoredPosition = anchoredPositionOther;
        }
        correctAnswerIndex = list.IndexOf((T)correctAnswerObj);//atualizar o indice a sua nova posicao uma vez que foi mudado de jeito random
    }

    private void CorrectAnswer()
    {
        CorrectAnswerImage.gameObject.SetActive(true);//ativar image feedback resposta certa
    }

    private void WrongAnswer()
    {
        WrongAnswerImage.gameObject.SetActive(true);//ativar image feedback resposta errada
    }

    private void CountDown()
    {
        if (!stopTime)
        {
            string text;
            guiTime = Time.timeSinceLevelLoad - starTime + stopTimeValue;//startTime inicia em 0, mas é usado para fazer reset a conta do tempo, stopTimeValue é uma variavel usada para que após pausar o tempo, este sendo despausado possa continuar a contagem no valor no qual foi pausado

            countDown = /*1 +*/ totalTime - guiTime;//pode ser aumentado +1 para que o primeiro valor em mostrar seja o tempo total estabelecido//-guiTime para cronometro descendente //guiTime para cronometro ascendente. NOTA:starTime é usado para fazer reset o tempo
            int minutes = Mathf.FloorToInt(countDown / 60);//minutes deve ficar com um valor enteiro, nao deve ser float
            int seconds = Mathf.FloorToInt(countDown % 60);//seconds deve ficar com um valor enteiro, nao deve ser float

            if (useAlertTime)
            {
                if (Mathf.FloorToInt(countDown) <= alertTime)
                {
                    TimeText.color = newTextColor;
                }
            }
            if (minutes == 0 && seconds <= 0)//parar execucao codigo de tempo
            {
                WrongAnswer();
                StateAnsweredQuestion();
            }
            text = String.Format("{0:00}:{1:00}", minutes, seconds);
            TimeText.text = text;
        }
    }

}
