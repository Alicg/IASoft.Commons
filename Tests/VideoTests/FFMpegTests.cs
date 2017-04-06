using System.Collections.Generic;
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
            const string OutputFile = OutputFolder + "Simple20SecCut_Medium.avi";
            var ffmpeg = new FFMpeg();
            ffmpeg.Cut(100, 20, @"M:\SVA.Videos\LRF -Basement- taurė 2017 Klaipėdos -Dragūnas- - HC Vilnius.mp4", OutputFile, GlobalExportProgress.Empty);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [Test]
        public void Concat3Videos_Test()
        {
            const string OutputFile = OutputFolder + "3Episodes60SecConcat_SuperFast.avi";
            const string FileToConcat1 = OutputFolder + "1EpisodeToConcat_SuperFast.avi";
            const string FileToConcat2 = OutputFolder + "2EpisodeToConcat_SuperFast.avi";
            const string FileToConcat3 = OutputFolder + "3EpisodeToConcat_SuperFast.avi";
            var ffmpeg = new FFMpeg(PresetParameters.SuperFast);
            ffmpeg.Cut(100, 20, @"M:\POLAND VS QATAR SEMI-FINAL 24th Men's Handball World Championship Qatar 2015.mp4", FileToConcat1, GlobalExportProgress.Empty);
            ffmpeg.Cut(300, 20, @"M:\POLAND VS QATAR SEMI-FINAL 24th Men's Handball World Championship Qatar 2015.mp4", FileToConcat2, GlobalExportProgress.Empty);
            ffmpeg.Cut(600, 20, @"M:\POLAND VS QATAR SEMI-FINAL 24th Men's Handball World Championship Qatar 2015.mp4", FileToConcat3, GlobalExportProgress.Empty);

            ffmpeg.Concat(OutputFile, GlobalExportProgress.Empty, FileToConcat1, FileToConcat2, FileToConcat3);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [Test]
        public void Concat4ProblemVideosFromArturas_Test()
        {
            const string OutputFile = OutputFolder + "4Episodes60SecConcat_SuperFast.mpg";
            const string FileToConcat1 = OutputFolder + "1EpisodeToConcat_SuperFast.mpg";
            const string FileToConcat2 = OutputFolder + "2EpisodeToConcat_SuperFast.mpg";
            const string FileToConcat3 = OutputFolder + "3EpisodeToConcat_SuperFast.mpg";
            const string FileToConcat4 = OutputFolder + "4EpisodeToConcat_SuperFast.mpg";
            var ffmpeg = new FFMpeg(PresetParameters.SuperFast);
            //ffmpeg.Cut(100, 20, @"M:\SVA.Videos\LRF -Basement- taurė 2017 Klaipėdos -Dragūnas- - HC Vilnius.mp4", FileToConcat1, GlobalExportProgress.Empty);
            ffmpeg.Cut(300, 20, @"M:\SVA.Videos\HDV_1626.mp4", FileToConcat1, GlobalExportProgress.Empty);
            ffmpeg.Cut(500, 20, @"M:\SVA.Videos\HDV_1626.mp4", FileToConcat2, GlobalExportProgress.Empty);
            //ffmpeg.Cut(680, 20, @"M:\SVA.Videos\HDV_1626.mp4", FileToConcat4, GlobalExportProgress.Empty);
            ffmpeg.Cut(100, 20, @"M:\SVA.Videos\LRF -Basement- taurė 2017 Klaipėdos -Dragūnas- - HC Vilnius.mp4", FileToConcat3, GlobalExportProgress.Empty);
            ffmpeg.Cut(300, 20, @"M:\SVA.Videos\LRF -Basement- taurė 2017 Klaipėdos -Dragūnas- - HC Vilnius.mp4", FileToConcat4, GlobalExportProgress.Empty);

            ffmpeg.Concat(OutputFile, GlobalExportProgress.Empty, FileToConcat3, FileToConcat4, FileToConcat1, FileToConcat2);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [Test]
        public void DrawImageOnOneVideo()
        {
            const string OutputFile = OutputFolder + "Image20SecCut_Medium.avi";
            var ffmpeg = new FFMpeg();

            var images = new List<DrawImageTimeRecord>();
            images.Add(new DrawImageTimeRecord(File.ReadAllBytes(SampleFiles.SamplePngImage), 100, 100, 1, 4));

            ffmpeg.DrawImage(SampleFiles.SampleVideo_5sec, images, OutputFile, GlobalExportProgress.Empty);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [TearDown]
        public void CleanUp()
        {
            Directory.Delete(OutputFolder, true);
        }
    }
}