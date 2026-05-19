
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Nesdesign.Models;
using PdfSharp;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Snippets.Font;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;


namespace Nesdesign.Handlers
{
    class PdfHandler
    {
        private BillData billData;
        public static string CITY = "Kraków";
        public static string SENDER_LABEL = "Nadawca:";
        public static string RECEIVER_LABEL = "Odbiorca:";
        public static string SENDER_LINE_1 = "NESDESIGN Sp. z o.o.";
        public static string SENDER_LINE_2 = "ul. Skotnicka 224, 30-394 Kraków";
        public static string SENDER_LINE_3 = "Marcin Gacoń";
        public static string BILL_NR_LABEL = "LIST PRZEWOZOWY NR:";
        public static string ORDER_NR_LABEL = "Zamówienie nr:";
        public string PARTS_TABLE_HEADER_1 = "Poz.";
        public string PARTS_TABLE_HEADER_2 = "Nazwa/Numer";
        public string PARTS_TABLE_HEADER_3 = "Liczba";
        private string receiverLine1 = "";
        private string receiverLine2 = "";
        private string receiverLine3 = "";

        
         
        public void AddTable(Section section)
        {
            float sectionWidth = section.PageSetup.PageWidth - section.PageSetup.LeftMargin - section.PageSetup.RightMargin;
            Table table = new Table
            {
                Borders = { Width = 0.75, Color=Color.FromRgb(0,0,0), Visible=false },
            };
            for (int i = 0; i < 9; i++)
            {
                Column column = table.AddColumn();
                column.Width = sectionWidth / 10;
            }
            table.Columns[8].Width = sectionWidth / 5;
            table.AddRow();
      

            for (int i =0; i<3;i++)
            {
                Cell cell = table.Rows[0][i * 3];
                cell.MergeRight = 2;
            }
            Row row1 = table.AddRow();
            row1.Format.Alignment = ParagraphAlignment.Right;
            //City
            Cell cityCell = table.Rows[1][6];
            cityCell.MergeRight = 1;
            cityCell.AddParagraph(CITY + ",");

            //Todays date
            Cell dateCell = table.Rows[1][8];
            dateCell.AddParagraph(DateTime.Today.ToString("dd.MM.yyyy"));

            //Margin
           
            Row marginRow = table.AddRow();
            marginRow.Height = 50;
            marginRow.Borders.Top.Visible = true;
            //Sender & Receiver
            table.AddRow();
            table.AddRow();
            table.AddRow();
            table.AddRow();
            for (int i = 0; i < 4; i++)
            {
                Row row = table.Rows[3 + i];
                row.Format.Alignment = ParagraphAlignment.Center;
                if (i > 0)
                    row.Format.Font.Bold = true;
                row[0].MergeRight = 3;
                row[6].MergeRight = 2;
            }
            //Sender
            table.Rows[3][0].AddParagraph(SENDER_LABEL);
            table.Rows[4][0].AddParagraph(SENDER_LINE_1);
            table.Rows[5][0].AddParagraph(SENDER_LINE_2);
            table.Rows[6][0].AddParagraph(SENDER_LINE_3);
            //Receiver
            table.Rows[3][6].AddParagraph(RECEIVER_LABEL);
            table.Rows[4][6].AddParagraph(receiverLine1);
            table.Rows[5][6].AddParagraph(receiverLine2);
            table.Rows[6][6].AddParagraph(receiverLine3);
            //Margin
            Row marginRow1 = table.AddRow();
            marginRow1.Height = 50;
            //Bill number
            Row billNumberRow = table.AddRow();
            //billNumberRow.Format.Font.Size = 16;
            //Bill label
            Cell billNumberLabelCell = table.Rows[8][0];
            billNumberLabelCell.MergeRight = 2;
            billNumberLabelCell.AddParagraph(BILL_NR_LABEL);

            Cell billNumberCell = table.Rows[8][7];
            billNumberCell.MergeRight = 1;
            billNumberCell.AddParagraph(billData.BillNr);
            //Order number
            Row orderNumberRow = table.AddRow();
            Cell orderNumberLabelCell = table.Rows[9][0];
            orderNumberLabelCell.MergeRight = 1;
            orderNumberLabelCell.AddParagraph(ORDER_NR_LABEL);
            
            Cell orderNumberCell = table.Rows[9][2];
            orderNumberCell.MergeRight = 2;
            orderNumberCell.Format.Font.Bold = true;
            orderNumberCell.AddParagraph(billData.OrderNumber);
            //Margin
            Row marginRow2 = table.AddRow();
            marginRow2.Height = 50;
            //Parts
            Row partsTableHeaderRow = table.AddRow();
            partsTableHeaderRow.Format.Shading.Color = Color.FromRgb(200, 200, 200);
            partsTableHeaderRow.Format.Font.Bold = true;
            partsTableHeaderRow.Borders.Visible = true;
            // Part ind header
            Cell partsTableHeaderCell1 = partsTableHeaderRow[0];
            partsTableHeaderCell1.AddParagraph(PARTS_TABLE_HEADER_1);
            partsTableHeaderCell1.Format.Alignment = ParagraphAlignment.Center;
            
            // Part Name header
            Cell partsTableHeaderCell2 = partsTableHeaderRow[1];
            partsTableHeaderCell2.MergeRight = 6;
            partsTableHeaderCell2.AddParagraph(PARTS_TABLE_HEADER_2);
            // Part Name count
            Cell partsTableHeaderCell3 = partsTableHeaderRow[8];
            partsTableHeaderCell3.Format.Alignment = ParagraphAlignment.Right;
            partsTableHeaderCell3.AddParagraph(PARTS_TABLE_HEADER_3);

            //Generating part table
            int ind = 1;
            foreach (Part part in billData.Parts)
            {
                if(part.Checked)
                {
                    Row partRow = table.AddRow();
                    partRow.Borders.Visible = true;

                    Cell cell1 = partRow[0];
                    cell1.Format.Alignment = ParagraphAlignment.Center;
                    cell1.AddParagraph(ind.ToString());

                    Cell cell2 = partRow[1];
                    cell2.MergeRight = 6;
                    cell2.AddParagraph(part.Name);

                    Cell cell3 = partRow[8];
                    cell3.Format.Alignment = ParagraphAlignment.Right;
                    cell3.AddParagraph(part.Quantity.ToString());

                }
                ind++;
            }
            /*
            // Cell 0,2 - Nesdesign
            Cell companyNameCell = table.Rows[0][2];
            
            companyNameCell.AddParagraph("NESDESIGN Sp.z o.o.");

            */






            section.Add(table);
        }

        public void CreatePDF(BillData billData)
        {
            this.billData = null;
            this.billData = billData;
            this.receiverLine1 = billData.CompanyName;
            this.receiverLine2 = billData.CompanyAddress;
            this.receiverLine3 = billData.Person;
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
            /*String tPdfFont = "Calibri";
            MigraDoc.DocumentObjectModel.Style style = document.Styles["Normal"];
            style.Font.Name = tPdfFont;*/
            var section = document.AddSection();
            section.PageSetup = document.DefaultPageSetup.Clone();
            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.LeftMargin /= 2;
            section.PageSetup.RightMargin /= 2;

            AddTable(section);

            string filename = Path.Join(SettingsManager.Instance.GetValue("BASE_PATH"),  billData.BillNr +".pdf");
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
            MessageBox.Show($"PDF został zapisany w: {filename}");

        }
    }
}
