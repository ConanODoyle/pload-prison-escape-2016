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
		%camera.setConrolObject(%client.dummyCamera);

	schedule(0, 0, setCameraViewLoop, %transform, %i+1, %nocontrol);
}