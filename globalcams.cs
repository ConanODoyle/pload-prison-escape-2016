function setAllCamerasView(%camPos, %targetPos, %nocontrol)
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
	setCameraViewLoop(%camTransform, 0, !%nocontrol);
}

function setCameraViewLoop(%transform, %i, %nocontrol)
{
	if (%i >= ClientGroup.getCount())
		return;
	%client = ClientGroup.getObject(%i);
	%camera = %client.camera;
	
	%client.setControlObject(%camera);
	%camera.setTransform(%transform);

	%camera.setFlyMode();
	%camera.mode = "Observer";
	if (%nocontrol)
		%camera.setControlObject(%client.dummyCamera);
		////////////////////////////////package onTrigger or respawnplayer to prevent players from spawning
	schedule(0, 0, setCameraViewLoop, %transform, %i+1, %nocontrol);
}

function allCameraPan(%pos1, %pos2, %speed, %targetPos)
{
	if (vectorDist(%pos1, %pos2) < 0.01)
	{
		talk(vectorDist(%pos1, %pos2));
		return;
	}
	//calculate normal vector from pos 1 to pos 2, then vectorscale by speed/(1000/33)
	%moveVector = vectorNormalize(vectorSub(%pos2, %pos1));
	%moveVector = vectorScale(%moveVector, %speed*20/10000);

	setAllCamerasView(%pos1, %targetPos);
	if (isEventPending($allCameraPan))
		cancel($allCameraPan);
	$allCameraPan = schedule(0, 0, allCameraPan, vectorAdd(%pos1, %moveVector), %pos2, %speed/1.00005, %targetPos);
}

function returnAllPlayerControl()
{
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		if (isObject(%client.player))
			%client.setControlObject(%client.player);
	}
}

//////Spectating cams//////

function spectateNextPlayer(%client, %num)
{
	%dir = (%num > 0 ? 1 : -1);
	%client.spectatingClient = (%client.spectatingClient + %num) % ClientGroup.getCount();
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		if (isObject(%player = ClientGroup.getObject(%client.spectatingClient).player)) 
		{
			break;
		}
		%client.spectatingClient = (%client.spectatingClient + %dir) % ClientGroup.getCount();
	}
	//centerprint controls + time till respawn
	%client.centerprint("<font:Arial:26><just:left>\c6Left Click<just:right>\c6Right Click <br><just:left><font:Arial Bold:26>\c3Next Player<just:right>\c3Prev Player ");

	if (!isObject(%player))
		return;
	%client.camera.setMode(Corpse, %player);
	%client.setControlObject(%client.camera);
}