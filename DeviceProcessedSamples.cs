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
            if(baseSet == null || baseSet.Count == 0)
            {
                throw new ArgumentException("Passed in null or empty sample set.");
            }

            double highestSTDCount = double.MinValue;
            ProcessedSample highestSample = baseSet.ElementAt(0);

            foreach(ProcessedSample sample in baseSet)
            {
                double stds = (useAbsolute) ? Math.Abs(sample.StandardDeviationCount) : sample.StandardDeviationCount;

                if(stds > highestSTDCount)
                {
                    highestSTDCount = stds;
                    highestSample = sample;
                }
            }

            return highestSample;
        }




        public static DeviceProcessedSamples CreateFromRaw(DeviceRawSamples deviceRawSamples)
        {
            SortedSet<RawSample> rawSamples = deviceRawSamples.RawSamples;

            if (rawSamples.Count == 0)
            {
                throw new Exception("No raw samples added before trying to process.");
            }

            //Make list of processed samples. Not yet sorted, so we can iterate easier.
            List<ProcessedSample> tempProcessedSamplesList = new List<ProcessedSample>();

            foreach (RawSample rawSample in rawSamples)
            {
                ProcessedSample newProcessedSample = new ProcessedSample()
                {
                    TimeStampUs = rawSample.TimeStampUs,
                    Value = rawSample.Value
                };

                tempProcessedSamplesList.Add(newProcessedSample);
            }

            //Calculate the mean, and simultaneously add new processed sample entry to a new list.
            double mean = 0;
            foreach (ProcessedSample sample in tempProcessedSamplesList)
            {
                mean += sample.Value;
            }

            mean /= rawSamples.Count;

            //Get the difference of each sample from the mean.
            double squaredmeandifferencemean = 0;
            for (int i = 0; i < tempProcessedSamplesList.Count; i++)
            {
                ProcessedSample sample = tempProcessedSamplesList[i]; //Shorthand.

                double differenceFromMean = sample.Value - mean;
                sample.DifferenceFromMean = differenceFromMean;
                squaredmeandifferencemean += Math.Pow(differenceFromMean, 2);
            }

            squaredmeandifferencemean /= tempProcessedSamplesList.Count;

            //Calculate the final standard deviation.
            double standardDeviation = Math.Sqrt(squaredmeandifferencemean);

            //Store the deviation count in each sample.
            for (int i = 0; i < tempProcessedSamplesList.Count; i++)
            {
                ProcessedSample sample = tempProcessedSamplesList[i]; //Shorthand.

                sample.StandardDeviationCount = sample.Value / standardDeviation;
            }

            //Store the temp list as the persistent set.
            SortedSet<ProcessedSample> finalSet = new SortedSet<ProcessedSample>(tempProcessedSamplesList, new SampleTimeComparer<ProcessedSample>());

            //Create a processed set and return it.
            return new DeviceProcessedSamples(deviceRawSamples.DeviceIndex, mean, standardDeviation, finalSet);

        }

        
    }
}
