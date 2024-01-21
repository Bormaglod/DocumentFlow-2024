//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Git: https://github.com/mephraim/ghostscriptsharp
// Date: 24.09.2023
//-----------------------------------------------------------------------

namespace DocumentFlow.Ghostscript.Enums;

/// <summary>
/// Output devices for GhostScript
/// </summary>
public enum GhostscriptDevices
{
    UNDEFINED,
    png16m,
    pnggray,
    png256,
    png16,
    pngmono,
    pngalpha,
    jpeg,
    jpeggray,
    tiffgray,
    tiff12nc,
    tiff24nc,
    tiff32nc,
    tiffsep,
    tiffcrle,
    tiffg3,
    tiffg32d,
    tiffg4,
    tifflzw,
    tiffpack,
    faxg3,
    faxg32d,
    faxg4,
    bmpmono,
    bmpgray,
    bmpsep1,
    bmpsep8,
    bmp16,
    bmp256,
    bmp16m,
    bmp32b,
    pcxmono,
    pcxgray,
    pcx16,
    pcx256,
    pcx24b,
    pcxcmyk,
    psdcmyk,
    psdrgb,
    pdfwrite,
    pswrite,
    epswrite,
    pxlmono,
    pxlcolor
}

/// <summary>
/// Native page sizes
/// </summary>
/// <remarks>
/// Missing 11x17 as enums can't start with a number, and I can't be bothered
/// to add in logic to handle it - if you need it, do it yourself.
/// </remarks>
public enum GhostscriptPageSizes
{
    UNDEFINED,
    ledger,
    legal,
    letter,
    lettersmall,
    archE,
    archD,
    archC,
    archB,
    archA,
    a0,
    a1,
    a2,
    a3,
    a4,
    a4small,
    a5,
    a6,
    a7,
    a8,
    a9,
    a10,
    isob0,
    isob1,
    isob2,
    isob3,
    isob4,
    isob5,
    isob6,
    c0,
    c1,
    c2,
    c3,
    c4,
    c5,
    c6,
    jisb0,
    jisb1,
    jisb2,
    jisb3,
    jisb4,
    jisb5,
    jisb6,
    b0,
    b1,
    b2,
    b3,
    b4,
    b5,
    flsa,
    flse,
    halfletter
}