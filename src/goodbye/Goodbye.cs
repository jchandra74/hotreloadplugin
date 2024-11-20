using core;

namespace goodbye;

public class GoodbyeWidget : IWidget
{
    public void Execute()
    {
        Console.WriteLine("Goodbye, World!");
    }
}
