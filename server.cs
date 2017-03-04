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
exec("./customization.cs");
exec("./vip.cs");
exec("./audio.cs");

exec("./Item_Prison/server.cs");
exec("./Item_PosTool/posTool.cs");
exec("./Item_Logo/server.cs");

exec("./Emitters/server.cs");

exec("./Brick_Prison/server.cs");
exec("./Brick_Garage_Door/server.cs");
exec("./Brick_Security_Camera/server.cs");

exec("./Bot_Dog/server.cs");
exec("./Player_Spotlight/server.cs");
exec("./Player_Buff/server.cs");
exec("./Player_Laundry_Cart/server.cs");
schedule(10000, 0, eval, "$Game::Item::PopTime = 40000;");

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

WrenchImage.canMountToBronson = 1;
hammerImage.canMountToBronson = 1;
printGunImage.canMountToBronson = 1;

AddDamageType("Satellite",	'<bitmap:Add-Ons/Gamemode_PPE/ci/CI_Satellite> %1',	 '%2 <bitmap:Add-Ons/Gamemode_PPE/ci/CI_Satellite> %1',0.2,1);
AddDamageType("Generator",	'<bitmap:Add-Ons/Gamemode_PPE/ci/CI_Generator> %1',	 '%2 <bitmap:Add-Ons/Gamemode_PPE/ci/CI_Generator> %1',0.2,1);
AddDamageType("Tower",	'<bitmap:Add-Ons/Gamemode_PPE/ci/CI_Tower> %1',	 '%2 <bitmap:Add-Ons/Gamemode_PPE/ci/CI_Tower> %1',0.2,1);
AddDamageType("Dog",	'<bitmap:Add-Ons/Gamemode_PPE/ci/Dog> %1',	 '%2 <bitmap:Add-Ons/Gamemode_PPE/ci/Dog> %1',0.2,1);
//.*".+\n\+-OWNER 4928\n(?!\+-.+)
//https://www.youtube.com/watch?v=lnGnJWKyBak