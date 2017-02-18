
function bottomprintTimerLoop(%timeLeft) {
	if (isEventPending($Server::PrisonEscape::timerSchedule)) {
		cancel($Server::PrisonEscape::timerSchedule);
	}
	if (%timeleft == $Server::PrisonEscape::timePerRound * 60 + 1) {
		spawnDeadPrisoners();
	} else if (%timeLeft % 60 == 0 && %timeleft) {
		spawnDeadInfirmary();
	}
	//display timeleft to everyone
	bottomprintAll("<font:Arial Bold:34><just:center>\c6" @ getTimeString(%timeLeft-1) @ " ", -1, 0);
	if (%timeleft == 0) {
		//guards win
		//schedule(1000, 0, guards)

		//color the bottomprint timer to indicate its up
		//and play win/lose sound on client
		guardsWin();
		return;
	}
	$Server::PrisonEscape::currTime = %timeLeft; //respawn wave every minute
	$Server::PrisonEscape::timerSchedule = schedule(1000, 0, bottomprintTimerLoop, %timeleft-1);
}

registerOutputEvent("fxDTSBrick", "prisonersWin", "", 1);

function fxDTSBrick::prisonersWin(%this, %client) {
	if (%client.isGuard || $Server::PrisonEscape::roundPhase != 2 || $prisonersHaveWon)
		return;
	
	$Server::PrisonEscape::firstPrisonerOut = %client;
	prisonersWin(%this);
}

function pickPrisonerSpawnPoint() {
	%start = getRandom(0, $Server::PrisonEscape::PrisonerSpawnPoints.getCount() - 1);
	%count = $Server::PrisonEscape::PrisonerSpawnPoints.getCount();
	for (%i = 0; %i < %count; %i++)	{
		%index = (%i + %start) % %count;
		%brick = $Server::PrisonEscape::PrisonerSpawnPoints.getObject(%index);
		if (%brick.spawnCount < 2) {
			break;
		}
		%brick = "";
	}
	if (isObject(%brick)) {
		%brick.spawnCount++;
		return %brick.getSpawnPoint();
	} else {
		echo("Can't find a spawnpoint with less than 2 spawns! Resetting...");
		resetPrisonerSpawnPointCounts();
		return $Server::PrisonEscape::PrisonerSpawnPoints.getObject(%start).getSpawnPoint();
	}
}

function resetPrisonerSpawnPointCounts() {
	for (%i = 0; %i < $Server::PrisonEscape::PrisonerSpawnPoints.getCount(); %i++) {
		$Server::PrisonEscape::PrisonerSpawnPoints.getObject(%i).spawnCount = 0;
	}
}

function pickInfirmarySpawnPoint() {
	%start = getRandom(0, $Server::PrisonEscape::InfirmarySpawnPoints.getCount() - 1);
	%count = $Server::PrisonEscape::InfirmarySpawnPoints.getCount();
	for (%i = 0; %i < %count; %i++)	{
		%index = (%i + %start) % %count;
		%brick = $Server::PrisonEscape::InfirmarySpawnPoints.getObject(%index);
		if (%brick.spawnCount < 2) {
			break;
		}
		%brick = "";
	}
	if (isObject(%brick)) {
		%brick.spawnCount++;
		return %brick.getSpawnPoint();
	} else {
		echo("Can't find an infirmary spawnpoint with less than 1 spawn! Resetting...");
		resetInfirmarySpawnPointCounts();
		return $Server::PrisonEscape::InfirmarySpawnPoints.getObject(%start).getSpawnPoint();
	}
}

function resetInfirmarySpawnPointCounts() {
	for (%i = 0; %i < $Server::PrisonEscape::InfirmarySpawnPoints.getCount(); %i++) {
		$Server::PrisonEscape::InfirmarySpawnPoints.getObject(%i).spawnCount = 0;
	}
}

function spawnDeadPrisoners() {
	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		%client = ClientGroup.getObject(%i);
		if (!isObject(%client.player) && !%client.isGuard) {
			%spawn = pickPrisonerSpawnPoint();
			%client.createPlayer(%spawn);
			%client.centerprint("");
		}
	}
	resetPrisonerSpawnPointCounts();
}

