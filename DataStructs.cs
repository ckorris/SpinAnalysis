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
        public double TimeStampUs;
        double ISpinSampleData.TimeStampUs => TimeStampUs;

        public int Value;
        int ISpinSampleData.Value => Value;
    }

    public struct ProcessedSample : ISpinSampleData
    {
        public double TimeStampUs;
        double ISpinSampleData.TimeStampUs => TimeStampUs;

        public int Value;
        int ISpinSampleData.Value => Value;

        public double DifferenceFromMean;
        public double StandardDeviationCount;
    }

    public struct GroupedProcessedSample : ISpinSampleData
    {
        public ProcessedSample ProcessedSampleSource;

        double ISpinSampleData.TimeStampUs => ProcessedSampleSource.TimeStampUs;
        int ISpinSampleData.Value => ProcessedSampleSource.Value;

        public int DeviceIndexWithinGroup;
    }



}
