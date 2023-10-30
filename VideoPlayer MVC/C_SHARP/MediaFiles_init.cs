using MediaInfo;
using System.Diagnostics;
using System.Linq;
using System.Resources;

namespace VideoPlayer_MVC.C_SHARP
{
	public class MediaFiles_init
	{
		public static async Task Init()
		{
			SemaphoreSlim semaphoreSlim = new SemaphoreSlim(Environment.ProcessorCount, Environment.ProcessorCount);
			MODELS.AppStartsInit = true;
			Console.WriteLine("Media Start init");

			MODELS.AllVideosAuthors.Add("Hledat vše");
			MODELS.AllVideosAuthors.Add("Neurčeno");
			MODELS.AllVideos = new List<MODELS.Video>();

			List<Task> tasks = new List<Task>();
			try
			{
				var VideoPath = Path.Combine(VideoPlayer_MVC.C_SHARP.MODELS.webRootPath, "video"); Console.WriteLine("VideoPath is " + VideoPath);
				foreach (var USERS in Directory.GetDirectories(VideoPath))
				{
					Console.WriteLine("USER is " + USERS);
					foreach (var YEARS in Directory.GetDirectories(USERS))
					{
						Console.WriteLine("YEAR is " + YEARS);
						foreach (var MEDIAFILE in Directory.GetFiles(YEARS))
						{
							tasks.Add(Task.Run(async () =>
							{
								await semaphoreSlim.WaitAsync();
								try
								{
									Console.WriteLine($"MEDIAFILE is {MEDIAFILE}");
									await MakeThumbnail(MEDIAFILE, VideoPlayer_MVC.C_SHARP.MODELS.webRootPath, 280);

									await Register_media_file(MEDIAFILE, VideoPlayer_MVC.C_SHARP.MODELS.webRootPath, USERS);
								}
								catch { }
								semaphoreSlim.Release();
							}));

						}
					}
				}
			}
			catch
			{
			}

			await Task.WhenAll(tasks);

			foreach (TimeSpan? item in MODELS.AllVideosDuration)
			{
				int Minutes = (int)item.GetValueOrDefault().TotalMinutes;
				MODELS.AllVideosDurationMinuts.Add(TimeSpan.FromMinutes(Minutes));
			}

			///seřazení
			MODELS.AllVideosAuthors = MODELS.AllVideosAuthors.Distinct().Order().ToList();
			MODELS.AllVideosAspectRatio = MODELS.AllVideosAspectRatio.Distinct().Order().ToList();
			MODELS.AllVideosFormat = MODELS.AllVideosFormat.Distinct().Order().ToList();
			MODELS.AllVideosFormatVersion = MODELS.AllVideosFormatVersion.Distinct().Order().ToList();
			MODELS.AllVideosFramerate = MODELS.AllVideosFramerate.Distinct().Order().ToList();
			MODELS.AllVideosDuration = MODELS.AllVideosDuration.Distinct().Order().ToList();
			MODELS.AllVideosDurationMinuts = MODELS.AllVideosDurationMinuts.Distinct().Order().ToList();
			MODELS.AllVideosAudioCodec = MODELS.AllVideosAudioCodec.Distinct().Order().ToList();
			MODELS.AllVideosAudioChannels = MODELS.AllVideosAudioChannels.Distinct().Order().ToList();
			MODELS.AllVideosVideoResolution = MODELS.AllVideosVideoResolution.Distinct().Order().ToList();


			//přidání null hodnot pro vymazání hledání
			MODELS.AllVideosAspectRatio.Add(null);
			MODELS.AllVideosFormat.Add(null);
			MODELS.AllVideosFormatVersion.Add(null);
			MODELS.AllVideosFramerate.Add(null);
			MODELS.AllVideosDuration.Add(null);
			MODELS.AllVideosDurationMinuts.Add(null);
			MODELS.AllVideosAudioCodec.Add(null);
			MODELS.AllVideosAudioChannels.Add(null);
			MODELS.AllVideosVideoResolution.Add(null);

			EntityFramework_Media.EF.BDContext bDContext = new EntityFramework_Media.EF.BDContext();
			Console.WriteLine("BDContext");
			int ID = 0;
			foreach (var item in MODELS.AllVideos)
			{
				try
				{
					Console.WriteLine("BDContext - adding " + item?.URL);
					ID++;
					bDContext.VIDEOTABLE.Add(new EntityFramework_Media.EF.VIDEOTABLE { ID = ID, DATETIME = item?.DateTime, FILE_INFO_NAME = item?.FileInfo?.Name, FOLDER = item?.Folder, FULLPATH_THUMBAIL = item?.thumbnail, FULLPATH_VIDEO = item?.FullPath });
					Console.WriteLine("BDContext - added " + item?.URL);
				}
				catch (Exception ex) 
				{
					Console.WriteLine(ex.Message);
				}
			}

			try { 
			await bDContext.SaveChangesAsync();
				Console.WriteLine("saved");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.InnerException);
			}

