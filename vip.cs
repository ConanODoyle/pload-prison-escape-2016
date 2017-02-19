$Server::PrisonEscape::VIP = "4928 4382 12307 53321 6531 104 215 117 0 1 2 293" @ " 1768 67024 34944 169132 177375 196624 15144 26663 32660 200355 18569 20419 33303 11532 67024 38483 48871 166247 41072 22556 26586 15144 109211 44383 15269 36965 32202";
$Server::PrisonEscape::SpecialBan0 = "49581 have fun not playing on my server asshole";
$Server::PrisonEscape::SpecialBan1 = "49070 have fun not playing on my server asshole";

$Server::PrisonEscape::joinStatus = "The server is full";

$NameOverrideCount = 2;
$NameOverride0 = "Queuenard\tQueuenard";
$NameOverride1 = "Mocha\tMocha";

if ($Pref::Server::maxPlayers !$= "") {
	$Server::PrisonEscape::maxPlayers = $Pref::Server::maxPlayers;
} else {
	$Server::PrisonEscape::maxPlayers = 10;
}

function updateServerPlayerCount() {
	webcom_postserver();
	pushServerName();

	commandToAll('NewPlayerListGui_UpdateWindowTitle', $Pref::Server::Name, $Pref::Server::maxPlayers);
	// for(%i = 0; %i< ClientGroup.getCount(); %i++) {
	// 	%cl = ClientGroup.getObject(%i);
	// 	%cl.sendPlayerListUpdate();
	// }
}
package PrisonEscape_VIP {
	function serverCmdMissionStartPhase3Ack(%cl, %val) {
		%cl.hasSpawnedOnce = 1;
		return parent::serverCmdMissionStartPhase3Ack(%cl, %val);
	}

	function GameConnection::onConnectRequest(%client, %netAddress, %LANname, %netName, %clanPrefix, %clanSuffix, %clientNonce) {
		if (ClientGroup.getCount() == $Pref::Server::maxPlayers) {
			$Pref::Server::maxPlayers++;
		} else if (ClientGroup.getCount() > $Pref::Server::maxPlayers) {
			$Pref::Server::maxPlayers = ClientGroup.getCount() + 1;
		}
		for (%i = 0; %i < $NameOverrideCount; %i++) {
			if (%netName $= getField($NameOverride[%i], 0)) {
				%netName = getField($NameOverride[%i], 1);
	 			%client.setPlayerName("au^timoamyo7zene", %netName);
			}
		}
		messageClient(fcn(Conan), '', "\c4Attempt to join by \c3" @ %netName);
		return parent::onConnectRequest(%client, %netAddress, %LANname, %netName, %clanPrefix, %clanSuffix, %clientNonce);
	}

	function GameConnection::onDrop(%client) {
		if ($Pref::Server::maxPlayers > $Server::PrisonEscape::maxPlayers && $Pref::Server::maxPlayers > ClientGroup.getCount()) {
			$Pref::Server::maxPlayers--;
			updateServerPlayerCount();
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
					messageClient(fcn(Conan), '', "\c4    Attempt to join succeeded");
					echo("    VIP status confirmed. Upping player limit...");
					%ret = parent::onLine(%this, %line);
					messageAll('', "<bitmap:base/client/ui/CI/star.png> \c3" @ %cl.name @ "\c4 is VIP!");
					updateServerPlayerCount();
					return %ret;
				}
			}
			echo("    VIP status not found");
			if ($Pref::Server::maxPlayers > $Server::PrisonEscape::maxPlayers) {
				messageClient(fcn(Conan), '', "\c4    Attempt to join failed");
				%cl.isBanReject = 1;
				%cl.schedule(10, delete, $Server::PrisonEscape::joinStatus);
				schedule(10, 0, eval, "$Pref::Server::maxPlayers--;");
				return;
			} else {
				%i = 0;
				while ($Server::PrisonEscape::SpecialBan[%i] !$= "") {
					%str = $Server::PrisonEscape::SpecialBan[%i];
					if (%blid == getWord(%str, 0)) {
						messageClient(fcn(Conan), '', "\c4    Attempt to join failed");
						%cl.isBanReject = 1;
						%cl.schedule(10, delete, getWords(%str, 1, getWordCount(%str)));
						schedule(10, 0, eval, "$Pref::Server::maxPlayers--;");
						return;
					}
					%i++;
				}
				echo("    Special case ban not found");
			}

			//schedule(30, 0, updateServerPlayerCount);
		}
		else if (%word $= "NO")
		{
			return parent::onLine(%this, %line);
		}
		return parent::onLine(%this, %line);
	}
};
activatePackage(PrisonEscape_VIP);

function serverCmdSetPlayerCount(%cl, %count) {
	if (!%cl.isSuperAdmin) {
		return;
	}
	$Server::PrisonEscape::maxPlayers = %count;
	$Pref::Server::maxPlayers = %count;
	updateServerPlayerCount();
	PPE_MessageAdmins("!!! - \c3" @ %cl.name @ "\c6 set the max playercount to \c4" @ %count);
}