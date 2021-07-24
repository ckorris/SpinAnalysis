using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using SpinAnalysis.DataStructs;

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
                    //TEST - Very bad.
                    if (typeof(T) == typeof(RawSample))
                    {
                        csvWriter.Context.RegisterClassMap<RawSampleMap>();
                    }

                    csvWriter.WriteRecords(collectionToExport);
                }
            }
        }

        public static void ExportRawSamples(Dictionary<int, DeviceRawSamples> sampleDict, string path)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.Context.RegisterClassMap<RawSampleMap>();

                    foreach(KeyValuePair<int, DeviceRawSamples> kvp in sampleDict)
                    {
                        //csvWriter.WriteComment(kvp.Value.DeviceIndex.ToString());                        
                        //csvWriter.NextRecord();
                        //csvWriter.WriteHeader<RawSampleMap>();
                        csvWriter.WriteRecords(kvp.Value.RawSamples);
                        //csvWriter.NextRecord();
                    }
                }
            }
        }

        public static Dictionary<int, DeviceRawSamples> ImportRawSamples(string filePath)
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                using (CsvReader csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    csvReader.Context.RegisterClassMap<RawSampleMap>();

                    Dictionary<int, DeviceRawSamples> outDictionary = new Dictionary<int, DeviceRawSamples>();

                    List<RawSample> importedList = csvReader.GetRecords<RawSample>().ToList();

                    foreach(RawSample sample in importedList)
                    {
                        if(outDictionary.ContainsKey(sample.DeviceNumber) == false)
                        {
                            DeviceRawSamples newSamples = new DeviceRawSamples(sample.DeviceNumber);
                            outDictionary.Add(sample.DeviceNumber, newSamples);
                        }

                        outDictionary[sample.DeviceNumber].AddRawSample(sample);
                    }

                    return outDictionary;
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

