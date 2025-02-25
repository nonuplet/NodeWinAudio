using Microsoft.JavaScript.NodeApi;

namespace NodeWinAudio;

[JSExport]
public static class HelloWorld
{
    public static string SayHello(string name)
    {
        return $"Hello from {name}!";
    }
}
