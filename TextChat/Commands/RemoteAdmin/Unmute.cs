﻿using EXILED.Extensions;
using System;
using TextChat.Extensions;
using TextChat.Interfaces;
using static TextChat.Database;

namespace TextChat.Commands.RemoteAdmin
{
	public class Unmute : ICommand
	{
		public string Description => "Unmute a player from the chat.";

		public string Usage => ".chat_unmute [PlayerID/UserID/Name]";

		public (string response, string color) OnCall(ReferenceHub sender, string[] args)
		{
			if (!sender.CheckPermission("tc.unmute")) return ("You don't have enough permissions to run this command!", "red");

			if (args.Length != 1) return ($"You have to provide one parameter! {Usage}", "red");

			ReferenceHub target = Player.GetPlayer(args[0]);

			if (target == null) return ($"Player {args[0]} was not found!", "red");

			var mutedPlayer = LiteDatabase.GetCollection<Collections.Chat.Mute>().FindOne(mute => mute.Target.Id == target.GetRawUserId() && mute.Expire > DateTime.Now);

			if (mutedPlayer == null) return ($"{target.GetNickname()} is not muted!", "red");

			mutedPlayer.Expire = DateTime.Now;

			LiteDatabase.GetCollection<Collections.Chat.Mute>().Update(mutedPlayer);

			target.SendConsoleMessage("You have been unmuted from the chat!", "green");

			return ($"{target.GetNickname()} has been unmuted from the chat", "green");
		}
	}
}
