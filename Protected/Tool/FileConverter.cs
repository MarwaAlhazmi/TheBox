using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Office;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.PowerPoint;
using System.Configuration;
using System.Runtime.InteropServices;


namespace TheBox.Protected.Tool
{
    public class FileConverter
    {
        protected Microsoft.Office.Interop.Word.Application objWord;
        protected Microsoft.Office.Interop.Excel.Application objExcel;
        
        protected Microsoft.Office.Interop.Excel.Workbooks objWorkbook;

        protected Microsoft.Office.Interop.PowerPoint.Application objPP;
        protected Microsoft.Office.Interop.PowerPoint._Presentation objPres;

        object fltDocFormat = 10;        //For filtered HTML Output
        protected object missing = System.Reflection.Missing.Value;
        //Is just to skeep the parameters which are passed as boject reference, these are seems to be optional parameters
        protected object readOnly = false;
        protected object isVisible = false;        //The process has to be in invisible mode


        public  string Convert(string FilePath)
        {
            string t = GetFileExtension(FilePath);
            switch (t)
            {
                case "DOC":
                case "DOCX":
                    return ConvertWord(FilePath);
                case "XLSX":
                case "XLS":
                    return ConvertExcel(FilePath);
                case "PPT":
                case "PPTX":
                    return ConvertPowerPoint(FilePath);
                default:
                    return "";
            }
        }
        private string GetFileExtension(string FilePath)
        {
            string[] file = FilePath.Split('\\');
            string FullFileName = file[file.Length - 1];
            // get extension
            string[] splittedFile = FullFileName.Split('.');
            string ext = splittedFile[splittedFile.Length - 1].ToUpper();

            return ext;

        }

        private string ConvertWord(string FilePath)
        {
            try
            {
                // new object 
                objWord = new Microsoft.Office.Interop.Word.Application();
                // get file name
                string[] file = FilePath.Split('\\');
                string FullFileName = file[file.Length - 1];
                // get extension
                string[] splittedFile = FullFileName.Split('.');
                string ext = splittedFile[splittedFile.Length - 1].ToUpper();
                string filename = splittedFile[splittedFile.Length - 2];


                // get convert folder directory
                string convertFolder = ConfigurationManager.AppSettings["ProjectHTMLRoot"];
                object target = HttpContext.Current.Server.MapPath(convertFolder)+ GetConvertPath(FilePath)+ filename + ".htm";//HttpContext.Current.Server.MapPath(convertFolder)  + GetConvertPath(FilePath) +
                object source = FilePath;
                

                if (ext == "DOC" || ext == "DOCX")
                {
                    //open the file internally in word. In the method all the parameters should be passed by object reference

                    objWord.Documents.Open(ref source, ref readOnly, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref  missing, ref missing, ref missing, ref isVisible, ref missing, ref missing, ref missing,
                    ref missing, ref missing);

                    //Do the background activity

                    objWord.Visible = false;

                    Microsoft.Office.Interop.Word.Document oDoc = objWord.ActiveDocument;
                    oDoc.SaveAs(ref target, ref fltDocFormat, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing);

                    //Close/quit word
                    return "../" + convertFolder + "/" + filename + ".htm";

                }
                else
                {
                    return "";
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                objWord.Quit(ref missing, ref missing, ref missing);
                objWord.DisplayAlerts = WdAlertLevel.wdAlertsNone;
                Marshal.ReleaseComObject(objWord);
            }
        }

        private string ConvertExcel(string FilePath)
        {
           
            try
            {
                // new object 
                objExcel = new Microsoft.Office.Interop.Excel.Application();
                objWorkbook = objExcel.Workbooks;
                objExcel.DisplayAlerts = false;
                // get file name
                string[] file = FilePath.Split('\\');
                string FullFileName = file[file.Length - 1];
                // get extension
                string[] splittedFile = FullFileName.Split('.');
                string ext = splittedFile[splittedFile.Length - 1].ToUpper();
                string filename = splittedFile[splittedFile.Length - 2].ToUpper();


                // get convert folder directory
                string convertFolder = ConfigurationManager.AppSettings["ProjectHTMLRoot"];
                object target = HttpContext.Current.Server.MapPath(convertFolder) + GetConvertPath(FilePath) + filename + ".htm";
                object source = FilePath;


                if (ext == "XLS" || ext == "XLSX")
                {
                    //open the file internally in word. In the method all the parameters should be passed by object reference
                    string i = source.ToString();
                    objWorkbook.Open(i, missing,
                          missing, missing, missing,
                          missing, missing, missing,
                          missing, missing, missing,
                          missing, missing, missing, missing);

                    //Do the background activity

                    objExcel.Visible = false;

                    Microsoft.Office.Interop.Excel.Workbook oDoc = objExcel.ActiveWorkbook;
                    string t = target.ToString();
                    oDoc.SaveAs(t, Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml, missing, missing, true, missing,
                        Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, missing, missing, missing);

                    return "../" + convertFolder + "/" + filename + ".htm";

                }
                else
                {
                    return "";
                }
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                //Close/quit word

                objWorkbook.Close();
                objExcel.Quit();
                objExcel.DisplayAlerts = true;
                Marshal.ReleaseComObject(objExcel);
            }


        }

        private string ConvertPowerPoint(string FilePath)
        {

            try
            {
                
                // new object 
                //objPP = new Microsoft.Office.Interop.PowerPoint.Application();
                //var objPresentation = objPP.ActivePresentation;
                
                // get file name
                string[] file = FilePath.Split('\\');
                string FullFileName = file[file.Length - 1];
                // get extension
                string[] splittedFile = FullFileName.Split('.');
                string ext = splittedFile[splittedFile.Length - 1].ToUpper();
                string filename = splittedFile[splittedFile.Length - 2].ToUpper();


                // get convert folder directory
                string convertFolder = ConfigurationManager.AppSettings["ProjectHTMLRoot"];
                object target = HttpContext.Current.Server.MapPath(convertFolder) + GetConvertPath(FilePath) + filename + ".htm";
                object source = FilePath;


                if (ext == "PPT" || ext == "PPTX")
                {
                    //open the file internally in word. In the method all the parameters should be passed by object reference
                    string i = source.ToString();

                    objPP = new Microsoft.Office.Interop.PowerPoint.Application();
                    objPP.DisplayAlerts = PpAlertLevel.ppAlertsNone;
                    Microsoft.Office.Interop.PowerPoint.Presentations oPresSet = objPP.Presentations;
                    Microsoft.Office.Interop.PowerPoint._Presentation oPres = oPresSet.Open(FilePath,
                    Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse,
                    Microsoft.Office.Core.MsoTriState.msoFalse);
                    string t = target.ToString();
                    oPres.SaveAs(t, PpSaveAsFileType.ppSaveAsHTMLv3, Microsoft.Office.Core.MsoTriState.msoTrue);

                    return "../" + convertFolder + "/" + filename + ".htm";

                }
                else
                {
                    return "";
                }
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                //Close/quit word
                //objPres.Close();
                objPP.Quit();
                Marshal.ReleaseComObject(objPP);
            }


        }

        private static string GetConvertPath(string source)
        {
            string[] arr = source.Split('\\');
            string strDefault = ConfigurationManager.AppSettings["ProjectRoot"];
            
            int i = Array.IndexOf(arr, strDefault);
            string Convert = "/";
          
            for (int u = i + 1 ; u < arr.Length - 1; u++)
            {
                Convert += arr[u] + "/";
            }
            return  Convert;
        }

    }
    public enum FileType
    {
        Word,
        Excel,
        PowerPoint,
        Text
    }

}