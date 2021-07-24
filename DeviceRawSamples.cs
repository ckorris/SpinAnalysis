using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SpinAnalysis.DataStructs;
using CsvHelper.Configuration;

namespace SpinAnalysis
{
    public class DeviceRawSamples
    {
        public int SampleCount { get => RawSamples.Count; }

        public SortedSet<RawSample> RawSamples { get; private set; } = new SortedSet<RawSample>(new SampleTimeComparer<RawSample>());

        public int DeviceIndex { get; private set; }
        public DeviceRawSamples(int deviceIndex)
        {
            DeviceIndex = deviceIndex;
        }

        public void AddRawSample(RawSample newSample)
        {
            RawSamples.Add(newSample);
        }

        public void AddRawSamples(List<RawSample> newSamples)
        {
            RawSamples.UnionWith(newSamples);
        }

        public sealed class DeviceRawSamplesMap : ClassMap<DeviceRawSamples>
        {
            public DeviceRawSamplesMap()
        {

        }
    }

    /// <summary>
    /// Clears the samples. Just for ease on the GC.
    /// </summary>
    public void Clear()
    {
        RawSamples.Clear();
    }

}
}
