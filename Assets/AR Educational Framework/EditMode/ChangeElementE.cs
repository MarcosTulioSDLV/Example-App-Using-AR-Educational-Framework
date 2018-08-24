using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChangeElementE : MonoBehaviour {

    private ChangeElement[] changeElementComponents;
    [HideInInspector] public bool firstTime = true;//variavel para executar uma unica ves o codigo, lembrando que está sendo usado [ExecuteInEditMode]

    private void Awake()
    {
        changeElementComponents = gameObject.GetComponents<ChangeElement>();
    }

    // Use this for initialization
    void Start () {

        if (!Application.isPlaying)
        {
            if (firstTime)
            {
                if (changeElementComponents.Length > 0)
                {
                    for (int i = 0; i < changeElementComponents.Length; i++)
                    {
                        //ativamos o campo changeMaterials do componente ChangeElement para usar esa funcionalidade sem problemas
                        changeElementComponents[i].ChangeMaterials = true;

                        //CRIAR INSTANCIA DO MARCADOR1 
                        Object resourceImageTarget1Prefab = Resources.Load("ImageTarget1Prefab");
                        if (resourceImageTarget1Prefab != null)
                        {
                            //criar instancia do marcador
                            GameObject marker1 = (GameObject)GameObject.Instantiate(resourceImageTarget1Prefab);
                            marker1.name = "ImageTarget";

                            //criar objeto para teste
                            GameObject objectCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            objectCube.transform.localPosition = marker1.transform.localPosition;
                            //ajutar a posicao em Y
                            Vector3 positionT = objectCube.transform.localPosition;
                            positionT.y += 0.65f;
                            objectCube.transform.localPosition = positionT;
                            //colocar objeto como filho do marcador
                            objectCube.transform.SetParent(marker1.transform);

                            //acrescenta automaticamente o marcador e seu objeto, nos campos da classe ChangeElement
                            changeElementComponents[i].Marker1 = marker1;
                            changeElementComponents[i].MyObject = objectCube;

                            //acrescenta o objeto também no campo para mudar seu material
                            changeElementComponents[i].SubObjectsWithMaterial[0].SubObjectWithMaterial = objectCube;

                            //criar os novos materiais e acrescenta eles nos campos necessarios
                            Material resourceMaterial = Resources.Load("Material", typeof(Material)) as Material;
                            if (resourceMaterial != null)
                            {
                                int MaterialsVectorSize = objectCube.GetComponent<Renderer>().sharedMaterials.Length;//NOTA: É USADO sharedMaterials AO INVES DE materials pois este codigo esta sendo executado em edit mode (no caso de usar material aqui da erro)
                                                                                                                    
                                //criar os novos materiais e atribuir eles nos campos MaterialsVector
                                Material[] newMaterials = new Material[MaterialsVectorSize];
                                for (int y = 0; y < newMaterials.Length; y++)
                                {

                                    newMaterials[y] = resourceMaterial;
                                }
                                //acrescenta os materiais
                                changeElementComponents[i].SubObjectsWithMaterial[0].MaterialsVector = newMaterials;
                            }
                            else
                            {
                                Debug.LogError("AR Educational Framework Message: The Resource: Material is not found in the folder Resources.");
                            }

                        }
                        else
                        {
                            Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ImageTarget1Prefab is not found in the folder Resources.");
                        }
                        //CRIAR INSTANCIA DO MARCADOR2
                        Object resourceImageTarget2Prefab = Resources.Load("ImageTarget2Prefab");
                        if (resourceImageTarget2Prefab != null)
                        {
                            //criar instancia do marcador
                            GameObject marker2 = (GameObject)GameObject.Instantiate(resourceImageTarget2Prefab);
                            marker2.name = "ImageTarget2";
                            Vector3 positionT2 = marker2.transform.localPosition;
                            positionT2.x += 1.4f;
                            marker2.transform.localPosition = positionT2;
                            //acrescenta automaticamente o marcador, no campo da classe ChangeElement
                            changeElementComponents[i].Marker2 = marker2;
                        }
                        else
                        {
                            Debug.LogError("AR Educational Framework Message: The GameObject Prefab: ImageTarget2Prefab is not found in the folder Resources.");
                        }
                    }
                }
                else
                {
                    Debug.LogError("AR Educational Framework Message: The GameObject: " + gameObject.name + " doesn't have ChangeElement component. you need to add it before add this class.");
                }

                firstTime = false;
            }
        }

    }

}
