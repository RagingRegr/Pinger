using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Pinger
{
	internal class Program
	{
		public static List<string> pingResults = new List<string>();
		public const int maxLinesOnScreen = 20;
		public const int startingRow = 3;
		public const string ipAdd = "159.49.47.135";
		public const int delay = 1000;
		public const int block = 25;
		public const int yellowPing = 100;
		public const int redPing = 200;
		public const string blockChar = "█";
		static void Main(string[] args)
		{
			Ping pingSender = new Ping();
			PingOptions options = new PingOptions();
			options.DontFragment = true;
			string data = "sadadajilawidjoaw";
			byte[] buffer = Encoding.ASCII.GetBytes(data);

			int timeout = 120;
			int counter = 0, sum = 0;
			int success = 0, failed = 0;
			while (true)
			{
				{
					PingReply reply = pingSender.Send(ipAdd, timeout, buffer, options);

					if (reply.Status == IPStatus.Success)
					{
						pingResults.Add(ReturnReplyInfo(reply));

						success++;
						counter++;
						sum += Convert.ToInt32(reply.RoundtripTime);

						RemoveFromListIfFull();
						Console.Clear();
						PrintReplyInfo(reply, sum, counter, success, failed);
						Console.SetCursorPosition(0, startingRow);
						PrintList(pingResults);

						Thread.Sleep(delay);
					}
					else
					{
						failed++;
					}
				}
			}
		}
		#region Printing

		private static void PrintList(List<string> list)
		{
			for (int j = 0; j < list.Count; j++)
			{
				Console.WriteLine(list[j]);
			}
		}
		private static void PrintReplyInfo(PingReply reply, int sum, int counter, int success, int failed)
		{
			string currentTime = DateTime.Now.ToString();
			Console.Write($"Started: {currentTime}\t" +
										$"Successful: {success}\t" +
										$"Failed: {failed}\n" +
										$"Pinging: {ipAdd}\t");
			PrintLastPing(reply);
			PrintAveragePing(reply, sum, counter);
		}
		private static void PrintLastPing(PingReply reply)
		{
			Console.Write("Last ping: ");
			ColorSwitcher(reply);
			PrintPBLatest(reply);
			Console.ForegroundColor = ConsoleColor.Gray;

			Console.Write($" {reply.RoundtripTime}ms\t");
		}
		private static void PrintAveragePing(PingReply reply, int sum, int counter)
		{
			Console.Write("Average ping: ");
			ColorSwitcher(reply);
			PrintPBAverage(reply, sum, counter);
			Console.ForegroundColor = ConsoleColor.Gray;

			Console.Write($" {sum / counter}ms\n");
		}

		#endregion

		#region ProgressBars

		private static void PrintPBLatest(PingReply reply)
		{
			int i = 0;
			do
			{
				Console.Write(blockChar);
				i++;
			} while (i < reply.RoundtripTime / block);
		}
		private static void PrintPBAverage(PingReply reply, int sum, int counter)
		{
			int i = 0;
			do
			{
				Console.Write(blockChar);
				i++;
			} while (i < (sum / counter) / block);
		}

		#endregion

		#region TextColoring

		private static void ColorSwitcher(PingReply reply)
		{
			if (reply.RoundtripTime > redPing)
			{
				Console.ForegroundColor = ConsoleColor.Red;
			}
			else if (reply.RoundtripTime > yellowPing)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Green;
			}
		}

		#endregion
		private static void RemoveFromListIfFull()
		{
			if (pingResults.Count > maxLinesOnScreen)
			{
				pingResults.RemoveAt(0);
			}
		}
		private static string ReturnReplyInfo(PingReply reply)
		{
			return $"Reply from {ipAdd}: bytes={reply.Buffer.Length} time={reply.RoundtripTime}ms TTL={reply.Options.Ttl}";
		}
	}
}
