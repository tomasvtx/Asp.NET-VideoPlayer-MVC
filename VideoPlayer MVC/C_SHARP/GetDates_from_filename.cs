using System.Globalization;
using static VideoPlayer_MVC.C_SHARP.GetDates_from_filename;

namespace VideoPlayer_MVC.C_SHARP
{
	public class GetDates_from_filename
	{
		public static async Task<DateTime?> Run(string StringName)
		{
			var Array = StringName.Split('/');

			DateTime? dateTime = null;
			try
			{
				foreach (ParseDate parseDate in ParseDate.parseDates)
				{
					parseDate.date = Array?.LastOrDefault();
					DateTime? result = await ParseDate.RUN(parseDate);
					if (result != null)
					{
						return await Task.FromResult(result);
					}
				}
			}
			catch (Exception dd) { throw dd; }

			return await Task.FromResult(dateTime);
		}

		public class ParseDate
		{
			public int Count_of_Starts { get; set; } = 0;
			public int Count_of_Successfully_use { get; set; } = 0;
			public int Count_of_Error { get; set; } = 0;
			public Exception? LastError { get; set; }
			public string? Name { get; set; }
			public string? date { get; set; }
			public int SubstringStart { get; set; }
			public int Substring { get; set; }
			public string format { get; set; } = "";
			public string lastimage { get; set; } = "";

			public static Task<DateTime?> RUN(ParseDate parseDate)
			{
				parseDate.Count_of_Starts++;
				IFormatProvider provider = new CultureInfo("cs-CZ");
				DateTime? dateTime = null;
				try
				{
					///04. Srpna 2008 15-00-20_2.25x_1440x1080_ahq-11.mp4
					dateTime = DateTime.ParseExact(parseDate.date?.Substring(parseDate.SubstringStart, parseDate.Substring) ?? "", parseDate.format, provider);
					parseDate.Count_of_Successfully_use++;
					parseDate.lastimage = parseDate?.date;
				}
				catch (Exception ee)
				{
					parseDate.Count_of_Error++;
					parseDate.LastError = ee;
				}

				return Task.FromResult(dateTime);
			}

			public static List<ParseDate> parseDates = new List<ParseDate>
					{
			new ParseDate
						  {
							Name = "S6300457 02.10. 2008 17-53-02_2.25x_1440x1080_amq-13.mp4",
							format = "dd.MM.  yyyy  HH-mm-ss",
							SubstringStart = 9,
							Substring = 22
						  },
						new ParseDate
						  {
							Name = "S6300646_10. rijna 09-09-54_2.25x_1440x1080_ahq-12.mp4",
							format = "yyyydd. MMMM  HH-mm-ss",
							SubstringStart = 9,
							Substring = 19
						  },
						new ParseDate
						  {
							Name = "20161126-Vid 20161126 142216",
							format = "yyyyMMdd HHmmss",
							SubstringStart = 13,
							Substring = 15
						  },
						new ParseDate
						  {
							Name = "20150614-V_20150614_163136_1.mp4",
							format = "yyyyMMdd_HHmmss",
							SubstringStart = 11,
							Substring = 15
						  },
						new ParseDate
						  {
							Name = "2009-03-13 08-38-16_2.25x_1440x1080_ahq-12.mp4",
							format = "yyyy-MM-dd HH-mm-ss",
							SubstringStart = 0,
							Substring = 19
						  },
						new ParseDate
						  {
								Name = "20071230-2007-12-30 19-29-33_1_ahq12.mp4",
							format = "yyyy-MM-dd  HH-mm-ss",
							SubstringStart = 9,
							Substring = 20
						  },
						new ParseDate
						  {
							Name = "27. Srpna 2008 10-04-14_amqs2.mp4",
							format = "dd. MMMM yyyy  HH-mm-ss",
							SubstringStart = 0,
							Substring = 24
						  },
						new ParseDate
						  {
							Name = "04. Srpna 2008 15-00-20_2.25x_1440x1080_ahq-11.mp4",
							format = "dd. MMMM yyyy HH-mm-ss",
							SubstringStart = 0,
							Substring = 23
						  },
						new ParseDate
						  {
							Name = "20161126",
							format = "yyyyMMdd",
							SubstringStart = 0,
							Substring = 8
						  },
	new ParseDate
	{
		Name = "S6302967 07.06. 2008_4.5x_1440x1080_amq-13.mp4",
		format = "dd.MM. yyyy",
		SubstringStart = 10,
		Substring = 11
	},
					};
		}
	}
}
