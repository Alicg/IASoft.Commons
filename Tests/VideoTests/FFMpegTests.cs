using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using FFMpegWrapper;
using NUnit.Framework;
using Video.Utils;

namespace VideoTests
{
    [TestFixture(Description = "Тесты самой обертки над FFMPeg.")]
    public class FFMpegTests
    {
        private const string OutputFolder = "FFMpegWrapperFolder//";

        private readonly string InputFolder = Path.Combine(Environment.GetEnvironmentVariable("SportFolder"), "Handball videos");

        private TemporaryFilesStorage temporaryFilesStorage = new TemporaryFilesStorage();

        [SetUp]
        public void Init()
        {
            new VideoUtils().EnableDebugMode();
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }
        }

        [Test]
        public void SimpleCut_Test()
        {
            const string OutputFile = OutputFolder + "Simple20SecCut_Medium.mp4";
            var ffmpeg = new FFMpeg(this.temporaryFilesStorage);
            var cutOptions = FFMpegCutOptions.BuildSimpleCatOptions(
                @"M:\SVA.Videos\LRF -Basement- taurė 2017 Klaipėdos -Dragūnas- - HC Vilnius.mp4",
                OutputFile,
                100,
                20,
                GlobalExportProgress.Empty);
            ffmpeg.Cut(cutOptions);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [Test]
        public void Cut_Effect_Concat3Videos_Test()
        {
            const string OutputFile = OutputFolder + "3Episodes60SecConcat_tmp.avi";
            const string FileToConcat1 = OutputFolder + "1EpisodeToConcat_tmp.avi";
            const string FileToConcat2 = OutputFolder + "2EpisodeToConcat_tmp.avi";
            const string FileToConcat3 = OutputFolder + "3EpisodeToConcat_tmp.avi";

            string source = Path.Combine(this.InputFolder, "Гандбол. ЛЧ. 2014-2015. 6-й тур. Багга. 25.11.2014.avi");

            var ffmpeg = new FFMpeg(this.temporaryFilesStorage);
            var cutOptions1 = FFMpegCutOptions.BuildCatOptionsWithConvertations(source,
                FileToConcat1,
                900,
                20,
                GlobalExportProgress.Empty,
                Size.Empty,
                null,
                new List<DrawImageTimeRecord>(),
                new List<TimeWarpRecord>());
            ffmpeg.Cut(cutOptions1);
            var cutOptions2 = FFMpegCutOptions.BuildCatOptionsWithConvertations(source,
                FileToConcat2,
                300,
                20,
                GlobalExportProgress.Empty,
                Size.Empty,
                null,
                new List<DrawImageTimeRecord>(),
                new List<TimeWarpRecord>
                {
                    new TimeWarpRecord(3, 12, 2)
                });
            ffmpeg.Cut(cutOptions2);
            var cutOptions3 = FFMpegCutOptions.BuildCatOptionsWithConvertations(source,
                FileToConcat3,
                600,
                20,
                GlobalExportProgress.Empty,
                Size.Empty,
                null,
                new List<DrawImageTimeRecord>(),
                new List<TimeWarpRecord>
                {
                    new TimeWarpRecord(3, 12, 2)
                });
            ffmpeg.Cut(cutOptions3);

            ffmpeg.Concat(OutputFile, "copy", "copy", GlobalExportProgress.Empty, FileToConcat2, FileToConcat1, FileToConcat3);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [Test]
        public void Cut_Concat_Effect3Videos_Test()
        {
            const string OutputFile = OutputFolder + "Cut_Effect_Concat3Videos.avi";
            const string FileToConcat1 = OutputFolder + "1EpisodeToConcat_tmp.avi";
            const string FileToConcat2 = OutputFolder + "2EpisodeToConcat_tmp.avi";
            const string FileToConcat2TW = OutputFolder + "2EpisodeToConcat_tmpTW.avi";
            const string FileToConcat3 = OutputFolder + "3EpisodeToConcat_tmp.avi";

            string source = Path.Combine(this.InputFolder, "Гандбол. Ч.Е. Финал. Дания-Франция. Багга. 26.01.2014.avi");

            var ffmpeg = new FFMpeg(this.temporaryFilesStorage);
            var cutOptions1 = FFMpegCutOptions.BuildSimpleCatOptions(source, FileToConcat1, 100, 20, GlobalExportProgress.Empty);
            ffmpeg.Cut(cutOptions1);
            var cutOptions2 = FFMpegCutOptions.BuildSimpleCatOptions(source, FileToConcat2, 300, 20, GlobalExportProgress.Empty);
            ffmpeg.Cut(cutOptions2);
            var cutOptions3 = FFMpegCutOptions.BuildSimpleCatOptions(source, FileToConcat3, 600, 20, GlobalExportProgress.Empty);
            ffmpeg.Cut(cutOptions3);

            ffmpeg.Concat(OutputFile, "copy", "copy", GlobalExportProgress.Empty, FileToConcat1, FileToConcat2, FileToConcat3);
            ffmpeg.ApplyTimeWarp(OutputFile, new List<TimeWarpRecord> { new TimeWarpRecord(23, 32, 2), new TimeWarpRecord(43, 52, 2) }, FileToConcat2TW, GlobalExportProgress.Empty);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [Test]
        public void Concat4ProblemVideosFromArturas_Test()
        {
            const string OutputFile = OutputFolder + "4Episodes60SecConcat_SuperFast.mkv";
            const string FileToConcat1 = OutputFolder + "1EpisodeToConcat_SuperFast.mp4";
            const string FileToConcat2 = OutputFolder + "2EpisodeToConcat_SuperFast.mp4";
            const string FileToConcat3 = OutputFolder + "3EpisodeToConcat_SuperFast.mp4";
            const string FileToConcat4 = OutputFolder + "4EpisodeToConcat_SuperFast.mp4";

            string Source1 = SampleFiles.RealInputVideoAVI;
            string Source2 = SampleFiles.RealInputVideoAVI2;
            
            var ffmpeg = new FFMpeg(this.temporaryFilesStorage);
            var cutOptions1 = FFMpegCutOptions.BuildCatOptionsWithConvertations(Source1, FileToConcat1, 300, 20, GlobalExportProgress.Empty, new Size(1280, 720));
            ffmpeg.Cut(cutOptions1);
            var cutOptions2 = FFMpegCutOptions.BuildCatOptionsWithConvertations(Source1, FileToConcat2, 500, 20, GlobalExportProgress.Empty, new Size(1280, 720));
            ffmpeg.Cut(cutOptions2);
            var cutOptions3 = FFMpegCutOptions.BuildCatOptionsWithConvertations(Source2, FileToConcat3, 100, 20, GlobalExportProgress.Empty, new Size(1280, 720));
            ffmpeg.Cut(cutOptions3);
            var cutOptions4 = FFMpegCutOptions.BuildCatOptionsWithConvertations(Source2, FileToConcat4, 300, 20, GlobalExportProgress.Empty, new Size(1280, 720));
            ffmpeg.Cut(cutOptions4);

            ffmpeg.Concat(OutputFile, "copy", "copy", GlobalExportProgress.Empty, FileToConcat3, FileToConcat4, FileToConcat1, FileToConcat2);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [Test]
        public void DrawImageOnOneVideo()
        {
            const string OutputFile = OutputFolder + "Image20SecCut_Medium.mp4";
            var ffmpeg = new FFMpeg(this.temporaryFilesStorage);

            var images = new List<DrawImageTimeRecord>();
            images.Add(new DrawImageTimeRecord(File.ReadAllBytes(SampleFiles.SamplePngImage), 100, 100, 1, 4));

            ffmpeg.DrawImage(SampleFiles.SampleVideo_5sec, images, OutputFile, GlobalExportProgress.Empty);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [Test]
        public void DrawImagesAndTextsOnOneVideo_Test()
        {
            const string OutputFile = OutputFolder + "DrawImagesAndTextsOnOneVideo.mp4";
            var ffmpeg = new FFMpeg(this.temporaryFilesStorage);

            var images = new List<DrawImageTimeRecord>();
            images.Add(new DrawImageTimeRecord(File.ReadAllBytes(SampleFiles.SamplePngImage), 100, 100, 1, 4));
            images.Add(new DrawImageTimeRecord(File.ReadAllBytes(SampleFiles.SamplePngImage), 100, 100, 8, 9));

            var text = new List<TextTimeRecord>();
            text.Add(new TextTimeRecord("First", 0, 7));
            text.Add(new TextTimeRecord("Second", 9, 14));
            text.Add(new TextTimeRecord("Third", 29, 34));

            ffmpeg.DrawImagesAndText(SampleFiles.Helicopter_1min_48sec, images, text, OutputFile, GlobalExportProgress.Empty);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [TearDown]
        public void CleanUp()
        {
            Directory.Delete(OutputFolder, true);
            this.temporaryFilesStorage.Dispose();
        }
    }
}