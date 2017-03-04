
function bottomprintTimerLoop(%timeLeft) {
	if (isEventPending($Server::PrisonEscape::timerSchedule)) {
		cancel($Server::PrisonEscape::timerSchedule);
	}
	if (%timeleft == $Server::PrisonEscape::timePerRound * 60 + 1) {
		spawnDeadPrisoners();
	} else if (%timeLeft % 90 == 0 && %timeleft) {
		spawnDeadInfirmary();
	}
	//display timeleft to everyone
	bottomprintAll("<font:Arial Bold:34><just:center>\c6" @ getTimeString(%timeLeft-1) @ " ", -1, 0);
	if (%timeleft == 0) {
		guardsWin();
		return; 
	}

	%prisoners = $prisonerCount["Total"] > 1 ? "Prisoners" : "Prisoner";
	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		%cl = ClientGroup.getObject(%i);
		if (isObject(%pl = %cl.player) && isObject(%m = %cl.player.getMountedImage(0)) && %m.getName() $= "LightMachineGunImage") {
			%cl.bottomprint("<font:Arial Bold:34><just:center>\c6" @ getTimeString(%timeLeft-1) @ "<just:right><font:impact:24><color:fff000>Heat <font:impact:28>" @ %pl.heatColor @ %pl.LMGHeat @ "/" @ $LMGMaxHeat, 2, 1);
		}
		if (%cl.isGuard) {
			continue;
		}

		if (!isObject(%cl.player)){
			%cl.bottomprint("<font:Arial Bold:34><just:center>\c6" @ getTimeString(%timeLeft-1) @ " <br><font:Impact:30>\c0Respawn wave in: " @ getTimeString((%timeLeft - 1) % 90));
		} else {
			%underStr = "\c6[\c1" @ %cl.location @ "\c6] <br>\c3" @ $prisonerCount[%cl.location] @ "/" @ $prisonerCount["Total"] SPC %prisoners;
			%cl.bottomprint("<font:Arial Bold:34><just:center>\c6" @ getTimeString(%timeLeft-1) @ " <br><font:Arial Bold:24>" @ %underStr, 2, 1);
		}
	}

	deleteVariables("$prisonerCount*");
	prisonerCountCheck(0);
	if (!isObject($Server::PrisonEscape::CommDish)) {
		$countCheckInProgress = 0;
	}

	$Server::PrisonEscape::currTime = %timeLeft;
	$Server::PrisonEscape::timerSchedule = schedule(1000, 0, bottomprintTimerLoop, %timeleft-1);
}

$alarmGuardsThreshold = 6;

function prisonerCountCheck(%i) {

	if (%i >= ClientGroup.getCount()) {
		$countCheckInProgress = 0;
		%offset = "<br><br><br><br><br><br><font:Arial Bold:24>";

		//guards EWS centerprint
		if (isObject($Server::PrisonEscape::CommDish)) {
			displayPrisonerCountToGuards();
		} else {
			pushCenterprintGuards(%offset @ "Early Warning System Disabled!", 10);
		}
		
		//Prisoners bottomprint
		%timeLeft = $Server::PrisonEscape::currTime;
		for (%i = 0; %i < ClientGroup.getCount(); %i++) {
			%cl = ClientGroup.getObject(%i);
			if (%cl.isGuard) {
				continue;
			}

			%prisoners = $prisonerCount["Total"] > 1 ? "Prisoners" : "Prisoner";
			if (!isObject(%cl.player)) {
				%cl.bottomprint("<font:Arial Bold:34><just:center>\c6" @ getTimeString(%timeLeft-1) @ " <br><font:Impact:30>\c0Respawn wave in: " @ getTimeString((%timeLeft - 1) % 90));
			} else {
				%underStr = "\c6[\c1" @ %cl.location @ "\c6] <br>\c3" @ $prisonerCount[%cl.location] @ "/" @ $prisonerCount["Total"] SPC %prisoners;
				%cl.bottomprint("<font:Arial Bold:34><just:center>\c6" @ getTimeString(%timeLeft-1) @ " <br><font:Arial Bold:24>" @ %underStr, 2, 1);
			}
		}
		return $prisonerCount["Total"];
	}

	$countCheckInProgress = 1;
	//talk("Count Check " @ %i);
	%cl = ClientGroup.getObject(%i);
	if (!isObject(%pl = %cl.player) || %cl.isGuard) {
		schedule(1, 0, prisonerCountCheck, %i + 1);
		return;
	} else if ((%loc = getRegion(%pl)) $= "Outside" || %loc $= "Yard") {
		$prisonerCount["Outside"]++;
	}

	if (%loc !$= "Outside") {
		$prisonerCount[%loc]++;
	}
	$prisonerCount["Total"]++;
	%cl.location = %loc;
	schedule(1, 0, prisonerCountCheck, %i + 1);
}

