
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Snippets.Font;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Nesdesign.Handlers
{
    class PdfHandler
    {
        /*
        public static void AddMetadata(PdfDocument document, string title)
        {
            document.Info.Title = title;
            document.Info.Author = "NESDESIGN sp z o.o.";
        }
        */
        public static void AddTable(Section section)
        {
            Table table = new Table
            {
                Borders = { Width = 0.75 },
                Columns =                 {
                    new Column {  },
                    new Column { },
                    new Column {  }
                },

            };
           // section.Add(table);
        }

        public static void CreatePDF()
        {
            if (Capabilities.Build.IsCoreBuild)
                GlobalFontSettings.FontResolver = new FailsafeFontResolver();

            var document = new Document
            {
                Info =
    {
        Title = "List przewozowy",
        Subject = "-",
        Author = "NESDESIGN sp. z.o.o"
    }
            };
            var section = document.AddSection();
            var headerTable = section.AddTable();
            AddTable(section);
            /*var paragraph = section.AddParagraph();
            paragraph.Format.Font.Color = Colors.DarkBlue;
            paragraph.AddFormattedText("Hello, World!", TextFormat.Bold);*/
            string filename = Path.Join(SettingsManager.Instance.GetValue("BASE_PATH"), "List przewozowy.pdf");
            var pdfRenderer = new PdfDocumentRenderer
            {
                Document = document,
                PdfDocument =
                 {
                    PageLayout = PdfPageLayout.SinglePage,
                    ViewerPreferences =
                {
                    FitWindow = true
                }
            }
           };
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(filename);

        }
    }
}
