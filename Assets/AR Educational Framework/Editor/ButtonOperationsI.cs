using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonOperations))]
[CanEditMultipleObjects]
public class ButtonOperationsI : Editor {

    SerializedProperty typeOfOperation, objects, markers, typeOfScale, scaleSpeed, maximumScale, minimumScale, typeOfRotation, rotationSpeed, axisOfRotation, senseOfRotationInX, senseOfRotationInY, senseOfRotationInZ;

    private void OnEnable()
    {
        typeOfOperation = serializedObject.FindProperty("typeOfOperation");//NOTA-SE QUE SE ACESSA NO CAMPO typeOfOperation  E NAO NA PROPRIEDADE TypeOfOperation 
        objects = serializedObject.FindProperty("objects"); 
        markers = serializedObject.FindProperty("markers");
        typeOfScale = serializedObject.FindProperty("typeOfScale");
        scaleSpeed = serializedObject.FindProperty("scaleSpeed");
        maximumScale = serializedObject.FindProperty("maximumScale");
        minimumScale = serializedObject.FindProperty("minimumScale");
        typeOfRotation = serializedObject.FindProperty("typeOfRotation");
        rotationSpeed = serializedObject.FindProperty("rotationSpeed");
        axisOfRotation = serializedObject.FindProperty("axisOfRotation");
        senseOfRotationInX = serializedObject.FindProperty("senseOfRotationInX");
        senseOfRotationInY = serializedObject.FindProperty("senseOfRotationInY");
        senseOfRotationInZ = serializedObject.FindProperty("senseOfRotationInZ");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(typeOfOperation);

        if (objects.arraySize < 1)//permitir minimo 1 objeto no vector objects, ou seja precisa-se pelo menos um elemento.
        {
            objects.arraySize = 1;
        }
        EditorGUILayout.PropertyField(objects, true);
        markers.arraySize = objects.arraySize;
        EditorGUILayout.PropertyField(markers, true);

        ButtonOperations.TypeOfOperations typeO = (ButtonOperations.TypeOfOperations)typeOfOperation.enumValueIndex;
        if (typeO == ButtonOperations.TypeOfOperations.rotate)//se tipo de operacao escolhido for rotacionar
        {
            EditorGUILayout.PropertyField(typeOfRotation);

            ButtonOperations.TypeOfRotation typeR = (ButtonOperations.TypeOfRotation)typeOfRotation.enumValueIndex;//obtemos o tipo de rotacao escolhido
            if (typeR == ButtonOperations.TypeOfRotation.inAxis)//se tipo de rotacao for em eixo, e nao reset
            {
                if (rotationSpeed.floatValue < 0)//se a velocidade da rotacao for negativa, entao se coloca em 0
                {
                    rotationSpeed.floatValue = 0;
                }
                EditorGUILayout.PropertyField(rotationSpeed);
                EditorGUILayout.PropertyField(axisOfRotation);
                ButtonOperations.AxisOfRotation axisR = (ButtonOperations.AxisOfRotation)axisOfRotation.enumValueIndex;//obtemos o eixo de rotacao escolhido (X, Y OU Z)
                if(axisR== ButtonOperations.AxisOfRotation.X)//se eixo de rotacao for X
                {
                    EditorGUILayout.PropertyField(senseOfRotationInX);//mostar sentidos de rotacao para rotacao em X (up ou down)
                }
                else if(axisR == ButtonOperations.AxisOfRotation.Y)//se eixo de rotacao for Y
                {
                    EditorGUILayout.PropertyField(senseOfRotationInY);//mostar sentidos de rotacao para rotacao em Y (right ou left)
                }
                else if(axisR == ButtonOperations.AxisOfRotation.Z)//se eixo de rotacao for Z
                {
                    EditorGUILayout.PropertyField(senseOfRotationInZ);//mostar sentidos de rotacao para rotacao em Z (pisitive ou negative)
                }
            }
        }
        else if (typeO == ButtonOperations.TypeOfOperations.scale)//se tipo de operacao escolhido for escalar
        {
            EditorGUILayout.PropertyField(typeOfScale);

            ButtonOperations.TypeOfScale typeS = (ButtonOperations.TypeOfScale)typeOfScale.enumValueIndex;//obtemos o tipo de escala escolhido (aumentar, diminuir ou reset)
            if (typeS == ButtonOperations.TypeOfScale.increase)//se foi escolhido aumentar escala
            {
                if (scaleSpeed.floatValue < 0)//se a velocidade da escala for negativa, entao se coloca em 0
                {
                    scaleSpeed.floatValue = 0;
                }
                EditorGUILayout.PropertyField(scaleSpeed);

                if (maximumScale.floatValue <= 1)//se a escala maxima for menor ou igual do que 1 (nao for maior do que 1)
                {
                    Debug.LogWarning("AR Educational Framework Message: The maximumScale must be greater than 1, it will be setting to default value 2 when is not a valid value.");
                    maximumScale.floatValue = 2f;//é colocado no valor por default (o dobro da escala do objeto)
                }
                EditorGUILayout.PropertyField(maximumScale);
            }
            else if (typeS == ButtonOperations.TypeOfScale.decrease)//se foi escolhido diminuir escala
            {

                if (scaleSpeed.floatValue < 0)//se a velocidade da escala for negativa, entao se coloca em 0
                {
                    scaleSpeed.floatValue = 0;
                }
                EditorGUILayout.PropertyField(scaleSpeed);

                if (minimumScale.floatValue <= 0 || minimumScale.floatValue >= 1)//se a escala minima for maior ou igual do que 1, ou menor ou igual do que 0
                {
                    Debug.LogWarning("AR Educational Framework Message: The minimumScale must be greater than 0 and less than 1, it will be setting to default value 0.5 when is not a valid value.");
                    minimumScale.floatValue = 0.5f;//é colocado em valor por default (a metade da escala do objeto)         
                }
                EditorGUILayout.PropertyField(minimumScale);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

}
