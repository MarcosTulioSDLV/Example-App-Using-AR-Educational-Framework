using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuestionController2))]
[CanEditMultipleObjects]
public class QuestionController2I: Editor{

    SerializedProperty active, marker, answerAlternatives, correctAnswerIndex, leftButton, rightButton, checkButton, resetButton;
    SerializedProperty useNextQuestionButton, nextQuestionButton, nextQuestion, usePreviousQuestionButton, previousQuestionButton, previousQuestion, randomOptions;
    SerializedProperty correctAnswerImage, wrongAnswerImage, useTime, timeDependingOfMarker, totalTime, timeText, useAlertTime, alertTime, newTextColor;
    SerializedProperty additionalElements0, additionalElements1, additionalElements2, useAnswerButton, answerButton, useDefaultAnswer, additionalElements3;

    private void OnEnable()
    {
        active = serializedObject.FindProperty("active");//NOTA-SE QUE ESTA SE ACESSANDO AO CAMPO active E NAO A PROPRIEDADE Active
        marker = serializedObject.FindProperty("marker");
        answerAlternatives = serializedObject.FindProperty("answerAlternatives");
        correctAnswerIndex = serializedObject.FindProperty("correctAnswerIndex");
        leftButton = serializedObject.FindProperty("leftButton");
        rightButton = serializedObject.FindProperty("rightButton");
        checkButton = serializedObject.FindProperty("checkButton");
        resetButton = serializedObject.FindProperty("resetButton");
        useNextQuestionButton = serializedObject.FindProperty("useNextQuestionButton");
        nextQuestionButton = serializedObject.FindProperty("nextQuestionButton");
        nextQuestion = serializedObject.FindProperty("nextQuestion");
        usePreviousQuestionButton = serializedObject.FindProperty("usePreviousQuestionButton");
        previousQuestionButton = serializedObject.FindProperty("previousQuestionButton");
        previousQuestion = serializedObject.FindProperty("previousQuestion");
        randomOptions = serializedObject.FindProperty("randomOptions");
        correctAnswerImage = serializedObject.FindProperty("correctAnswerImage");
        wrongAnswerImage = serializedObject.FindProperty("wrongAnswerImage");
        useTime = serializedObject.FindProperty("useTime");
        timeDependingOfMarker = serializedObject.FindProperty("timeDependingOfMarker");
        totalTime = serializedObject.FindProperty("totalTime");
        timeText = serializedObject.FindProperty("timeText");
        useAlertTime = serializedObject.FindProperty("useAlertTime");
        alertTime = serializedObject.FindProperty("alertTime");
        newTextColor = serializedObject.FindProperty("newTextColor");
        additionalElements0 = serializedObject.FindProperty("additionalElements0");
        additionalElements1 = serializedObject.FindProperty("additionalElements1");
        additionalElements2 = serializedObject.FindProperty("additionalElements2");
        useAnswerButton = serializedObject.FindProperty("useAnswerButton");
        answerButton = serializedObject.FindProperty("answerButton");
        useDefaultAnswer = serializedObject.FindProperty("useDefaultAnswer");
        additionalElements3 = serializedObject.FindProperty("additionalElements3");

        //MENSAGEM DE ERRO
        QuestionController2 questionController2 = (QuestionController2)target;
        QuestionController2[] allQuestionController2Components = questionController2.gameObject.GetComponents<QuestionController2>();//obtem todos os QuestionController2 no objeto 
        if (allQuestionController2Components.Length > 1)
        {
            Debug.LogError("AR Educational Framework Message: The GameObject: " + questionController2.name + " has several QuestionController2 component, multiple QuestionController2 component in the same GameObject is not supported. please remove and add one of them in other GameObject.");
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(active);
        EditorGUILayout.PropertyField(marker);
        if (answerAlternatives.arraySize <= 2)//permite limitar o numero de elementos alternativas(no caso modelos 3D) para cada pergunta (deve ser minimo 2).
        {
            answerAlternatives.arraySize = 2;
        }
        EditorGUILayout.PropertyField(answerAlternatives, true);

        if(correctAnswerIndex.intValue>= answerAlternatives.arraySize)
        {
            correctAnswerIndex.intValue = answerAlternatives.arraySize-1;
        }
        else if (correctAnswerIndex.intValue<0)
        {
            correctAnswerIndex.intValue = 0;
        }
        EditorGUILayout.PropertyField(correctAnswerIndex);
        EditorGUILayout.PropertyField(leftButton);
        EditorGUILayout.PropertyField(rightButton);
        EditorGUILayout.PropertyField(checkButton);
        EditorGUILayout.PropertyField(resetButton);
        EditorGUILayout.PropertyField(useNextQuestionButton);
        if (useNextQuestionButton.boolValue)
        {
            EditorGUILayout.PropertyField(nextQuestionButton);
            EditorGUILayout.PropertyField(nextQuestion);
        }
        EditorGUILayout.PropertyField(usePreviousQuestionButton);
        if(usePreviousQuestionButton.boolValue)
        {
            EditorGUILayout.PropertyField(previousQuestionButton);
            EditorGUILayout.PropertyField(previousQuestion);
        }
        EditorGUILayout.PropertyField(randomOptions);
        EditorGUILayout.PropertyField(correctAnswerImage);
        EditorGUILayout.PropertyField(wrongAnswerImage);

        EditorGUILayout.PropertyField(useTime);
        if (useTime.boolValue)//se está ativa opcao useTime (usar tempo para a pergunta)
        {
            EditorGUILayout.PropertyField(timeDependingOfMarker);

            if (totalTime.intValue <= 0) totalTime.intValue = 1;//ajustar totalTime no minimo a 1
            EditorGUILayout.PropertyField(totalTime);
            EditorGUILayout.PropertyField(timeText);

            EditorGUILayout.PropertyField(useAlertTime);
            if (useAlertTime.boolValue == true)//se está ativa opcao useAlertTime
            {
                if (alertTime.intValue >= totalTime.intValue)//ajustar alertTime para nao ultrapassar totalTime(deve ser sempre menor ou igual do que totalTime)
                {
                    alertTime.intValue = totalTime.intValue;
                }
                if (alertTime.intValue <= 0) alertTime.intValue = 1;//ajustar alertTime no minimo a 1
                EditorGUILayout.PropertyField(alertTime);
                EditorGUILayout.PropertyField(newTextColor);
            }
        }

        EditorGUILayout.PropertyField(additionalElements0, true);
        EditorGUILayout.PropertyField(additionalElements1,true);
        EditorGUILayout.PropertyField(additionalElements2,true);
        EditorGUILayout.PropertyField(useAnswerButton);
        if (useAnswerButton.boolValue)
        {
            EditorGUILayout.PropertyField(answerButton);
            EditorGUILayout.PropertyField(useDefaultAnswer);
            EditorGUILayout.PropertyField(additionalElements3,true);
        }

        serializedObject.ApplyModifiedProperties();
    }

}
