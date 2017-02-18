exec("./statistics.cs");
exec("./packages.cs");
exec("./phase - lobby.cs");
exec("./phase - pregame.cs");
exec("./phase - game.cs");
exec("./chat.cs");
exec("./stun.cs");
exec("./globalcams.cs");
exec("./rounds.cs");
exec("./collection.cs");
exec("./vip.cs");

exec("./Item_Prison/server.cs");
exec("./Item_PosTool/posTool.cs");

exec("./Brick_Prison/server.cs");
exec("./Brick_Garage_Door/server.cs");
exec("./Brick_Security_Camera/server.cs");

exec("./Bot_Dog/server.cs");
exec("./Player_Spotlight/server.cs");
exec("./Player_Buff/server.cs");
exec("./Player_Laundry_Cart/server.cs");
schedule(10000, 0, eval, "$Game::Item::PopTime = 60000;");

exec("./Event_Camera_Control_Advanced/server.cs");
if (isPackage(ChatEval)) {
	deactivatePackage(ChatEval);
	activatePackage(ChatEval);
}
if (isPackage(DiscordNode)) {
	deactivatePackage(DiscordNode);
	activatePackage(DiscordNode);
}

function serverCmdWrench(%cl) {
	if (!isObject(%pl = %cl.player) || !%cl.isAdmin) {
		return;
	}
	%pl.mountImage(WrenchImage, 0);
	%pl.playThread(1, armReadyRight);
}