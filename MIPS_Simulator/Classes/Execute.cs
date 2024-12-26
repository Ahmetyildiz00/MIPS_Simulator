using System;
using System.Drawing;

public class Execute
{
    private RegisterManager registerManager;
    private MemoryManager memoryManager;
    public int ProgramCounter { get; set; }

    public Execute(RegisterManager regManager, MemoryManager memManager, MIPS_Simulator.Form1 form1)
    {
        registerManager = regManager;
        memoryManager = memManager;
        ProgramCounter = 0;

    }

    public Execute(RegisterManager registerManager, MemoryManager memoryManager)
    {
        this.registerManager = registerManager;
        this.memoryManager = memoryManager;
    }

    public void ExecuteInstruction(string machineCode)
    {
        string opcode = machineCode.Substring(0, 6); // İlk 6 bit: opcode

        if (opcode == "000000") // R-format
        {
            string funct = machineCode.Substring(26, 6); // Funct alanı (son 6 bit)
            if (funct == "001000") // jr (jump register)
            {
                ExecuteJumpRegister(machineCode); // jr komutu için özel metot
                return; // jr komutu sonrası başka bir komut çalıştırılmayacak
            }
            else
            {
                ExecuteRFormat(machineCode); // Diğer R-format komutları
            }
        }
        else if (opcode == "001000") // addi (I-format)
        {
            ExecuteAddImmediate(machineCode);
        }
        else if (opcode == "100011") // lw (I-format)
        {
            ExecuteLoadWord(machineCode);
        }
        else if (opcode == "101011") // sw (I-format)
        {
            ExecuteStoreWord(machineCode);
        }
        else if (opcode == "000100") // beq (I-format)
        {
            if (ExecuteBranchEqual(machineCode)) return; // Eğer branch gerçekleştiyse, bir sonraki komutlara geçme
        }
        else if (opcode == "000101") // bne (I-format)
        {
            if (ExecuteBranchNotEqual(machineCode)) return; // Eğer branch gerçekleştiyse, bir sonraki komutlara geçme
        }
        else if (opcode == "000010") // j (J-format)
        {
            ExecuteJump(machineCode); // Jump sonrası döngüden çık
            return;
        }
        else if (opcode == "000011") // jal (J-format)
        {
            ExecuteJumpAndLink(machineCode); // Jal sonrası döngüden çık
            return;
        }
        else
        {
            throw new InvalidOperationException($"Desteklenmeyen opcode: {opcode}");
        }

        ProgramCounter += 4; // Bir sonraki komuta geç

    }


    // R-Format Talimatlarını İşle
    private void ExecuteRFormat(string machineCode)
    {
        string funct = machineCode.Substring(26, 6); // Funct son 6 bit
        string rs = machineCode.Substring(6, 5);    // rs 6-10 bit
        string rt = machineCode.Substring(11, 5);   // rt 11-15 bit
        string rd = machineCode.Substring(16, 5);   // rd 16-20 bit

        string rsName = GetRegisterName(rs);
        string rtName = GetRegisterName(rt);
        string rdName = GetRegisterName(rd);

        int rsValue = registerManager.GetRegisterValue(GetRegisterName(rs));
        int rtValue = registerManager.GetRegisterValue(GetRegisterName(rt));

        if (funct == "100000") // add
        {
            registerManager.SetRegisterValue(GetRegisterName(rd), rsValue + rtValue);
        }
        else if (funct == "100010") // sub
        {
            registerManager.SetRegisterValue(GetRegisterName(rd), rsValue - rtValue);
        }
        else if (funct == "100100") // and
        {
            registerManager.SetRegisterValue(GetRegisterName(rd), rsValue & rtValue);
        }
        else if (funct == "100101") // or
        {
            registerManager.SetRegisterValue(GetRegisterName(rd), rsValue | rtValue);
        }
        else if (funct == "101010") // slt 
        {
            registerManager.SetRegisterValue(GetRegisterName(rd), rsValue < rtValue ? 1 : 0);
        }
        else if (funct == "000000") // sll
        {
            int shamt = Convert.ToInt32(machineCode.Substring(21, 5), 2); // Shamt (5 bit)
            registerManager.SetRegisterValue(GetRegisterName(rd), rtValue << shamt);
        }
        else if (funct == "000010") // srl
        {
            int shamt = Convert.ToInt32(machineCode.Substring(21, 5), 2); // Shamt (5 bit)
            registerManager.SetRegisterValue(GetRegisterName(rd), rtValue >> shamt);
        }
        else
        {
            throw new InvalidOperationException($"Desteklenmeyen R-format funct: {funct}");
        }
    }

    // I-Format: addi
    private void ExecuteAddImmediate(string machineCode)
    {
        string rs = machineCode.Substring(6, 5);    // rs
        string rt = machineCode.Substring(11, 5);   // rt
        string immediate = machineCode.Substring(16, 16); // Immediate (16 bit)

        int rsValue = registerManager.GetRegisterValue(GetRegisterName(rs));
        int immValue = (short)Convert.ToInt32(immediate, 2); // İşaretli tam sayı dönüşümü

        registerManager.SetRegisterValue(GetRegisterName(rt), rsValue + immValue);
    }

