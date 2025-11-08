using System;
using System.Runtime.InteropServices;

namespace PreappPartnersLib.FileSystems
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CpkPtr : IDisposable
    {
        public void* Ptr;

        public CpkPtr(void* ptr)
        {
            Ptr = ptr;
        }

        public CpkHeader* Header => (CpkHeader*)Ptr;
        public CpkLookupTableEntry* LookupTable => (CpkLookupTableEntry*)(Header + 1);
        public CpkEntry* Entries => (CpkEntry*)(LookupTable + 65536);

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)Ptr);
        }
    }
}
