using System;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Utilities;
using Microsoft.Diagnostics.Runtime.Utilities.Pdb;

namespace NetCoreAdmin.Threaddump
{
    /// <summary>
    /// Source https://github.com/microsoft/clrmd/blob/master/src/Samples/FileAndLineNumbers/Program.cs
    /// </summary>
    internal static class Extensions
    {
        private static readonly Dictionary<PdbInfo, PdbReader> PdbReaders = new Dictionary<PdbInfo, PdbReader>();

        public static FileAndLineNumber GetSourceLocation(this ClrStackFrame frame)
        {
            PdbReader reader = GetReaderForFrame(frame);
            if (reader is null)
            {
                return default;
            }

            PdbFunction function = reader.GetFunctionFromToken(frame.Method.MetadataToken);
            int ilOffset = FindIlOffset(frame);

            return FindNearestLine(function, ilOffset);
        }

        private static FileAndLineNumber FindNearestLine(PdbFunction function, int ilOffset)
        {
            int distance = int.MaxValue;
            FileAndLineNumber nearest = default;

            foreach (PdbSequencePointCollection sequenceCollection in function.SequencePoints)
            {
                foreach (PdbSequencePoint point in sequenceCollection.Lines)
                {
                    int dist = (int)Math.Abs(point.Offset - ilOffset);
                    if (dist < distance)
                    {
                        nearest.File = sequenceCollection.File.Name;
                        nearest.Line = (int)point.LineBegin;
                    }

                    distance = dist;
                }
            }

            return nearest;
        }

        private static int FindIlOffset(ClrStackFrame frame)
        {
            ulong ip = frame.InstructionPointer;
            int last = -1;
            foreach (ILToNativeMap item in frame.Method.ILOffsetMap)
            {
                if (item.StartAddress > ip)
                {
                    return last;
                }

                if (ip <= item.EndAddress)
                {
                    return item.ILOffset;
                }

                last = item.ILOffset;
            }

            return last;
        }

        private static PdbReader GetReaderForFrame(ClrStackFrame frame)
        {
            ClrModule module = frame.Method?.Type?.Module!;
            PdbInfo info = module?.Pdb!;

            PdbReader reader = null!;
            if (info != null)
            {
                if (!PdbReaders.TryGetValue(info, out reader!))
                {
                    SymbolLocator locator = GetSymbolLocator(module!);
                    string pdbPath = locator.FindPdb(info);
                    if (pdbPath != null)
                    {
                        reader = new PdbReader(pdbPath);
                    }

                    PdbReaders[info] = reader;
                }
            }

            return reader!;
        }

        private static SymbolLocator GetSymbolLocator(ClrModule module)
        {
            return module.Runtime.DataTarget.SymbolLocator;
        }
    }
}
