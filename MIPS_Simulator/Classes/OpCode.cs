using System.Collections.Generic;

public class OpCode
{
    public Dictionary<string, int> Registers { get; private set; }
    public Dictionary<string, int> RFormat { get; private set; }
    public Dictionary<string, int> IFormat { get; private set; }
    public Dictionary<string, int> JFormat { get; private set; }

    public OpCode()
    {
        Registers = new Dictionary<string, int>
        {
            { "$zero", 0 }, { "$at", 1 }, { "$v0", 2 }, { "$v1", 3 },
            { "$a0", 4 }, { "$a1", 5 }, { "$a2", 6 }, { "$a3", 7 },
            { "$t0", 8 }, { "$t1", 9 }, { "$t2", 10 }, { "$t3", 11 },
            { "$t4", 12 }, { "$t5", 13 }, { "$t6", 14 }, { "$t7", 15 },
            { "$s0", 16 }, { "$s1", 17 }, { "$s2", 18 }, { "$s3", 19 },
            { "$s4", 20 }, { "$s5", 21 }, { "$s6", 22 }, { "$s7", 23 },
            { "$s8", 24 }, { "$s9", 25 }, { "$k0", 26 }, { "$k1", 27 },
            { "$gp", 28 }, { "$sp", 29 }, { "$fp", 30 }, { "$ra", 31 }
        };

        RFormat = new Dictionary<string, int>
        {
            { "add", 0x20 }, { "sub", 0x22 }, { "and", 0x24 },
            { "or", 0x25 }, { "slt", 0x2A }, { "sll", 0x00 }, { "srl", 0x02 },{ "jr", 0x08 }
        };

        IFormat = new Dictionary<string, int>
        {
            { "addi", 0x8 }, { "lw", 0x23 }, { "sw", 0x2B },
            { "beq", 0x4 }, { "bne", 0x5 }
        };

        JFormat = new Dictionary<string, int>
        {
            { "j", 0x2 }, { "jal", 0x3 }
        };
    }
}
