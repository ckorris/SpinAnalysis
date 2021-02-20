﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SpinAnalysis.DataStructs;

namespace SpinAnalysis
{
    public class DeviceRawSamples
    {
        public int SampleCount { get => RawSamples.Count; }

        public SortedSet<RawSample> RawSamples { get; private set; }  = new SortedSet<RawSample>(new SampleTimeComparer<RawSample>());

        public int DeviceIndex { get; private set; }
        public DeviceRawSamples(int deviceIndex)
        {
            DeviceIndex = deviceIndex;
        }

        public void AddRawSamples(List<RawSample> newSamples)
        {
            RawSamples.UnionWith(newSamples);
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