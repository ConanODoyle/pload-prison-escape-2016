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
		if (%client.isGuard && isObject(%player = %client.player) && %player.tool[%slot] !$= "SteakItem") 
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

	function minigameCanDamage(%obj1, %obj2) {
		%cl1 = %obj1.client;
		%cl2 = %obj2.client;

		%db1 = %obj1.getDatablock().getName();
		%db2 = %obj2.getDatablock().getName();

		if (%db1 $= "ShepherdDogArmor" || %db2 $= "ShepherdDogArmor") {
			if (%cl1.isGuard || %cl2.isGuard) {
				return 0;
			}
		} else if (%cl1.isGuard == %cl2.isGuard) {
			return 0;
		}

		return 1;
	}
};
activatePackage(PrisonEscape_Base);

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