using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonOperations : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public enum TypeOfOperations{scale,rotate}
    [SerializeField] private TypeOfOperations typeOfOperation;
    public TypeOfOperations TypeOfOperation{get{return typeOfOperation;}}
    [SerializeField] private GameObject[] objects = new GameObject[1];
    public GameObject[] Objects { get {return objects;} }
    [SerializeField] private GameObject[] markers;//NOTA:O TAMANHO DE markers É O MESMO QUE DO VETOR objects, E É AJUSTADO AUTOMATICAMENTE NA CLASSE ButtonOperationsI (que customiza o inspector)
    public GameObject[] Markers{ get{return markers;}}
    public enum TypeOfScale { increase, decrease, resetScale };
    [SerializeField] private TypeOfScale typeOfScale;
    [SerializeField] private float scaleSpeed = 1f;
    [SerializeField] private float maximumScale = 2f;//NOTA:deveria ser sempre maior do que 1
    [SerializeField] private float minimumScale = 0.5f;//NOTA:deveria ser sempre menor do que 1
    [SerializeField] private bool increaseButtonIsClick;
    [SerializeField] private bool decreaseButtonIsClick;
    private static List<GeralObjectsContainer> geralObjectsContainerList = new List<GeralObjectsContainer>();//classe containers com objetos gerias e respetivos objetos empty, para os objetos que vao ser escalados pelo framework, lembrando que cada objeto será vinculado como filho dum objeto vazio empty colocado no ponto do marcador e que será escalado ao inves do proprio objeto, permitindo escalar o objeto de jeito proporcional sem se sobrepor no marcador. 
    public static List<GeralObjectsContainer> GeralObjectsContainerList { get{return geralObjectsContainerList;}}
    private List<bool> visibleMarkerAndActiveObjectList = new List<bool>();//lista bool que indica o estado de visibilidade e ativacao dum casal marcador-objeto
    private Button myButton;
    public enum TypeOfRotation { inAxis, resetRotation };
    [SerializeField] private TypeOfRotation typeOfRotation;
    [SerializeField] private float rotationSpeed = 2;
    public enum AxisOfRotation{X,Y,Z}
    [SerializeField] private AxisOfRotation axisOfRotation;
    public enum SenseOfRotationInX{up,down};
    [SerializeField] private SenseOfRotationInX senseOfRotationInX;
    public enum SenseOfRotationInY{right,left}
    [SerializeField] private SenseOfRotationInY senseOfRotationInY;
    public enum SenseOfRotationInZ{positive,negative}
    [SerializeField] private SenseOfRotationInZ senseOfRotationInZ;
    private bool rotationButtonIsClik;
    private Quaternion[] originalRotations;
    //ATRIBUTOS RELACIONADOS COM: PULAR OBJETOS QUANDO JÁ MARCADORES JUNTOS ESTAO ESCALANDO O OBJETO TAMBEM
    [SerializeField] private bool[] objectsCondition;//vetor de bool que indica se um objeto será pulado por o botao atual quando já tiver um componente ChangeElement associado que também irá mudar tamanho quando marcadores juntos, isto permite evitar escalar o mesmo objeto de dois jeitos diferentes no mesmo tempo.
    public bool[] ObjectsCondition{ get{return objectsCondition;}}

    private void Awake()
    {
        //MENSAGEM ERRO MARKER SEM COMPONENTE NewTrackableEventHandler
        bool AnyMarkerWithoutNewTrackableEventHandlerComponent = false;
        foreach (GameObject marker in Markers)
        {
            if (marker.GetComponent<NewTrackableEventHandler>() == null)
            {
                Debug.LogError("AR Educational Framework Message: The ImageTarget: " + marker.name + " doesn't have NewTrackableEventHandler component. you need to substitute DefaultTrackableEventHandler component for it.");
                AnyMarkerWithoutNewTrackableEventHandlerComponent = true;
            }
        }
        if (AnyMarkerWithoutNewTrackableEventHandlerComponent)
        {
            this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
            return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
        }
        //MENSAGEM ERRO: OBJECT IS NOT CHILD OF MARKER
        bool anyObjectIsNotChildOfMarker = false;
        for (int i = 0; i < Objects.Length; i++)
        {
             if (!Objects[i].transform.IsChildOf(Markers[i].transform))
             {
                Debug.LogError("AR Educational Framework Message: The ImageTarget: " + Markers[i].name + " doesn't have child "+ Objects[i].name + " in the Unity hierarchy. you need to make the object: "+Objects[i].name+", child of marker: "+ Markers[i].name+".");
                anyObjectIsNotChildOfMarker = true;
             }
        }
        if (anyObjectIsNotChildOfMarker)
        {
            this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
            return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
        }

        //CRIAR O OBJETO EMPTY PARA cada objeto de Objects E ACRESCENTAR NA LISTA GERAL DE OBJETOS EMPTY INTERMEDIARIOS USADOS PARA ESCALAR OBJETOS PROPORCIONALMENTE
        if (TypeOfOperation == TypeOfOperations.scale)//sempre que que vai se escalar com o botao
        {
            for (int i = 0; i < Objects.Length; i++)//percorre os objetos associados ao botao
            {
                List<GameObject> originalObjects = new List<GameObject>();
                foreach (GeralObjectsContainer geralObjectContainer in GeralObjectsContainerList)//obtem lista atual dos objetos gerais (que vao ser escalados por botoes ou por marcadores de controle)
                {
                    originalObjects.Add(geralObjectContainer.OriginalObject);
                }
                if (!originalObjects.Contains(Objects[i]))//ainda nao foi acrescentado o objeto atual na lista, entao pode se acrescentar
                {
                    GameObject objEmty = new GameObject(Objects[i].name + "Emty");//cria um emty novo com o mesmo nome do que o objeto atual + Empy
                    objEmty.transform.localPosition = Markers[i].transform.localPosition;//posiciona ele no mesmo lugar do marcador
                    objEmty.transform.parent = Markers[i].transform;//fazer objeto emty filho do marcador,necessario para nao tirar o objeto do seu contexto inicial quando for colocado como filho do emty
                    objEmty.transform.localScale = Vector3.one;//IMPORTANTE RESET O VALOR A 1,1,1 POIS SE O MARCADOR NAO TEM TANAHO 1,1,1, O OBJ EMTY QUANDO SE FAZ FILHO DO MARCADOR FICARIA COM OUTRO TAMANHO E DARIA ERRADO AS OPERACOES MATEMATICAS DO ALGORITMO

                    //CODIGO RELACIONADO COM: SUPORTAR IMAGENS EM CANVAS PARA SEREM ESCALADAS
                    Transform myParent = Objects[i].transform.parent;
                    if (!(UnityEngine.Object.ReferenceEquals(myParent,Markers[i])) && (myParent.GetComponent<Canvas>() != null))//se pai do objeto atual nao for marcador e for um canvas
                    {
                        objEmty.transform.SetParent(myParent.transform);//fazer o objEmty filho do canvas
                        objEmty.transform.localScale = Vector3.one;//IMPORTANTE RESET O VALOR A 1,1,1 POIS SE O CANVAS NAO TEM TANAHO 1,1,1, O OBJ EMTY QUANDO SE FAZ FILHO DO CANVAS FICARIA COM OUTRO TAMANHO E DARIA ERRADO AS OPERACOES MATEMATICAS DO ALGORITMO
                    }
                    //---

                    Objects[i].transform.SetParent(objEmty.transform);//fazer objeto atual filho do objeto empty
                    GeralObjectsContainer geralObjectN = new GeralObjectsContainer(Objects[i], objEmty);//acrescenta na lista geral o objeto atual e seu empty criado
                    GeralObjectsContainerList.Add(geralObjectN);
                }
            }
        }

    }

    // Use this for initialization
    void Start()
    {
        myButton = gameObject.GetComponent<Button>();

        if (TypeOfOperation == TypeOfOperations.scale)
        {
            //inicializar vetor com true
            objectsCondition = new bool[Objects.Length];
            for (int i = 0; i < ObjectsCondition.Length; i++)
            {
                ObjectsCondition[i] = true;
            }
        }
        else if(TypeOfOperation == TypeOfOperations.rotate)
        {
            originalRotations = new Quaternion[Objects.Length];

            for (int i = 0; i < Objects.Length; i++)
            {
                originalRotations[i] = Objects[i].transform.localRotation;//guardar a rotacao original dos objetos de Objects
            }
        }
    }

    void OnDisable()//executado quando o botao for disable
    {
        if(myButton!=null)
            myButton.interactable = false;//reset valor de interactable quando o botao for desativado

        //reset valor de variaveis a seu estado inicial. isto permite que quando seja desativado os botoes (devido a tempo finalizado) estas variaveis nao fiquem em seus estados atuais quando reiniciar a perguta, fazendo que os objetos aumentem ou rotacionem sozinhos por erro
        increaseButtonIsClick = false;
        decreaseButtonIsClick = false;
        rotationButtonIsClik = false;
    }

    // Update is called once per frame
    void Update()
    {
        //DESBLOQUEAR BOTAO QUANDO PELO MENOS UM DOS MARCADORES ASSOCIADOS AO BOTAO ESTIVER VISSIVEL, E SEU OBJETO ATIVADO. (OU BLOQUEAR QUANDO NAO TIVER MARCADOR VISSIVEL E OBJETO ATIVADO)
        //---
        for (int i = 0; i < Markers.Length; i++)
        {
                if (Markers[i].GetComponent<NewTrackableEventHandler>().MarkerFound && Objects[i].activeSelf && !AllRendererAreDisable(Objects[i]))//se algum casal marcador-objeto esta disponivel (ou seja: marcador vissivel e objeto ativado)
                {
                    visibleMarkerAndActiveObjectList.Add(true);
                }
                else
                {
                    visibleMarkerAndActiveObjectList.Add(false);
                }
        }
        if (visibleMarkerAndActiveObjectList.Contains(true) && !myButton.interactable)//NOTA: a condicao visibleMarkerAndActiveObjectList.Contains(true) significa que tem pelo menos um casal marcador-objeto disponivel
        {
                myButton.interactable = true;
        }
        if (!visibleMarkerAndActiveObjectList.Contains(true) && myButton.interactable)//NOTA: a condicao !visibleMarkerAndActiveObjectList.Contains(true) significa que nao tem nenhum casal marcador-objeto disponivel
        {
                myButton.interactable = false;
        }
        visibleMarkerAndActiveObjectList.Clear();//importantante fazer reset na lista cada frame para fazer a validacao da lista certa 
        //---
        
        if (TypeOfOperation == TypeOfOperations.scale)//FOI ESCOLHIDO ESCALAR
        {
            if (myButton.interactable)//SOMENTE SE BOTAO INTERACTABLE É QUE DEVE SER EXECUTADO O CODIGO DOS BOTOES
            {
                if (increaseButtonIsClick)//se está pressionando botao incrementar tamanho
                {
                    List<GameObject> originalObjects = new List<GameObject>();
                    List<GameObject> emtyObjects = new List<GameObject>();
                    foreach (GeralObjectsContainer geralObjectContainer in GeralObjectsContainerList)//obtem em listas os objetos gerais e seus objetos empty respetivos que vao ser escalados por botoes ou por marcadores de controle
                    {
                        originalObjects.Add(geralObjectContainer.OriginalObject);
                        emtyObjects.Add(geralObjectContainer.EmtyObject);
                    }

                    for (int i = 0; i < Objects.Length; i++)//percorremos o vector dos objetos a escalar
                    {
                        if (!Markers[i].GetComponent<NewTrackableEventHandler>().MarkerFound || Objects[i].activeSelf == false || AllRendererAreDisable(Objects[i]))//PULA O OBJETO E BOTAO NAO FAZ NADA NELE SE MARCADOR ASSOCIADO AO OBJETO ATUAL NAO FOR VISSIVEL, OU SE OBJETO ESTIVER DESATIVADO
                        {
                                continue;
                        }
                        if (ObjectsCondition[i] == false)//PULAR OBJETO QUANDO JÁ MARCADORES JUNTOS ESTAO ESCALANDO O OBJETO, este vetor é modificado desde o script ChangeElement
                        {
                            continue;
                        }

                        for (int y = 0; y < originalObjects.Count; y++)//para cada objeto percorremos a lista geral para encontrar seu indice e obter o objeto empty vinculado a ele (NOTA: lembre-se que o framework faz escalas dos objetos de jeito proporcional para nao se sobrepor ao marcador, e portanto atraves dum objeto empty intermediario e nao do objeto diretamente)
                        {
                            if (UnityEngine.Object.ReferenceEquals(Objects[i],originalObjects[y]))//quando seja encontrado o objeto atual na lista, entao usar o indice para obter o objeto emty nesse indice que deve ser escalado (emty vinculado a esse objeto) 
                            {
                                GameObject emtyObj = emtyObjects[y];//obter emty vinculado ao objeto atual que deve ser escalado

                                //escalar
                                if (emtyObj.transform.localScale.y < maximumScale)//se a escalado do objeto emptyObj for menor do que maximumScale, e portanto ainda pode aumentar sua escala
                                {
                                    Vector3 ScaleT = emtyObj.transform.localScale + new Vector3(scaleSpeed, scaleSpeed, scaleSpeed) * Time.deltaTime;//obtem  escala atual mais o aumento 
                                    if (ScaleT.y >= maximumScale)//escala atual mais o aumento fica maior ou igual do que escala maxima, neste caso nao se aumentará normal senao que neste ultimo frame se colocará diretamente o myObject na scala maxima 
                                    {
                                        emtyObj.transform.localScale = new Vector3(maximumScale, maximumScale, maximumScale);
                                    }
                                    else//escala atual mais o aumento fica menor do que a escala maxima, portanto pode-se aumentar sem problema mais um frame na velocidade normal
                                    {
                                        emtyObj.transform.localScale = ScaleT;
                                    }
                                }
                            }
                        }
                    }
                }

                if (decreaseButtonIsClick)//se está pressionando botao diminuir tamanho
                {
                    List<GameObject> originalObjects = new List<GameObject>();
                    List<GameObject> emtyObjects = new List<GameObject>();
                    foreach (GeralObjectsContainer geralObjectContainer in GeralObjectsContainerList)//obtem em listas os objetos gerais e seus objetos empty respetivos que vao ser escalados por botoes ou por marcadores de controle
                    {
                        originalObjects.Add(geralObjectContainer.OriginalObject);
                        emtyObjects.Add(geralObjectContainer.EmtyObject);
                    }

                    for (int i = 0; i < Objects.Length; i++)//percorremos o vector dos objetos a escalar
                    {
                        if (!Markers[i].GetComponent<NewTrackableEventHandler>().MarkerFound || Objects[i].activeSelf == false || AllRendererAreDisable(Objects[i]))//PULA O OBJETO E BOTAO NAO FAZ NADA NELE SE MARCADOR ASSOCIADO AO OBJETO ATUAL NAO FOR VISSIVEL, OU SE OBJETO ESTIVER DESATIVADO
                        {
                            continue;
                        }
                        if (ObjectsCondition[i] == false)//PULAR OBJETO QUANDO JÁ MARCADORES JUNTOS ESTAO ESCALANDO O OBJETO, este vetor é modificado desde o script ChangeElement
                        {
                            continue;
                        }

                        for (int y = 0; y < originalObjects.Count; y++)//para cada objeto percorremos a lista geral para encontrar seu indice e obter o objeto empty vinculado a ele (NOTA: lembre-se que o framework faz escalas dos objetos de jeito proporcional para nao se sobrepor ao marcador, e portanto atraves dum objeto empty intermediario e nao do objeto diretamente)
                        {
                            if (UnityEngine.Object.ReferenceEquals(Objects[i],originalObjects[y]))//quando seja encontrado o objeto atual na lista, entao usar o indice para obter o objeto emty nesse indice que deve ser escalado (emty vinculado a esse objeto) 
                            {
                                GameObject emtyObj = emtyObjects[y];//emty vinculado ao objeto atual que deve ser escalado

                                //escalar
                                if (emtyObj.transform.localScale.y > minimumScale)//se a escala do objeto emptyObj for maior do que minimumScale, e portanto ainda pode diminuir sua escala
                                {
                                    Vector3 ScaleT = emtyObj.transform.localScale - new Vector3(scaleSpeed, scaleSpeed, scaleSpeed) * Time.deltaTime;//obtem escala atual menos o decremento 

                                    if (ScaleT.y <= minimumScale)//escala atual menos decremento fica menor ou igual do que escala minima, neste caso nao se diminuira normal senao que neste ultimo frame se colocará diretamente o objeto na scala manima 
                                    {
                                        emtyObj.transform.localScale = new Vector3(minimumScale, minimumScale, minimumScale);
                                    }
                                    else//escala atual menos decremento fica maior do que a escala minima, portanto pode-se diminuir sem problema mais um frame na velocidade normal
                                    {
                                        emtyObj.transform.localScale = ScaleT;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (TypeOfOperation == TypeOfOperations.rotate)//FOI ESCOLHIDO ROTACIONAR
        {
            if (typeOfRotation == TypeOfRotation.inAxis)//FOI ESCOLHIDO ROTACIONAR EM AXIS e nao reset
            {
                float rotationSpeedFactor = 30;//variavel que multiplica o valor da variavel rotationSpeed para fazer ela maior, pois precisam-se valores altos uma vez que Time.deltaTime faz com que a rotacao seja muito pouca. 
                if (myButton.interactable)//SOMENTE SE BOTAO INTERACTABLE É QUE DEVE SER EXECUTADO O CODIGO DOS BOTOES
                {
                    if (rotationButtonIsClik)//se está pressionando botao de rotacionar
                    {
                        for (int i = 0; i < Objects.Length; i++)
                        {
                            if (!Markers[i].GetComponent<NewTrackableEventHandler>().MarkerFound || Objects[i].activeSelf == false || AllRendererAreDisable(Objects[i]))//PULA O OBJETO E BOTAO NAO FAZ NADA NELE SE MARCADOR ASSOCIADO AO OBJETO ATUAL NAO FOR VISSIVEL, OU SE OBJETO ESTIVER DESATIVADO
                            {
                                continue;
                            }

                            if (axisOfRotation == AxisOfRotation.X)//se eixo de rotacao for X
                            {
                                if (senseOfRotationInX == SenseOfRotationInX.up)//se sentido da rotacao em X for acima
                                {
                                    Objects[i].transform.Rotate(new Vector3(rotationSpeed*rotationSpeedFactor, 0, 0) * Time.deltaTime, Space.World);
                                }
                                else if (senseOfRotationInX == SenseOfRotationInX.down)//se sentido da rotacao em X for embaixo
                                {
                                    Objects[i].transform.Rotate(new Vector3(-rotationSpeed*rotationSpeedFactor, 0, 0) * Time.deltaTime, Space.World);
                                }
                            }
                            else if (axisOfRotation == AxisOfRotation.Y)//se eixo de rotacao for Y
                            {
                                if (senseOfRotationInY == SenseOfRotationInY.right)//se sentido da rotacao em Y for direita
                                {
                                    Objects[i].transform.Rotate(new Vector3(0, -rotationSpeed*rotationSpeedFactor, 0) * Time.deltaTime, Space.World);
                                }
                                else if (senseOfRotationInY == SenseOfRotationInY.left)//se sentido da rotacao em Y for esquerda
                                {
                                    Objects[i].transform.Rotate(new Vector3(0, rotationSpeed*rotationSpeedFactor, 0) * Time.deltaTime, Space.World);
                                }
                            }
                            else if (axisOfRotation == AxisOfRotation.Z)//se eixo de rotacao for Z
                            {
                                if (senseOfRotationInZ == SenseOfRotationInZ.positive)//se sentido da rotacao em Z for positiva
                                {
                                    Objects[i].transform.Rotate(new Vector3(0, 0, -rotationSpeed*rotationSpeedFactor) * Time.deltaTime, Space.World);
                                }
                                else if (senseOfRotationInZ == SenseOfRotationInZ.negative)//se sentido da rotacao em Z for negativa
                                {
                                    Objects[i].transform.Rotate(new Vector3(0, 0, rotationSpeed*rotationSpeedFactor) * Time.deltaTime, Space.World);
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    private bool AllRendererAreDisable(GameObject obj)//retorna true se todos os componentes Renderer dum objeto estiverem disable
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)//se tem componentes Renderer
        {
            foreach (Renderer renderer in renderers)
            {
                if (renderer.enabled)//tem algum renderer enable
                {
                    return false;
                }
            }
            return true;//todos os renderer foram disable
        }
        else//se nao tem Renderer o objeto retorna false
        {
            return false;//se nao tem Renderer o objeto retorna false, isto para que nesse caso as condicoes que usam este metodo considerem unicamente o valor de verdade das das outras variaveis e esta seja insignificante.
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (TypeOfOperation == TypeOfOperations.scale)//SE FOI ESCOLHIDO ESCALAR
        {
            if (typeOfScale == TypeOfScale.increase)//se esta usando o botao para incrementar o tamnho do objeto
            {
                increaseButtonIsClick = true;
            }
            else if (typeOfScale == TypeOfScale.decrease)//se esta usando o batao para diminuir o tamanho do objeto
            {
                decreaseButtonIsClick = true;
            }
        }
        else if(TypeOfOperation == TypeOfOperations.rotate)//SE FOI ESCOLHIDO ROTACIONAR
        {
            rotationButtonIsClik = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (TypeOfOperation == TypeOfOperations.scale)//SE FOI ESCOLHIDO ESCALAR
        {
            if (typeOfScale == TypeOfScale.increase)//se esta usando o botao para incrementar o tamnho do objeto
            {
                increaseButtonIsClick = false;
            }
            else if (typeOfScale == TypeOfScale.decrease)//se esta usando o batao para diminuir o tamanho do objeto
            {
                decreaseButtonIsClick = false;
            }
        }
        else if (TypeOfOperation == TypeOfOperations.rotate)//SE FOI ESCOLHIDO ROTACIONAR
        {
            rotationButtonIsClik = false;
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (TypeOfOperation == TypeOfOperations.scale)//SE FOI ESCOLHIDO ESCALAR
        {
            if (typeOfScale == TypeOfScale.resetScale)//SE FOI ESCOLHIDO RESET ESCALA
                ResetScale();
        }
        else if (TypeOfOperation == TypeOfOperations.rotate)//FOI ESCOLHIDO ROTACIONAR
        {
            if (typeOfRotation == TypeOfRotation.resetRotation)//SE FOI ESCOLHIDO RESET ROTACAO
            {
                if (myButton.interactable)//SOMENTE SE BOTAO INTERACTABLE É QUE DEVE SER EXECUTADO O CODIGO DOS BOTOES
                {
                    for (int i = 0; i < Objects.Length; i++)
                    {
                        if (!Markers[i].GetComponent<NewTrackableEventHandler>().MarkerFound || Objects[i].activeSelf == false || AllRendererAreDisable(Objects[i]))//PULA O OBJETO E BOTAO NAO FAZ NADA NELE SE MARCADOR ASSOCIADO AO OBJETO ATUAL NAO FOR VISSIVEL, OU SE OBJETO ESTIVER DESATIVADO
                        {
                            continue;
                        }
                        Objects[i].transform.localRotation = originalRotations[i];
                    }
                }
            }
        }
    }

    private void ResetScale()//reset escala do objeto a sua escala original
    {
        if (myButton.interactable)//SOMENTE SE BOTAO INTERACTABLE É QUE DEVE SER EXECUTADO O CODIGO DOS BOTOES
        {
            List<GameObject> originalObjects = new List<GameObject>();
            List<GameObject> emtyObjects = new List<GameObject>();
            foreach (GeralObjectsContainer geralObjectContainer in GeralObjectsContainerList)//obtem em listas os objetos gerais e seus objetos empty respetivos que vao ser escalados por botoes ou por marcadores de controle
            {
                originalObjects.Add(geralObjectContainer.OriginalObject);
                emtyObjects.Add(geralObjectContainer.EmtyObject);
            }

            for (int i = 0; i < Objects.Length; i++)//percorremos o vetor dos objetos a escalar
            {
                if (!Markers[i].GetComponent<NewTrackableEventHandler>().MarkerFound || Objects[i].activeSelf == false || AllRendererAreDisable(Objects[i]))//PULA O OBJETO E BOTAO NAO FAZ NADA NELE SE MARCADOR ASSOCIADO AO OBJETO ATUAL NAO FOR VISSIVEL, OU SE OBJETO ESTIVER DESATIVADO
                {
                    continue;
                }
                if (ObjectsCondition[i] == false)//PULAR OBJETO QUANDO JÁ MARCADORES JUNTOS ESTAO ESCALANDO O OBJETO, este vetor é modificado desde o script ChangeElement
                {
                    continue;
                }

                for (int y = 0; y < originalObjects.Count; y++)//para cada objeto percorremos a lista geral para encontrar seu indice e obter o objeto empty vinculado a ele (NOTA: lembre-se que o framework faz escalas dos objetos de jeito proporcional para nao se sobrepor ao marcador, e portanto atraves dum objeto empty intermediario e nao do objeto diretamente)
                {
                    if (UnityEngine.Object.ReferenceEquals(Objects[i],originalObjects[y]))//quando seja encontrado o objeto atual na lista, entao usar o indice para obter o objeto emty nesse indice sobre o qual deve ser feito reset da escala (emty vinculado a esse objeto) 
                    {
                        GameObject emtyObj = emtyObjects[y];//obter o emty vinculado ao objeto atual sobre o qual deve ser feito reset da escala
                        emtyObj.transform.localScale = Vector3.one;//reset a escala 
                    }
                }
            }
        }
    }

}

