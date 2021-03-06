using System;
using System.Collections.Generic;
using System.Text;
using SpinAnalysis.DataStructs;

namespace SpinAnalysis
{
    public class RawDeviceRegistrar
    {
        public Dictionary<int, DeviceRawSamples> DevicesByIndex { get; private set; } = new Dictionary<int, DeviceRawSamples>();

        public void AddRawSamples(int deviceIndex, List<RawSample> newSamples)
        {
            if(DevicesByIndex.ContainsKey(deviceIndex) == false)
            {
                DevicesByIndex.Add(deviceIndex, new DeviceRawSamples(deviceIndex));
            }

            DevicesByIndex[deviceIndex].AddRawSamples(newSamples);
        }

        public void Export(string path) //TODO: foreach all of them once I figure out how this works.
        {
            //CSVUtilities.ExportFile(DevicesByIndex[0].RawSamples, path);
            CSVUtilities.ExportRawSamples(DevicesByIndex, path);
        }

        public void Clear()
        {
            foreach(int index in DevicesByIndex.Keys)
            {
                DevicesByIndex[index].Clear();
            }

            DevicesByIndex.Clear();
        }

    }
}
