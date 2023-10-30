using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Reflection.Emit;
using VideoPlayer_MVC.C_SHARP;
using VideoPlayer_MVC.Models;
using static VideoPlayer_MVC.C_SHARP.MODELS;

namespace VideoPlayer_MVC.Controllers
{
	public class HomeController : Controller
	{

		public IActionResult StartupWait()
		{
			ServerMonitor();

			if (MODELS.AppStartsInit == false && MODELS.AllVideos.Count == 0)
			{
				_ = MediaFiles_init.Init();
			}

			if (AppStarts)
			{
				return RedirectToAction("List", "Home", "");
			}

			return View();
		}

		public IActionResult ServerMonitor()
		{
			if (MODELS.Monitor is not null)
			{
				if (MODELS.Monitor.Status == TaskStatus.RanToCompletion)
				{
					MODELS.MonitorResult = MODELS.Monitor.Result;
					MODELS.Monitor = null;
				}
			}
			else
			{
				MODELS.Monitor = MyPerformanceMonitor.Program.ServerMonitorAsync();
				MODELS.Monitor.Wait(1);
			}

			return View();
		}

		public IActionResult Login(string Page = "logon")
		{
			ViewBag.Action = Page;
			return View();
		}

		[HttpPost]
		public IActionResult Login(Login login)
		{
			ViewBag.Action = login.Page;
			if (login == null)
			{
				ViewBag.SpravnostHesla = "Přihlaste se prosím";
				return View();
			}
			else
			{
				if (login.Username == "Tomas" && login.Password == "PLMnko123")
				{
					ViewBag.SpravnostHesla = "Přihlásili jste se";
					return RedirectToAction(login?.Page, "Home", $"ID={MODELS.InstanceID}");
				}
				else
				{
					ViewBag.SpravnostHesla = "Nesprávné jméno nebo heslo";
					return View();
				}
			}
		}

		public IActionResult ReindexAsync()
		{
			if (Reindex == null)
			{
				ViewBag.ReindexStatus = "Spouštím";
				Reindex = MediaFiles_init.Init();
				Reindex.Wait(1);
			}
			else
			{
				ViewBag.ReindexStatus = Reindex.Status;

				if (Reindex.Status == TaskStatus.RanToCompletion)
				{
					Reindex = null;
					return RedirectToAction("List", "Home", "");
				}
			}
			return View();
		}

		public IActionResult VypnoutServer()
		{
			try
			{
				ExitWindowsEx.Execute.Hybrid();
				ViewBag.Restart = "Hybrid";
			}
			catch (Exception ex)
			{
				try
				{
					ExitWindowsEx.Execute.Shutdown();
					ViewBag.Restart = "Shutdown " + ex.Message;
				}
				catch (Exception ex2)
				{
					ViewBag.Restart = ex2.Message;
				}
			}

			return View();
		}
		public IActionResult RestartIIS()
		{
			try
			{
				ExitWindowsEx.Execute.Reboot();
				ViewBag.Restart = "Reboot";
			}
			catch (Exception ex)
			{
				ViewBag.Restart = ex.Message;
			}

			return View();
		}

		public IActionResult State_of_DateTask()
		{
			ViewBag.State_of_DateTasks = GetDates_from_filename.ParseDate.parseDates;

			return View();
		}

