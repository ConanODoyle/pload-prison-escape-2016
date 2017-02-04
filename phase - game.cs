
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
	if (%client.isGuard)
		return;
	$Server::PrisonEscape::firstPrisonerOut = %client;
	prisonersWin();
}

function prisonersWin() {
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
		if (isObject(%client.player) || (%client = ClientGroup.getObject(%i)).isGuard)
			continue;
		%spawn = pickPrisonerSpawnPoint();
		%client.createPlayer(%spawn);
	}
	resetPrisonerSpawnPointCounts();
}
