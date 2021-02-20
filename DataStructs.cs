using System;
using System.Collections.Generic;
using System.Text;

namespace SpinAnalysis.DataStructs
{
    public struct RawSample
    {
        public double TimeStampUs;
        public int Value;
    }

    public struct ProcessedSample
    {
        public double TimeStampUs;
        public int Value;
        public double DifferenceFromMean;
        public double StandardDeviationCount;
    }

    public struct ProcessedMetaData //Not sure if I'll use.
    {
        public double Mean;
        public double StandardDeviation;
    }
}
