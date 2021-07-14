using System;
using System.Collections.Generic;
using System.Text;
using SpinAnalysis.DataStructs;

namespace SpinAnalysis
{
    public static class SampleSetUtilities
    {
        public static void SplitSampleDataIntoArrays<T, TimeFormat, ValueFormat>(SortedSet<T> baseSet, out TimeFormat[] timeStampsUs, out ValueFormat[] values) 
            where T : ISpinSampleData
            where TimeFormat : IConvertible
            where ValueFormat : IConvertible
        {
            timeStampsUs = new TimeFormat[baseSet.Count];
            values = new ValueFormat[baseSet.Count];

            int index = 0;
            foreach(T sample in baseSet)
            {

                timeStampsUs[index] = (TimeFormat)Convert.ChangeType(sample.TimeStampUs, typeof(TimeFormat));
                values[index] = (ValueFormat)Convert.ChangeType(sample.Value, typeof(ValueFormat));
                index++;
            }
        }

        public static void SplitProcessedSampleDataIntoArrays<TimeFormat, ValueFormat>(SortedSet<ProcessedSample> baseSet, out TimeFormat[] timeStampsUs, out ValueFormat[] values,
            out ValueFormat[] differencesFromMean, out ValueFormat[] standardDeviationCount)
        {
            timeStampsUs = new TimeFormat[baseSet.Count];
            values = new ValueFormat[baseSet.Count];
            differencesFromMean = new ValueFormat[baseSet.Count];
            standardDeviationCount = new ValueFormat[baseSet.Count];

            int index = 0;
            foreach (ProcessedSample sample in baseSet)
            {
                timeStampsUs[index] = (TimeFormat)Convert.ChangeType(sample.TimeStampUs, typeof(TimeFormat));
                values[index] = (ValueFormat)Convert.ChangeType(sample.Value, typeof(ValueFormat));
                differencesFromMean[index] = (ValueFormat)Convert.ChangeType(sample.DifferenceFromMean, typeof(ValueFormat));
                standardDeviationCount[index] = (ValueFormat)Convert.ChangeType(sample.StandardDeviationCount, typeof(ValueFormat));
                index++;
            }
        }

    }
}
