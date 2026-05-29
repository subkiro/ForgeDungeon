// Editor/RaritySettingsSOEditor.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(RaritySettingsSO))]
public class RaritySettingsSOEditor : Editor
{
    private SerializedProperty listProperty;
    private List<Rarity> missingRarities;
    private HashSet<int> duplicateIndices;

    private void OnEnable()
    {
        listProperty = serializedObject.FindProperty("RarityParameters");
        RunChecks();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        RunChecks();

        // Missing rarities info & button
        if (missingRarities.Count > 0)
        {
            string missingNames = string.Join(", ", missingRarities.Select(r => r.ToString()));
            EditorGUILayout.HelpBox($"Missing entries for: {missingNames}", MessageType.Info);
            if (GUILayout.Button("Add Missing Entries"))
            {
                AddMissingEntries();
                RunChecks();
            }
        }

        // Duplicate rarities info & button
        if (duplicateIndices.Count > 0)
        {
            var dupRarities = new HashSet<Rarity>();
            for (int i = 0; i < listProperty.arraySize; i++)
            {
                if (duplicateIndices.Contains(i))
                {
                    var rarity = (Rarity)listProperty.GetArrayElementAtIndex(i)
                        .FindPropertyRelative("Rarity").enumValueIndex;
                    dupRarities.Add(rarity);
                }
            }
            string dupNames = string.Join(", ", dupRarities.Select(r => r.ToString()));
            EditorGUILayout.HelpBox($"Duplicate entries for: {dupNames}", MessageType.Warning);
            if (GUILayout.Button("Remove Duplicates"))
            {
                RemoveDuplicateEntries();
                RunChecks();
            }
        }

        // Draw the list with duplicate highlighting and rarity names as labels
        DrawListWithHighlighting();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawListWithHighlighting()
    {
        listProperty.isExpanded = EditorGUILayout.Foldout(listProperty.isExpanded, "Rarity Parameters", true);
        if (!listProperty.isExpanded)
            return;

        EditorGUI.indentLevel++;
        int arraySize = listProperty.arraySize;
        for (int i = 0; i < arraySize; i++)
        {
            SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
            bool isDuplicate = duplicateIndices.Contains(i);

            // Get the rarity enum value and convert to a readable string
            SerializedProperty rarityProp = element.FindPropertyRelative("Rarity");
            string rarityLabel = ((Rarity)rarityProp.enumValueIndex).ToString();

            Color originalBg = GUI.backgroundColor;
            if (isDuplicate)
                GUI.backgroundColor = Color.red;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            // Use the rarity name as the label for the whole element
            EditorGUILayout.PropertyField(element, new GUIContent(rarityLabel), true);
            EditorGUILayout.EndVertical();

            GUI.backgroundColor = originalBg;
        }
        EditorGUI.indentLevel--;
    }

    private void RunChecks()
    {
        var allRarities = System.Enum.GetValues(typeof(Rarity)) as Rarity[];
        var existingRarities = new HashSet<Rarity>();
        for (int i = 0; i < listProperty.arraySize; i++)
        {
            SerializedProperty rarityProp = listProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Rarity");
            existingRarities.Add((Rarity)rarityProp.enumValueIndex);
        }
        missingRarities = allRarities.Where(r => !existingRarities.Contains(r)).ToList();

        var firstOccurrence = new Dictionary<Rarity, int>();
        duplicateIndices = new HashSet<int>();
        for (int i = 0; i < listProperty.arraySize; i++)
        {
            Rarity rarity = (Rarity)listProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Rarity").enumValueIndex;
            if (firstOccurrence.ContainsKey(rarity))
                duplicateIndices.Add(i);
            else
                firstOccurrence[rarity] = i;
        }
    }

    private void AddMissingEntries()
    {
        var allRarities = System.Enum.GetValues(typeof(Rarity)) as Rarity[];
        var existingRarities = new HashSet<Rarity>();
        for (int i = 0; i < listProperty.arraySize; i++)
            existingRarities.Add((Rarity)listProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Rarity").enumValueIndex);

        foreach (var rarity in allRarities)
        {
            if (!existingRarities.Contains(rarity))
            {
                int newIndex = listProperty.arraySize;
                listProperty.InsertArrayElementAtIndex(newIndex);
                SerializedProperty newElement = listProperty.GetArrayElementAtIndex(newIndex);
                newElement.FindPropertyRelative("Rarity").enumValueIndex = (int)rarity;
                newElement.FindPropertyRelative("color").colorValue = Color.white;
                newElement.FindPropertyRelative("QualityThreshold").doubleValue = 0.0;
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void RemoveDuplicateEntries()
    {
        var firstSeen = new Dictionary<Rarity, int>();
        var indicesToRemove = new List<int>();
        for (int i = 0; i < listProperty.arraySize; i++)
        {
            Rarity rarity = (Rarity)listProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Rarity").enumValueIndex;
            if (firstSeen.ContainsKey(rarity))
                indicesToRemove.Add(i);
            else
                firstSeen[rarity] = i;
        }

        for (int i = indicesToRemove.Count - 1; i >= 0; i--)
            listProperty.DeleteArrayElementAtIndex(indicesToRemove[i]);

        serializedObject.ApplyModifiedProperties();
    }
}