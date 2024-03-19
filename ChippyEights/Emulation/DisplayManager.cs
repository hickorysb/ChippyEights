using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ChippyEights.Graphics;

public static class DisplayManager
{
    public static RenderWindow Window;

    public static void InitializeWindow()
    {
        Window = new RenderWindow(VideoMode.DesktopMode, "ChippyEights", Styles.Close | Styles.Titlebar | Styles.Resize, new ContextSettings() { AntialiasingLevel = 0 });
        Window.SetFramerateLimit(2000);
        Window.GetView().Size = new Vector2f(Window.Size.X, Window.Size.Y);
        Window.Resized += (sender, args) =>
        {
            Window.GetView().Size = new Vector2f(args.Width, args.Height);
        };
    }
}