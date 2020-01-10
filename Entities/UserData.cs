﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

using guid = System.UInt64;

namespace Valkyrja.entities
{
	[Table("users")]
	public class UserData
	{
		[Required]
		[Column("serverid")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public guid ServerId{ get; set; } = 0;

		[Required]
		[Column("userid")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public guid UserId{ get; set; } = 0;

		[Column("username")]
		public string LastUsername{ get; set; } = "";

		[Column("verified")]
		public bool Verified{ get; set; } = false;

		[Column("karma_count")]
		public Int64 KarmaCount{ get; set; } = 0;

		[Column("warning_count")]
		public Int64 WarningCount{ get; set; } = 0;

		[Column("notes", TypeName = "text")]
		public string Notes{ get; set; } = "";

		[Column("last_thanks_time")]
		public DateTime LastThanksTime{ get; set; } = DateTime.MinValue;

		[Column("banned_until")]
		public DateTime BannedUntil{ get; set; } = DateTime.MinValue;

		[Column("muted_until")]
		public DateTime MutedUntil{ get; set; } = DateTime.MinValue;

		[Column("ignored")]
		public bool Ignored{ get; set; } = false;

		[Column("count_message")]
		public Int64 CountMessages{ get; set; } = 0;

		[Column("count_attachments")]
		public Int64 CountAttachments{ get; set; } = 0;

		[Column("level_relative")]
		public Int64 Level{ get; set; } = 0;

		[Column("exp_relative")]
		public Int64 Exp{ get; set; } = 0;

		[Column("memo", TypeName = "text")]
		public string Memo{ get; set; } = "";

		public string GetWhoisString(ServerContext dbContext, SocketGuildUser user = null)
		{
			StringBuilder whoisString = new StringBuilder();

			if( user != null )
				whoisString.AppendLine($"<@{this.UserId}>: `{this.UserId}` | `{user.GetUsername()}`\n" +
									   $"    Account created at: `{Utils.GetTimestamp(Utils.GetTimeFromId(this.UserId))}`");
			else
				whoisString.AppendLine($"<@{this.UserId}>: `{this.UserId}`\n" +
									   $"    Account created at: `{Utils.GetTimestamp(Utils.GetTimeFromId(this.UserId))}`");

			if( user?.JoinedAt != null )
				whoisString.AppendLine($"    Joined the server: `{Utils.GetTimestamp(user.JoinedAt.Value)}`");

			if( user != null )
				whoisString.AppendLine("    Roles: " + user.Roles.Select(r => r.Name.Replace('`', '\'')).ToNames());

			if( this.Verified )
				whoisString.AppendLine("    Verified: `true`");

			if( this.Ignored )
				whoisString.AppendLine("    Ignored by Logging: `true`");

			if( this.MutedUntil > DateTime.UtcNow )
				whoisString.AppendLine("    Muted until: " + Utils.GetTimestamp(this.MutedUntil));

			if( this.BannedUntil > DateTime.UtcNow )
				whoisString.AppendLine("    Banned until: " + Utils.GetTimestamp(this.BannedUntil));

			List<string> foundUsernames = dbContext.Usernames
				.Where(u => u.ServerId == this.ServerId && u.UserId == this.UserId)
				.Select(u => u.Name.Replace('`', '\'')).ToList();
			whoisString.Append("    Known usernames: ");
			whoisString.AppendLine(foundUsernames.ToNames());

			List<string> foundNicknames = dbContext.Nicknames
				.Where(u => u.ServerId == this.ServerId && u.UserId == this.UserId)
				.Select(u => u.Name.Replace('`', '\'')).ToList();
			whoisString.Append("    Known nicknames: ");
			whoisString.AppendLine(foundNicknames.ToNames());

			if( this.WarningCount > 0 || !string.IsNullOrEmpty(this.Notes) )
			{
				whoisString.AppendLine($"They have {this.WarningCount} warnings, with these notes:");
				foreach( string w in this.Notes.Split('|') )
				{
					whoisString.AppendLine($"* {w.Trim()}");
				}
			}

			return whoisString.ToString().Replace("@everyone", "@-everyone").Replace("@here", "@-here");
		}

		public void AddWarning(string warning)
		{
			this.WarningCount++;
			this.Notes += string.IsNullOrEmpty(this.Notes) ? warning : (" | " + warning);
		}
	}

	[Table("usernames")]
	public class Username
	{
		[Required]
		[Column("serverid")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public guid ServerId{ get; set; } = 0;

		[Required]
		[Column("userid")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public guid UserId{ get; set; } = 0;

		[Required]
		[Column("username")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public string Name{ get; set; } = "";
	}

	[Table("nicknames")]
	public class Nickname
	{
		[Required]
		[Column("serverid")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public guid ServerId{ get; set; } = 0;

		[Required]
		[Column("userid")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public guid UserId{ get; set; } = 0;

		[Required]
		[Column("nickname")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public string Name{ get; set; } = "";
	}
}
