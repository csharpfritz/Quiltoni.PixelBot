using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Api;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace Quiltoni.PixelBot
{
	public class GoogleSheetProxy
	{

		static string ApplicationName = "Quiltoni PixelBot";
		private UserCredential _GoogleCredential;
		static string[] Scopes = { SheetsService.Scope.Spreadsheets };
		private bool _First = true;

		public GoogleSheetProxy(IOptions<PixelBotConfig> configuration, ILoggerFactory loggerFactory) {

			Config = configuration.Value;
			this.Logger = loggerFactory.CreateLogger("GoogleSheetProxy");

			ConfigureGoogleSheetsAccess();

		}

		public static PixelBotConfig Config { get; set; }

		public ILogger Logger { get; }

		public string Spreadsheet { get { return Config.Google.SheetId; } }

		public IChatService Twitch { get; set; }

		private void ConfigureGoogleSheetsAccess() {

			var googleSecrets = new ClientSecrets {
				ClientId = Config.Google.ClientId,
				ClientSecret = Config.Google.ClientSecret
			};

			_GoogleCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(googleSecrets, Scopes, "user", CancellationToken.None, new FileDataStore("token.json", true)).GetAwaiter().GetResult();

		}

		public void AddPixelsForChatters(string channel, int numPixelsToAdd, string actingUser) {
			SheetsService service;
			IList<IList<object>> values;
			GetSheetService(out service, out values);
			var appends = new ValueRange() { Values = new List<IList<object>>() };
			var updates = new List<ValueRange>();
			var logrows =  new List<IList<object>>();

			CreateLogSheet(service);

			var api = new TwitchAPI();
			api.Settings.AccessToken = Config.Twitch.AccessToken;

			var viewers = api.Undocumented.GetChattersAsync(channel).GetAwaiter().GetResult();

			foreach (var viewer in viewers) {
				var userName = viewer.Username.ToLowerInvariant().Trim();

				var thisRow = values.FirstOrDefault(r => r[0].ToString().Trim().ToLowerInvariant() == userName);
				if (thisRow == null) {
					Logger.LogDebug($"Adding row for {userName}");
					appends.Values.Add(new List<object>() {userName, numPixelsToAdd });
				} else {
					Logger.LogDebug($"Updating row for {userName}");

					var pos = values.IndexOf(thisRow);
					var newValue = (int.Parse(thisRow.Count < 2 ? "0" : thisRow[1].ToString())) + numPixelsToAdd;
					if (newValue < 0) newValue = 0;

					var update = new ValueRange() { Values = new List<IList<object>>() };

					update.Values.Add(new List<object>() { newValue });
					update.Range = $"Pixels!B{4 + pos}";

					updates.Add(update);
				}

				logrows.Add(ActivityLogRow(actingUser, userName, "ChatterPixels", numPixelsToAdd));
			}

			var updatereq = service.Spreadsheets.Values.BatchUpdate(new BatchUpdateValuesRequest { Data = updates, ValueInputOption = "USER_ENTERED" }, Spreadsheet);
			var appendreq = service.Spreadsheets.Values.Append(appends, Spreadsheet, "Pixels!A4:B");
			appendreq.ValueInputOption = AppendRequest.ValueInputOptionEnum.USERENTERED;

			updatereq.Execute();
			appendreq.Execute();

			ResortSpreadsheet(service);

			WriteActivityLogRowsToSheet(service, logrows);

			Twitch.BroadcastMessageOnChannel($"Successfully granted {numPixelsToAdd} Pixels to all people in chat");
		}

		public void AddPixelsForUser(string userName, int numPixelsToAdd, string actingUser) {
			SheetsService service;
			IList<IList<object>> values;
			GetSheetService(out service, out values);

			userName = userName.ToLowerInvariant().Trim();

			var thisRow = values.FirstOrDefault(r => r[0].ToString().Trim().ToLowerInvariant() == userName);
			if (thisRow == null) {
				Logger.LogDebug($"Adding row for {userName}");

				var rangeToSet = new List<IList<object>> {
					new List<object> {userName, numPixelsToAdd}
				};
				var append = new AppendCellsRequest();
				append.SheetId = 0;
				append.Fields = "*";
				var newRow = new RowData() { Values = new List<CellData> { } };
				newRow.Values.Add(new CellData { UserEnteredValue = new ExtendedValue { StringValue = userName } });
				newRow.Values.Add(new CellData { UserEnteredValue = new ExtendedValue { NumberValue = numPixelsToAdd }, UserEnteredFormat = new CellFormat { HorizontalAlignment = "CENTER" } });

				append.Rows = new[] { newRow };
				var appendRequest = new Request();
				appendRequest.AppendCells = append;
				service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest { Requests = new[] { appendRequest } }, Spreadsheet).Execute();

				ResortSpreadsheet(service);

				LogActivityOnSheet(service, actingUser, userName, "Add", numPixelsToAdd);

				Twitch.BroadcastMessageOnChannel($"Successfully granted {userName} their first {numPixelsToAdd} pixels!");

			}
			else {

				Logger.LogDebug($"Updating row for {userName}");

				var pos = values.IndexOf(thisRow);
				var newValue = (int.Parse(thisRow.Count < 2 ? "0" : thisRow[1].ToString())) + numPixelsToAdd;
				if (newValue < 0) newValue = 0;
				var rangeToSet = new List<IList<object>> {
					new List<object> {newValue}
				};

				UpdateRequest update = service.Spreadsheets.Values.Update(
					new Google.Apis.Sheets.v4.Data.ValueRange { Values = rangeToSet, Range = $"Pixels!B{4 + pos}" },
					Spreadsheet,
					$"Pixels!B{4 + pos}"
				);
				update.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;
				update.Execute();

				LogActivityOnSheet(service, actingUser, userName, "Add", numPixelsToAdd);

				Twitch.BroadcastMessageOnChannel($"Successfully granted {userName} an additional {numPixelsToAdd} pixels.  Their new total is {newValue} pixels");

			}

		}

		private void GetSheetService(out SheetsService service, out IList<IList<object>> values) {
			service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer {
				HttpClientInitializer = _GoogleCredential,
				ApplicationName = ApplicationName
			});
			var request = service.Spreadsheets.Values.Get(Spreadsheet, "Pixels!A4:B");
			var response = request.Execute();

			values = response.Values;
		}

		private int GetSheetIdForTitle(SheetsService service, string sheetTitle) {

			var sheets = service.Spreadsheets.Get(Spreadsheet).Execute().Sheets;

			for (var pos = 0; pos < sheets.Count; pos++) {
				if (sheets[pos].Properties.Title.Equals(sheetTitle, StringComparison.InvariantCultureIgnoreCase)) {
					return pos;
				}
			}

			return -1;

		}

		private void ResortSpreadsheet(SheetsService service) {
			var sheetId = GetSheetIdForTitle(service, "Pixels");

			BatchUpdateSpreadsheetRequest reqbody = new BatchUpdateSpreadsheetRequest();

			var sortReq = new Request() {
				SortRange = new SortRangeRequest() {
					Range = new GridRange() {
						StartColumnIndex = 0,
						EndColumnIndex = 12,
						StartRowIndex = 3,
						SheetId = sheetId
					},
					SortSpecs = new List<SortSpec>() {
						new SortSpec() {
							DimensionIndex = 0,
							SortOrder = "ASCENDING"
						}
					}
				}
			};

			var formatReq = new Request() {
				RepeatCell = new RepeatCellRequest() {
					Cell = new CellData() {
						UserEnteredFormat = new CellFormat() {
							HorizontalAlignment = "CENTER"
						}
					},
					Range = new GridRange() {
						StartColumnIndex = 1,
						EndColumnIndex = 2,
						StartRowIndex = 3,
						SheetId = sheetId
					},
					Fields = "userEnteredFormat(horizontalAlignment)"

				}
			};


			IList<Request> requests = new List<Request> {
				sortReq,
				formatReq
			};
			reqbody.Requests = requests;

			SpreadsheetsResource.BatchUpdateRequest sortreq = service.Spreadsheets.BatchUpdate(reqbody, Spreadsheet);
			sortreq.Execute();

		}

		private List<object> ActivityLogRow(string actingUser, string userModified, string command, int change = 0) {
			var newRow = new List<object>();
			newRow.Add(DateTime.Now.ToString("MMM dd, yyyy - h:mm:ss tt"));
			newRow.Add(actingUser);
			newRow.Add(userModified);
			newRow.Add(command);
			newRow.Add(change);

			return newRow;
		}

		private void WriteActivityLogRowsToSheet(SheetsService service, List<IList<object>> rows) {
			CreateLogSheet(service);

			AppendRequest appendRequest = service.Spreadsheets.Values.Append(new ValueRange { Values = rows }, Spreadsheet, "PixelBotLog!A2:E");
			appendRequest.InsertDataOption = AppendRequest.InsertDataOptionEnum.INSERTROWS;
			appendRequest.ValueInputOption = AppendRequest.ValueInputOptionEnum.USERENTERED;
			_ = appendRequest.Execute();
		}

		private void LogActivityOnSheet(SheetsService service, string actingUser, string userModified, string command, int change = 0) {
			var rows = new List<IList<object>>();
			List<object> newRow = ActivityLogRow(actingUser, userModified, command, change);

			rows.Add(newRow);

			WriteActivityLogRowsToSheet(service, rows);
		}

		private void CreateLogSheet(SheetsService service) {

			// Only run this the first time
			if (_First) return;
			_First = false;

			var thisSpreadsheet = service.Spreadsheets.Get(Spreadsheet).Execute();
			if (!thisSpreadsheet.Sheets.Any(s => s.Properties.Title == "PixelBotLog")) {
				var newSheetRequest = new AddSheetRequest() {
					Properties = new SheetProperties {
						Title = "PixelBotLog"
					}
				};
				service.Spreadsheets.BatchUpdate(
				new BatchUpdateSpreadsheetRequest {
					Requests = new[] {
					new Request { AddSheet = newSheetRequest } }
				}, Spreadsheet).Execute();

				var newRecords = new List<IList<object>>();
				var newRow = new List<object>();
				newRow.Add("Date");
				newRow.Add("Acting User");
				newRow.Add("Record Updated");
				newRow.Add("Command");
				newRow.Add("Pixels Changed");

				newRecords.Add(newRow);
				AppendRequest appendRequest = service.Spreadsheets.Values.Append(new ValueRange { Values = newRecords }, Spreadsheet, "PixelBotLog!A1:E");
				appendRequest.InsertDataOption = AppendRequest.InsertDataOptionEnum.INSERTROWS;
				appendRequest.ValueInputOption = AppendRequest.ValueInputOptionEnum.USERENTERED;
				_ = appendRequest.Execute();

			}

		}

		public int FindPixelsForUser(string userName) {

			SheetsService service;
			IList<IList<object>> values;
			GetSheetService(out service, out values);

			userName = userName.ToLowerInvariant().Trim();

			var thisRow = values.FirstOrDefault(r => r[0].ToString().Trim().ToLowerInvariant() == userName);
			if (thisRow == null || thisRow.Count < 2) 
			{
				return 0;
			} 

			return int.Parse(thisRow[1].ToString());

		}

	}

}
