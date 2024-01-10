using System.Collections.Generic;
using UnityEngine;

public class CommandManager : Singleton<CommandManager>
{
    private readonly Stack<ICommand> _commandsBuffer = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Undo();
        }
    }

    public void AddCommand(ICommand command)
    {
        command.Execute();
        _commandsBuffer.Push(command);
    }

    public void Undo()
    {
        if (_commandsBuffer.Count == 0)
            return;

        var cmd = _commandsBuffer.Pop();
        cmd.Undo();
    }
}