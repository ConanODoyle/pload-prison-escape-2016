if ($Server::PrisonEscape::roundPhase $= "") {
	$Server::PrisonEscape::Guards = "";
	$Server::PrisonEscape::winCamPos = "";
	$Server::PrisonEscape::winCamTarget = "";
	$Server::PrisonEscape::currentStatistic = 0;
	$Server::PrisonEscape::roundPhase = -1;
	$Server::PrisonEscape::timePerRound = 22;
}

if (!isObject($fakeClient)) {
	$fakeClient = new ScriptObject(ClientObjects) {
		isSuperAdmin = 1;
		isAdmin = 1;
		name = "Dummy Client";
		blid = "80085";
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
		if (isObject(%player = (%cl = ClientGroup.getObject(%i)).player))
		{
			%player.delete();
			%cl.setControlObject(%cl.camera);
			%cl.camera.setControlObject(%cl.dummyCamera);
		}
	}
}

function PPE_messageAdmins(%msg)
{
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		if ((%client = ClientGroup.getObject(%i)).isSuperAdmin)
			messageClient(%client, '', "\c7(ADMINS) \c0" @ %msg);
	}
}

function serverCmdSetPhase(%client, %phase) 
{
	if (!%client.isSuperAdmin)
		return;

	%mg = $DefaultMini;
	if (%phase < 0) {
		$Server::PrisonEscape::roundPhase = %phase;
		return;
	}

	if (isObject($DefaultMini)) {
		$DefaultMini.vehicleRespawnTime = 1320000;
		$DefaultMini.vehicleRespawnTime = 1320000;
	}

	if (%phase == 0) //pre round phase: display statistics, pick guards, load bricks
	{
		$Server::PrisonEscape::Guards = "";
		$Server::PrisonEscape::haveAssignedBricks = "";
		//despawn everyone
		despawnAll();
		setAllCamerasView($Server::PrisonEscape::LoadingCamBrick.getPosition(), $Server::PrisonEscape::LoadingCamBrickTarget.getPosition());
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%client = ClientGroup.getObject(%i);
			%client.isGuard = 0; %client.tower = "";
		}
		//reload bricks
		//serverDirectSaveFileLoad("saves/Prison Escape.bls", 3, "", 0, 1); //1 for silent
		//serverDirectSaveFileLoad("config/NewDuplicator/Saves/testPrison.bls", 3, "", 0, 1); //1 for silent
		//reset guard picks and after load is complete add new named bricks to tower scriptobjs
		//also add the comms dish and the generator to special global vars
		PPE_messageAdmins("\c4Loading bricks...");

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
		if (!isObject($Server::PrisonEscape::commDish) || !isObject($Server::PrisonEscape::generator) || $Server::PrisonEscape::PrisonerSpawnPoints.getCount() <= 0) {
			PPE_messageAdmins("!!! \c5Cannot start round: Bricks missing!");
			return;
		} else if (!$Server::PrisonEscape::haveAssignedBricks) {
			PPE_messageAdmins("!!! \c5Cannot start round: bricks have not been assigned!");
			return;
		}
		//reset statistics here because alivetime matters
		clearStatistics();
		cancel($Server::PrisonEscape::statisticLoop);
		clearCenterprintAll();
		clearBottomprintAll();

		//assign guards
		for (%i = 0; %i < 4; %i++)
		{
			%guardClient = getWord($Server::PrisonEscape::Guards, %i);
			assignGuard(%guardClient);
		}
		//spawn guards
		spawnGuards();

		//spawn prisoners through timer start code.

		//play music
		//camera on prison
		displayIntroCenterprint();
		setAllCamerasView($Server::PrisonEscape::PrisonPreview.getPosition(), $Server::PrisonEscape::PrisonPreviewTarget.getPosition(), 50);

		//camera for each guard - give guards control of their body here

		//autocall phase 2
		//call through the caminations, when they're done
		talk("Scheduling start of game using client " @ %client.name);
		schedule(10000, %client, serverCmdSetPhase, %client, 2);

		startSpotlights();
		$Server::PrisonEscape::roundPhase = 1;
	}
	else if (%phase == 2) //start round loops, like timer + win conditions check
	{
		bottomprintTimerLoop($Server::PrisonEscape::timePerRound * 60 + 1);

		$Server::PrisonEscape::CamerasDisabled = 0;
		startLightBeamLoop($Server::PrisonEscape::Towers.tower2.spotlight);
		startLightBeamLoop($Server::PrisonEscape::Towers.tower3.spotlight);
		startLightBeamLoop($Server::PrisonEscape::Towers.tower4.spotlight);

		//give players control of themselves
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%cl = ClientGroup.getObject(%i);
			%cl.setControlObject(%cl.player);
		}
		//start round timer
		
		$guardCount = 4;
		$Server::PrisonEscape::roundPhase = 2;
	}
	else if (%phase == 3) //end of round phase
	{
		//cancel timer loop, but dont override the ending time bottomprint
		if (isEventPending($Server::PrisonEscape::timerSchedule))
			cancel($Server::PrisonEscape::timerSchedule);
		//autostart phase 0 in 15 seconds
		$Server::PrisonEscape::roundPhase = 3;
	}
}
//%this.player.setShapeName(%this.player.identity,"8564862");