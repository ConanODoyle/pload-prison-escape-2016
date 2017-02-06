
function bottomprintTimerLoop(%timeLeft)
{
	if (isEventPending($Server::PrisonEscape::timerSchedule))
		cancel($Server::PrisonEscape::timerSchedule);
	//display timeleft to everyone
	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		ClientGroup.getObject(%i).bottomprint("<font:Arial Bold:34><just:center>\c6" @ getTimeString(%timeLeft-1) @ " ", -1, 0);
	}

	if (%timeleft == 0)
	{
		//one final check if prisoners are in win zone

		//guards win
		//schedule(1000, 0, guards)

		//color the bottomprint timer to indicate its up
		//and play win/lose sound on client
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%client = ClientGroup.getObject(%i);
			if (%client.isGuard)
			{
				%client.bottomprint("<font:Arial Bold:34><just:center>\c2" @ getTimeString(%timeLeft) @ " ", -1, 0);
				%client.centerprint("<font:Arial Bold:34>\c2Guards Win! ");
			}
			else
			{
				%client.bottomprint("<font:Arial Bold:34><just:center>" @ getTimeString(%timeLeft) @ " ", -1, 0);
				%client.centerprint("<font:Arial Bold:34>Guards Win! ");
			}
		}
		return;
	}
	$Server::PrisonEscape::currTime = %timeLeft; //respawn wave every 3 minutes
	$Server::PrisonEscape::timerSchedule = schedule(1000, 0, bottomprintTimerLoop, %timeleft-1);
}

function guardsWin() {
	if (isEventPending($Server::PrisonEscape::prisonerWinSchedule))
		cancel($Server::PrisonEscape::prisonerWinSchedule);
	if (isEventPending($Server::PrisonEscape::timerSchedule))
		cancel($Server::PrisonEscape::timerSchedule);

	//set cameras up

	//playsound on clients
}

registerOutputEvent("fxDTSBrick", "prisonersWin", "", 1);

function fxDTSBrick::prisonersWin(%this, %client) 
{
	if (%client.isGuard || $Server::PrisonEscape::roundPhase != 2)
		return;
	$Server::PrisonEscape::firstPrisonerOut = %client;
	prisonersWin(%this);
}

function prisonersWin(%brick) {
	if (isEventPending($Server::PrisonEscape::prisonerWinSchedule))
		cancel($Server::PrisonEscape::prisonerWinSchedule);
	if (isEventPending($Server::PrisonEscape::timerSchedule))
		cancel($Server::PrisonEscape::timerSchedule);

	//set cameras up

	//playsound on clients
}

function spawnDeadPrisoners()
{
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		if (isObject(%client.player) || %client.isGuard)
			continue;
		%spawn = pickPrisonerSpawnPoint();
		%client.createPlayer(%spawn);
		%client.centerprint("");
	}
	resetPrisonerSpawnPointCounts();
}

function pickPrisonerSpawnPoint() 
{
	%start = getRandom(0, $Server::PrisonEscape::PrisonerSpawnPoints.getCount() - 1);
	%count = $Server::PrisonEscape::PrisonerSpawnPoints.getCount();
	for (%i = 0; %i < %count; %i++)
	{
		%index = (%i + %start) % %count;
		%brick = $Server::PrisonEscape::PrisonerSpawnPoints.getObject(%index);
		if (%brick.spawnCount < 2) 
		{
			break;
		}
		%brick = "";
	}
	if (isObject(%brick))
	{
		%brick.spawnCount++;
		return %brick.getSpawnPoint();
	}
	else
	{
		echo("Can't find a spawnpoint with less than 2 spawns! Resetting...");
		resetPrisonerSpawnPointCounts();
		return $Server::PrisonEscape::PrisonerSpawnPoints.getObject(%start).getSpawnPoint();
	}
}

function resetPrisonerSpawnPointCounts()
{
	for (%i = 0; %i < $Server::PrisonEscape::PrisonerSpawnPoints.getCount(); %i++)
	{
		$Server::PrisonEscape::PrisonerSpawnPoints.getObject(%i).spawnCount = 0;
	}
}