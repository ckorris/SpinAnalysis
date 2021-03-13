using SpinAnalysis.DataStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpinAnalysis
{
    public class ProcessedDeviceGroup
    {
        private List<DeviceProcessedSamples> _deviceSamples; 

        public SortedSet<GroupedProcessedSample> MostSignificantValues { get; private set; } = new SortedSet<GroupedProcessedSample>();

        public ProcessedDeviceGroup(List<DeviceProcessedSamples> deviceSamples)
        {
            if(deviceSamples == null || deviceSamples.Count == 0)
            {
                throw new ArgumentException("Passed in a null or empty list of device samples.");
            }

            int deviceCount = deviceSamples.Count; //Shorthand.

            //For now we have no way of gracefully handling if the samples started at slightly different times, so enforce that they started at the same time.
            //Do the same for the number of samples.
            double firstStartTime = deviceSamples[0].ProcessedSamples.ElementAt(0).TimeStampUs;
            for(int i = 1; i < deviceCount; i++)
            {
                double startTime = deviceSamples[i].ProcessedSamples.ElementAt(0).TimeStampUs;
                if(startTime != firstStartTime)
                {
                    throw new Exception("Samples in group did not start at the same time. First item started at " + firstStartTime + "㎲, but item at index " + i +
                        " started at " + startTime + "㎲.");
                }

                if(deviceSamples[i].ProcessedSamples.Count != deviceSamples[0].ProcessedSamples.Count)
                {
                    throw new Exception("Devices in group have different numbers of samples. First item has " + deviceSamples[0].ProcessedSamples.Count + " samples, but device at index " + i +
                        " has " + deviceSamples[i].ProcessedSamples.Count + " samples.");
                }

            }

            int sampleCount = deviceSamples[0].ProcessedSamples.Count; //Shorthand.

            //Make a list with the most significant of each device.
            List<GroupedProcessedSample> groupedSamples = new List<GroupedProcessedSample>();

            Dictionary<int, IEnumerator<ProcessedSample>> enumerators = new Dictionary<int, IEnumerator<ProcessedSample>>(); //Key is index within this group, as in, index in list passed in args.
            
            for(int i = 0; i < deviceCount; i++)
            {
                enumerators.Add(i, deviceSamples[i].ProcessedSamples.GetEnumerator());
            }

            for(int s = 0; s < sampleCount; s++)
            {
                ProcessedSample mostSignificantSample = enumerators[0].Current;
                int mostSignificantDeviceIndex = 0;
                for(int d = 0; d < deviceCount; d++)
                {

                    if(enumerators[d].Current.StandardDeviationCount > mostSignificantSample.StandardDeviationCount)
                    {
                        mostSignificantSample = enumerators[d].Current;
                        mostSignificantDeviceIndex = d;
                    }
                    enumerators[d].MoveNext();
                }

                GroupedProcessedSample groupedSample = new GroupedProcessedSample()
                {
                    ProcessedSampleSource = mostSignificantSample,
                    DeviceIndexWithinGroup = mostSignificantDeviceIndex
                };

                groupedSamples.Add(groupedSample);
            }

            MostSignificantValues = new SortedSet<GroupedProcessedSample>(groupedSamples, new SampleTimeComparer<GroupedProcessedSample>());

        }

    }
}
