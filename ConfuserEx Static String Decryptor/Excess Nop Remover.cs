using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfuserEx_Static_String_Decryptor
{
    class Excess_Nop_Remover
    {
        public static void NopRemover(ModuleDefMD modulee)
        {
            foreach (TypeDef types in modulee.Types)
            {
                foreach (MethodDef methods in types.Methods)
                {
                    if (!methods.HasBody) continue;
                    RemoveUnusedNops(methods);
                }
            }
        }
        public static void RemoveUnusedNops(MethodDef MethodDef)
        {
            if (MethodDef.HasBody)
            {
                for (int i = 0; i < MethodDef.Body.Instructions.Count; i++)
                {
                    Instruction Instr = MethodDef.Body.Instructions[i];

                    if (Instr.OpCode == OpCodes.Nop)
                    {
                        if (!IsNopBranchTarget(MethodDef, Instr))
                        {
                            if (!IsNopSwitchTarget(MethodDef, Instr))
                            {
                                if (!IsNopExceptionHandlerTarget(MethodDef, Instr))
                                {
                                    // Remove Nop
                                    MethodDef.Body.Instructions.RemoveAt(i);
                                    i--; // Needed?
                                }
                            }
                        }
                    }
                }
            }
        }


        private static bool IsNopBranchTarget(MethodDef MethodDef, Instruction NopInstr)
        {
            for (int i = 0; i < MethodDef.Body.Instructions.Count; i++)
            {
                Instruction Instr = MethodDef.Body.Instructions[i];

                if (Instr.OpCode.OperandType == OperandType.InlineBrTarget ||
                    Instr.OpCode.OperandType == OperandType.ShortInlineBrTarget)
                {
                    if (Instr.Operand != null)
                    {
                        Instruction OperandInstr = (Instruction)Instr.Operand;

                        if (OperandInstr == NopInstr)
                            return true;
                    }
                }
            }
            return false;
        }

        private static bool IsNopSwitchTarget(MethodDef MethodDef, Instruction NopInstr)
        {
            for (int i = 0; i < MethodDef.Body.Instructions.Count; i++)
            {
                Instruction Instr = MethodDef.Body.Instructions[i];

                if (Instr.OpCode.OperandType == OperandType.InlineSwitch)
                {
                    if (Instr.Operand != null)
                    {
                        Instruction[] SwitchTargets = (Instruction[])Instr.Operand;

                        if (SwitchTargets.Contains(NopInstr))
                            return true;
                    }

                }
            }
            return false;
        }

        private static bool IsNopExceptionHandlerTarget(MethodDef MethodDef, Instruction NopInstr)
        {

            if (MethodDef.Body.HasExceptionHandlers == false)
                return false;

            var ExHandlers = MethodDef.Body.ExceptionHandlers;

            foreach (ExceptionHandler ExHandler in ExHandlers)
            {
                if (ExHandler.FilterStart == NopInstr)
                    return true;
                if (ExHandler.HandlerEnd == NopInstr)
                    return true;
                if (ExHandler.HandlerStart == NopInstr)
                    return true;
                if (ExHandler.TryEnd == NopInstr)
                    return true;
                if (ExHandler.TryStart == NopInstr)
                    return true;
            }
            return false;
        }
    }
}
