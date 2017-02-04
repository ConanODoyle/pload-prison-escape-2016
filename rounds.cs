$Server::PrisonEscape::Guards = "";
$Server::PrisonEscape::winCamPos = "";
$Server::PrisonEscape::winCamTarget = "";
$Server::PrisonEscape::currentStatistic = 0;
$Server::PrisonEscape::roundPhase = -1;

if (!isObject($fakeClient)) {
	$fakeClient = new ScriptObject(ClientObjects) {
		isSuperAdmin = 1;
		isAdmin = 1;
		name = "Dummy Client";
	};
}

exec("./globalcams.cs");

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

function PPE_messageAdmins(%msg)
{
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		if ((%client = ClientGroup.getObject(%i)).isSuperAdmin)
			messageClient(%client, '', "\c7(ADMINS) \c3" @ %msg);
	}
}

function swapStatistics() 
{
	if (isEventPending($Server::PrisonEscape::statisticLoop))
		return;

	%stat = getStatistic();

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
		PPE_messageAdmins("\c4Loading bricks...");
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