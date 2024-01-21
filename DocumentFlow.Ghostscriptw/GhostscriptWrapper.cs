//-----------------------------------------------------------------------
// Copyright © 2010-2023 Тепляшин Сергей Васильевич. 
// Contacts: <sergio.teplyashin@yandex.ru>
// License: https://opensource.org/licenses/GPL-3.0
// Git: https://github.com/mephraim/ghostscriptsharp
// Date: 24.09.2023
//-----------------------------------------------------------------------

using DocumentFlow.Ghostscript.API;
using DocumentFlow.Ghostscript.Enums;
using DocumentFlow.Ghostscript.Settings;

using System.Collections;
using System.Drawing;

namespace DocumentFlow.Ghostscript;

/// <summary>
/// Wraps the Ghostscript API with a C# interface
/// </summary>
public class GhostscriptWrapper
{
    private static readonly string[] ARGS = new string[] {
			// Keep gs from writing information to standard output
            "-q",
            "-dQUIET",

            "-dBATCH",                  // Keep gs from going into interactive mode
            "-dNOPAUSE",                // Do not prompt and pause for each page
            "-dNOPROMPT",               // Disable prompts for user interaction           
			"-dNumRenderingThreads=4",  // Multi-core, come-on!
            
            // Configure the output anti-aliasing, resolution, etc
            "-dAlignToPixels=0",
            "-dGridFitTT=2",
            "-dTextAlphaBits=4",
            "-dGraphicsAlphaBits=4"
    };

    /// <summary>
    /// Generates a thumbnail jpg for the pdf at the input path and saves it 
    /// at the output path
    /// </summary>
    public static void GeneratePageThumb(string inputPath, string outputPath, int page, int dpix, int dpiy, int width = 0, int height = 0)
    {
        GeneratePageThumbs(inputPath, outputPath, page, page, dpix, dpiy, width, height);
    }

    /// <summary>
    /// Generates a collection of thumbnail jpgs for the pdf at the input path 
    /// starting with firstPage and ending with lastPage.
    /// Put "%d" somewhere in the output path to have each of the pages numbered
    /// </summary>
    public static void GeneratePageThumbs(string inputPath, string outputPath, int firstPage, int lastPage, int dpix, int dpiy, int width = 0, int height = 0)
    {
        if (IntPtr.Size == 4)
        {
            GhostScript32.CallAPI(GetArgs(inputPath, outputPath, firstPage, lastPage, dpix, dpiy, width, height));
        }
        else
        {
            GhostScript64.CallAPI(GetArgs(inputPath, outputPath, firstPage, lastPage, dpix, dpiy, width, height));
        }
    }

    /// <summary>
    /// Rasterises a PDF into selected format
    /// </summary>
    /// <param name="inputPath">PDF file to convert</param>
    /// <param name="outputPath">Destination file</param>
    /// <param name="settings">Conversion settings</param>
    public static void GenerateOutput(string inputPath, string outputPath, GhostscriptSettings settings)
    {
        if (IntPtr.Size == 4)
        {
            GhostScript32.CallAPI(GetArgs(inputPath, outputPath, settings));
        }
        else
        {
            GhostScript64.CallAPI(GetArgs(inputPath, outputPath, settings));
        }
    }

    /// <summary>
    /// Returns an array of arguments to be sent to the Ghostscript API
    /// </summary>
    /// <param name="inputPath">Path to the source file</param>
    /// <param name="outputPath">Path to the output file</param>
    /// <param name="firstPage">The page of the file to start on</param>
    /// <param name="lastPage">The page of the file to end on</param>
    private static string[] GetArgs(
        string inputPath,
        string outputPath,
        int firstPage,
        int lastPage,
        int dpix,
        int dpiy,
        int width,
        int height)
    {
        // To maintain backwards compatibility, this method uses previous hardcoded values.

        GhostscriptSettings s = new()
        {
            Device = GhostscriptDevices.jpeg
        };

        s.Page.Start = firstPage;
        s.Page.End = lastPage;
        s.Resolution = new Size(dpix, dpiy);

        GhostscriptPageSize pageSize = new();
        if (width == 0 && height == 0)
        {
            pageSize.Native = GhostscriptPageSizes.a4;
        }
        else
        {
            pageSize.Manual = new Size(width, height);
        }

        s.Size = pageSize;

        return GetArgs(inputPath, outputPath, s);
    }

    /// <summary>
    /// Returns an array of arguments to be sent to the Ghostscript API
    /// </summary>
    /// <param name="inputPath">Path to the source file</param>
    /// <param name="outputPath">Path to the output file</param>
    /// <param name="settings">API parameters</param>
    /// <returns>API arguments</returns>
    private static string[] GetArgs(
        string inputPath,
        string outputPath,
        GhostscriptSettings settings)
    {
        ArrayList args = new(ARGS);

        if (settings.Device == GhostscriptDevices.UNDEFINED)
        {
            throw new ArgumentException("An output device must be defined for Ghostscript", nameof(GhostscriptSettings.Device));
        }

        if (settings.Page.AllPages == false && (settings.Page.Start <= 0 && settings.Page.End < settings.Page.Start))
        {
            throw new ArgumentException("Pages to be printed must be defined.", "GhostscriptSettings.Pages");
        }

        if (settings.Resolution.IsEmpty)
        {
            throw new ArgumentException("An output resolution must be defined", "GhostscriptSettings.Resolution");
        }

        if (settings.Size.Native == GhostscriptPageSizes.UNDEFINED && settings.Size.Manual.IsEmpty)
        {
            throw new ArgumentException("Page size must be defined", "GhostscriptSettings.Size");
        }

        // Output device
        args.Add(string.Format("-sDEVICE={0}", settings.Device));

        // Pages to output
        if (settings.Page.AllPages)
        {
            args.Add("-dFirstPage=1");
        }
        else
        {
            args.Add(string.Format("-dFirstPage={0}", settings.Page.Start));
            if (settings.Page.End >= settings.Page.Start)
            {
                args.Add(string.Format("-dLastPage={0}", settings.Page.End));
            }
        }

        // Page size
        if (settings.Size.Native == GhostscriptPageSizes.UNDEFINED)
        {
            args.Add(string.Format("-dDEVICEWIDTHPOINTS={0}", settings.Size.Manual.Width));
            args.Add(string.Format("-dDEVICEHEIGHTPOINTS={0}", settings.Size.Manual.Height));
            args.Add("-dFIXEDMEDIA");
            args.Add("-dPDFFitPage");
        }
        else
        {
            args.Add(string.Format("-sPAPERSIZE={0}", settings.Size.Native.ToString()));
        }

        // Page resolution
        args.Add(string.Format("-dDEVICEXRESOLUTION={0}", settings.Resolution.Width));
        args.Add(string.Format("-dDEVICEYRESOLUTION={0}", settings.Resolution.Height));

        // Files
        args.Add(string.Format("-sOutputFile={0}", outputPath));
        args.Add(inputPath);

        return (string[])args.ToArray(typeof(string));
    }
}