function spawnDeadInfirmary() {
	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		%client = ClientGroup.getObject(%i);
		if (!isObject(%client.player) && !%client.isGuard) {
			%spawn = pickInfirmarySpawnPoint();
			%client.createPlayer(%spawn);
			%client.centerprint("");
		}
	}
	resetInfirmarySpawnPointCounts();
}

function validateGameWin() {
	for (%i = 1; %i <= 4; %i++) {
		if (!$Server::PrisonEscape::Towers.tower[%i].isDestroyed)
			break;
	}
	if (%i == 5) {
		prisonersWin();
	}
}

function prisonersWin(%brick) {
	if (isEventPending($Server::PrisonEscape::prisonerWinSchedule))
		cancel($Server::PrisonEscape::prisonerWinSchedule);
	if (isEventPending($Server::PrisonEscape::timerSchedule))
		cancel($Server::PrisonEscape::timerSchedule);

	$prisonersHaveWon = 1;

	//set cameras up
	if (!isObject(%brick)) {
		%winString = "<font:Palatino Linotype:24>\c6All the towers have been destroyed! ";
	} else {
		%winString = "<font:Palatino Linotype:24>\c6The prisoners have escaped! ";
		%id = %brick.getAngleID();
		switch (%id) {
			case 0: %vec = "0 15 5";
			case 1: %vec = "-15 0 5";
			case 2: %vec = "0 -15 5";
			case 3: %vec = "15 0 5";
		}
		%start = %brick.getPosition();
		%end = vectorAdd(%vec, %start);
		setAllCamerasView(%end, %start);
		returnAllPlayerControlCamera();
	}

	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		%client = ClientGroup.getObject(%i);
		if (!%client.isGuard) {
			%client.bottomprint("<font:Arial Bold:34><just:center>\c2" @ getTimeString($Server::PrisonEscape::currTime-1) @ " ", -1, 0);
			%client.centerprint("<font:Arial Bold:34>\c2Prisoners Win! <br>" @ %winString);
		} else {
			%client.bottomprint("<font:Arial Bold:34><just:center>" @ getTimeString($Server::PrisonEscape::currTime-1) @ " ", -1, 0);
			%client.centerprint("<font:Arial Bold:34>Prisoners Win! <br>" @ %winString);
		}
	}
	//playsound on clients

	schedule(10000, 0, serverCmdSetPhase, $fakeClient, 3);
}

function guardsWin() {
	if (isEventPending($Server::PrisonEscape::prisonerWinSchedule))
		cancel($Server::PrisonEscape::prisonerWinSchedule);
	if (isEventPending($Server::PrisonEscape::timerSchedule))
		cancel($Server::PrisonEscape::timerSchedule);

	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		%client = ClientGroup.getObject(%i);
		if (%client.isGuard) {
			%client.bottomprint("<font:Arial Bold:34><just:center>\c2" @ getTimeString($Server::PrisonEscape::currTime-1) @ " ", -1, 0);
			%client.centerprint("<font:Arial Bold:34>\c2Guards Win! <br><font:Palatino Linotype:24>\c6The prisoners didn't make it out! ");
		} else {
			%client.bottomprint("<font:Arial Bold:34><just:center>" @ getTimeString($Server::PrisonEscape::currTime-1) @ " ", -1, 0);
			%client.centerprint("<font:Arial Bold:34>Guards Win! <br><font:Palatino Linotype:24>\c6The prisoners didn't make it out! ");
		}
	}

	//set cameras up

	//playsound on clients

	schedule(10000, 0, serverCmdSetPhase, $fakeClient, 3);
}

package PrisonEscape_GamePhase {
	function fxDTSBrick::killDelete(%this, %player) {
		if ($Server::PrisonEscape::Generator == %this) {
			disableSpotlights(%player.client);
		} else if ($Server::PrisonEscape::CommDish == %this) {
			disableCameras(%player.client);
		}

		return parent::killDelete(%this);
	}
};
activatePackage(PrisonEscape_GamePhase);

registerOutputEvent("fxDTSBrick", "disableSpotlights", "", 1);

function fxDTSBrick::disableSpotlights(%this, %client) {
	disableSpotlights(%client);
}

