using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace CsvReader
{
    class CsvReader
    {
        public List<string> ReadCsv()
        {
            List<string> sqlValues = new List<string>();
            int rowIndex = 0;

            using (TextFieldParser parser = new TextFieldParser(FilePath, Encoding.UTF8))
            {
                //row
                while (!parser.EndOfData)
                {
                    string csvLine = parser.ReadLine(); //get all string in a row

                    //skip header
                    if (rowIndex >= HeaderRowCount)
                    {
                        int commaCount = csvLine.Split(',').Length; //may be more than the field count 
                        string sqlValue = "";

                        //column
                        for (int i = 0; i < commaCount; i++)
                        {
                            int startIndex = 0;
                            string startString = "";
                            string endString = "";
                            string csvField = "";
                            bool doubleQuotesFlag = false;
                            bool lastColumnFlag = false;

                            //get first letter
                            if (csvLine.Length > 0)
                            {
                                startString = csvLine.Substring(0, 1);
                            }

                            //check double quote in first letter
                            if (startString == "\"")
                            {
                                startIndex = 1;
                                endString = "\",";
                                doubleQuotesFlag = true;
                            }
                            else
                            {
                                endString = ",";
                            }

                            // check commma in last letter
                            if (csvLine.Contains(endString)) //not last
                            {
                                //get field value and remove the value from a row
                                csvField = csvLine.Substring(startIndex, csvLine.IndexOf(endString, startIndex) - startIndex);
                                csvLine = csvLine.Remove(0, csvLine.IndexOf(endString, startIndex) + endString.Length);
                            }
                            else //last field
                            {
                                if (doubleQuotesFlag == false)
                                {
                                    csvField = csvLine.Substring(startIndex);
                                }
                                else
                                {
                                    csvField = csvLine.Substring(startIndex, csvLine.Length - startIndex - 1); //get field value except for last double quote
                                }

                                lastColumnFlag = true;
                            }

                            //edit field value for SQL
                            if (csvField == "" && doubleQuotesFlag == false)
                            {
                                csvField = "NULL"; //set null if no double quotes and empty string
                            }
                            else
                            {
                                csvField = "'" + csvField.Replace("\"\"", "\"").Replace("'", "''") + "'";
                            }

                            if (i > 0)
                            {
                                sqlValue += ",";
                            }

                            sqlValue += csvField;

                            if (lastColumnFlag == true)
                            {
                                break;
                            }
                        }

                        sqlValues.Add(sqlValue);
                    }

                    rowIndex++;
                }
            }

            return sqlValues;
        }

        public string FilePath { get; set; }

        public int HeaderRowCount { get; set; }
    }
}
