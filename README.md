# MIPS Simulator

## Project Overview
The MIPS Simulator is a tool for executing MIPS Assembly code. It allows users to input assembly code, converts it into machine code, and simulates its execution by updating registers, memory, and the program counter in real-time. It also features a step-by-step execution mode for detailed analysis.

---

## How to Download and Use the Project

### **1. Download the Project**
1. Navigate to the GitHub repository: [MIPS Simulator Repository](https://github.com/Ahmetyildiz00/MIPS_Simulator.git).
2. Click the green **"Code"** button and select **"Download ZIP"** to download the project as a ZIP file.
3. Extract the ZIP file to a folder on your computer.

Alternatively, if you have Git installed:
```bash
git clone https://github.com/Ahmetyildiz00/MIPS_Simulator.git` 

### **2. Open the Project**

1.  Ensure you have **Visual Studio** installed with the `.NET Framework` development tools.
2.  Open the project folder and double-click the `MIPS_Simulator.sln` file to open the solution in Visual Studio.

### **3. Run the Project**

1.  In Visual Studio, build the project using `Ctrl + Shift + B` or the **Build Solution** option from the menu.
2.  Run the application using `F5` or the **Start Debugging** button.

----------

## Features

-   **Assembly Code Execution**: Converts and executes MIPS Assembly instructions.
-   **Step-by-Step Mode**: Execute instructions one at a time to observe their effects.
-   **Register and Memory Updates**: Dynamically displays changes to registers and memory.
-   **Instruction and Data Memory Visualization**: Shows how instructions and data are stored.

----------

## Supported Instructions

-   **Arithmetic**: `add`, `addi`, `sub`
-   **Logical**: `and`, `or`
-   **Data Transfer**: `lw`, `sw`
-   **Branching**: `beq`, `bne`
-   **Jumping**: `j`, `jal`, `jr`
-   **Shifting**: `sll`, `srl`

----------

## Example Usage

assembly

Kodu kopyala

`addi $t0, $zero, 8      # Load 8 into $t0
addi $t1, $zero, 20     # Load 20 into $t1
sw $t1, 4($t0)          # Store $t1 at memory address ($t0 + 4)
lw $t2, 4($t0)          # Load from memory address ($t0 + 4) into $t2` 

-   **Run Mode**: Executes all instructions at once.
-   **Step-By-Step Mode**: Executes one instruction at a time, displaying real-time updates.

----------

## Requirements

-   Visual Studio with `.NET Framework` support.
-   Windows OS to run the Windows Forms Application.

----------

## Future Improvements

-   Adding pipeline simulation for advanced MIPS features.
-   Improving performance for large-scale assembly programs.
```
