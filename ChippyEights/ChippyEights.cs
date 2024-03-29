﻿using ChippyEights.Emulation;
using ChippyEights.EmulatorLifecycle;
using ChippyEights.Graphics;
using SFML.Graphics;
using SFML.System;

namespace ChippyEights;

public static class ChippyEights
{
    internal static Chip8Cpu MyCpu = null!; // Done to suppress IDE warnings

    private static string? romPath;
    
    public static void Main(string[] args)
    {
        if(!EmulatorInitialization.InitializeEngine(args)) Environment.Exit(3);
        RectangleShape drawField = new RectangleShape(new Vector2f(64, 32));
        
        MyCpu = new Chip8Cpu();
        
        LoadRomData(args);

        DisplayManager.Window.Closed += (_, _) =>
        {
            EmulatorShutdown.Shutdown();
        };
        Texture texture = new Texture(64, 32);
        Image display = new Image(64, 32);
        while (DisplayManager.Window.IsOpen)
        {
            EmulationTime.Restart();
            DisplayManager.Window.DispatchEvents();
            DisplayManager.Window.Clear(Color.Black);
            if (!DisplayManager.Window.IsOpen) continue;
            MyCpu.Cycle();
            
            for (int i = 0; i < 64 * 32; i++)
            {
                byte pixel = MyCpu.Graphics[i];
                bool visible = pixel == 1;
                display.SetPixel((uint)(i % 64), (uint)(i / 64), visible ? Color.Green : Color.Transparent);
            }
            texture.Update(display);
            drawField.Texture = texture;
            drawField.Size = new Vector2f(DisplayManager.Window.Size.X, DisplayManager.Window.Size.Y);
            DisplayManager.Window.Draw(drawField);
            DisplayManager.Window.Display();
        }
    }

    internal static void LoadRomData(string[]? args = null)
    {
        try
        {
            if (args != null)
            {

                int romParam = Array.IndexOf(args, "-rom");

                if (romParam == -1 || args.Length == romParam - 1)
                {
                    Console.WriteLine("No rom path specified, please try again");
                    EmulatorShutdown.Shutdown();
                    Environment.Exit(1);
                }

                romPath = args[romParam + 1];
            }

            if (string.IsNullOrEmpty(romPath))
            {
                Console.WriteLine("No rom path specified, please try again");
                EmulatorShutdown.Shutdown();
                Environment.Exit(1);
            }
            
            MyCpu.LoadMemory(File.ReadAllBytes(romPath));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            Environment.Exit(5);
        }
    }
}