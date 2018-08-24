using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ChangeElement))]
[CanEditMultipleObjects]
public class ChangeElementI : Editor {

    ChangeElement changeElement;
    SerializedProperty marker1, marker2, subObjectsWithMaterial, subObjectsWithMaterialO, myObject;
    SerializedProperty typeOfChange, changeScale, changeRotation, changeMaterials, newScale, newRotation, newObject; 

    private void OnEnable()
    {
        changeElement = (ChangeElement)target;//NOTA:FOI NECESSARIO CRIAR changeElement, POIS NAO FOI POSSIVEL OBTER O COMPONENTE RENDERER ATRAVES DE subObjectsWithMaterial NO METODO SetSizeNewMaterialsVector().

        marker1 = serializedObject.FindProperty("marker1");
        marker2 = serializedObject.FindProperty("marker2");
        subObjectsWithMaterial = serializedObject.FindProperty("subObjectsWithMaterial");//NOTA-SE QUE SE ESTA ACESSANDO AO CAMPO subObjectsWithMaterial E NAO A PROPRIEDADE SubObjectsWithMaterial
        subObjectsWithMaterialO = serializedObject.FindProperty("subObjectsWithMaterialO");
        myObject = serializedObject.FindProperty("myObject");
        typeOfChange = serializedObject.FindProperty("typeOfChange");
        changeScale = serializedObject.FindProperty("changeScale");
        changeRotation = serializedObject.FindProperty("changeRotation");
        changeMaterials = serializedObject.FindProperty("changeMaterials");
        newScale = serializedObject.FindProperty("newScale");
        newRotation = serializedObject.FindProperty("newRotation");
        newObject = serializedObject.FindProperty("newObject");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(marker1);
        EditorGUILayout.PropertyField(marker2);
        EditorGUILayout.PropertyField(myObject);

        EditorGUILayout.PropertyField(typeOfChange);
        ChangeElement.TypeOfChanges type = (ChangeElement.TypeOfChanges)typeOfChange.enumValueIndex;
        if(type == ChangeElement.TypeOfChanges.ChangeObjectProperties)//se quer mudar só propriedades do objeto
        {
            EditorGUILayout.PropertyField(changeScale);
            if (changeScale.boolValue == true)//se quer mudar a escala do objeto
            {
                EditorGUILayout.PropertyField(newScale);
            }
            EditorGUILayout.PropertyField(changeRotation);
            if (changeRotation.boolValue == true)//se quer mudar a rotacao do objeto
            {
                EditorGUILayout.PropertyField(newRotation);
            }
            EditorGUILayout.PropertyField(changeMaterials);
            if (changeMaterials.boolValue == true)//se quer mudar cor (ou materiais) do objeto
            {
                SetSizeNewMaterialsVector();
                if (subObjectsWithMaterial.arraySize < 1)//permitir minimo tamanho 1, no vector de sub-objetos subObjectsWithMaterial
                {
                    subObjectsWithMaterial.arraySize = 1;
                }
                EditorGUILayout.PropertyField(subObjectsWithMaterial, true);
                subObjectsWithMaterialO.arraySize = subObjectsWithMaterial.arraySize;//ajustar tamaho do vetor subObjectsWithMaterialO dependendo do vetor subObjectsWithMaterial
            }
        }
        else if(type== ChangeElement.TypeOfChanges.ChangeObject)//se quer trocar o objeto por outro
        {
            EditorGUILayout.PropertyField(newObject);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void SetSizeNewMaterialsVector()
    {
        int numSubObjectsWithMaterial = changeElement.SubObjectsWithMaterial.Length;//NOTA: VALIDO TAMBÉM ATRAVES DE subObjectsWithMaterial ASSIM:  subObjectsWithMaterial.arraySize;
        for (int i = 0; i < numSubObjectsWithMaterial; i++)
        {
            if (changeElement.SubObjectsWithMaterial[i].SubObjectWithMaterial != null)//SE JÁ FOI ATRIBUIDO O SUB-OBJETO //NOTA: VALIDO TAMBÉM ATRAVES DE subObjectsWithMaterial ASSIM: subObjectsWithMaterial.GetArrayElementAtIndex(i).FindPropertyRelative("subObjectWithMaterial") != null 
            {
                Renderer componentRenderer = changeElement.SubObjectsWithMaterial[i].SubObjectWithMaterial.GetComponent<Renderer>();//NOTA:NESTA LINHA NAO FOI POSSIVEL OBTER O COMPONENTE RENDERER ATRAVES DE subObjectsWithMaterial deste classe diretamente, PORTANTO FOI NECESSARIO CRIAR changeElement
                if (componentRenderer != null)//se o sub-objeto tem componente Renderer, ou seja vetor de materiais
                {
                    //NOTA: É USADO sharedMaterials AO INVES DE materials pois este codigo customiza o inspetor (no caso de usar material aqui da erro)
                    int materialsVectLenght = componentRenderer.sharedMaterials.Length;//obtem o tamanho do vetor de materiais
                    subObjectsWithMaterial.GetArrayElementAtIndex(i).FindPropertyRelative("materialsVector").arraySize= materialsVectLenght;//ajusta o tamanho do vetor materialsVector igual ao tamanho obtido
                }
                else//se o sub-objeto nao tem componente Renderer
                {
                    if (changeElement.SubObjectsWithMaterial[i].SubObjectWithMaterial.Equals(changeElement.MyObject) && (changeElement.MyObject.GetComponentsInChildren<Renderer>().Length>0))//se o objeto atribuido for o mesmo do que o sub-objeto atribuido, e tem filhos com componente Renderer, entao envia uma mensagem mais especifica pois se garante que esse objeto tem componentes Renderer em seus filhos e nao nele diretamente.
                    {
                        Debug.LogError("AR Educational Framework Message: The Object: " + changeElement.SubObjectsWithMaterial[i].SubObjectWithMaterial.name + " doesn't have Renderer component with materials for change. you need to add in the SubObjectWithMaterial field, a child of the object " + changeElement.MyObject.name + " with a Renderer component.");//NOTA: VALIDO TAMBÉM ATRAVES DE subObjectsWithMaterial ASSIM: subObjectsWithMaterial.GetArrayElementAtIndex(i).FindPropertyRelative("subObjectWithMaterial").name
                    }
                    else
                    {
                        Debug.LogError("AR Educational Framework Message: The Object: " + changeElement.SubObjectsWithMaterial[i].SubObjectWithMaterial.name + " doesn't have Renderer component with materials for change.");//NOTA: VALIDO TAMBÉM ATRAVES DE subObjectsWithMaterial ASSIM: subObjectsWithMaterial.GetArrayElementAtIndex(i).FindPropertyRelative("subObjectWithMaterial").name
                    }
                    subObjectsWithMaterial.GetArrayElementAtIndex(i).FindPropertyRelative("materialsVector").arraySize = 0;//coloca o tamanho do vetor materialsVector em 0         
                }
            }
            else//SE AINDA NAO FOI FOI ATRIBUIDO O SUB-OBJETO
            {
                subObjectsWithMaterial.GetArrayElementAtIndex(i).FindPropertyRelative("materialsVector").arraySize = 0;//coloca o tamanho do vetor materialsVector em 0.NOTA: acessando a subObjectsWithMaterial deste jeito, permitiu ao vetor no modo de edicao funcionar melhor do que acessando do outro jeito
            }
        }
    }

}