function disableSpotlights(%client) {
	if (!$Server::PrisonEscape::Towers.tower1.isDestroyed) {
		clearLightBeam($Server::PrisonEscape::Towers.tower1.spotlight);
	}
	if (!$Server::PrisonEscape::Towers.tower2.isDestroyed) {
		clearLightBeam($Server::PrisonEscape::Towers.tower2.spotlight);
	}
	if (!$Server::PrisonEscape::Towers.tower3.isDestroyed) {
		clearLightBeam($Server::PrisonEscape::Towers.tower3.spotlight);
	}
	if (!$Server::PrisonEscape::Towers.tower4.isDestroyed) {
		clearLightBeam($Server::PrisonEscape::Towers.tower4.spotlight);
	}
	if (!isObject(%client)) {
		messageAll('MsgStartUpload', "\c4The spotlights have been disabled!");
	} else {
		messageAll('MsgStartUpload', "\c4The spotlights have been disabled by \c3" @ %client.name @ "\c4!");
	}
}

function disableCameras(%client) {
	if ($Server::PrisonEscape::Towers.tower1.guard.player.isInCamera) {
		serverCmdLight($Server::PrisonEscape::Towers.tower1.guard);
		$Server::PrisonEscape::Towers.tower1.guard.player.setDamageFlash(50);
	}
	if ($Server::PrisonEscape::Towers.tower2.guard.player.isInCamera) {
		serverCmdLight($Server::PrisonEscape::Towers.tower2.guard);
		$Server::PrisonEscape::Towers.tower2.guard.player.setDamageFlash(50);
	}
	if ($Server::PrisonEscape::Towers.tower3.guard.player.isInCamera) {
		serverCmdLight($Server::PrisonEscape::Towers.tower3.guard);
		$Server::PrisonEscape::Towers.tower3.guard.player.setDamageFlash(50);
	}
	if ($Server::PrisonEscape::Towers.tower4.guard.player.isInCamera) {
		serverCmdLight($Server::PrisonEscape::Towers.tower4.guard);
		$Server::PrisonEscape::Towers.tower4.guard.player.setDamageFlash(50);
	}
	$Server::PrisonEscape::CamerasDisabled = 1;
	if (!isObject(%client)) {
		messageAll('MsgStartUpload', "\c4The cameras and spotlight auto tracking have been disabled!");
	} else {
		messageAll('MsgStartUpload', "\c4The cameras and spotlight auto tracking have been disabled by \c3" @ %client.name @ "\c4!");
	}
}

function validatePlayerPosition(%player) {
	%scale = %player.getScale(); //usually 1
	%pos = %player.getHackPosition();
	%box = vectorScale("1.25 1.25 2", getWord(%scale, 2));

	initContainerBoxSearch(%pos, %boxSize, $TypeMasks::fxBrickObjectType);
	if (isObject(%brick = containerSearchNext())) {
		%xOff0 = 0; %xOff1 = 0.8; %xOff2 = 0; %xOff3 = -0.8;
		%yOff0 = 0.8; %yOff1 = 0; %yOff2 = -0.8; %yOff3 = 0;
		for (%i = 0; %i < 4; %i++) {
			%end = vectorAdd(%player.getPosition(), %xOff[%i] SPC %yOff[%i] SPC 0);
			%ray = containerRaycast(%pos, %end, $TypeMasks::fxBrickObjectType);
			if (!isObject(getWord(%ray, 0))) {
				%player.setTransform(%end SPC getWords(%player.getTransform(), 3, 6));
			}
		}
		return 1;
	}
	return 0;
}

function Player::removeItems(%player, %string, %client) {
	if (!isObject(%player)) {
		return;
	}

	for (%i = 0; %i < getWordCount(%string); %i++) {
		%word = getWord(%string, %i);
		for (%j = 0; %j < %player.getDatablock().maxTools; %j++) {
			if (strPos(%player.tool[%j].getName(), %word) >= 0) {
				%player.tool[%i] = "";
				%player.weaponCount--;
				messageClient(%client, 'MsgItemPickup', '', %j, "");
			}
		}
	}
}

registerOutputEvent("Player", "removeItems", "string 200 156", 1);
