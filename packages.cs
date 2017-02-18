package PrisonEscape_Base
{
	function serverCmdSuicide(%client)
	{
		if ($Server::PrisonEscape::Testing)
			return parent::serverCmdSuicide(%client);
		return;
	}

	function serverCmdDropTool(%client, %slot) 
	{
		if (%client.isGuard && isObject(%player = %client.player) && %player.tool[%slot].getName() !$= "SteakItem") 
		{
			return;
		}
		return parent::serverCmdDropTool(%client, %slot);
	}

	function Observer::onTrigger(%this, %obj, %trig, %state) {
		%client = %obj.getControllingClient();
		if ((%trig == 0 || %trig == 4) && %state == 1 && $Server::PrisonEscape::roundPhase >= 0)
		{
			if (isObject(%pl = %obj.getControllingClient().player) && %pl.isInCamera) {
				return parent::onTrigger(%this, %obj, %trig, %state);
			}
			if ($Server::PrisonEscape::roundPhase < 2)
				return;
			if ($Server::PrisonEscape::roundPhase == 2 && !isObject(%client.player) && %state == 1)
			{
				//interrupt any timed cameras
				//for those who joined the round late since they get camera action or something.
				%client.isViewingIntro = 0;
				//toggle player spectate
				if (%trig == 0)
					spectateNextPlayer(%client, 1);
				else if (%trig == 4)
					spectateNextPlayer(%client, -1);
				return;
			}
			if ($Server::PrisonEscape::roundPhase == 3)
			{
				//TODO
			}
			return;
		}
		
		return parent::onTrigger(%this, %obj, %trig, %state);
	}

	function GameConnection::createPlayer(%this, %pos)
	{
		%parent = parent::createPlayer(%this, %pos);
		%this.centerPrint("");
		if (!%this.isGuard)
		{
			%this.player.setShapeNameColor(".9 .34 .08");
			%this.player.setShapeNameDistance(15);
		}
		else
		{
			%this.player.setShapeNameColor(".54 .7 .55");
			%this.player.setShapeNameDistance(20);
		}

		giveItems(%this);

		if (%this.originalFOV > 0) {
			%this.setControlCameraFOV(%this.originalFOV);
		}

		return %parent;
	}

	function GameConnection::applyBodyParts(%this)
	{
		if (!isObject(%this.player)) {
			return parent::applyBodyParts(%this);
		}
		if (%this.player.getDatablock().getName() $= "BuffArmor") 
		{
			%this.player.unHideNode("ALL");
		}
		else if(%this.applyingUniform || !isObject(%this.minigame))
		{
			return parent::applyBodyParts(%this);
		}
	}

	function GameConnection::applyBodyColors(%this)
	{
		if (isObject(%this.player) && %this.player.getDatablock().getName() $= "BuffArmor") 
		{
			%color = %this.headColor;
			%tint = max(getWord(%this.headColor, 0) - 0.14, 0) SPC max(getWord(%this.headColor, 1) - 0.16, 0) SPC getWords(%this.headColor, 2, 3);

			%this.player.setNodeColor("ALL", %color);
			%this.player.setNodeColor("nipples", %tint);
			%this.player.setNodeColor("face", "0 0 0 1");
			%this.player.setNodeColor("pants", "0.1 0.1 0.1 1");
			%this.player.setNodeColor("lShoe", "0.1 0.1 0.1 1");
			%this.player.setNodeColor("rShoe", "0.1 0.1 0.1 1");
		}
		else if(%this.applyingUniform || !isObject(%this.minigame))
		{
			return parent::applyBodyColors(%this);
		}
		else
		{
			%this.applyUniform();
		}
	}

	function GameConnection::onDeath(%cl, %obj, %killer, %pos, %part) {
		if ($Server::PrisonEscape::roundPhase == 2) {
			//%ret = parent::onDeath(%cl, %obj, %killer, %pos, %part);
			spectateNextPlayer(%cl, 0);
			%cl.setControlObject(%cl.camera);
			%cl.camera.setControlObject(%cl.camera);
			%cl.player = "";
			return;
		} else {
			return parent::onDeath(%cl, %obj, %killer, %pos, %part);
		}
	}

	function respawnCountDownTick(%val) {
		//removes the purple text countdown
		return;
	}

	function handleYourDeath(%netstring, %killMsg, %val1, %val2, %val3, %num) {
		//removes the purple text countdown
		return;
	}

	function SimObject::onCameraEnterOrbit(%obj) {
		//removes the bottomprint counter
		return;
	}

	function SimObject::onCameraLeaveOrbit(%obj) {
		//removes the bottomprint counter
		return;
	}

	function GameConnection::onDrop(%this, %val) {
		if (%this.bl_id !$= "" && strPos($Server::PrisonEscape::Guards, %this.bl_id) >= 0) {
			serverCmdRemoveGuard($fakeClient, %this.name);
		}
		if (%this.isGuard) {
			PPE_messageAdmins("<font:Palatino Linotype:36>!!! \c6Tower \c3" @ %this.tower @ "\c6's guard has just left the game!");
		}
		return parent::onDrop(%this, %val);
	}

	function Player::clearTools(%this, %client) {
		%this.unMountImage(0);
		%this.unMountImage(1);
		%this.unMountImage(2);
		%this.unMountImage(3);
		return parent::clearTools(%this, %client);
	}

	function Armor::onCollision(%this, %obj, %col, %vel, %speed) {
		%name = %col.getDatablock().getName();
		if (%obj.getDatablock().getName() $= "BuffArmor" && %col.getClassName() $= "Item") {
			return;
		} else if ((%name $= "riotSmokeGrenadeItem" || %name $= "yellowKeyItem") && isObject(%col.spawnBrick)) {
			%ret = parent::onCollision(%this, %obj, %col, %vel, %speed);
			if (isEventPending(%col.fadeInSchedule)) {
				%col.spawnBrick.originalItem = %col.getDatablock().getName();
				%col.delete();
				return %ret;
			} else {
				return parent::onCollision(%this, %obj, %col, %vel, %speed);
			}
		}
		return parent::onCollision(%this, %obj, %col, %vel, %speed);
	}

	function minigameCanDamage(%obj1, %obj2) {
		if (%obj1.getClassName() !$= "GameConnection") {
			%cl1 = %obj1.client;
			%db1 = %obj1.getDatablock().getName();
		} else {
			%cl1 = %obj1;
		}

		if (%obj2.getClassName() !$= "GameConnection") {
			%db2 = %obj2.getDatablock().getName();
			%cl2 = %obj2.client;
		} else {
			%cl2 = %obj2;
		}

		if (%db1 $= "ShepherdDogHoleBot" || %db2 $= "ShepherdDogHoleBot") {
			if (%cl1.isGuard || %cl2.isGuard) {
				return 0;
			}
		} else if (%cl1.isGuard == %cl2.isGuard) {
			return 0;
		} else if (%db1 $= "SpotlightArmor" || %db1 $= "SpotlightArmor") {
			return 0;
		}

		return 1;
	}

	function WeaponImage::onMount(%this, %obj, %slot) {
		if (%obj.getDatablock().getName() $= "BuffArmor" && %this.getName() !$= "BuffBashImage") {
			if (isObject(%cl = %obj.client)) {
				%cl.centerPrint("\c6You can't equip items as Bronson!", 1);
				%obj.unMountImage(%slot);
			}
			return;
		} else if (isObject(%mnt = %obj.getObjectMount()) && %mnt.getDatablock().getName() $= "LaundryCartArmor" && %obj.getMountNode() $= 0) {
			if (isObject(%cl = %obj.client)) {
				%cl.centerPrint("\c6You can't equip items as while pushing a cart!", 1);
				%obj.unMountImage(%slot);
			}
			return;
		}
		return parent::onMount(%this, %obj, %slot); 
	}

	function Armor::onDisabled(%this, %obj, %enabled) {
		for (%i = 0; %i < %this.maxTools; %i++) {
			if (isObject(%obj.tool[%i]) && %obj.tool[%i].getName() $= "yellowKeyItem") {
				%i = new Item()
				{
					datablock = yellowKeyItem;
					canPickup = true;
					rotate = false;
					position = %obj.getHackPosition();
					minigame = getMinigameFromObject(%obj);
				};
				MissionCleanup.add(%i);
				%i.setVelocity("0 0 15");
				%i.schedule(60000, fadeOut);
				%i.schedule(61000, delete);
			}
		}
		return parent::onDisabled(%this, %obj, %enabled);
	}

    function ServerCmdLeaveMinigame(%client) {
		if(%client.minigame == $DefaultMini && !%client.isadmin && !%client.issuperadmin) {
		    messageclient(%client, '', "You cannot leave the default minigame unless you are an admin.");
		} else {
		    parent::ServerCmdLeaveMinigame(%client);
		}
    }

    function GameConnection::onClientEnterGame(%client) {
    	%ret = parent::onClientEnterGame(%client);
	    if (isObject($DefaultMini)) {
			$DefaultMini.addmember(%client);

			%phase = $Server::PrisonEscape::roundPhase;
			if (%phase >= 0) {
				if (isObject(%client.player)) { 
					%client.player.delete();
					%client.setControlObject(%client.camera);
					%client.camera.setControlObject(%client.dummycamera);
				}
				if (%phase == 0) {
					%client.setCameraView($Server::PrisonEscape::LoadingCamBrick, $Server::PrisonEscape::LoadingCamBrickTarget);
				} else if (%phase == 1) {
					%client.centerprint("<font:Arial Bold:34><shadowcolor:666666><shadow:0:4><color:E65714>JailBreak! <br><font:Arial Bold:26><color:ffffff>Team up and escape!", 10);
					%client.setCameraView($Server::PrisonEscape::PrisonPreview, $Server::PrisonEscape::PrisonPreviewTarget);
				} else if (%phase >= 2) {
					%client.camera.setControlObject(%client.camera);
					spectateNextPlayer(%cl, 0);
				}
			}
	    }
		return %ret;
    }
};
activatePackage(PrisonEscape_Base);

