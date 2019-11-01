using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using LINQtoCSV;

namespace PixelBot.ReplayLog
{
	public class LogRecord
	{

		[CsvColumn(Name = "Date", FieldIndex = 1)]
		public string Date { get; set; }

		public DateTime DateStamp
		{
			get
			{
				return DateTime.ParseExact(Date, "MMM dd, yyyy - h:mm:ss tt", CultureInfo.InvariantCulture);
			}
		}

		[CsvColumn(Name = "Acting User", FieldIndex = 2)]
		public string ActingUser { get; set; }

		[CsvColumn(Name = "Record Updated", FieldIndex = 3)]
		public string UpdatedUser { get; set; }

		[CsvColumn(Name = "Command", FieldIndex = 4)]
		public string Command { get; set; }

		[CsvColumn(Name = "Pixels Changed", FieldIndex = 5)]
		public int Changed { get; set; }

	}
}