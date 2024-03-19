using ChippyEights.Emulation;
using ChippyEights.Graphics;

namespace ChippyEights.EmulatorLifecycle;

public class EmulatorShutdown
{
    public static void Shutdown()
    {
        ChippyEights.MyCpu.BeeperSound.Stop();
        ChippyEights.MyCpu.BeeperSound.SoundBuffer.Dispose();
        ChippyEights.MyCpu.BeeperSound.Dispose();
        DisplayManager.Window.Close();
        while (EmulationTime.CurrentFrameDeltaTime < 0.5f) {} // Wait for audio engine to finish
    }
}