function displayPrisonerCountToGuards() {
	%offset = "<br><br><br><br><br><br><font:Arial Bold:24>";
	if (!$isAlarmActive && $prisonerCount["Outside"] > $alarmGuardsThreshold) {
		//alarm guards
		serverPlay2D(Beep_Siren_Sound);
		setRiotMusic(1);
		$isAlarmActive = 1;
	} else if ($isAlarmActive && $prisonerCount["Outside"] <= $alarmGuardsThreshold) {
		serverPlay2D(RiotOverSound);
		setRiotMusic(3);
		$isAlarmActive = 0;
	}
	for (%i = 0; %i < $prisonerCount["Outside"]; %i++) {
		%Xstring = "X" SPC %Xstring;
	}
	%print = %offset @ "\c6Prisoners Outside: " @ $prisonerCount["Outside"] + 0 @ "<br><font:Consolas:18>\c3" @ %Xstring;
	%redprint = %offset @ "\c0Prisoners Outside: " @ $prisonerCount["Outside"] + 0 @ "<br><font:Consolas:18>\c3" @ %Xstring;

	if ($isAlarmActive) {
		centerprintGuards(%redprint, 1);
		schedule(500, $Server::PrisonEscape::CommDish, centerprintGuards, %print, 1);
	} else {
		centerprintGuards(%print, 2);
	}
}

function playSoundOnGuards(%sound) {
	for (%i = 1; %i < 5; %i++) {
		%tower = $Server::PrisonEscape::Towers.tower[%i];
		if (!%tower.isDestroyed && isObject(%cl = %tower.guard) && isObject(%cl.player)) {
			%cl.playSound(%sound);
		}
	}
}

function centerprintGuards(%msg, %time) {
	for (%i = 1; %i < 5; %i++) {
		%tower = $Server::PrisonEscape::Towers.tower[%i];
		if (!%tower.isDestroyed && isObject(%cl = %tower.guard) && isObject(%cl.player) && !%cl.isInCamera && !%cl.pushedCenterprint) {
			centerprint(%cl, %msg, %time);
		}
	}
}

function pushCenterprintGuards(%msg, %time) {
	for (%i = 1; %i < 5; %i++) {
		%tower = $Server::PrisonEscape::Towers.tower[%i];
		if (!%tower.isDestroyed && isObject(%cl = %tower.guard) && isObject(%cl.player) && !%cl.isInCamera) {
			pushCenterprint(%cl, %msg, %time);
		}
	}
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
		return %brick.getPosition() SPC getWords(%brick.getSpawnPoint(), 3, 6);
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
			if (!%client.pushedCenterprint) {
				%client.centerprint("");
			}
			%client.spawnTime = getSimTime();
			commandToClient(%client, 'showBricks', 0);
		}
	}
	resetPrisonerSpawnPointCounts();
}

