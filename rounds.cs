
$Server::PrisonEscape::Guards = "";
$Server::PrisonEscape::winCamPos = "";
$Server::PrisonEscape::winCamTarget = "";
$Server::PrisonEscape::currentStatistic = 0;
$Server::PrisonEscape::roundPhase = -1;

$fakeClient = new ScriptObject() {
	isSuperAdmin = 1;
};

////////////////////////////
//////////preround//////////
////////////////////////////


function serverCmdAddGuard(%client, %name) 
{
	if (!%client.isSuperAdmin)
		return;

	$Server::PrisonEscape::Guards = trim($Server::PrisonEscape::Guards SPC findclientbyname(%name));
	messageAdmins(%client, '', "\c7" @ %cl.name @ " added " @ findclientbyname(%name).name @ " to the guard list.");
	displayRoundLoadingInfo();
}

function serverCmdRemoveGuard(%client, %name)
{
	if (!%client.isSuperAdmin)
		return;

	%guard = findclientbyname(%name);
	$Server::PrisonEscape::Guards = strReplace($Server::PrisonEscape::Guards, %guard, "");
	$Server::PrisonEscape::Guards = strReplace($Server::PrisonEscape::Guards, "  ", " ");
	$Server::PrisonEscape::Guards = trim($Server::PrisonEscape::Guards);
	messageAdmins(%client, '', "\c7" @ %cl.name @ " removed " @ findclientbyname(%name).name @ " from the guard list.");
	displayRoundLoadingInfo();
}

function getGuardNames()
{
	%list = "<just:right>";
	for (%i = 0; %i < getWordCount($Server::PrisonEscape::Guards); %i++)
	{
		%list = %list @ "\c2" @ (getWord($Server::PrisonEscape::Guards, %i).name) @ " - Guard " @ (%i + 1) @ " <br>";
	}

	for (%i = getWordCount($Server::PrisonEscape::Guards); %i < 4; %i++)
	{
		%list = %list @ "\c6NONE - Guard " @ (%i + 1) @ " <br>";
	}

	return %list;
}

function displayRoundLoadingInfo() 
{
	%statisticString = $Server::PrisonEscape::statisticString;
	%guards1 = getGuardNames();
	%guards2 = getSubStr(%guards1, strPos(%guards1, "Guard 2 <br>") + 12, strLen(%guards1));
	%guards1 = getSubStr(%guards1, 0, strPos(%guards1, "Guard 2 <br>") + 12);
	%centerprintString = "<font:Arial Bold:20>" @ %guards1 @ "<just:center>\c3" @ %statisticString @ " <just:right>\c6" @ %guards2;
	centerprintAll(%centerprintString);
	bottomprintAll(generateBottomPrint(), -1, 1);
}

function generateBottomPrint() 
{
	%header = "<just:center><font:Arial Bold:34><shadowcolor:666666><shadow:0:4><color:E65714>JailBreak! <br><font:Arial Bold:26>\c7-      - <br>";
	%footer = "<shadow:0:3><color:ffffff>Please wait until the next round to play<font:Impact:1><br>";
	return %header @ %footer;
}

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

///////////////////////////////
//////////duringround//////////
///////////////////////////////

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

/////////////////////////////
//////////postround//////////
/////////////////////////////

exec("./globalcams.cs");
//function setAllCamerasView(%camPos, %targetPos)
//function setCameraViewLoop(%transform, %i, %nocontrol)
//function allCameraPan(%pos1, %pos2, %speed, %targetPos)

/////////////////////////////////
////////Generic Functions////////
/////////////////////////////////

function despawnAll() 
{
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		if (isObject(%player = ClientGroup.getObject(%i).player))
		{
			%player.delete();
		}
	}
}

function messageAdmins(%msg)
{
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		if ((%client = ClientGroup.getObject(%i)).isSuperAdmin)
			messageClient(%client, '', %msg);
	}
}

