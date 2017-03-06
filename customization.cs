exec("./hair.cs");

function serverCmdBarber(%cl) {
	if (!isObject(%pl = %cl.player)) {
		return;
	} else if ($Server::PrisonEscape::roundPhase > 0 && getRegion(%pl) !$= "Infirmary") {
		return;
	} else if (vectorLen(%pl.getVelocity()) > 0.1) {
		messageClient(%cl, '', "You need to stop moving to use the barber!");
		return;
	}
	%pl.setVelocity("0 0 0");
	schedule(1, %pl, startPlayerHairCustomization, %pl);
}

function startPlayerHairCustomization(%pl, %brick) {
	if (!isObject(%cl = %pl.client)) {
		return;
	}

	%mount = new AIPlayer(Customization) {
		datablock = EmptyHoleBot;
	};
	MissionCleanup.add(%mount);
	if (isObject(%brick)) {
		%mount.setTransform(%brick.getTransform());
	} else {
		%mount.setTransform(%pl.getTransform());
	}
	%pl.customizationMount = %mount;
	%pl.setTransform("0 0 0");

	%mount.mountObject(%pl, 0);
	%pl.isCustomizing = 1;
	%cl.isCustomizing = 1;
	%cl.lastCustomizeTime = getSimTime();
	%pl.canDismount = 0;
	if (isObject(%brick)) {
		%brick.customizingClient = %cl;
	}

	%cl.camera.setMode("Observer");
	%cl.camera.setTransform(getHairCamPosition(%brick, %pl));
	%cl.setControlObject(%cl.camera);
	%cl.camera.setControlObject(%pl);
	%cl.customizingMode = "Hair";

	putOnHair(%pl, getHairName(%cl, $PrisonEscape::Hair::savedHair[%cl.bl_id]));

	if ($PrisonEscape::Hair::Unlocked[%cl.bl_id] $= "") {
		$PrisonEscape::Hair::Unlocked[%cl.bl_id] = "0";
		messageClient(%cl, '', "\c2You got two free first hairdos!");
		giveRandomHair(%cl);
		giveRandomHair(%cl);
	}

	%index = $PrisonEscape::Hair::currentHair[%cl.bl_id] = $PrisonEscape::Hair::savedHair[%cl.bl_id];
	centerPrint(%cl, "<br><br><br><br><br>\c6Plant Brick: Confirm \c7||\c6 Light: Exit" @ getFormattedHairCenterprint(%cl, $PrisonEscape::Hair::Unlocked[%cl.bl_id], %index));

	%pl.playThread(0, sit);
	serverCmdUnUseTool(%cl);
}

function getHairCamPosition(%obj, %pl) {
	if (%obj.getClassName() $= "fxDTSBrick") {
		%id = %obj.getAngleID();
		switch (%id) {
			case 0: %vec = "0 2 2.2";
			case 1: %vec = "-2 0 2.2";
			case 2: %vec = "0 -2 2.2";
			case 3: %vec = "2 0 2.2";
		}
		%start = %obj.getPosition();
		%end = vectorAdd(%vec, %start);
		
		%pos = vectorAdd(%obj.getPosition(), %vec);
	} else if (!isObject(%obj)) {
		%pos = vectorAdd(%pl.getEyeTransform(), vectorAdd(%pl.getForwardVector(), "0 0 0.5"));
	}
	%delta = vectorSub(getWords(vectorAdd(%pl.getEyeTransform(), "0 0 -0.4"), 0, 2), %pos);
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

	putOnHair(%pl, getHairName(%cl, $PrisonEscape::Hair::savedHair[%cl.bl_id]));

	if (isObject(%pl.customizingBrick)) {
		%pl.customizingBrick.customizingClient = "";
	}

	centerPrint(%cl, "");
	%pl.playThread(0, root);

	export("$PrisonEscape::Hair*", "Add-ons/Gamemode_PPE/hair.cs");
}

registerOutputEvent("fxDTSBrick", "startHairCustomization", "", 1);

function fxDTSBrick::startHairCustomization(%this, %cl) {
	if (!isObject(%pl = %cl.player)) {
		return;
	} else if (isObject(%this.customizingClient)) {
		messageClient(%cl, '', "This chair is in use!");
		return;
	}

	startPlayerHairCustomization(%pl, %this);
	%pl.customizingBrick = %this;
}

function getHairName(%cl, %id) {
	return $Hair[getWord($PrisonEscape::Hair::Unlocked[%cl.bl_id], %id)];
}

