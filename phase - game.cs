
function bottomprintTimerLoop(%timeLeft) {
	if (isEventPending($Server::PrisonEscape::timerSchedule)) {
		cancel($Server::PrisonEscape::timerSchedule);
	}
	if (%timeLeft % 60 == 0) {
		spawnDeadPrisoners();
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
		setAllCamerasView(%end, %start, 0, 90);
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
}