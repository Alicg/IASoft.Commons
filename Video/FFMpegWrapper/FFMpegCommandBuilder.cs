﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FFMpegWrapper
{
    public class FFMpegCommandBuilder
    {
        private Action<double, double, int> progressCallback;
        private readonly StringBuilder parametersAccumulator = new StringBuilder();

        private static readonly IDictionary<PresetParameters, string> PresetsDictionary = new Dictionary<PresetParameters, string>
        {
            {PresetParameters.UltraFast, "ultrafast"},
            {PresetParameters.SuperFast, "superfast"},
            {PresetParameters.Medium, "medium"},
            {PresetParameters.Slower, "slower"}
        };

        private readonly TemporaryFilesStorage temporaryFilesStorage;
        private IObservable<double> stopSignal;
        private bool ignoreErrors;
        private ProcessPriorityClass processPriorityClass;

        public FFMpegCommandBuilder(TemporaryFilesStorage temporaryFilesStorage)
        {
            this.temporaryFilesStorage = temporaryFilesStorage;
        }

        public FFMpegCommandBuilder StartFrom(double startSecond)
        {
            this.parametersAccumulator.AppendFormat(CultureInfo.InvariantCulture, " -ss {0} ", startSecond);
            return this;
        }

        public FFMpegCommandBuilder DurationIs(double durationSeconds)
        {
            this.parametersAccumulator.AppendFormat(CultureInfo.InvariantCulture, " -t {0} ", durationSeconds);
            return this;
        }

        public FFMpegCommandBuilder ConcatInputsFrom(string[] filesToConcat)
        {
            var intermediateFile = this.GetIntermediateFile(".txt");
            using (var sw = new StreamWriter(intermediateFile))
            {
                foreach (var inputFile in filesToConcat)
                {
                    sw.WriteLine("file '{0}'", inputFile);
                }
                sw.Flush();
            }

            this.parametersAccumulator.AppendFormat(" -f concat -safe 0 -i \"{0}\"", intermediateFile);
            return this;
        }

        /// <summary>
        /// Reserved for future needs.
        /// </summary>
        /// <param name="filesToConcat"></param>
        /// <returns></returns>
        private FFMpegCommandBuilder ConcatInputsFromFilter(string[] filesToConcat)
        {
            var intermediateFile = this.GetIntermediateFile(".txt");
            var filterComplexBuilder = new StringBuilder();
            using (var sw = new StreamWriter(intermediateFile))
            {
                for (var index = 0; index < filesToConcat.Length; index++)
                {
                    var inputFile = filesToConcat[index];
                    sw.WriteLine("file '{0}'", inputFile);
                    filterComplexBuilder.Append($"[{index}:0] [{index}:0] ");
                }
                sw.Flush();
            }

            this.parametersAccumulator.Append(
                $" -i \"{intermediateFile}\"" +
                $" -filter_complex \"{filterComplexBuilder} concat=n={filesToConcat.Length}:v=1:a=1 [v] [a]\"" +
                $" -map \"[v]\" -map \"[a]\"");
            return this;
        }

        public FFMpegCommandBuilder AppendCustom(string customParameter)
        {
            this.parametersAccumulator.Append($" {customParameter} ");
            return this;
        }

        public FFMpegCommandBuilder InputFrom(string inputPath)
        {
            this.parametersAccumulator.AppendFormat(" -i \"{0}\" ", inputPath);
            return this;
        }

        public FFMpegCommandBuilder DrawImages(IList<DrawImageTimeRecord> imagesTimeTable)
        {
            const string OverlayTemplate = "{5}overlay={0}:{1}:enable='between(t,{2},{3})'{4}";
            var imagesFiles = new List<string>();
            var totalFilterScript = "";
            var id = 0;
            foreach (var timeRecord in imagesTimeTable)
            {
                var imageFile = this.GetIntermediateFile(".png");
                imagesFiles.Add(imageFile);
                File.WriteAllBytes(imageFile, timeRecord.ImageData);
                totalFilterScript += string.Format(CultureInfo.InvariantCulture,
                    OverlayTemplate,
                    timeRecord.LeftOffset,
                    timeRecord.TopOffset,
                    timeRecord.ImageStartSecond,
                    timeRecord.ImageEndSecond,
                    id == imagesTimeTable.Count - 1 ? "" : $" [tmp{id}];", //последний overlay без [tmp];
                    id == 0 ? "" : $"[tmp{id - 1}] "); //первый overlay без [tmp]
                id++;
            }

            var scriptFile = this.GetIntermediateFile(".txt");
            File.WriteAllText(scriptFile, totalFilterScript, Encoding.Default);
            var imageFiles = imagesFiles.Aggregate("", (t, c) => $"{t} -i \"{c}\"");

            this.parametersAccumulator.AppendFormat(" {0} -filter_complex_script:v \"{1}\"", imageFiles, scriptFile);
            return this;
        }

        public FFMpegCommandBuilder DrawImagesAndText(IList<DrawImageTimeRecord> imagesTimeTable, IList<TextTimeRecord> textTimeRecords, string pathToFonts, int fontSize)
        {
            const string OverlayTemplate = "{5}overlay={0}:{1}:enable='between(t,{2},{3})'{4}";
            var imagesFiles = new List<string>();
            var totalFilterScript = "";
            var id = 0;
            var lastStream = "";
            foreach (var timeRecord in imagesTimeTable)
            {
                var imageFile = this.GetIntermediateFile(".png");
                imagesFiles.Add(imageFile);
                File.WriteAllBytes(imageFile, timeRecord.ImageData);
                var lastOverlayInFilter = (id == imagesTimeTable.Count - 1) && !textTimeRecords.Any();
                totalFilterScript += string.Format(CultureInfo.InvariantCulture,
                    OverlayTemplate,
                    timeRecord.LeftOffset,
                    timeRecord.TopOffset,
                    timeRecord.ImageStartSecond,
                    timeRecord.ImageEndSecond,
                    lastOverlayInFilter ? "" : $" [tmp{id}];", //последний overlay без [tmp];
                    id == 0 ? "" : $"[tmp{id - 1}] "); //первый overlay без [tmp]
                lastStream = $"[tmp{id}]";
                id++;
            }

            totalFilterScript += lastStream;
            foreach (var textTimeRecord in textTimeRecords)
            {
                var i = 0;
                foreach (var line in textTimeRecord.Lines)
                {
                    totalFilterScript +=
                        $"drawtext=enable='between(t,{textTimeRecord.StartSecond.ToString(CultureInfo.InvariantCulture)},{textTimeRecord.EndSecond.ToString(CultureInfo.InvariantCulture)})'" +
                        $":fontfile={pathToFonts}" +
                        $":text='{line}'" +
                        $":fontsize={fontSize}" +
                        ":fontcolor=red" +
                        ":x=(main_w/2-text_w/2)" +
                        $":y=main_h-(text_h*({textTimeRecord.Lines.Count} -{i})) - 15,";
                    i++;
                }
            }
            totalFilterScript = totalFilterScript.Remove(totalFilterScript.LastIndexOf(','));
            totalFilterScript += "[text_out]";

            var scriptFile = this.GetIntermediateFile(".txt");
            File.WriteAllText(scriptFile, totalFilterScript, Encoding.Default);
            var imageFiles = imagesFiles.Aggregate("", (t, c) => $"{t} -i \"{c}\"");

            this.parametersAccumulator.AppendFormat(" {0} -filter_complex_script:v \"{1}\" -map [text_out] -map 0:a", imageFiles, scriptFile);
            return this;
        }

        public FFMpegCommandBuilder DrawText(IList<TextTimeRecord> textTimeRecords, string pathToFonts, int fontSize)
        {
            var drawTextParameter = "[in]";
            foreach (var textTimeRecord in textTimeRecords)
            {
                var i = 0;
                foreach (var line in textTimeRecord.Lines)
                {
                    drawTextParameter +=
                        $"drawtext=enable='between(t,{textTimeRecord.StartSecond},{textTimeRecord.EndSecond})'" +
                        $":fontfile={pathToFonts}" +
                        $":text='{line}'" +
                        $":fontsize={fontSize}" +
                        ":fontcolor=red" +
                        ":x=(main_w/2-text_w/2)" +
                        $":y=main_h-(text_h*({textTimeRecord.Lines.Count} -{i})) - 15,";
                    i++;
                }
            }
            drawTextParameter = drawTextParameter.Remove(drawTextParameter.LastIndexOf(','));
            var scriptFile = this.GetIntermediateFile(".txt");
            File.WriteAllText(scriptFile, drawTextParameter, Encoding.Default);

            this.parametersAccumulator.AppendFormat(" -filter_script:v \"{0}\"", scriptFile);
            return this;
        }

        public FFMpegCommandBuilder ApplyTimeWarp(IEnumerable<TimeWarpRecord> timeWarpRecords)
        {
            const string trimTemplate = "[0:v]trim={0}:{1},setpts=PTS-STARTPTS[v{2}];";
            const string timeWarpTemplate = "[v{0}]setpts=PTS*{1}[timewarp{0}];";
            var trims = string.Empty;
            var warps = string.Empty;
            var merges = string.Empty;
            double previousTrimEnd = 0;
            int trimIndex = 1;
            var warpRecords = timeWarpRecords as TimeWarpRecord[] ?? timeWarpRecords.ToArray();
            foreach (var timeWarpRecord in warpRecords)
            {
                trims += string.Format(CultureInfo.InvariantCulture, trimTemplate, previousTrimEnd, timeWarpRecord.StartSecond, trimIndex);
                merges += $"[v{trimIndex}]";
                trimIndex++;

                trims += string.Format(CultureInfo.InvariantCulture, trimTemplate, timeWarpRecord.StartSecond, timeWarpRecord.EndSecond, trimIndex);
                warps += string.Format(CultureInfo.InvariantCulture, timeWarpTemplate, trimIndex, timeWarpRecord.Coefficient);
                merges += $"[timewarp{trimIndex}]";

                trimIndex++;
                previousTrimEnd = timeWarpRecord.EndSecond;
            }
            trims += string.Format(CultureInfo.InvariantCulture, "[0:v]trim=start={0},setpts=PTS-STARTPTS[v{1}];", previousTrimEnd, trimIndex);
            merges += $"[v{trimIndex}]";
            this.parametersAccumulator.Append($" -filter_complex \"{trims} {warps} {merges}concat=n={warpRecords.Length * 2 + 1}:v=1:a=0[out]\" -map [out] ");
            return this;
        }

        public FFMpegCommandBuilder OutputAudioCodec(string outputCodec)
        {
            this.parametersAccumulator.AppendFormat(" -c:a {0} ", outputCodec);
            return this;
        }

        public FFMpegCommandBuilder OutputVideoCodec(string outputCodec)
        {
            this.parametersAccumulator.AppendFormat(" -c:v {0} ", outputCodec);
            return this;
        }

        public FFMpegCommandBuilder OutputSize(int width, int height)
        {
            this.parametersAccumulator.AppendFormat(" -s {0}x{1} ", width, height);
            return this;
        }

        public FFMpegCommandBuilder OutputScale(Size size)
        {
            if (size != Size.Empty)
            {
                this.parametersAccumulator.AppendFormat("-vf \"scale={0}x{1}\" ", size.Width, size.Height);
            }
            return this;
        }

        public FFMpegCommandBuilder OutputFrameRate(int frameRate)
        {
            this.parametersAccumulator.AppendFormat(" -r {0} ", frameRate);
            return this;
        }

        public FFMpegCommandBuilder OutputPreset(PresetParameters presetParameter)
        {
            var preset = PresetsDictionary.ContainsKey(presetParameter) ? PresetsDictionary[presetParameter] : PresetsDictionary[PresetParameters.Medium];
            this.parametersAccumulator.AppendFormat(" -preset {0} ", preset);
            return this;
        }

        public FFMpegCommandBuilder OutputTune(string tune)
        {
            this.parametersAccumulator.AppendFormat(" -tune {0} ", tune);
            return this;
        }

        public FFMpegCommandBuilder WithFlags(string flags)
        {
            this.parametersAccumulator.AppendFormat(" -flags {0} ", flags);
            return this;
        }

        public FFMpegCommandBuilder OutputTo(string outputPath)
        {
            this.parametersAccumulator.AppendFormat(" \"{0}\" ", outputPath);
            return this;
        }

        public FFMpegCommandBuilder WithProgressCallback(Action<double, double, int> progressCallback)
        {
            this.progressCallback = progressCallback;
            return this;
        }

        public FFMpegCommandBuilder WithStopSignal(IObservable<double> stopSignal)
        {
            this.stopSignal = stopSignal;
            return this;
        }

        public FFMpegCommandBuilder WithPriority(ProcessPriorityClass processPriorityClass)
        {
            this.processPriorityClass = processPriorityClass;
            return this;
        }

        public FFMpegCommandBuilder IgnoreErrors()
        {
            this.ignoreErrors = true;
            return this;
        }

        public FFMpegCommand BuildCommand(string pathToFfMpegExe)
        {
            return new FFMpegCommand(pathToFfMpegExe,
                this.parametersAccumulator.ToString(),
                this.progressCallback,
                this.stopSignal,
                this.processPriorityClass,
                this.ignoreErrors);
        }

        private string GetIntermediateFile(string ext)
        {
            return this.temporaryFilesStorage.GetIntermediateFile(ext);
        }
    }
}