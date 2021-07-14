using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpinAnalysis.DataStructs
{
    public interface ISpinSampleData
    {
        double TimeStampUs { get; }
        int Value { get; }
    }

    public class SampleTimeComparer<T> : IComparer<T> where T : ISpinSampleData
    {
        public int Compare(T x, T y)
        {
            return x.TimeStampUs.CompareTo(y.TimeStampUs);
        }
    }

    public struct RawSample : ISpinSampleData
    {
        public int DeviceNumber;

        public double TimeStampUs;

        double ISpinSampleData.TimeStampUs => TimeStampUs;

        public int Value;
        int ISpinSampleData.Value => Value;
    }

    public partial class RawSampleMap : ClassMap<RawSample>
    {
        public RawSampleMap()
        {
            Map(m => m.DeviceNumber);
            Map(m => m.TimeStampUs);
            Map(m => m.Value);
        }
    }

    public struct ProcessedSample : ISpinSampleData
    {
        public int DeviceNumber;

        public double TimeStampUs;

        double ISpinSampleData.TimeStampUs => TimeStampUs;

        public int Value;
        int ISpinSampleData.Value => Value;

        public double DifferenceFromMean;
        public double StandardDeviationCount;
    }

    public partial class ProcessedSampleMap : ClassMap<ProcessedSample>
    {
        public ProcessedSampleMap()
        {
            Map(m => m.DeviceNumber);
            Map(m => m.TimeStampUs);
            Map(m => m.Value);
            Map(m => m.DifferenceFromMean);
            Map(m => m.StandardDeviationCount);
        }
    }

    public struct GroupedProcessedSample : ISpinSampleData
    {
        public ProcessedSample ProcessedSampleSource;

        double ISpinSampleData.TimeStampUs => ProcessedSampleSource.TimeStampUs;
        int ISpinSampleData.Value => ProcessedSampleSource.Value;

        public int DeviceIndexWithinGroup;
    }



}
