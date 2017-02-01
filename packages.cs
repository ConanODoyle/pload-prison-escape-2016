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

		return %parent;
	}

	function GameConnection::applyBodyParts(%this)
	{
		if(%this.applyingUniform || !isObject(%this.minigame))
		{
			return parent::applyBodyParts(%this);
		}
	}

	function GameConnection::applyBodyColors(%this)
	{
		if(%this.applyingUniform || !isObject(%this.minigame))
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
};
activatePackage(PrisonEscape_Base);

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

function pickPrisonerSpawnPoint() 
{
	%start = getRandom(0, $Server::PrisonEscape::PrisonerSpawnPoints.getCount() - 1);
	for (%i = %start; %i < $Server::PrisonEscape::PrisonerSpawnPoints.getCount(); %i++)
	{
		%index = %i % $Server::PrisonEscape::PrisonerSpawnPoints.getCount();
		%brick = $Server::PrisonEscape::PrisonerSpawnPoints.getObject(%index);
		if (%brick.spawnCount < 2) 
		{
			break;
		}
		%brick = "";
	}
	if (isObject(%brick))
	{
		%brick.spawnCount++;
		return %brick.getSpawnPoint();
	}
	else
	{
		echo("Can't find a spawnpoint with less than 2 spawns! Resetting...");
		resetPrisonerSpawnPointCounts();
		return $Server::PrisonEscape::PrisonerSpawnPoints.getObject(%start).getSpawnPoint();
	}
}

function resetPrisonerSpawnPointCounts()
{
	for (%i = 0; %i < $Server::PrisonEscape::PrisonerSpawnPoints.getCount(); %i++)
	{
		$Server::PrisonEscape::PrisonerSpawnPoints.getObject(%i).spawnCount = 0;
	}
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