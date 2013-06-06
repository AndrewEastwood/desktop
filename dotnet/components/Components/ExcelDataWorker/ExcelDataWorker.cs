using System;
using System.Collections.Generic;
using System.Text;
using components.Shared.Defaults;
using System.IO;
using System.Collections;
using System.Data;
using OfficeOpenXml;

namespace components.Components.ExcelDataWorker
{
    public class ExcelDataWorker : DefaultComponent
    {
        public void FileWrite(string filePath, DataTable dataTable)
        {
            if (dataTable == null)
                return;

            FileInfo objFileInfo = new FileInfo(filePath);
            using (ExcelPackage pck = new ExcelPackage(objFileInfo))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Bills_" + DateTime.Now.ToString("dd-MM-yy_HH:mm:ss"));
                ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                pck.Save();
            }
        }

        public DataSet FileRead(string filePath) {
        //{
        //    DataLayoutCollection dataStore = new DataLayoutCollection();
        //    IExcelDataReader excelReader = null;

        //    using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
        //    {
        //        if (System.IO.Path.GetExtension(filePath).ToLower().Contains(".xls"))
        //            //1. Reading from a binary Excel file ('97-2003 format; *.xls)
        //            excelReader = ExcelReaderFactory.CreateBinaryReader(fs);
        //        else
        //            //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
        //            excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);

        //        excelReader.IsFirstRowAsColumnNames = true;
        //        dataStore.AddRange(excelReader.AsDataSet());
        //        excelReader.Close();

        //    }

        //    return dataStore;

            DataSet ds =new DataSet();
            FileInfo objFileInfo = new FileInfo(filePath);
            bool hasHeader = false; // adjust accordingly
            using (ExcelPackage package = new ExcelPackage(objFileInfo))
            {    // Get the work book in the file
                ExcelWorkbook workBook = package.Workbook;
                if (workBook != null)
                    for (int i = 0; i < workBook.Worksheets.Count; i++)
                    {
                        // Get the first worksheet
                        ExcelWorksheet currentWorksheet = workBook.Worksheets[i];
                        DataTable dt = new DataTable(currentWorksheet.Name);

                        // init header
                        for (int c = 0; c < currentWorksheet.Dimension.End.Column; c++)
                            dt.Columns.Add(hasHeader ? currentWorksheet.Cells[0, c].Text : string.Format("Column {0}", currentWorksheet.Cells[1, c].Value));

                        // populate data
                        for (int r = (hasHeader ? 0 : 1); r < currentWorksheet.Dimension.End.Row; r++)
                        {
                            DataRow dRow = dt.NewRow();
                            for (int c = 0; c < currentWorksheet.Dimension.End.Column; c++)
                                dRow[c] = currentWorksheet.Cells[r, c].Value;
                            dt.Rows.Add(dRow);
                        }

                        // add datatable into ds
                        ds.Tables.Add(dt);
                    }
            }

            return ds;
        }
    }
}
