using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client.Enums;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace Quiltoni.PixelBot
{
	public class PixelBot : IHostedService
	{

		private TwitchClient _Client;
		static string[] Scopes = { SheetsService.Scope.Spreadsheets };
		static string ApplicationName = "Quiltoni PixelBot";
		private UserCredential _GoogleCredential;
		private static bool _First = true;

		public PixelBot(IOptions<PixelBotConfig> configuration, ILoggerFactory loggerFactory)
		{

			Config = configuration.Value;
			this.Logger = loggerFactory.CreateLogger("PixelBot");

		}

		public static PixelBotConfig Config { get; set; }

		public ILogger Logger { get; }

		public string Channel { get { return Config.Twitch.Channel; } }

		public string Spreadsheet { get { return Config.Google.SheetId; } }

		public Task StartAsync(CancellationToken cancellationToken)
		{

			var creds = new ConnectionCredentials(Config.Twitch.UserName, Config.Twitch.AccessToken);
			_Client = new TwitchClient();
			_Client.Initialize(creds);

			_Client.OnConnected += _Client_OnConnected;
			_Client.OnMessageReceived += _Client_OnMessageReceived;
			_Client.OnNewSubscriber += _Client_OnNewSubscriber;
			_Client.OnReSubscriber += _Client_OnReSubscriber;
			_Client.OnGiftedSubscription += _Client_OnGiftedSubscription;
			_Client.OnRaidNotification += _Client_OnRaidNotification;

			_Client.Connect();

			ConfigureGoogleSheetsAccess();

			return Task.CompletedTask;

		}

		private void _Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
		{

			var pixels = new int[] { 3, int.Parse(e.RaidNotificaiton.MsgParamViewerCount) }.Max();
			pixels = pixels > 200 ? 200 : pixels;

			AddPixelsForUser(e.Channel, pixels, "PixelBot-Raid");

		}

		private static readonly Dictionary<SubscriptionPlan, int> _PixelRewards = new Dictionary<SubscriptionPlan, int> {
			{ SubscriptionPlan.Prime, 10 },
			{ SubscriptionPlan.Tier1, 10 },
			{ SubscriptionPlan.Tier2, 25 },
			{ SubscriptionPlan.Tier3, 60 },
		};

		private void _Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
		{

			AddPixelsForUser(e.ReSubscriber.DisplayName, _PixelRewards[e.ReSubscriber.SubscriptionPlan], "PixelBot-Resub");

		}

		private void _Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
		{

			AddPixelsForUser(e.Subscriber.DisplayName, _PixelRewards[e.Subscriber.SubscriptionPlan], "PixelBot-Sub");

		}

		private void _Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
		{

			AddPixelsForUser(e.GiftedSubscription.DisplayName, 2, "PixelBot-SubGifter");
			AddPixelsForUser(e.GiftedSubscription.MsgParamRecipientDisplayName, _PixelRewards[e.GiftedSubscription.MsgParamSubPlan], "PixelBot-SubGift");

		}

		private void _Client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
		{
			_Client.JoinChannel(Channel);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_Client.Disconnect();
			return Task.CompletedTask;
		}

		private void ConfigureGoogleSheetsAccess()
		{

			var googleSecrets = new ClientSecrets {
				ClientId = Config.Google.ClientId,
				ClientSecret = Config.Google.ClientSecret
			};

			_GoogleCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(googleSecrets, Scopes, "user", CancellationToken.None, new FileDataStore("token.json", true)).GetAwaiter().GetResult();

		}

		private void _Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
		{

			if (e.ChatMessage.IsMe) return;

			if (e.ChatMessage.Message.StartsWith("!add") && (e.ChatMessage.IsBroadcaster || e.ChatMessage.IsModerator))
			{

				var parts = e.ChatMessage.Message.Split(' ');
				if (parts.Length != 3)
				{
					_Client.SendWhisper(e.ChatMessage.DisplayName, "Invalid format to add pixels.  \"!add username pixelsToAdd\"");
					return;
				}
				else if (!int.TryParse(parts[2], out int pixels))
				{
					_Client.SendWhisper(e.ChatMessage.DisplayName, "Invalid format to add pixels.  \"!add username pixelsToAdd\"");
					return;
				}

				AddPixelsForUser(parts[1].Trim(), int.Parse(parts[2]), e.ChatMessage.DisplayName);

			}
			else if (e.ChatMessage.Message.StartsWith("!add")) {

				_Client.SendMessage(Channel, "Only moderators can execute the !add command");

			}
			else if (e.ChatMessage.Message == "!mypixels")
			{

				ReportPixelsForUser(e.ChatMessage.DisplayName);

			}

		}

		private void ReportPixelsForUser(string userName)
		{
			SheetsService service;
			IList<IList<object>> values;
			GetSheetService(out service, out values);

			userName = userName.ToLowerInvariant().Trim();

			var thisRow = values.FirstOrDefault(r => r[0].ToString().Trim().ToLowerInvariant() == userName);
			if (thisRow == null || thisRow.Count < 2) {
				_Client.SendMessage(Channel, $"{userName} does not currently have any pixels");
			} else {
				_Client.SendMessage(Channel, $"{userName} currently has {thisRow[1]} pixels");
			}

		}

		private void AddPixelsForUser(string userName, int numPixelsToAdd, string actingUser)
		{
			SheetsService service;
			IList<IList<object>> values;
			GetSheetService(out service, out values);

			userName = userName.ToLowerInvariant().Trim();

			var thisRow = values.FirstOrDefault(r => r[0].ToString().Trim().ToLowerInvariant() == userName);
			if (thisRow == null)
			{
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

				_Client.SendMessage(Channel, $"Successfully granted {userName} their first {numPixelsToAdd} pixels!");

			}
			else
			{

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

				_Client.SendMessage(Channel, $"Successfully granted {userName} an additional {numPixelsToAdd} pixels.  Their new total is {newValue} pixels");

			}




		}

		private void GetSheetService(out SheetsService service, out IList<IList<object>> values)
		{
			service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer
			{
				HttpClientInitializer = _GoogleCredential,
				ApplicationName = ApplicationName
			});
			var request = service.Spreadsheets.Values.Get(Spreadsheet, "Pixels!A4:B");
			var response = request.Execute();

			values = response.Values;
		}

		private int GetSheetIdForTitle(SheetsService service, string sheetTitle) {

			var sheets = service.Spreadsheets.Get(Spreadsheet).Execute().Sheets;

			for (var pos=0; pos<sheets.Count; pos++) {
				if (sheets[pos].Properties.Title.Equals(sheetTitle, StringComparison.InvariantCultureIgnoreCase)) {
					return pos;
				}
			}

			return -1;

		}

		private void ResortSpreadsheet(SheetsService service)
		{
			BatchUpdateSpreadsheetRequest reqbody = new BatchUpdateSpreadsheetRequest();
			SortSpec ss = new SortSpec();
			ss.DimensionIndex = 0;
			ss.SortOrder = "ASCENDING";

			GridRange rangetosort = new GridRange();
			rangetosort.StartColumnIndex = 0;
			rangetosort.EndColumnIndex = 12;
			rangetosort.StartRowIndex = 3;
			//rangetosort.EndRowIndex = totalrows;
			rangetosort.SheetId = 0;

			SortRangeRequest srr = new SortRangeRequest();
			srr.Range = rangetosort;
			IList<SortSpec> sortspecs = new List<SortSpec>();
			sortspecs.Add(ss);
			srr.SortSpecs = sortspecs;
			Request req1 = new Request();
			req1.SortRange = srr;
			IList<Request> req2 = new List<Request>();
			req2.Add(req1);
			reqbody.Requests = req2;

			SpreadsheetsResource.BatchUpdateRequest sortreq = service.Spreadsheets.BatchUpdate(reqbody, Spreadsheet);
			sortreq.Execute();

		}

		private void LogActivityOnSheet(SheetsService service, string actingUser, string userModified, string command, int change = 0)
		{

			CreateLogSheet(service);

			var newRecords = new List<IList<object>>();
			var newRow = new List<object>();
			newRow.Add(DateTime.Now.ToString("MMM dd, yyyy - h:mm:ss tt"));
			newRow.Add(actingUser);
			newRow.Add(userModified);
			newRow.Add(command);
			newRow.Add(change);

			newRecords.Add(newRow);
		 	AppendRequest appendRequest = service.Spreadsheets.Values.Append(new ValueRange { Values = newRecords }, Spreadsheet, "PixelBotLog!A2:E");
			appendRequest.InsertDataOption = AppendRequest.InsertDataOptionEnum.INSERTROWS;
			appendRequest.ValueInputOption = AppendRequest.ValueInputOptionEnum.USERENTERED;
			_ = appendRequest.Execute();

		}

		private void CreateLogSheet(SheetsService service)
		{

			// Only run this the first time
			if (_First) return;
			_First = false;

			var thisSpreadsheet = service.Spreadsheets.Get(Spreadsheet).Execute();
			if (!thisSpreadsheet.Sheets.Any(s => s.Properties.Title == "PixelBotLog"))
			{
				var newSheetRequest = new AddSheetRequest()
				{
					Properties = new SheetProperties
					{
						Title = "PixelBotLog"
					}
				};
				service.Spreadsheets.BatchUpdate(
				new BatchUpdateSpreadsheetRequest
				{
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
	}
}
