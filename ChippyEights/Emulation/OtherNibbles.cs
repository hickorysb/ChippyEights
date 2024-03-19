namespace ChippyEights.Emulation;

public enum OtherNibbles
{
    Gdt = 0x07, // Nibble for getting delay timer
    Wfk = 0x0A, // Nibble for waiting for key
    Sdt = 0x15, // Nibble for setting delay timer
    Sst = 0x18, // Nibble for setting sound timer
    Iadd = 0x1E, // Nibble for adding to index reg
    GetFont = 0x29, // Nibble for getting font start
    Bcd = 0x33, // Nibble for storing bcd in memory
    Store = 0x55, // Nibble for storing registers V0 through Vx in memory starting at I
    Load = 0x65 // Nibble for loading V0 through Vx from memory starting at I
}