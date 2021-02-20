using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SpinAnalysis.DataStructs;

namespace SpinAnalysis
{
    class Device
    {
        public bool IsProcessed { get; private set; } = false;

        public int SampleCount { get => _rawSamples.Count; }

        private SortedSet<RawSample> _rawSamples = new SortedSet<RawSample>(new InLineComparer<RawSample>((a, b) => a.TimeStampUs.CompareTo(b.TimeStampUs)));
        private SortedSet<ProcessedSample> _processedSamples;


        private double _mean;

        public double Mean
        {
            get
            {
                ThrowExceptionIfNotProcessedBeforeAccessingProperty("Mean");
                return _mean;
            }
        }

        private double _standardDeviation;
        public double StandardDeviation
        {
            get
            {
                ThrowExceptionIfNotProcessedBeforeAccessingProperty("StandardDeviation");
                return _standardDeviation;
            }
        }

        public void AddRawSamples(List<RawSample> newSamples)
        {
            if(IsProcessed == true)
            {
                throw new Exception("Tried to add new raw samples after processing had been done. Call reset first.");
            }

            _rawSamples.UnionWith(newSamples);

        }

        public void Reset()
        {
            _rawSamples.Clear();
            _processedSamples.Clear();


            IsProcessed = false;
        }

        public void Process()
        {
            if(_rawSamples.Count == 0)
            {
                throw new Exception("No raw samples added before trying to process.");
            }

            //Make list of processed samples. Not yet sorted, so we can iterate easier.
            List<ProcessedSample> tempProcessedSamplesList = new List<ProcessedSample>();

            foreach (RawSample rawSample in _rawSamples)
            {
                ProcessedSample newProcessedSample = new ProcessedSample()
                { 
                    TimeStampUs = rawSample.TimeStampUs,
                    Value = rawSample.Value
                };

                tempProcessedSamplesList.Add(newProcessedSample);
            }

            //Calculate the mean, and simultaneously add new processed sample entry to a new list.
            _mean = 0;
            foreach(ProcessedSample sample in tempProcessedSamplesList)
            {
                _mean += sample.Value;
            }

            _mean /= _rawSamples.Count;

            //Get the difference of each sample from the mean.
            double squaredmeandifferencemean = 0;
            for(int i = 0; i < tempProcessedSamplesList.Count; i++)
            {
                ProcessedSample sample = tempProcessedSamplesList[i]; //Shorthand.

                double differenceFromMean = sample.Value - _mean;
                sample.DifferenceFromMean = differenceFromMean;
                squaredmeandifferencemean += Math.Pow(differenceFromMean, 2);
            }

            squaredmeandifferencemean /= tempProcessedSamplesList.Count;

            //Calculate the final standard deviation.
            _standardDeviation = Math.Sqrt(squaredmeandifferencemean);

            //Store the deviation count in each sample.
            for(int i = 0; i < tempProcessedSamplesList.Count; i++)
            {
                ProcessedSample sample = tempProcessedSamplesList[i]; //Shorthand.

                sample.StandardDeviationCount = sample.Value / _standardDeviation;
            }

            //Store the temp list as the persistent set.
            _processedSamples = new SortedSet<ProcessedSample>(tempProcessedSamplesList, new InLineComparer<ProcessedSample>((a, b) => a.TimeStampUs.CompareTo(b.TimeStampUs)));

            IsProcessed = true;
        }


        private class InLineComparer<T> : IComparer<T>
        {
            private readonly Comparison<T> _comparison;

            public InLineComparer(Comparison<T> comparison)
            {
                _comparison = comparison;
            }

            public int Compare(T x, T y)
            {
                return _comparison(x, y);
            }

            
        }

        private void ThrowExceptionIfNotProcessedBeforeAccessingProperty(string propertyName)
        {
            if(IsProcessed == false)
            {
                throw new Exception("Tried to access " + propertyName + " before data was processsed.");
            }
        }
    }
}
