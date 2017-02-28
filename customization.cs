function startPlayerHairCustomization(%pl, %brick) {
	if (!isObject(%cl = %pl.client)) {
		return;
	}

	%mount = new AIPlayer(Customization) {
		datablock = EmptyHoleBot;
	};
	MissionCleanup.add(%mount);
	%mount.setTransform(%brick.getTransform());
	%pl.customizationMount = %mount;
	%pl.setTransform("0 0 0");

	%mount.mountObject(%pl, 0);
	%pl.isCustomizing = 1;
	%cl.isCustomizing = 1;
	%pl.canDismount = 0;

	%cl.camera.setMode("Observer");
	%cl.camera.setTransform(getHairCamPosition(%brick, %pl));
	%cl.setControlObject(%cl.camera);
	%cl.camera.setControlObject(%pl);
	%cl.customizingMode = "Hair";

	%index = $PrisonEscape::Hair::currentHair[%cl.bl_id] = $PrisonEscape::Hair::savedHair[%cl.bl_id];
	centerPrint(%cl, "\c6Plant Brick: Lock in choice" @ getFormattedHairCenterprint(%cl, $PrisonEscape::Hair::Unlocked[%cl.bl_id], %index));

	if ($PrisonEscape::Hair::Unlocked[%cl.bl_id] $= "") {
		$PrisonEscape::Hair::Unlocked[%cl.bl_id] = "0";
	}
	%pl.playThread(0, sit);
	serverCmdUnUseTool(%cl);
}

function getHairCamPosition(%brick, %pl) {
	%winString = "<font:Palatino Linotype:24>\c6Testing cams ";
	%id = %brick.getAngleID();
	switch (%id) {
		case 0: %vec = "0 2 2";
		case 1: %vec = "-2 0 2";
		case 2: %vec = "0 -2 2";
		case 3: %vec = "2 0 2";
	}
	%start = %brick.getPosition();
	%end = vectorAdd(%vec, %start);
	
	%pos = vectorAdd(%brick.getPosition(), %vec);
	%delta = vectorSub(getWords(%pl.getEyeTransform(), 0, 2), %pos);
	%deltaX = getWord(%delta, 0);
	%deltaY = getWord(%delta, 1);
	%deltaZ = getWord(%delta, 2);
	%deltaXYHyp = vectorLen(%deltaX SPC %deltaY SPC 0);

	%rotZ = mAtan(%deltaX, %deltaY) * -1; 
	%rotX = mAtan(%deltaZ, %deltaXYHyp);

	%aa = eulerRadToMatrix(%rotX SPC 0 SPC %rotZ); //this function should be called eulerToAngleAxis...
	return %pos SPC %aa;
}


function stopPlayerHairCustomization(%pl) {
	if (!isObject(%cl = %pl.client) || !%cl.isCustomizing) {
		return;
	}

	%pl.canDismount = 1;
	%pl.dismount();
	%pl.setTransform(%pl.customizationMount.getTransform());
	%pl.setTransform(%pl.customizationMount.getPosition() SPC rotFromTransform(%pl.getTransform()));

	%pl.customizationMount.delete();
	%pl.isCustomizing = 0;
	%cl.isCustomizing = 0;

	%cl.camera.setMode("Observer");
	%cl.setControlObject(%pl);
	%cl.customizingMode = "";

	putOnHair(%pl, $PrisonEscape::Hair::savedHair[%cl.bl_id]);

	centerPrint(%cl, "");
	%pl.playThread(0, root);
}



