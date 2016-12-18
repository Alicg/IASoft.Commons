using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using FFMpegWrapper;

public class MediaHandler
{
    public static bool DebugModeEnabled = false;
    public static string LogFile = "ffmpegwrapper.log";
    private readonly string fontsPath;
    private int _disable_license_validation = 1;

	private DateTime _expirydate = DateTime.Parse("07/07/2009");

	private DateTime _lowerexpirydate = DateTime.Parse("06/06/2009");

	private int _exitprocess = 30000;

	private string _servicepath = "";

	private string _ffmpegpath = "";

	private string _flvtoolpath = "";

	private string _inputpath = "";

	private string _outputpath = "";

	private string _presetpath = "";

	private string _mp4boxpath = "";

	private string _filename = "";

	private string _outputfilename = "";

	private int _width;

	private int _height;

	private double _video_bitrate;

	private double _audio_bitrate;

	private int _audio_samplingrate;

	private double _framerate;

	private string _duration = "";

	private bool _deinterlace;

	private bool _maxquality;

	private string _vcodec = "";

	private string _acodec = "";

	private bool _disableaudio;

	private bool _disablevideo;

	private int _channels;

	private string _aspectratio = "";

	private string _targetfiletype = "";

	private string _mp4videotype = "normal";

	private string _customcommand = "";

	private string _force = "";

	private string _startpoint = "";

	private string _extension = "";

	private int _gopsize;

	private int _bitrate_tolerance = 4000;

	private int _limit_size;

	private int _maxrate;

	private int _minrate;

	private int _buffersize;

	private int _pass;

	private int _quality_scale;

	private string[] _filenames;

	private int _thread;

	private string _imagename = ""; 

	private string _frametime = "";

	private string _image_format = "jpg";

	private int _no_of_thumbs;

	private double _frame_time = 1;

	private int _frame_start_position;

	private bool _grab_multiple_thumbs;

	private bool _auto_transition;

	private int _thumb_mode;

	private string _watermarkpath = "";

	private string _watermarkimage = "";

	private int _padtop;

	private int _padbottom;

	private int _padright;

	private int _padleft;

	private string _padcolor = "";

	private int _croptop;

	private int _cropbottom;

	private int _cropright;

	private int _cropleft;

	private string _params = "";

	private int _encoding_option;

	private StringBuilder _processinglog = new StringBuilder();

	private string _processlog = "";

	private bool _backgroundprocessing;

	private string _title_expression = "(\\s+|)title(\\s+|):(\\s+|)(?<vtitle>.+)";

	private string _comment_expression = "(\\s+|)comment(\\s+|):(\\s+|)Footage(\\s+|):(\\s+|)(?<comment_footage>.+?\\|)(\\s+|)Producer(\\s+|):(\\s+|)(?<comment_producer>.+?\\|)(\\s+|)Music(\\s+|):(\\s+|)(?<comment_music>.+)";

	private string minfo_pattern = "Duration: (?<hours>\\d{1,3}):(?<minutes>\\d{2}):(?<seconds>\\d{2})(.(?<fractions>\\d{1,3}))?, start: (?<start>\\d+(\\.\\d+)?), bitrate: (?<bitrate>\\d+(\\.\\d+)?(\\s+)kb/s)?";

	private string strm_pattern = "Stream(\\s|)#(?<number>\\d+?\\.\\d+?)(\\((?<language>\\w+)\\))?(\\s|):(\\s|)(?<type>\\w+)(\\s|):(\\s|)(?<data>.+)";

	private string frame_info_pattern = "frame=(\\s+|)(?<frame>\\d+)?(\\s+|)fps=(\\s+|)(?<fps>\\d+)?(\\s+|)q=(?<q>\\d+.\\d+)?(\\s+|)size=(\\s+|)(?<fsize>\\d+)?kB(\\s+|)time=(?<time_hr>\\d+)?:(?<time_min>\\d+)?:(?<time_sec>\\d+)?.(?<time_frac>\\d+)?\\sbitrate=(\\s+|)(?<bitrate>\\d+.\\d+)kbits/s";

	public VideoInfo vinfo = new VideoInfo();

	private Process process;

	private int elapsedTime;

	private bool eventHandled;

	private bool _processstarted;

	public string ACodec
	{
		get
		{
			return this._acodec;
		}
		set
		{
			this._acodec = value;
		}
	}

	public string AspectRatio
	{
		get
		{
			return this._aspectratio;
		}
		set
		{
			this._aspectratio = value;
		}
	}

	public double Audio_Bitrate
	{
		get
		{
			return this._audio_bitrate;
		}
		set
		{
			this._audio_bitrate = value;
		}
	}

	public int Audio_SamplingRate
	{
		get
		{
			return this._audio_samplingrate;
		}
		set
		{
			this._audio_samplingrate = value;
		}
	}

	public bool Auto_Transition_Time
	{
		get
		{
			return this._auto_transition;
		}
		set
		{
			this._auto_transition = value;
		}
	}

	public bool BackgroundProcessing
	{
		get
		{
			return this._backgroundprocessing;
		}
		set
		{
			this._backgroundprocessing = value;
		}
	}

	public int Bitrate_Tolerance
	{
		get
		{
			return this._bitrate_tolerance;
		}
		set
		{
			this._bitrate_tolerance = value;
		}
	}

	public int BufferSize
	{
		get
		{
			return this._buffersize;
		}
		set
		{
			this._buffersize = value;
		}
	}

	public int Channel
	{
		get
		{
			return this._channels;
		}
		set
		{
			this._channels = value;
		}
	}

	public int CropBottom
	{
		get
		{
			return this._cropbottom;
		}
		set
		{
			this._cropbottom = value;
		}
	}

	public int CropLeft
	{
		get
		{
			return this._cropleft;
		}
		set
		{
			this._cropleft = value;
		}
	}

	public int CropRight
	{
		get
		{
			return this._cropright;
		}
		set
		{
			this._cropright = value;
		}
	}

	public int CropTop
	{
		get
		{
			return this._croptop;
		}
		set
		{
			this._croptop = value;
		}
	}

	private string CustomCommand
	{
		get
		{
			return this._customcommand;
		}
		set
		{
			this._customcommand = value;
		}
	}

	public bool Deinterlace
	{
		get
		{
			return this._deinterlace;
		}
		set
		{
			this._deinterlace = value;
		}
	}

	public bool DisableAudio
	{
		get
		{
			return this._disableaudio;
		}
		set
		{
			this._disableaudio = value;
		}
	}

	public bool DisableVideo
	{
		get
		{
			return this._disablevideo;
		}
		set
		{
			this._disablevideo = value;
		}
	}

	private string Duration
	{
		get
		{
			return this._duration;
		}
		set
		{
			this._duration = value;
		}
	}

	public int EncodingType
	{
		get
		{
			return this._encoding_option;
		}
		set
		{
			this._encoding_option = value;
		}
	}

	public int ExitProcess
	{
		get
		{
			return this._exitprocess;
		}
		set
		{
			this._exitprocess = value;
		}
	}

	public string FFMPEGPath
	{
		get
		{
			return this._ffmpegpath;
		}
		set
		{
			this._ffmpegpath = value;
		}
	}

	public string FileName
	{
		get
		{
			return this._filename;
		}
		set
		{
			this._filename = value;
		}
	}

	public string[] FileNames
	{
		get
		{
			return this._filenames;
		}
		set
		{
			this._filenames = value;
		}
	}

	public string FLVToolPath
	{
		get
		{
			return this._flvtoolpath;
		}
		set
		{
			this._flvtoolpath = value;
		}
	}

	public string Force
	{
		get
		{
			return this._force;
		}
		set
		{
			this._force = value;
		}
	}

	public string Frame_Time
	{
		get
		{
			return this._frametime;
		}
		set
		{
			this._frametime = value;
		}
	}

	public double FrameRate
	{
		get
		{
			return this._framerate;
		}
		set
		{
			this._framerate = value;
		}
	}

	public int Gop_Size
	{
		get
		{
			return this._gopsize;
		}
		set
		{
			this._gopsize = value;
		}
	}

	public int Height
	{
		get
		{
			return this._height;
		}
		set
		{
			this._height = value;
		}
	}

	public string Image_Format
	{
		get
		{
			return this._image_format;
		}
		set
		{
			this._image_format = value;
		}
	}

	public string ImageName
	{
		get
		{
			return this._imagename;
		}
		set
		{
			this._imagename = value;
		}
	}

	public string InputPath
	{
		get
		{
			return this._inputpath;
		}
		set
		{
			this._inputpath = value;
		}
	}

	public int Limit_File_Size
	{
		get
		{
			return this._limit_size;
		}
		set
		{
			this._limit_size = value;
		}
	}

	public bool MaxQuality
	{
		get
		{
			return this._maxquality;
		}
		set
		{
			this._maxquality = value;
		}
	}

	public int MaxRate
	{
		get
		{
			return this._maxrate;
		}
		set
		{
			this._maxrate = value;
		}
	}

	public int MinRate
	{
		get
		{
			return this._minrate;
		}
		set
		{
			this._minrate = value;
		}
	}

	public string MP4BoxPath
	{
		get
		{
			return this._mp4boxpath;
		}
		set
		{
			this._mp4boxpath = value;
		}
	}

	private bool Multiple_Thumbs
	{
		get
		{
			return this._grab_multiple_thumbs;
		}
		set
		{
			this._grab_multiple_thumbs = value;
		}
	}

	public int No_Of_Thumbs
	{
		get
		{
			return this._no_of_thumbs;
		}
		set
		{
			this._no_of_thumbs = value;
		}
	}

	public string OutputExtension
	{
		get
		{
			return this._extension;
		}
		set
		{
			this._extension = value;
		}
	}

	public string OutputFileName
	{
		get
		{
			return this._outputfilename;
		}
		set
		{
			this._outputfilename = value;
		}
	}

	public string OutputPath
	{
		get
		{
			return this._outputpath;
		}
		set
		{
			this._outputpath = value;
		}
	}

	public int PadBottom
	{
		get
		{
			return this._padbottom;
		}
		set
		{
			this._padbottom = value;
		}
	}

	public string PadColor
	{
		get
		{
			return this._padcolor;
		}
		set
		{
			this._padcolor = value;
		}
	}

	public int PadLeft
	{
		get
		{
			return this._padleft;
		}
		set
		{
			this._padleft = value;
		}
	}

	public int PadRight
	{
		get
		{
			return this._padright;
		}
		set
		{
			this._padright = value;
		}
	}

	public int PadTop
	{
		get
		{
			return this._padtop;
		}
		set
		{
			this._padtop = value;
		}
	}

	public string Parameters
	{
		get
		{
			return this._params;
		}
		set
		{
			this._params = value;
		}
	}

	public int Pass
	{
		get
		{
			return this._pass;
		}
		set
		{
			this._pass = value;
		}
	}

	public string PresetPath
	{
		get
		{
			return this._presetpath;
		}
		set
		{
			this._presetpath = value;
		}
	}

	public string ProcessingLog
	{
		get
		{
			return this._processlog;
		}
		set
		{
			this._processlog = value;
		}
	}

	public int Scale_Quality
	{
		get
		{
			return this._quality_scale;
		}
		set
		{
			this._quality_scale = value;
		}
	}

	public string ServicePath
	{
		get
		{
			return this._servicepath;
		}
		set
		{
			this._servicepath = value;
		}
	}

	public string Start_Position
	{
		get
		{
			return this._startpoint;
		}
		set
		{
			this._startpoint = value;
		}
	}

	public string TargetFileType
	{
		get
		{
			return this._targetfiletype;
		}
		set
		{
			this._targetfiletype = value;
		}
	}

	public int Thread
	{
		get
		{
			return this._thread;
		}
		set
		{
			this._thread = value;
		}
	}

	public int Thumb_Start_Position
	{
		get
		{
			return this._frame_start_position;
		}
		set
		{
			this._frame_start_position = value;
		}
	}

	public double Thumb_Transition_Time
	{
		get
		{
			return this._frame_time;
		}
		set
		{
			this._frame_time = value;
		}
	}

	public int ThumbMode
	{
		get
		{
			return this._thumb_mode;
		}
		set
		{
			this._thumb_mode = value;
		}
	}

	public string VCodec
	{
		get
		{
			return this._vcodec;
		}
		set
		{
			this._vcodec = value;
		}
	}

