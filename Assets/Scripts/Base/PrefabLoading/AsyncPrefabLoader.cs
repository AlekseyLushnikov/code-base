using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AsyncPrefabLoader<T> where T : Component
{
    public event Action<float> OnProgress;
    private Queue<LoadPrefabCommand<T>> _commands; 
    private int _loadPerFrame;
    private int _commandsCount;
    
    public IEnumerator StartLoad(Queue<LoadPrefabCommand<T>> commands, int loadPerFrame)
    {
        _commands = commands;
        _commandsCount = _commands.Count;
        _loadPerFrame = loadPerFrame;
        yield return LoadAsync();
    }

    private IEnumerator LoadAsync()
    {
        var loaded = 0;
        while (loaded < _commandsCount)
        {
            yield return null;
            for (var i = 0; i < _loadPerFrame; i++)
            {
                if (_commands.Count <= 0) continue;
                
                var command = _commands.Dequeue();
                var obj = Object.Instantiate(command.Prefab, command.Parent);
                obj.transform.position = command.Position;
                obj.transform.rotation = command.Rotation;
                command.OnObjectLoaded?.Invoke(obj);
            }
            loaded += _loadPerFrame;
            OnProgress?.Invoke((float) loaded / _commandsCount);
        }
    }
}
