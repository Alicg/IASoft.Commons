using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FFMpegWrapper
{
    public class FFMpegCommandBuilder
    {
        private readonly StringBuilder parametersAccumulator = new StringBuilder();

        private static readonly IDictionary<PresetParameters, string> PresetsDictionary = new Dictionary<PresetParameters, string>
        {
            {PresetParameters.SuperFast, "superfast"},
            {PresetParameters.Medium, "medium"},
            {PresetParameters.Slower, "slower"}
        };

        private readonly IList<string> temporaryFiles = new List<string>();

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

            this.parametersAccumulator.AppendFormat(" -f concat -safe 0 -i \"{0}\" ", intermediateFile);
            return this;
        }

        public FFMpegCommandBuilder InputFrom(string inputPath)
        {
            this.parametersAccumulator.AppendFormat(" -i \"{0}\" ", inputPath);
            return this;
        }

        public FFMpegCommandBuilder DrawImages(IList<DrawImageTimeRecord> imagesTimeTable)
        {
            const string OverlayTamplate = "{5}overlay={0}:{1}:enable='between(t,{2},{3})'{4}";
            var imagesFiles = new List<string>();
            var totalFilterScript = "";
            var id = 0;
            foreach (var timeRecord in imagesTimeTable)
            {
                var imageFile = this.GetIntermediateFile(".png");
                imagesFiles.Add(imageFile);
                File.WriteAllBytes(imageFile, timeRecord.ImageData);
                totalFilterScript += string.Format(CultureInfo.InvariantCulture,
                    OverlayTamplate,
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

            this.parametersAccumulator.AppendFormat(" {0} -filter_complex_script:v {1}", imageFiles, scriptFile);
            return this;
        }

        public FFMpegCommandBuilder DrawText(string[] textLines, string pathToFonts, int fontSize)
        {
            var drawTextParameter = "[in]";
            var i = 0;
            foreach (var line in textLines)
            {
                drawTextParameter +=
                    $"drawtext=fontfile={pathToFonts}"+
                    $":text='{line}'"+
                    $":fontsize={fontSize}"+
                    ":fontcolor=red"+
                    ":x=(main_w/2-text_w/2)"+
                    $":y=main_h-(text_h*({textLines.Length} -{i})) - 15,";
                i++;
            }
            drawTextParameter = drawTextParameter.Remove(drawTextParameter.LastIndexOf(','));
            var scriptFile = this.GetIntermediateFile(".txt");
            File.WriteAllText(scriptFile, drawTextParameter, Encoding.Default);

            this.parametersAccumulator.AppendFormat(" -filter_script:v \"{0}\"", scriptFile);
            return this;
        }

        public FFMpegCommandBuilder OutputVideoCodec(string outputCodec)
        {
            this.parametersAccumulator.AppendFormat(" -c:v {0} ", outputCodec);
            return this;
        }

        public FFMpegCommandBuilder OutputPreset(PresetParameters presetParameter)
        {
            var preset = PresetsDictionary.ContainsKey(presetParameter) ? PresetsDictionary[presetParameter] : PresetsDictionary[PresetParameters.Medium];
            this.parametersAccumulator.AppendFormat(" -preset {0}", preset);
            return this;
        }

        public FFMpegCommandBuilder OutputTune(string tune)
        {
            this.parametersAccumulator.AppendFormat(" -tune {0}", tune);
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

        public FFMpegCommand BuildCommand(string pathToFfMpegExe)
        {
            return new FFMpegCommand(pathToFfMpegExe, this.parametersAccumulator.ToString(), this.temporaryFiles);
        }

        private string GetIntermediateFile(string ext)
        {
            var intermediateFile = Path.Combine(Directory.GetCurrentDirectory(), $"{Guid.NewGuid()}{ext}");
            this.temporaryFiles.Add(intermediateFile);
            return intermediateFile;
        }
    }
}