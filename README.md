# ChippyEights

ChippyEights is a very basic CHIP-8 emulator written in C# as a learning project for writing emulators. The name is a play on Crazy Eights

Some of this code was based on [this](https://youtu.be/YHkBgR6yvbY) tutorial by Iridescence.

I'm quite happy with how it turned out to be honest. It uses SFML as the rendering and audio library and it is based on .NET 8 so it should be pretty cross platform out of the box.

You'll have to compile it yourself if you want to test, but you use it by doing:
```bash
.\ChippyEights.exe -rom [path to ROM]
```
You can also execute the following to see some additional command line options:
```bash
.\ChippyEights.exe -h
```

Thank you, I hope that if you find this you might be able to learn a thing or two yourself about emulation!