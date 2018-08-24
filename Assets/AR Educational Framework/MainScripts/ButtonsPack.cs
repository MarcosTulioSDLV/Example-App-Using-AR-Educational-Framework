using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class ButtonsPack : MonoBehaviour {

    private GameObject canvas;
    private GameObject resetRotationButton;
    public GameObject ResetRotationButton{get{return resetRotationButton;}}
    private GameObject leftRotationButton;
    public GameObject LeftRotationButton{get{return leftRotationButton;}}
    private GameObject rightRotationButton;
    public GameObject RightRotationButton{get{return rightRotationButton;}}
    private GameObject upRotationButton;
    public GameObject UpRotationButton{get{return upRotationButton;}}
    private GameObject downRotationButton;
    public GameObject DownRotationButton{get{return downRotationButton;}}
    private GameObject resetScaleButton;
    public GameObject ResetScaleButton { get { return resetScaleButton; } }
    private GameObject increaseScaleButton;
    public GameObject IncreaseScaleButton { get { return increaseScaleButton; } }
    private GameObject decreaseScaleButton;
    public GameObject DecreaseScaleButton{get{return decreaseScaleButton;}}
    [HideInInspector] public bool firstTime = true;//variavel para executar uma unica ves o codigo, lembrando que está sendo usado [ExecuteInEditMode]

    // Use this for initialization
    void Start()
    {
        if (!Application.isPlaying)
        {
            if (firstTime)
            {
                //CRIAR INSTANCIA DO CANVAS             
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
                //CRIAR BOTOES DE ROTACAO
                //CRIAR INSTANCIA DO BOTAO PARA O BOTAO RESET ROTACAO
                Object resource1ButtonPrefab = Resources.Load("ButtonOperationsPrefabs/ResetRotationButtonPrefab");
                if (resource1ButtonPrefab != null)
                {
                    //criar instancia do botao
                    resetRotationButton = (GameObject)GameObject.Instantiate(resource1ButtonPrefab);
                    ResetRotationButton.name = "ResetRotationButton";
                    ResetRotationButton.transform.SetParent(canvas.transform);
                    ResetRotationButton.transform.localPosition = new Vector3(141, -95, 0);
                }
                else
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ResetRotationButtonPrefab is not found in the folder Resources/ButtonOperationsPrefabs.");
                }
                //CRIAR INSTANCIA DO BOTAO PARA O BOTAO LEFT
                Object resource2ButtonPrefab = Resources.Load("ButtonOperationsPrefabs/LeftRotationButtonPrefab");
                if (resource2ButtonPrefab != null)
                {
                    //criar instancia do botao
                    leftRotationButton = (GameObject)GameObject.Instantiate(resource2ButtonPrefab);
                    LeftRotationButton.name = "LeftRotationButton";
                    LeftRotationButton.transform.SetParent(canvas.transform);
                    LeftRotationButton.transform.localPosition = new Vector3(-19, -95, 0);
                }
                else
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject Prefab: LeftRotationButtonPrefab is not found in the folder Resources/ButtonOperationsPrefabs.");
                }
                //CRIAR INSTANCIA DO BOTAO PARA O BOTAO RIGHT
                Object resource3ButtonPrefab = Resources.Load("ButtonOperationsPrefabs/RightRotationButtonPrefab");
                if (resource3ButtonPrefab != null)
                {
                    //criar instancia do botao
                    rightRotationButton = (GameObject)GameObject.Instantiate(resource3ButtonPrefab);
                    RightRotationButton.name = "RightRotationButton";
                    RightRotationButton.transform.SetParent(canvas.transform);
                    RightRotationButton.transform.localPosition = new Vector3(301, -95, 0);
                }
                else
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject Prefab: RightRotationButtonPrefab is not found in the folder Resources/ButtonOperationsPrefabs.");
                }
                //CRIAR INSTANCIA DO BOTAO PARA O BOTAO UP
                Object resource4ButtonPrefab = Resources.Load("ButtonOperationsPrefabs/UpRotationButtonPrefab");
                if (resource4ButtonPrefab != null)
                {
                    //criar instancia do botao
                    upRotationButton = (GameObject)GameObject.Instantiate(resource4ButtonPrefab);
                    UpRotationButton.name = "UpRotationButton";
                    UpRotationButton.transform.SetParent(canvas.transform);
                    UpRotationButton.transform.localPosition = new Vector3(141, -65, 0);
                }
                else
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject Prefab: UpRotationButtonPrefab is not found in the folder Resources/ButtonOperationsPrefabs.");
                }
                //CRIAR INSTANCIA DO BOTAO PARA O BOTAO DOWN
                Object resource5ButtonPrefab = Resources.Load("ButtonOperationsPrefabs/DownRotationButtonPrefab");
                if (resource5ButtonPrefab != null)
                {
                    //criar instancia do botao
                    downRotationButton = (GameObject)GameObject.Instantiate(resource5ButtonPrefab);
                    DownRotationButton.name = "DownRotationButton";
                    DownRotationButton.transform.SetParent(canvas.transform);
                    DownRotationButton.transform.localPosition = new Vector3(141, -125, 0);
                }
                else
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject Prefab: DownRotationButtonPrefab is not found in the folder Resources/ButtonOperationsPrefabs.");
                }
                //CRIAR INSTANCIA DO BOTAO PARA O BOTAO RESET SCALE
                Object resource6ButtonPrefab = Resources.Load("ButtonOperationsPrefabs/ResetScaleButtonPrefab");
                if (resource6ButtonPrefab != null)
                {
                    //criar instancia do botao
                    resetScaleButton = (GameObject)GameObject.Instantiate(resource6ButtonPrefab);
                    ResetScaleButton.name = "ResetScaleButton";
                    ResetScaleButton.transform.SetParent(canvas.transform);
                    ResetScaleButton.transform.localPosition = new Vector3(-252, -95, 0);
                }
                else
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ResetScaleButtonPrefab is not found in the folder Resources/ButtonOperationsPrefabs.");
                }
                //CRIAR INSTANCIA DO BOTAO PARA O BOTAO INCREASE SCALE
                Object resource7ButtonPrefab = Resources.Load("ButtonOperationsPrefabs/IncreaseScaleButtonPrefab");
                if (resource7ButtonPrefab != null)
                {
                    //criar instancia do botao
                    increaseScaleButton = (GameObject)GameObject.Instantiate(resource7ButtonPrefab);
                    IncreaseScaleButton.name = "IncreaseScaleButton";
                    IncreaseScaleButton.transform.SetParent(canvas.transform);
                    IncreaseScaleButton.transform.localPosition = new Vector3(-252, -65, 0);
                }
                else
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject Prefab: IncreaseScaleButtonPrefab is not found in the folder Resources/ButtonOperationsPrefabs.");
                }
                //CRIAR INSTANCIA DO BOTAO PARA O BOTAO DECREASE SCALE
                Object resource8ButtonPrefab = Resources.Load("ButtonOperationsPrefabs/DecreaseScaleButtonPrefab");
                if (resource8ButtonPrefab != null)
                {
                    //criar instancia do botao
                    decreaseScaleButton = (GameObject)GameObject.Instantiate(resource8ButtonPrefab);
                    DecreaseScaleButton.name = "DecreaseScaleButton";
                    DecreaseScaleButton.transform.SetParent(canvas.transform);
                    DecreaseScaleButton.transform.localPosition = new Vector3(-252, -125, 0);
                }
                else
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject Prefab: DecreaseScaleButtonPrefab is not found in the folder Resources/ButtonOperationsPrefabs.");
                }

                firstTime = false;
            }
        }
    }

}
