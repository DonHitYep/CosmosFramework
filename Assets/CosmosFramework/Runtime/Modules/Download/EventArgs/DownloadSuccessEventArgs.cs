﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class DownloadSuccessEventArgs : GameEventArgs
    {
        public string URI { get; private set; }
        public string DownloadPath { get; private set; }
        public byte[] DownloadData { get; private set; }
        public override void Release()
        {
            URI = null;
            DownloadData = null;
            DownloadPath = null;
        }
        public static DownloadSuccessEventArgs Create(string uri, string downloadPath, byte[] downloadData)
        {
            var eventArgs = ReferencePool.Accquire<DownloadSuccessEventArgs>();
            eventArgs.URI = uri;
            eventArgs.DownloadData= downloadData;
            eventArgs.DownloadPath = downloadPath;
            return eventArgs;
        }
        public static void Release(DownloadSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
