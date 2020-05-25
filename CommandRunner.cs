/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using CommandLine;
using CommandLine.Text;

using Saltbox.Terminal.Commands;

namespace Saltbox.Terminal
{
    [CreateAssetMenu(fileName = "Commands", menuName = "Terminal/CommandRunner")]
    public class CommandRunner : ScriptableObject
    {
        private List<ITerminalControl> controls = new List<ITerminalControl>();
        private Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

        [SerializeField]
        private string heading = "-Saltbox Terminal-";
        [SerializeField]
        private string copywrite = "Copyright (c) 2020 Saltbox Games";

        [SerializeField]
        private List<ScriptableCommand> runtimeCommands;

#if UNITY_EDITOR
        [SerializeField]
        private List<ScriptableCommand> editorCommands;
#endif

        [SerializeField]
        private Color defaultColor = Color.white;
        public Color DefaultColor => defaultColor;

        [SerializeField]
        private Color warningColor = Color.yellow;
        public Color WarningColor => warningColor;

        [SerializeField]
        private Color errorColor = Color.red;
        public Color ErrorColor => errorColor;

        private void OnEnable()
        {
            commands.Add("help", new HelpCommand());
            commands.Add("clear", new ClearCommand());

            if(Application.isPlaying && runtimeCommands != null)
            {
                foreach (ScriptableCommand command in runtimeCommands)
                {
                    AddCommand(command.Name, command);
                }
            }

#if UNITY_EDITOR
            if(editorCommands != null)
            {
                foreach (ScriptableCommand command in editorCommands)
                {
                    AddCommand(command.Name, command);
                }
            }
#endif
        }

        public void AddControl(ITerminalControl control)
        {
            controls.Add(control);
            control.OnInputSubmit += Control_OnInputSubmit;
        }

        public void RemoveControl(ITerminalControl control)
        {
            control.OnInputSubmit -= Control_OnInputSubmit;
            controls.Remove(control);
        }

        public void AddCommand(string name, ICommand command)
        {
            if(commands.ContainsKey(name))
            {
                Debug.LogError($"Command {name} already exists!");
                return;
            }

            commands.Add(name, command);
        }

        public void RemoveCommand(string name)
        {
            commands.Remove(name);
        }

        private void Control_OnInputSubmit(ITerminalControl sender, TerminalEventArgs args)
        {
            string content = args.content;

            WriteLine($"{sender.Name}/> {content}", DefaultColor);

            string[] split = args.content.Split(' ');
            if(split.Length == 0)
            {
                return;
            }

            string command = split[0];
            string[] commandArgs = split.Skip(1).ToArray();

            if (commands.TryGetValue(command, out ICommand com))
            {
                if (Parser.Default.ParseArguments(commandArgs, com))
                {
                    if(!com.Invoke(sender, this))
                    {
                        sender.WriteLine(GetHelpText(com), ErrorColor);
                    }
                }
                else
                {
                    sender.WriteLine(GetHelpText(com), ErrorColor);
                }
            }
            else
            {
                sender.WriteLine($"{command} is not a know command", ErrorColor);
                sender.WriteLine("\t type help for a list of know commands", DefaultColor);
            }
        }

        private HelpText GetHelpText(ICommand command)
        {
            HelpText help = HelpText.AutoBuild(command);
            help.Copyright = copywrite;
            help.Heading = heading;
            help.AddDashesToOption = true;

            return help;
        }

        public void WriteLine(string content, Color color)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].WriteLine(content, color);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].Clear();
            }
        }

        private class HelpCommand : ICommand
        {
            [ValueOption(0)]
            [Option('c', "command", DefaultValue = null, HelpText = "command to get help text for")]
            public string commandName { get; set; }

            public bool Invoke(ITerminalControl sender, CommandRunner runner)
            {
                if (commandName == null)
                {
                    string result = "Known Commands:";
                    foreach (string name in runner.commands.Keys)
                    {
                        result += "\n\t" + name;
                    }

                    sender.WriteLine(result, runner.DefaultColor);
                    return true;
                }

                ICommand other;
                if (!runner.commands.TryGetValue(commandName, out other))
                {
                    sender.WriteLine($"{commandName} is not a known command!", runner.DefaultColor);
                    return true;
                }

                sender.WriteLine(runner.GetHelpText(other), runner.DefaultColor);
                return true;
            }
        }
    }
}