function spawnDeadInfirmary() {
	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		%client = ClientGroup.getObject(%i);
		if (%client.isGuard) {
			continue;
		}

		if (!isObject(%client.player)) {
			%spawn = pickInfirmarySpawnPoint();
			%client.createPlayer(%spawn);
			if (!%client.pushedCenterprint) {
				%client.centerprint("");
			}
			%client.spawnTime = getSimTime();
		}
		// else if (isObject(%client.player) && %client.getControlObject() != %client.player) {
		// 	%client.setControlObject(%client.player);
		// }
		commandToClient(%client, 'showBricks', 0);
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
	
	setStatistic("Winner", "Prisoners");
	//set cameras up
	if (!isObject(%brick)) {
		setStatistic("WinnerMethod", "Towers");
		%winString = "<font:Palatino Linotype:24>\c6All the towers have been destroyed! ";
	} else {
		setStatistic("WinnerMethod", "Wall");
		%winString = "<font:Palatino Linotype:24>\c6The prisoners have escaped! ";
		%id = %brick.getAngleID();
		switch (%id) {
			case 0: %vec = "0 5 2";
			case 1: %vec = "-5 0 2";
			case 2: %vec = "0 -5 2";
			case 3: %vec = "5 0 2";
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

	schedule(100, 0, serverCmdSetPhase, $fakeClient, 3);
}

function guardsWin() {
	if (isEventPending($Server::PrisonEscape::prisonerWinSchedule))
		cancel($Server::PrisonEscape::prisonerWinSchedule);
	if (isEventPending($Server::PrisonEscape::timerSchedule))
		cancel($Server::PrisonEscape::timerSchedule);

	setStatistic("Winner", "Guards");
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

	schedule(100, 0, serverCmdSetPhase, $fakeClient, 3);
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

	%type = $DamageType::Generator;

	if (!isObject(%client)) {
		pushCenterprintAll("<font:Impact:30>\c4The spotlights have been disabled!", 20);
		%msg = $DamageType::SuicideBitmap[%type];
	} else {
		pushCenterprintAll("<font:Impact:30>\c4The spotlights have been disabled by \c3" @ %client.name @ "\c4!", 20);
		%msg = $DamageType::MurderBitmap[%type];
	}

	messageAll('MsgStartUpload', %client.name @ " <bitmap:" @ %msg @ "> [" @ getTimeString($Server::PrisonEscape::currTime-1) @ "]");

	setStatistic("GeneratorDisabled", $Server::PrisonEscape::currTime);
}

function disableCameras(%client) {
	if ($Server::PrisonEscape::Towers.tower1.guard.player.isInCamera) {
		serverCmdLight($Server::PrisonEscape::Towers.tower1.guard);
	}
	if ($Server::PrisonEscape::Towers.tower2.guard.player.isInCamera) {
		serverCmdLight($Server::PrisonEscape::Towers.tower2.guard);
	}
	if ($Server::PrisonEscape::Towers.tower3.guard.player.isInCamera) {
		serverCmdLight($Server::PrisonEscape::Towers.tower3.guard);
	}
	if ($Server::PrisonEscape::Towers.tower4.guard.player.isInCamera) {
		serverCmdLight($Server::PrisonEscape::Towers.tower4.guard);
	}
	
	$Server::PrisonEscape::Towers.tower1.guard.player.setDamageFlash(50);
	$Server::PrisonEscape::Towers.tower2.guard.player.setDamageFlash(50);
	$Server::PrisonEscape::Towers.tower3.guard.player.setDamageFlash(50);
	$Server::PrisonEscape::Towers.tower4.guard.player.setDamageFlash(50);

	$Server::PrisonEscape::CamerasDisabled = 1;
	
	%type = $DamageType::Satellite;

	if (!isObject(%client)) {
		pushCenterprintAll("<font:Impact:30>\c4The cameras and spotlight auto tracking have been disabled!", 20);
		%msg = $DamageType::SuicideBitmap[%type];
	} else {
		pushCenterprintAll("<font:Impact:30>\c4The cameras and spotlight auto tracking have been disabled by \c3" @ %client.name @ "\c4!", 20);
		%msg = $DamageType::MurderBitmap[%type];
	}

	messageAll('MsgStartUpload', %client.name @ " <bitmap:" @ %msg @ "> [" @ getTimeString($Server::PrisonEscape::currTime-1) @ "]");

	setStatistic("CommDishDestroyed", $Server::PrisonEscape::currTime);
}

function Player::removeItems(%player, %string, %client) {
	if (!isObject(%player)) {
		return;
	}

	for (%i = 0; %i < getWordCount(%string); %i++) {
		%word = getWord(%string, %i);
		for (%j = 0; %j < %player.getDatablock().maxTools; %j++) {
			if (strPos(%player.tool[%j].getName(), %word) >= 0) {
				serverCmdunUseTool(%client);
				%player.tool[%j] = "";
				%player.weaponCount--;
				messageClient(%client, 'MsgItemPickup', '', %j, "");
			}
		}
	}
}

registerOutputEvent("Player", "removeItems", "string 200 156", 1);

function spawnEmittersLoop(%i) {
	if (isEventPending($infoEmitterLoop) || $Server::PrisonEscape::roundPhase < 1) {
		return;
	}

	%brick = $Server::PrisonEscape::InfoBricks.getObject(%i);
	%name = getSubStr(%brick.getName(), 1, strLen(%brick.getName()));
	%type = getSubStr(%name, strPos(%name, "info") + 4, strLen(%name));
	switch$ (%type) {
		case "Bronson": %data = InfoBronsonProjectile;
		case "Bucket": %data = InfoBucketProjectile;
		case "Tray": %data = InfoTrayProjectile;
		case "LaundryCart": %data = InfoLaundryCartProjectile;
		case "Generator": %data = InfoGeneratorProjectile;
		case "SniperRifle": %data = InfoSniperRifleProjectile;
		case "Cameras": %data = InfoCamerasProjectile;
		case "SmokeGrenade": %data = InfoSmokeGrenadeProjectile;
		case "SatDish": %data = InfoSatDishProjectile;
		case "Soap": %data = InfoSoapProjectile;
		default: %data = "";
	}
	if (isObject(%data)) {
		%proj = new Projectile(Info) {
			datablock = %data;
			initialPosition = %brick.getPosition();
			initialVelocity = "0 0 1";
		};
		%proj.explode();
		$infoEmitterLoop = schedule(500, 0, spawnEmittersLoop, (%i + 1) % $Server::PrisonEscape::InfoBricks.getCount());
	} else {
		$infoEmitterLoop = schedule(1, 0, spawnEmittersLoop, (%i + 1) % $Server::PrisonEscape::InfoBricks.getCount());
	}
}

function killTower(%id) {
	%tower = $Server::PrisonEscape::Towers.tower[%id];
	%tower.isDestroyed = 1;
	%client = %tower.guard;

	setStatistic("Tower" @ %id @ "Destroyed", $Server::PrisonEscape::currTime);

	//remove the guard's items
	if (isObject(%client.player))
		%client.player.kill();

	//destroy the bricks but sequentially as to not lag everyone to death
	%tower.destroy();
	pushCenterPrintAll("<font:Impact:40>\c4Tower \c3" @ %id @ "\c4 has fallen!", 20);
	
	%type = $DamageType::MurderBitmap[$DamageType::Tower];
	messageAll('', "<bitmap:" @ %type @ "> " @ %id @ " [" @ getTimeString($Server::PrisonEscape::currTime-1) @ "]");
}

function validateTower(%id, %brick) {
	%tower = $Server::PrisonEscape::Towers.tower[%id];
	%tower.remove(%brick);
	if (%tower.getCount() <= %tower.origCount - 4) {
		killTower(%id);
	}
	validateGameWin();
}

function SimSet::destroy(%this) {
	if (%this.getCount() <= 0)
		return;
	%brick = %this.getObject(%this.getCount()-1);
	%brick.fakeKillBrick((getRandom()-0.5)*20 SPC (getRandom()-0.5)*20 SPC 15, 2);
	%brick.schedule(2000, delete);
	%this.remove(%brick);
	if (isObject(%brick.item))
		%brick.item.delete();
	if (isObject(%brick.vehicle))
		%brick.vehicle.kill();
	serverPlay3D("brickBreakSound", %brick.getPosition());

	%this.schedule(1, destroy);
}

function serverCmdSpawnItem(%cl, %item) {
	if (!%cl.isJanitor) {
		return;
	} else if (!isObject(%item) || !isObject(%cl.player)) {
		return;
	}

	%cl.player.addItem(%item, %cl);
	messageAll('', "\c4The janitor has left some items around the prison!");
}

function serverCmdGiveGuardsItem(%cl, %item) {
	if (!%cl.isSuperAdmin || !isObject(%item)) {
		return;
	}

	if (isObject(%pl = $Server::PrisonEscape::Towers.tower1.guard.player)) {
		%pl.addItem(%item, $Server::PrisonEscape::Towers.tower1.guard);
	}
	if (isObject(%pl = $Server::PrisonEscape::Towers.tower2.guard.player)) {
		%pl.addItem(%item, $Server::PrisonEscape::Towers.tower2.guard);
	}
	if (isObject(%pl = $Server::PrisonEscape::Towers.tower3.guard.player)) {
		%pl.addItem(%item, $Server::PrisonEscape::Towers.tower3.guard);
	}
	if (isObject(%pl = $Server::PrisonEscape::Towers.tower4.guard.player)) {
		%pl.addItem(%item, $Server::PrisonEscape::Towers.tower4.guard);
	}
}

function pushCenterprint(%cl, %msg, %time) {
	%cl.pushedCenterprint = 1;
	centerprint(%cl, %msg, %time);
	schedule(%time * 1000, %cl, popCenterprint, %cl);
}

function popCenterprint(%cl) {
	%cl.pushedCenterprint = 0;
}

function pushCenterprintAll(%msg, %time) {
	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		%cl = ClientGroup.getObject(%i);
		%cl.pushedCenterprint = 1;
		centerprint(%cl, %msg, %time);
		schedule(%time * 1000, %cl, popCenterprint, %cl);
	}
}

function popCenterprintAll(%cl) {
	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		%cl = ClientGroup.getObject(%i);
		popCenterprint(%cl);
	}
}