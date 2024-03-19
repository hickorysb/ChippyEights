using ChippyEights.Emulation;
using ChippyEights.Graphics;
using ChippyEights.Types;
using SFML.Window;

namespace ChippyEights.EmulatorLifecycle;

public static class EmulatorInitialization
{
    public static bool InitializeEngine(string[] args)
    {
        if (args.Contains("-dc")) Quirks.Clipping = false;
        if (args.Contains("-ddw")) Quirks.DisplayWait = false;
        if (args.Contains("-ms")) Quirks.ShiftWithY = false;
        if (args.Contains("-mii")) Quirks.MemoryIncrement = false;
        if (args.Contains("-mj")) Quirks.ModifiedJump = true;
        if (args.Contains("-dfr")) Quirks.ResetFXorAndOr = false;

        if (args.Contains("-h"))
        {
            Console.WriteLine("===Chippy Eights Help===");
            Console.WriteLine("ChippyEights is a CHIP-8 emulator written in C# by North Western Bear (https://github.com/hickorysb)");
            Console.WriteLine("ChippyEights.exe [options] -rom [path to ROM]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("-dc\tDisables the clipping quirk, may be required for some roms");
            Console.WriteLine("-ddw\tDisables the display wait quirk, may be required for some roms");
            Console.WriteLine("-ms\tModifies the shift opcodes, may be required for some roms");
            Console.WriteLine("-mii\tDisables the memory index increment for Fx55 and Fx65, may be required for some roms");
            Console.WriteLine("-mj\tModifies the jump opcode to use vX instead of v0, may be required for some roms");
            Console.WriteLine("-dfr\tDisables vF being reset by AND, OR, and XOR, may be required for some roms");
            Environment.Exit(0);
        }
        
        DisplayManager.InitializeWindow();
        DisplayManager.Window.KeyPressed += (sender, keyEventArgs) =>
        {
            switch (keyEventArgs.Code)
            {
                case Keyboard.Key.Num1:
                    ChippyEights.MyCpu.Keys[0x1] = KeyState.Pressed;
                    break;
                case Keyboard.Key.Num2:
                    ChippyEights.MyCpu.Keys[0x2] = KeyState.Pressed;
                    break;
                case Keyboard.Key.Num3:
                    ChippyEights.MyCpu.Keys[0x3] = KeyState.Pressed;
                    break;
                case Keyboard.Key.Num4:
                    ChippyEights.MyCpu.Keys[0xC] = KeyState.Pressed;
                    break;
                case Keyboard.Key.Q:
                    ChippyEights.MyCpu.Keys[0x4] = KeyState.Pressed;
                    break;
                case Keyboard.Key.W:
                    ChippyEights.MyCpu.Keys[0x5] = KeyState.Pressed;
                    break;
                case Keyboard.Key.E:
                    ChippyEights.MyCpu.Keys[0x6] = KeyState.Pressed;
                    break;
                case Keyboard.Key.R:
                    ChippyEights.MyCpu.Keys[0xD] = KeyState.Pressed;
                    break;
                case Keyboard.Key.A:
                    ChippyEights.MyCpu.Keys[0x7] = KeyState.Pressed;
                    break;
                case Keyboard.Key.S:
                    ChippyEights.MyCpu.Keys[0x8] = KeyState.Pressed;
                    break;
                case Keyboard.Key.D:
                    ChippyEights.MyCpu.Keys[0x9] = KeyState.Pressed;
                    break;
                case Keyboard.Key.F:
                    ChippyEights.MyCpu.Keys[0xE] = KeyState.Pressed;
                    break;
                case Keyboard.Key.Z:
                    ChippyEights.MyCpu.Keys[0xA] = KeyState.Pressed;
                    break;
                case Keyboard.Key.X:
                    ChippyEights.MyCpu.Keys[0x0] = KeyState.Pressed;
                    break;
                case Keyboard.Key.C:
                    ChippyEights.MyCpu.Keys[0xB] = KeyState.Pressed;
                    break;
                case Keyboard.Key.V:
                    ChippyEights.MyCpu.Keys[0xF] = KeyState.Pressed;
                    break;
                case Keyboard.Key.T:
                    if (keyEventArgs.Alt)
                    {
                        ChippyEights.MyCpu.Dispose();
                        ChippyEights.MyCpu = new Chip8Cpu();
                        ChippyEights.LoadRomData();
                        GC.Collect();
                    }
                    break;
            }
        };
        DisplayManager.Window.KeyReleased += (sender, keyEventArgs) =>
        {
            switch (keyEventArgs.Code)
            {
                case Keyboard.Key.Num1:
                    ChippyEights.MyCpu.Keys[0x1] = KeyState.Released;
                    break;
                case Keyboard.Key.Num2:
                    ChippyEights.MyCpu.Keys[0x2] = KeyState.Released;
                    break;
                case Keyboard.Key.Num3:
                    ChippyEights.MyCpu.Keys[0x3] = KeyState.Released;
                    break;
                case Keyboard.Key.Num4:
                    ChippyEights.MyCpu.Keys[0xC] = KeyState.Released;
                    break;
                case Keyboard.Key.Q:
                    ChippyEights.MyCpu.Keys[0x4] = KeyState.Released;
                    break;
                case Keyboard.Key.W:
                    ChippyEights.MyCpu.Keys[0x5] = KeyState.Released;
                    break;
                case Keyboard.Key.E:
                    ChippyEights.MyCpu.Keys[0x6] = KeyState.Released;
                    break;
                case Keyboard.Key.R:
                    ChippyEights.MyCpu.Keys[0xD] = KeyState.Released;
                    break;
                case Keyboard.Key.A:
                    ChippyEights.MyCpu.Keys[0x7] = KeyState.Released;
                    break;
                case Keyboard.Key.S:
                    ChippyEights.MyCpu.Keys[0x8] = KeyState.Released;
                    break;
                case Keyboard.Key.D:
                    ChippyEights.MyCpu.Keys[0x9] = KeyState.Released;
                    break;
                case Keyboard.Key.F:
                    ChippyEights.MyCpu.Keys[0xE] = KeyState.Released;
                    break;
                case Keyboard.Key.Z:
                    ChippyEights.MyCpu.Keys[0xA] = KeyState.Released;
                    break;
                case Keyboard.Key.X:
                    ChippyEights.MyCpu.Keys[0x0] = KeyState.Released;
                    break;
                case Keyboard.Key.C:
                    ChippyEights.MyCpu.Keys[0xB] = KeyState.Released;
                    break;
                case Keyboard.Key.V:
                    ChippyEights.MyCpu.Keys[0xF] = KeyState.Released;
                    break;
            }
        };
        return true;
    }
}