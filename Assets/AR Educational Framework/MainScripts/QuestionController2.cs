using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionController2 : MonoBehaviour {

    [SerializeField] private bool active = true;
    public bool Active { get {return active;} set {active = value;}}
    [SerializeField] private GameObject marker;
    public GameObject Marker{get{return marker;}set{marker = value;}}
    private bool firstFrameActive = true;
    private bool firstFrameDesactive = true;
    [SerializeField] private List<GameObject>answerAlternatives= new List<GameObject>();
    public List<GameObject> AnswerAlternatives{get{return answerAlternatives;}}
    [SerializeField] private int correctAnswerIndex = 0;
    [SerializeField] private int actualIndex = 0;//indice usado para gerenciar o vector dos objetos alternativa e permitir ativar e desativar eles com botoes
    [SerializeField] private Button leftButton;
    public Button LeftButton{ get {return leftButton;} set{leftButton = value;}}
    [SerializeField] private Button rightButton;
    public Button RightButton{ get {return rightButton;} set{rightButton = value;}}
    [SerializeField] private Button checkButton;
    public Button CheckButton{ get {return checkButton;} set{checkButton = value;}}
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
    public UnityEngine.UI.Image CorrectAnswerImage{get{return correctAnswerImage;}set{correctAnswerImage = value;}}
    [SerializeField] private UnityEngine.UI.Image wrongAnswerImage;
    public UnityEngine.UI.Image WrongAnswerImage{get{return wrongAnswerImage;}set{wrongAnswerImage = value;}}
    [SerializeField] private bool useTime = true;
    [SerializeField] private bool timeDependingOfMarker = false;//permite definir se quer que o tempo esteja vinculado com o marcador 
    [SerializeField] private int totalTime = 15;//tempo maximo em segundos
    [SerializeField] private Text timeText;
    public Text TimeText{ get{return timeText;} set{timeText = value;}}
    [SerializeField] private bool useAlertTime = true;
    [SerializeField] private int alertTime = 7;//tempo no qual vai se trocar a cor o texto de tempo para alertar fim do tempo
    [SerializeField] private Color newTextColor = Color.red;
    private bool stopTime = false;
    private float guiTime;
    private float starTime = 0f;
    private float countDown;
    private Color originalColor;
    [SerializeField] private GameObject[] additionalElements0 = new GameObject[0];//elementos adicionais estado 0, ativos enquanto a pergunta esteja ativa (ou controller ativo), ou seja ativo em todos os estados.
    [SerializeField] private GameObject[] additionalElements1 = new GameObject[0];//elementos adicionais estado1, pergunta está sendo respondida
    public GameObject[] AdditionalElements1 { get{return additionalElements1;} set{additionalElements1 = value;}}
    [SerializeField] private GameObject[] additionalElements2 = new GameObject[0];//elementos adicionais estado2, após responder a pergunta
    [SerializeField] private bool useAnswerButton = true;
    [SerializeField] private Button answerButton;
    public Button AnswerButton{ get{return answerButton;} set{answerButton = value;}}
    [SerializeField] private bool useDefaultAnswer = true;
    [SerializeField] private GameObject[] additionalElements3 = new GameObject[0];//elementos adicionais estado3, após responder a pergunta e quando se ve resposta certa
    public GameObject[] AdditionalElements3 { get {return additionalElements3;} set{additionalElements3 = value;}}
    private GameObject correctAnswerObj;//objeto de resposta
    private bool questionAnswered = false;
    private float stopTimeValue = 0;//stopTimeValue é uma variavel usada para que após pausar o tempo, entao ao ser despausado, possa continuar a contagem no valor no qual tinha sido pausado
    private bool paused = true;
    private bool firstTime = true;
    private int originalTotalTime;
    private bool inscresedTotalTime = false;
    //ATRIBUTOS RELACIONADOS COM RESET TRANSFORM OF THE ELEMENTS
    private Vector3[] answerAlternativesOriginalPosition;
    private Quaternion[] answerAlternativesOriginalRotation;
    private Vector3[] answerAlternativesOriginalScale;
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
    [SerializeField] private List<GameObject> substituteObjects = new List<GameObject>();
    private Vector3[] substituteObjectsOriginalPosition;
    private Quaternion[] substituteObjectsOriginalRotation;
    private Vector3[] substituteObjectsOriginalScale;
    [SerializeField] private List<GameObject> substituteObjectsOfAnswerAlternatives = new List<GameObject>();
    private Vector3[] substituteObjectsOfAnswerAlternativesOriginalPosition;
    private Quaternion[] substituteObjectsOfAnswerAlternativesOriginalRotation;
    private Vector3[] substituteObjectsOfAnswerAlternativesOriginalScale;
    //ATRIBUTOS RELACIONADOS COM RESET SCALE emty objects
    private List<GameObject> originalObjects = new List<GameObject>();
    private List<GameObject> emtyObjects = new List<GameObject>();
    //---
    [SerializeField] private ActivateInformation[] activateInformationComponents;//armazena os componentes ActivateInformation, usados para indicar-lhes quando esta se mudando de uma pergunta a outra. isto é necessario para que no frame no qual se está mudando de pergunta, nao se execute o codigo de ativar linhas e info, evitando ativacoes em ordem random delas que podem ocorrer. portanto nesse frame se modifica a variavel isChangingQuestion de cada ActivateInformation a true.
    [SerializeField] private ChangeElement[] changeElementComponents;//RELACIONADO COM:RESET VALORES DE COMPONENTES ChangeElement. ajustar valores de ChangeElement para permitir que após fazer reset na pergunta, e quando marcadores seja isolados, entao o objeto possa voltar ao seu valor original devido a que já foi feito reset (e nao o ultimo valor guardado obtido quando os marcadores tinham ficado juntos). 
    //RESET COR
    private List<GameObject> allObjects = new List<GameObject>();//lista que vai armazenar o total de objetos sem repetir (objetos da pergunta + adicional elements sem repetir)
    [SerializeField] private List<GameObject> allObjectsWithRenderer = new List<GameObject>();//lista com objetos que tem componentes Renderer, ou seja objetos que tem materiais.
    [SerializeField] private List<ObjectContainerWithRenderer> objectsContainerWithRenderer = new List<ObjectContainerWithRenderer>();//estrutura para guardar objeto, seu componente renderer (necessario para buscá), e materiais.
    //---
    private NewTrackableEventHandler markerNewTrackableEventHandlerComponent;
    //ATRIBUTOS RELACIONADOS COM MENSAGEM DE ERRO SEVERAL QUESTIONCONTROLLER ACTIVE
    private QuestionController1[] questionController1Components;
    private QuestionController2[] questionController2Components;

    private void SaveObjectsOfArrayInAllObjects(GameObject[] vect)//CODIGO RELACIONADO COM: RESET COR, armazernar no vector allObjects todos os objetos sem repetir(para esta classe, objetos alternativa + adicional elements sem repetir)
    {
        foreach(GameObject obj in vect)
        {
            if (!allObjects.Contains(obj))//se objeto nao esta na lista.
            {
                allObjects.Add(obj);
            }
        }
    }
    private void InitializeVectObjectsContainerWithRenderer()//CODIGO RELACIONADO COM:RESET COR, inicializar objectsContainerWithRenderer, e enche o vector allObjectsWithRenderer com os objetos de allObject que tenham componente Renderer
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
                    objectsContainerWithRenderer.Add(new ObjectContainerWithRenderer(allObjects[i], rendererT, materialsT));//armazena para cada objeto na estrura objectsContainerWithRenderer, o mesmo objeto associado usado como identificador, seu componente Renderer(usado para buscá) e uma copia do vector dos materiais do componente. 
                }
            }
        }
    }
    private void ResetObjectsColor(List<GameObject> objectList)//CODIGO RELACIONADO COM: RESET COR, fazer reset nos materias dos objetos duma lista (NOTA: materias sao elementos que controlam a cor dos objetos)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            //util na abordagem QuestionController2 quando a lista passada por parametro é a lista de objetos pergunta answerAlternatives , os quais podem nao ter Renderer. permite pular na execucao diretamente os objetos que nao tem componente Renderer para evitar fazer o codigo embaixo desnecessariamente, e melhorar eficiencia. NOTA:o cogigo funciona aida sim esta condicao, pois embaixo se validará, porem deste jeito fica mais eficiente.
            Renderer[] componentsRendererO = objectList[i].GetComponentsInChildren<Renderer>();
            if (componentsRendererO.Length == 0)//objeto nao tem componente Renderer
            {
                continue;
            }
            //---
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
            foreach (QuestionController2 questionController2 in questionController2Components)
            {
                if (!UnityEngine.Object.ReferenceEquals(questionController2,this) && (questionController2.Active == true))
                {
                    Debug.LogError("AR Educational Framework Message: Several QuestionController component have Active in true. The QuestionControllers in the objects: " +this.gameObject.name+" and "+questionController2.gameObject.name+", have Active in true. only one QuestionController must have Active in true.");
                    severalQuestionControllersActive = true;
                }
           
            }
            foreach (QuestionController1 questionController1 in questionController1Components)
            {
                if(questionController1.Active == true)
                {
                    Debug.LogError("AR Educational Framework Message: Several QuestionController component have Active in true. The QuestionControllers in the objects: "+this.gameObject.name+" and "+questionController1.gameObject.name+", have Active in true. only one QuestionController must have Active in true.");
                    severalQuestionControllersActive = true;
                }
            } 
        }
        if (severalQuestionControllersActive)
        {
            this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
            return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
        }

        activateInformationComponents = GameObject.FindObjectsOfType<ActivateInformation>();
        changeElementComponents = GameObject.FindObjectsOfType<ChangeElement>();//CODIGO RELACIONADO COM: RESET VALORES DE ChangeElement

        markerNewTrackableEventHandlerComponent = Marker.GetComponent<NewTrackableEventHandler>();
        //MENSAGEM ERRO MARKER SEM COMPONENTE NewTrackableEventHandler
        if (markerNewTrackableEventHandlerComponent == null)
        {
            Debug.LogError("AR Educational Framework Message: The ImageTarget: " + Marker.name + " doesn't have NewTrackableEventHandler component. you need to substitute DefaultTrackableEventHandler component for it.");
            this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
            return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
        }
        //MENSAGEM ERRO OBJECT IS NOT CHILD OF MARKER
        bool anyAnswerAlternativeIsNotChildOfMarker=false;
        for (int i = 0; i < AnswerAlternatives.Count; i++)
        {
             if (!AnswerAlternatives[i].transform.IsChildOf(Marker.transform))
             {
                    Debug.LogError("AR Educational Framework Message: The ImageTarget: " + Marker.name + " doesn't have child " + AnswerAlternatives[i].name + " in the Unity hierarchy. you need to make the object: " + AnswerAlternatives[i].name + ", child of marker: " + Marker.name+".");
                    anyAnswerAlternativeIsNotChildOfMarker = true;             
             }
        }
        if (anyAnswerAlternativeIsNotChildOfMarker)
        {
            this.enabled = false;// desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
            return;// nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.           
        }

        //CODIGO RELACIONADO COM: RESET COR
        SaveObjectsOfArrayInAllObjects(AnswerAlternatives.ToArray());
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
                if (changeElement.TypeOfChange == ChangeElement.TypeOfChanges.ChangeObject)//componente changElement esta trocando o objeto por outro
                {
                    if (UnityEngine.Object.ReferenceEquals(obj,changeElement.MyObject))
                    {
                        if (!substituteObjects.Contains(changeElement.NewObject))
                            substituteObjects.Add(changeElement.NewObject);//encher lista substituteObjects de todos os objetos subtitutos

                        if (AnswerAlternatives.Contains(obj))//se o objeto atual é um objeto alternativa 
                        {
                            if(!substituteObjectsOfAnswerAlternatives.Contains(changeElement.NewObject))
                            substituteObjectsOfAnswerAlternatives.Add(changeElement.NewObject);//encher lista substituteObjectsOfAnswerAlternatives dos objetos substitutos que substituem objetos alternativa.
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

        //JEITO RESET SCALE emty objects
        foreach (GeralObjectsContainer geralObj in ButtonOperations.GeralObjectsContainerList)//obtem em listas os objetos gerais e seus objetos emptys respetivos que vao ser escalados por botoes ou por marcadores de controle
        {
            originalObjects.Add(geralObj.OriginalObject);
            emtyObjects.Add(geralObj.EmtyObject);
        }

        //CODIGO RELACIONADO COM: RESET COR
        InitializeVectObjectsContainerWithRenderer();

        //CODIGO RELACIONADO COM: RESET TRANSFORM OF THE ELEMENTS
        //elementos alternativa
        answerAlternativesOriginalPosition = new Vector3[AnswerAlternatives.Count];
        answerAlternativesOriginalRotation = new Quaternion[AnswerAlternatives.Count];
        answerAlternativesOriginalScale = new Vector3[AnswerAlternatives.Count];
        SaveOriginalTransformOfElements(AnswerAlternatives.ToArray(), answerAlternativesOriginalPosition, answerAlternativesOriginalRotation, answerAlternativesOriginalScale);

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
        if (substituteObjectsOfAnswerAlternatives.Count > 0)//se tem objetos substituindo algum objeto alternativa (objeto usado como alternativa de resposta)
        {
            substituteObjectsOfAnswerAlternativesOriginalPosition = new Vector3[substituteObjectsOfAnswerAlternatives.Count];
            substituteObjectsOfAnswerAlternativesOriginalRotation = new Quaternion[substituteObjectsOfAnswerAlternatives.Count];
            substituteObjectsOfAnswerAlternativesOriginalScale = new Vector3[substituteObjectsOfAnswerAlternatives.Count];
            SaveOriginalTransformOfElements(substituteObjectsOfAnswerAlternatives.ToArray(), substituteObjectsOfAnswerAlternativesOriginalPosition, substituteObjectsOfAnswerAlternativesOriginalRotation, substituteObjectsOfAnswerAlternativesOriginalScale);
        }

        if (useTime)
        {
            if (useAlertTime) originalColor = TimeText.color;
            if(timeDependingOfMarker) originalTotalTime = totalTime;//usado para ajustar totalTime dependendo de se o inicia o markador vissivel ou nao vissivel
        }

        if (randomOptions)
        {
            correctAnswerObj = AnswerAlternatives[correctAnswerIndex];//obtem o objecto resposta antes de reajustar a lista random
        }

            LeftButton.onClick.AddListener(LeftButtonMethod);
            RightButton.onClick.AddListener(RightButtontMethod);
            CheckButton.onClick.AddListener(CheckButtonMethod);
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

            //CODIGO PARA FAZER interactable true ou false EM BOTOES, DEPENDENDO DE SE MARCADOR VISSIVEL OU NAO
            if (markerNewTrackableEventHandlerComponent.MarkerFound)//marcador esta vissivel
            {
                    LeftButton.interactable = true;
                    RightButton.interactable = true;
                    CheckButton.interactable = true;
            }
            else if (!markerNewTrackableEventHandlerComponent.MarkerFound)//marcador nao esta vissivel 
            {
                    LeftButton.interactable = false;
                    RightButton.interactable = false;
                    CheckButton.interactable = false;
            }

            if (useTime)
            {
                //CODIGO RELACIONADO COM: PAUSAR TEMPO
                //usado para ajustar o valor original do totalTime em +1 ou deixar seu valor original, dependendo de se inicia o markador nao vissivel ou vissivel. usa-se para consertar defeito de relogio em nao mostrar o valor inicial certinho com o qual comeca.
                if (timeDependingOfMarker)
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
                if (timeDependingOfMarker)//quer que o tempo dependa de se o marcador for vissivel ou nao
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
                            stopTimeValue = guiTime;//stopTimeValue se coloca em guiTime para  permitir que o contagem possa continuar desde o valor onde foi pausado 
                            stopTime = false;
                            paused = false;
                        }
                    }
                }
            }
        }
    }

    private void SaveOriginalTransformOfElements(GameObject[] elements, Vector3[] elementsOriginalPosition, Quaternion[] elementsOriginalRotation, Vector3[] elementsOriginalScale)//armazenar os valores inicias de transform(localposition,localrotation,localscale) dos elementos para fazer o reset quando for necesserio (nesta classe, usado tanto com elementos adicionais quanto elementos alternativa)
    {
        for (int i = 0; i < elements.Length; i++)
        {
            elementsOriginalPosition[i] = elements[i].transform.localPosition;
            elementsOriginalRotation[i] = elements[i].transform.localRotation;
            elementsOriginalScale[i] = elements[i].transform.localScale;
        }
    }

    private void ResetElementsTransform(GameObject[] elements, Vector3[] elementsOriginalPosition, Quaternion[] elementsOriginalRotation, Vector3[] elementsOriginalScale)//usado para reset tranform de elementos (nesta classe, usado tanto com adicional elements quanto elementos alternativa)
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

    private void ResetButtonMethod()//botao de reseting pergunta, se usa quando a pergunta é inicializada ou quando se hunde o botao de reset questao  
    {
        if (Active)//NOTA:esta condicao é necessaria aqui unicamente divido a que o metodo também pode ser executado atraves do botao de reset questao que pode ser um botao reusado em varios QuestionController, e nesse caso deve ser testada a condicao
        {

            //CODIGO RELACIONADO COM: RESET TRANSFORM OF THE ELEMENTS (elementos adicionais)   
            ResetElementsTransform(additionalElements0, additionalElements0OriginalPosition, additionalElements0OriginalRotation, additionalElements0OriginalScale); //reset os elementos additionalElements0 a seus valores iniciais, isto pois gameobjects 3D podem ter mudado estes valores.
            ResetElementsTransform(AdditionalElements1, additionalElements1OriginalPosition, additionalElements1OriginalRotation, additionalElements1OriginalScale);//reset os elementos additionalElements1 a seus valores iniciais, isto pois gameobjects 3D podem ter mudado estes valores.
            ResetElementsTransform(additionalElements2, additionalElements2OriginalPosition, additionalElements2OriginalRotation, additionalElements2OriginalScale);//reset os elementos additionalElements2 a seus valores iniciais, isto pois gameobjects 3D podem ter mudado estes valores.
            ResetElementsTransform(AdditionalElements3, additionalElements3OriginalPosition, additionalElements3OriginalRotation, additionalElements3OriginalScale);//reset os elementos additionalElements3 a seus valores iniciais, isto pois gameobjects 3D podem ter mudado estes valores.

            //desativa elementos estado2 e 3
            ResetButton.gameObject.SetActive(false);
            if (useNextQuestionButton)nextQuestionButton.gameObject.SetActive(false);
            if (usePreviousQuestionButton) previousQuestionButton.gameObject.SetActive(false);
            //desativa elementos estado2
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
                foreach(GameObject obj in AdditionalElements3)
                {
                    obj.SetActive(false);
                }
            }

            //ativa elementos adicionais estado 0
            foreach(GameObject obj in additionalElements0)
            {
                obj.SetActive(true);
            }

            //ativar elementos estado1
            LeftButton.gameObject.SetActive(true);
            RightButton.gameObject.SetActive(true);
            CheckButton.gameObject.SetActive(true);
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
                if (timeDependingOfMarker)
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

            if (randomOptions)//se estiver ativada a opcao random, entao reset a ordem de jeito random
            {
                ReshuffleList(AnswerAlternatives , answerAlternativesOriginalPosition, answerAlternativesOriginalRotation, answerAlternativesOriginalScale);//reset a ordem de jeito random dos objetos da lista 
            }

            actualIndex = 0;//reset atualIndex

            //CODIGO RELACIONADO COM: RESET TRANSFORM OF THE ELEMENTS (objetos alternativa)
            ResetElementsTransform(AnswerAlternatives.ToArray(), answerAlternativesOriginalPosition, answerAlternativesOriginalRotation, answerAlternativesOriginalScale);

            //CODIGO RELACIONADO COM: RESET TRANSFORM SUBSTITUTES OBJECTS. NOTA: nota-se que substituteObjects já está incluindo substituteObjectsOfAnswerAlternatives, e portanto é suficiente este codigo para reset transform de todos os objetos substitutos.
            if (substituteObjects.Count > 0)
                ResetElementsTransform(substituteObjects.ToArray(), substituteObjectsOriginalPosition, substituteObjectsOriginalRotation, substituteObjectsOriginalScale);

            //CODIGO RELACIONADO COM: RESET COR
            ResetObjectsColor(allObjectsWithRenderer);//neste caso reset cor de todos os elementos com Renderer, tanto elementos alternativa, quanto elementos adicionais, E AINDA ELEMENTOS SUBSTITUTOS

            //CODIGO RELACIONADO COM: RESET VALORES DE ChangeElement
            changeElementComponents = GameObject.FindObjectsOfType<ChangeElement>();//obtem todos os componentes ChangeElement 
            foreach (GameObject obj in allObjects)
            {      
                foreach (ChangeElement changeElement in changeElementComponents)
                {         
                    if (UnityEngine.Object.ReferenceEquals(obj,changeElement.MyObject))//se algum objeto de allObjects(ou seja, algum objeto pergunta ou adicinal element) esta sendo mudado pelo atual componente ChangeElement
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

            for (int i = 0; i < AnswerAlternatives.Count; i++)//ativa o primeiro objeto alternativa e desativa o resto de objetos inicialmente,deve ir após o atualIndex ser inicializado a 0
            {
                if (i == 0)
                {
                    AnswerAlternatives[i].gameObject.SetActive(true);
                }
                else if (i > 0)
                {
                    AnswerAlternatives[i].gameObject.SetActive(false);
                }
            }

        }
    }

    private IEnumerator ChangeQuestion(GameObject newQuestionController)
    {
        if (Active)
        {
            yield return new WaitForEndOfFrame();//NOTA: esta linha é necessaria por dois fatos: 1:executar unicamente o codigo do QuestionController Active em true e nao de outros quando for precionado o botao. o codigo quando este botao for precionado muda o estado de Active dos dois QuestionController, assim o segundo QuestionController poderia ser executado também quando nao é seu turno. com esta espera, se garante que primeiro sejam lidos todos o codigos do QuestionController enquanto tem seu valor Active original, finalizar esse frame, e após isso mudar os estados. 
                                                 //2: como o codigo a seguir se executa em um novo frame, entao já seu ordem de execucao fica como está estabelecido normalmente, ou seja primeiro desactivar objetos (codigo que esta em update)- e depois ativar objetos(codigo que esta em lateupdate)), nao é necessario colocar outro delay, e o comportamento será igual a como se executa por default inicialmente, quando um unico QuestionController está ativo (este se executa de ultimo para ativar objetos que podem ser reusados em QuestionController) e o resto de controller desativos (se executam primeiro para que objetos que podem ser reusados, possam ser ativados depois) 

            //AVISAR ActivateInformation, muda a variavel isChangingQuestion a true de cada componente ActivateInformation para fazer o codigo de ativar e desativar texts, images e linhas que nao se execute neste frame.
            activateInformationComponents = GameObject.FindObjectsOfType<ActivateInformation>();
            foreach (ActivateInformation activateInformationComponent in activateInformationComponents)
            {
                activateInformationComponent.IsChangingQuestion = true;
            }

            Active = false;//desativar o QuestionController atual (da pergunta atual)

            if (newQuestionController.GetComponent<QuestionController2>() != null)//se a proxima pergunta tem também um componente tipo QuestionController2 (é uma pergunta do mesmo tipo do que a atual)
            {
                newQuestionController.GetComponent<QuestionController2>().Active = true;//ativar o proximo QuestionController (da proxima pergunta)
            }
            else if (newQuestionController.GetComponent<QuestionController1>() != null)//se a proxima pergunta tem um componente tipo QuestionController1
            {
                newQuestionController.GetComponent<QuestionController1>().Active = true;//ativar o proximo QuestionController (da proxima pergunta)
            }
        }
    }

    private void DisableElements()//desativa todos os elementos pois nao vai ser usado este QuestionController no momento
    {
        foreach (GameObject obj in AnswerAlternatives)//desativar objetos alternativa
        {
            obj.SetActive(false);
        }
        //desativar botoes 
        LeftButton.gameObject.SetActive(false);
        RightButton.gameObject.SetActive(false);
        CheckButton.gameObject.SetActive(false);
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

        foreach(GameObject obj in additionalElements0)//desativar elementos adicionais estado 0
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
            foreach(GameObject obj in AdditionalElements3)//desativar elementos adicionais estado3
            {
                obj.SetActive(false);
            }
        }
    }

    private void AnswerButtonMethod()//pressionado botao de ver resposta certa, pasar a estado 3
    {
        if (Active)//NOTA: as condicoes if(active) antes do codigo dos botoes, garante que ainda tendo botoes reusados para varias perguntas numa mesma cena,só seja executado o codigo para a pergunta ativa nesse momento. 
        {

            //desativar elementos estado2 necessarios (se deixam ativos botoes de repetir pergunta, e next e previous no caso de existir)
            AnswerButton.gameObject.SetActive(false);     
            foreach(GameObject obj in additionalElements2)//desativa elementos adicionais estado2
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
        
            if (useDefaultAnswer)//vai ser usado o jeito default para mostrar a resposta, que consiste em ativar o objeto de resposta
            {
                //CODIGO RELACIONADO COM: RESET TRANSFORM OF THE ELEMENTS (objetos alternativa)
                ResetElementsTransform(AnswerAlternatives.ToArray(), answerAlternativesOriginalPosition, answerAlternativesOriginalRotation, answerAlternativesOriginalScale);

                //CODIGO RELACIONADO COM: RESET TRANSFORM OF substituteObjectsOfAnswerAlternatives (objetos sustitutos dos objetos alternativa)
                if (substituteObjectsOfAnswerAlternatives.Count > 0)
                    ResetElementsTransform(substituteObjectsOfAnswerAlternatives.ToArray(),substituteObjectsOfAnswerAlternativesOriginalPosition,substituteObjectsOfAnswerAlternativesOriginalRotation,substituteObjectsOfAnswerAlternativesOriginalScale);

                //CODIGO RELACIONADO COM: RESET COR, NOTA: neste caso reset cor unicamente dos objetos alternativa
                ResetObjectsColor(AnswerAlternatives);

                //CODIGO RELACIONADO COM: RESET COR, RESET COR OF substituteObjectsOfAnswerAlternatives (objetos sustitutos dos objetos alternativa)
                if (substituteObjectsOfAnswerAlternatives.Count > 0)
                    ResetObjectsColor(substituteObjectsOfAnswerAlternatives);

                //CODIGO RELACIONADO COM: RESET VALORES DE ChangeElement (para objetos alternativa e objetos substitutos dos objetos alternativa)
                changeElementComponents = GameObject.FindObjectsOfType<ChangeElement>();//obtem todos os componentes ChangeElement 
                List<GameObject> answerAlternativesAndSubstituteObjectsOfAnswerAlternatives = new List<GameObject>();//lista com todos os elementos de AnswerAlternatives e substituteObjectsOfAnswerAlternatives juntos
                foreach (GameObject obj in AnswerAlternatives)
                {
                    if (!answerAlternativesAndSubstituteObjectsOfAnswerAlternatives.Contains(obj))
                    {
                        answerAlternativesAndSubstituteObjectsOfAnswerAlternatives.Add(obj);//acrescentamentos os objetos alternativa (AnswerAlternatives) na lista
                    }
                }
                foreach (GameObject obj in substituteObjectsOfAnswerAlternatives)
                {
                    if (!answerAlternativesAndSubstituteObjectsOfAnswerAlternatives.Contains(obj))
                    {
                        answerAlternativesAndSubstituteObjectsOfAnswerAlternatives.Add(obj);//acrescentamos os objetos substitutos dos objetos alternativa (substituteObjectsOfAnswerAlternatives) na lista
                    }
                }
                foreach (GameObject obj in answerAlternativesAndSubstituteObjectsOfAnswerAlternatives)
                {
                    foreach(ChangeElement changeElement in changeElementComponents)
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
                //ativar objeto de resposta
                if (randomOptions)//se esta usando opcao random para perguntas,entao ativar o objeto atraves de correctAnswerObj que se refere ao objeto resposta
                {
                    correctAnswerObj.SetActive(true);
                }
                else//se nao esta usando opcao random,entao pode-se ativar o objeto atraves do index, pois os objetos da lista nunca mudaram de posicao.
                {
                    AnswerAlternatives[correctAnswerIndex].SetActive(true);
                }
            }
        }
    }

    private void CheckButtonMethod()//confirmar opcao (escolher opcao)
    {
        if (Active)
        {
            if (!randomOptions)// opcoes nao random
            {
                if (actualIndex == correctAnswerIndex)
                {
                    CorrectAnswer();
                }
                else
                {
                    WrongAnswer();
                }
            }
            else //if(randomOptions) // opcoes ramdom
            {
                if (UnityEngine.Object.ReferenceEquals(AnswerAlternatives[actualIndex], correctAnswerObj))
                {
                    CorrectAnswer();
                }
                else
                {
                    WrongAnswer();
                }
            }
            StateQuestionAnswered();
        }
    }

    private void StateQuestionAnswered()//mudar ao estado pergunta respondida , estado2
    {
        questionAnswered = true;

        //desativar elementos estado1 após respoder pergunta
        LeftButton.gameObject.SetActive(false);
        RightButton.gameObject.SetActive(false);
        CheckButton.gameObject.SetActive(false);
        foreach (GameObject obj in AdditionalElements1)//desastiva elementos adicionais estado1
        {
            obj.SetActive(false);
        }

        AnswerAlternatives[actualIndex].gameObject.SetActive(false);//desativa o atual objeto alternativa que é o unico ativo no momento

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

    private void LeftButtonMethod()
    {
        if (Active)
        {
            if (actualIndex >= 1)
            {
                AnswerAlternatives[actualIndex].SetActive(false);
                actualIndex--;
                AnswerAlternatives[actualIndex].SetActive(true);
            }
        }
    }

    private void RightButtontMethod()
    {
        if (Active)
        {
            if (actualIndex <= AnswerAlternatives.Count - 2)
            {
                AnswerAlternatives[actualIndex].gameObject.SetActive(false);
                actualIndex++;
                AnswerAlternatives[actualIndex].SetActive(true);
            }
        }
    }

    private void ReshuffleList(List<GameObject> list, Vector3[] answerAlternativesOriginalPosition, Quaternion[] answerAlternativesOriginalRotation, Vector3[] answerAlternativesOriginalScale)//reset de jeito random objetos duma lista, ver mais:https://forum.unity.com/threads/random-shuffle-array.86737/ 
    {
        for(int i = 0; i < list.Count; i++)
        {
            GameObject temp = list[i];
            //CODIGO RELACIONADO COM: RESET TRANSFORM OF THE ELEMENTS (necessario para menter o vector dos tranform(answerAlternativesOriginalPosition,answerAlternativesOriginalRotation e answerAlternativesOriginalScale)dos elementos alternativa atualizado quando objetos sao trocados de jeito random)
            Vector3 OriginalPositionT = answerAlternativesOriginalPosition[i];
            Quaternion OriginalRotationT = answerAlternativesOriginalRotation[i];
            Vector3 OriginalScaleT = answerAlternativesOriginalScale[i];
            //---
            int index = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[index];
            //CODIGO RELACIONADO COM: RESET TRANSFORM OF THE ELEMENTS (necessario para menter o vector dos tranform(answerAlternativesOriginalPosition,answerAlternativesOriginalRotation e answerAlternativesOriginalScale)dos elementos alternativa atualizado quando objetos sao trocados de jeito random)
            answerAlternativesOriginalPosition[i] = answerAlternativesOriginalPosition[index];
            answerAlternativesOriginalRotation[i] = answerAlternativesOriginalRotation[index];
            answerAlternativesOriginalScale[i] = answerAlternativesOriginalScale[index];
            //---
            list[index] = temp;
            //CODIGO RELACIONADO COM: RESET TRANSFORM OF THE ELEMENTS (necessario para menter o vector dos tranform(answerAlternativesOriginalPosition,answerAlternativesOriginalRotation e answerAlternativesOriginalScale)dos elementos alternativa atualizado quando objetos sao trocados de jeito random)
            answerAlternativesOriginalPosition[index] = OriginalPositionT;
            answerAlternativesOriginalRotation[index] = OriginalRotationT;
            answerAlternativesOriginalScale[index] = OriginalScaleT;
        }
        correctAnswerIndex = AnswerAlternatives.IndexOf(correctAnswerObj);//atualizar o indice a sua nova posicao uma vez que foi mudado de jeito random
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
                StateQuestionAnswered();
            }
            text = String.Format("{0:00}:{1:00}", minutes, seconds);
            TimeText.text = text;
        }
    }

}
