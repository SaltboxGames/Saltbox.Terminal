# Saltbox.Terminal
[![openupm](https://img.shields.io/npm/v/com.saltboxgames.saltbox.terminal?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.saltboxgames.saltbox.terminal/)

A CommandLine Terminal for Unity Games

![terminal](https://user-images.githubusercontent.com/1238853/82847125-ba05ae80-9ec2-11ea-8789-a28f338ffa17.png)


## Installing from **OpenUPM**
You can easily install this package using OpenUPM CLI
```
openupm add com.saltboxgames.saltbox.terminal
```
**Note**: 
this method requires `Node 12` and you can install OpenUPM using 
```
npm install -g openupm-cli
```

## Usage
Assets and scripts are available under the `saltbox.terminal` namespace.
*This documentation is incomplete*

The default Terminal is available under Window dropdown. The first time you open the terminal it will create two scriptable objects `Commands` and `TerminalSettings`. 

#### TerminalSettings 
`TerminalSettings` is used to store information about the terminal editor window. it can't be moved, it's only used by the editor so it won't be compiled into your game.

#### CommandRunner
`Commands` is and instance of `CommandRunner` used to handle any terminal controls and commands you wish to write. It will be regenerated if the runner field in `TerminalSettings` is null.

### Creating New Commands
Adding a new command can be done by implementing the `ICommand` interface. and adding it to the command runner. `Option` attributes will automatically be populated during execution. 

```cs
    //Ex; clear --all
    class ClearCommand : ICommand
    {
        [Option('a', "all", HelpText="Clear All Terminals")]
        public bool clearAll { get; set; }

        public bool Invoke(ITerminalControl sender, CommandRunner runner)
        {
            if (clearAll)
            {
                runner.Clear();
                return true;
            }
            sender.Clear();
            return true;
        }
    }
```

```cs
    runner.AddCommand('clear', new ClearCommand());
```

Alternatively any commands that require Unity Assets can be created by inheriting from `ScriptableCommand`.

```cs
    //Ex; spawn-block -c 10
    [CreateAssetMenu(fileName = "Spawner", menuName = "Terminal/Commands/Spawner")]
    public class SpawnPrefab : ScriptableCommand
    {
        [SerializeField]
        private GameObject prefab;

        [Option('c', "count", Required = true, HelpText = "number of prefabs to spawn")]
        public int count { get; set; }

        public override string Name => "spawn-" + prefab.Name;

        public override bool Invoke(ITerminalControl sender, CommandRunner runner)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject.Instantiate(prefab);
            }
            return true;
        }
    }
```

you can then add an instance of your command to either the Runtime or Editor Commands list in your command runner.

![command-runner](https://user-images.githubusercontent.com/1238853/82846319-950f3c80-9ebe-11ea-8cc6-96053b78ccc7.png)