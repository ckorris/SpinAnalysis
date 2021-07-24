using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpinAnalysis.DataStructs;

namespace SpinAnalysis
{
    public class DeviceProcessedSamples
    {
        public double Mean;
        public double StandardDeviation;

        public SortedSet<ProcessedSample> ProcessedSamples { get; private set; }

        private DeviceProcessedSamples(int deviceIndex, double mean, double standardDeviation, SortedSet<ProcessedSample> processedSamples)
        {
            Mean = mean;
            StandardDeviation = standardDeviation;
            ProcessedSamples = processedSamples;
        }

        public SortedSet<ProcessedSample> FilterByDeviationCount(double minimumDeviations)
        {
            return FilterByDeviationCount(ProcessedSamples, minimumDeviations);
        }

        public static SortedSet<ProcessedSample> FilterByDeviationCount(SortedSet<ProcessedSample> baseSet, double minimumDeviations)
        {
            SortedSet<ProcessedSample> filteredSet = new SortedSet<ProcessedSample>(baseSet, new SampleTimeComparer<ProcessedSample>());
            filteredSet.RemoveWhere(ps => ps.StandardDeviationCount < minimumDeviations);
            return filteredSet;
        }

        public SortedSet<ProcessedSample> FilterByTimeRange(double minTimeUs, double maxTimeUs)
        {
            return FilterByTimeRange(ProcessedSamples, minTimeUs, maxTimeUs);
        }

        public static SortedSet<ProcessedSample> FilterByTimeRange(SortedSet<ProcessedSample> baseSet, double minTimeUs, double maxTimeUs)
        {
            SortedSet<ProcessedSample> filteredSet = new SortedSet<ProcessedSample>(baseSet, new SampleTimeComparer<ProcessedSample>());
            filteredSet.RemoveWhere(ps => ps.TimeStampUs < minTimeUs || ps.TimeStampUs > maxTimeUs);
            return filteredSet;
        }

        public ProcessedSample GetPeakDeviation(bool useAbsolute = true)
        {
            return GetPeakDeviation(ProcessedSamples, useAbsolute);
        }

        public static ProcessedSample GetPeakDeviation(SortedSet<ProcessedSample> baseSet, bool useAbsolute = true)
        {
            if (baseSet == null || baseSet.Count == 0)
            {
                throw new ArgumentException("Passed in null or empty sample set.");
            }

            double highestSTDCount = double.MinValue;
            ProcessedSample highestSample = baseSet.ElementAt(0);

            foreach (ProcessedSample sample in baseSet)
            {
                double stds = (useAbsolute) ? Math.Abs(sample.StandardDeviationCount) : sample.StandardDeviationCount;

                if (stds > highestSTDCount)
                {
                    highestSTDCount = stds;
                    highestSample = sample;
                }
            }

            return highestSample;
        }




        public static DeviceProcessedSamples CreateFromRaw(DeviceRawSamples deviceRawSamples)
        {
            List<RawSample> rawSamples = deviceRawSamples.RawSamples.ToList();

            if (rawSamples.Count == 0)
            {
                throw new Exception("No raw samples added before trying to process.");
            }

            //Calculate the mean, and simultaneously add new processed sample entry to a new list.
            double mean = 0;
            foreach (RawSample sample in rawSamples)
            {
                mean += sample.Value;
            }

            mean /= rawSamples.Count;

            //Get the difference of each sample from the mean.
            double squaredmeandifferencemean = 0;
            double[] differencesFromMean = new double[rawSamples.Count];
            for (int i = 0; i < rawSamples.Count; i++)
            {
                RawSample sample = rawSamples[i];

                double differenceFromMean = sample.Value - mean;
                differencesFromMean[i] = differenceFromMean;
                squaredmeandifferencemean += Math.Pow(differenceFromMean, 2);
            }

            squaredmeandifferencemean /= rawSamples.Count;

            //Calculate the final standard deviation.
            double standardDeviation = Math.Sqrt(squaredmeandifferencemean);

            //Calculate the deviation count for each sample, and also make a new list of them.
            List<ProcessedSample> processedSamples = new List<ProcessedSample>();
            for (int i = 0; i < rawSamples.Count; i++)
            {
                RawSample sample = rawSamples[i]; //Shorthand.
                ProcessedSample newProcessedSample = new ProcessedSample()
                {
                    Value = sample.Value,
                    TimeStampUs = sample.TimeStampUs,
                    DifferenceFromMean = differencesFromMean[i],
                    StandardDeviationCount = sample.Value / standardDeviation
                };
                processedSamples.Add(newProcessedSample);
            }

            //Store the temp list as the persistent set.
            SortedSet<ProcessedSample> finalSet = new SortedSet<ProcessedSample>(processedSamples, new SampleTimeComparer<ProcessedSample>());

            //Create a processed set and return it.
            return new DeviceProcessedSamples(deviceRawSamples.DeviceIndex, mean, standardDeviation, finalSet);

        }


    }
}
