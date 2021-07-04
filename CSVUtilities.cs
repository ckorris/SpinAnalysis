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
        public static void ExportFile<T>(IEnumerable<T> collectionToExport, string path) 
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    //csvWriter.Configuration.in
                    csvWriter.WriteRecords(collectionToExport);
                }
            }
        }

        public static IEnumerable<T> ImportFile<T>(string path)
        {
            IEnumerable<T> loaded;

            using (StreamReader streamReader = new StreamReader("TestFile.csv"))
            {
                using (CsvReader csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    loaded = csvReader.GetRecords<T>();
                }
            }

            return loaded;

        }
    }

}

