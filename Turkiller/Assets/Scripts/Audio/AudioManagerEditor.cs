using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private SerializedProperty painSoundsProperty;
    private SerializedProperty deathSoundsProperty;
    private List<SerializedProperty> otherProperties = new List<SerializedProperty>();

    private void OnEnable()
    {
        SerializedObject obj = serializedObject;

        // Récupérer les propriétés spécifiques
        painSoundsProperty = obj.FindProperty("_painSounds");
        deathSoundsProperty = obj.FindProperty("_deathSounds");

        otherProperties.Clear();

        // Récupération automatique des autres propriétés sérialisées
        SerializedProperty iterator = obj.GetIterator();
        if (iterator.NextVisible(true)) // Vérification que l'itération est valide
        {
            do
            {
                if (iterator.name != "_painSounds" && iterator.name != "_deathSounds")
                {
                    otherProperties.Add(obj.FindProperty(iterator.name));
                }
            }
            while (iterator.NextVisible(false)); // `false` pour ne pas parcourir les enfants des objets
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck(); // Début de la détection de changement

        // Affichage des propriétés sérialisables normales
        foreach (var property in otherProperties)
        {
            EditorGUILayout.PropertyField(property, true);
        }

        // Affichage personnalisé des tableaux 2D
        AudioManager manager = (AudioManager)target;
        DrawSound2DArray(ref manager._painSounds, "Pain Sounds");
        DrawSound2DArray(ref manager._deathSounds, "Death Sounds");

        if (EditorGUI.EndChangeCheck()) // Vérifie si quelque chose a changé
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target); // Marquer comme modifié uniquement si nécessaire
        }
    }

    private void DrawSound2DArray(ref SerializableSoundArray[] soundArray, string label)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        // Initialize soundArray if null
        if (soundArray == null)
        {
            soundArray = new SerializableSoundArray[0];
        }

        // Display the number of rows and resize if necessary
        int rowCount = EditorGUILayout.IntField("Number of rows", soundArray.Length);

        if (rowCount != soundArray.Length)
        {
            System.Array.Resize(ref soundArray, rowCount);
        }

        // Display rows
        for (int i = 0; i < soundArray.Length; i++)
        {
            // Initialize each row if necessary
            if (soundArray[i] == null)
            {
                soundArray[i] = new SerializableSoundArray();
            }

            // Initialize sounds array if necessary
            if (soundArray[i].sounds == null)
            {
                soundArray[i].sounds = new Sound[0];
            }

            EditorGUILayout.LabelField($"Row {i + 1}", EditorStyles.boldLabel);
            int colCount = EditorGUILayout.IntField($"Columns of Row {i + 1}", soundArray[i].sounds.Length);

            if (colCount != soundArray[i].sounds.Length)
            {
                System.Array.Resize(ref soundArray[i].sounds, colCount);
            }

            for (int j = 0; j < soundArray[i].sounds.Length; j++)
            {
                EditorGUILayout.BeginHorizontal();
                if (soundArray[i].sounds[j] == null)
                {
                    soundArray[i].sounds[j] = new Sound();
                }
                soundArray[i].sounds[j]._name = EditorGUILayout.TextField($"Name {i},{j}", soundArray[i].sounds[j]._name);
                soundArray[i].sounds[j]._clip = (AudioClip)EditorGUILayout.ObjectField(soundArray[i].sounds[j]._clip, typeof(AudioClip), false);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
        }
    }
}

#endif