package PrisonCustomization {
	function Observer::onTrigger(%this, %obj, %trig, %state) {
		%cl = %obj.getControllingClient();
		if (%cl.isCustomizing) {
			eval("toggle" @ %cl.customizingMode @ "Mode(" @ %cl @ ", " @ %trig @ ", " @ %state @ ");");
			return;
		}
		return parent::onTrigger(%this, %obj, %trig, %state);
	}

	function serverCmdHat(%cl, %na, %nb, %nc, %nd, %ne) {
		messageClient(%cl, '', "You need to go to the barber to change your hair!");
		//messageClient(%cl, '', "You need to go to the janitor to change your bucket!");
		return;
	}

	function serverCmdHair(%cl) {
		messageClient(%cl, '', "You need to go to the barber to change your hair!");
		return;
	}

	function serverCmdPlantBrick(%cl) {
		if (%cl.isCustomizing) {
			%currentHair = $PrisonEscape::Hair::currentHair[%cl.bl_id];
			%cl.centerPrint("\c2 Hairdo saved!" @ getFormattedHairCenterprint(%cl, $PrisonEscape::Hair::Unlocked[%cl.bl_id], %currentHair));
			$PrisonEscape::Hair::savedHair[%cl.bl_id] = $PrisonEscape::Hair::currentHair[%cl.bl_id];
			return;
		}
		return parent::serverCmdPlantBrick(%cl);
	}
};
activatePackage(PrisonCustomization);

/////////////
/// hairs ///
/////////////

$HairCount = 15;
$Hair[0] = "None";
$Hair[1] = "CombOverHair";
$Hair[2] = "CornRowsHair";
$Hair[3] = "SuavHair";
$Hair[4] = "MohawkHair";
$Hair[5] = "NeatHair";
$Hair[6] = "JudgeHair";
$Hair[7] = "Wig";
$Hair[8] = "BobCutHair";
$Hair[9] = "GoateeHair";
$Hair[10] = "OldWomanHair";
$Hair[11] = "EmoHair";
$Hair[12] = "BroadHair";
$Hair[13] = "PunkHair";
$Hair[14] = "AfroHair";

function toggleHairMode(%cl, %trig, %state) {
	%hairUnlocked = $PrisonEscape::Hair::Unlocked[%cl.bl_id];
	%hairUnlockedCount = getWordCount($PrisonEscape::Hair::Unlocked[%cl.bl_id]);

	if (%state == 1) {
		%pl = %cl.player;
		if (%trig == 0) {
			$PrisonEscape::Hair::currentHair[%cl.bl_id] += %hairUnlockedCount - 1;
			$PrisonEscape::Hair::currentHair[%cl.bl_id] %= %hairUnlockedCount;
		} else if (%trig == 4) {
			$PrisonEscape::Hair::currentHair[%cl.bl_id] += 1;
			$PrisonEscape::Hair::currentHair[%cl.bl_id] %= %hairUnlockedCount;
		}

		%currentHair = $PrisonEscape::Hair::currentHair[%cl.bl_id];

		if (%currentHair >= %hairUnlockedCount) {
			PPE_MessageAdmins("!!! - \c3" @ %cl.name @ "\c6's hair index is invalid!");
			return;

		}
		%print = "\c6Plant Brick: Lock in choice" @ getFormattedHairCenterprint(%cl, $PrisonEscape::Hair::Unlocked[%cl.bl_id], %currentHair);
		centerPrint(%cl, %print);
		putOnHair(%pl, $Hair[getWord(%hairUnlocked, %currentHair)]);
	}
}

function getFormattedHairCenterprint(%cl, %list, %index) {

	%hairs = (%index + 1) @ " / " @ getWordCount(%list);
	%currentHair = strReplace($Hair[getWord(%list, %index)], "Hair", "");

	%final = "<br><font:Arial Bold:24>\c3Left Click       <font:Palatino Linotype:24>\c3[\c6" @ %hairs @ "\c3]<font:Arial Bold:24>\c3       Right Click <br><font:Palatino Linotype:24><just:center>\c6" @ %currentHair @ " ";
	return %final;
}

function putOnHair(%pl, %hair) {
	if ((%hair $= "" || %hair $= "None") && %pl.getMountedImage(2) != 0) {
		%pl.unMountImage(2);
	} else {
		%pl.mountImage("Hat" @ %hair @ "Data", 2);
	}
}