		public IActionResult List(string? VideoContain, int? ImageGrouping, int? ImageRows, DateTime? DateMin = null, DateTime? DateMax = null, string? Autor = null, string? ar = null, string? formatvid = null,string? formatvid_verz = null, int? fps = null, TimeSpan? lenght = null,string? lenghtID = null, string? audio_format = null, int? audio_channels = null, string? video_res = null, int ID_QUERY = 0)
		{
			VideoContain ??= MODELS.VideoContain;
			DateMin ??= MODELS.DateMin;
			DateMax ??= MODELS.DateMax;
			ImageGrouping ??= MODELS.ImageGrouping;
			ImageRows ??= MODELS.ImageRows;
			Autor ??= VideoPlayer_MVC.C_SHARP.MODELS.Autor;

			MODELS.VideoContain = VideoContain;
			MODELS.DateMin = DateMin;
			MODELS.DateMax = DateMax;
			MODELS.Autor = Autor;
			MODELS.AspectRatio = ar;
			MODELS.Format = formatvid;
			MODELS.FormatVersion = formatvid_verz;
			MODELS.Framerate = fps;
			MODELS.Duration = lenght;
			MODELS.DurationID = lenghtID;
			MODELS.AudioCodec = audio_format;
			MODELS.AudioChannels = audio_channels;
			MODELS.VideoResolution = video_res;

			VideoPlayer_MVC.C_SHARP.MODELS.Autor = Autor;
			MODELS.ImageGrouping = ImageGrouping ?? 6;
			MODELS.ImageRows = ImageRows ?? 2;

			ViewBag.ID_QUERY = ID_QUERY;

			string Hledám = "";
			var Search = AllVideos;
			if (DateMin != null && DateMin > DateTime.MinValue)
			{
				MODELS.DateMin = DateMin;
				Search = Search.Where(ee => ee.DateTime >= DateMin).ToList();
				Hledám += $"starší než: {DateMin}  ";
				SearchResult = Search;
			}
			if (DateMax != null && DateMax > DateTime.MinValue)
			{
				MODELS.DateMax = DateMax;
				Search = Search.Where(ee => ee.DateTime < DateMax).ToList();
				Hledám += $"mladší než: {DateMax}  ";
				SearchResult = Search;
			}
			if (VideoContain != null && VideoContain.Length > 0)
			{
				MODELS.VideoContain = VideoContain;
				Search = Search.Where(ee => ee.URL?.Contains(VideoContain ?? "") ?? true).ToList();
				Hledám += $"název obsahuje: {VideoContain}  ";
				SearchResult = Search;
			}
			if (ar != null)
			{
				Search = Search.Where(ee => ee.mediaInfo?.AspectRatio == ar).ToList();
				Hledám += $"název obsahuje: {ar}  ";
				SearchResult = Search;
			}
			
			if (formatvid != null)
			{
				Search = Search.Where(ee => ee.mediaInfo?.Format == formatvid).ToList();
				Hledám += $"název obsahuje: {formatvid}  ";
				SearchResult = Search;
			}
			if (formatvid_verz != null)
			{
				Search = Search.Where(ee => ee.mediaInfo?.VideoCodec == formatvid_verz).ToList();
				Hledám += $"název obsahuje: {formatvid_verz}  ";
				SearchResult = Search;
			}
			if (fps != null)
			{
				Search = Search.Where(ee => ee.mediaInfo?.Framerate == fps).ToList();
				Hledám += $"název obsahuje: {fps}  ";
				SearchResult = Search;
			}
			if (lenght != null && lenghtID!=null)
			{
				switch (lenghtID)
				{
					case "Delsi_a_rovno_nez_zvolene":
						Search = Search.Where(ee => TimeSpan.FromMilliseconds((double)(ee.mediaInfo?.Duration ?? 0)) >= lenght).ToList();
						Hledám += $"název obsahuje: {lenght}  ";
						SearchResult = Search;
						break;

					case "Delsi_nez_zvolene":
						Search = Search.Where(ee => TimeSpan.FromMilliseconds((double)(ee.mediaInfo?.Duration ?? 0)) > lenght).ToList();
						Hledám += $"název obsahuje: {lenght}  ";
						SearchResult = Search;
						break;

					case "Kratsi_a_rovno_nez_zvolene":
						Search = Search.Where(ee => TimeSpan.FromMilliseconds((double)(ee.mediaInfo?.Duration ?? 0)) <= lenght).ToList();
						Hledám += $"název obsahuje: {lenght}  ";
						SearchResult = Search;
						break;

					case "Kratsi_nez_zvolene":
						Search = Search.Where(ee => TimeSpan.FromMilliseconds((double)(ee.mediaInfo?.Duration ?? 0)) < lenght).ToList();
						Hledám += $"název obsahuje: {lenght}  ";
						SearchResult = Search;
						break;
				}
			}
			if (audio_format != null)
			{
				Search = Search.Where(ee => ee.mediaInfo?.AudioCodec == audio_format).ToList();
				Hledám += $"název obsahuje: {audio_format}  ";
				SearchResult = Search;
			}
			if (audio_channels != null)
			{
				Search = Search.Where(ee => ee.mediaInfo?.AudioChannels == audio_channels).ToList();
				Hledám += $"název obsahuje: {audio_channels}  ";
				SearchResult = Search;
			}
			if (video_res != null)
			{
				Search = Search.Where(ee => ee.mediaInfo?.VideoResolution == video_res).ToList();
				Hledám += $"název obsahuje: {video_res}  ";
				SearchResult = Search;
			}
			switch (Autor)
			{
				case not null and "Neurčeno":
					{
						Search = Search.Where(ee => ee.Folder == "").ToList();
						Hledám += $"složka je: {Autor}  ";
						SearchResult = Search;
						break;
					}

				case not null and not "Neurčeno" and not "Hledat vše":
					{
						Search = Search.Where(ee => ee.Folder == Autor).ToList();
						Hledám += $"složka je: {Autor}  ";
						SearchResult = Search;
						break;
					}
			}

			SearchResult = SearchResult.OrderBy(dd => dd.DateTime).ToList();
			ViewBag.ImageGrouping = 100 / (ImageGrouping / ImageRows);
			var xyz = SearchResult.Select((e, i) => new { Item = e, Grouping = i / ImageGrouping }).GroupBy(e => e.Grouping);
			var Indexes = xyz.Select((f, d) => new { Item = f, Grouping = d / 13 }).GroupBy(w => w.Grouping);

			if (xyz.Any())
			{
				ViewBag.ZadnaVideo = Hledám == "" ? $"počet: {SearchResult.Count()}" : $"Výsledky hledání: {Hledám}  počet: {SearchResult.Count()}" + "skupin: " + Indexes.Count();

				List<List<(int, string)>> uls = new List<List<(int, string)>>();
				int ID = 0;
				for (int i = 0; i < Indexes.Count(); i++)
				{
					List<(int, string)> row = new List<(int, string)>();
					foreach (var item1 in Indexes.ElementAt(i))
					{
						var PageDateMin = item1?.Item?.FirstOrDefault()?.Item.DateTime;
						var PageDateMax = item1?.Item?.LastOrDefault()?.Item.DateTime;

						string Dates = "";
						if (PageDateMin.GetValueOrDefault() - PageDateMin.GetValueOrDefault().Date == new TimeSpan(0, 0, 0))
						{
							Dates += PageDateMin.GetValueOrDefault().ToShortDateString();
						}
						else
						{
							var dd = Dates.Length;
							Dates += PageDateMin.GetValueOrDefault().ToString();
						}
						Dates += " - ";
						if (PageDateMax.GetValueOrDefault() - PageDateMax.GetValueOrDefault().Date == new TimeSpan(0, 0, 0))
						{
							Dates += PageDateMax.GetValueOrDefault().ToShortDateString();
						}
						else
						{
							Dates += PageDateMax.GetValueOrDefault().ToString();
						}

						row.Add((ID++, Dates));
					}
					uls.Add(row);
				}
				ViewBag.uls_ = uls;

				ViewBag.SearchRes = xyz.ElementAt(ID_QUERY);
			}
			else
			{
				ViewBag.ZadnaVideo = $"Výsledky hledání: \n{Hledám}Nenalezena žádná videa!\n upravte filtr";
				ViewBag.SearchRes = null;
				ViewBag.uls_ = new List<List<(int, string)>>();
			}

			return View();
		}

		[HttpGet]
		public IActionResult VideoPlay(bool next = false, bool prev = false, string? video = null)
		{
			if (next)
			{
				ID++;

				if (ID >= SearchResult.Count)
				{
					ID = 0;
					return RedirectToAction("List", "Home", "");
				}
			}

			if (prev)
			{
				ID--;

				if (ID < 0)
				{
					ID = 0;
					return RedirectToAction("List", "Home", "");
				}
			}

			MODELS.Video? _video = null;
			if (video == null)
			{
				_video = SearchResult[ID];
			}
			else
			{
				_video = SearchResult.FirstOrDefault(ee => ee.URL == video);

				if (_video == null)
				{
					ID = 0;
					return RedirectToAction("List", "Home", "");
				}
				ID = SearchResult.IndexOf(_video);
			}

			ViewBag.Video = _video.URL;

			bool Is_available_Time = (_video.DateTime - _video.DateTime.GetValueOrDefault().Date) == new TimeSpan(0, 0, 0);
			ViewBag.ARG = Is_available_Time ? _video.DateTime.GetValueOrDefault().ToShortDateString() : _video.DateTime.ToString();

			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}