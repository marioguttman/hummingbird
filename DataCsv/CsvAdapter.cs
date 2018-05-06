using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

namespace DataCsv {
    public class CsvAdapter {

        // ****************************** Module Variables *********************************
        //public DataSet DataSet { get { return this.dataSet; } }
        public DataTable DataTable { get { return this.dataTable; } }

        //private DataSet dataSet;
        private DataTable dataTable;

        private string filePath = null;
        private string programName;

        // ******************************** Constructor ************************************
        /// <summary>
        /// Reads and writes to a .csv file from a data table.
        /// </summary>
        /// <param name="filePath">Full path to the .csv file.</param>
        /// <param name="dataTable">A System.Data.DataTable.</param>
        /// <param name="programName">A string with the calling program name for use in error messages.</param>
        public CsvAdapter(string filePath, DataTable dataTable, string programName)  {
            this.filePath = filePath;
            this.dataTable = dataTable;
            this.programName = programName;
        }


        #region Public Functions
        // ****************************************************** Public Functions **********************************************************
        public bool ReadFile() {
            try {
                int row = 1;
                using (CsvFileReader csvFileReader = new CsvFileReader(this.filePath)) {
                    List<string> columnValues = new List<string>();
                    while (csvFileReader.ReadRow(columnValues)) {
                        if (row > 1) {  // Skip first row that has column headings.
                            DataRow dataRow = this.dataTable.NewRow();
                            for (int i = 0; i < dataTable.Columns.Count; i++) {
                                if (this.dataTable.Columns[i].DataType == typeof(string)) dataRow[i] = columnValues[i];
                                else if (this.dataTable.Columns[i].DataType == typeof(int)) dataRow[i] = Convert.ToInt32(columnValues[i]);
                                else if (this.dataTable.Columns[i].DataType == typeof(double)) dataRow[i] = Convert.ToDouble(columnValues[i]);
                                else if (this.dataTable.Columns[i].DataType == typeof(DateTime)) dataRow[i] = Convert.ToDateTime(columnValues[i]);
                            }
                            this.dataTable.Rows.Add(dataRow);
                        }
                        row++;
                    }
                }
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(
                      "ReadCsvFile() failed.\n\n"
                    + "System error message:\n" + exception.Message, this.programName);
                return false;
            }
        }
        public bool WriteFile() {
            try {
                using (CsvFileWriter csvFileWriter = new CsvFileWriter(this.filePath)) {
                    List<string> columnValues = new List<string>();
                    foreach (DataColumn column in this.dataTable.Columns) {
                        columnValues.Add(column.ColumnName);
                    }
                    csvFileWriter.WriteRow(columnValues);
                    foreach (DataRow dataRow in this.dataTable.Rows) {
                        columnValues = new List<string>();
                        foreach (DataColumn column in this.dataTable.Columns) {
                            columnValues.Add(dataRow[column.ColumnName].ToString());
                        }
                        csvFileWriter.WriteRow(columnValues);
                    }
                }
                return true;
            }
            catch (Exception exception) {
                System.Windows.Forms.MessageBox.Show(
                      "WriteCsvFile() failed.\n\n"
                    + "System error message:\n" + exception.Message, this.programName);
                return false;
            }
        }
        #endregion
    }
}
