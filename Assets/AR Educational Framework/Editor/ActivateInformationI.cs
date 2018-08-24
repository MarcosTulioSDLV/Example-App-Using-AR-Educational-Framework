using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActivateInformation))]
[CanEditMultipleObjects]
public class ActivateInformationI : Editor{

    SerializedProperty marker1, marker2, myObject, informationFollowARCamera, myARCamera, informationSpeed, myCanvas, texts3D, sprites, points, typeOfInformation, useLine, startWidthLine, endWidthLine, startColorLine, endColorLine, useIconsForPoints;
    private ActivateInformation activateInformation;

    private void OnEnable()
    {
        activateInformation = (ActivateInformation)target;//NOTA:foi necessario para acessar ao MyCanvas e obter o numero dos seus filhos, o que permitirá inicializar o vetor dos pontos (points) no valor obtido

        marker1 = serializedObject.FindProperty("marker1");
        marker2 = serializedObject.FindProperty("marker2");
        myObject = serializedObject.FindProperty("myObject");
        informationFollowARCamera = serializedObject.FindProperty("informationFollowARCamera");
        myARCamera = serializedObject.FindProperty("myARCamera");
        informationSpeed = serializedObject.FindProperty("informationSpeed");
        myCanvas = serializedObject.FindProperty("myCanvas");//notar que se acessa ao campo myCanvas, e nao a propriedade MyCanvas.
        texts3D = serializedObject.FindProperty("texts3D");
        sprites = serializedObject.FindProperty("sprites");
        points = serializedObject.FindProperty("points");
        typeOfInformation = serializedObject.FindProperty("typeOfInformation");
        useLine = serializedObject.FindProperty("useLine");
        startWidthLine = serializedObject.FindProperty("startWidthLine");
        endWidthLine = serializedObject.FindProperty("endWidthLine");
        startColorLine = serializedObject.FindProperty("startColorLine");
        endColorLine = serializedObject.FindProperty("endColorLine");
        useIconsForPoints = serializedObject.FindProperty("useIconsForPoints");
    }

