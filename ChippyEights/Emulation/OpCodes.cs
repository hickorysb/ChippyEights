namespace ChippyEights.Types;

public enum OpCodes
{
    Sys = 0x0, // First nibble for Sys related calls
    Cls = 0x00E0, // Full opcode for clear screen
    Ret = 0x00EE, // Full opcode for return
    Jump = 0x1, // First nibble for Jump to nnn
    Call = 0x2, // First nibble for Call nnn
    Sev = 0x3, // First nibble for skip next if equals value
    Snev = 0x4, // First nibble for skip next if not equals value
    Ser = 0x5, // First nibble for skip next if equals register
    Ld = 0x6,  // First nibble for load value
    Add = 0x7, // First nibble for add value
    Reg = 0x8, // First nibble for a variety of register operations
    Sner = 0x9, // First nibble for skip next if not equals register
    Ldiv = 0xA, // First nibble for load register I value,
    Jump0 = 0xB, // First nibble for jump nnn + v0 or Jump xnn + vX depending on quirks 
    Rnd = 0xC, // First nibble for random number generation 
    Drw = 0xD, // First nibble for draw sprite
    Skpk = 0xE, // First nibble for skipping based on key states
    Other = 0xF // First nibble for other things
}