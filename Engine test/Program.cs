using Engine_test;
using OpenGLEngine;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

MonitorInfo monitor = Monitors.GetPrimaryMonitor();
//Console.WriteLine($"Width: {monitor.HorizontalResolution} Height: {monitor.VerticalResolution}");
//Console.WriteLine("BRUH");
float[] planeVerts =
{
    -10f, -2f, -10f,    0f, 0f,
    10f, -2f, -10f,     1f, 0f,
    10f, -2f, 10f,      1f, 1f,
    -10f, -2f, 10f,     0f, 1f
};
uint[] planeIndices =
{
    0, 1, 2,
    0, 3, 2
};
using (Game window = new Game(monitor.HorizontalResolution, monitor.VerticalResolution, "test"))
{
    Shader shader = window.getDefaultShader();

    Entity plane = Entity.createEntity(shader, planeVerts, planeIndices, new (string, int)[] { ("aPosition", 3), ("aTexPos", 2) }, "Plane");
    plane.loadTextures("C:\\Users\\brian\\source\\repos\\OpenGLEngine\\Engine test\\textures\\", new string[] { "ground.jpg" });

    Cube cube = Shapes.AddCubeToGame(window, new Vector3(1f, 1f, 1f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
    window.addDrawableObject(plane);

    GameLoop game = new GameLoop(cube);
    Thread t = new Thread(game.run);
    t.Start();

    window.Run();

    game.stop();
}