    private void DrawLineOptions()
    {
        EditorGUILayout.PropertyField(startWidthLine);
        EditorGUILayout.PropertyField(endWidthLine);
        EditorGUILayout.PropertyField(startColorLine);
        EditorGUILayout.PropertyField(endColorLine);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(marker1);
        EditorGUILayout.PropertyField(marker2);
        EditorGUILayout.PropertyField(myObject);
        EditorGUILayout.PropertyField(informationFollowARCamera);
        if (informationFollowARCamera.boolValue)
        {
            EditorGUILayout.PropertyField(myARCamera);
            EditorGUILayout.PropertyField(informationSpeed);

            if (informationSpeed.floatValue < 0)//nao permitir valores negativos para a velocidade da informacao para seguir a camera
            {
                informationSpeed.floatValue = 0;
            }
        }
        EditorGUILayout.PropertyField(typeOfInformation);
        ActivateInformation.TypeOfInformation type = (ActivateInformation.TypeOfInformation)typeOfInformation.enumValueIndex;
        if (type == ActivateInformation.TypeOfInformation.texts3D)
        {
            EditorGUILayout.PropertyField(texts3D, true);
            EditorGUILayout.PropertyField(useLine);

            if (texts3D.arraySize < 1)//permitir minimo 1 objeto no vector texts3D, ou seja precisa-se pelo menos um elemento texts3D.
            {
                texts3D.arraySize = 1;
            }

            if (useLine.boolValue == true)
            {
                points.arraySize = texts3D.arraySize;//estabelece tamanho do vector de pontos(points) para linhas, dependendo do tamanho do vector de texts3D
                EditorGUILayout.PropertyField(points, true);
                EditorGUILayout.PropertyField(useIconsForPoints);

                DrawLineOptions();
            }
        }
        else if(type == ActivateInformation.TypeOfInformation.sprites)
        {
            EditorGUILayout.PropertyField(sprites, true);
            EditorGUILayout.PropertyField(useLine);

            if (sprites.arraySize < 1)//permitir minimo 1 objeto no vector sprites, ou seja precisa-se pelo menos um elemento sprites.
            {
                sprites.arraySize = 1;
            }

            if (useLine.boolValue == true)
            {
                points.arraySize = sprites.arraySize;//estabelece tamanho do vector de pontos (points) para linhas, dependendo do tamanho do vector de sprites
                EditorGUILayout.PropertyField(points, true);
                EditorGUILayout.PropertyField(useIconsForPoints);

                DrawLineOptions();
            }
        }
        else if(type== ActivateInformation.TypeOfInformation.canvasOfImages)
        {
            EditorGUILayout.PropertyField(myCanvas);
            EditorGUILayout.PropertyField(useLine);

            if (useLine.boolValue == true)
            {
                if (activateInformation.MyCanvas != null)//se canvas já atribuido no campo, entao pode se estabelecer o tamaho do vetor de pontos (points)
                {
                    points.arraySize = activateInformation.MyCanvas.transform.childCount;//estabelece o tamanho do vetor de pontos (points) para linhas, dependendo do numero de filhos do canvas (deveria ser o numero de images a usar)
                }
                else//se canvas nao atribuido no campo, o tamanho do vetor de pontos (points) fica sempre em 0
                {
                    points.arraySize = 0;
                }
                EditorGUILayout.PropertyField(points, true);
                EditorGUILayout.PropertyField(useIconsForPoints);

                DrawLineOptions();
            }
        }

        //MENSAGEM : ALGUM OBJETO QUE NAO É GAMEOBJECT EMPTY ESTÁ SENDO ARRASTADO NOS CAMPOS Points (só gameobjects empty deberiam ser arrastados)
        if (useLine.boolValue)
        {
            if (activateInformation.Points != null)
                foreach (GameObject point in activateInformation.Points)
                {
                    if (point != null)
                    {
                        Component[] componentsInPoint = point.GetComponentsInChildren<Component>();//obtemos os componentes no objeto point (e nos seus filhos no caso de ter)
                        if (componentsInPoint.Length > 1)
                        {
                            Debug.LogWarning("AR Educational Framework Message: The object: " + point.name + " is not a empty GameObject. you should to add only empty GameObjects in Points fields.");
                        }
                    }
                }
        }

        serializedObject.ApplyModifiedProperties();
    }

    //PERMITE CRIAR ICONE CUSTOMIZADO AUTOMATICAMENTE QUANDO FOR ARRASTADO O GAMEOBJECT EMPTY NO CAMPO Points
    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Active)]
    static void DrawGizmoForMyScript(ActivateInformation scr, GizmoType gizmoType)
    {
         if (scr.UseLine && scr.UseIconsForPoints)//se está ativada a opcao usar linhas e usar icono para pontos 
         {
              if (scr.Points != null)//se vetor já está inicializado
              {
                  if (scr.MyObject != null)//se o objeto principal ja foi atribuido no campo (nesse caso, terá que validar uma condicao a mais para criar os gizmos(ou icones))
                  {
                      if (scr.MyObject.activeSelf)//se o objeto esta ativado
                      {
                          //criar os gizmos (ou icones)
                          foreach (GameObject point in scr.Points)
                          {
                              if (point != null && point.activeSelf)//se o ponto (gameobject vazio que representa ele) já foi atribuido no campo, e está ativado
                              {
                                  Gizmos.DrawIcon(point.transform.position, "Gizmo1", false);//criar os gizmos (ou icones)
                              }
                          }
                      }
                  }
                  else//se objeto principal ainda nao foi atribuido no campo
                  {
                      //criar os gizmos (ou icones)
                      foreach (GameObject point in scr.Points)
                      {
                          if (point != null && point.activeSelf)//se o ponto (gameobject vazio que representa ele) já foi atribuido no campo, e está ativado
                          {
                              Gizmos.DrawIcon(point.transform.position, "Gizmo1", false);//criar os gizmos (ou icones)
                          }
                      }
                  }
              }
         }
    }


}