function swapStatistics() 
{
	if (isEventPending($Server::PrisonEscape::statisticLoop))
		return;

	switch ($Server::PrisonEscape::currentStatistic)
	{
		case 0: %stat = "Sharpshooter\c6: " 	@ 
			(strLen($Server::PrisonEscape::SharpshooterGuard.name) > 10 ? getSubStr($Server::PrisonEscape::SharpshooterGuard.name, 9) @ "." : $Server::PrisonEscape::SharpshooterGuard.name)
			SPC "-" SPC ($Server::PrisonEscape::TopAcc <= 0 ? 0 : $Server::PrisonEscape::TopAcc) SPC "acc";
		case 1: %stat = "Escape Artist\c6: " 	@ 
			(strLen($Server::PrisonEscape::MVPPrisoner.name) > 10 ? getSubStr($Server::PrisonEscape::MVPPrisoner.name, 9) @ "." : $Server::PrisonEscape::MVPPrisoner.name)
			SPC "-" SPC ($Server::PrisonEscape::TopChisel <= 0 ? 0 : $Server::PrisonEscape::TopChisel) SPC "bricks";
		case 2: %stat = "Riot Control\c6: "	@
			(strLen($Server::PrisonEscape::MVPGuard.name) > 10 ? getSubStr($Server::PrisonEscape::MVPGuard.name, 9) @ "." : $Server::PrisonEscape::MVPGuard.name)
			SPC "-" SPC ($Server::PrisonEscape::MVPGuard.getScore() <= 0 ? 0 : $Server::PrisonEscape::MVPGuard.getScore()) SPC "kills";
		case 3: %stat = "Guard Messages Sent\c6: " @ ($Server::PrisonEscape::GuardMessagesSent <= 0 ? 0 : $Server::PrisonEscape::GuardMessagesSent);
		case 4: %stat = "Prisoner Messages Sent\c6: " @ ($Server::PrisonEscape::PrisonerMessagesSent <= 0 ? 0 : $Server::PrisonEscape::PrisonerMessagesSent);
		case 5: %stat = "Prisoner Deaths\c6: " @ ($Server::PrisonEscape::PrisonerDeaths <= 0 ? 0 : $Server::PrisonEscape::PrisonerDeaths);
		case 6: %stat = "Bricks Destroyed\c6: " @ ($Server::PrisonEscape::BricksDestroyed <= 0 ? 0 : $Server::PrisonEscape::BricksDestroyed);
		case 7: %stat = "Bullets Fired\c6: " @ ($Server::PrisonEscape::SniperRifleBullets <= 0 ? 0 : $Server::PrisonEscape::SniperRifleBullets);
		case 8: %stat = "Chisel Swings\c6: " @ ($Server::PrisonEscape::ChiselAttacks <= 0 ? 0 : $Server::PrisonEscape::ChiselAttacks);
		case 9: %stat = "Trays Used\c6: " @ ($Server::PrisonEscape::TraysUsed <= 0 ? 0 : $Server::PrisonEscape::TraysUsed);
		case 10: %stat = "Buckets Used\c6: " @ ($Server::PrisonEscape::BucketsUsed <= 0 ? 0 : $Server::PrisonEscape::BucketsUsed);
		case 11: %stat = "Steaks Eaten\c6: " @ ($Server::PrisonEscape::SteaksEaten <= 0 ? 0 : $Server::PrisonEscape::SteaksEaten);
	}

	$Server::PrisonEscape::statisticString = %stat;
	$Server::PrisonEscape::currentStatistic++;
	$Server::PrisonEscape::currentStatistic %= 12;
	$Server::PrisonEscape::statisticLoop = schedule(6000, 0, swapStatistics);
	displayRoundLoadingInfo();
}

function serverCmdSetPhase(%client, %phase) 
{
	if (!%client.isSuperAdmin)
		return;

	if (%phase == 0) //pre round phase: display statistics, pick guards, load bricks
	{
		//despawn everyone
		setAllCamerasView($Server::PrisonEscape::LoadingCamBrick.getPosition(), $Server::PrisonEscape::LoadingCamBrickTarget.getPosition());
		despawnAll();
		//reload bricks
		serverDirectSaveFileLoad("saves/Prison Escape.bls", 3, "", 0, 1); //1 for silent
		//reset guard picks and after load is complete add new named bricks to tower scriptobjs
		//also add the comms dish and the generator to special global vars
		messageAdmins("\c4Loading bricks...");
		$Server::PrisonEscape::generator = 0;
		$Server::PrisonEscape::commDish = 0;
		//reset the spawn group
		$Server::PrisonEscape::PrisonerSpawnPoints.delete();
		$Server::PrisonEscape::PrisonerSpawnPoints = new ScriptObject()
		{
			count = 0;
		};
		//assignBricks();
		//reset guard list
		$Server::PrisonEscape::Guards = "";

		//start statistics display loop
		calculateStatistics();
		swapStatistics();
		displayRoundLoadingInfo();
		$Server::PrisonEscape::roundPhase = 0;
	} 
	else if (%phase == 1) //start the round caminations and spawn everyone but dont give them control of their bodies yet
	{
		//reset statistics here because alivetime matters
		clearStatistics();
		cancel($Server::PrisonEscape::statisticLoop);
		clearCenterprintAll();
		clearBottomprintAll();

		//assign guards
		for (%i = 0; %i < 4; %i++)
		{
			%client = getWord($Server::PrisonEscape::Guards, %i);
			assignGuard(%client);
		}
		//spawn guards
		//write guard spawn function to call an individual guard spawn function separately.
		//allows us to assign guards mid round if anyone leaves (usually ourselves)
		//ploark-XRanan's Prison Escape!
		spawnGuards();

		//spawn prisoners ensure no double spawnpoint spawns??
		//spawn prisoners through timer start code.

		//play music
		//camera on prison

		//camera for each guard - give guards control of their body here

		//autocall phase 2
		//call through the caminations, when they're done
		$Server::PrisonEscape::roundPhase = 1;
	}
	else if (%phase == 2) //start round loops, like timer + win conditions check
	{
		//turn on all the spotlights
		//iterate through brickgroup ntname "tower[#]" and manually toggle them on.

		//give players control of themselves
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%cl = ClientGroup.getObject(%i);
			%cl.setControlObject(%cl.player);
		}
		//start round timer
		bottomprintTimerLoop($Server::PrisonEscape::timePerRound * 60 + 1);
		//create win trigger zones	
		$guardCount = 4;
		prisonersWinLoop(0);
		$Server::PrisonEscape::roundPhase = 2;
	}
	else if (%phase == 3) //end of round phase
	{
		//cancel timer loop, but dont override the ending time bottomprint
		if (isEventPending($Server::PrisonEscape::bottomprintTimerLoop))
			cancel($Server::PrisonEscape::bottomprintTimerLoop);
		if (isEventPending($Server::PrisonEscape::prisonersWinLoop))
			cancel($Server::PrisonEscape::prisonersWinLoop);
		
		//assign camera, but dont remove player control so everyone can climb out and run and stuff
		//set fov really low (but save fov beforehand) so we can play emitter effects without letting people run in front of it
		//then of course reset back to normal.
		//play round end music

		//autostart phase 0 in 15 seconds

		$Server::PrisonEscape::roundPhase = 3;
	}
}
//%this.player.setShapeName(%this.player.identity,"8564862");