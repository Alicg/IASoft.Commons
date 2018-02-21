using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public StringBuilder ParametersAccumulator => this.parametersAccumulator;

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

        public FFMpegCommandBuilder ConcatDrawImagesAndText(IList<string> filesToConcat,
            IList<DrawImageTimeRecord> imagesTimeTable,
            IList<TextTimeRecord> textTimeRecords,
            Size finalScale,
            string pathToFonts,
            int fontSize)
        {
            string outConcatVStream;
            string outConcatAStream;
            var concatFilter = this.BuildConcatFilter(filesToConcat.Count, finalScale, out outConcatVStream, out outConcatAStream);

            var imagesFiles = new List<string>();
            string imageStream;
            var drawImagesFilter = this.BuildDrawImagesFilter(outConcatVStream,
                filesToConcat.Count,
                imagesTimeTable,
                imagesFiles,
                out imageStream);

            string textOutputStream;
            var drawTextFilter = this.BuildDrawTextFilter(imageStream,
                textTimeRecords,
                pathToFonts,
                fontSize,
                out textOutputStream);

            var semicoloneAfterConcatFitler =
                !string.IsNullOrEmpty(drawImagesFilter) || !string.IsNullOrEmpty(drawTextFilter)
                    ? ";"
                    : "";
            var semicoloneBetweenImageAndTextFilters =
                !string.IsNullOrEmpty(drawImagesFilter) && !string.IsNullOrEmpty(drawTextFilter)
                    ? ";"
                    : "";
            var scriptFile = this.GetIntermediateFile(".txt");
            File.WriteAllText(scriptFile,
                concatFilter + semicoloneAfterConcatFitler + drawImagesFilter + semicoloneBetweenImageAndTextFilters +
                drawTextFilter,
                Encoding.Default);

            var inputFiles = filesToConcat.Union(imagesFiles).Aggregate("", (t, c) => $"{t} -i \"{c}\"");

            this.parametersAccumulator.AppendFormat(" {0} -filter_complex_script:v \"{1}\" -map {2} -map {3}",
                inputFiles,
                scriptFile,
                textOutputStream,
                outConcatAStream);
            return this;
        }

        public FFMpegCommandBuilder DrawImagesAndText(IList<DrawImageTimeRecord> imagesTimeTable, IList<TextTimeRecord> textTimeRecords, string pathToFonts, int fontSize)
        {
            var imagesFiles = new List<string>();
            string imageStream;
            var drawImagesFilter = this.BuildDrawImagesFilter("", 0, imagesTimeTable, imagesFiles, out imageStream);

            string textOutputStream;
            var drawTextFilter = this.BuildDrawTextFilter(imageStream, textTimeRecords, pathToFonts, fontSize, out textOutputStream);

            var semicolone = !string.IsNullOrEmpty(drawImagesFilter) && !string.IsNullOrEmpty(drawTextFilter)
                ? ";"
                : "";
            
            var scriptFile = this.GetIntermediateFile(".txt");
            File.WriteAllText(scriptFile, drawImagesFilter + semicolone + drawTextFilter, Encoding.Default);
            var imageFiles = imagesFiles.Aggregate("", (t, c) => $"{t} -i \"{c}\"");

            this.parametersAccumulator.AppendFormat(" {0} -filter_complex_script:v \"{1}\" -map {2} -map 0:a", imageFiles, scriptFile, textOutputStream);
            return this;
        }

        public FFMpegCommandBuilder DrawImages(IList<DrawImageTimeRecord> imagesTimeTable)
        {
            var imagesFiles = new List<string>();
            string outImagesStream;
            var totalFilterScript = this.BuildDrawImagesFilter("", 0, imagesTimeTable, imagesFiles, out outImagesStream);

            var scriptFile = this.GetIntermediateFile(".txt");
            File.WriteAllText(scriptFile, totalFilterScript, Encoding.Default);
            var imageFiles = imagesFiles.Aggregate("", (t, c) => $"{t} -i \"{c}\"");

            this.parametersAccumulator.AppendFormat(" {0} -filter_complex_script:v \"{1}\" -map {2}", imageFiles, scriptFile, outImagesStream);
            return this;
        }

        public FFMpegCommandBuilder DrawText(IList<TextTimeRecord> textTimeRecords, string pathToFonts, int fontSize)
        {
            string outTextStream; 
            var drawTextParameter = this.BuildDrawTextFilter("[in]", textTimeRecords, pathToFonts, fontSize, out outTextStream);
            var scriptFile = this.GetIntermediateFile(".txt");
            File.WriteAllText(scriptFile, drawTextParameter, Encoding.Default);

            this.parametersAccumulator.AppendFormat(" -filter_script:v \"{0}\" -map {1}", scriptFile, outTextStream);
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
            
            var scriptFile = this.GetIntermediateFile(".txt");
            File.WriteAllText(scriptFile, $"{trims} {warps} {merges}concat=n={warpRecords.Length * 2 + 1}:v=1:a=0[out]", Encoding.Default);
            
            this.parametersAccumulator.Append($" -filter_complex_script:v \"{scriptFile}\" -map [out] ");
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

        private string BuildConcatFilter(int count, Size finalScale, out string outVideoStream, out string outAudioStream)
        {
            var setDARfilter = new StringBuilder();
            var concatFilterBuilder = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                if (finalScale.IsEmpty)
                {
                    setDARfilter.Append($"[{i}:v]setsar=16/9[v{i}];");
                }
                else
                {
                    setDARfilter.Append($"[{i}:v]scale={finalScale.Width}x{finalScale.Height},setsar=16/9[v{i}];");
                }
                concatFilterBuilder.Append($"[v{i}][{i}:a]");
            }

            outVideoStream = "[vv]";
            outAudioStream = "[a]";
            setDARfilter.Append(concatFilterBuilder).Append($"concat=n={count}:v=1:a=1{outVideoStream}{outAudioStream}");
            return setDARfilter.ToString();
        }

        private string BuildDrawTextFilter(string inputStream, IList<TextTimeRecord> textTimeRecords, string pathToFonts, int fontSize, out string outputStream)
        {
            if (!textTimeRecords.Any())
            {
                outputStream = inputStream;
                return string.Empty;
            }
            
            var drawTextFitler = inputStream;
            foreach (var textTimeRecord in textTimeRecords)
            {
                var i = 0;
                foreach (var line in textTimeRecord.Lines)
                {
                    drawTextFitler +=
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

            var lastCommaIndex = drawTextFitler.LastIndexOf(',');
            if (lastCommaIndex != -1)
            {
                drawTextFitler = drawTextFitler.Remove(lastCommaIndex);
            }
            outputStream = "[text_out]";
            drawTextFitler += outputStream;
            return drawTextFitler;
        }

        private string BuildDrawImagesFilter(string inputStream, int inputImagesStartsAt, IList<DrawImageTimeRecord> imagesTimeTable, List<string> imagesFiles, out string outputStream)
        {
            var totalFilterScript = "";
            if (!imagesTimeTable.Any())
            {
                outputStream = inputStream;
                return totalFilterScript;
            }

            const string OverlayTemplate = "{0}{1}overlay={2}:{3}:enable='between(t,{4},{5})'{6}";
            var previousStream = inputStream;
            var id = inputImagesStartsAt;
            foreach (var timeRecord in imagesTimeTable)
            {
                var imageFile = this.GetIntermediateFile(".png");
                imagesFiles.Add(imageFile);
                File.WriteAllBytes(imageFile, timeRecord.ImageData);
                totalFilterScript += string.Format(CultureInfo.InvariantCulture,
                    OverlayTemplate,
                    previousStream,
                    $"[{id}:v]",
                    timeRecord.LeftOffset,
                    timeRecord.TopOffset,
                    timeRecord.ImageStartSecond,
                    timeRecord.ImageEndSecond,
                    $" [tmp{id}];");
                previousStream = $"[tmp{id}]";
                id++;
            }

            outputStream = previousStream;
            totalFilterScript = totalFilterScript.Remove(totalFilterScript.LastIndexOf(';'));

            return totalFilterScript;
        }

        private string GetFileWithInputs(IEnumerable<string> inputFiles, IEnumerable<string> inputImages)
        {
            var fileWithInputs = this.GetIntermediateFile(".txt");
            using (var sw = new StreamWriter(fileWithInputs))
            {
                foreach (var inputFile in inputFiles)
                {
                    sw.WriteLine("file '{0}'", inputFile);
                }
                foreach (var inputImageFile in inputImages)
                {
                    sw.WriteLine("file '{0}'", inputImageFile);
                }
                sw.Flush();
            }

            return fileWithInputs;
        }
    }
}