    // I-Format: lw
    private void ExecuteLoadWord(string machineCode)
    {
        string baseRegister = machineCode.Substring(6, 5); // rs
        string rt = machineCode.Substring(11, 5);          // rt
        string offset = machineCode.Substring(16, 16);     // Offset

        int baseValue = registerManager.GetRegisterValue(GetRegisterName(baseRegister));
        int signedOffset = (short)Convert.ToInt32(offset, 2); // Offset'i işaretli çevir
        int memoryAddress = baseValue + signedOffset;

        Console.WriteLine($"[LW] Base Register: {baseRegister}, Offset: {signedOffset}, Address: {memoryAddress}");

        byte[] data = memoryManager.ReadData(memoryAddress, 4);
        int loadedValue = BitConverter.ToInt32(data, 0);

        Console.WriteLine($"[LW] Loaded Value: {loadedValue}");

        registerManager.SetRegisterValue(GetRegisterName(rt), loadedValue);
    }


    // I-Format: sw
    private void ExecuteStoreWord(string machineCode)
    {
        string baseRegister = machineCode.Substring(6, 5); // rs
        string rt = machineCode.Substring(11, 5);          // rt
        string offset = machineCode.Substring(16, 16);     // Offset

        int baseValue = registerManager.GetRegisterValue(GetRegisterName(baseRegister));
        int signedOffset = (short)Convert.ToInt32(offset, 2); // Offset'i işaretli çevir
        int memoryAddress = baseValue + signedOffset;

        Console.WriteLine($"[SW] Base Register: {baseRegister}, Offset: {signedOffset}, Address: {memoryAddress}");
        Console.WriteLine($"[SW] Value to Store: {registerManager.GetRegisterValue(GetRegisterName(rt))}");

        int value = registerManager.GetRegisterValue(GetRegisterName(rt));
        byte[] data = BitConverter.GetBytes(value);
        memoryManager.WriteData(memoryAddress, data);
    }



    // I-Format: beq
    private bool ExecuteBranchEqual(string machineCode)
    {
        string rs = machineCode.Substring(6, 5);    // rs
        string rt = machineCode.Substring(11, 5);   // rt
        string offset = machineCode.Substring(16, 16); // Offset

        int rsValue = registerManager.GetRegisterValue(GetRegisterName(rs));
        int rtValue = registerManager.GetRegisterValue(GetRegisterName(rt));

        if (rsValue == rtValue) // Eğer eşitlik sağlanırsa
        {
            int signedOffset = (short)Convert.ToInt32(offset, 2); // İşaretli tam sayı dönüşümü
            ProgramCounter += signedOffset * 4; // ProgramCounter'ı offset ile güncelle
            return true; // Branch gerçekleşti
        }

        ProgramCounter += 4; // Branch gerçekleşmezse sıradaki komuta geç
        return false; // Branch gerçekleşmedi
    }

    // I-Format: bne
    private bool ExecuteBranchNotEqual(string machineCode)
    {
        string rs = machineCode.Substring(6, 5);    // rs
        string rt = machineCode.Substring(11, 5);   // rt
        string offset = machineCode.Substring(16, 16); // Offset

        int rsValue = registerManager.GetRegisterValue(GetRegisterName(rs));
        int rtValue = registerManager.GetRegisterValue(GetRegisterName(rt));

        if (rsValue != rtValue) // Eğer eşit değilse
        {
            int signedOffset = (short)Convert.ToInt32(offset, 2); // İşaretli tam sayı dönüşümü
            ProgramCounter += signedOffset * 4; // ProgramCounter'ı offset ile güncelle
            return true; // Branch gerçekleşti
        }

        ProgramCounter += 4; // Branch gerçekleşmezse sıradaki komuta geç
        return false; // Branch gerçekleşmedi
    }

    // J-Format: j
    private void ExecuteJump(string machineCode)
    {
        string address = machineCode.Substring(6, 26); // Jump address
        ProgramCounter = Convert.ToInt32(address, 2) << 2; // Hedef adres
        return;
    }

    // J-Format: jal
    private void ExecuteJumpAndLink(string machineCode)
    {
        string address = machineCode.Substring(6, 26);
        registerManager.SetRegisterValue("$ra", ProgramCounter + 4); // Geri dönüş adresi
        ProgramCounter = Convert.ToInt32(address, 2) << 2;
        return;
    }
    // J-Format: jr
    private void ExecuteJumpRegister(string machineCode)
    {
        if (machineCode.Length < 32)
        {
            throw new InvalidOperationException($"Makine kodu eksik: {machineCode}");
        }

        string rs = machineCode.Substring(6, 5); // rs alanı (6-10 bit)
        int rsValue = registerManager.GetRegisterValue(GetRegisterName(rs)); // Register değerini al
        ProgramCounter = rsValue; // ProgramCounter'ı rs'nin değerine ayarla

        Console.WriteLine($"[JR] Jump to address: {ProgramCounter}");
        return;
    }


    // Register adını binary string'den al
    private string GetRegisterName(string binary)
    {
        int regIndex = Convert.ToInt32(binary, 2);
        foreach (var reg in new OpCode().Registers)
        {
            if (reg.Value == regIndex)
            {
                Console.WriteLine($"[DEBUG] GetRegisterName: Binary {binary} -> Register {reg.Key}");
                return reg.Key;
            }
        }
        throw new Exception($"Register bulunamadı: {binary}");
    }
}
