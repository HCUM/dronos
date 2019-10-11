using UnityEditor;
using UnityEngine;

using System.CodeDom;
using Microsoft.CSharp;
using System.IO;
using System.CodeDom.Compiler;

using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System;


namespace Valve.VR
{
    public class SteamVR_Input_Action_GenericPropertyEditor<T> : PropertyDrawer where T : SteamVR_Action, new()
    {
        protected T[] actions;
        protected string[] enumItems;
        public int selectedIndex = notInitializedIndex;

        protected const int notInitializedIndex = -1;
        protected const int noneIndex = 0;
        protected int addIndex = 1;

        protected void Awake()
        {
            actions = SteamVR_Input.GetActions<T>();
            if (actions != null && actions.Length > 0)
            {
                List<string> enumList = actions.Select(action => action.fullPath).ToList();

                enumList.Insert(noneIndex, "None");

                //replace forward slashes with backslack instead
                for (int index = 0; index < enumList.Count; index++)
                    enumList[index] = enumList[index].Replace('/', '\\');

                enumList.Add("Add...");
                enumItems = enumList.ToArray();
            }
            else
            {
                enumItems = new string[] { "None", "Add..." };
            }

            addIndex = enumItems.Length - 1;

            /*
            //keep sub menus:
            for (int index = 0; index < enumItems.Length; index++)
                if (enumItems[index][0] == '/')
                    enumItems[index] = enumItems[index].Substring(1);
            */
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (SteamVR_Input.actions == null || SteamVR_Input.actions.Length == 0)
            {
                EditorGUI.BeginProperty(position, label, property);
                EditorGUI.LabelField(position, "Please generate SteamVR Input actions");
                EditorGUI.EndProperty();
                return;
            }

            if (enumItems == null || enumItems.Length == 0)
            {
                Awake();
            }

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            property.serializedObject.Update();
            
            SerializedProperty actionPathProperty = property.FindPropertyRelative("actionPath");

            if (actionPathProperty != null)
            {
                string currentPath = actionPathProperty.stringValue;
                if (string.IsNullOrEmpty(currentPath) == false)
                {
                    for (int actionsIndex = 0; actionsIndex < actions.Length; actionsIndex++)
                    {
                        if (actions[actionsIndex].fullPath == currentPath)
                        {
                            selectedIndex = actionsIndex + 1; // account for none option
                            break;
                        }
                    }
                }
            }

            if (selectedIndex == notInitializedIndex)
                selectedIndex = 0;

            
            Rect labelPosition = position;
            labelPosition.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(labelPosition, label);

            Rect fieldPosition = position;
            fieldPosition.x = (labelPosition.x + labelPosition.width);
            fieldPosition.width = EditorGUIUtility.currentViewWidth - (labelPosition.x + labelPosition.width) - 5;

            bool showInputWindow = false;

            int wasSelected = selectedIndex;
            selectedIndex = EditorGUI.Popup(fieldPosition, selectedIndex, enumItems);
            if (selectedIndex != wasSelected)
            {
                if (selectedIndex == noneIndex || selectedIndex == notInitializedIndex)
                {
                    selectedIndex = noneIndex;

                    actionPathProperty.stringValue = "";

                    //Undo.RecordObject(property.serializedObject.targetObject, "nulling action");
                    //fieldInfo.SetValue(property.serializedObject.targetObject, new T());
                    //property.serializedObject.ApplyModifiedProperties();

                    //property.objectReferenceValue = new T();
                }
                else if (selectedIndex == addIndex)
                {
                    selectedIndex = wasSelected; // don't change the index
                    showInputWindow = true;
                }
                else
                {
                    int actionIndex = selectedIndex - 1; // account for none option

                    actionPathProperty.stringValue = actions[actionIndex].GetPath();

                    //Undo.RegisterCompleteObjectUndo(property.serializedObject.targetObject, "Setting action");
                    //fieldInfo.SetValue(property.serializedObject.targetObject, actions[actionIndex]);
                    //property.serializedObject.ApplyModifiedProperties();

                    //property.objectReferenceValue = actions[actionIndex];

                    //this is broken right now. setting the string value works because I'm making copies elsewhere but I'd like to stop doing that. There shouldn't be copies.
                }
            }

            EditorGUI.EndProperty();

            if (showInputWindow)
                SteamVR_Input_EditorWindow.ShowWindow(); //show the input window so they can add a new action
        }
    }
}