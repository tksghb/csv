# CSV

Read CSV file based on RFC 4180 below and return string to insert into database.


## Definition of the CSV Format

It's based on RFC 4180.

* Each field may or may not be enclosed in double quotes.
* Fields containing double quotes and commas should be enclosed in double-quotes.
* A double-quote appearing inside a field must be escaped by preceding it with another double quote.
* Null appearing inside a field must be nothing.
* Empty string appearing inside a field must be enclosed in double-quotes.
* Boolean data type appearing inside a field must be true/false or 1/0.
* There may be header line(s).
* Encoding is UTF-8.

| Column1 | Column2        | Column3 |        | CSV File          |        | Return              |
| ------- | -------------- | ------- | ------ | ----------------- | -------| ------------------  |
| aaa     | bb,b           | ccc     |   ->   | aaa,"bb,b",ccc    |   ->   | 'aaa','bb,b','ccc'  |
| aaa     | bb"b           | ccc     |   ->   | aaa,"bb""b",ccc   |   ->   | 'aaa','bb"b','ccc'  |
| aaa     | "bbb"          | ccc     |   ->   | aaa,"""bbb""",ccc |   ->   | 'aaa','"bbb"','ccc' |
| aaa     | (null)         | ccc     |   ->   | aaa,,ccc          |   ->   | 'aaa',NULL,'ccc'    |
| aaa     | (empty string) | ccc     |   ->   | aaa,"",ccc        |   ->   | 'aaa','','ccc'      |
| aaa     | true           | ccc     |   ->   | aaa,1,ccc         |   ->   | 'aaa','1','ccc'     |
| aaa     | true           | ccc     |   ->   | aaa,true,ccc      |   ->   | 'aaa','true','ccc'  |


## Reason to Use

There is no library to read CSV file which is based on RFC 4180 definition above. The methods below are NOT possible.

* ~~TextFieldParser.ReadFields~~
* ~~SqlBulkCopy~~
* ~~SSIS~~


## Usage

1. Add Reference<br/>
   Right click your "References" in your "Solution Explorer" and click "Add Reference", then search for "Microsoft.VisualBasic" and add it to use TextFieldParser.

2. Add CsvReader.cs<br/>
   Copy and paste CsvReader.cs in your project.

3. Execute the Method<br/>
   Create instance, set CSV file path and number of header row, then execute the method!


## Example

```C#
string filePath = @"C:\YourFolder"; //CSV file path
string connectionString = @"Server=YourServer;Database=YourDB;Trusted_Connection=true"; //database connection

//get csv file name in a folder
string[] csvFiles = Directory.GetFiles(filePath, "*.csv")
                             .Select(path => Path.GetFileName(path))
                             .ToArray();

//throw exception if no csv file
if (csvFiles.Count() == 0)
    throw new FileNotFoundException();

//manipulate each csv file
foreach (string file in csvFiles)
{
    //read csv file
    CsvReader csvReader = new CsvReader();
    csvReader.FilePath = filePath + @"\" + file;
    csvReader.HeaderRowCount = 1;
    List<string> csvValues = csvReader.ReadCsv();

    //insert into database
    string tableName = file.Substring(0, file.Length - 4); //if table name == file name before ".csv"
    foreach (string csvValue in csvValues)
    {
        string queryString = "INSERT INTO " + tableName + " VALUES (" + csvValue + ") ";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            command.Connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
```

## License

This code is available under [MIT License](https://en.wikipedia.org/wiki/MIT_License).
