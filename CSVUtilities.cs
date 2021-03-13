using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;

namespace SpinAnalysis
{
    public static class CSVUtilities
    {
        public static void ExportFileTest()
        {
            List<TestRecord> testRecords = new List<TestRecord>()
            {
                new TestRecord(){TimeStampMS = 500, InputValue = 50},
                new TestRecord(){TimeStampMS = 520, InputValue = 75},
                new TestRecord(){TimeStampMS = 540, InputValue = 125},
            };


            using (StreamWriter streamWriter = new StreamWriter("TestFile.csv"))
            {
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    //csvWriter.Configuration.in
                    csvWriter.WriteRecords(testRecords);
                }


            }

        }

        public static IEnumerable<TestRecord> ImportFileTest()
        {
            List<TestRecord> testRecords;

            using(StreamReader streamReader = new StreamReader("TestFile.csv"))
            {
                using(CsvReader csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    testRecords = csvReader.GetRecords<TestRecord>().ToList();
                }
            }

            return testRecords;
            
        }
    }

    public class TestRecord
    {
        public int TimeStampMS { get; set; }
        public int InputValue { get; set; }
    }
}

