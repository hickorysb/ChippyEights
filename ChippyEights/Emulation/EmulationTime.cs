using SFML.System;

namespace ChippyEights.Emulation;

public static class EmulationTime
{
    public static readonly Clock SecondClock = new Clock();
    public static float DeltaTime;
    
    private static readonly Clock mainGameClock = new Clock();
    
    public static float CurrentFrameDeltaTime => mainGameClock.ElapsedTime.AsSeconds();

    internal static void Restart()
    { 
        DeltaTime = mainGameClock.Restart().AsSeconds();
        if(SecondClock.ElapsedTime.AsMilliseconds() >= 1000)
        {
            SecondClock.Restart();
        }
    }
}