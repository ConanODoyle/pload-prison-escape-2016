$Server::PrisonEscape::VIP = "4928 4382 12307 104 215 0 1 2";
$Server::PrisonEscape::maxPlayers = 2;

function updateServerInformation() {
	webcom_postserver();
	pushServerName();

	for(%i = 0; %i< ClientGroup.getCount(); %i++) {
		%cl = ClientGroup.getObject(%i);
		%cl.sendPlayerListUpdate();
	}
}
package PrisonEscape_VIP {
	function GameConnection::authCheck(%client) {
		talk(%client.name SPC %client.bl_id);
		return parent::authCheck(%client);
	}

	function GameConnection::onConnectRequest(%client, %netAddress, %LANname, %netName, %clanPrefix, %clanSuffix, %clientNonce) {
		if (ClientGroup.getCount() == $Pref::Server::maxPlayers) {
			$Pref::Server::maxPlayers++;
			$Pref::Server::reservedSlot = trim($Pref::Server::reservedSlot SPC %netName);
		}
		return parent::onConnectRequest(%client, %netAddress, %LANname, %netName, %clanPrefix, %clanSuffix, %clientNonce);
	}

	function GameConnection::onDrop(%client) {
		if ($Pref::Server::maxPlayers > $Server::PrisonEscape::maxPlayers) {
			$Pref::Server::maxPlayers--;
			updateServerInformation();
		}
		return parent::onDrop(%client);
	}

	function servAuthTCPObj::onLine(%this, %line) {
		%word = getWord(%line, 0);
		if (%word $= "YES")
		{
			%blid = getWord(%line, 1);
			echo("PPE - Checking for VIP status");
			for (%i = 0; %i < getWordCount($Server::PrisonEscape::VIP); %i++) {
				%reservedBLID = getWord($Server::PrisonEscape::VIP, %i);
				if (%blid == %reservedBLID) {
					echo("    VIP status confirmed. Upping player limit...");
					updateServerInformation();
					return parent::onLine(%this, %line);
				}
			}
			if ($Pref::Server::maxPlayers > $Server::PrisonEscape::maxPlayers) {
				%cl = %this.client;
				%cl.isBanReject = 1;
				%cl.schedule(10, delete, "This server is full");
				$Pref::Server::maxPlayers--;
				return;
			}
			schedule(30, 0, updateServerInformation);
// 			%this.client.bl_id = getWord(%line, 1);
// 			%this.client.setBLID("au^timoamyo7zene", getWord(%line, 1));
// 			if (%this.client.getBLID() != getNumKeyID())
// 			{
// 				%reason = $BanManagerSO.isBanned(%this.client.getBLID());
// 				if (%reason)
// 				{
// 					%reason = getField(%reason, 1);
// 					echo("BL_ID " @ %this.client.getBLID() @ " is banned, rejecting");
// 					%this.client.isBanReject = 1;
// 					%this.client.schedule(10, delete, "
// You are banned from this server.
// Reason: " @ %reason);
// 					return;
// 				}
// 			}
// 			if (!%this.client.getHasAuthedOnce())
// 			{
// 				echo("Auth Init Successfull: " @ %this.client.getPlayerName());
// 				%this.client.setHasAuthedOnce(1);
// 				%this.client.startLoad();
// 				%this.client.killDupes();
// 				%this.client.schedule(60.0 * 1000.0 * 5.0, authCheck);
// 			}
// 			else
// 			{
// 				echo("Auth Continue Successfull: " @ %this.client.getPlayerName());
// 				%this.client.schedule(60.0 * 1000.0 * 5.0, authCheck);
// 			}
		}
		else if (%word $= "NO")
		{
			return parent::onLine(%this, %line);
		}
		return parent::onLine(%this, %line);
	}
};
activatePackage(PrisonEscape_VIP);