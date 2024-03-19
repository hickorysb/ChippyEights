using ChippyEights.Types;
using SFML.Audio;
using static ChippyEights.Utilities.DateTimeUtilities;

namespace ChippyEights.Emulation;

public class Chip8Cpu
{
    internal ushort Index;
    internal readonly byte[] Registers = new byte[16];
    internal ushort ProgramCounter;
    internal ushort CurrentOpCode;
    internal byte[] Graphics = new byte[64 * 32];
    internal readonly KeyState[] Keys = new KeyState[16];
    internal readonly Sound BeeperSound;

    private const uint samples = 44994;
    
    private readonly byte[] memory = new byte[4096];
    private byte delayTimer;
    private byte soundTimer;
    private readonly ushort[] stack = new ushort[16];
    private ushort stackPointer;
    private int cycleCount;
    private float timeTilDecrement = 0.0166666666f;
    private readonly short[] rawAudioSamples = new short[samples];
    private byte? keyPressed;
    private readonly byte[] chip8Font =
    {
        0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
        0x20, 0x60, 0x20, 0x20, 0x70, // 1
        0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
        0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
        0x90, 0x90, 0xF0, 0x10, 0x10, // 4
        0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
        0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
        0xF0, 0x10, 0x20, 0x40, 0x40, // 7
        0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
        0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
        0xF0, 0x90, 0xF0, 0x90, 0x90, // A
        0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
        0xF0, 0x80, 0x80, 0x80, 0xF0, // C
        0xE0, 0x90, 0x90, 0x90, 0xE0, // D
        0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
        0xF0, 0x80, 0xF0, 0x80, 0x80 // F
    };

    private readonly Random randomNumberGenerator;

    public Chip8Cpu()
    {
        randomNumberGenerator = new Random((int)ConvertToUnixTimestamp(DateTime.Now));
        
        ProgramCounter = 0x200;
        
        for (var i = 0; i < chip8Font.Length; i++)
        {
            memory[i] = chip8Font[i];
        }
        
        // Thanks to https://github.com/SFML/SFML/wiki/Tutorial:-Play-Sine-Wave for part of this audio code
        const ushort amplitude = 30000;
        const float twoPi = MathF.PI * 2;
        const float increment = 440f/44100;
        float x = 0f;
        for (ushort i = 0; i < samples; i++) {
            rawAudioSamples[i] = (short)(amplitude * MathF.Sin(x * twoPi));
            x += increment;
        }

        SoundBuffer buffer = new SoundBuffer(rawAudioSamples, 1, 44100);
        BeeperSound = new Sound(buffer);
        BeeperSound.Loop = true;
    }

    private void IncrementProgramCounter()
    {
        ProgramCounter += 2;
    }

