using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace TheBox.Protected.Tool
{
    public class WordXLS
    {
        #region Data Members
        public string Message = string.Empty;          // To store the Error or Message
        private Microsoft.Office.Interop.Word.Application OfficeWord;       // The Interop Object for Word
        private Microsoft.Office.Interop.Excel.Application OfficeExcel;     // The Interop Object for Excel
        object Unknown = Type.Missing;                  // For passing Empty values
        public enum StatusType { SUCCESS, FAILED };     // To Specify Success or Failure Types
        public StatusType Status;                       // To know the Current Status
        #endregion

        public WordXLS()
        {
            Status = StatusType.FAILED;
            Message = string.Empty;
        }

        /// <summary>
        /// Convert word to Html
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Target"></param>
        public void WordToHTML(object Source, object Target)
        {
            OfficeWord = new Microsoft.Office.Interop.Word.Application();
            OfficeWord.Visible = false;
            OfficeWord.Application.Visible = false;
            OfficeWord.WindowState = Microsoft.Office.Interop.Word.WdWindowState.wdWindowStateMinimize;
            try
            {
                OfficeWord.Documents.Open(ref Source, ref Unknown,
                         ref Unknown, ref Unknown, ref Unknown,
                         ref Unknown, ref Unknown, ref Unknown,
                         ref Unknown, ref Unknown, ref Unknown,
                         ref Unknown, ref Unknown, ref Unknown, ref Unknown);

                object format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML;

                OfficeWord.ActiveDocument.SaveAs(ref Target, ref format,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown);

                Status = StatusType.SUCCESS;
                Message = Status.ToString();
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (OfficeWord != null)
                {
                   // OfficeWord.Documents.Close(ref Unknown, ref Unknown, ref Unknown);
                    //((Microsoft.Office.Interop.Word._Application)OfficeWord).Quit(ref Unknown, ref Unknown, ref Unknown);
                }
            }

        }

        public void ExcelToHTML(string Source, string Target)
        {
            OfficeExcel = new Microsoft.Office.Interop.Excel.Application();
            OfficeExcel.Visible = false;
            OfficeExcel.Application.Visible = false;
            OfficeExcel.WindowState = Microsoft.Office.Interop.Excel.XlWindowState.xlMinimized;
            try
            {
                OfficeExcel.Workbooks.Open(Source, Unknown,
                          Unknown, Unknown, Unknown,
                          Unknown, Unknown, Unknown,
                          Unknown, Unknown, Unknown,
                          Unknown, Unknown, Unknown, Unknown);

                object format = Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml;

                OfficeExcel.ActiveWorkbook.SaveAs(Target, format, Unknown,
                    Unknown, Unknown, Unknown, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Unknown
                    , Unknown, Unknown, Unknown, Unknown);

                Status = StatusType.SUCCESS;
                Message = Status.ToString();
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (OfficeExcel != null)
                {
                    OfficeExcel.Workbooks.Close();
                    OfficeExcel.Quit();
                }
            }

        }

    }
}