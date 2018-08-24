using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateInformation : MonoBehaviour {

    [SerializeField] private GameObject marker1;
    public GameObject Marker1 {get {return marker1;} set {marker1 = value;}}
    [SerializeField] private GameObject marker2;
    public GameObject Marker2 {get {return marker2;} set {marker2 = value;}}
    [SerializeField] private bool marker1Found = false; 
    [SerializeField] private bool marker2Found = false;
    [SerializeField] private bool markersFound = false;
    private NewTrackableEventHandler marker1NewTrackableEventHandlerComponent;
    private NewTrackableEventHandler marker2NewTrackableEventHandlerComponent;
    [SerializeField] private GameObject myObject;
    public GameObject MyObject{ get{return myObject;} set{myObject = value;}}
    [SerializeField] private bool informationFollowARCamera = true;
    [SerializeField] private GameObject myARCamera;
    public GameObject MyARCamera{ get{return myARCamera;} set{myARCamera = value;}}
    [SerializeField] private float informationSpeed = 3;
    private Transform[] images;
    private Transform[] imagesT;
    [SerializeField] private Canvas myCanvas;//deve ter dentro só images UI que vao ser usadas, pois o algoritmo usa o numero de filhos para definir o vector dos pontos points
    public Canvas MyCanvas { get {return myCanvas;}}
    [SerializeField] private TextMesh[] texts3D;
    public TextMesh[] Texts3D{ get {return texts3D;}}
    private Transform[] texts3DT;
    [SerializeField] private SpriteRenderer[] sprites;
    private Transform[] spritesT;
    private GameObject instanceCanvas;
    private GameObject instanceTexts;
    private GameObject instanceSprites;
    [SerializeField] private GameObject[] points;//vetor que possui objetos vazios representando pontos de origem das linhas. NOTA: O TAMANHO DE POINTS É DEFINIDO PELO SCRIPT QUE CUSTOMIZA O INSPETOR 
    public GameObject[] Points{ get{return points;}}
    private GameObject[] objLines;
    private GameObject objLinesContainer;
    private LineRenderer[] lines;
    private int numImagensInCanvas;
    private int numTexts;
    private int numSprites;
    public enum TypeOfInformation{texts3D, sprites,canvasOfImages}
    [SerializeField] private TypeOfInformation typeOfInformation;
    [SerializeField] private bool useLine=true;
    public bool UseLine { get{return useLine;} set{useLine = value;}}
    [SerializeField] private Color startColorLine = Color.white;
    [SerializeField] private Color endColorLine = Color.white;
    [SerializeField] private float startWidthLine= 0.03f;
    [SerializeField] private float endWidthLine= 0.03f;
    private Material myMaterial;
    private bool isChangingQuestion = false;//variavel para indicar quando esta sendo mudado duma pergunta a outra, ela é colocada em true quando se executa ChangeQuestion na clase QuestionController, ou seja quando se esta mudando de pergunta.NOTA: nesse frame nao deve ser executado o codido de ativar e desativar elementos(images ou textos) e linhas, e esta variavel é usada para controlar isso.
    public bool IsChangingQuestion{ get {return isChangingQuestion;} set {isChangingQuestion = value;}}
    [SerializeField] private Renderer[] myObjectRenderers;
    [SerializeField] private bool useIconsForPoints = true;
    public bool UseIconsForPoints { get{return useIconsForPoints;}}

    private void initializeLine(int i)
    {
        lines[i].positionCount = 2;
        if (myMaterial != null)//no caso de nao existir o material LineMaterial na pasta Resources,entao nao se faz a atribuicao do material e fica o codigo ainda executavel com linhas de cor default        
            lines[i].material = myMaterial; 
        lines[i].startColor = startColorLine;
        lines[i].endColor = endColorLine;
        lines[i].startWidth = startWidthLine;
        lines[i].endWidth = endWidthLine;
    }

    private void setLines()//atualizar as linhas para ir do ponto origem até seu elemento respetivo (texto ou imagem)
    {
        for(int i = 0; i < objLines.Length; i++)
        {
            lines[i].positionCount = 2;//define-se dois pontos para a linha(ponto origem definido atraves de points e o ponto final na posicao de cada elemento info(Images,texts3D,ou sprites)
            lines[i].SetPosition(0, Points[i].transform.position);
            if(typeOfInformation == TypeOfInformation.canvasOfImages)
            {
                lines[i].SetPosition(1, images[i].transform.position);
            }
            else if(typeOfInformation == TypeOfInformation.texts3D)
            {
                lines[i].SetPosition(1, Texts3D[i].transform.position);
            }
            else if (typeOfInformation == TypeOfInformation.sprites)
            {
                lines[i].SetPosition(1, sprites[i].transform.position); 
            }
            lines[i].startColor = startColorLine;
            lines[i].endColor = endColorLine;
            lines[i].startWidth = startWidthLine;
            lines[i].endWidth = endWidthLine;
        }
    }

     
    private void Awake()
    {
        myObjectRenderers = MyObject.GetComponentsInChildren<Renderer>();
        //CODIGO RELACIONADO COM: INTERACAO MARCADORES
        marker1NewTrackableEventHandlerComponent = Marker1.GetComponent<NewTrackableEventHandler>();
        marker2NewTrackableEventHandlerComponent = Marker2.GetComponent<NewTrackableEventHandler>();
        //MENSAGEM ERRO MARKER SEM COMPONENTE NewTrackableEventHandler
        if (marker1NewTrackableEventHandlerComponent==null || marker2NewTrackableEventHandlerComponent == null)
        {
            if (marker1NewTrackableEventHandlerComponent == null)
            {
                Debug.LogError("AR Educational Framework Message: The ImageTarget: " + Marker1.name + " doesn't have NewTrackableEventHandler component. you need to substitute DefaultTrackableEventHandler component for it.");
            }
            if(marker2NewTrackableEventHandlerComponent == null)
            {
                Debug.LogError("AR Educational Framework Message: The ImageTarget: " + Marker2.name + " doesn't have NewTrackableEventHandler component. you need to substitute DefaultTrackableEventHandler component for it.");
            }
            this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
            return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
        }
        //NOVO MENSAGEM ERRO OBJECT IS NOT CHILD OF MARKER
        if (!MyObject.transform.IsChildOf(Marker1.transform))
        {
            Debug.LogError("AR Educational Framework Message: The ImageTarget: " + Marker1.name + " doesn't have child " + MyObject.name + " in the Unity hierarchy. you need to make the object: " + MyObject.name + ", child of the marker: " + Marker1.name+".");
            this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
            return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
        }
    }

    // Use this for initialization
    void Start() {

        myMaterial = Resources.Load("LineMaterial", typeof(Material)) as Material;//obtem o material de nome LineMaterial que deve estar num arquivo chamado Resources, este material é necessario permitir usar linhas com cor, no caso de nao serem usados as linhas ficarao por default purple
        if (myMaterial == null)//no caso de nao existir o material LineMaterial na pasta Resources, entao se envia mensagem de erro
        {
            Debug.LogError("AR Educational Framework Message: The Resource: LineMaterial is not found in the folder Resources, lines will draw only in default color.");
        }

        if (typeOfInformation == TypeOfInformation.canvasOfImages)//FOI ESCOLHIDO CANVASOFIMAGES
        {
            //INICIALIZAR
            //NOTA: é necessario um objeto instanceCanvas temporario com objetos vazios nas posicoes das images. este canvas será filho de myObject e portanto seus objetos vazios herdarao o comportamento de myObject (rotando, escalando, e se movimentando). finalemente as images usarao as posicoes dos objetos vazios.
            //deve-se notar que as images poderiam ser colocadas diretamente como filhas do myObject, porem, nesse caso elas mudariam seu tamanho quando o myObject mudar o dele, o que nao seria desejavel, para evitar isso é que foi criado o instanceCanvas e seus objetos vazios como intermediarios entre a relacao de heranca de myObject e as images.
            instanceCanvas = new GameObject("InstanceCanvas");//criamos um objeto temporario para armazenar objetos empty encarregados de ter as posicoes que usarao as images
            instanceCanvas.transform.parent = MyObject.transform;
            instanceCanvas.SetActive(false);//desativamos instanceCanvas

            if (MyCanvas.renderMode != RenderMode.WorldSpace)//MENSAGEM DE ERRO, CANVAS NAO CONFIGURADO A World Space EM Render Mode
            {
                Debug.LogError("AR Educational Framework Message: The Canvas: " + MyCanvas.name + " is not setting to World Space in the Render Mode option. the Canvas is only supported for 3D images, therefore you need to change the Canvas to World Space.");
                this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
                return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
            }

            numImagensInCanvas = MyCanvas.transform.childCount;//se obtem o numero de images a usar a partir do numero de filhos de Mycanvas (NOTA: é necessario que o Canvas arrastado em MyCanvas, tenha unicamente os elementos images e nao otros elementos)
            images = new Transform[numImagensInCanvas];//vector que comtem as images a usar                                           
            imagesT = new Transform[numImagensInCanvas];//vector temporario que comtem os objetos empty com as posicoes das images

            if (UseLine)
            {
                objLinesContainer = new GameObject("objLinesContainerFor" + MyObject.name + "Images");
                objLinesContainer.transform.parent = Marker1.transform;
                objLines = new GameObject[numImagensInCanvas];//cria vector de objetos linhas que possue objetos vazios com as linhas (ou componentes LineRenderer)
                lines = new LineRenderer[numImagensInCanvas];//cria o vector das linhas (ou componentes LineRenderer)
            }

            if (MyCanvas.transform.parent == MyObject.transform)//desparentar myCanvas se fosse filho de myObject
            {
                MyCanvas.transform.SetParent(Marker1.transform);//fazemos o myCanvas filho do marcador1, marcador vinculado a myObject (o myObject é filho deste marcador). NOTA: foi necessario usar SetParent ao inves de atribuir atraves da propriedade (assim: MyCanvas.transform.parent) devido a aviso do unity. 
            }

            for (int i = 0; i < numImagensInCanvas; i++)
            {
                //RELACIONADO COM IMAGENS
                images[i] = MyCanvas.transform.GetChild(i);//obtemos as images, contendo elas num vetor de Transforms

                new GameObject("InstanceImage" + (i + 1)).transform.parent = instanceCanvas.transform;//criar os objetos vazios filhos de istanceCanvas temporario,e que irao a manter as posicoes das images
                imagesT[i] = instanceCanvas.transform.GetChild(i);//atribui os filhos de istanceCanvas no vector
                imagesT[i].position = images[i].position;//inicializar posicao de objetos filhos de instanceCanvas na posicao das images

                //LINHA
                if (UseLine)
                {
                    objLines[i] = new GameObject("OLineImage" + (i + 1));//criar os objetos linha (objetos que comtem as linhas ou LineRenderer)            
                    objLines[i].transform.parent = objLinesContainer.transform;
                    objLines[i].AddComponent<LineRenderer>();//criar a linha ou componente LineRenderer para cada objeto linha
                    lines[i] = objLines[i].GetComponent<LineRenderer>();//enchemos o vector lines com as linhas (ou componente LineRenderer) de cada objeto linha

                    Points[i].transform.parent = MyObject.transform;//fazemos pontos herdar do myObject para que possam seguir ele quando se movimentar 
                    initializeLine(i);
                }
            }

        }
        else if (typeOfInformation == TypeOfInformation.texts3D)//FOI ESCOLHIDO TEXTS3D, PROCESSO ANALOGO AO ANTERIOR
        {
            //INICIALIZAR
            instanceTexts = new GameObject("InstanceTexts3D");
            instanceTexts.transform.parent = MyObject.transform;
            instanceTexts.SetActive(false);

            numTexts = Texts3D.Length;
            texts3DT = new Transform[numTexts];

            if (UseLine)
            {
                objLinesContainer=new GameObject("objLinesContainerFor"+MyObject.name+"Texts3D");
                objLinesContainer.transform.parent = Marker1.transform;
                objLines = new GameObject[numTexts];
                lines = new LineRenderer[numTexts];
            }

            for (int i = 0; i < numTexts; i++)
            {
                // RELACIONADO COM TEXTOS
                Texts3D[i].anchor = TextAnchor.MiddleCenter;//INICIALIZAR OS TEXTS NA OPCAO ANCHOR: MIDDLE CENTER

                if (Texts3D[i].transform.parent == MyObject.transform)//desparentar textos se fossem filhos de myObject
                {
                    Texts3D[i].transform.parent = Marker1.transform;//fazemos os textos filhos do marcador1, marcador vinculado a myObject (o myObject é filho deste marcador).
                }

                new GameObject("InstanceText3D" + (i + 1)).transform.parent = instanceTexts.transform;
                texts3DT[i] = instanceTexts.transform.GetChild(i);
                texts3DT[i].position = Texts3D[i].transform.position;

                //LINHA
                if (UseLine)
                {
                    objLines[i] = new GameObject("OLineText3D" + (i + 1));
                    objLines[i].transform.parent = objLinesContainer.transform;
                    objLines[i].AddComponent<LineRenderer>(); 
                    lines[i] = objLines[i].GetComponent<LineRenderer>();

                    Points[i].transform.parent = MyObject.transform;
                    initializeLine(i);
                }
            }

        }
        else if(typeOfInformation==TypeOfInformation.sprites)//FOI ESCOLHIDO SPRITES, PROCESSO ANALOGO AO ANTERIOR
        {
            //INICIALIZAR
            instanceSprites = new GameObject("instanceSprites");
            instanceSprites.transform.parent = MyObject.transform;
            instanceSprites.SetActive(false);

            numSprites = sprites.Length;
            spritesT = new Transform[numSprites];

            if (UseLine)
            {
                objLinesContainer = new GameObject("objLinesContainerFor" + MyObject.name + "Sprites");
                objLinesContainer.transform.parent = Marker1.transform;
                objLines = new GameObject[numSprites];
                lines = new LineRenderer[numSprites];
            }

            for (int i = 0; i < numSprites; i++)
            {
                // RELACIONADO COM SPRITES
                if (sprites[i].transform.parent == MyObject.transform)//desparentar sprites se fossem filhos de myObject
                {
                    sprites[i].transform.parent = Marker1.transform;//fazemos os sprites filhos do marcador1, marcador vinculado a myObject (o myObject é filho deste marcador).
                }

                new GameObject("InstanceSprite" + (i + 1)).transform.parent = instanceSprites.transform;
                spritesT[i] = instanceSprites.transform.GetChild(i);
                spritesT[i].position = sprites[i].transform.position;

                //LINHA
                if (UseLine)
                {
                    objLines[i] = new GameObject("OLineSprite" + (i + 1));
                    objLines[i].transform.parent = objLinesContainer.transform;
                    objLines[i].AddComponent<LineRenderer>();
                    lines[i] = objLines[i].GetComponent<LineRenderer>();

                    Points[i].transform.parent = MyObject.transform;
                    initializeLine(i);
                }
            }

        }

    }

    // Update is called once per frame
    void Update () {

        //CODIGO RELACIONADO COM: INTERACAO MARCADORES
        marker1Found = marker1NewTrackableEventHandlerComponent.MarkerFound;
        marker2Found = marker2NewTrackableEventHandlerComponent.MarkerFound;
        MarkersFound();
        
        if (UseLine)
        {
            setLines();//atualiza as linhas para ir do ponto origem até seu elemento respetivo
        }

        if (typeOfInformation == TypeOfInformation.canvasOfImages)//FOI ESCOLHIDO CANVASOFIMAGES
        {
            for (int i = 0; i < images.Length; i++)
            {
                  images[i].transform.position = imagesT[i].transform.position;//posiciona as images de myCanvas na posicao dos objetos emty
            }
            if (informationFollowARCamera)
                InfoFollowARCamera(images);
        }
        else if (typeOfInformation == TypeOfInformation.texts3D)//FOI ESCOLHIDO TEXTS3D
        {
            for (int i = 0; i < Texts3D.Length; i++)
            {
                 Texts3D[i].transform.position = texts3DT[i].transform.position;//posiciona os texts3D na posicao dos objetos emty 
            }
            if (informationFollowARCamera)
                InfoFollowARCamera(Texts3D);
        }
        else if (typeOfInformation == TypeOfInformation.sprites)//FOI ESCOLHIDO SPRITES
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].transform.position = spritesT[i].transform.position;//posiciona os sprites na posicao dos objetos emty 
            }
            if (informationFollowARCamera)
                InfoFollowARCamera(sprites);
        }

    }

    private void LateUpdate()
    {
        StartCoroutine(DisableAndEnableInfo());
    }

    private bool AllRendererAreEnable()//retorna true se todos os componentes Renderer de myObject estao enable (no caso de ter Renderer)
    {
        if (myObjectRenderers.Length > 0)//myObject tem renderer 
        {
                foreach (Renderer renderer in myObjectRenderers)
                {
                    if (!renderer.enabled)//tem algum renderer disable
                    {
                        return false;
                    }
                }
                return true;//todos os renderer foram enable
        }
        else//myObject nao tem renderer
        {
            return true;
        }
    }

    private IEnumerator DisableAndEnableInfo()//DESATIVAR E ATIVAR INFO(TEXTS3D, SPRITES OU IMAGES) E LINHAS, DEPENDENDO TAMBÉM DO ESTADO DE ATIVACAO DO OBJETO
    {
        if (!IsChangingQuestion)//se no momento, nao esta sendo mudada a pergunta por outra. esta variavel será mudada desde componentes QuestionController quando estiver sendo mudada a pergunta para evitar executar este codigo nesse momento.
        {
                if (typeOfInformation == TypeOfInformation.canvasOfImages)//FOI ESCOLHIDO CANVASOFIMAGES. desativar e ativar images
                {
                    if (MyObject.activeSelf == true && AllRendererAreEnable())//se o myObject estiver ativado
                    {
                        //NOTA: o uso de isChangingQuestion modificada desde QuestionController, assim como o WaitForEndOfFrame() é conveniente para ativar os elementos info e linhas e evitar alguns defeitos trocando de estados ou perguntas com elementos comuns reusados.

                        if (markersFound)//se marcadores juntos
                        {
                            yield return new WaitForEndOfFrame();//delay conveniente para ativar os elementos info e linhas.
                            //ativar elementos info e linhas
                            MyCanvas.gameObject.SetActive(true);
                            for (int i = 0; i < images.Length; i++)
                            {
                                if (UseLine)
                                    objLines[i].SetActive(true);
                            }
                        }
                        else//se marcadores isolados
                        {
                            //desativar elementos info e linhas
                            MyCanvas.gameObject.SetActive(false);
                            for (int i = 0; i < images.Length; i++)
                            {
                                if (UseLine)
                                    objLines[i].SetActive(false);
                            }
                        }
                    }
                    else//se myObject estiver desativado
                    {
                        //desativar elementos info e linhas
                        MyCanvas.gameObject.SetActive(false);
                        for (int i = 0; i < images.Length; i++)
                        {
                            if (UseLine)
                                objLines[i].SetActive(false);
                        }
                    }
                }
                else if (typeOfInformation == TypeOfInformation.texts3D)//FOI ESCOLHIDO TEXTS3D, desativar e ativar texts3D
                {
                    if (MyObject.activeSelf == true && AllRendererAreEnable())//se o myObject estiver ativado
                    {
                        if (markersFound)//se marcadores juntos
                        {
                            yield return new WaitForEndOfFrame();//delay conveniente para ativar os elementos info e linhas
                            //ativar elementos info e linhas
                            for (int i = 0; i < Texts3D.Length; i++)
                            {
                                Texts3D[i].gameObject.SetActive(true);
                                if (UseLine)
                                    objLines[i].SetActive(true);
                            }
                        }
                        else//se marcadores isolados
                        {
                            //desativar elementos info e linhas
                            for (int i = 0; i < Texts3D.Length; i++)
                            {
                                Texts3D[i].gameObject.SetActive(false);
                                if (UseLine)
                                    objLines[i].SetActive(false);
                            }
                        }
                    }
                    else//se o myObject estiver desativado 
                    {
                        //desativar elementos info e linhas
                        for (int i = 0; i < Texts3D.Length; i++)
                        {
                            Texts3D[i].gameObject.SetActive(false);
                            if (UseLine)
                                objLines[i].SetActive(false);
                        }
                    }
                }
                else if (typeOfInformation == TypeOfInformation.sprites)//FOI ESCOLHIDO SPRITE, desativar e ativar sprites
                {
                    if (MyObject.activeSelf == true && AllRendererAreEnable())//se o myObject estiver ativado
                    {
                        if (markersFound)//marcadores juntos
                        {
                            yield return new WaitForEndOfFrame();//delay conveniente para ativar os elementos info e linhas.
                            //ativar elementos info e linhas
                            for (int i = 0; i < sprites.Length; i++)
                            {
                                sprites[i].gameObject.SetActive(true);
                                if (UseLine)
                                    objLines[i].SetActive(true);
                            }
                        }
                        else// marcadores isolados
                        {
                            //desativar elementos info e linhas
                            for (int i = 0; i < sprites.Length; i++)
                            {
                                sprites[i].gameObject.SetActive(false);
                                if (UseLine)
                                    objLines[i].SetActive(false);
                            }
                        }
                    }
                    else//se o myObject estiver desativado 
                    {
                        //desativar elementos info e linhas
                        for (int i = 0; i < sprites.Length; i++)
                        {
                            sprites[i].gameObject.SetActive(false);
                            if (UseLine)
                                objLines[i].SetActive(false);
                        }
                    }
                }
        }
        else
        {
            IsChangingQuestion = false;
        }     
    }

    //CODIGO RELACIONADO COM: INTERACAO MARCADORES
    private void MarkersFound()//atualizar a variavel markersFound para indicar se os dois marcadores estao juntos ou isolados
    {
        if (marker1Found && marker2Found)//ambos marcadores sao encontrados
        {
            markersFound = true;
        }
        else//nenhum ou só um dos marcadores está sendo encontrado
        {
            markersFound = false;
        }
    }

    private void InfoFollowARCamera(Component [] vecInfo)//metodo para a informacao seguir a ARCamera.NOTA: foi conveniente colocar o tipo do vector de um tipo mais geral do que transform para suportar o uso do metodo com maior numero das classes do vector (p.ex tipo TextMesh)
    {
        for (int i = 0; i < vecInfo.Length; i++)
        {
            Quaternion direction = Quaternion.LookRotation(vecInfo[i].transform.position - MyARCamera.transform.position);
            vecInfo[i].transform.rotation = Quaternion.Lerp(vecInfo[i].transform.rotation, direction, informationSpeed * Time.deltaTime);
            //modo 2 adicional (NOTA: este modo é funcional também, porém nao permite ajustar a velocidade)
            /*vecInfo[i].transform.LookAt(MyARCamera.transform.position);
            vecInfo[i].transform.Rotate(0, 180, 0);*/
        }
    }

}
