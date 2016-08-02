////////////////////////////
//////////preround//////////
////////////////////////////

$Server::PrisonEscape::Guards = "";

function serverCmdAddGuard(%client, %name) 
{
	if (!%client.isSuperAdmin)
		return;

	$Server::PrisonEscape::Guards = trim($Server::PrisonEscape::Guards SPC findclientbyname(%name));
	messageClient(%client, '', "\c7Added " @ findclientbyname(%name).name @ " to the guard list.");
	displayRoundLoadingInfo();
	talk($Server::PrisonEscape::Guards);
}

function serverCmdRemoveGuard(%client, %name)
{
	if (!%client.isSuperAdmin)
		return;

	%guard = findclientbyname(%name);
	$Server::PrisonEscape::Guards = strReplace($Server::PrisonEscape::Guards, %guard, "");
	$Server::PrisonEscape::Guards = strReplace($Server::PrisonEscape::Guards, "  ", " ");
	$Server::PrisonEscape::Guards = trim($Server::PrisonEscape::Guards);
	messageClient(%client, '', "\c7Removed " @ findclientbyname(%name).name @ " from the guard list.");
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
	echo(%guards1);
	echo(%guards2);
	%centerprintString = "<font:Arial Bold:20>" @ %guards1 @ "<just:center>" @ %statisticString @ " <just:right>\c6" @ %guards2;
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
	//display timeleft to everyone
	%min = mFloor(%timeLeft / 60);
	%sec = %timeleft % 60;
	bottomprintAll("<font:Arial Bold:24><just:center>\c6" @ %min @ ":" @ %sec @ " ", -1, 1);

	if (%timeleft == 0)
	{
		//one final check if prisoners are in win zone

		//guards win in one second

		//color the bottomprint timer to indicate its up
		//and play win/lose sound on client
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%client = ClientGroup.getObject(%i);
			if (%client.isGuard)
			{
				bottomprint(%client, "<font:Arial Bold:24><just:center>\c2" @ %min @ ":" @ %sec @ " ", -1, 1);
			}
			else
			{
				bottomprint(%client, "<font:Arial Bold:24><just:center>" @ %min @ ":" @ %sec @ " ", -1, 1);
			}
		}
	}

	$Server::PrisonEscape::timerSchedule = schedule(1000, 0, bottomprintTimerLoop, %timeleft-1);
}

$guardCount = 4;
function prisonersWinLoop(%i)
{
	if (%i >= ClientGroup.getCount())
	{
		%i = 0;
		//if guards are all dead prisoners win too!
		if ($guardCount <= 0)
			//prisoners win
		$guardCount = 0;
	}

	%client = ClientGroup.getObject(%i);
	if (isObject(%player = %client.player) && !%client.isGuard)
	{
		%pos = %player.getPosition();
		%x = getWord(%pos, 0);
		%y = getWord(%pos, 1);
		if (/*%x or %y not within bounds*/)
			//prisoners win
	} 
	else if (%client.isGuard && isObject(%player))
		$guardCount++;

	$Server::PrisonEscape::prisonerWinSchedule = schedule(0, 0, prisonersWinLoop, %i + 1);
}


/////////////////////////////
//////////postround//////////
/////////////////////////////


function setAllCamerasView(%camPos, %targetPos)
{
	//calculate the position and rotation of camera
	%pos = %camPos;
	%delta = vectorSub(%targetPos, %pos);
	%deltaX = getWord(%delta, 0);
	%deltaY = getWord(%delta, 1);
	%deltaZ = getWord(%delta, 2);
	%deltaXYHyp = vectorLen(%deltaX SPC %deltaY SPC 0);

	%rotZ = mAtan(%deltaX, %deltaY) * -1; 
	%rotX = mAtan(%deltaZ, %deltaXYHyp);

	%aa = eulerRadToMatrix(%rotX SPC 0 SPC %rotZ); //this function should be called eulerToAngleAxis...
	%camTransform = %pos SPC %aa;

	//apply this on everyone
	setCameraViewLoop(%camTransform, 0, 1);
}

function setCameraViewLoop(%transform, %i, %nocontrol)
{
	if (%i >= ClientGroup.getCount())
		return;
	%client = ClientGroup.getObject(%i);
	%camera = %client.camera;
	
	%client.setControlObject(%camera);
	%camera.setTransform(%transform);();

	%camera.setFlyMode	%camera.mode = "Observer";
	if (%nocontrol)
		%camera.setControlObject(%client.dummyCamera);

	schedule(0, 0, setCameraViewLoop, %transform, %i+1, %nocontrol);
}

/////////////////////////////////
////////Generic Functions////////
/////////////////////////////////

