//assumes the brickgroup is of blid $buildBLID

$Server::PrisonEscape::Towers = new ScriptGroup()
{};
$Server::PrisonEscape::PrisonerSpawnPoints = new ScriptObject()
{
	count = 0;
	//spawn0 = "";
};

function assignBricks()
{
	%brickgroup = "BrickGroup_" @ $buildBLID; //probably public, but set up this way to test with nonpublic bricks
	%towerGroup = $Server::PrisonEscape::Towers;

	for (%i = 1; %i < 5; %i++)
		if (!isObject(%towerGroup.tower[%i]))
			%towerGroup.tower[%i] = new ScriptObject() {class = "TowerGroup"; brickCount = 0;};
		else
		{
			%towerGroup.tower[%i].delete();
			%towerGroup.tower[%i] = new ScriptObject() {class = "TowerGroup"; brickCount = 0;};
		}
	//iterate through and save the bricks into the scriptobjects
	$Server::PrisonEscape::PrisonerSpawnPoints = new ScriptObject()
	{
		count = 0;
		//spawn0 = "";
	};
	saveBricks(%brickgroup, 0);
}

function assignGuard(%client)
{
	//randomly pick a tower to assign the client to
	%towernum = getRandom(1, 4);

	//if its filled, look for an empty spot
	if (isObject($Server::PrisonEscape::Towers.tower[%towernum].guard))
		for (%i = 0; %i < 4; %i++) //only do this four times; if all are filled it'll just take the place of the first one selected.
		{
			%towernum = (%towernum % 4) + 1;
			if (!isObject($Server::PrisonEscape::Towers.tower[%towernum].guard))
				break;
		}
	echo(%client.name @ " assigned to tower " @ %towernum);
	%client.isGuard = 1;

	$Server::PrisonEscape::Towers.tower[%towernum].guard = %client;
}

function replaceGuard(%client, %tower)
{
	%tower = $Server::PrisonEscape::Towers.tower[%tower];
	if (isObject(%tower.guard))
	{
		%tower.guard.isGuard = 0;
		//spawnPrisoner
	}
	%tower.guard = %client;
	//spawnguard
	%client.isGuard = 1;
	messageAll("\c3" @ %client.name @ "\c6 is now a guard at Tower " @ %tower @ "!");
}

function removeGuard(%client)
{
	for (%i = 0; %i < 4; %i++) //only do this four times; if all are filled it'll just take the place of the first one selected.
	{
		%towernum = %i + 1;
		if ($Server::PrisonEscape::Towers.tower[%towernum].guard == %client) 
		{
			$Server::PrisonEscape::Towers.tower[%towernum].guard = "";
			%client.isGuard = 0;
			return;
		}
	}
	echo("Cannot find " @ %client.name @ " in the list of guards!");
}

function killTower(%id)
{
	%tower = $Server::PrisonEscape::Towers.tower[%id];
	%client = %tower.guard;

	//kill the guard
	if (isObject(%client.player))
		%client.player.kill();

	//destroy the bricks but sequentially as to not lag everyone to death
	%tower.destroy(0);
}

function saveBricks(%brickgroup, %i) //edit to use the brickgroup data later?? (NTName[id][index] + NTName[id]count)
{									//would make it easier to spawn spotlights and such
	//talk(%brickgroup.getName() SPC %i);
	//look for bricks with the name of tower#
	if (%i >= %brickgroup.getCount())
	{
		messageAdmins("Tower Bricks saved to TowerGroup from " @ %brickgroup);
		messageAdmins("--Tower1 brickcount: " @ $Server::PrisonEscape::Towers.tower1.brickCount);
		messageAdmins("--Tower2 brickcount: " @ $Server::PrisonEscape::Towers.tower2.brickCount);
		messageAdmins("--Tower3 brickcount: " @ $Server::PrisonEscape::Towers.tower3.brickCount);
		messageAdmins("--Tower4 brickcount: " @ $Server::PrisonEscape::Towers.tower4.brickCount);
		messageAdmins("Generator: " @ $Server::PrisonEscape::Generator SPC "CommDish: " @ $Server::PrisonEscape::commDish);
		return;
	}
	%brick = %brickgroup.getObject(%i);
	%name = %brick.getName();

	//reset brick health
	%brick.damage = 0;

	//skip if there is no name
	if (%name $= "")
	{
		schedule(0, 0, saveBricks, %brickgroup, %i+1);
		return;
	}
	%name = getSubStr(%name, 1, strLen(%name));

	if (strPos(%name, "tower") >= 0)
	{
		%name = getSubStr(%name, 0, 6); //tower# is only 6 chars

		if (%name $= "tower1") $Server::PrisonEscape::Towers.tower1.addBrick(%brick);
		else if (%name $= "tower2") $Server::PrisonEscape::Towers.tower2.addBrick(%brick);
		else if (%name $= "tower3") $Server::PrisonEscape::Towers.tower3.addBrick(%brick);
		else if (%name $= "tower4") $Server::PrisonEscape::Towers.tower4.addBrick(%brick);
	} 
	else if (strPos(%name, "generator") >= 0)
	{
		$Server::PrisonEscape::generator = %brick;
	}
	else if (strPos(%name, "commDish") >= 0)
	{
		$Server::PrisonEscape::commDish = %brick;
	}
	else if (strPos(%brick.getDatablock().getName(), "Spawn") >= 0)
	{
		if (%name $= "spawn")
		{
			$Server::PrisonEscape::PrisonerSpawnPoints.spawn[$Server::PrisonEscape::PrisonerSpawnPoints.count] = %brick;
			$Server::PrisonEscape::PrisonerSpawnPoints.count++;
		}
		else
		{
			$Server::PrisonEscape::TowerSpawn[%name] = %brick;
			echo("Tower " @ %name "  spawnpoint has been saved.");
		}
	}
	//0 length schedule to run function when server can run it to not lag people
	schedule(0, 0, saveBricks, %brickgroup, %i+1);
}

function TowerGroup::addBrick(%this, %brick)
{
	if (!isObject(%brick) || %brick.getClassName() !$= "fxDTSBrick")
	{
		echo("TowerGroup: Cannot add " @ %brick @ "!");
		return;
	}

	%this.brick[%this.brickCount] = %brick;
	%this.brickCount++;
}

function TowerGroup::destroy(%this, %brickIndex)
{
	if (%brickIndex >= %this.brickCount)
		return;
	%this.brick[%brickIndex].fakeKillBrick(getRandom()-0.5 @ getRandom()-0.5 @ -1, 2);
	%this.brick[%brickIndex].schedule(2000, delete);
	serverPlay3D("brickBreakSound", %this.brick[%brickIndex].getPosition());

	%this.schedule(0, destroy, %brickIndex+1);
}