			MODELS.SearchResult = MODELS.AllVideos;
			MODELS.AppStarts = true;
		}

		private static async Task<Task> MakeThumbnail(string MEDIAFILE, string webRootPath, int ThumbnailResolution)
		{
			Console.WriteLine("\n\nMakeThumbnail");
			if (MEDIAFILE.EndsWith("mp4"))
			{
				string MEDIAFILE_for_bat = await Vlozit_do_uvozovek(MEDIAFILE);
				string Thumbnail_for_bat = await Vlozit_do_uvozovek($"{MEDIAFILE.Replace(".mp4", "")}.png");
				FileInfo ThumbnailFile = new FileInfo($"{MEDIAFILE.Replace(".mp4", "")}.png");
				Console.WriteLine($"ThumbnailFile {ThumbnailFile} is exist: {ThumbnailFile.Exists}");

				if (!ThumbnailFile.Exists)
				{
					string args = " -n -i " + MEDIAFILE_for_bat + " -frames:v 1 -filter:v scale=\"" + ThumbnailResolution + ":-1\" " + Thumbnail_for_bat;
					string FFmpegPath = Path.Combine(webRootPath, "ffmpeg-2023-08-14-git-c704901324-full_build", "bin", "ffmpeg");
					Console.WriteLine($"{FFmpegPath}{args}");
					try
					{
						var FFmpeg = Process.Start(FFmpegPath, args);
						FFmpeg.WaitForExitAsync().Wait(15000);
						FFmpeg.Close();
					}
					catch { }
				}
			}

			return Task.CompletedTask;
		}

		private static Task<string> Vlozit_do_uvozovek(string STRING)
		{
			return Task.FromResult($"\"{STRING}\"");
		}

		private static async Task Register_media_file(string MEDIAFILE, string webRootPath, string USERS)
		{
			if (!MEDIAFILE.EndsWith("jpg") && !MEDIAFILE.EndsWith("png"))
			{
				FileInfo File = new FileInfo(MEDIAFILE);
				string FolderName = USERS.Replace(webRootPath, "").Replace("\\video\\", "");
				Console.WriteLine($"FolderName is {FolderName}");

				string URL = MEDIAFILE.Replace(webRootPath, "").Replace("\\", "/");
				MODELS.AllVideos.Add(new MODELS.Video
				{
					mediaInfo = await MediaInfo(MEDIAFILE),
					FullPath = USERS,
					Folder = FolderName,
					URL = URL,
					thumbnail = MEDIAFILE.Replace(webRootPath, "").Replace("\\", "/").Replace("mp4", "png"),
					FileInfo = File,
					DateTime = (await VideoDate(File.Name)).Item1
				});

				MODELS.AllVideosAuthors.Add(FolderName);
			}
		}
		private static async Task<MediaInfoWrapper?> MediaInfo(string MEDIAFILE)
		{
			Exception? exception = null;
			MediaInfoWrapper? mediaInfo = null;
			try
			{
				mediaInfo = new MediaInfoWrapper(MEDIAFILE);

				MODELS.AllVideosAspectRatio.Add(mediaInfo.AspectRatio);
				MODELS.AllVideosFormat.Add(mediaInfo.Format);
				MODELS.AllVideosFormatVersion.Add(mediaInfo.VideoCodec);
				MODELS.AllVideosFramerate.Add((int?)mediaInfo.Framerate);
				MODELS.AllVideosDuration.Add(TimeSpan.FromMilliseconds(mediaInfo.Duration));
				MODELS.AllVideosAudioCodec.Add(mediaInfo.AudioCodec);
				MODELS.AllVideosAudioChannels.Add(mediaInfo.AudioChannels);
				MODELS.AllVideosVideoResolution.Add(mediaInfo.VideoResolution);
			}
			catch (Exception xx)
			{
				exception = xx;
			}
			Console.WriteLine($"MediaInfo for {MEDIAFILE} is {mediaInfo?.VideoCodec}{exception?.Message}");

			return await Task.FromResult(mediaInfo);
		}

		private static async Task<(DateTime?, Exception?)> VideoDate(string name)
		{
			try
			{
				DateTime? dateTime = await GetDates_from_filename.Run(name);
				return (dateTime, null);
			}
			catch (Exception xx)
			{
				return (null, xx);
			}
		}
	}
}
