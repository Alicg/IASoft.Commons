using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using FFMpegWrapper;
using NUnit.Framework;

namespace VideoTests
{
    [TestFixture]
    public class FFMpegCommandBuilderTests
    {
        [Test]
        public void DrawTextTest()
        {
            var builder = new FFMpegCommandBuilder(new TemporaryFilesStorage());
            var drawTextParams = builder.DrawText(new List<TextTimeRecord> {new TextTimeRecord("Text1", 1, 3)}, "PathToFonts", 10).ParametersAccumulator.ToString();
            var pattern = @"-filter_script:v ""(?<textFilterFile>.*?)""";
            var textFilterFile = Regex.Match(drawTextParams, pattern).Groups["textFilterFile"].Value;
            var textFilter = File.ReadAllText(textFilterFile);
            Assert.AreEqual(
                "[in]drawtext=enable='between(t,1,3)':fontfile=PathToFonts:text='Text1':fontsize=10:fontcolor=red:x=(main_w/2-text_w/2):y=main_h-(text_h*(1 -0)) - 15[text_out]",
                textFilter);
        }

        [Test]
        public void DrawImageTest()
        {
            var builder = new FFMpegCommandBuilder(new TemporaryFilesStorage());
            var drawImageParams =
                builder.DrawImages(new List<DrawImageTimeRecord> {new DrawImageTimeRecord(new byte[0], 0, 0, 0, 4)}).ParametersAccumulator.ToString();
            var pattern = @"-filter_complex_script:v ""(?<imageFilterFile>.*?)""";
            var imageFilterFile = Regex.Match(drawImageParams, pattern).Groups["imageFilterFile"].Value;
            var imageFilter = File.ReadAllText(imageFilterFile);
            Assert.AreEqual("[0:v]overlay=0:0:enable='between(t,0,4)' [tmp0]", imageFilter);
        }

        [Test]
        public void ConcatAndDrawTextAndDrawImagesTest()
        {
            var builder = new FFMpegCommandBuilder(new TemporaryFilesStorage());
            var complexParams = builder
                .ConcatDrawImagesAndText(
                    new List<string> {"input1.avi", "input2.avi", "input3.avi"},
                    new List<DrawImageTimeRecord> {new DrawImageTimeRecord(new byte[0], 0, 0, 0, 4)},
                    new List<TextTimeRecord> {new TextTimeRecord("Text1", 1, 3)},
                    new Size(1000, 500),
                    "PathToFonts",
                    10).ParametersAccumulator.ToString();
            var pattern = @"-filter_complex_script:v ""(?<complexFilterFile>.*?)""";
            var filterFile = Regex.Match(complexParams, pattern).Groups["complexFilterFile"].Value;
            var filter = File.ReadAllText(filterFile);
            Assert.AreEqual(
                "[0:v]scale=1000x500,setdar=16/9[v0];[1:v]scale=1000x500,setdar=16/9[v1];[2:v]scale=1000x500,setdar=16/9[v2];[v0][0:a][v1][1:a][v2][2:a]concat=n=3:v=1:a=1[vv][a];[vv][3:v]overlay=0:0:enable='between(t,0,4)' [tmp3];[tmp3]drawtext=enable='between(t,1,3)':fontfile=PathToFonts:text='Text1':fontsize=10:fontcolor=red:x=(main_w/2-text_w/2):y=main_h-(text_h*(1 -0)) - 15[text_out]",
                filter);
        }
    }
}