function despawnAll() 
{
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		if (isObject(%player = %ClientGroup.getObject(%i).player))
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

$Server::PrisonEscape::currentStatistic = 0;
function swapStatistics() 
{
	switch ($Server::PrisonEscape::currentStatistic)
	{
		case 0: %stat = "Sharpshooter: " 	@ $Server::PrisonEscape::SharpshooterGuard.name SPC "-" SPC $Server::PrisonEscape::TopAcc SPC "acc";
		case 1: %stat = "Escape Artist: " 	@ $Server::PrisonEscape::MVPPrisoner.name 		SPC "-" SPC $Server::PrisonEscape::TopChisel SPC "bricks";
		case 2: %stat = "Riot Control: "	@ $Server::PrisonEscape::MVPGuard.name			SPC "-" SPC $Server::PrisonEscape::MVPGuard.getScore() SPC "kills";
		case 3: %stat = "Guard Messages Sent: " @ $Server::PrisonEscape::GuardMessagesSent;
		case 4: %stat = "Prisoner Messages Sent: " @ $Server::PrisonEscape::PrisonerMessagesSent;
		case 5: %stat = "Prisoner Deaths: " @ $Server::PrisonEscape::PrisonerDeaths;
		case 6: %stat = "Bricks Destroyed: " @ $Server::PrisonEscape::BricksDestroyed;
		case 7: %stat = "Bullets Fired: " @ $Server::PrisonEscape::SniperRifleBullets;
		case 8: %stat = "Chisel Attacks: " @ $Server::PrisonEscape::ChiselAttacks;
		case 9: %stat = "Trays Used: " @ $Server::PrisonEscape::TraysUsed;
		case 10: %stat = "Buckets Used: " @ $Server::PrisonEscape::BucketsUsed;
		case 11: %stat = "Steaks Eaten: " @ $Server::PrisonEscape::SteaksEaten;
	}

	$statisticString = %stat;
	$Server::PrisonEscape::currentStatistic++;
	$Server::PrisonEscape::currentStatistic %= 12;
	$Server::PrisonEscape::statisticLoop = schedule(5000, 0, swapStatistics);
	displayRoundLoadingInfo();
}

function serverCmdSetPhase(%client, %phase) 
{
	if (!%client.isSuperAdmin)
		return;

	if (%phase == 0) //pre round phase: display statistics, pick guards, load bricks
	{
		//despawn everyone
		setAllCamerasView($PrisonEscape::LoadingCamBrick.getPosition(), $PrisonEscape::LoadingCamBrickTarget.getPosition());
		despawnAll();
		//reload bricks
		serverDirectSaveFileLoad("saves/Prison Escape.bls", 3, "", 0, 1); //1 for silent
		//reset guard picks and after load is complete add new named bricks to tower scriptobjs
		//also add the comms dish and the generator to special global vars
		messageAdmins("\c4Loading bricks...");
		$Server::PrisonEscape::generator = 0;
		$Server::PrisonEscape::commDish = 0;
		//$buildBLID = 4928;
		//assignBricks();
		$Server::PrisonEscape::Guards = "";

		//start statistics display loop
		calculateStatistics();
		swapStatistics();
		displayRoundLoadingInfo();
	} 
	else if (%phase == 1) //start the round caminations and spawn everyone but dont give them control of their bodies yet
	{
		//reset statistics
		clearStatistics();
		clearCenterprintAll();
		clearBottomprintAll();

		//assign guards
		for (%i = 0; %i < 4; %i++)
		{
			%client = getWord($Server::PrisonEscape::Guards, %i);
			assignGuard(%client);
		}
		//spawn guards

		//spawn prisoners ensure no double spawnpoint spawns??

		//play music
		//camera on prison

		//camera for each guard - give guards control of their body here

		//autocall phase 2
	}
	else if (%phase == 2) //start round loops, like timer + win conditions check
	{
		//turn on all the spotlights
		//iterate through brickgroup ntname "spotlights" and manually toggle them on.
		//give players items
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			giveItems(ClientGroup.getObject(%i));
		}
		//give players control of themselves
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%cl = ClientGroup.getObject(%i);
			%cl.setControlObject(%cl.player);
		}
		//start round timer
		bottomprintTimerLoop(20 * 60);
		//create win trigger zones
		$guardCount = 4;
		prisonersWinLoop(0);
	}
	else if (%phase == 3) //end of round phase
	{
		//cancel timer loop, but dont override the ending time bottomprint
		
		//assign camera, but dont remove player control so everyone can climb out and run and stuff
		
		//play round end music
		
		//disable suicide (probably just disable it entirely)
		
		//autostart phase 0 in 15 seconds
		
	}
}