public class VideoInfo
{
	private string _start = "";

	private string _exception = "";

	private string _ffmpegoutput = "";

	private int _errorcode;

	private string _filename = "";

	private int _hours;

	private int _minutes;

	private int _seconds;

	private int _duration_sec;

	private string _duration = "";

	private string start_position = "";

	private int _input_width;

	private int _input_height;

	private int _width;

	private int _height;

	private string _input_type = "";

	private string _output_type = "";

	private string _output_audio_codec = "";

	private string _output_audio_samplingrate = "";

	private string _output_audio_channel = "";

	private string _output_audio_bitrate = "";

	private string _input_audio_codec = "";

	private string _input_audio_samplingrate = "";

	private string _input_audio_channel = "";

	private string _input_audio_bitrate = "";

	private string _output_video_codec = "";

	private string _output_video_bitrate = "";

	private string _output_video_framerate = "";

	private string _input_video_codec = "";

	private string _input_video_bitrate = "";

	private string _input_video_framerate = "";

	private string _title = "";

	private string _producer = "";

	private string _footage = "";

	private string _music = "";

	private long _processedsize;

	private int _processedtime;

	private long _totalsize;

	private double _processingleft = 100;

	private double _processingcompleted;

	public string Acodec
	{
		get
		{
			return this._output_audio_codec;
		}
		set
		{
			this._output_audio_codec = value;
		}
	}

	public string Audio_Bitrate
	{
		get
		{
			return this._output_audio_bitrate;
		}
		set
		{
			this._output_audio_bitrate = value;
		}
	}

	public string Channel
	{
		get
		{
			return this._output_audio_channel;
		}
		set
		{
			this._output_audio_channel = value;
		}
	}

	public string Duration
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

	public int Duration_Sec
	{
		get
		{
			return this._duration_sec;
		}
		set
		{
			this._duration_sec = value;
		}
	}

	public int ErrorCode
	{
		get
		{
			return this._errorcode;
		}
		set
		{
			this._errorcode = value;
		}
	}

	public string ErrorMessage
	{
		get
		{
			return this._exception;
		}
		set
		{
			this._exception = value;
		}
	}

	public string FFMPEGOutput
	{
		get
		{
			return this._ffmpegoutput;
		}
		set
		{
			this._ffmpegoutput = value;
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

	public string Footage
	{
		get
		{
			return this._footage;
		}
		set
		{
			this._footage = value;
		}
	}

	public string FrameRate
	{
		get
		{
			return this._output_video_framerate;
		}
		set
		{
			this._output_video_framerate = value;
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

	public int Hours
	{
		get
		{
			return this._hours;
		}
		set
		{
			this._hours = value;
		}
	}

	public string Input_Acodec
	{
		get
		{
			return this._input_audio_codec;
		}
		set
		{
			this._input_audio_codec = value;
		}
	}

	public string Input_Audio_Bitrate
	{
		get
		{
			return this._input_audio_bitrate;
		}
		set
		{
			this._input_audio_bitrate = value;
		}
	}

	public string Input_Channel
	{
		get
		{
			return this._input_audio_channel;
		}
		set
		{
			this._input_audio_channel = value;
		}
	}

	public string Input_FrameRate
	{
		get
		{
			return this._input_video_framerate;
		}
		set
		{
			this._input_video_framerate = value;
		}
	}

	public int Input_Height
	{
		get
		{
			return this._input_height;
		}
		set
		{
			this._input_height = value;
		}
	}

	public string Input_SamplingRate
	{
		get
		{
			return this._input_audio_samplingrate;
		}
		set
		{
			this._input_audio_samplingrate = value;
		}
	}

	public string Input_Type
	{
		get
		{
			return this._input_type;
		}
		set
		{
			this._input_type = value;
		}
	}

	public string Input_Vcodec
	{
		get
		{
			return this._input_video_codec;
		}
		set
		{
			this._input_video_codec = value;
		}
	}

	public string Input_Video_Bitrate
	{
		get
		{
			return this._input_video_bitrate;
		}
		set
		{
			this._input_video_bitrate = value;
		}
	}

	public int Input_Width
	{
		get
		{
			return this._input_width;
		}
		set
		{
			this._input_width = value;
		}
	}

	public int Minutes
	{
		get
		{
			return this._minutes;
		}
		set
		{
			this._minutes = value;
		}
	}

	public string Music
	{
		get
		{
			return this._music;
		}
		set
		{
			this._music = value;
		}
	}

	public long ProcessedSize
	{
		get
		{
			return this._processedsize;
		}
		set
		{
			this._processedsize = value;
		}
	}

	public int ProcessedTime
	{
		get
		{
			return this._processedtime;
		}
		set
		{
			this._processedtime = value;
		}
	}

	public double ProcessingCompleted
	{
		get
		{
			return this._processingcompleted;
		}
		set
		{
			this._processingcompleted = value;
		}
	}

	public double ProcessingLeft
	{
		get
		{
			return this._processingleft;
		}
		set
		{
			this._processingleft = value;
		}
	}

	public string Producer
	{
		get
		{
			return this._producer;
		}
		set
		{
			this._producer = value;
		}
	}

	public string SamplingRate
	{
		get
		{
			return this._output_audio_samplingrate;
		}
		set
		{
			this._output_audio_samplingrate = value;
		}
	}

	public int Seconds
	{
		get
		{
			return this._seconds;
		}
		set
		{
			this._seconds = value;
		}
	}

	public string Start
	{
		get
		{
			return this._start;
		}
		set
		{
			this._start = value;
		}
	}

	public string Title
	{
		get
		{
			return this._title;
		}
		set
		{
			this._title = value;
		}
	}

	public long TotalSize
	{
		get
		{
			return this._totalsize;
		}
		set
		{
			this._totalsize = value;
		}
	}

	public string Type
	{
		get
		{
			return this._output_type;
		}
		set
		{
			this._output_type = value;
		}
	}

	public string Vcodec
	{
		get
		{
			return this._output_video_codec;
		}
		set
		{
			this._output_video_codec = value;
		}
	}

	public string Video_Bitrate
	{
		get
		{
			return this._output_video_bitrate;
		}
		set
		{
			this._output_video_bitrate = value;
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

	public VideoInfo()
	{
	}
}