using System;

public class MemoryManager
{
    private byte[] instructionMemory;
    private byte[] dataMemory;

    public MemoryManager()
    {
        instructionMemory = new byte[512];
        dataMemory = new byte[512];
    }

    public void WriteInstruction(int address, byte[] data)
    {
        if (address < 0 || address + data.Length > instructionMemory.Length)
            throw new IndexOutOfRangeException("Instruction Memory: Geçersiz adres.");

        Buffer.BlockCopy(data, 0, instructionMemory, address, data.Length);
    }

    public byte[] ReadInstruction(int address, int length)
    {
        if (address < 0 || address + length > instructionMemory.Length)
            throw new IndexOutOfRangeException("Instruction Memory: Geçersiz adres.");

        byte[] data = new byte[length];
        Buffer.BlockCopy(instructionMemory, address, data, 0, length);
        return data;
    }

    public void WriteData(int address, byte[] data)
    {
        if (address < 0 || address + data.Length > dataMemory.Length)
            throw new InvalidOperationException("Data Memory: Adres veya uzunluk dizi sınırlarını aşıyor.");

        if (address % 4 != 0)
            throw new InvalidOperationException("Data Memory: Adres 4 byte hizalamasında olmalıdır.");

        Buffer.BlockCopy(data, 0, dataMemory, address, data.Length);
    }


    public byte[] ReadData(int address, int length)
    {
        if (address < 0 || address + length > dataMemory.Length)
            throw new InvalidOperationException("Data Memory: Adres veya uzunluk dizi sınırlarını aşıyor.");

        if (address % 4 != 0)
            throw new InvalidOperationException("Data Memory: Adres 4 byte hizalamasında olmalıdır.");

        byte[] data = new byte[length];
        Buffer.BlockCopy(dataMemory, address, data, 0, length);
        return data;
    }


    public void ResetMemory()
    {
        Array.Clear(instructionMemory, 0, instructionMemory.Length);
        Array.Clear(dataMemory, 0, dataMemory.Length);
    }
}
