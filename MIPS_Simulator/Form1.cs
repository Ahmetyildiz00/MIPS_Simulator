using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace MIPS_Simulator
{
    public partial class Form1 : Form
    {
        private RegisterManager registerManager;
        private MemoryManager memoryManager;
        private string[] assemblyLines; // Assembly kodlarını tutar
        private int currentStep; // Şu anki adımı takip eder
        private Execute executor; // Kodları çalıştırmak için Execute sınıfı örneği


        public Form1()
        {
            InitializeComponent();
            registerManager = new RegisterManager();
            memoryManager = new MemoryManager();
            executor = new Execute(registerManager, memoryManager, this); // Executor'u başlat
            InitializeRegisters();
            currentStep = 0; // İlk adım
        }


        private void InitializeRegisters()
        {
            lvRegisters.Items.Clear();
            var registers = registerManager.GetAllRegisters();
            foreach (var reg in registers)
            {
                ListViewItem item = new ListViewItem(reg.Key);
                item.SubItems.Add(reg.Value.ToString());
                lvRegisters.Items.Add(item);
            }
        }

        private void UpdateRegisters()
        {
            lvRegisters.Items.Clear();
            var registers = registerManager.GetAllRegisters();
            foreach (var reg in registers)
            {
                ListViewItem item = new ListViewItem(reg.Key);
                item.SubItems.Add(reg.Value.ToString());
                lvRegisters.Items.Add(item);
            }
        }


        private byte[] ConvertToBytes(string binaryString)
        {
            int numOfBytes = binaryString.Length / 8;
            byte[] bytes = new byte[numOfBytes];

            for (int i = 0; i < numOfBytes; i++)
            {
                bytes[i] = Convert.ToByte(binaryString.Substring(i * 8, 8), 2);
            }

            return bytes;
        }

        private void UpdateInstructionMemory()
        {
            txtInstructionMemory.Clear();
            for (int i = 0; i < 512; i += 4)
            {
                byte[] data = memoryManager.ReadInstruction(i, 4);
                string instruction = "";
                foreach (byte b in data)
                {
                    instruction += Convert.ToString(b, 2).PadLeft(8, '0') + " ";
                }
                if (!string.IsNullOrWhiteSpace(instruction.Trim()) && instruction.Trim() != "00000000 00000000 00000000 00000000")
                {
                    txtInstructionMemory.AppendText($"Address {i}: {instruction.Trim()}" + Environment.NewLine);
                }
            }
        }



        private void UpdateDataMemory()
        {
            txtDataMemory.Clear();
            for (int i = 0; i < 512; i += 4)
            {
                byte[] data = memoryManager.ReadData(i, 4);
                int value = BitConverter.ToInt32(data, 0);
                if (value != 0) // Boş olmayan adresleri göster
                {
                    txtDataMemory.AppendText($"Address [{i}]: {value}" + Environment.NewLine);
                }
            }
        }


        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                FuncCode funcCode = new FuncCode();
                Execute executor = new Execute(registerManager, memoryManager);

                string[] assemblyLines = txtAssemblyCode.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, int> labels = funcCode.GetLabels(assemblyLines); // Etiketleri al

                foreach (var label in labels)
                {
                    Console.WriteLine($"Etiket: {label.Key}, Adres: {label.Value}");
                }

                txtMachineCode.Clear();

                int currentAddress = 0;

                foreach (string line in assemblyLines)
                {
                    if (!line.EndsWith(":")) // Etiketleri atla
                    {
                        string machineCode = funcCode.ParseAssemblyToMachineCode(line, labels, currentAddress);
                        txtMachineCode.AppendText(machineCode + Environment.NewLine);
                        memoryManager.WriteInstruction(currentAddress, ConvertToBytes(machineCode));
                        executor.ExecuteInstruction(machineCode);
                        currentAddress += 4;
                    }
                }
                while (executor.ProgramCounter < currentAddress)
                {
                    Console.WriteLine($"[DEBUG] Current PC: {executor.ProgramCounter}");

                    byte[] instructionBytes = memoryManager.ReadInstruction(executor.ProgramCounter, 4);
                    string machineCode = ConvertBytesToMachineCode(instructionBytes);

                    Console.WriteLine($"[DEBUG] Machine Code: {machineCode}");

                    int currentPC = executor.ProgramCounter;
                    executor.ExecuteInstruction(machineCode);

                    // Eğer ProgramCounter değişmediyse sıradaki komuta geç
                    if (executor.ProgramCounter == currentPC)
                    {
                        executor.ProgramCounter += 4;
                    }

                    Console.WriteLine($"[DEBUG] Updated PC: {executor.ProgramCounter}");
                }



                UpdateRegisters();
                UpdateInstructionMemory();
                UpdateDataMemory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string ConvertBytesToMachineCode(byte[] bytes)
        {
            // Byte'ları 8-bit binary string'e dönüştür ve birleştir
            return string.Join("", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
        }

        private void btnStepByStep_Click(object sender, EventArgs e)
        {
            try
            {
                if (assemblyLines == null || currentStep == 0) // İlk adım için assembly kodlarını yükle
                {
                    assemblyLines = txtAssemblyCode.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    currentStep = 0; // İlk adımı sıfırla
                }

                if (currentStep >= assemblyLines.Length)
                {
                    MessageBox.Show("All instructions have been executed.", "Step-By-Step", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return; // Tüm adımlar bitti
                }

                string currentLine = assemblyLines[currentStep]; // Mevcut assembly satırı

                if (!currentLine.EndsWith(":")) // Etiketleri atla
                {
                    // Mevcut assembly satırını makine koduna dönüştür
                    FuncCode funcCode = new FuncCode();
                    Dictionary<string, int> labels = funcCode.GetLabels(assemblyLines);
                    string machineCode = funcCode.ParseAssemblyToMachineCode(currentLine, labels, currentStep * 4);

                    // Makine kodunu instruction memory'e yaz
                    memoryManager.WriteInstruction(currentStep * 4, ConvertToBytes(machineCode));

                    // Makine kodunu çalıştır
                    executor.ExecuteInstruction(machineCode);

                    // Makine kodunu UI'ya yaz
                    txtMachineCode.AppendText(machineCode + Environment.NewLine);

                    // Instruction memory'yi UI'ya güncelle
                    UpdateInstructionMemory();
                }

                // Adımı ilerlet
                currentStep++;

                // Register'ları güncelle
                UpdateRegisters();

                // UI'nın son adımı işlediğini göster
                Console.WriteLine($"Step {currentStep}/{assemblyLines.Length} executed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during step-by-step execution: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void btnReset_Click(object sender, EventArgs e)
        {
            registerManager.ResetRegisters();
            memoryManager.ResetMemory();
            InitializeRegisters();
            txtMachineCode.Clear();
            txtInstructionMemory.Clear();
            txtDataMemory.Clear();
            txtAssemblyCode.Clear();
            currentStep = 0; // Step-by-step adımını sıfırla
            assemblyLines = null; // Assembly kodlarını sıfırla
        }

    }
}
