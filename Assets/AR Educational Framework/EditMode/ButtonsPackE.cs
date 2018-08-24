using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ButtonsPackE : MonoBehaviour {

    private GameObject marker;
    private GameObject objectCube;
    [HideInInspector] public bool firstTime = true;//variavel para executar uma unica ves o codigo, lembrando que está sendo usado [ExecuteInEditMode]
    private ButtonsPack[] buttonsPackComponents;

    private void Awake()
    {
        buttonsPackComponents = gameObject.GetComponents<ButtonsPack>();
    }

    // Use this for initialization
    void Start()
    {
        if (!Application.isPlaying)
        {
            if (firstTime)
            {

                if (buttonsPackComponents.Length > 0)
                {
                    for (int i = 0; i < buttonsPackComponents.Length; i++)
                    {
                        //CRIAR INSTANCIA DO MARCADOR
                        Object resourceImageTarget1Prefab = Resources.Load("ImageTarget1Prefab");//obtem o prefab do ImageTarget ImageTarget1Prefab, que está num arquivo Resources
                        if (resourceImageTarget1Prefab != null)
                        {
                            //criar uma instancia do prefabs e inicializar ela
                            marker = (GameObject)GameObject.Instantiate(resourceImageTarget1Prefab);
                            marker.name = "ImageTarget";
                            //criar um objeto para teste
                            objectCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            objectCube.transform.localPosition = marker.transform.localPosition;
                            //ajustar a posicao em Y do objeto
                            Vector3 positionT = objectCube.transform.localPosition;
                            positionT.y += 0.65f;
                            objectCube.transform.localPosition = positionT;
                            //colocar objeto como filho do marcador
                            objectCube.transform.SetParent(marker.transform);
                        }
                        else
                        {
                            Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ImageTarget1Prefab is not found in the folder Resources.");
                        }

                        //acrescenta automaticamente o marcador e seu objeto, nos campos da classe ButtonOperations do botao criado ResetRotationButton
                        buttonsPackComponents[i].ResetRotationButton.GetComponent<ButtonOperations>().Objects[0] = objectCube;
                        buttonsPackComponents[i].ResetRotationButton.GetComponent<ButtonOperations>().Markers[0] = marker;

                        //acrescenta automaticamente o marcador e seu objeto, nos campos da classe ButtonOperations do botao criado LeftRotationButton
                        buttonsPackComponents[i].LeftRotationButton.GetComponent<ButtonOperations>().Objects[0] = objectCube;
                        buttonsPackComponents[i].LeftRotationButton.GetComponent<ButtonOperations>().Markers[0] = marker;

                        //acrescenta automaticamente o marcador e seu objeto, nos campos da classe ButtonOperations do botao criado RightRotationButton
                        buttonsPackComponents[i].RightRotationButton.GetComponent<ButtonOperations>().Objects[0] = objectCube;
                        buttonsPackComponents[i].RightRotationButton.GetComponent<ButtonOperations>().Markers[0] = marker;

                        //acrescenta automaticamente o marcador e seu objeto, nos campos da classe ButtonOperations do botao criado UpRotationButton
                        buttonsPackComponents[i].UpRotationButton.GetComponent<ButtonOperations>().Objects[0] = objectCube;
                        buttonsPackComponents[i].UpRotationButton.GetComponent<ButtonOperations>().Markers[0] = marker;

                        //acrescenta automaticamente o marcador e seu objeto, nos campos da classe ButtonOperations do botao criado DownRotationButton
                        buttonsPackComponents[i].DownRotationButton.GetComponent<ButtonOperations>().Objects[0] = objectCube;
                        buttonsPackComponents[i].DownRotationButton.GetComponent<ButtonOperations>().Markers[0] = marker;

                        //acrescenta automaticamente o marcador e seu objeto, nos campos da classe ButtonOperations do botao criado ResetScaleButton
                        buttonsPackComponents[i].ResetScaleButton.GetComponent<ButtonOperations>().Objects[0] = objectCube;
                        buttonsPackComponents[i].ResetScaleButton.GetComponent<ButtonOperations>().Markers[0] = marker;

                        //acrescenta automaticamente o marcador e seu objeto, nos campos da classe ButtonOperations do botao criado IncreaseScaleButton
                        buttonsPackComponents[i].IncreaseScaleButton.GetComponent<ButtonOperations>().Objects[0] = objectCube;
                        buttonsPackComponents[i].IncreaseScaleButton.GetComponent<ButtonOperations>().Markers[0] = marker;

                        //acrescenta automaticamente o marcador e seu objeto, nos campos da classe ButtonOperations do botao criado DecreaseScaleButton
                        buttonsPackComponents[i].DecreaseScaleButton.GetComponent<ButtonOperations>().Objects[0] = objectCube;
                        buttonsPackComponents[i].DecreaseScaleButton.GetComponent<ButtonOperations>().Markers[0] = marker;
                    }
                }
                else
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject: " + gameObject.name + " doesn't have ButtonsPack component. you need to add it before add this class.");
                }

                firstTime = false;
            }
        }
    }

}
