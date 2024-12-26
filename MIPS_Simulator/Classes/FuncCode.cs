using System;
using System.Collections.Generic;

public class FuncCode
{
    private OpCode opCode;

    public FuncCode()
    {
        opCode = new OpCode();
    }
    public Dictionary<string, int> GetLabels(string[] assemblyLines)
    {
        Dictionary<string, int> labels = new Dictionary<string, int>();
        int currentAddress = 0;

        foreach (string line in assemblyLines)
        {
            string trimmedLine = line.Trim();

            if (trimmedLine.EndsWith(":")) // Eğer satır bir etiket içeriyorsa
            {
                string label = trimmedLine.TrimEnd(':');
                labels[label] = currentAddress; // Etiketin adresini kaydet
            }
            else
            {
                currentAddress += 4; // Her talimat 4 byte yer kaplar
            }
        }

        return labels;
    }

    public string ParseAssemblyToMachineCode(string assemblyCode, Dictionary<string, int> labels, int currentAddress)
    {
        string[] parts = assemblyCode.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
        string instruction = parts[0];

        if (opCode.RFormat.ContainsKey(instruction))
        {
            return ParseRFormat(parts);
        }
        else if (opCode.IFormat.ContainsKey(instruction))
        {
            return ParseIFormat(parts, labels, currentAddress);
        }
        else if (opCode.JFormat.ContainsKey(instruction))
        {
            return ParseJFormat(parts, labels); // labels parametresini gönder
        }

        throw new InvalidOperationException("Geçersiz talimat formatı.");
    }


    private string ParseRFormat(string[] parts)
    {
        string opcode = "000000"; // R-format opcode her zaman sıfırdır.
        string rs, rt, rd, shamt, funct;
        
        if (parts[0] == "jr") // jr komutu için özel işlem
        {
            rs = ConvertToBinary(opCode.Registers[parts[1]], 5); // rs register
            rt = "00000"; // jr komutunda rt kullanılmaz
            rd = "00000"; // jr komutunda rd kullanılmaz
            shamt = "00000"; // jr komutunda shamt sıfırdır
            funct = "001000"; // jr komutunun funct değeri
        }

        else if (parts[0] == "sll" || parts[0] == "srl") // sll ve srl komutları için özel işlem
        {
            rs = "00000"; // sll ve srl komutlarında rs her zaman sıfırdır
            rt = ConvertToBinary(opCode.Registers[parts[2]], 5); // rt register (kaynak)
            rd = ConvertToBinary(opCode.Registers[parts[1]], 5); // rd register (hedef)
            shamt = ConvertToBinary(int.Parse(parts[3]), 5); // shift amount (shamt)
        }
        else // Diğer R-format komutları için
        {
            rs = ConvertToBinary(opCode.Registers[parts[2]], 5); // rs register
            rt = ConvertToBinary(opCode.Registers[parts[3]], 5); // rt register
            rd = ConvertToBinary(opCode.Registers[parts[1]], 5); // rd register
            shamt = "00000"; // Diğer komutlarda shamt sıfırdır
        }

        funct = ConvertToBinary(opCode.RFormat[parts[0]], 6); // Komutun funct değeri

        return opcode + rs + rt + rd + shamt + funct;
    }



    private string ParseIFormat(string[] parts, Dictionary<string, int> labels, int currentAddress)
    {
        string opcode = ConvertToBinary(opCode.IFormat[parts[0]], 6); // Opcode
        string rs = ""; // Source register
        string rt = ConvertToBinary(opCode.Registers[parts[1]], 5);   // Target register
        int offset = 0;


        if (parts[0] == "lw" || parts[0] == "sw") // lw ve sw özel durumu
        {
            // Offset ve Base Register ayrıştırma (örneğin: 4($t0))
            if (parts[2].Contains("("))
            {
                string[] offsetAndBase = parts[2].Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                offset = int.Parse(offsetAndBase[0]); // Offset
                rs = ConvertToBinary(opCode.Registers[offsetAndBase[1]], 5); // Base register
            }
            else
            {
                throw new InvalidOperationException("lw/sw formatı geçersiz: Base register veya offset eksik.");
            }
        }
        else if (parts[0] == "beq" || parts[0] == "bne") // beq ve bne özel durumu
        {
            // Eğer offset bir etiketse
            if (labels.ContainsKey(parts[3]))
            {
                offset = (labels[parts[3]] - (currentAddress + 4)) / 4; // Etiket adresine göre offset hesapla
                rs = ConvertToBinary(opCode.Registers[parts[2]], 5); // rs
            }
            else
            {
                throw new InvalidOperationException($"Offset değeri geçersiz veya etikette bulunamadı: {parts[3]}");
            }

            rs = ConvertToBinary(opCode.Registers[parts[1]], 5); // Source register (örneğin $t0)
            rt = ConvertToBinary(opCode.Registers[parts[2]], 5); // Target register (örneğin $t1)
        }
        else // Diğer I-format komutları
        {
            if (int.TryParse(parts[3], out offset)) // Eğer offset bir sayıysa
            {
                rs = ConvertToBinary(opCode.Registers[parts[2]], 5); // rs
            }
            else
            {
                throw new InvalidOperationException($"Offset değeri geçersiz: {parts[3]}");
            }
        }


        string immediate = ConvertToBinary(offset, 16); // Offset'i 16 bit binary'e çevir
        return opcode + rs + rt + immediate;
    }

    private string ParseJFormat(string[] parts, Dictionary<string, int> labels)
    {
        string opcode = ConvertToBinary(opCode.JFormat[parts[0]], 6);

        // Etiketin adresini al
        if (labels.ContainsKey(parts[1]))
        {
            int address = labels[parts[1]] / 4; // Adresin 4 byte'a bölünmüş hali
            string binaryAddress = ConvertToBinary(address, 26);
            return opcode + binaryAddress;
        }
        else
        {
            throw new InvalidOperationException($"Etiket bulunamadı: {parts[1]}");
        }
    }


    private string ConvertToBinary(int value, int bits)
    {
        return Convert.ToString(value, 2).PadLeft(bits, '0');
    }
}
