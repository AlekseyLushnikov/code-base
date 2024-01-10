public class ExampleCommand : ICommand
{
    private string _value;
    private char _lastC;

    public ExampleCommand(char c)
    {
        _lastC = c;
    }

    public void Execute()
    {
        _value += _lastC;
    }

    public void Undo()
    {
        if (_value.Length > 0)
        {
            _value.Remove(_value.Length - 1);
        }
    }
}