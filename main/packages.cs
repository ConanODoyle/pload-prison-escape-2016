package PrisonEscape_Base
{
	function serverCmdSuicide(%client)
	{
		if ($Server::PrisonEscape::Testing)
			return parent::serverCmdSuicide(%client);
		return;
	}

	function Observer::onTrigger(%this, %obj, %trig, %state) {
		%client = %obj.getControllingClient();
		
		if (%trig == 0)
		{
			if ($Server::PrisonEscape::roundPhase < 2)
				return;
			if ($Server::PrisonEscape::roundPhase == 2 && !isObject(%client.player) && %state == 1)
			{
				//interrupt any timed cameras
				%client.isViewingIntro = 0;
				//toggle player spectate
				spectateNextPlayer(%client);
			}
			if ($Server::PrisonEscape::roundPhase == 3)
			{

			}
		}
		
		Parent::onTrigger(%this, %obj, %trig, %state);
	}


	function GameConnection::createPlayer(%this, %pos)
	{
		%parent = parent::createPlayer(%this, %pos);
		if (!%this.isGuard)
		{
			%this.player.setShapeNameColor(".9 .34 .08");
			%this.player.setShapeNameDistance(15);

			giveItems(%this);

			return %parent;
		}
		else
		{
			%this.player.setShapeNameColor(".54 .7 .55");
			%this.player.setShapeNameDistance(20);

			giveItems(%this);

			return %parent;
		}
	}


	function GameConnection::applyBodyParts(%this)
	{
		if(%this.applyingUniform || !isObject(%this.minigame))
		{
			return parent::applyBodyParts(%this);
		}
	}
	
	//@private
	function GameConnection::applyBodyColors(%this)
	{
		if(!%this.applyingUniform && isObject(%this.minigame))
		{
			%this.applyUniform();
		}
		else
		{
			return parent::applyBodyColors(%this);
		}
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
			%player.setNodeColor(chest, %color);
			%player.setNodeColor(femChest, %color);
			%player.setNodeColor(rArm, %color);
			%player.setNodeColor(rArmSlim, %color);
			%player.setNodeColor(lArm, %color);
			%player.setNodeColor(lArmSlim, %color);
			%player.setNodeColor(pants, %color);
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
	%start = getRandom(0, $Server::PrisonEscape::PrisonerSpawnPoints.count - 1);
	for (%i = %start; %i < $Server::PrisonEscape::PrisonerSpawnPoints.count; %i++)
	{
		%index = %i % $Server::PrisonEscape::PrisonerSpawnPoints.count;
		%brick = $Server::PrisonEscape::PrisonerSpawnPoints.spawn[%index];
		if (%brick.spawnCount < 2)
			break;
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
		return $Server::PrisonEscape::PrisonerSpawnPoints.spawn[%start].getSpawnPoint();
	}
}

function resetPrisonerSpawnPointCounts()
{
	for (%i = 0; %i < $Server::PrisonEscape::PrisonerSpawnPoints.count; %i++)
	{
		%brick = $Server::PrisonEscape::PrisonerSpawnPoints.spawn[%i];
		%brick.spawnCount = 0;
	}
}

function giveItems(%client) 
{
	if (!isObject(%player = %client.player))
		return;
	if (%client.isGuard)
	{
		%player.addItem(SniperRifleSpotlightItem, %client);
		//%player.addItem(WhistleItem, %client);
		%player.addItem(SteakItem, %client);
		//%player.addItem(BatonItem, %client);
	}
	else
	{
		%player.addItem(ChiselItem, %client);
	}
}

function Player::addItem(%player,%image,%client)
{
   for(%i = 0; %i < %player.getDatablock().maxTools; %i++)
   {
      %tool = %player.tool[%i];
      if(%tool == 0)
      {
         %player.tool[%i] = %image;
         %player.weaponCount++;
         messageClient(%client,'MsgItemPickup','',%i,%image);
         break;
      }
   }
}