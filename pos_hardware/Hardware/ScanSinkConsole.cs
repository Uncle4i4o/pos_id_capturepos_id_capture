﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH.Alika.POS.Hardware
{
    class ScanSinkConsole : IScanSink
    {
        public void HandleCodeLineScan(object sender, CodeLineScanEvent e)
        {
            IScanSource source = sender as IScanSource;
            Console.WriteLine(source.DocumentSourceId);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(e));
        }

        public void Dispose()
        {
            
        }
    }
}