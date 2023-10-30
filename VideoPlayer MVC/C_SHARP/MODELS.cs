using MediaInfo;
using System.Diagnostics;

namespace VideoPlayer_MVC.C_SHARP
{
	public class MODELS
	{
		public static List<string> AllVideosAuthors = new List<string>();
		public static List<MODELS.Video> AllVideos = new List<MODELS.Video>();
		public static List<MODELS.Video> SearchResult = new List<MODELS.Video>();

		public static int ID = 0;


		public static int InstanceID;
		public static Stopwatch StartTime { get; internal set; } = new Stopwatch();
		public static bool AppStartsInit { get; internal set; } = false;
		public static bool AppStarts { get; set; } = false;

		public static string webRootPath { get; set; } = "";
		public static Task? Reindex { get; set; }
		public static Task<MyPerformanceMonitor.Program.SystemReport?>? Monitor { get; set; }
		public static MyPerformanceMonitor.Program.SystemReport? MonitorResult { get; set; }
		

		public static List<string?> AllVideosAspectRatio = new List<string?>();
		public static List<string?> AllVideosFormat = new List<string?>();
		public static List<string?> AllVideosFormatVersion = new List<string?>();
		public static List<int?> AllVideosFramerate = new List<int?>();
		public static List<TimeSpan?> AllVideosDuration = new List<TimeSpan?>();
		public static List<TimeSpan?> AllVideosDurationMinuts = new List<TimeSpan?>();
		public static List<string?> AllVideosAudioCodec = new List<string?>();
		public static List<int?> AllVideosAudioChannels = new List<int?>();
		public static List<string?> AllVideosVideoResolution = new List<string?>();

		/// <summary>
		/// Počet obrázků na stránku
		/// </summary>
		public static string? VideoContain = null;
		public static DateTime? DateMin = null;
		public static DateTime? DateMax = null;
		public static string? Autor = null;
		public static string? AspectRatio = null;
		public static string? Format = null;
		public static string? FormatVersion = null;
		public static int? Framerate = null;
		public static TimeSpan? Duration = null;
		public static string? DurationID { get; set; }
		public static string? AudioCodec = null;
		public static int? AudioChannels = null;
		public static string? VideoResolution = null;

		public static int ImageGrouping = 6;

		public static int ImageRows = 2;
		public class Video
		{
			public string? URL { get; set; }
			public string? thumbnail { get; set; }
			public FileInfo? FileInfo { get; set; }
			public DateTime? DateTime { get; set; }
			public string? Folder { get; set; }
			public string? FullPath { get; set; }
			public MediaInfoWrapper? mediaInfo { get; set; }

			public override string? ToString()
			{
				return URL ?? string.Empty;
			}
		}

		public class Search
		{			
			public DateTime DateMin { get; set; }
			public DateTime DateMax { get; set; }
			public string? VideoContain { get; set; }
			public string? Autor { get; set; }
			public MediaInfoWrapper? mediaInfo { get; set; }
		}

		public class Login
		{
			public string? Page { get; set; }
			public string? Username { get; set; }
			public string? Password { get; set; }
		}
	}
}
