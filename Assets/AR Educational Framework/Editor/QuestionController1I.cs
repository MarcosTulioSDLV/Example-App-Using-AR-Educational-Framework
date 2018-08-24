using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuestionController1))]
[CanEditMultipleObjects]
public class QuestionController1I: Editor{

    SerializedProperty active, useMarker, marker, useQuestionObjects, questionObjects, typeOfAnswerAlternatives, texts, images, correctAnswerIndex, useTextBackGround, textBackGroundImage, alternativesDependingOfMarker;
    SerializedProperty resetButton, useNextQuestionButton, nextQuestionButton, nextQuestion, usePreviousQuestionButton, previousQuestionButton, previousQuestion;
    SerializedProperty randomOptions, correctAnswerImage, wrongAnswerImage, useTime, timeDependingOfMarker, totalTime, timeText, useAlertTime;
    SerializedProperty alertTime, newTextColor, additionalElements0, additionalElements1, additionalElements2, useAnswerButton;
    SerializedProperty answerButton, useDefaultAnswer, correctAnswerImage2, additionalElements3;

    private void OnEnable()
    {
        active = serializedObject.FindProperty("active");//NOTA-SE QUE ESTA SE ACESSANDO AO CAMPO active E NAO A PROPRIEDADE Active
        useMarker = serializedObject.FindProperty("useMarker");
        marker = serializedObject.FindProperty("marker");
        useQuestionObjects = serializedObject.FindProperty("useQuestionObjects");
        questionObjects = serializedObject.FindProperty("questionObjects");
        typeOfAnswerAlternatives = serializedObject.FindProperty("typeOfAnswerAlternatives");
        texts = serializedObject.FindProperty("texts");
        images = serializedObject.FindProperty("images");
        correctAnswerIndex = serializedObject.FindProperty("correctAnswerIndex");
        alternativesDependingOfMarker = serializedObject.FindProperty("alternativesDependingOfMarker");
        useTextBackGround = serializedObject.FindProperty("useTextBackGround");
        textBackGroundImage = serializedObject.FindProperty("textBackGroundImage");
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
        correctAnswerImage2 = serializedObject.FindProperty("correctAnswerImage2");
        additionalElements3 = serializedObject.FindProperty("additionalElements3");

        //MENSAGEM DE ERRO
        QuestionController1 questionController1 = (QuestionController1)target;
        QuestionController1[] allQuestionController1Components = questionController1.gameObject.GetComponents<QuestionController1>();//obtem todos os QuestionController1 no objeto 
        if (allQuestionController1Components.Length > 1)
        {
            Debug.LogError("AR Educational Framework Message: The GameObject: " + questionController1.name + " has several QuestionController1 component, multiple QuestionController1 component in the same GameObject is not supported. please remove and add one of them in other GameObject.");
        }
    }

    public override void OnInspectorGUI()
    {

        serializedObject.Update();

        EditorGUILayout.PropertyField(active);
        EditorGUILayout.PropertyField(useMarker);
        if (useMarker.boolValue)
        {
            EditorGUILayout.PropertyField(marker);
            EditorGUILayout.PropertyField(useQuestionObjects);
            if (useQuestionObjects.boolValue)
            {
                if (questionObjects.arraySize <= 0)
                {
                    questionObjects.arraySize = 1;
                }
                EditorGUILayout.PropertyField(questionObjects, true);
            }
        }
        EditorGUILayout.PropertyField(typeOfAnswerAlternatives);

        QuestionController1.TypeOfAnswerAlternatives typeOfElementTemp = (QuestionController1.TypeOfAnswerAlternatives)typeOfAnswerAlternatives.enumValueIndex;
        if (typeOfElementTemp == QuestionController1.TypeOfAnswerAlternatives.Texts)
        {
            Limit(texts);//permite limitar o numero de elementos alternativas(no caso texts) para cada pergunta (deve ser minimo 2), e o indice para que esteja no rango das alternativas.
            EditorGUILayout.PropertyField(texts, true);           
        }
        else if (typeOfElementTemp == QuestionController1.TypeOfAnswerAlternatives.Images)
        {
            Limit(images);//permite limitar o numero de elementos alternativa(no caso images) para cada pergunta (deve ser minimo 2), e o indice para que esteja no rango das alternativas.
            EditorGUILayout.PropertyField(images, true); 
        } 

        EditorGUILayout.PropertyField(correctAnswerIndex);
        if (useMarker.boolValue)
        {
            EditorGUILayout.PropertyField(alternativesDependingOfMarker);
        }

        if (typeOfElementTemp == QuestionController1.TypeOfAnswerAlternatives.Texts)
        {
            EditorGUILayout.PropertyField(useTextBackGround);
            if (useTextBackGround.boolValue)
            {
                EditorGUILayout.PropertyField(textBackGroundImage);
            }
        }
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
        if (useTime.boolValue)//se esta ativa opcao useTime (usar tempo para a pergunta)
        {
            if (useMarker.boolValue)
            {
                EditorGUILayout.PropertyField(timeDependingOfMarker);
            }

            if (totalTime.intValue <= 0) totalTime.intValue = 1;//ajustar totalTime no minimo a 1
            EditorGUILayout.PropertyField(totalTime);
            EditorGUILayout.PropertyField(timeText);

            EditorGUILayout.PropertyField(useAlertTime);

            if (useAlertTime.boolValue == true)//se está ativa opcao useAlertTime 
            {
                if(alertTime.intValue>= totalTime.intValue)//ajustar alertTime para nao ultrapassar totalTime(deve ser sempre menor ou igual do que totalTime)
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
            if (useDefaultAnswer.boolValue)
            {
                EditorGUILayout.PropertyField(correctAnswerImage2);
            }
            EditorGUILayout.PropertyField(additionalElements3, true);
        }

        serializedObject.ApplyModifiedProperties();

    }

    private void Limit(SerializedProperty element)//permite limitar o numero de elementos alternativa(texts ou images) para cada pergunta (deve ser minimo 2), e o indice para que esteja no rango das alternativas.
    {
        if (element.arraySize <= 2)
        {
            element.arraySize = 2;
        }

        if (correctAnswerIndex.intValue >= element.arraySize)
        {
            correctAnswerIndex.intValue = element.arraySize - 1;
        }
        else if (correctAnswerIndex.intValue < 0)
        {
            correctAnswerIndex.intValue = 0;
        }
    }


}
