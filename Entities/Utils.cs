﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Botwinder.entities
{
	public class Utils
	{
	}

	public static class DiscordEx
	{
		public static async Task SendMessageSafe(this IUser self, string message, Embed embed = null) => await SendMessageSafe(async m => await self.SendMessageAsync(m, false, embed), message);
		public static async Task SendMessageSafe(this ISocketMessageChannel self, string message, Embed embed = null) => await SendMessageSafe(async m => await self.SendMessageAsync(m, false, embed), message);
		//public static async Task SendMessageSafe(this IDMChannel self, string message, Embed embed = null) => await SendMessageSafe(async m => await self.SendMessageAsync(m, false, embed), message); // I don't think that we will ever need this one.

		public static async Task SendMessageSafe(Func<string, Task> sendMessage, string message)
		{
			string safetyCopy = "";
			string newChunk = "";

			while( message.Length > GlobalConfig.MessageCharacterLimit )
			{
				int split = message.Substring(0, GlobalConfig.MessageCharacterLimit).LastIndexOf('\n');
				string chunk = "";

				if( split == -1 )
				{
					chunk = message;
					message = "";
				}
				else
				{
					chunk = message.Substring(0, split);
					message = message.Substring(split + 1);
				}

				while( chunk.Length > GlobalConfig.MessageCharacterLimit )
				{
					safetyCopy = newChunk;
					split = chunk.Substring(0, GlobalConfig.MessageCharacterLimit).LastIndexOf(' ');
					if( split == -1 || (safetyCopy.Length == (newChunk = chunk.Substring(0, split)).Length && safetyCopy == newChunk) )
					{
						await sendMessage("I've encountered an error trying send a single word longer than " + GlobalConfig.MessageCharacterLimit.ToString() + " characters.");
						return;
					}

					await sendMessage(newChunk);
					chunk = chunk.Substring(split + 1);
				}
				await sendMessage(chunk);
			}

			if( !string.IsNullOrWhiteSpace(message) )
				await sendMessage(message);
		}
	}
}
