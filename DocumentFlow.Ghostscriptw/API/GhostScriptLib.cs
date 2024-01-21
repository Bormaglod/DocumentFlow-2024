//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Date: 24.09.2023
//-----------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;

namespace DocumentFlow.Ghostscript.API;

public static class GhostScript
{
    public static void Initialize()
    {
        NativeLibrary.SetDllImportResolver(typeof(GhostScript).Assembly, ImportResolver);
    }

    private static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        IntPtr libHandle = IntPtr.Zero;
        if (libraryName.StartsWith("gsdll"))
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ThirdParty");
            if (RuntimeInformation.ProcessArchitecture == Architecture.X86)
            {
                libHandle = NativeLibrary.Load(Path.Combine(path, "gsdll32.dll"));
            }
            else if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                libHandle = NativeLibrary.Load(Path.Combine(path, "gsdll64.dll"));
            }
        }

        return libHandle;
    }
}
