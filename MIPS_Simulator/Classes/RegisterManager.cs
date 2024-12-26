using System.Collections.Generic;
using System.Linq;

public class RegisterManager
{
    private Dictionary<string, int> registers;
    private Dictionary<string, int> initialRegisters;

    public RegisterManager()
    {
        initialRegisters = new Dictionary<string, int>
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
        registers = new Dictionary<string, int>(initialRegisters);
    }

    public int GetRegisterValue(string register)
    {
        return registers[register];
    }

    public void SetRegisterValue(string register, int value)
    {
        registers[register] = value;
    }

    public Dictionary<string, int> GetAllRegisters()
    {
        return registers;
    }

    public void ResetRegisters()
    {
        // Başlangıç değerlerine geri döndür
        foreach (var key in initialRegisters.Keys)
        {
            registers[key] = initialRegisters[key];
        }
    }

}
