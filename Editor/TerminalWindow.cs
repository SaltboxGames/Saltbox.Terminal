/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Saltbox.Terminal.Editor
{
    public class TerminalWindow : EditorWindow, ITerminalControl
    {
        private readonly string SETTINGS_PATH = "Assets/TerminalSettings.asset";
        private readonly string COMMAND_RUNNER_PATH = "Assets/Commands.asset";
        private readonly string INPUT_FIELD_NAME = "input";

        public event TerminalEvent OnInputSubmit;

        private Vector2 scrollPosition;

        private string input = "";
        private string output = "";
        private StringBuilder builder;
        
        [SerializeField]
        private TerminalWindowSettings settings;

        public string Name => "Editor";

        // Start is called before the first frame update
        [MenuItem("Window/Terminal")]
        static void Init()
        {
            TerminalWindow window = GetWindow<TerminalWindow>(false, "Terminal");
            window.Show();
        }

        private void OnEnable()
        {
            builder = new StringBuilder();

            settings = LoadOrCreate<TerminalWindowSettings>(SETTINGS_PATH);
            if(settings.runner == null)
            {
                settings.runner = LoadOrCreate<CommandRunner>(COMMAND_RUNNER_PATH);
                SaveSettings();
            }

            settings.runner.AddControl(this);
        }

        private void OnDestroy()
        {
            settings.runner.RemoveControl(this);
            SaveSettings();
        }

        private void SaveSettings()
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        private T LoadOrCreate<T>(string path) where T: ScriptableObject
        {
            T obj = AssetDatabase.LoadAssetAtPath<T>(path);
            if (obj == null)
            {
                obj = CreateInstance<T>();
                AssetDatabase.CreateAsset(obj, path);
            }
            return obj;
        }

        private void OnGUI()
        {
            settings.runner = (CommandRunner) EditorGUILayout.ObjectField("", settings.runner, typeof(CommandRunner), false);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            GUILayout.TextArea(output, EditorStyles.textField, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            GUI.SetNextControlName(INPUT_FIELD_NAME);
            input = EditorGUILayout.TextField(input);
            HandleKeyInput();
        }
        
        private void SendCommand()
        {
            OnInputSubmit?.Invoke(this, new TerminalEventArgs()
            {
                content = input
            });
            input = "";

            EditorGUI.FocusTextInControl(INPUT_FIELD_NAME);
            scrollPosition.y = float.PositiveInfinity;

            Repaint();
        }

        public void Clear()
        {
            builder.Clear();
            UpdateOutput();
        }

        public void Write(string content, Color color)
        {
            builder.Append(content);
            UpdateOutput();
        }

        public void WriteLine(string content, Color color)
        {
            builder.Append(content);
            builder.Append(Environment.NewLine);
            UpdateOutput();
        }

        private void HandleKeyInput()
        {
            Event e = Event.current;
            if (!e.isKey)
            {
                return;
            }

            switch(e.keyCode)
            {
                case KeyCode.Return:
                    SendCommand();
                    break;
                case KeyCode.Tab:
                    break;
            }
        }

        private void UpdateOutput()
        {
            output = builder.ToString();
            Repaint();
        }

        public void OnBeforeSerialize()
        {
            throw new NotImplementedException();
        }

        public void OnAfterDeserialize()
        {
            throw new NotImplementedException();
        }
    }
}





