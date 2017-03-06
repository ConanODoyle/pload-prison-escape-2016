if ($Server::PrisonEscape::roundPhase $= "") {
	$Server::PrisonEscape::Guards = "";
	$Server::PrisonEscape::winCamPos = "";
	$Server::PrisonEscape::winCamTarget = "";
	$Server::PrisonEscape::currentStatistic = 0;
	$Server::PrisonEscape::roundPhase = -1;
	$Server::PrisonEscape::timePerRound = 20;
}

if (!isObject($fakeClient)) {
	$fakeClient = new ScriptObject(ClientObjects) {
		isSuperAdmin = 1;
		isAdmin = 1;
		name = "Dummy Client";
		bl_id = "80085";
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
		if (isObject(%pl = (%cl = ClientGroup.getObject(%i)).player))
		{
			%pl.delete();
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

function serverCmdSetPhase(%cl, %phase) 
{
	if (!%cl.isSuperAdmin)
		return;

	%mg = $DefaultMini;
	if (%phase < 0) {
		$Server::PrisonEscape::roundPhase = %phase;
		cancel($Server::PrisonEscape::statisticLoop);
		cancel($Server::PrisonEscape::timerSchedule);
		cancel($nextRoundPhaseSchedule);
		return;
	}

	cancel($nextRoundPhaseSchedule);

	if (isObject($DefaultMini)) {
		$DefaultMini.vehicleRespawnTime = 660000;
	}

	if (%phase == 0) //pre round phase: display statistics, pick guards, load bricks
	{
		$Server::PrisonEscape::roundPhase = 0;
		despawnAll();
		if (isEventPending($Server::PrisonEscape::timerSchedule))
			cancel($Server::PrisonEscape::timerSchedule);

		$prisonersHaveWon = 0;
		$Server::PrisonEscape::Guards = "";
		$Server::PrisonEscape::haveAssignedBricks = "";
		//despawn everyone
		displayLogo($Server::PrisonEscape::LoadingCamBrick.getPosition(), $Server::PrisonEscape::LoadingCamBrickTarget.getPosition(), LogoClosedShape, 1);
		//setAllCamerasView($Server::PrisonEscape::LoadingCamBrick.getPosition(), $Server::PrisonEscape::LoadingCamBrickTarget.getPosition());
		spawnDeadLobby();

		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%t = ClientGroup.getObject(%i);
			%t.isGuard = 0; %t.tower = "";
			%t.setScore(0);
			commandToClient(%t, 'showBricks', 0);
		}
		//reload bricks
		//serverDirectSaveFileLoad("saves/Prison Escape.bls", 3, "", 0, 1); //1 for silent
		//serverDirectSaveFileLoad("config/NewDuplicator/Saves/testPrison.bls", 3, "", 0, 1); //1 for silent
		//reset guard picks and after load is complete add new named bricks to tower scriptobjs
		//also add the comms dish and the generator to special global vars
		PPE_messageAdmins("\c4Loading bricks...");
		serverDirectSaveFileLoad("saves/Autosaver/prison.bls", 3, "", 0, 1);

		//assignBricks();
		//reset guard list
		$Server::PrisonEscape::Guards = "";

		//start statistics display loop
		giveRandomHair(findClientByBL_ID($bestPrisoner));
		messageAll('', "\c3" @ findClientByBL_ID($bestPrisoner).name @ "\c6 unlocked a hairdo for being the <color:ff8724>MVP Prisoner\c6!");
		giveRandomHair(findClientByBL_ID($bestGuard));
		messageAll('', "\c3" @ findClientByBL_ID($bestGuard).name @ "\c6 unlocked a hairdo for being the \c1MVP Guard\c6!");

		if (getStatistic("Winner") $= "Guards") {
			for (%i = 1; %i < 5; %i++) {
				%guards = getStatistic("Tower" @ %i @ "Guard");
				for (%j = 0; %j < getWordCount(%guards); %j++) {
					giveRandomHair(findClientByBL_ID(getWord(%guards, %j)));
					messageAll('', "\c3" @ findClientByBL_ID(getWord(%guards, %j)).name @ "\c6 unlocked a hairdo for winning as Guard!");
				}
			}
		}
		swapStatistics();
		displayRoundLoadingInfo();
	} 
	else if (%phase == 1) //start the round caminations and spawn everyone but dont give them control of their bodies yet
	{
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%t = ClientGroup.getObject(%i);
			if (%t.isCustomizing) {
				stopPlayerHairCustomization(%t.player);
			}
		}
		spawnKillGround();
		despawnAll();
		
		setAllCamerasView($Server::PrisonEscape::LoadingCamBrick.getPosition(), $Server::PrisonEscape::LoadingCamBrickTarget.getPosition(), 50);
		displayLogo($Server::PrisonEscape::LoadingCamBrick.getPosition(), $Server::PrisonEscape::LoadingCamBrickTarget.getPosition(), LogoOpenShape, 0);
		$LogoShape.setScale("2.2 2.2 2.2");
		$LogoShape.schedule(100, setScale, "2 2 2");
		serverPlay3d(Beep_Siren_Sound, $LogoDish.getPosition());
		serverPlay3d(BrickBreakSound, $LogoDish.getPosition());
		schedule(50, 0, serverPlay3d, BrickBreakSound, $LogoDish.getPosition());
		schedule(100, 0, serverPlay3d, BrickBreakSound, $LogoDish.getPosition());

		cancel($Server::PrisonEscape::statisticLoop);
		clearStatistics();
		clearCenterprintAll();
		clearBottomprintAll();

		schedule(2000, 0, displayIntroCenterprint);
		$nextRoundPhaseSchedule = schedule(5000, 0, serverCmdSetPhase, $fakeClient, 1.5);
	}
	else if (%phase == 1.5)
	{
		$Server::PrisonEscape::roundPhase = 2;
		for (%i = 0; %i < ClientGroup.getCount(); %i++) {
			%cl = ClientGroup.getObject(%i);
			%cl.camera.setWhiteOut(0.5);
		}
		setRiotMusic(3);
		if (!isObject($Server::PrisonEscape::commDish) || !isObject($Server::PrisonEscape::generator) || $Server::PrisonEscape::PrisonerSpawnPoints.getCount() <= 0) {
			PPE_messageAdmins("!!! \c5Cannot start round: Bricks missing!");
			return;	
		} else if (!$Server::PrisonEscape::haveAssignedBricks) {
			PPE_messageAdmins("!!! \c5Cannot start round: bricks have not been assigned!");
			return;
		}
		//reset statistics here because alivetime matters

		//assign guards
		for (%i = 0; %i < 4; %i++)
		{
			%guardClient = getWord($Server::PrisonEscape::Guards, %i);
			assignGuard(%guardClient);
		}
		//spawn guards
		spawnGuards();
		spawnEmittersLoop(0);

		//spawn prisoners through timer start code.

		//play music
		//camera on prison
		displayIntroCenterprint();
		setAllCamerasView($Server::PrisonEscape::PrisonPreview.getPosition(), $Server::PrisonEscape::PrisonPreviewTarget.getPosition(), 50);

		//camera for each guard - give guards control of their body here

		//autocall phase 2
		//call through the caminations, when they're done
		$nextRoundPhaseSchedule = schedule(10000, $fakeClient, serverCmdSetPhase, $fakeClient, 2);

		startSpotlights();
	}
	else if (%phase == 2) //start round loops, like timer + win conditions check
	{
		if (isEventPending($Server::PrisonEscape::timerSchedule))
			cancel($Server::PrisonEscape::timerSchedule);
		if (isEventPending($Server::PrisonEscape::statisticLoop))
			cancel($Server::PrisonEscape::statisticLoop);

		$Server::PrisonEscape::roundPhase = 2;
		bottomprintTimerLoop($Server::PrisonEscape::timePerRound * 60 + 1);

		$Server::PrisonEscape::CamerasDisabled = 0;
		spawnEmittersLoop(0);

		//give players control of themselves
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%cl = ClientGroup.getObject(%i);
			%cl.setControlObject(%cl.player);
		}
		//start round timer
		
		$guardCount = 4;
	}
	else if (%phase == 3) //end of round phase
	{
		//cancel timer loop, but dont override the ending time bottomprint
		if (isEventPending($Server::PrisonEscape::timerSchedule))
			cancel($Server::PrisonEscape::timerSchedule);
		//autostart phase 0 in 15 seconds
		$Server::PrisonEscape::roundPhase = 3;

		returnAllPlayerControlCamera();
		$nextRoundPhaseSchedule = schedule(15000, 0, serverCmdSetPhase, $fakeClient, 0);
		collectStatistics();
		//clearStatistics();
		if (isObject(SM_Music)) {
			SM_Music.delete();
		}
	}
}
//%this.player.setShapeName(%this.player.identity,"8564862");