function ServerCmdSetDefaultMinigame(%client)
{
	if (!%client.isSuperAdmin) {
		return;
	}

    if(isObject(%client.minigame)) {
	    $DefaultMini = %client.minigame;
	    activatepackage(DefaultMini);
	    for(%i = 0; %i < ClientGroup.getCount();%i++) {
			%target = ClientGroup.getObject(%i);

			if(!%target.hasSpawnedOnce)
			    continue;

			if(%target.minigame != $DefaultMini) {
			    if(isObject(%target.minigame)) {
					%target.minigame.removeMember(%target);
			    }
			    $DefaultMini.addMember(%target);
			}
	    }
	    messageall('', "A default minigame has been set by \c2" @ %client.getPlayerName());
    } else {
		messageclient(%client, '', "You are not in a minigame!");
    }
}


function max(%a, %b) {
	if (%a < %b) {
		return %b;
	}
	else 
	{
		return %a;
	}
}

$Skill4LifePink = "0.963 0.341 0.313 1";
$SwollowColor = ".9 .34 .08 1";

function GameConnection::applyUniform(%this)
{
	%player = %this.player;
	if(!isObject(%player))
		return;

	%this.applyingUniform = true;
	
	%color = (%this.isGuard ? ".54 .7 .55 1" : ".9 .34 .08 1");
	
	switch(%this.isGuard)
	{
		case 0: //Prisoner Uniform
			%this.applyBodyColors();
			%this.applyBodyParts();

			%i = -1;
			while((%node = $pack[%i++]) !$= "")
				%player.hideNode(%node);

			%i = -1;
			while((%node = $secondpack[%i++]) !$= "")
				%player.hideNode(%node);

			%i = -1;
			while((%node = $hat[%i++]) !$= "")
				%player.hideNode(%node);

			%i = -1;
			while((%node = $accent[%i++]) !$= "")
				%player.hideNode(%node);

			if (%player.isNodeVisible(skirtHip))
			{
				%player.hideNode(skirtHip);
				%player.hideNode(skirtTrimLeft);
				%player.hideNode(skirtTrimRight);
				%player.unHideNode(pants);
			}
			if (%this.bl_id == 4382) {
				%color = $Skill4LifePink;
				%this.player.unHideNode($secondpack[2]);
				%this.player.setNodeColor($secondpack[2], "1 1 0 1");
			} else if (%this.bl_id == 6531) {
				%color = $SwollowColor;
				%this.player.unHideNode($pack[3]);
				%this.player.setNodeColor($pack[3], "0.388 0 0.117 1");
				%this.player.unHideNode($secondpack[6]);
				%this.player.setNodeColor($secondpack[6], "0.388 0 0.117 1");
			} else if (%this.bl_id == 1768) {
				%this.player.unHideNode($secondpack[6]);
				%this.player.setNodeColor($secondpack[6], "0.2 0.2 0.2 1");
			}
			%player.hideNode(lPeg);
			%player.hideNode(rPeg);
			%player.setNodeColor(chest, %color);
			%player.setNodeColor(femChest, %color);
			%player.setNodeColor(rArm, %color);
			%player.setNodeColor(rArmSlim, %color);
			%player.setNodeColor(lArm, %color);
			%player.setNodeColor(lArmSlim, %color);
			%player.setNodeColor(pants, %color);
			%player.unHideNode(lShoe);
			%player.unHideNode(rShoe);
			%player.setNodeColor(lShoe, "0.1 0.1 0.1 1");
			%player.setNodeColor(rShoe, "0.1 0.1 0.1 1");
			%player.setNodeColor(lPeg, "0.1 0.1 0.1 1");
			%player.setNodeColor(rPeg, "0.1 0.1 0.1 1");

			%player.setDecalName("Mod-Prisoner");

		case 1: //Guard Uniform
			%this.applyBodyColors();
			%this.applyBodyParts();

			%i = -1;
			while((%node = $pack[%i++]) !$= "")
				if (%i != 4)
					%player.hideNode(%node);

			%i = -1;
			while((%node = $secondpack[%i++]) !$= "")
				%player.hideNode(%node);
				
			%i = -1;
			while((%node = $hat[%i++]) !$= "")
				%player.hideNode(%node);

			%i = -1;
			while((%node = $accent[%i++]) !$= "")
				%player.hideNode(%node);

			if (%player.isNodeVisible(skirtHip))
			{
				%player.hideNode(skirtHip);
				%player.hideNode(skirtTrimLeft);
				%player.hideNode(skirtTrimRight);
				%player.unHideNode(pants);
				%player.unHideNode(lShoe);
				%player.unHideNode(rShoe);
			}
			%player.unHideNode(copHat);
			%player.setNodeColor(copHat, %color);
			%player.setNodeColor(chest, %color);
			%player.setNodeColor(femChest, %color);
			%player.setNodeColor(rArm, %color);
			%player.setNodeColor(rArmSlim, %color);
			%player.setNodeColor(lArm, %color);
			%player.setNodeColor(lArmSlim, %color);
			%player.setNodeColor(pants, "0.1 0.1 0.1 1");
			%player.setNodeColor(lShoe, "0.1 0.1 0.1 1");
			%player.setNodeColor(rShoe, "0.1 0.1 0.1 1");
			%player.setNodeColor(lPeg, "0.1 0.1 0.1 1");
			%player.setNodeColor(rPeg, "0.1 0.1 0.1 1");

			%player.setDecalName("Mod-Police");
	}
	%this.applyingUniform = false;
}

function giveItems(%client) 
{
	if (!isObject(%player = %client.player)) 
	{
		return;
	}

	if (%client.isGuard)
	{
		%player.addItem(SniperRifleSpotlightItem, %client);
		//%player.addItem(WhistleItem, %client);
		%player.addItem(SteakItem, %client);
	}
	else
	{
		%player.addItem(ChiselItem, %client);
	}
}

function Player::addItem(%this, %item, %client)
{
	%item = %item.getID();
	for(%i = 0; %i < %this.getDatablock().maxTools; %i++)
	{
		%tool = %this.tool[%i];
		if(%tool == 0)
		{
			%this.tool[%i] = %item.getID();
			%this.weaponCount++;
			messageClient(%client, 'MsgItemPickup', '', %i, %item.getID());
			break;
		}
	}
}

registerOutputEvent("Player", "addItem", "datablock ItemData", 1);