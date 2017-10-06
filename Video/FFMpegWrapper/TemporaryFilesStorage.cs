﻿using System;
using System.Collections.Generic;
using System.IO;

namespace FFMpegWrapper
{
    public class TemporaryFilesStorage : IDisposable
    {
        private readonly List<string> temporaryFiles = new List<string>();
        
        public string GetIntermediateFile(string outputExtension)
        {
            var tempFile = Path.Combine(Directory.GetCurrentDirectory(), $"{Guid.NewGuid()}{outputExtension}");
            this.temporaryFiles.Add(tempFile);
            return tempFile;
        }

        public void Dispose()
        {
            foreach (var intermediateFile in this.temporaryFiles)
            {
                File.Delete(intermediateFile);
            }
        }
    }
}