using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChangeElement : MonoBehaviour {

    [SerializeField] private GameObject marker1;
    public GameObject Marker1 { get{return marker1;} set{marker1 = value;}}
    [SerializeField] private GameObject marker2;
    public GameObject Marker2 { get{return marker2;} set{marker2 = value;}}
    [SerializeField] private SubObjectContainerWithMaterial[] subObjectsWithMaterial = new SubObjectContainerWithMaterial[1];//USADA PARA MANTER SUB-OBJETOS E MATERIAIS ASSOCIADOS.
    public SubObjectContainerWithMaterial[] SubObjectsWithMaterial{ get {return subObjectsWithMaterial;}}
    [SerializeField] private SubObjectContainerWithMaterial[] subObjectsWithMaterialO = new SubObjectContainerWithMaterial[1];//USADA PARA ARMAZENAR OS MATERIAIS ORIGINAIS DOS SUB-OBJETOS E ASSIM PODER FAZER RESET NELES, NOTA:no script de customizar o inspetor (ChangeElementI), se ajusta o tamaho do subObjectsWithMaterialO dependendo de subObjectsWithMaterial 
    [SerializeField] private GameObject myObject;
    public GameObject MyObject { get { return myObject; } set { myObject = value;}}
    [SerializeField] private bool marker1Found = false;
    [SerializeField] private bool marker2Found = false;
    [SerializeField] private bool markersFound =false;
    private NewTrackableEventHandler marker1NewTrackableEventHandlerComponent;
    private NewTrackableEventHandler marker2NewTrackableEventHandlerComponent;
    private bool firstFrameColorChanged=true;
    public enum TypeOfChanges{ChangeObjectProperties,ChangeObject}
    [SerializeField] private TypeOfChanges typeOfChange;
    public TypeOfChanges TypeOfChange{ get { return typeOfChange;}}
    [SerializeField] private bool changeScale;
    public bool ChangeScale { get {return changeScale;}}
    [SerializeField] private bool changeRotation;
    public bool ChangeRotation { get {return changeRotation;}}
    [SerializeField] private bool changeMaterials;
    public bool ChangeMaterials { get {return changeMaterials;} set {changeMaterials = value;}}
    [SerializeField] private Vector3 newScale=new Vector3(1,1,1); 
    [SerializeField] private Vector3 newRotation;
    [SerializeField] private GameObject newObject;
    public GameObject NewObject{ get { return newObject;}}
    private Quaternion originalRotation;
    public Quaternion OriginalRotation{ get {return originalRotation;} set {originalRotation = value;}}
    private Vector3 originalScaleObject;
    public Vector3 OriginalScaleObject{ get { return originalScaleObject;} set{originalScaleObject = value;}}
    private bool firstFrameMarkersFound=true;
    private bool firstFrameMarkersFound2 = true;
    private List<GameObject> originalObjects = new List<GameObject>();
    private List<GameObject> emtyObjects = new List<GameObject>();
    private GameObject emtyObj;
    public GameObject EmtyObj { get {return emtyObj;}}
    private Vector3 scaleFactor;//escala em porcentagem
    private Vector3 newScaleT;
    //ATRIBUTOS RELACIONADOS COM BLOQUEAR BOTOES QUANDO MARCADORES JUNTOS MUDANDO ESCALA
    private ButtonOperations[] objectsWithButtonOperations;//conterá botoes que podem estar escalando objetos
    private List<ButtonOperations> objectsWithButtonOperationsF = new List<ButtonOperations>();//conterá botoes que estao escalando objetos, isto permitirá evitar seu uso para um objeto (que já muda sua escala atraves de ChangeElement) enquando estiver marcadores juntos 
    [SerializeField] private Renderer[] myObjectRenderers;
    [SerializeField] private Renderer[] newObjectRenderers;
    //ATRIBUTOS RELACIONADOS BLOQUEAR OUTROS CHANGEELEMENT QUANDO UN DELES JÁ ESTÁ TROCANDO O OBJETO 
    [SerializeField] private bool otherChangeElementComponentIsChangingMyObject=false;
    public bool OtherChangeElementComponentIsChangingMyObject{get{return otherChangeElementComponentIsChangingMyObject;} set{otherChangeElementComponentIsChangingMyObject = value;}}
    private ChangeElement[] changeElementComponents;
    private List<ChangeElement> othersChangeElementComponentsChangingMyObject = new List<ChangeElement>();
    private bool firstFrameMarkersFound3 = true;

    public void GetOriginalMaterials()//obter os materias inicias (ou atuais, no caso de mudar eles na execucao), que vao permitir inicializar o myObject a seu estado após isolar os marcadores
    {
        //INICIALIZA subObjectsWithMaterialO, com os materials originais do objeto
        for (int i = 0; i < subObjectsWithMaterialO.Length; i++)//NOTA:subObjectsWithMaterialO tem o mesmo tamanho do que subObjectsWithMaterial pois foi ajustado já na classe ChangeElementI.
        {
            subObjectsWithMaterialO[i].MaterialsVector = new Material[SubObjectsWithMaterial[i].MaterialsVector.Length];//define o tamaho dos vetores materialsVector de subObjectsWithMaterialO tais como os de subObjectsWithMaterial.

            GameObject objectWithMaterialA = SubObjectsWithMaterial[i].SubObjectWithMaterial;//obtemos o objeto SubObjectWithMaterial que serao usado para busca dos vetores de materiais 

            //OBTER RENDERER
            Renderer componentRenderer = objectWithMaterialA.GetComponent<Renderer>();
            if (componentRenderer != null)
            {
                subObjectsWithMaterialO[i].MaterialsVector = componentRenderer.materials;//NOTA:subObjectsWithMaterialO fica com uma copia dos vectores de materiais originais (embora material seja uma classe, e as classes sao atribuidas por referencia), com a classe Material quando se faz uma atribuicao se faz internamente um clone dos elementos (se faz uma copia), para evitar isso usar Renderer.sharedMaterial 
            }
            else
            {
                Debug.LogError("AR Educational Framework Message: The Object: " + objectWithMaterialA.name + " doesn't have Renderer component.");
            }
        }
    }

    private void Awake()
    {
        //CODIGO RELACIONADO COM: INTERACAO MARCADORES
        marker1NewTrackableEventHandlerComponent = Marker1.GetComponent<NewTrackableEventHandler>();
        marker2NewTrackableEventHandlerComponent = Marker2.GetComponent<NewTrackableEventHandler>();
        //MENSAGEM ERRO MARKER SEM COMPONENTE NewTrackableEventHandler
        if (marker1NewTrackableEventHandlerComponent == null || marker2NewTrackableEventHandlerComponent == null)
        {
            if (marker1NewTrackableEventHandlerComponent == null)
            {
                Debug.LogError("AR Educational Framework Message: The ImageTarget: " + Marker1.name + " doesn't have NewTrackableEventHandler component. you need to substitute DefaultTrackableEventHandler component for it.");
            }
            if (marker2NewTrackableEventHandlerComponent == null)
            {
                Debug.LogError("AR Educational Framework Message: The ImageTarget: " + Marker2.name + " doesn't have NewTrackableEventHandler component. you need to substitute DefaultTrackableEventHandler component for it.");
            }
            this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
            return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
        }
        //MENSAGEM ERRO OBJECT IS NOT CHILD OF MARKER
        if (!MyObject.transform.IsChildOf(Marker1.transform))
        {
            Debug.LogError("AR Educational Framework Message: The ImageTarget: " + Marker1.name + " doesn't have child " + MyObject.name + " in the Unity hierarchy. you need to make the object: " + MyObject.name + ", child of marker: " + Marker1.name+".");
            this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
            return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
        }
        //MENSAGEM DE ERRO SUB-OBJETO NAO É FILHO DO OBJETO
        if (TypeOfChange == TypeOfChanges.ChangeObjectProperties)
        {
            if (ChangeMaterials)
            {
                bool anySubObjectWithMaterialIsNotChildOfMyObject = false;
                for (int i = 0; i < SubObjectsWithMaterial.Length; i++)
                {
                    GameObject subObjectWithMaterialA = SubObjectsWithMaterial[i].SubObjectWithMaterial;//obtemos o sub-objeto com material atual

                    if (!subObjectWithMaterialA.transform.IsChildOf(MyObject.transform))
                    {
                        Debug.LogError("AR Educational Framework Message: The object: " +subObjectWithMaterialA.name+ " is not valid. you need to add a child of the object " +MyObject.name+ " with a Renderer component, or the object " +MyObject.name+ " itself if it is a primitive object from Unity with its Renderer component directly on it (e.g Cube, Sphere etc).");
                        anySubObjectWithMaterialIsNotChildOfMyObject = true;
                    }
                }
                if (anySubObjectWithMaterialIsNotChildOfMyObject)
                {
                    this.enabled = false;// desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
                    return;// nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
                }
            }
        }

        //CODIGO RELACIONADO COM: TROCAR OBJETO POR OUTRO
        //ACRESCENTA NA LISTA renderers do componente NewTrackableEventHandler do marker1, os renderer dos objetos myObject e o novo objeto newObject a trocar. isto pois o marker1 vai pular a ativacao desses renderer quando for vissivel na camera(nao ira ativar esses objetos quando for encontrado), ao inves disso, serao ativados diretamente por este codigo. NOTA: quando o marcador nao encontrado, aí sim marker1 vai desativar normalmente esses renderer.
        //---
        if (TypeOfChange == TypeOfChanges.ChangeObject && this.enabled)
        {
            myObjectRenderers = MyObject.GetComponentsInChildren<Renderer>();
            newObjectRenderers = NewObject.GetComponentsInChildren<Renderer>();
            if (myObjectRenderers.Length > 0 && newObjectRenderers.Length > 0)//SÓ se o myObject e newObject tem componentes Renderer ira funcionar a caracteristica de trocar o objeto pelo outro
            {
                foreach (Renderer renderer in myObjectRenderers)
                {
                    if (!marker1NewTrackableEventHandlerComponent.Renderers.Contains(renderer))
                        marker1NewTrackableEventHandlerComponent.Renderers.Add(renderer);
                }
                foreach (Renderer renderer in newObjectRenderers)
                {
                    if (!marker1NewTrackableEventHandlerComponent.Renderers.Contains(renderer))
                        marker1NewTrackableEventHandlerComponent.Renderers.Add(renderer);
                }

                //MENSAGEM ERRO OBJECT IS NOT CHILD OF MARKER
                if (!NewObject.transform.IsChildOf(Marker1.transform))
                {
                    Debug.LogError("AR Educational Framework Message: The ImageTarget: " + Marker1.name + " doesn't have child " + NewObject.name + " in the Unity hierarchy. you need to make the object: " + NewObject.name + ", child of marker: " + Marker1.name + ".");
                    this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
                    return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
                }
            }
            else
            {
                if (myObjectRenderers.Length == 0)
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject: " + MyObject.name + " doesn't have any Renderer component.");
                }
                if (newObjectRenderers.Length == 0)
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject: " + NewObject.name + " doesn't have any Renderer component.");
                }

                this.enabled = false;//desativa o componente atual, porém ainda para este frame terminaria de executar o codigo de embaixo, e portanto usa-se return.
                return;//nao executar o codigo de embaixo para este frame, em conjunto com a linha anterior, apartir de aqui, é desativado o script atual e nao se executa o codigo de embaixo nem para terminar este frame.
            }
        }
        //---
    
        if (typeOfChange == TypeOfChanges.ChangeObjectProperties)
        {
            if (changeScale)
            {
                //CODIGO RELACIONADO COM: BLOQUEAR BOTOES QUANDO MARCADORES JUNTOS MUDANDO ESCALA
                objectsWithButtonOperations = GameObject.FindObjectsOfType<ButtonOperations>();//obtem todos os componentes de tipo ButtonOperations, ou seja deve obter (implicitamente) os botoes que podem mudar escala 
                foreach (ButtonOperations obj in objectsWithButtonOperations)//percorre o vector e coloca numa lista só os componentes que sao usados para escalar(nao pra rotacionar)
                {
                    if (obj.TypeOfOperation == ButtonOperations.TypeOfOperations.scale)//se está sendo usado para escalar
                    {
                        objectsWithButtonOperationsF.Add(obj);
                    }
                }
                //----

                //CRIAR O OBJETO EMPTY PARA O objeto myObject (NO CASO DE NAO TER) E ACRESCENTAR ELE NA LISTA GERAL DE OBJETOS EMPTY INTERMEDIARIOS USADOS PARA ESCALAR OBJETOS PROPORCIONALMENTE
                //NOTA: PARA ESTE TRECHO DE CODIGO, FOI NECESSARIO QUE O MARKER1 FOSSE CONSIDERADO COMO MARCADOR VINCULADO AO OBJETO E MARKER2 COMO MARCADOR DE CONTROLE
                List<GameObject> originalObjects = new List<GameObject>();
                foreach (GeralObjectsContainer geralObjectContainer in ButtonOperations.GeralObjectsContainerList)//PERCORRE E OBTEM LISTA GERAL DE OBJETOS USADOS PARA ESCALAR PROPORCIONALMENTE
                {
                    originalObjects.Add(geralObjectContainer.OriginalObject);
                }
                if (!originalObjects.Contains(MyObject))//nao foi acrescentado o myObject atual na lista geral (ou seja nao sera mudada sua transform atraves de botoes), entao neste codigo deve se acrescentar nessa lista geral para mudar escala com marcadores
                {
                    GameObject objEmty = new GameObject(MyObject.name + "Emty");//cria um objeto emty novo com o mesmo nome do que o myObject atual
                    objEmty.transform.localPosition = Marker1.transform.localPosition;//posiciona ele no mesmo lugar do marcador atual
                    objEmty.transform.parent = Marker1.transform;//fazer myObject emty filho do marcador, necessario para nao tirar o myObject do seu contexto inicial quando for colocado como filho do emty
                    objEmty.transform.localScale = Vector3.one;//IMPORTANTE RESET O VALOR A 1,1,1 POIS SE O MARCADOR NAO TEM TANAHO 1,1,1, O OBJ EMTY QUANDO SE FAZ FILHO DO MARCADOR FICARIA COM OUTRO TAMANHO E DARIA ERRADO AS OPERACOES MATEMATICAS DO ALGORITMO

                    //CODIGO RELACIONADO COM: SUPORTAR IMAGENS EM CANVAS PARA SEREM ESCALADAS
                    Transform myParent = MyObject.transform.parent;
                    if (!(UnityEngine.Object.ReferenceEquals(myParent,Marker1)) && (myParent.GetComponent<Canvas>() != null))//se pai do objeto myObject nao for marcador e for um canvas
                    {
                        objEmty.transform.SetParent(myParent.transform);//fazer o objEmty filho do canvas
                        objEmty.transform.localScale = Vector3.one;//IMPORTANTE RESET O VALOR A 1,1,1 POIS SE O CANVAS NAO TEM TANAHO 1,1,1, O OBJ EMTY QUANDO SE FAZ FILHO DO CANVAS FICARIA COM OUTRO TAMANHO E DARIA ERRADO AS OPERACOES MATEMATICAS DO ALGORITMO
                    }
                    //---
                    MyObject.transform.SetParent(objEmty.transform);//fazer myObject filho do objeto empty

                    GeralObjectsContainer geralObjectN = new GeralObjectsContainer(MyObject, objEmty);//acrescenta na lista geral myObject e seu empty criado
                    ButtonOperations.GeralObjectsContainerList.Add(geralObjectN);
                }

            }
        }

        //CODIGO RELACIONADO COM: BLOQUEAR OUTROS CHANGEELEMENT QUANDO UN DELES jÁ ESTA TROCANDO O OBJETO 
        if (myObjectRenderers.Length > 0 && newObjectRenderers.Length > 0)//SÓ se o myObject e newObject tem componentes Renderer ira funcionar a caracteristica de trocar o objeto pelo outro
        {
            changeElementComponents = GameObject.FindObjectsOfType<ChangeElement>();
            foreach (ChangeElement changeElement in changeElementComponents)
            {
                if (!(UnityEngine.Object.ReferenceEquals(changeElement,this)) && changeElement.TypeOfChange == ChangeElement.TypeOfChanges.ChangeObject && UnityEngine.Object.ReferenceEquals(changeElement.MyObject,MyObject))//se o componente changeElement nao for o atual, e estiver mudando o mesmo objeto que este 
                {
                    othersChangeElementComponentsChangingMyObject.Add(changeElement);//acrescenta na lista othersChangeElementComponentsChangingMyObject os changeElement que podem mudar o objeto, exetuando o atual changeElement
                }
            }
        }

    }

    // Use this for initialization
    void Start () {

        if (TypeOfChange == TypeOfChanges.ChangeObjectProperties)//esta escolhida a opcao de mudar propriedades 
        {
            if (ChangeScale)//esta escolhida a opcao mudar transform
            {   
                OriginalRotation = MyObject.transform.localRotation;    

                scaleFactor.x = newScale.x / MyObject.transform.localScale.x;
                scaleFactor.y = newScale.y / MyObject.transform.localScale.y;
                scaleFactor.z = newScale.z / MyObject.transform.localScale.z;

                //OBTER O OBJETO EMPTY VINCULADO AO OBJETO myObject
                foreach (GeralObjectsContainer geralObjectContainer in ButtonOperations.GeralObjectsContainerList)//obtem em listas os objetos gerais e seus objetos empty respetivos que vao ser escalados por botoes ou por marcadores de controle
                {
                     originalObjects.Add(geralObjectContainer.OriginalObject);
                     emtyObjects.Add(geralObjectContainer.EmtyObject);
                }
                for (int y = 0; y < originalObjects.Count; y++)//percorremos a lista geral para encontrar o indice do myObject nela e obter o objeto empty vinculado a ele
                {
                      if (UnityEngine.Object.ReferenceEquals(MyObject, originalObjects[y]))//quando seja encontrado o myObject na lista, entao o objeto emty desse indice deve ser escalado (emty vinculado a esse objeto)
                      {
                          emtyObj = emtyObjects[y];//acrescenta em emtyObj o objeto emty vinculado ao myObject que deve ser escalado ao inves dele para permitir escala proporcional
                      }
                }
                OriginalScaleObject = EmtyObj.transform.localScale;//guarda escala do objeto emty    
            }
        }
    }

    private void LateUpdate()
    {
        if (TypeOfChange == TypeOfChanges.ChangeObjectProperties)
        {    
             if (ChangeScale)//esta escolhida a opcao de mudar scale
             {
                    if (markersFound)//se marcadores juntos, entao mudar a escala do objeto
                    {
                        if (firstFrameMarkersFound2)
                        {
                            OriginalScaleObject = EmtyObj.transform.localScale;//guardar escala do objeto emty                 

                            EmtyObj.transform.localScale = Vector3.one;//reset tamanho do empty antes de fazer as operacoes, pois se tiver outro tamanho, nao daria o mesmo as operacoes matematicas a seguir                                                             
                            newScaleT = new Vector3(0, 0, 0);
                            newScaleT.x = scaleFactor.x * EmtyObj.transform.localScale.x;
                            newScaleT.y = scaleFactor.y * EmtyObj.transform.localScale.y;
                            newScaleT.z = scaleFactor.z * EmtyObj.transform.localScale.z;

                            firstFrameMarkersFound2 = false;
                        }
                        EmtyObj.transform.localScale = newScaleT;
                        DisableOrEnableButtonsThatChangingScaleOfMyObject(false);//evitar que botoes possam mudar escala deste objeto, quando marcadores juntos já estao mudando escala do objeto
                    }
                    else if (!firstFrameMarkersFound2)//se marcadores isolados e já entrou no trecho anterior,entao reset a escala do myObject ao estado original
                    {
                        EmtyObj.transform.localScale = OriginalScaleObject;
                        DisableOrEnableButtonsThatChangingScaleOfMyObject(true);//permitir que botoes possam voltar a mudar escala deste objeto

                        firstFrameMarkersFound2 = true;
                    }
             } 
        }
    }

    private void DisableOrEnableButtonsThatChangingScaleOfMyObject(bool value)//NOTA:COM FALSE DESABLE PARA O OBJETO DO SCRIPT(myObject) OS BOTOES QUE POSSAM MUDAR A SUA ESCALA,NOTA-SE QUE OS BOTOES NAO SERAO UTEIS PARA myObject, PORÉM SIM, PARA OS OUTROS OBJETOS QUE POSSAM MUDAR.COM TRUE RESET AO ESTADO ORIGINAL AS VARIAVIES MUDADAS.
    {
        //CODIGO RELACION COM: BLOQUEAR BOTOES QUANDO MARCADORES JUNTOS MUDANDO ESCALA, BLOQUEAR BOTOES PARA O OBJETO myObject DESTE SCRIPT,QUANDO MARCADORES JUNTOS MUDANDO ESCALA
        foreach (ButtonOperations buttonOperations in objectsWithButtonOperationsF)//percorre cada um dos componentes buttonOperation de cada botao, para saber se podem mudar o objeto deste script
        {
            GameObject[] objectsInButtonOperations = buttonOperations.Objects;//obtem o vector dos objetos de cada componente ButtonOperations de cada botao, ou seja os objetos que pode escalar esse botao.
            bool[] objectsConditionInComponetScale = buttonOperations.ObjectsCondition;//obtem o vector das condicoes de objetos de cada componente buttonOperation em cada botao, esse vector indica um valor bool para cada objeto. quando estiver em true pode ser trocada sua escala normalmente, e quando estiver em false será pulado na execucao do botao e nao escalará.

            for (int i = 0; i < objectsInButtonOperations.Length; i++)
            {
                if (UnityEngine.Object.ReferenceEquals(objectsInButtonOperations[i],MyObject))//se algum Objeto do vector de objetos do botao atual for igual ao objeto desde script, ou seja o botao pode escalar o objeto desde script.
                {
                    objectsConditionInComponetScale[i] = value;//com false, permite pular na execucao do botao a este Objeto(ver codigo em script ButtonOperations).com true permite que o objeto seja escalado normalmente pelo botao.
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
        //---

        if (TypeOfChange == TypeOfChanges.ChangeObjectProperties)//se está escolhida a opcao de mudar propriedades
        {
                if (ChangeRotation)//se está escolhida a opcao de mudar rotacao
                {
                    if (markersFound)//se marcadores juntos, entao mudar a rotacao do myObject
                    {
                        if (firstFrameMarkersFound)//armazena os valores de rotacao no primeiro frame e antes de serem trocados ao novo valor
                        {
                            OriginalRotation = MyObject.transform.localRotation;
                            firstFrameMarkersFound = false;
                        }
                        MyObject.transform.localRotation = Quaternion.Euler(newRotation);
                    }
                    else if (!markersFound && !firstFrameMarkersFound)//se marcadores isolados, entao reset a rotacao do myObject ao estado original
                    {
                        MyObject.transform.localRotation = OriginalRotation;
                        firstFrameMarkersFound = true;
                    }
                }

                if (ChangeMaterials)//esta escolhida a opcao de mudar materiais
                {                   
                    if (markersFound)//se marcadores juntos, entao muda a cor(ou materiais)
                    {
                        if (firstFrameColorChanged)
                        {   
                            GetOriginalMaterials();//no primeiro frame no qual os marcadores estao juntos, obtem os materias atuais que vao permitir inicializar o myObject a seu estado após isolar os marcadores                 
                            firstFrameColorChanged = false;
                        }

                        for (int i = 0; i < SubObjectsWithMaterial.Length; i++)//percorrer o vector subObjectsWithMaterial 
                        {
                            GameObject objectWithMaterialA = SubObjectsWithMaterial[i].SubObjectWithMaterial;//obtemos o sub-objeto atual com materiais
                            Material[] newMaterialsVectA = SubObjectsWithMaterial[i].MaterialsVector;//obtemos o vector de materiais do sub-objeto atual, que sao os materias novos

                            Renderer componentRenderer = objectWithMaterialA.GetComponent<Renderer>();
                            if (componentRenderer != null)
                            {                
                                Material[] originalMaterialsVectT = componentRenderer.materials;//vector de materiais temporal é criado, e enche-se com uma copia dos materiais originais do sob-objeto atual. //NOTA: originalMaterialsVectT fica com uma copia dos materias originais, (embora material seja uma classe, e as classes sao atribuidas por referencia), com a classe Material quando se faz uma atribuicao se faz internamente um clone dos elementos (se faz é uma copia), para evitar isso usar Renderer.sharedMaterial 

                                for (int y = 0; y < newMaterialsVectA.Length; y++)
                                {
                                    if (newMaterialsVectA[y] != null)
                                    originalMaterialsVectT[y] = newMaterialsVectA[y];//atribui o novo material no vector temporal (sempre que tiver o campo do novo material com um material atribuido, no caso de nao ter, se entende que nao quer mudar esse material e fica o mesmo), notar que newMaterialsVectA tem o mesmo tamanho que originalMaterialsVectT
                                }
                                componentRenderer.materials = originalMaterialsVectT;//atribui a copia com os novos materiais no sub-objeto atual
                            }   
                            else
                            {
                                Debug.LogError("AR Educational Framework Message: The Object: " + objectWithMaterialA.name + " doesn't have Renderer component.");
                            }                  
                        }
                    }
                    else //if (!markersFound)//se marcadores isolados, reset o cor (ou materiais)
                    {
                        if (!firstFrameColorChanged)//se já foi mudada a cor quando marcadores juntos, ou seja já entrou no codigo anterior
                        {
                            ResetObjectColor();
                            firstFrameColorChanged = true;
                        }
                    }
                }
        }
        else if(TypeOfChange == TypeOfChanges.ChangeObject)//se está escolhida a opcao de mudar o myObject
        {  
            if (myObjectRenderers.Length > 0 && newObjectRenderers.Length > 0)//só se o myObject e newObject tem componentes Renderer ira funcionar o codigo de trocar objeto por outro
            {       
                //NOTA:as duas condicoes a seguir serao para quando o marcador1 for encontrado, lembre-se que quando marcador1 nao é encontrado, o newTRackableEventHandler coloca em false todos os Renderer, incluindo os de estes elementos.
                //NOTA:quando marcador1 encontrado, este codigo está encarregado de ativar e desativar os renderer de myObject e newObject (newTRackableEventHandler que por default era encarregado dessa tarefa foi editado para nao faze-lo) 
                if (MyObject.activeSelf)//se está ativado o objeto (ou seja, é seu turno usando perguntas do framework com QuestionController)
                {
                    if (!OtherChangeElementComponentIsChangingMyObject)//variavel usada para permitir bloquear execucao de outros changeElement quando um deles esta trocando objeto
                    {
                        if (markersFound)//se marcadores juntos
                        {
                            //CODIGO RELACIONADO COM: BLOQUEAR OUTROS CHANGEELEMENT QUANDO UN DELES ESTA TROCANDO O OBJETO
                            if (firstFrameMarkersFound3)//no primeiro frame no qual dois marcadores juntos estao trocando o objeto, deve se pausar ou bloquear o codigo dos outros componentes ChangeElement que também podem trocar o mesmo objeto.
                            {
                                foreach (ChangeElement changeElement in othersChangeElementComponentsChangingMyObject)//percorre a lista othersChangeElementComponentsChangingMyObject e bloquea a execuacao dos outros changeElement que podem mudar o objeto
                                {
                                    changeElement.OtherChangeElementComponentIsChangingMyObject = true;
                                }
                                firstFrameMarkersFound3 = false;
                            }
                            //--
                            foreach (Renderer renderer in myObjectRenderers)
                            {
                                renderer.enabled = false;//desativa os Renderer do objeto original
                            }
                            foreach (Renderer renderer in newObjectRenderers)
                            {
                                renderer.enabled = true;//ativa os Renderer do novo objeto
                            }
                            NewObject.SetActive(true);//ativa o novo objeto
                        }
                        else if (marker1Found)//só marcador1 encontrado
                        {
                            foreach (Renderer renderer in myObjectRenderers)
                            {
                                renderer.enabled = true;//ativa os Renderer do objeto original
                            }
                            foreach (Renderer renderer in newObjectRenderers)
                            {
                                renderer.enabled = false;//desativa os Renderer do novo objeto
                            }
                            NewObject.SetActive(false);//desativa o novo objeto

                            //CODIGO RELACIONADO COM: BLOQUEAR OUTROS CHANGEELEMENT QUANDO UN DELES ESTA TROCANDO O OBJETO             
                            foreach (ChangeElement changeElement in othersChangeElementComponentsChangingMyObject)//reset ao estado original o valor OtherChangeElementComponentIsChangingMyObject de cada changeElement da lista, para permitir usar eles novamente
                            {
                                changeElement.OtherChangeElementComponentIsChangingMyObject = false;
                            }
                            firstFrameMarkersFound3 = true;//reset para permitir executar novamente o codigo de bloquear outros componentes changeElement
                        }
                        else//nenhum marcador encontrado (ou marcador2 de controle encontrado)
                        {
                            //CODIGO RELACIONADO COM: BLOQUEAR OUTROS CHANGEELEMENT QUANDO UN DELES ESTA TROCANDO O OBJETO
                            foreach (ChangeElement changeElement in othersChangeElementComponentsChangingMyObject)//reset ao estado original o valor OtherChangeElementComponentIsChangingMyObject de cada changeElement da lista, para permitir usar eles novamente
                            {
                                changeElement.OtherChangeElementComponentIsChangingMyObject = false;
                            }
                            firstFrameMarkersFound3 = true;//reset para permitir executar novamente o codigo de bloquear outros componentes changeElement
                        }
                    }
                }
                else//se objeto está desativado
                {
                    //reset estado inicial
                    foreach (Renderer renderer in myObjectRenderers)
                    {
                        renderer.enabled = false;//desativa os Renderer do objeto original
                    }
                    foreach (Renderer renderer in newObjectRenderers)
                    {
                        renderer.enabled = false;//desativa os Renderer do novo objeto
                    }
                    NewObject.SetActive(false);//desativa o novo objeto
                }
            }
        }

    }

    private void ResetObjectColor()
    {
        for (int i = 0; i < SubObjectsWithMaterial.Length; i++)
        {
            GameObject objectWithMaterialA = SubObjectsWithMaterial[i].SubObjectWithMaterial;
            Renderer componentRenderer = objectWithMaterialA.GetComponent<Renderer>();
            if (componentRenderer != null)
            {
                componentRenderer.materials = subObjectsWithMaterialO[i].MaterialsVector;//reatribui o vector materials inicial para cada sub-objeto
            }
            else
            {
                Debug.LogError("AR Educational Framework Message: The Object: " + objectWithMaterialA.name + " doesn't have Renderer component.");
            }
        }
    }

    //CODIGO RELACIONADO COM: INTERACAO MARCADORES
    private void MarkersFound()//atualizar a variavel markersFound para indicar se os dois marcadores estao juntos ou isolados
    {
        if (marker1Found && marker2Found)//ambos os marcadores sao encontrados
        {
            markersFound = true;
        }
        else//nenhum ou só um dos marcadores está sendo encontrado
        {
            markersFound = false;
        }
    }

}