	public double Video_Bitrate
	{
		get
		{
			return this._video_bitrate;
		}
		set
		{
			this._video_bitrate = value;
		}
	}

	public string VideoType
	{
		get
		{
			return this._mp4videotype;
		}
		set
		{
			this._mp4videotype = value;
		}
	}

	public string WaterMarkImage
	{
		get
		{
			return this._watermarkimage;
		}
		set
		{
			this._watermarkimage = value;
		}
	}

	public string WaterMarkPath
	{
		get
		{
			return this._watermarkpath;
		}
		set
		{
			this._watermarkpath = value;
		}
	}

	public int Width
	{
		get
		{
			return this._width;
		}
		set
		{
			this._width = value;
		}
	}

	public MediaHandler(string ffmpegPath)
	{
	    FFMPEGPath = ffmpegPath;
	    ServicePath = ffmpegPath;
    }

    public MediaHandler(string ffmpegPath, string fontsPath)
    {
        this.fontsPath = fontsPath;
        FFMPEGPath = ffmpegPath;
        ServicePath = ffmpegPath;
    }

    public void Concat(string outputFile, params string[] inputFiles)
    {
        var intermediateFile = GetIntermediateFile(".txt");
        using (var sw = new StreamWriter(intermediateFile))
        {
            foreach (var inputFile in inputFiles)
            {
                sw.WriteLine("file '{0}'", inputFile);
            }
            sw.Flush();
        }
        Parameters = string.Format("-f concat -i \"{0}\" " +
                                   "-c:v libx264 -flags +ildct+ilme -preset superfast -tune fastdecode " +
                                   " \"{1}\"", intermediateFile, outputFile);
        ProcessCMD();
        File.Delete(intermediateFile);
    }

    public void Cat(string inputFile, double start, double end, string outputFile)
    {
        //var strStart = new TimeSpan(0, 0, start).ToString();
        //var strEnd = new TimeSpan(0, 0, end).ToString();
        Parameters = string.Format(CultureInfo.InvariantCulture, "-ss {0} -t {1} -i \"{2}\" " +
                                   "-c:v libx264 -flags +ildct+ilme -preset superfast -tune fastdecode " +
                                   " \"{3}\"",
                                   start, end, inputFile, outputFile);
        ProcessCMD();
    }

    public void CatAndDrawText(string inputFile, int start, int end, string outputFile, string overlayText)
    {
        if (overlayText == null)
        {
            Cat(inputFile, start, end, outputFile);
            return;
        }
        var lastDot = inputFile.LastIndexOf('.');
        var inputExt = inputFile.Substring(lastDot);
        var intermediateFile = GetIntermediateFile(inputExt);
        Cat(inputFile, start, end, intermediateFile);
        DrawText(intermediateFile, overlayText, outputFile);
        File.Delete(intermediateFile);
    }

    public void CatAndDrawTextAndDrawImage(string inputFile, double start, double end, string outputFile, string overlayText, List<DrawImageTimeRecord> imagesTimeTable, Action<double> progressAction)
    {
        var imagesExist = imagesTimeTable != null && imagesTimeTable.Any();
        if (overlayText == null && (!imagesExist))
        {
            Cat(inputFile, start, end, outputFile);
            progressAction(1);//100% progress
            return;
        }
        var lastDot = inputFile.LastIndexOf('.');
        var inputExt = inputFile.Substring(lastDot);
        var intermediateFile1 = GetIntermediateFile(inputExt);

        Cat(inputFile, start, end, intermediateFile1);
        progressAction(0.5);//50% progress
        
        if (overlayText != null)
        {
            var intermediateFile2 = imagesExist ? GetIntermediateFile(inputExt) : outputFile;
            DrawText(intermediateFile1, overlayText, intermediateFile2);
            progressAction(0.75);//75% progress
            File.Delete(intermediateFile1);
            intermediateFile1 = intermediateFile2;
        }
        if (imagesExist)
        {
            DrawImage(intermediateFile1, imagesTimeTable, outputFile);
            File.Delete(intermediateFile1);
        }
        progressAction(1);//100% progress
    }

    public void DrawText(string inputFile, string overlayText, string outputFile)
    {
        var fontSize = 30;
        const int charsInLine = 40;
        var lines = GetTranspositionedText(overlayText, charsInLine);
        var drawTextParameter = "[in]";
        int i = 0;
        foreach (var line in lines)
        {
            drawTextParameter +=
                $"drawtext=fontfile={this.fontsPath}:text='{line}':fontsize={fontSize}:fontcolor=red:x=(main_w/2-text_w/2):y=main_h-(text_h*({lines.Count()} -{i})) - 15,";
            i++;
        }
        drawTextParameter = drawTextParameter.Remove(drawTextParameter.LastIndexOf(','));
        var scriptFile = MakeFilterScriptFile(drawTextParameter);
        Parameters =
            string.Format(
                "-i \"{0}\" -filter_script:v \"{1}\" " +
                "-c:v libx264 -flags +ildct+ilme -preset superfast -tune fastdecode \"{2}\"",
                inputFile, scriptFile, outputFile);
        ProcessCMD();
        File.Delete(scriptFile);
    }

    public void DrawImage(string inputFile, List<DrawImageTimeRecord> imagesTimeTable, string outputFile)
    {
        if (!imagesTimeTable.Any())
            return;
        var overlayTamplate = "{5}overlay={0}:{1}:enable='between(t,{2},{3})'{4}";
        var imagesFiles = new List<string>();
        var totalFilterScript = "";
        var id = 0;
        foreach (var timeRecord in imagesTimeTable)
        {
            var imageFile = GetIntermediateFile(".gif");
            imagesFiles.Add(imageFile);
            File.WriteAllBytes(imageFile, timeRecord.ImageData);
            totalFilterScript += string.Format(CultureInfo.InvariantCulture, overlayTamplate, timeRecord.LeftOffset, timeRecord.TopOffset,
                timeRecord.ImageStartSecond, timeRecord.ImageEndSecond, 
                id == imagesTimeTable.Count - 1 ? "" : string.Format(" [tmp{0}];", id),//последний overlay без [tmp]; 
                id == 0 ? "" : string.Format("[tmp{0}] ", (id - 1)));//первый overlay без [tmp]
            id++;
        }
        var scriptFile = MakeFilterScriptFile(totalFilterScript);
        Parameters = string.Format(CultureInfo.InvariantCulture, "-i \"{0}\" {1} " +
                                   "-filter_complex_script:v \"{2}\" -c:v libx264 -flags +ildct+ilme" +
                                   " \"{3}\"", inputFile, imagesFiles.Aggregate("", (t, c) => string.Format("{0} -i \"{1}\"", t, c)), scriptFile, outputFile);
        ProcessCMD();
        foreach (var imageFile in imagesFiles)
        {
            File.Delete(imageFile);
        }
        File.Delete(scriptFile);
    }

    /// <summary>
    /// Создает файл для -filter_script, по скрипту
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    private string MakeFilterScriptFile(string script)
    {
        var intermediateFile1 = GetIntermediateFile(".txt");
        using (var fs = new FileStream(intermediateFile1, FileMode.Create))
        {
            var scriptBytes = Encoding.UTF8.GetBytes(script);
            fs.Write(scriptBytes, 0, scriptBytes.Length);
        }
        return intermediateFile1;
    }

    private string GetIntermediateFile(string ext)
    {
        return string.Format("{0}{1}", Path.GetTempFileName(), ext);
        //return Path.Combine(Directory.GetCurrentDirectory(), String.Format("{0}{1}", Guid.NewGuid(), ext));
    }

    private string[] GetTranspositionedText(string text, int charsInLine)
    {
        var i = 0;
        var prevInd = 0;
        if (text.Length <= charsInLine)
            return new[] {text};
        int lastSpacePosition = 0;
        while (true)
        {
            var spacePosition = text.IndexOf(' ', lastSpacePosition + 1, charsInLine - lastSpacePosition);
            if(spacePosition == -1)
                break;
            lastSpacePosition = spacePosition;
        }
        return
            new[] {text.Substring(0, lastSpacePosition)}.Union(
                GetTranspositionedText(text.Substring(lastSpacePosition + 1, text.Length - lastSpacePosition - 1), charsInLine)).ToArray();
    }

    public FFMpegVideoInfo GetVideoInfo(string inputFile)
    {
        Parameters = string.Format("-i \"{0}\"", inputFile);
        var result = ProcessCMD();
        return ParseVideoInfo(result);
    }

    private FFMpegVideoInfo ParseVideoInfo(string str)
    {
        var regex = new Regex(@"Video:[^~]*?(?<Width>\d{3,5})x(?<Height>\d{3,5})");
        var match = regex.Match(str);
        var width = Convert.ToInt32(match.Groups["Width"].Value);
        var height = Convert.ToInt32(match.Groups["Height"].Value);
        var videoInfo = new FFMpegVideoInfo(width, height);
        return videoInfo;
    }

    /// <summary>
    /// Возвращает битмап с указанной позиции в видео сериализованный в байтмассив
    /// </summary>
    /// <param name="videoFile">исходный видео-файл</param>
    /// <param name="position">позиция в секундах</param>
    /// <param name="imageSize">размер полученной битмапа</param>
    /// <returns></returns>
    public byte[] GetBitmapFromVideoAsByte(string videoFile, double position, FFMpegImageSize imageSize)
    {
        var intermediateFile = GetIntermediateFile(".jpg");
        var quality = imageSize.ToString();
        Parameters = string.Format(CultureInfo.InvariantCulture, "-ss {1} -i \"{0}\" -vframes 1 -s {2} \"{3}\"", videoFile, position, quality, intermediateFile);
        ProcessCMD();
        try
        {
            using (var fs = new FileStream(intermediateFile, FileMode.Open))
            {
                var byteImage = new byte[fs.Length];
                fs.Read(byteImage, 0, (int) fs.Length);
                return byteImage;
            }
        }
        finally
        {
            File.Delete(intermediateFile);
        }
    }

	public void ADVPROC(string _ffmpegpath, string cmd)
	{
		this.elapsedTime = 0;
		this.eventHandled = false;
		try
		{
			this.process = new Process();
			this.process.StartInfo.UseShellExecute = false;
			this.process.StartInfo.RedirectStandardInput = true;
			this.process.StartInfo.RedirectStandardOutput = true;
			this.process.StartInfo.RedirectStandardError = true;
			this.process.StartInfo.CreateNoWindow = true;
			this.process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			this.process.EnableRaisingEvents = true;
			this.process.StartInfo.FileName = _ffmpegpath;
			this.process.StartInfo.Arguments = string.Concat(" ", cmd);
			this.process.ErrorDataReceived += new DataReceivedEventHandler(this.process_ErrorDataReceived);
			this.process.OutputDataReceived += new DataReceivedEventHandler(this.process_OutputDataReceived);
			this.process.Exited += new EventHandler(this.process_Exited);
			this.process.Start();
			this._processinglog.Append("Processing Started<br />.................................<br />");
			this.process.BeginOutputReadLine();
			this.process.BeginErrorReadLine();
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			this._processinglog.Append(string.Concat("Error Occured<br />", exception.Message, "<br />.................................<br />"));
			string message = exception.Message;
			if (this.process != null)
			{
				this.process.Dispose();
			}
		}
		if (!this.BackgroundProcessing)
		{
			while (!this.eventHandled)
			{
				MediaHandler mediaHandler = this;
				mediaHandler.elapsedTime = mediaHandler.elapsedTime + 100;
				if (this.elapsedTime > 30000)
				{
					return;
				}
				System.Threading.Thread.Sleep(100);
			}
		}
	}