package PrisonCustomization {
	function Observer::onTrigger(%this, %obj, %trig, %state) {
		%cl = %obj.getControllingClient();
		if (%cl.isCustomizing && getSimTime() - %cl.lastCustomizeTime > 100) {
			eval("toggle" @ %cl.customizingMode @ "Mode(" @ %cl @ ", " @ %trig @ ", " @ %state @ ");");
			return;
		}
		return parent::onTrigger(%this, %obj, %trig, %state);
	}

	function serverCmdHat(%cl, %na, %nb, %nc, %nd, %ne) {
		if (!%cl.hatTest) {
			messageClient(%cl, '', "You need to go to the barber to change your hair!");
			return;
		}
		//messageClient(%cl, '', "You need to go to the janitor to change your bucket!");
		return parent::serverCmdHat(%cl, %na, %nb, %nc, %nd, %ne);
	}

	function serverCmdHair(%cl) {
		messageClient(%cl, '', "You need to go to the barber to change your hair!");
		return;
	}

	function serverCmdPlantBrick(%cl) {
		if (%cl.isCustomizing) {
			%currentHair = $PrisonEscape::Hair::currentHair[%cl.bl_id];
			%cl.centerPrint("<br><br><br><br><br>\c2 Hairdo saved!" @ getFormattedHairCenterprint(%cl, $PrisonEscape::Hair::Unlocked[%cl.bl_id], %currentHair));
			$PrisonEscape::Hair::savedHair[%cl.bl_id] = $PrisonEscape::Hair::currentHair[%cl.bl_id];
			return;
		}
		return parent::serverCmdPlantBrick(%cl);
	}

	function Armor::onTrigger(%this, %obj, %trig, %state) {
		if (%obj.isCustomizing) {
			return;
		}

		return parent::onTrigger(%this, %obj, %trig, %state);
	}

	function serverCmdLight(%cl) {
		if (%cl.isCustomizing && isObject(%cl.player)) {
			stopPlayerHairCustomization(%cl.player);
			return;
		}
		return parent::serverCmdLight(%cl);
	}

	function GameConnection::onDrop(%cl) {
		if (%cl.isCustomizing) {
			stopPlayerHairCustomization(%cl.player);
		}
		return parent::onDrop(%cl);
	}

	function GameConnection::onDeath(%cl, %obj, %killer, %pos, %part) {
		if (%cl.isCustomizing) {
			stopPlayerHairCustomization(%cl.player);
		}
		return parent::onDeath(%cl, %obj, %killer, %pos, %part);
	}

	function Armor::onDisabled(%this, %obj, %enabled) {
		if (%obj.isCustomizing) {
			stopPlayerHairCustomization(%obj);
		}
		return parent::onDisabled(%this, %obj, %enabled);
	}
};
activatePackage(PrisonCustomization);


function serverCmdoffsetLocations(%cl, %x, %y, %z) {
	if (!%cl.isSuperAdmin) {
		return;
	}

	for (%i = 0; %i < $locationNum; %i++) {
		$location[%i @ "::pos0"] = vectorAdd($location[%i @ "::pos0"], %x SPC %y SPC %z);
		$location[%i @ "::pos1"] = vectorAdd($location[%i @ "::pos1"], %x SPC %y SPC %z);
	}
	export("$location*", "Add-ons/gamemode_ppe/locations.cs");
}

/////////////
/// hairs ///
/////////////

$HairCount = 31;
$Hair[0] = "None";
$Hair[1] = "Comb-OverHair";
$Hair[2] = "Corn RowsHair";
$Hair[3] = "SuavHair";
$Hair[4] = "MohawkHair";
$Hair[5] = "NeatHair";
$Hair[6] = "JudgeHair";
$Hair[7] = "Wig";
$Hair[8] = "BobCutHair";
$Hair[9] = "GoateeHair";
$Hair[10] = "Old WomanHair";
$Hair[11] = "EmoHair";
$Hair[12] = "BroadHair";
$Hair[13] = "PunkHair";
$Hair[14] = "AfroHair";
$Hair[15] = "PhoenixHair";
$Hair[16] = "DaphneHair";
$Hair[17] = "Flat-TopHair";
$Hair[18] = "MomHair";
$Hair[19] = "ThinningHair";
$Hair[20] = "BaldingHair";
$Hair[21] = "PompadourHair";
$Hair[22] = "FamiliarHair";
$Hair[23] = "MulletHair";
$Hair[24] = "SlickHair";
$Hair[25] = "Turban";
$Hair[26] = "PonytailHair";
$Hair[27] = "BowlHair";
$Hair[28] = "BunnHair";
$Hair[29] = "ShaggyHair";
$Hair[30] = "SaiyanHair";

