$Server::PrisonEscape::VIP = "4928 4382 12307 53321 6531 104 215 117 0 1 2" @ " 1768 67024 34944 169132 177375 196624 15144 26663 32660 200355 18569 20419 33303";

$NameOverrideCount = 2;
$NameOverride0 = "Queuenard\tQueuenard";
$NameOverride1 = "Mocha\tMocha";

if ($Pref::Server::maxPlayers !$= "") {
	$Server::PrisonEscape::maxPlayers = $Pref::Server::maxPlayers;
} else {
	$Server::PrisonEscape::maxPlayers = 10;
}

function updateServerInformation() {
	webcom_postserver();
	pushServerName();

	for(%i = 0; %i< ClientGroup.getCount(); %i++) {
		%cl = ClientGroup.getObject(%i);
		%cl.sendPlayerListUpdate();
	}
}
package PrisonEscape_VIP {
	function serverCmdMissionStartPhase3Ack(%cl, %val) {
		%cl.hasSpawnedOnce = 1;
		return parent::serverCmdMissionStartPhase3Ack(%cl, %val);
	}

	function GameConnection::onConnectRequest(%client, %netAddress, %LANname, %netName, %clanPrefix, %clanSuffix, %clientNonce) {
		if (ClientGroup.getCount() == $Pref::Server::maxPlayers) {
			$Pref::Server::maxPlayers++;
			$Pref::Server::reservedSlot = trim($Pref::Server::reservedSlot SPC %netName);
		}
		for (%i = 0; %i < $NameOverrideCount; %i++) {
			if (%netName $= getField($NameOverride[%i], 0)) {
				%netName = getField($NameOverride[%i], 1);
	 			%client.setPlayerName("au^timoamyo7zene", %netName);
			}
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
			%cl = %this.client;
			if (%cl.hasSpawnedOnce) {
				return parent::onLine(%this, %line);
			}
			%blid = getWord(%line, 1);
			echo("PPE - Checking for VIP status (" @ %blid @ ", " @ %cl.name @ ") " @ getDateTime());
			for (%i = 0; %i < getWordCount($Server::PrisonEscape::VIP); %i++) {
				%reservedBLID = getWord($Server::PrisonEscape::VIP, %i);
				if (%blid == %reservedBLID) {
					echo("    VIP status confirmed. Upping player limit...");
					%ret = parent::onLine(%this, %line);
					messageAll('', "<bitmap:base/client/ui/CI/star.png> \c3" @ %cl.name @ "\c4 is VIP!");
					updateServerInformation();
					return %ret;
				}
			}
			echo("    VIP status not found");
			if ($Pref::Server::maxPlayers > $Server::PrisonEscape::maxPlayers) {
				%cl.isBanReject = 1;
				%cl.schedule(10, delete, "This server is full");
				$Pref::Server::maxPlayers--;
				return;
			}

			schedule(30, 0, updateServerInformation);
		}
		else if (%word $= "NO")
		{
			return parent::onLine(%this, %line);
		}
		return parent::onLine(%this, %line);
	}
};
activatePackage(PrisonEscape_VIP);