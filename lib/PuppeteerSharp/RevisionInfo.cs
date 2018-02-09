﻿using System;
using System.IO;

namespace PuppeteerSharp
{
    public struct RevisionInfo
    {
        public int Revision { get; set; }
        public string FolderPath { get; set; }
        public string ExecutablePath { get; set; }
        public bool Downloaded => Directory.Exists(FolderPath);
    }
}