function serverCmdGrantHair(%cl, %target, %hair) {
	if (!%cl.isAdmin) {
		return;
	}
	if (stripchars(%hair, "1234567890") !$= "") {
		messageClient(%cl, '', "!!! Please put in a valid hair ID");
		return;
	} else if (%hair >= $HairCount || %hair <= 0) {
		messageClient(%cl, '', "!!! Please put in a valid hair ID");
		return;
	}

	%t = fcn(%target);

	if (!isObject(%t)) {
		messageClient(%cl, '', "!!! Cannot find client " @ %target @ "!");
		return;
	}

	if ($PrisonEscape::Hair::Unlocked[%t.bl_id] $= "") {
		$PrisonEscape::Hair::Unlocked[%t.bl_id] = "0";
		messageClient(%t, '', "\c2You got two free first hairdos!");
		giveRandomHair(%t);
		giveRandomHair(%t);
	}
	

	if (hasHair(%t, %hair)) {
		messageClient(%cl, '', "!!! \c3" @ %t.name @ "\c6 already has the " @ strReplace($Hair[%hair], "Hair", "") @ " hair!");
		return;
	}

	$PrisonEscape::Hair::Unlocked[%t.bl_id] = trim($PrisonEscape::Hair::Unlocked[%t.bl_id] SPC %hair);

	$PrisonEscape::Hair::Unlocked[%t.bl_id] = strReplace($PrisonEscape::Hair::Unlocked[%t.bl_id], "  ", " ");

	messageClient(%cl, '', "\c6You have given \c3" @ %t.name @ "\c6 the \c3" @ strReplace($Hair[%hair], "Hair", "") @ "\c6 hair");
	if (%cl.name !$= "Dummy Client") {
		messageClient(%t, '', "\c6You have been given the \c3" @ strReplace($Hair[%hair], "Hair", "") @ "\c6 hair by \c3" @ %cl.name);
	}

	export("$PrisonEscape::Hair*", "Add-ons/Gamemode_PPE/hair.cs");
}

function serverCmdRevokeHair(%cl, %target, %hair) {
	if (!%cl.isAdmin) {
		return;
	}
	if (stripchars(%hair, "1234567890") !$= "") {
		messageClient(%cl, '', "!!! Please put in a valid hair ID");
		return;
	} else if (%hair >= $HairCount || %hair <= 0) {
		messageClient(%cl, '', "!!! Please put in a valid hair ID");
		return;
	}

	%t = fcn(%target);

	if (!isObject(%t)) {
		messageClient(%cl, '', "!!! Cannot find client " @ %target @ "!");
		return;
	}

	if ($PrisonEscape::Hair::Unlocked[%t.bl_id] $= "") {
		$PrisonEscape::Hair::Unlocked[%t.bl_id] = "0";
		messageClient(%t, '', "\c2You got two free first hairdos!");
		giveRandomHair(%t);
		giveRandomHair(%t);
	}

	if (hasHair(%t, %hair)) {
		$PrisonEscape::Hair::Unlocked[%t.bl_id] = removeWord($PrisonEscape::Hair::Unlocked[%t.bl_id], %i);
		$PrisonEscape::Hair::Unlocked[%t.bl_id] = trim(strReplace($PrisonEscape::Hair::Unlocked[%t.bl_id], "  ", " "));

		messageClient(%cl, '', "\c6You have removed \c3" @ %t.name @ "\c6 the \c3" @ strReplace($Hair[%hair], "Hair", "") @ "\c6 hair");
		messageClient(%t, '', "\c6You have had the \c3" @ strReplace($Hair[%hair], "Hair", "") @ "\c6 hair removed by \c3" @ %cl.name);

		$PrisonEscape::Hair::savedHair[%t.bl_id] = 0;
		putOnHair(%pl, getHairName(%t, $PrisonEscape::Hair::savedHair[%t.bl_id]));

		export("$PrisonEscape::Hair*", "Add-ons/Gamemode_PPE/hair.cs");
		return;
	}

	messageClient(%cl, '', "!!! \c3" @ %t.name @ "\c6 does not have the " @ strReplace($Hair[%hair], "Hair", "") @ " hair!");
}

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
		%print = "<br><br><br><br><br>\c6Plant Brick: Confirm \c7||\c6 Light: Exit" @ getFormattedHairCenterprint(%cl, $PrisonEscape::Hair::Unlocked[%cl.bl_id], %currentHair);
		centerPrint(%cl, %print);
		putOnHair(%pl, getHairName(%cl, %currentHair));
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
		%pl.mountImage("Hat" @ stripchars(%hair, " -1234567890_+=.,;':?!@#$%&*()[]{}\"/<>") @ "Data", 2);

		%i = -1;
		while((%node = $hat[%i++]) !$= "")
			%pl.hideNode(%node);

		%i = -1;
		while((%node = $accent[%i++]) !$= "")
			%pl.hideNode(%node);
	}
}

function giveRandomHair(%cl) {
	if (!isObject(%cl)) {
		PPE_MessageAdmins("!!! - Cannot give random hair - client does not exist!");
		return;
	}
	%hair = getRandom(0, $HairCount-1);
	%loopCount = 0;

	while (hasHair(%cl, %hair) && %loopCount < $HairCount) {
		%hair++;
		%hair %= $HairCount;
		%loopCount++;
	}

	if (%loopCount == $HairCount) {
		messageClient(%cl, '', "\c6You won a hairdo last round, but you already have all the hairs!");
	} else {
		messageClient(%cl, '', "\c6You have been given the \c3" @ strReplace($Hair[%hair], "Hair", "") @ "\c6 hair!");
		serverCmdGrantHair($fakeClient, %cl.name, %hair);
	}
}

function hasHair(%cl, %hair) {
	for (%i = 0; %i < getWordCount($PrisonEscape::Hair::Unlocked[%cl.bl_id]); %i++) {
		%currHair = getWord($PrisonEscape::Hair::Unlocked[%cl.bl_id], %i);
		if (%hair == %currHair) {
			return 1;
		}
	}
	return 0;
}