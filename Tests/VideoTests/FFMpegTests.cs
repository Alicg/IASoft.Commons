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
        public void Concat3Videos_Test()
        {
            const string OutputFile = OutputFolder + "3Episodes60SecConcat_SuperFast.mp4";
            const string FileToConcat1 = OutputFolder + "1EpisodeToConcat_SuperFast.mp4";
            const string FileToConcat2 = OutputFolder + "2EpisodeToConcat_SuperFast.mp4";
            const string FileToConcat3 = OutputFolder + "3EpisodeToConcat_SuperFast.mp4";

            const string Source = @"M:\SVA.Videos\POLAND VS QATAR SEMI-FINAL 24th Men's Handball World Championship Qatar 2015.mp4";
            
            var ffmpeg = new FFMpeg(this.temporaryFilesStorage);
            var cutOptions1 = FFMpegCutOptions.BuildSimpleCatOptions(Source, FileToConcat1, 100, 20, GlobalExportProgress.Empty);
            ffmpeg.Cut(cutOptions1);
            var cutOptions2 = FFMpegCutOptions.BuildSimpleCatOptions(Source, FileToConcat2, 300, 20, GlobalExportProgress.Empty);
            ffmpeg.Cut(cutOptions2);
            var cutOptions3 = FFMpegCutOptions.BuildSimpleCatOptions(Source, FileToConcat3, 600, 20, GlobalExportProgress.Empty);
            ffmpeg.Cut(cutOptions3);

            ffmpeg.Concat(OutputFile, GlobalExportProgress.Empty, FileToConcat1, FileToConcat2, FileToConcat3);

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

            const string Source1 = @"M:\SVA.Videos\HDV_1626.mp4";
            const string Source2 = @"M:\SVA.Videos\LRF -Basement- taurė 2017 Klaipėdos -Dragūnas- - HC Vilnius.mp4";
            
            var ffmpeg = new FFMpeg(this.temporaryFilesStorage);
            var cutOptions1 = FFMpegCutOptions.BuildCatOptionsWithConvertations(Source1, FileToConcat1, 300, 20, GlobalExportProgress.Empty, new Size(1280, 720));
            ffmpeg.Cut(cutOptions1);
            var cutOptions2 = FFMpegCutOptions.BuildCatOptionsWithConvertations(Source1, FileToConcat2, 500, 20, GlobalExportProgress.Empty, new Size(1280, 720));
            ffmpeg.Cut(cutOptions2);
            var cutOptions3 = FFMpegCutOptions.BuildCatOptionsWithConvertations(Source2, FileToConcat3, 100, 20, GlobalExportProgress.Empty, new Size(1280, 720));
            ffmpeg.Cut(cutOptions3);
            var cutOptions4 = FFMpegCutOptions.BuildCatOptionsWithConvertations(Source2, FileToConcat4, 300, 20, GlobalExportProgress.Empty, new Size(1280, 720));
            ffmpeg.Cut(cutOptions4);

            ffmpeg.Concat(OutputFile, GlobalExportProgress.Empty, FileToConcat3, FileToConcat4, FileToConcat1, FileToConcat2);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [Test]
        public void WhatIsFaster_Cut3EpisodesWithConversion_ThenConcat_OrCut3EpisodesWithNoConversion_ThenConcat_ThenConvert_Test()
        {
            const string OutputFile = OutputFolder + "3EpisodesWithConversion_ThenConcat.mkv";
            const string FileToConcat1 = OutputFolder + "1EpisodeToConcat.mkv";
            const string FileToConcat2 = OutputFolder + "2EpisodeToConcat.mkv";
            const string FileToConcat3 = OutputFolder + "3EpisodeToConcat.mkv";

            const string Source2 = @"M:\SVA.Videos\LRF -Basement- taurė 2017 Klaipėdos -Dragūnas- - HC Vilnius.mp4";

            var sw = Stopwatch.StartNew();
            
            var ffmpeg = new FFMpeg(this.temporaryFilesStorage);
            var cutOptions1 = FFMpegCutOptions.BuildCatOptionsWithConvertations(Source2, FileToConcat1, 4018, 15, GlobalExportProgress.Empty, new Size(704, 400));
            ffmpeg.Cut(cutOptions1);
            var cutOptions2 = FFMpegCutOptions.BuildCatOptionsWithConvertations(Source2, FileToConcat2, 3318, 15, GlobalExportProgress.Empty, new Size(704, 400));
            ffmpeg.Cut(cutOptions2);
            var cutOptions3 = FFMpegCutOptions.BuildCatOptionsWithConvertations(Source2, FileToConcat3, 4318, 15, GlobalExportProgress.Empty, new Size(704, 400));
            ffmpeg.Cut(cutOptions3);
            ffmpeg.Concat(OutputFile, GlobalExportProgress.Empty, FileToConcat1, FileToConcat2, FileToConcat3);
            
            sw.Stop();

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

        [TearDown]
        public void CleanUp()
        {
            Directory.Delete(OutputFolder, true);
            this.temporaryFilesStorage.Dispose();
        }
    }
}