	public string Encode_OGG()
	{
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			return "129";
		}
		if (!this.Validate_InputPath())
		{
			return "101";
		}
		string str = string.Concat("\"", this.MP4BoxPath, "\"");
		string[] inputPath = new string[] { "\"", this.InputPath, "\\", this.FileName, "\"" };
		string str1 = string.Concat(inputPath);
		string[] outputPath = new string[] { "\"", this.OutputPath, "\\", this.OutputFileName, "\"" };
		string str2 = string.Concat(outputPath);
		string[] parameters = new string[] { str1, " ", this.Parameters, " -o ", str2 };
		return this.Process_OGG_CMD(str, string.Concat(parameters));
	}

	private string Execute_FFMPEG()
	{
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			return "129";
		}
		if (!this.Validate_FFMPEG())
		{
			return "100";
		}
		if (this.CustomCommand == "")
		{
			return "114";
		}
		string str = string.Concat("\"", this.FFMPEGPath, "\"");
		return this.Process_CMD_Custom(str, this.CustomCommand);
	}

	private VideoInfo FFMPEG_OUTPUT_VALIDATION(string ffmpeg_data)
	{
		VideoInfo videoInfo = new VideoInfo();
		if (this.WaterMarkImage != "" && this.WaterMarkPath != "")
		{
			string str = string.Concat(this.FFMPEGPath.Remove(this._ffmpegpath.LastIndexOf("ffmpeg.exe")), "vhook\\watermark.dll");
			string[] waterMarkPath = new string[] { "\"", this.WaterMarkPath, "\\", this.WaterMarkImage, "\"" };
			string str1 = string.Concat(waterMarkPath);
			if (!File.Exists(str))
			{
				videoInfo.ErrorCode = 136;
				return videoInfo;
			}
			if (str1.Contains(" "))
			{
				videoInfo.ErrorCode = 137;
				return videoInfo;
			}
		}
		if (ffmpeg_data == "")
		{
			videoInfo.ErrorCode = 110;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "Unknown format"))
		{
			videoInfo.ErrorCode = 131;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "no such file or directory"))
		{
			videoInfo.ErrorCode = 105;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "Unsupported codec"))
		{
			videoInfo.ErrorCode = 104;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "Unknown encoder"))
		{
			videoInfo.ErrorCode = 132;
			return videoInfo;
		}
		if (!this.isMatch(ffmpeg_data, "Duration"))
		{
			videoInfo.ErrorCode = 107;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "unrecognized option"))
		{
			videoInfo.ErrorCode = 115;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "Could not open"))
		{
			videoInfo.ErrorCode = 116;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "video codec not compatible"))
		{
			videoInfo.ErrorCode = 118;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "Unknown codec"))
		{
			videoInfo.ErrorCode = 133;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "Video hooking not compiled"))
		{
			videoInfo.ErrorCode = 123;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "incorrect parameters"))
		{
			videoInfo.ErrorCode = 134;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "Failed to add video hook"))
		{
			videoInfo.ErrorCode = 135;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "Unable to parse option value"))
		{
			videoInfo.ErrorCode = 145;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "incorrect codec parameters"))
		{
			videoInfo.ErrorCode = 146;
			return videoInfo;
		}
		if (this.isMatch(ffmpeg_data, "File for preset") && this.isMatch(ffmpeg_data, "not found"))
		{
			videoInfo.ErrorCode = 144;
			return videoInfo;
		}
		if (!this.isMatch(ffmpeg_data, "not within the padded area"))
		{
			return videoInfo;
		}
		videoInfo.ErrorCode = 148;
		return videoInfo;
	}

	private string FixCode(string html)
	{
		html = html.Replace("  ", "&nbsp; ");
		html = html.Replace("  ", " &nbsp;");
		html = html.Replace("\t", "&nbsp;&nbsp;&nbsp;");
		html = html.Replace("<", "&lt;");
		html = html.Replace(">", "&gt;");
		html = html.Replace("\n\n\n\n", "<br/><br/>");
		html = html.Replace("\n\n\n", "<br/><br/>");
		html = html.Replace("\n\n", "<br/><br/>");
		html = html.Replace("\n", "<br/>");
		return html;
	}

	public VideoInfo Get_Info()
	{
		VideoInfo videoInfo;
		VideoInfo message = new VideoInfo();
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			message.ErrorCode = 129;
			return message;
		}
		if (!this.Validate_FFMPEG())
		{
			message.ErrorCode = 100;
			return message;
		}
		if (!this.Validate_InputPath())
		{
			message.ErrorCode = 101;
			return message;
		}
		string str = string.Concat("\"", this.FFMPEGPath, "\"");
		string[] inputPath = new string[] { "\"", this.InputPath, "\\", this.FileName, "\"" };
		string fileName = string.Concat(inputPath);
		if (this.FileName.StartsWith("http"))
		{
			fileName = this.FileName;
		}
		try
		{
			string str1 = string.Concat(" -i ", fileName);
			message = this.PARSE_FFMPEG_OUTPUT(this.Process_CMD(str, str1));
			videoInfo = message;
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			message.ErrorCode = 121;
			message.ErrorMessage = exception.Message;
			videoInfo = message;
		}
		return videoInfo;
	}

	public VideoInfo Grab_Thumb()
	{
		VideoInfo videoInfo = new VideoInfo();
		string str = "";
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			videoInfo.ErrorCode = 129;
			return videoInfo;
		}
		if (!this.Validate_FFMPEG())
		{
			videoInfo.ErrorCode = 100;
			return videoInfo;
		}
		if (!this.Validate_InputPath())
		{
			videoInfo.ErrorCode = 101;
			return videoInfo;
		}
		if (!this.Validate_OutputPath())
		{
			videoInfo.ErrorCode = 102;
			return videoInfo;
		}
		string str1 = string.Concat("\"", this.FFMPEGPath, "\"");
		string[] inputPath = new string[] { "\"", this.InputPath, "\\", this.FileName, "\"" };
		string fileName = string.Concat(inputPath);
		if (this.FileName.StartsWith("http"))
		{
			fileName = this.FileName;
		}
		string str2 = "";
		string str3 = "";
		if (this.VCodec == "")
		{
			this.VCodec = "image2";
		}
		if (this.Width % 2 == 1)
		{
			this.Width = this.Width + 1;
		}
		if (this.Height % 2 == 1)
		{
			this.Height = this.Height + 1;
		}
		string str4 = " ";
		if (this.Width == 0 || this.Height == 0)
		{
			str4 = " ";
		}
		else
		{
			object[] width = new object[] { " -s ", this.Width, "x", this.Height, " " };
			str4 = string.Concat(width);
		}
		if (!this.Multiple_Thumbs)
		{
			str = this.Return_Output_Name(this.FileName, this.ImageName, this.Image_Format);
			string[] outputPath = new string[] { "\"", this.OutputPath, "\\", str, "\"" };
			str2 = string.Concat(outputPath);
			string str5 = "";
			str5 = (this.Thumb_Start_Position <= 0 ? string.Concat(" -ss ", this.Frame_Time) : string.Concat(" -ss ", this.Thumb_Start_Position));
			string[] vCodec = new string[] { " ", str5, " -i ", fileName, " -vframes 1 ", str4, "-f ", this.VCodec, " -y ", str2 };
			str3 = this.Process_CMD(str1, string.Concat(vCodec));
		}
		else
		{
			if (this.No_Of_Thumbs < 1)
			{
				this.No_Of_Thumbs = 1;
			}
			videoInfo = this.Get_Info();
			if (videoInfo.ErrorCode > 0)
			{
				return videoInfo;
			}
			int durationSec = videoInfo.Duration_Sec;
			if (durationSec < 0)
			{
				videoInfo.ErrorCode = 125;
				return videoInfo;
			}
			if (this.ThumbMode != 0)
			{
				if (!this.Auto_Transition_Time)
				{
					if (this.No_Of_Thumbs * (int)this.Thumb_Transition_Time + this.Thumb_Start_Position > durationSec)
					{
						videoInfo.ErrorCode = 124;
						return videoInfo;
					}
				}
				else if (this.Thumb_Start_Position > 0)
				{
					if (this.No_Of_Thumbs != 1)
					{
						this.Thumb_Transition_Time = (double)((durationSec - this.Thumb_Start_Position) / this.No_Of_Thumbs);
					}
					else
					{
						this.Thumb_Transition_Time = (double)((durationSec - this.Thumb_Start_Position) / 2);
					}
				}
				else if (this.No_Of_Thumbs != 1)
				{
					this.Thumb_Transition_Time = (double)(durationSec / this.No_Of_Thumbs);
				}
				else
				{
					this.Thumb_Transition_Time = (double)(durationSec / 2);
				}
				this.Image_Format = "%03d.jpg";
				str = this.Return_Thumb_Name(this.FileName, this.ImageName, this.Image_Format);
				string[] strArrays = new string[] { "\"", this.OutputPath, "\\", str, "\"" };
				str2 = string.Concat(strArrays);
				string str6 = "";
				if (this.Thumb_Start_Position > 0)
				{
					str6 = string.Concat(" -ss ", this.Thumb_Start_Position, " ");
				}
				string str7 = "";
				str7 = (this.No_Of_Thumbs <= 0 ? " -vframes 1 " : string.Concat(" -vframes ", this.No_Of_Thumbs, " "));
				string str8 = " -r 1 ";
				if (this.Thumb_Transition_Time > 1)
				{
					str8 = (this.Thumb_Transition_Time <= 20 ? string.Concat(" -r 1/", this.Thumb_Transition_Time, " ") : " -r 1/20 ");
				}
				else if (this.Thumb_Start_Position < 1)
				{
					str8 = string.Concat(" -r ", 1 / this.Thumb_Transition_Time, " ");
				}
				string[] vCodec1 = new string[] { " ", str6, " -i ", fileName, " ", str7, " ", str8, " ", str4, " -f ", this.VCodec, " -y ", str2 };
				str3 = this.Process_CMD(str1, string.Concat(vCodec1));
			}
			else
			{
				if (this.Thumb_Start_Position > 0)
				{
					if (this.No_Of_Thumbs != 1)
					{
						this.Thumb_Transition_Time = (double)((durationSec - this.Thumb_Start_Position) / this.No_Of_Thumbs);
					}
					else
					{
						this.Thumb_Transition_Time = (double)((durationSec - this.Thumb_Start_Position) / 2);
					}
				}
				else if (this.No_Of_Thumbs != 1)
				{
					this.Thumb_Transition_Time = (double)(durationSec / this.No_Of_Thumbs);
				}
				else
				{
					this.Thumb_Transition_Time = (double)(durationSec / 2);
				}
				int i = 0;
				int num = 1;
				int thumbStartPosition = 0;
				if (this.Thumb_Start_Position > 0)
				{
					thumbStartPosition = this.Thumb_Start_Position;
				}
				string str9 = "";
				for (i = 0; i <= this.No_Of_Thumbs - 1; i++)
				{
					if (this.ImageName == "")
					{
						this.ImageName = this.FileName;
					}
					if (this.isMatch(this.ImageName, "\\."))
					{
						this.ImageName = this.ImageName.Remove(this.ImageName.LastIndexOf("."));
					}
					if (num >= 10)
					{
						str9 = (num < 10 || i >= 100 ? num.ToString() : string.Concat("0", num));
					}
					else
					{
						str9 = string.Concat("00", num);
					}
					if (!this.Image_Format.StartsWith("."))
					{
						this.Image_Format = string.Concat(".", this.Image_Format);
					}
					str = string.Concat(this.ImageName, str9, this.Image_Format);
					string[] outputPath1 = new string[] { "\"", this.OutputPath, "\\", str, "\"" };
					str2 = string.Concat(outputPath1);
					string parameters = "";
					if (this.Parameters != "")
					{
						parameters = this.Parameters;
					}
					object[] objArray = new object[] { " -ss ", thumbStartPosition, " -i ", fileName, " -vframes 1 ", parameters, " ", str4, "-f ", this.VCodec, " -y ", str2 };
					str3 = this.Process_OGG_CMD(str1, string.Concat(objArray));
					thumbStartPosition = thumbStartPosition + (int)this.Thumb_Transition_Time;
					if (thumbStartPosition > durationSec)
					{
						thumbStartPosition = durationSec - 1;
					}
					num++;
				}
			}
		}
		if (str3 == "")
		{
			videoInfo.ErrorCode = 110;
			return videoInfo;
		}
		if (str3.Contains("Error while opening codec for output stream"))
		{
			videoInfo.ErrorCode = 128;
			return videoInfo;
		}
		videoInfo.FileName = str;
		videoInfo.FFMPEGOutput = str3;
		return videoInfo;
	}

	private VideoInfo ImagesToVideo()
	{
		VideoInfo videoInfo;
		VideoInfo message = new VideoInfo();
		string str = "";
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			message.ErrorCode = 129;
			return message;
		}
		if (!this.Validate_FFMPEG())
		{
			message.ErrorCode = 100;
			return message;
		}
		if (this.FileName == "")
		{
			message.ErrorCode = 101;
			return message;
		}
		string str1 = string.Concat(this.FileName, "001.jpg");
		if (!File.Exists(string.Concat(this.InputPath, "\\", str1)))
		{
			message.ErrorCode = 101;
			return message;
		}
		if (!this.Validate_OutputPath())
		{
			message.ErrorCode = 102;
			return message;
		}
		if (this.WaterMarkPath != "" && this.WaterMarkImage != "" && !this.Validate_WaterMarkPath())
		{
			message.ErrorCode = 108;
			return message;
		}
		if (this.OutputFileName == "")
		{
			message.ErrorCode = 141;
			message.ErrorMessage = "Output File Name must specifiy, e.g sample.avi or sample, extension will skip if specified.";
			return message;
		}
		if (this.OutputExtension == "")
		{
			message.ErrorCode = 140;
			message.ErrorMessage = "Must specify output media type (OutputExtension)";
			return message;
		}
		if (!this.OutputExtension.StartsWith("."))
		{
			this.OutputExtension = string.Concat(".", this.OutputExtension);
		}
		str = (!this.isMatch(this.OutputFileName, "\\.") ? string.Concat(this.OutputFileName, this.OutputExtension) : string.Concat(this.OutputFileName.Remove(this.OutputFileName.LastIndexOf(".")), this.OutputExtension));
		if (this.Width % 2 == 1)
		{
			this.Width = this.Width + 1;
		}
		if (this.Height % 2 == 1)
		{
			this.Height = this.Height + 1;
		}
		string str2 = string.Concat("\"", this.FFMPEGPath, "\"");
		string[] inputPath = new string[] { "\"", this.InputPath, "\\", this.FileName, "%03d.jpg\"" };
		string str3 = string.Concat(inputPath);
		string[] outputPath = new string[] { "\"", this.OutputPath, "\\", str, "\"" };
		string str4 = string.Concat(outputPath);
		try
		{
			string str5 = this.Prepare_Command(str3, str4, false, true);
			string str6 = this.Process_CMD(str2, str5);
			message = this.PARSE_FFMPEG_OUTPUT(str6);
			message.FileName = str;
			message.FFMPEGOutput = str6;
			videoInfo = message;
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			message.ErrorCode = 121;
			message.ErrorMessage = exception.Message;
			videoInfo = message;
		}
		return videoInfo;
	}

	public string isAudio()
	{
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			return "129";
		}
		if (!this.Validate_FFMPEG())
		{
			return "100";
		}
		if (!this.Validate_InputPath())
		{
			return "101";
		}
		string str = string.Concat("\"", this.FFMPEGPath, "\"");
		string[] inputPath = new string[] { "\"", this.InputPath, "\\", this.FileName, "\"" };
		string fileName = string.Concat(inputPath);
		if (this.FileName.StartsWith("http"))
		{
			fileName = this.FileName;
		}
		string str1 = string.Concat(" -i ", fileName);
		string str2 = this.Process_CMD(str, str1);
		if (str2 == "")
		{
			return "110";
		}
		if (!this.Validate_Video(str2))
		{
			return "111";
		}
		if (this.isMatch(str2, "no such file or directory"))
		{
			return "105";
		}
		if (this.isMatch(str2, "Unknown format"))
		{
			return "104";
		}
		if (this.isMatch(str2, "Unknown encoder"))
		{
			return "107";
		}
		if (!this.isMatch(str2, "Duration"))
		{
			return "107";
		}
		if (this.isMatch(str2, "unrecognized option"))
		{
			return "115";
		}
		if (this.isMatch(str2, "Could not open"))
		{
			return "116";
		}
		if (str2.Remove(0, str2.LastIndexOf("Duration")).Contains("Video:"))
		{
			return "video";
		}
		return "audio";
	}

	private bool isMatch(string value, string expression)
	{
		if (Regex.Match(value, expression).Success)
		{
			return true;
		}
		return false;
	}

	private VideoInfo Join_Videos()
	{
		VideoInfo videoInfo;
		VideoInfo message = new VideoInfo();
		string str = "";
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			message.ErrorCode = 129;
			return message;
		}
		if (!this.Validate_FFMPEG())
		{
			message.ErrorCode = 100;
			return message;
		}
		if (!this.Validate_OutputPath())
		{
			message.ErrorCode = 102;
			return message;
		}
		if (this.FileNames == null)
		{
			message.ErrorCode = 138;
			message.ErrorMessage = "Join Video: Please specify filenames.";
			return message;
		}
		if ((int)this.FileNames.Length < 2)
		{
			message.ErrorCode = 143;
			message.ErrorMessage = "Join Video: Must contain two or more clips.";
			return message;
		}
		if (this.OutputFileName == "")
		{
			message.ErrorCode = 141;
			message.ErrorMessage = "Join Video: Output File Name must specifiy, e.g sample.avi or sample, extension will skip if specified.";
			return message;
		}
		if (this.OutputExtension == "")
		{
			message.ErrorCode = 140;
			message.ErrorMessage = "Join Video: Must specify output media type (OutputExtension)";
			return message;
		}
		int i = 0;
		string str1 = "";
		bool flag = true;
		bool flag1 = true;
		for (i = 0; i <= (int)this.FileNames.Length - 1; i++)
		{
			str1 = this.FileNames[i].ToString();
			if (!File.Exists(string.Concat(this.InputPath, "\\", str1)))
			{
				flag = false;
			}
			if (!str1.EndsWith(".mpg"))
			{
				flag1 = false;
			}
		}
		if (!flag)
		{
			message.ErrorCode = 139;
			message.ErrorMessage = "Join Video: Input file path validation failed, please make sure that all joining clips / files must be located in input path directory";
			return message;
		}
		if (!this.OutputExtension.StartsWith("."))
		{
			this.OutputExtension = string.Concat(".", this.OutputExtension);
		}
		str = (!this.isMatch(this.OutputFileName, "\\.") ? string.Concat(this.OutputFileName, this.OutputExtension) : string.Concat(this.OutputFileName.Remove(this.OutputFileName.LastIndexOf(".")), this.OutputExtension));
		string str2 = string.Concat("\"", this.FFMPEGPath, "\"");
		string[] outputPath = new string[] { "\"", this.OutputPath, "\\", str, "\"" };
		string str3 = string.Concat(outputPath);
		ArrayList arrayLists = new ArrayList();
		ArrayList arrayLists1 = new ArrayList();
		ArrayList arrayLists2 = new ArrayList();
		Guid guid = Guid.NewGuid();
		string str4 = guid.ToString().Substring(0, 6);
		string str5 = "";
		for (i = 0; i <= (int)this.FileNames.Length - 1; i++)
		{
			string str6 = string.Concat(str4, i, ".mpg");
			string[] inputPath = new string[] { "\"", this.InputPath, "\\", this.FileNames[i].ToString(), "\"" };
			string str7 = string.Concat(inputPath);
			string[] strArrays = new string[] { "\"", this.OutputPath, "\\", str6, "\"" };
			string str8 = string.Concat(strArrays);
			str5 = (i != 0 ? string.Concat(str5, " ", str8) : str8);
			arrayLists2.Add(str6);
			arrayLists.Add(str7);
			arrayLists1.Add(str8);
		}
		if (this.Width % 2 == 1)
		{
			this.Width = this.Width + 1;
		}
		if (this.Height % 2 == 1)
		{
			this.Height = this.Height + 1;
		}
		string str9 = " ";
		if (this.Width == 0 || this.Height == 0)
		{
			str9 = " ";
		}
		else
		{
			object[] width = new object[] { " -s ", this.Width, "x", this.Height, " " };
			str9 = string.Concat(width);
		}
		try
		{
			string str10 = "";
			if (!flag1 || !(this.OutputExtension == ".mpg"))
			{
				i = 0;
				while (i <= arrayLists1.Count - 1)
				{
					message.ErrorCode = 0;
					string[] strArrays1 = new string[] { " -i ", arrayLists[i].ToString(), " -sameq ", str9, " -y ", arrayLists1[i].ToString() };
					str10 = string.Concat(strArrays1);
					message = this.PARSE_FFMPEG_OUTPUT(this.Process_CMD(str2, str10));
					if (message.ErrorCode <= 0)
					{
						i++;
					}
					else
					{
						for (i = 0; i <= arrayLists1.Count - 1; i++)
						{
							if (File.Exists(string.Concat(this.OutputPath, "\\", arrayLists2[i].ToString())))
							{
								File.Delete(string.Concat(this.OutputPath, "\\", arrayLists2[i].ToString()));
							}
						}
						message.ErrorCode = 142;
						message.ErrorMessage = "Join Video: Failed to create temp mpg files for attaching videos.";
						videoInfo = message;
						return videoInfo;
					}
				}
				string[] outputPath1 = new string[] { "\"", this.OutputPath, "\\", str4, "_full.mpg\"" };
				string str11 = string.Concat(outputPath1);
				str10 = string.Concat(str5, " > ", str11);
				this.Process_CMD("cmd.exe", string.Concat("/C type ", str10));
				str10 = this.Prepare_Command(str11, str3, false, true);
				message = this.PARSE_FFMPEG_OUTPUT(this.Process_CMD(str2, str10));
				if (message.ErrorCode <= 0)
				{
					if (File.Exists(string.Concat(this.OutputPath, "\\", str4, "_full.mpg")))
					{
						File.Delete(string.Concat(this.OutputPath, "\\", str4, "_full.mpg"));
					}
					for (i = 0; i <= arrayLists1.Count - 1; i++)
					{
						if (File.Exists(string.Concat(this.OutputPath, "\\", arrayLists2[i].ToString())))
						{
							File.Delete(string.Concat(this.OutputPath, "\\", arrayLists2[i].ToString()));
						}
					}
				}
				else
				{
					message.ErrorCode = 142;
					message.ErrorMessage = "Final video publishing failed.";
					videoInfo = message;
					return videoInfo;
				}
			}
			else
			{
				str10 = string.Concat(str5, " > ", str3);
				this.Process_CMD("cmd.exe", string.Concat("/C type ", str10));
			}
			videoInfo = message;
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			message.ErrorCode = 121;
			message.ErrorMessage = exception.Message;
			videoInfo = message;
		}
		return videoInfo;
	}

	private VideoInfo PARSE_FFMPEG_INPUT_DATA(VideoInfo info, string ffmpeg_data)
	{
		string value = "";
		string str = "";
		string str1 = "Stream #(?<number>\\d+?\\.\\d+?)(\\((?<language>\\w+)\\))?(\\[(?<abc>\\w+)\\])?:\\s(?<type>\\w+):\\s(?<data>.+)";
		string str2 = "(?<width>\\d+)x(?<height>\\d+)";
		string str3 = "(?<bitrate>\\d+(\\.\\d+)?(\\s+)kb/s)";
		string str4 = "(?<codec>\\w+),";
		string str5 = "(?<framerate>\\d+(\\.\\d+)? (tbr|fps))";
		string str6 = "(?<samplingrate>\\d+(\\.\\d+)?(\\s+)Hz), (?<channel>\\w+)";
		Match match = Regex.Match(ffmpeg_data, str1);
		if (!match.Success)
		{
			str1 = "Stream #(?<number>\\d+:\\d+?)(\\((?<language>\\w+)\\))?(\\[(?<abc>\\w+)\\])?:\\s(?<type>\\w+):\\s(?<data>.+)";
			match = Regex.Match(ffmpeg_data, str1);
			if (match.Success)
			{
				info.Input_Type = match.Groups["type"].Value;
				if (info.Input_Type != "Video")
				{
					str = match.Groups["data"].Value;
				}
				else
				{
					value = match.Groups["data"].Value;
				}
			}
		}
		else
		{
			info.Input_Type = match.Groups["type"].Value;
			if (info.Input_Type != "Video")
			{
				str = match.Groups["data"].Value;
			}
			else
			{
				value = match.Groups["data"].Value;
			}
		}
		string str7 = "";
		str7 = (str == "" ? value : str);
		Match match1 = Regex.Match(str7, str1);
		if (match1.Success)
		{
			info.Input_Type = match1.Groups["type"].Value;
			if (info.Input_Type != "Video")
			{
				str = match1.Groups["data"].Value;
			}
			else
			{
				value = match1.Groups["data"].Value;
			}
		}
		Match match2 = Regex.Match(value, str2);
		if (match2.Success)
		{
			info.Input_Width = Convert.ToInt32(match2.Groups["width"].Value);
			info.Input_Height = Convert.ToInt32(match2.Groups["height"].Value);
			if ((info.Input_Width == 0 || info.Input_Width > 4000 || info.Input_Height == 0 || info.Input_Height > 4000) && (info.Input_Width != 0 || info.Input_Height != 0))
			{
				info.Input_Width = 0;
				info.Input_Height = 0;
				match2 = match2.NextMatch();
				info.Input_Width = Convert.ToInt32(match2.Groups["width"].Value);
				info.Input_Height = Convert.ToInt32(match2.Groups["height"].Value);
			}
		}
		Match match3 = Regex.Match(value, str3);
		if (match3.Success)
		{
			info.Input_Video_Bitrate = match3.Groups["bitrate"].Value;
		}
		Match match4 = Regex.Match(value, str4);
		if (match4.Success)
		{
			info.Input_Vcodec = match4.Groups["codec"].Value;
		}
		Match match5 = Regex.Match(value, str5);
		if (match5.Success)
		{
			info.Input_FrameRate = match5.Groups["framerate"].Value;
		}
		Match match6 = Regex.Match(str, str6);
		if (match6.Success)
		{
			info.Input_SamplingRate = match6.Groups["samplingrate"].Value;
			info.Input_Channel = match6.Groups["channel"].Value;
		}
		Match match7 = Regex.Match(str, str3);
		if (match7.Success)
		{
			info.Input_Audio_Bitrate = match7.Groups["bitrate"].Value;
		}
		Match match8 = Regex.Match(str, str4);
		if (match8.Success)
		{
			info.Input_Acodec = match8.Groups["codec"].Value;
		}
		if (value != "")
		{
			info.Input_Type = "video";
		}
		return info;
	}

	private VideoInfo PARSE_FFMPEG_OUTPUT(string ffmpeg_data)
	{
		VideoInfo videoInfo = new VideoInfo();
		ffmpeg_data = this.FixCode(ffmpeg_data);
		videoInfo.FFMPEGOutput = ffmpeg_data;
		videoInfo = this.FFMPEG_OUTPUT_VALIDATION(ffmpeg_data);
		if (videoInfo.ErrorCode > 0)
		{
			return videoInfo;
		}
		string str = "Input #(?<data>.+)";
		string str1 = "Output #(?<data>.+)";
		string str2 = "Duration: (?<hours>\\d{1,3}):(?<minutes>\\d{2}):(?<seconds>\\d{2})(.(?<fractions>\\d{1,3}))?, start: (?<start>\\d+(\\.\\d+)?), bitrate: (?<bitrate>\\d+(\\.\\d+)?(\\s+)kb/s)?";
		string value = "";
		string value1 = "";
		Match match = Regex.Match(ffmpeg_data, str);
		if (match.Success)
		{
			value = match.Groups["data"].Value;
		}
		if (value == "")
		{
			videoInfo.ErrorCode = 110;
			videoInfo.ErrorMessage = "No data retrieved from ffmpeg output";
			return videoInfo;
		}
		Match match1 = Regex.Match(value, str2);
		if (match1.Success)
		{
			videoInfo.Hours = Convert.ToInt32(match1.Groups["hours"].Value);
			videoInfo.Minutes = Convert.ToInt32(match1.Groups["minutes"].Value);
			videoInfo.Seconds = Convert.ToInt32(match1.Groups["seconds"].Value);
			if (Convert.ToInt32(match1.Groups["fractions"].Value) > 5)
			{
				videoInfo.Seconds = videoInfo.Seconds + 1;
			}
			videoInfo.Start = match1.Groups["start"].Value;
			videoInfo.Video_Bitrate = match1.Groups["bitrate"].Value;
			object[] hours = new object[] { videoInfo.Hours, ":", videoInfo.Minutes, ":", videoInfo.Seconds };
			videoInfo.Duration = string.Concat(hours);
			videoInfo.Duration_Sec = videoInfo.Hours * 3600 + videoInfo.Minutes * 60 + videoInfo.Seconds;
		}
		Match match2 = Regex.Match(ffmpeg_data, str1);
		if (match2.Success)
		{
			value1 = match2.Groups["data"].Value;
		}
		if (value1 == "")
		{
			this.PARSE_FFMPEG_OUTPUT_DATA(videoInfo, value);
		}
		else
		{
			this.PARSE_FFMPEG_INPUT_DATA(videoInfo, value);
			this.PARSE_FFMPEG_OUTPUT_DATA(videoInfo, value1);
		}
		return videoInfo;
	}

	private VideoInfo PARSE_FFMPEG_OUTPUT_DATA(VideoInfo info, string ffmpeg_data)
	{
		string value = "";
		string str = "";
		string str1 = "Stream #(?<number>\\d+?\\.\\d+?)(\\((?<language>\\w+)\\))?(\\[(?<abc>\\w+)\\])?:\\s(?<type>\\w+):\\s(?<data>.+)";
		string str2 = "(?<width>\\d+)x(?<height>\\d+)";
		string str3 = "(?<bitrate>\\d+(\\.\\d+)?(\\s+)kb/s)";
		string str4 = "(?<codec>\\w+),";
		string str5 = "(?<framerate>\\d+(\\.\\d+)? (tbr|fps))";
		string str6 = "(?<samplingrate>\\d+(\\.\\d+)?(\\s+)Hz), (?<channel>\\w+)";
		Match match = Regex.Match(ffmpeg_data, str1);
		if (!match.Success)
		{
			str1 = "Stream #(?<number>\\d+:\\d+?)(\\((?<language>\\w+)\\))?(\\[(?<abc>\\w+)\\])?:\\s(?<type>\\w+):\\s(?<data>.+)";
			match = Regex.Match(ffmpeg_data, str1);
			if (match.Success)
			{
				info.Input_Type = match.Groups["type"].Value;
				if (info.Input_Type != "Video")
				{
					str = match.Groups["data"].Value;
				}
				else
				{
					value = match.Groups["data"].Value;
				}
			}
		}
		else
		{
			info.Type = match.Groups["type"].Value;
			if (info.Type != "Video")
			{
				str = match.Groups["data"].Value;
			}
			else
			{
				value = match.Groups["data"].Value;
			}
		}
		string str7 = "";
		str7 = (str == "" ? value : str);
		Match match1 = Regex.Match(str7, str1);
		if (match1.Success)
		{
			info.Type = match1.Groups["type"].Value;
			if (info.Type != "Video")
			{
				str = match1.Groups["data"].Value;
			}
			else
			{
				value = match1.Groups["data"].Value;
			}
		}
		Match match2 = Regex.Match(value, str2);
		if (match2.Success)
		{
			info.Width = Convert.ToInt32(match2.Groups["width"].Value);
			info.Height = Convert.ToInt32(match2.Groups["height"].Value);
			if ((info.Width == 0 || info.Width > 4000 || info.Height == 0 || info.Height > 4000) && (info.Width != 0 || info.Height != 0))
			{
				info.Width = 0;
				info.Height = 0;
				match2 = match2.NextMatch();
				info.Width = Convert.ToInt32(match2.Groups["width"].Value);
				info.Height = Convert.ToInt32(match2.Groups["height"].Value);
			}
		}
		Match match3 = Regex.Match(value, str3);
		if (match3.Success)
		{
			info.Video_Bitrate = match3.Groups["bitrate"].Value;
		}
		Match match4 = Regex.Match(value, str4);
		if (match4.Success)
		{
			info.Vcodec = match4.Groups["codec"].Value;
		}
		Match match5 = Regex.Match(value, str5);
		if (match5.Success)
		{
			info.FrameRate = match5.Groups["framerate"].Value;
		}
		Match match6 = Regex.Match(str, str6);
		if (match6.Success)
		{
			info.SamplingRate = match6.Groups["samplingrate"].Value;
			info.Channel = match6.Groups["channel"].Value;
		}
		Match match7 = Regex.Match(str, str3);
		if (match7.Success)
		{
			info.Audio_Bitrate = match7.Groups["bitrate"].Value;
		}
		Match match8 = Regex.Match(str, str4);
		if (match8.Success)
		{
			info.Acodec = match8.Groups["codec"].Value;
		}
		if (value != "")
		{
			info.Type = "video";
		}
		return info;
	}

	private void ParseValidationInfo(string data)
	{
		this.vinfo.ErrorMessage = data;
		if (data == "")
		{
			this.vinfo.ErrorCode = 110;
		}
		if (this.isMatch(data, "Unknown format"))
		{
			this.vinfo.ErrorCode = 131;
			return;
		}
		if (this.isMatch(data, "no such file or directory"))
		{
			this.vinfo.ErrorCode = 105;
			return;
		}
		if (this.isMatch(data, "Unsupported codec"))
		{
			this.vinfo.ErrorCode = 104;
			return;
		}
		if (this.isMatch(data, "Unknown encoder"))
		{
			this.vinfo.ErrorCode = 132;
			return;
		}
		if (!this.isMatch(data, "Duration"))
		{
			this.vinfo.ErrorCode = 107;
			return;
		}
		if (this.isMatch(data, "unrecognized option"))
		{
			this.vinfo.ErrorCode = 115;
			return;
		}
		if (this.isMatch(data, "Could not open"))
		{
			this.vinfo.ErrorCode = 116;
			return;
		}
		if (this.isMatch(data, "video codec not compatible"))
		{
			this.vinfo.ErrorCode = 118;
			return;
		}
		if (this.isMatch(data, "Unknown codec"))
		{
			this.vinfo.ErrorCode = 133;
			return;
		}
		if (this.isMatch(data, "Video hooking not compiled"))
		{
			this.vinfo.ErrorCode = 123;
			return;
		}
		if (this.isMatch(data, "incorrect parameters"))
		{
			this.vinfo.ErrorCode = 134;
			return;
		}
		if (this.isMatch(data, "Failed to add video hook"))
		{
			this.vinfo.ErrorCode = 135;
			return;
		}
		if (this.isMatch(data, "Unable to parse option value"))
		{
			this.vinfo.ErrorCode = 145;
			return;
		}
		if (this.isMatch(data, "incorrect codec parameters"))
		{
			this.vinfo.ErrorCode = 146;
			return;
		}
		if (this.isMatch(data, "File for preset") && this.isMatch(data, "not found"))
		{
			this.vinfo.ErrorCode = 144;
			return;
		}
		if (this.isMatch(data, "not within the padded area"))
		{
			this.vinfo.ErrorCode = 148;
		}
	}

	private string Prepare_Command(string inputpath, string outputpath, bool isaudio, bool iswatermark)
	{
		string str = "";
		string str1 = "";
		if (this.Width > 0 && this.Height > 0)
		{
			object[] width = new object[] { " -s ", this.Width, "x", this.Height, " " };
			str1 = string.Concat(width);
		}
		string str2 = "";
		if (this.MaxQuality)
		{
			str2 = " -sameq ";
		}
		string str3 = "";
		if (this.DisableVideo)
		{
			str3 = " -vn ";
		}
		string str4 = "";
		if (this.DisableAudio)
		{
			str4 = " -an ";
		}
		string str5 = "";
		if (this.AspectRatio != "")
		{
			str5 = string.Concat(" -aspect ", this.AspectRatio, " ");
		}
		string str6 = "";
		if (this.Channel == 1 || this.Channel == 2)
		{
			str6 = string.Concat(" -ac ", this.Channel, " ");
		}
		string str7 = "";
		if (this.Audio_SamplingRate != 0)
		{
			str7 = string.Concat(" -ar ", this.Audio_SamplingRate, " ");
		}
		string str8 = "";
		if (this.Audio_Bitrate != 0)
		{
			str8 = string.Concat(" -ab ", this.Audio_Bitrate, "k ");
		}
		string str9 = "";
		if (this.Video_Bitrate != 0)
		{
			str9 = string.Concat(" -b ", this.Video_Bitrate, "k ");
		}
		string str10 = "";
		if (this.FrameRate != 0)
		{
			str10 = string.Concat(" -r ", this.FrameRate, " ");
		}
		string str11 = "";
		if (this.Duration != "")
		{
			str11 = string.Concat(" -t ", this.Duration, " ");
		}
		string str12 = "";
		if (this.VCodec != "")
		{
			str12 = string.Concat(" -vcodec ", this.VCodec, " ");
		}
		string str13 = "";
		if (this.ACodec != "")
		{
			str13 = string.Concat(" -acodec ", this.ACodec, " ");
		}
		string str14 = "";
		if (this.Deinterlace)
		{
			str14 = " -deinterlace ";
		}
		string str15 = "";
		if (this.PadTop > 0)
		{
			str15 = string.Concat(" -padtop ", this.PadTop, " ");
		}
		string str16 = "";
		if (this.PadBottom > 0)
		{
			str16 = string.Concat(" -padbottom ", this.PadBottom, " ");
		}
		string str17 = "";
		if (this.PadLeft > 0)
		{
			str17 = string.Concat(" -padleft ", this.PadLeft, " ");
		}
		string str18 = "";
		if (this.PadRight > 0)
		{
			str18 = string.Concat(" -padright ", this.PadRight, " ");
		}
		string str19 = "";
		if (this.PadColor != "")
		{
			str19 = string.Concat(" -padcolor ", this.PadColor, " ");
		}
		string str20 = "";
		if (this.CropTop > 0)
		{
			str20 = string.Concat(" -croptop ", this.CropTop, " ");
		}
		string str21 = "";
		if (this.CropBottom > 0)
		{
			str21 = string.Concat(" -cropbottom ", this.CropBottom, " ");
		}
		string str22 = "";
		if (this.CropLeft > 0)
		{
			str22 = string.Concat(" -cropleft ", this.CropLeft, " ");
		}
		string str23 = "";
		if (this.CropRight > 0)
		{
			str23 = string.Concat(" -cropright ", this.CropRight, " ");
		}
		string str24 = "";
		if (this.Start_Position != "")
		{
			str24 = string.Concat(" -ss ", this.Start_Position, " ");
		}
		string str25 = "";
		if (this.Force != "")
		{
			str25 = string.Concat(" -f ", this.Force, " ");
		}
		string str26 = "";
		if (this.Gop_Size > 0)
		{
			str26 = string.Concat(" -g ", this.Gop_Size, " ");
		}
		string str27 = "";
		if (this.Bitrate_Tolerance != 4000)
		{
			str27 = string.Concat(" -bt ", this.Bitrate_Tolerance, " ");
		}
		string str28 = "";
		if (this.Limit_File_Size > 0)
		{
			str28 = string.Concat(" -fs ", this.Limit_File_Size, " ");
		}
		string str29 = "";
		if (this.MaxRate > 0)
		{
			str29 = string.Concat(" -maxrate ", this.MaxRate, " ");
		}
		string str30 = "";
		if (this.MinRate > 0)
		{
			str30 = string.Concat(" -minrate ", this.MinRate, " ");
		}
		string str31 = "";
		if (this.BufferSize > 0)
		{
			str31 = string.Concat(" -bufsize ", this.BufferSize, " ");
		}
		string str32 = "";
		if (this.Pass > 0 && this.Pass < 3)
		{
			str32 = string.Concat(" -pass ", this.Pass, " ");
		}
		string str33 = "";
		if (this.Scale_Quality > 0 && this.Scale_Quality < 32)
		{
			str33 = string.Concat(" -qscale ", this.Scale_Quality, " ");
		}
		string str34 = "";
		if (this.PresetPath != "")
		{
			str34 = string.Concat(" -vpre \"", this.PresetPath, "\"");
		}
		string str35 = "";
		if (this.TargetFileType != "")
		{
			str35 = string.Concat(" -target ", this.TargetFileType, " ");
		}
		string str36 = "";
		if (this.Parameters != "")
		{
			str36 = string.Concat(" ", this.Parameters, " ");
		}
		if (isaudio)
		{
			string[] strArrays = new string[] { " ", str24, str11, " -i ", inputpath, str32, str26, str27, str28, str12, str29, str30, str31, str33, str36, str6, str34, str13, str9, str7, str8, str25, " -vn -y ", outputpath };
			str = string.Concat(strArrays);
		}
		else if (this.WaterMarkPath != "" && this.WaterMarkImage != "" && iswatermark)
		{
			if (!this.DisableVideo)
			{
				string str37 = string.Concat(this.FFMPEGPath.Remove(this._ffmpegpath.LastIndexOf("ffmpeg.exe")), "vhook\\watermark.dll");
				string[] waterMarkPath = new string[] { "\"", this.WaterMarkPath, "\\", this.WaterMarkImage, "\"" };
				string str38 = string.Concat(waterMarkPath);
				string[] strArrays1 = new string[] { " ", str24, str11, " -i ", inputpath, str32, str26, str27, str28, str12, str29, str30, str31, str33, str35, str34, str13, str36, str6, str15, str16, str17, str18, str19, str20, str21, str22, str23, str25, str5, str10, str9, str8, str7, str1, str2, str3, str4, str14, " -vhook \"", str37, " -f ", str38, " -m 1\" -y ", outputpath };
				str = string.Concat(strArrays1);
			}
			else
			{
				string[] strArrays2 = new string[] { " ", str24, str11, " -i ", inputpath, str32, str26, str27, str28, str29, str12, str30, str31, str33, str35, str34, str13, str36, str6, str8, str7, str25, str3, str4, " -y ", outputpath };
				str = string.Concat(strArrays2);
			}
		}
		else if (!this.DisableVideo)
		{
			string[] strArrays3 = new string[] { " ", str24, str11, " -i ", inputpath, str32, str26, str27, str28, str12, str29, str30, str31, str33, str35, str34, str13, str36, " ", str6, str15, str16, str17, str18, str20, str21, str22, str23, str19, str5, str10, str9, str8, str7, str1, str2, str25, str3, str4, str14, " -y ", outputpath };
			str = string.Concat(strArrays3);
		}
		else
		{
			string[] strArrays4 = new string[] { " ", str24, str11, " -i ", inputpath, str32, str26, str27, str28, str12, str29, str30, str31, str33, str35, str34, str13, str36, str6, str8, str7, str3, str4, str25, " -y ", outputpath };
			str = string.Concat(strArrays4);
		}
		return str;
	}

	public VideoInfo Process()
	{
		VideoInfo videoInfo;
		VideoInfo message = new VideoInfo();
		string str = "";
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			message.ErrorCode = 129;
			return message;
		}
		if (!this.Validate_FFMPEG())
		{
			message.ErrorCode = 100;
			return message;
		}
		if (!this.Validate_InputPath())
		{
			message.ErrorCode = 101;
			return message;
		}
		if (!this.Validate_OutputPath())
		{
			message.ErrorCode = 102;
			return message;
		}
		if (this.WaterMarkPath != "" && this.WaterMarkImage != "" && !this.Validate_WaterMarkPath())
		{
			message.ErrorCode = 108;
			return message;
		}
		if (this.OutputFileName == "")
		{
			message.ErrorCode = 141;
			message.ErrorMessage = "Output File Name must specifiy, e.g sample.avi or sample, extension will skip if specified.";
			return message;
		}
		if (this.OutputExtension == "")
		{
			message.ErrorCode = 140;
			message.ErrorMessage = "Must specify output media type (OutputExtension)";
			return message;
		}
		if (!this.OutputExtension.StartsWith("."))
		{
			this.OutputExtension = string.Concat(".", this.OutputExtension);
		}
		str = (!this.isMatch(this.OutputFileName, "\\.") ? string.Concat(this.OutputFileName, this.OutputExtension) : string.Concat(this.OutputFileName.Remove(this.OutputFileName.LastIndexOf(".")), this.OutputExtension));
		if (this.Width % 2 == 1)
		{
			this.Width = this.Width + 1;
		}
		if (this.Height % 2 == 1)
		{
			this.Height = this.Height + 1;
		}
		string str1 = string.Concat("\"", this.FFMPEGPath, "\"");
		string[] inputPath = new string[] { "\"", this.InputPath, "\\", this.FileName, "\"" };
		string str2 = string.Concat(inputPath);
		string[] outputPath = new string[] { "\"", this.OutputPath, "\\", str, "\"" };
		string str3 = string.Concat(outputPath);
		try
		{
			string str4 = this.Prepare_Command(str2, str3, false, true);
			string str5 = this.Process_CMD(str1, str4);
			message = this.PARSE_FFMPEG_OUTPUT(str5);
			message.FileName = str;
			message.FFMPEGOutput = str5;
			videoInfo = message;
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			message.ErrorCode = 121;
			message.ErrorMessage = exception.Message;
			videoInfo = message;
		}
		return videoInfo;
	}

	private string Process_CMD(string _ffmpegpath, string cmd)
	{
		string end = "";
		Process process = new Process();
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		process.StartInfo.FileName = _ffmpegpath;
		process.StartInfo.Arguments = cmd;
		if (!process.Start())
		{
			return "007";
		}
        process.PriorityClass = ProcessPriorityClass.High;
		process.WaitForExit(this.ExitProcess);
		end = process.StandardError.ReadToEnd();
		if (!process.HasExited)
		{
			process.Kill();
		}
		return end;
	}

	private string Process_CMD_Custom(string _ffmpegpath, string cmd)
	{
		Process process = new Process();
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		process.StartInfo.FileName = _ffmpegpath;
		process.StartInfo.Arguments = string.Concat(" ", cmd);
		if (!process.Start())
		{
			return "007";
		}
		process.WaitForExit(this.ExitProcess);
		if (!process.HasExited)
		{
			process.Kill();
		}
		return process.StandardError.ReadToEnd();
	}

	private void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
	{
		if (e.Data == null)
		{
			return;
		}
		int durationSec = 0;
		Match match = null;
		if (e.Data.Contains("Press [q] to stop,"))
		{
			this._processstarted = true;
		}
		else if (e.Data.Contains("title"))
		{
			match = Regex.Match(e.Data.Trim(), this._title_expression);
			if (match.Success)
			{
				this.vinfo.Title = match.Groups["vtitle"].Value;
			}
		}
		else if (e.Data.Contains("comment"))
		{
			match = Regex.Match(e.Data.Trim(), this._comment_expression);
			if (match.Success)
			{
				this.vinfo.Footage = match.Groups["comment_footage"].Value;
				this.vinfo.Producer = match.Groups["comment_producer"].Value;
				this.vinfo.Music = match.Groups["comment_music"].Value;
			}
		}
		else if (e.Data.Contains("Duration"))
		{
			match = Regex.Match(e.Data, this.minfo_pattern);
			if (match.Success)
			{
				this.vinfo.Hours = Convert.ToInt32(match.Groups["hours"].Value);
				this.vinfo.Minutes = Convert.ToInt32(match.Groups["minutes"].Value);
				this.vinfo.Seconds = Convert.ToInt32(match.Groups["seconds"].Value);
				if (Convert.ToInt32(match.Groups["fractions"].Value) > 5)
				{
					this.vinfo.Seconds = this.vinfo.Seconds + 1;
				}
				this.vinfo.Start = match.Groups["start"].Value;
				this.vinfo.Video_Bitrate = match.Groups["bitrate"].Value;
				VideoInfo videoInfo = this.vinfo;
				object[] hours = new object[] { this.vinfo.Hours, ":", this.vinfo.Minutes, ":", this.vinfo.Seconds };
				videoInfo.Duration = string.Concat(hours);
				this.vinfo.Duration_Sec = this.vinfo.Hours * 3600 + this.vinfo.Minutes * 60 + this.vinfo.Seconds;
			}
		}
		else if (e.Data.Contains("frame") && e.Data.Contains("size"))
		{
			match = Regex.Match(e.Data, this.frame_info_pattern);
			if (match.Success)
			{
				Convert.ToInt32(match.Groups["frame"].Value);
				this.vinfo.ProcessedSize = Convert.ToInt64(match.Groups["fsize"].Value);
				int num = Convert.ToInt32(match.Groups["time_hr"].Value);
				int num1 = Convert.ToInt32(match.Groups["time_min"].Value);
				int num2 = Convert.ToInt32(match.Groups["time_sec"].Value);
				if (Convert.ToInt32(match.Groups["time_frac"].Value) > 5)
				{
					num2++;
				}
				int num3 = num * 3600;
				int num4 = num1 * 60;
				this.vinfo.ProcessedTime = num3 + num4 + num2;
				durationSec = this.vinfo.Duration_Sec - this.vinfo.ProcessedTime;
				this.vinfo.ProcessingLeft = Math.Round((double)(durationSec * 100) / (double)this.vinfo.Duration_Sec, 2);
				if (this.vinfo.ProcessingLeft < 0.5)
				{
					this.vinfo.ProcessingLeft = 0;
				}
				this.vinfo.ProcessingCompleted = 100 - this.vinfo.ProcessingLeft;
			}
		}
		else if (e.Data.Contains("Stream"))
		{
			string value = "";
			string str = "";
			match = Regex.Match(e.Data, this.strm_pattern);
			if (!match.Success)
			{
				this.strm_pattern = "Stream #(?<number>\\d+:\\d+?)(\\((?<language>\\w+)\\))?(\\[(?<abc>\\w+)\\])?:\\s(?<type>\\w+):\\s(?<data>.+)";
				match = Regex.Match(e.Data, this.strm_pattern);
				if (match.Success)
				{
					this.vinfo.Input_Type = match.Groups["type"].Value;
					if (this.vinfo.Input_Type != "Video")
					{
						str = match.Groups["data"].Value;
					}
					else
					{
						value = match.Groups["data"].Value;
					}
				}
			}
			else
			{
				this.vinfo.Input_Type = match.Groups["type"].Value;
				if (this.vinfo.Input_Type != "Video")
				{
					str = match.Groups["data"].Value;
				}
				else
				{
					value = match.Groups["data"].Value;
				}
			}
			string str1 = "(?<bitrate>\\d+(\\.\\d+)?(\\s+)kb/s)";
			string str2 = "(?<codec>\\w+)?,";
			string str3 = "(?<framerate>\\d+(\\.\\d+)?(\\s|)(tbr|fps))";
			string str4 = "(?<samplingrate>\\d+(\\.\\d+)?(\\s+)Hz),(\\s|)(?<channel>\\w+)";
			if (value != "")
			{
				match = Regex.Match(value, "(?<width>\\d+)x(?<height>\\d+)");
				if (match.Success)
				{
					this.vinfo.Width = Convert.ToInt32(match.Groups["width"].Value);
					this.vinfo.Height = Convert.ToInt32(match.Groups["height"].Value);
					if ((this.vinfo.Width == 0 || this.vinfo.Width > 4000 || this.vinfo.Height == 0 || this.vinfo.Height > 4000) && (this.vinfo.Width != 0 || this.vinfo.Height != 0))
					{
						this.vinfo.Width = 0;
						this.vinfo.Height = 0;
						match = match.NextMatch();
						this.vinfo.Width = Convert.ToInt32(match.Groups["width"].Value);
						this.vinfo.Height = Convert.ToInt32(match.Groups["height"].Value);
					}
				}
				match = Regex.Match(value, str1);
				if (match.Success)
				{
					this.vinfo.Video_Bitrate = match.Groups["bitrate"].Value;
				}
				match = Regex.Match(value, str2);
				if (match.Success)
				{
					this.vinfo.Vcodec = match.Groups["codec"].Value;
				}
				match = Regex.Match(value, str3);
				if (match.Success)
				{
					this.vinfo.FrameRate = match.Groups["framerate"].Value;
				}
			}
			if (str != "")
			{
				match = Regex.Match(str, str4);
				if (match.Success)
				{
					this.vinfo.SamplingRate = match.Groups["samplingrate"].Value;
					this.vinfo.Channel = match.Groups["channel"].Value;
				}
				match = Regex.Match(str, str1);
				if (match.Success)
				{
					this.vinfo.Audio_Bitrate = match.Groups["bitrate"].Value;
				}
				match = Regex.Match(str, str2);
				if (match.Success)
				{
					this.vinfo.Acodec = match.Groups["codec"].Value;
				}
			}
		}
		this._processinglog.Append(string.Concat("<br />.................................<br />Error Output<br />.....................................<br />", e.Data, "<br />.....................................<br />"));
	}

	private void process_Exited(object sender, EventArgs e)
	{
		if (!this._processstarted)
		{
			this.ParseValidationInfo(this._processinglog.ToString());
		}
		this.vinfo.ProcessingCompleted = 100;
		this.vinfo.ProcessingLeft = 0;
		this.eventHandled = true;
		this.vinfo.FFMPEGOutput = this._processinglog.ToString();
		this.process.Dispose();
	}

	public string Process_FFMPEG2Theora()
	{
		if (!this.Validate_InputPath())
		{
			return "101";
		}
		string str = string.Concat("\"", this.FFMPEGPath, "\"");
		string[] inputPath = new string[] { "\"", this.InputPath, "\\", this.FileName, "\"" };
		string str1 = string.Concat(inputPath);
		string[] outputPath = new string[] { "\"", this.OutputPath, "\\", this.OutputFileName, "\"" };
		string str2 = string.Concat(outputPath);
		string[] parameters = new string[] { str1, " ", this.Parameters, " -o ", str2 };
		this.ADVPROC(str, string.Concat(parameters));
		return "success";
	}

	private string Process_FLVTool_CMD(string _flvtoolpath, string cmd)
	{
		Process process = new Process();
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		process.StartInfo.FileName = _flvtoolpath;
		process.StartInfo.Arguments = string.Concat(" -U ", cmd);
		process.Start();
		string end = process.StandardOutput.ReadToEnd();
		end = string.Concat(end, " ", process.StandardError.ReadToEnd());
		process.WaitForExit();
		if (!process.HasExited)
		{
			process.Kill();
		}
		if (this.isMatch(end, "Permission denied"))
		{
			return "117";
		}
		return "success";
	}

	private string Process_Mencoder_CMD(string _ffmpegpath, string cmd)
	{
		Process process = new Process();
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		process.StartInfo.FileName = _ffmpegpath;
		process.StartInfo.Arguments = cmd;
		if (!process.Start())
		{
			return "007";
		}
		process.WaitForExit(this.ExitProcess);
		if (!process.HasExited)
		{
			process.Kill();
		}
		return process.StandardError.ReadToEnd();
	}

	private string Process_MP4Box_CMD(string _mp4boxpath, string cmd)
	{
		Process process = new Process();
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		process.StartInfo.FileName = _mp4boxpath;
		string parameters = "-isma -hint";
		if (this.Parameters != "")
		{
			parameters = this.Parameters;
		}
		process.StartInfo.Arguments = string.Concat(" ", parameters, " ", cmd);
		process.Start();
		string end = process.StandardOutput.ReadToEnd();
		end = string.Concat(end, " ", process.StandardError.ReadToEnd());
		process.WaitForExit();
		if (!process.HasExited)
		{
			process.Kill();
		}
		if (this.isMatch(end, "Permission denied"))
		{
			return "117";
		}
		return "success";
	}

	private string Process_OGG_CMD(string _fullpath, string cmd)
	{
        Process process = new Process();
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		process.StartInfo.FileName = _fullpath;
		process.StartInfo.Arguments = cmd;
	    process.StartInfo.Verb = "runas";
        process.Start();
        process.PriorityClass = ProcessPriorityClass.RealTime;
        string errorMessage = process.StandardError.ReadToEnd();
	    if (DebugModeEnabled)
        {
            var sb = new StringBuilder();
            var outputMessage = process.StandardOutput.ReadToEnd();
            sb.AppendLine($"{DateTime.Now} : {cmd}");
            if (!string.IsNullOrEmpty(outputMessage))
            {
                sb.AppendLine($"Log:\r\n{outputMessage} \r\n----------");
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                sb.AppendLine($"Error:\r\n{errorMessage} \r\n----------");
            }
            sb.AppendLine("----------");
            var lockTaken = Monitor.TryEnter(LogFile, new TimeSpan(0, 0, 1));
            if (lockTaken)
            {
                File.AppendAllText(LogFile, sb.ToString());
                Monitor.Exit(LogFile);
            }
        }
	    process.WaitForExit();
        if (!process.HasExited)
		{
			process.Kill();
        }
        return errorMessage;
	}

    private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
	{
		this._processinglog.Append(string.Concat("<br />.................................<br />Data Output<br />.....................................<br />", e.Data, "<br />.....................................<br />"));
	}

	public string ProcessCMD()
	{
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			return "129";
		}
		string str = string.Concat("\"", this.ServicePath, "\"");
		return this.Process_OGG_CMD(str, this.Parameters);
	}

	public VideoInfo ProcessMedia()
	{
		VideoInfo videoInfo;
		string str = "";
		if (!this.Validate_FFMPEG())
		{
			this.vinfo.ErrorCode = 100;
			return this.vinfo;
		}
		if (!this.Validate_InputPath())
		{
			this.vinfo.ErrorCode = 101;
			return this.vinfo;
		}
		if (!this.Validate_OutputPath())
		{
			this.vinfo.ErrorCode = 102;
			return this.vinfo;
		}
		if (this.WaterMarkPath != "" && this.WaterMarkImage != "" && !this.Validate_WaterMarkPath())
		{
			this.vinfo.ErrorCode = 108;
			return this.vinfo;
		}
		if (this.OutputFileName == "")
		{
			this.vinfo.ErrorCode = 141;
			this.vinfo.ErrorMessage = "Output File Name must specifiy, e.g sample.avi or sample, extension will skip if specified.";
			return this.vinfo;
		}
		if (this.OutputExtension == "")
		{
			this.vinfo.ErrorCode = 140;
			this.vinfo.ErrorMessage = "Must specify output media type (OutputExtension)";
			return this.vinfo;
		}
		string fileName = string.Concat(this.InputPath, "\\", this.FileName);
		if (this.FileName.Contains("\\") || this.FileName.Contains("/"))
		{
			fileName = this.FileName;
		}
		this.vinfo = this.Get_Info();
		if (!this.OutputExtension.StartsWith("."))
		{
			this.OutputExtension = string.Concat(".", this.OutputExtension);
		}
		str = (!this.isMatch(this.OutputFileName, "\\.") ? string.Concat(this.OutputFileName, this.OutputExtension) : string.Concat(this.OutputFileName.Remove(this.OutputFileName.LastIndexOf(".")), this.OutputExtension));
		this.vinfo.FileName = str;
		if (this.Width % 2 == 1)
		{
			this.Width = this.Width + 1;
		}
		if (this.Height % 2 == 1)
		{
			this.Height = this.Height + 1;
		}
		string str1 = string.Concat("\"", this.FFMPEGPath, "\"");
		string str2 = string.Concat("\"", fileName, "\"");
		string[] outputPath = new string[] { "\"", this.OutputPath, "\\", str, "\"" };
		string str3 = string.Concat(outputPath);
		try
		{
			string str4 = this.Prepare_Command(str2, str3, false, true);
			this.ADVPROC(str1, str4);
			videoInfo = this.vinfo;
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			this.vinfo.ErrorCode = 121;
			this.vinfo.ErrorMessage = exception.Message;
			videoInfo = this.vinfo;
		}
		return videoInfo;
	}

	private bool Rename_Video(string oldpath, string newpath)
	{
		bool flag = false;
		if (!(new FileInfo(oldpath)).Exists)
		{
			flag = false;
		}
		else
		{
			File.Move(oldpath, newpath);
			flag = true;
		}
		return flag;
	}

	private string Return_Output_Name(string filename, string outputfilename, string format)
	{
		string str = format;
		if (this.OutputExtension == "")
		{
			str = (!format.Contains(".") ? string.Concat(".", format) : format);
		}
		else
		{
			str = (!this.OutputExtension.Contains(".") ? string.Concat(".", this.OutputExtension) : this.OutputExtension);
		}
		string str1 = "";
		if (outputfilename == "")
		{
			str1 = (!this.isMatch(filename, "\\.") ? string.Concat(filename, str) : string.Concat(filename.Remove(filename.LastIndexOf(".")), str));
		}
		else
		{
			str1 = (!this.isMatch(outputfilename, "\\.") ? string.Concat(outputfilename, str) : string.Concat(outputfilename.Remove(outputfilename.LastIndexOf(".")), str));
		}
		return str1;
	}

	private string Return_Thumb_Name(string filename, string outputfilename, string format)
	{
		string str = format;
		if (this.OutputExtension == "")
		{
			str = (!format.Contains(".") ? string.Concat(".", format) : format);
		}
		else
		{
			str = (!this.OutputExtension.Contains(".") ? string.Concat(".", this.OutputExtension) : this.OutputExtension);
		}
		string str1 = "";
		if (outputfilename == "")
		{
			str1 = (!this.isMatch(filename, "\\.") ? string.Concat(filename, str) : string.Concat(filename.Remove(filename.LastIndexOf(".")), str));
		}
		else
		{
			str1 = (!this.isMatch(outputfilename, "\\.") ? string.Concat(outputfilename, str) : string.Concat(outputfilename.Remove(outputfilename.LastIndexOf(".")), str));
		}
		return str1;
	}

	private int Return_Total_Seconds(string Duration)
	{
		TimeSpan zero = TimeSpan.Zero;
		TimeSpan.TryParse(Duration, out zero);
		return (int)zero.TotalSeconds;
	}

	public string Set_Buffering()
	{
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			return "129";
		}
		if (!this.Validate_FLVToolPath())
		{
			return "113";
		}
		if (!this.Validate_InputPath())
		{
			return "101";
		}
		string str = string.Concat("\"", this.FLVToolPath, "\"");
		string[] inputPath = new string[] { "\"", this.InputPath, "\\", this.FileName, "\"" };
		return this.Process_FLVTool_CMD(str, string.Concat(inputPath));
	}

	public string Set_MP4_Buffering()
	{
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			return "129";
		}
		if (!this.Validate_MP4BoxPath())
		{
			return "147";
		}
		string str = string.Concat("\"", this.MP4BoxPath, "\"");
		string[] inputPath = new string[] { "\"", this.InputPath, "\\", this.FileName, "\"" };
		string fileName = string.Concat(inputPath);
		if (this.FileName.StartsWith("http"))
		{
			fileName = this.FileName;
		}
		return this.Process_MP4Box_CMD(str, fileName);
	}

	private VideoInfo Split_Video(int length)
	{
		VideoInfo videoInfo;
		VideoInfo info = new VideoInfo();
		string str = "";
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			info.ErrorCode = 129;
			return info;
		}
		if (!this.Validate_FFMPEG())
		{
			info.ErrorCode = 100;
			return info;
		}
		if (!this.Validate_InputPath())
		{
			info.ErrorCode = 101;
			return info;
		}
		if (!this.Validate_OutputPath())
		{
			info.ErrorCode = 102;
			return info;
		}
		if (this.WaterMarkPath != "" && this.WaterMarkImage != "" && !this.Validate_WaterMarkPath())
		{
			info.ErrorCode = 108;
			return info;
		}
		if (this.OutputFileName == "")
		{
			info.ErrorCode = 141;
			info.ErrorMessage = "Split Video: Output File Name must specifiy, e.g sample.avi or sample, extension will skip if specified.";
			return info;
		}
		if (this.OutputExtension == "")
		{
			info.ErrorCode = 140;
			info.ErrorMessage = "Split Video: Must specify output media type (OutputExtension)";
			return info;
		}
		if (!this.OutputExtension.StartsWith("."))
		{
			this.OutputExtension = string.Concat(".", this.OutputExtension);
		}
		if (this.Width % 2 == 1)
		{
			this.Width = this.Width + 1;
		}
		if (this.Height % 2 == 1)
		{
			this.Height = this.Height + 1;
		}
		string str1 = string.Concat("\"", this.FFMPEGPath, "\"");
		string[] inputPath = new string[] { "\"", this.InputPath, "\\", this.FileName, "\"" };
		string str2 = string.Concat(inputPath);
		info = this.Get_Info();
		if (info.ErrorCode > 0)
		{
			return info;
		}
		int durationSec = info.Duration_Sec;
		int num = 1;
		if (length < durationSec)
		{
			num = Convert.ToInt32(Math.Floor((double)durationSec / (double)length));
		}
		try
		{
			int num1 = 0;
			string str3 = "";
			int num2 = 0;
			this.Start_Position = "0";
			this.Duration = length.ToString();
			if (num != 1)
			{
				num1 = 0;
				while (num1 <= num - 1)
				{
					str3 = (num1 >= 10 ? "" : "0");
					if (!this.isMatch(this.OutputFileName, "\\."))
					{
						object[] outputFileName = new object[] { this.OutputFileName, "_", str3, num1, this.OutputExtension };
						str = string.Concat(outputFileName);
					}
					else
					{
						object[] objArray = new object[] { this.OutputFileName.Remove(this.OutputFileName.LastIndexOf(".")), "_", str3, "_", num1, this.OutputExtension };
						str = string.Concat(objArray);
					}
					string[] outputPath = new string[] { "\"", this.OutputPath, "\\", str, "\"" };
					string str4 = string.Concat(outputPath);
					string str5 = this.Prepare_Command(str2, str4, false, true);
					info = this.PARSE_FFMPEG_OUTPUT(this.Process_CMD(str1, str5));
					if (info.ErrorCode <= 0)
					{
						num2 = num2 + length + 1;
						this.Start_Position = num2.ToString();
						num1++;
					}
					else
					{
						videoInfo = info;
						return videoInfo;
					}
				}
			}
			else
			{
				str = (!this.isMatch(this.OutputFileName, "\\.") ? string.Concat(this.OutputFileName, this.OutputExtension) : string.Concat(this.OutputFileName.Remove(this.OutputFileName.LastIndexOf(".")), this.OutputExtension));
				string[] strArrays = new string[] { "\"", this.OutputPath, "\\", str, "\"" };
				string str6 = string.Concat(strArrays);
				string str7 = this.Prepare_Command(str2, str6, false, true);
				info = this.PARSE_FFMPEG_OUTPUT(this.Process_CMD(str1, str7));
				if (info.ErrorCode > 0)
				{
					videoInfo = info;
					return videoInfo;
				}
			}
			info.FileName = this.OutputFileName;
			videoInfo = info;
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			info.ErrorCode = 121;
			info.ErrorMessage = exception.Message;
			videoInfo = info;
		}
		return videoInfo;
	}

	public VideoInfo Split_Video(int length, int total_clips)
	{
		VideoInfo videoInfo;
		VideoInfo info = new VideoInfo();
		string str = "";
		if (this._disable_license_validation == 0 && (DateTime.Now.Date > this._expirydate.Date || DateTime.Now.Date < this._lowerexpirydate.Date))
		{
			info.ErrorCode = 129;
			return info;
		}
		if (!this.Validate_FFMPEG())
		{
			info.ErrorCode = 100;
			return info;
		}
		if (!this.Validate_InputPath())
		{
			info.ErrorCode = 101;
			return info;
		}
		if (!this.Validate_OutputPath())
		{
			info.ErrorCode = 102;
			return info;
		}
		if (this.WaterMarkPath != "" && this.WaterMarkImage != "" && !this.Validate_WaterMarkPath())
		{
			info.ErrorCode = 108;
			return info;
		}
		if (this.OutputFileName == "")
		{
			info.ErrorCode = 141;
			info.ErrorMessage = "Split Video: Output File Name must specifiy, e.g sample.avi or sample, extension will skip if specified.";
			return info;
		}
		if (this.OutputExtension == "")
		{
			info.ErrorCode = 140;
			info.ErrorMessage = "Split Video: Must specify output media type (OutputExtension)";
			return info;
		}
		if (!this.OutputExtension.StartsWith("."))
		{
			this.OutputExtension = string.Concat(".", this.OutputExtension);
		}
		if (this.Width % 2 == 1)
		{
			this.Width = this.Width + 1;
		}
		if (this.Height % 2 == 1)
		{
			this.Height = this.Height + 1;
		}
		string str1 = string.Concat("\"", this.FFMPEGPath, "\"");
		string[] inputPath = new string[] { "\"", this.InputPath, "\\", this.FileName, "\"" };
		string str2 = string.Concat(inputPath);
		info = this.Get_Info();
		if (info.ErrorCode > 0)
		{
			return info;
		}
		int durationSec = info.Duration_Sec;
		int num = 1;
		if (length < durationSec)
		{
			num = Convert.ToInt32(Math.Floor((double)durationSec / (double)length));
		}
		if (total_clips > num)
		{
			info.ErrorCode = 122;
			info.ErrorMessage = "Total time of all clips plus clip time exceeds from original video time.";
			return info;
		}
		num = total_clips;
		try
		{
			int num1 = 0;
			string str3 = "";
			int num2 = 0;
			//this.Start_Position = "0";
			this.Duration = length.ToString();
			if (num != 1)
			{
				num1 = 0;
				while (num1 <= num - 1)
				{
					str3 = (num1 >= 10 ? "" : "0");
					if (!this.isMatch(this.OutputFileName, "\\."))
					{
						object[] outputFileName = new object[] { this.OutputFileName, "_", str3, num1, this.OutputExtension };
						str = string.Concat(outputFileName);
					}
					else
					{
						object[] objArray = new object[] { this.OutputFileName.Remove(this.OutputFileName.LastIndexOf(".")), "_", str3, "_", num1, this.OutputExtension };
						str = string.Concat(objArray);
					}
					string[] outputPath = new string[] { "\"", this.OutputPath, "\\", str, "\"" };
					string str4 = string.Concat(outputPath);
					string str5 = this.Prepare_Command(str2, str4, false, true);
					info = this.PARSE_FFMPEG_OUTPUT(this.Process_CMD(str1, str5));
					if (info.ErrorCode <= 0)
					{
						num2 = num2 + length + 1;
						this.Start_Position = num2.ToString();
						num1++;
					}
					else
					{
						videoInfo = info;
						return videoInfo;
					}
				}
			}
			else
			{
				str = (!this.isMatch(this.OutputFileName, "\\.") ? string.Concat(this.OutputFileName, this.OutputExtension) : string.Concat(this.OutputFileName.Remove(this.OutputFileName.LastIndexOf(".")), this.OutputExtension));
				string[] strArrays = new string[] { "\"", this.OutputPath, "\\", str, "\"" };
				string str6 = string.Concat(strArrays);
				string str7 = this.Prepare_Command(str2, str6, false, true);
				info = this.PARSE_FFMPEG_OUTPUT(this.Process_CMD(str1, str7));
				if (info.ErrorCode > 0)
				{
					videoInfo = info;
					return videoInfo;
				}
			}
			info.FileName = this.OutputFileName;
			videoInfo = info;
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			info.ErrorCode = 121;
			info.ErrorMessage = exception.Message;
			videoInfo = info;
		}
		return videoInfo;
	}

	private bool Validate_FFMPEG()
	{
		bool flag = false;
		if (this.FFMPEGPath != "")
		{
			flag = (File.Exists(this.FFMPEGPath) ? true : false);
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	private bool Validate_FLVToolPath()
	{
		bool flag = false;
		if (this.FLVToolPath != "")
		{
			flag = (File.Exists(this.FLVToolPath) ? true : false);
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	private bool Validate_InputPath()
	{
		bool flag = false;
		if (this.FileName.StartsWith("http"))
		{
			return true;
		}
		if (this.InputPath == "")
		{
			flag = false;
		}
		else if (!this.FileName.Contains("%"))
		{
			flag = (File.Exists(string.Concat(this.InputPath, "\\", this.FileName)) ? true : false);
		}
		else
		{
			flag = true;
		}
		return flag;
	}

	private bool Validate_MP4BoxPath()
	{
		bool flag = false;
		if (this.MP4BoxPath != "")
		{
			flag = (File.Exists(this.MP4BoxPath) ? true : false);
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	private bool Validate_OutputPath()
	{
		bool flag = false;
		if (this.OutputPath != "")
		{
			flag = (Directory.Exists(this.OutputPath) ? true : false);
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	private bool Validate_PresetPath()
	{
		bool flag = false;
		if (this.PresetPath != "")
		{
			flag = (File.Exists(this.PresetPath) ? true : false);
		}
		else
		{
			flag = true;
		}
		return flag;
	}

	private bool Validate_Video(string output)
	{
		if (this.isMatch(output, "Unknown Format"))
		{
			return false;
		}
		return true;
	}

	private bool Validate_WaterMarkPath()
	{
		bool flag = false;
		if (this.WaterMarkPath != "")
		{
			flag = (File.Exists(string.Concat(this.WaterMarkPath, "\\", this.WaterMarkImage)) ? true : false);
		}
		else
		{
			flag = false;
		}
		return flag;
	}
}