    public void Cycle()
    {
        bool blankingInterval = false;
        timeTilDecrement -= EmulationTime.DeltaTime;
        if (timeTilDecrement <= 0f)
        {
            blankingInterval = true;
            timeTilDecrement = 0.0166666666f;
            if (delayTimer >= 1) delayTimer--;
            if (soundTimer >= 1) soundTimer--;
        }
        CurrentOpCode = (ushort)(memory[ProgramCounter] << 8 | memory[ProgramCounter + 1]);

        Console.WriteLine("Cycle: " + ++cycleCount);
        
        byte firstFourBits = (byte)(CurrentOpCode >> 12);

        switch ((OpCodes)firstFourBits)
        {
            case OpCodes.Sys:
            {
                switch ((OpCodes)CurrentOpCode)
                {
                    case OpCodes.Cls:
                        Graphics = new byte[64 * 32];
                        break;
                    case OpCodes.Ret:
                        ProgramCounter = stack[--stackPointer];
                        break;
                }

                IncrementProgramCounter();
                break;
            }
            case OpCodes.Jump:
            {
                ProgramCounter = (ushort)(CurrentOpCode & 0x0FFF);
                break;
            }
            case OpCodes.Call:
            {
                stack[stackPointer] = ProgramCounter;
                stackPointer++;
                ProgramCounter = (ushort)(CurrentOpCode & 0x0FFF);
                break;
            }
            case OpCodes.Sev:
            {
                byte x = (byte)((CurrentOpCode & 0x0F00) >> 8);
                byte value = (byte)(CurrentOpCode & 0x00FF);
                if (Registers[x] == value)
                {
                    IncrementProgramCounter();
                }
                
                IncrementProgramCounter();
                break;
            }
            case OpCodes.Snev:
            {
                byte x = (byte)((CurrentOpCode & 0x0F00) >> 8);
                byte value = (byte)(CurrentOpCode & 0x00FF);
                if (Registers[x] != value)
                {
                    IncrementProgramCounter();
                }
                
                IncrementProgramCounter();
                break;
            }
            case OpCodes.Ser:
            {
                byte x = (byte)((CurrentOpCode & 0x0F00) >> 8);
                byte y = (byte)((CurrentOpCode & 0x00F0) >> 4);
                if (Registers[x] == Registers[y])
                {
                    IncrementProgramCounter();
                }
                
                IncrementProgramCounter();
                break;
            }
            case OpCodes.Ld:
            {
                byte x = (byte)((CurrentOpCode & 0x0F00) >> 8);
                byte value = (byte)(CurrentOpCode & 0x00FF);
                Registers[x] = value;
                IncrementProgramCounter();
                break;
            }
            case OpCodes.Add:
            {
                byte x = (byte)((CurrentOpCode & 0x0F00) >> 8);
                byte value = (byte)(CurrentOpCode & 0x00FF);
                Registers[x] += value;
                IncrementProgramCounter();
                break;
            }
            case OpCodes.Reg:
            {
                byte x = (byte)((CurrentOpCode & 0x0F00) >> 8);
                byte y = (byte)((CurrentOpCode & 0x00F0) >> 4);
                byte lastNibble = (byte)(CurrentOpCode & 0x000F);
                switch ((RegNibbles)lastNibble)
                {
                    case RegNibbles.Mov:
                        Registers[x] = Registers[y];
                        break;
                    case RegNibbles.Or:
                        Registers[x] |= Registers[y];
                        if(Quirks.ResetFXorAndOr) Registers[0xF] = 0;
                        break;
                    case RegNibbles.And:
                        Registers[x] &= Registers[y];
                        if(Quirks.ResetFXorAndOr) Registers[0xF] = 0;
                        break;
                    case RegNibbles.Xor:
                        Registers[x] ^= Registers[y];
                        if(Quirks.ResetFXorAndOr) Registers[0xF] = 0;
                        break;
                    case RegNibbles.Add:
                        int sum = Registers[x] + Registers[y];
                        Registers[x] = (byte)(sum & 0xFF);
                        Registers[0xF] = (byte)(sum > 255 ? 1 : 0);
                        break;
                    case RegNibbles.SubY:
                        byte vx = Registers[x];
                        byte vy = Registers[y];
                        Registers[x] = (byte)(vx - vy);
                        Registers[0xF] = (byte)(vx >= vy ? 1 : 0);
                        break;
                    case RegNibbles.ShiftRight:
                    {
                        byte val;
                        if (Quirks.ShiftWithY)
                        {
                            val = Registers[y];
                            Registers[x] = (byte)(Registers[y] >> 1);
                        }
                        else
                        {
                            val = Registers[x];
                            Registers[x] = (byte)(Registers[x] >> 1);
                        }
                        Registers[0xF] = (byte)(val & 0x1);
                        break;
                    }
                    case RegNibbles.SubX:
                        Registers[x] = (byte)(Registers[y] - Registers[x]);
                        Registers[0xF] = (byte)(Registers[y] >= Registers[x] ? 1 : 0);
                        break;
                    case RegNibbles.ShiftLeft:
                    {
                        byte val;
                        if (Quirks.ShiftWithY)
                        {
                            val = Registers[y];
                            Registers[x] = (byte)(Registers[y] << 1);
                        }
                        else
                        {
                            val = Registers[x];
                            Registers[x] = (byte)(Registers[x] << 1);
                        }
                        Registers[0xF] = (byte)((val & 0x80) >> 7);
                        break;
                    }
                }
                IncrementProgramCounter();
                break;
            }
            case OpCodes.Sner:
            {
                byte x = (byte)((CurrentOpCode & 0x0F00) >> 8);
                byte y = (byte)((CurrentOpCode & 0x00F0) >> 4);
                if (Registers[x] != Registers[y])
                {
                    IncrementProgramCounter();
                }
                
                IncrementProgramCounter();
                break;
            }
            case OpCodes.Ldiv:
            {
                Index = (ushort)(CurrentOpCode & 0x0FFF);
                IncrementProgramCounter();
                break;
            }
            case OpCodes.Jump0:
            {
                if (Quirks.ModifiedJump)
                {
                    byte x = (byte)((CurrentOpCode & 0x0F00) >> 8);
                    ProgramCounter = (ushort)((CurrentOpCode & 0x0FFF) + Registers[x]);
                }
                else
                {
                    ProgramCounter = (ushort)((CurrentOpCode & 0x0FFF) + Registers[0]);
                }

                break;
            }
            case OpCodes.Rnd:
            {
                byte x = (byte)((CurrentOpCode & 0x0F00) >> 8);
                byte kk = (byte)(CurrentOpCode & 0x00FF);
                Registers[x] = (byte)(randomNumberGenerator.Next(0, 256) & kk);
                IncrementProgramCounter();
                break;
            }
            case OpCodes.Drw:
            {
                if (!blankingInterval && Quirks.DisplayWait) return;
                byte x = (byte)((CurrentOpCode & 0x0F00) >> 8);
                byte y = (byte)((CurrentOpCode & 0x00F0) >> 4);
                byte height = (byte)(CurrentOpCode & 0x000F);
                Registers[0xF] = 0;
                for (int horizontalLine = 0; horizontalLine < height; horizontalLine++)
                {
                    byte currentPixel = memory[Index + horizontalLine];
                    for (int verticalLine = 0; verticalLine < 8; verticalLine++)
                    {
                        if ((currentPixel & (0x80 >> verticalLine)) != 0)
                        {
                            int totalX = (Registers[x] + verticalLine) % 64;
                            int totalY = (Registers[y] + horizontalLine) % 32;
                            if(Quirks.Clipping && (Registers[x] % 64 + verticalLine >= 64 || Registers[y] % 32 + horizontalLine >= 32)) continue;
                            int index = totalX + (totalY * 64);
                            if (Graphics[index] == 1)
                            {
                                Registers[0xF] = 1;
                            }
                            Graphics[index] ^= 1;
                        }
                    }
                }
                IncrementProgramCounter();
                break;
            }
            case OpCodes.Skpk:
            {
                byte x = (byte)((CurrentOpCode & 0x0F00) >> 8);
                byte lastByte = (byte)(CurrentOpCode & 0x00FF);
                switch ((SkpkNibbles)lastByte)
                {
                    case SkpkNibbles.Sip:
                    {
                        if (Keys[Registers[x]] == KeyState.Pressed)
                        {
                            IncrementProgramCounter();
                        }

                        break;
                    }
                    case SkpkNibbles.Sinp:
                    {
                        if (Keys[Registers[x]] == KeyState.Released)
                        {
                            IncrementProgramCounter();
                        }

                        break;
                    }
                }
                IncrementProgramCounter();
                break;
            }
            case OpCodes.Other:
            {
                byte x = (byte)((CurrentOpCode & 0x0F00) >> 8);
                byte lastByte = (byte)(CurrentOpCode & 0x00FF);
                switch ((OtherNibbles)lastByte)
                {
                    case OtherNibbles.Gdt:
                        Registers[x] = delayTimer;
                        break;
                    case OtherNibbles.Wfk:
                    {
                        bool keyPress = false;
                        for (int i = 0; i < Keys.Length; i++)
                        {
                            if (Keys[i] == 0) continue;
                            keyPressed = (byte)i;
                            break;
                        }

                        if (keyPressed != null && Keys[keyPressed.Value] == 0)
                        {
                            Registers[x] = keyPressed.Value;
                            keyPress = true;
                        }
                    
                        if (!keyPress)
                        {
                            return;
                        }

                        break;
                    }
                    case OtherNibbles.Sdt:
                        delayTimer = Registers[x];
                        break;
                    case OtherNibbles.Sst:
                        soundTimer = Registers[x];
                        break;
                    case OtherNibbles.Iadd:
                        Index += Registers[x];
                        break;
                    case OtherNibbles.GetFont:
                        Index = (ushort)(Registers[x] * 5);
                        break;
                    case OtherNibbles.Bcd:
                        memory[Index] = (byte)(Registers[x] / 100);
                        memory[Index + 1] = (byte)((Registers[x] / 10) % 10);
                        memory[Index + 2] = (byte)(Registers[x] % 10);
                        break;
                    case OtherNibbles.Store:
                    {
                        for (int i = 0; i <= x; i++)
                        {
                            memory[Index + i] = Registers[i];
                        }

                        if(Quirks.MemoryIncrement) Index++;
                        break;
                    }
                    case OtherNibbles.Load:
                    {
                        for (int i = 0; i <= x; i++)
                        {
                            Registers[i] = memory[Index + i];
                        }
                    
                        if(Quirks.MemoryIncrement) Index++;
                        break;
                    }
                }
                IncrementProgramCounter();
                break;
            }
        }
        
        if(soundTimer > 0 && BeeperSound.Status != SoundStatus.Playing) BeeperSound.Play();
        if(soundTimer == 0 && BeeperSound.Status != SoundStatus.Stopped) BeeperSound.Stop();
    }

    public void LoadMemory(byte[] romData)
    {
        for (int i = 0; i < romData.Length; i++)
        {
            memory[i + 0x200] = romData[i];
        }
    }
}