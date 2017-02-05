//assumes the brickgroup is of blid $buildBLID

if (!isObject($Server::PrisonEscape::Towers)) {
	$Server::PrisonEscape::Towers = new ScriptGroup() {};
}

if (!isObject($Server::PrisonEscape::PrisonerSpawnPoints)) {
	$Server::PrisonEscape::PrisonerSpawnPoints = new ScriptGroup() {};
}

$buildBLID = 4928;

function assignBricks() {
	%brickgroup = "BrickGroup_" @ $buildBLID; //probably public, but set up this way to test with nonpublic bricks
	%towerGroup = $Server::PrisonEscape::Towers;

	for (%i = 1; %i < 5; %i++) {
		if (!isObject(%towerGroup.tower[%i]))
			%towerGroup.tower[%i] = new ScriptGroup() {};
		else {
			%towerGroup.tower[%i].clear();
			%towerGroup.tower[%i].guard = "";
			%towerGroup.tower[%i].spawn = "";
			%towerGroup.isDestroyed = 0;
		}
	}
	$Server::PrisonEscape::PrisonerSpawnPoints.clear();
	$Server::PrisonEscape::generator = 0;
	$Server::PrisonEscape::commDish = 0;
	//iterate through and save the bricks into the scriptobjects

	PPE_messageAdmins("!!! \c5Beginning search for gamemode bricks...");
	prisonEscape_saveBricks(%brickgroup, 0);
}

function assignGuard(%client) {
	//randomly pick a tower to assign the client to
	%towernum = getRandom(1, 4);

	//if its filled, look for an empty spot
	if (isObject($Server::PrisonEscape::Towers.tower[%towernum].guard))
		for (%i = 0; %i < 4; %i++) {
			%towernum = (%towernum % 4) + 1;
			if (!isObject($Server::PrisonEscape::Towers.tower[%towernum].guard))
				break;
		}
	echo(%client.name @ " assigned to tower " @ %towernum);
	if (%i == 3) { //remove guard status of overridden player
		$Server::PrisonEscape::Towers.tower[%towernum].guard.isGuard = 0;
	}
	%client.isGuard = 1;

	$Server::PrisonEscape::Towers.tower[%towernum].guard = %client;
}

function replaceGuard(%client, %tower) {
	%tower = $Server::PrisonEsape::Towers.tower[%tower];
	if (!isObject %tower) || %tower.isDestroyed) {
		PPE_messageAdmins("!!! \c5Failed to replace guard at tower " @ %tower "!");
	}

	if (isObject(%tower.guard)) {
		%tower.guard.isGuard = 0;
		//guard spawn as prisoner
	}
	%tower.guard = %client;
	%client.isGuard = 1;
	//respawn client at tower
	messageAll("\c3" @ %client.name @ "\c6 is now a guard at Tower " @ %tower @ "!");
}

function removeGuard(%client) {
	for (%i = 0; %i < 4; %i++) {
		%towernum = %i + 1;
		if ($Server::PrisonEscape::Towers.tower[%towernum].guard == %client) {
			$Server::PrisonEscape::Towers.tower[%towernum].guard = "";
			%client.isGuard = 0;
			return;
		}
	}
	echo("Cannot find " @ %client.name @ " in the list of guards!");
}

function killTower(%id) {
	%tower = $Server::PrisonEscape::Towers.tower[%id];
	%tower.isDestroyed = 1;
	%client = %tower.guard;

	//kill the guard
	if (isObject(%client.player))
		%client.player.kill();

	//destroy the bricks but sequentially as to not lag everyone to death
	%tower.destroy(0);
}

function prisonEscape_saveBricks(%brickgroup, %i) {									//would make it easier to spawn spotlights and such
	//talk(%brickgroup.getName() SPC %i);
	//look for bricks with the name of tower#
	if (%i >= %brickgroup.getCount()) {
		PPE_messageAdmins("!!! \c5Tower Bricks saved to TowerGroup from " @ %brickgroup);
		PPE_messageAdmins("!!! \c5--Tower1 brickcount: " @ $Server::PrisonEscape::Towers.tower1.brickCount);
		PPE_messageAdmins("!!! \c5--Tower2 brickcount: " @ $Server::PrisonEscape::Towers.tower2.brickCount);
		PPE_messageAdmins("!!! \c5--Tower3 brickcount: " @ $Server::PrisonEscape::Towers.tower3.brickCount);
		PPE_messageAdmins("!!! \c5--Tower4 brickcount: " @ $Server::PrisonEscape::Towers.tower4.brickCount);
		PPE_messageAdmins("!!! \c5Generator: " @ $Server::PrisonEscape::Generator SPC "CommDish: " @ $Server::PrisonEscape::commDish);
		return;
	}
	%brick = %brickgroup.getObject(%i);
	%name = %brick.getName();

	//reset brick health
	%brick.damage = 0;

	//skip if there is no name
	if (%name $= "") {
		schedule(0, 0, prisonEscape_saveBricks, %brickgroup, %i+1);
		return;
	}
	%name = getSubStr(%name, 1, strLen(%name)); //removes underscore in name

	if (strPos(%name, "tower") >= 0) {
		%name = getSubStr(%name, 0, 6);

		%tower = -1;
		switch$ (%name) {
			case "tower1": %tower = 1; $Server::PrisonEscape::Towers.tower1.add(%brick);
			case "tower2": %tower = 2; $Server::PrisonEscape::Towers.tower2.add(%brick);
			case "tower3": %tower = 3; $Server::PrisonEscape::Towers.tower3.add(%brick);
			case "tower4": %tower = 4; $Server::PrisonEscape::Towers.tower4.add(%brick);
		}

		if (strPos(%name, "spawn") >= 0) {
			$Server::PrisonEscape::Towers.tower[%tower].spawnpoint = %brick;
			echo("Tower " @ %name @ " spawnpoint has been saved.");
		}
	} else if (strPos(%name, "generator") >= 0) {
		$Server::PrisonEscape::generator = %brick;
	} else if (strPos(%name, "commDish") >= 0) {
		$Server::PrisonEscape::commDish = %brick;
	} else if (strPos(%brick.getDatablock().getName(), "Spawn") >= 0) {
		if (%name $= "spawn") {
			$Server::PrisonEscape::PrisonerSpawnPoints.add(%brick);
		}
		else {
			PPE_messageAdmins("!!! \c5Out of place spawnpoint found! ID: " @ %brick);
		}
	}

	schedule(1, %brickgroup, prisonEscape_saveBricks, %brickgroup, %i+1);
}

function ScriptGroup::destroy(%this) {
	if (%this.getCount() <= 0)
		return;
	%brick = %this.getObject(0);
	%brick.fakeKillBrick(getRandom()-0.5 @ getRandom()-0.5 @ -1, 2);
	%brick.schedule(2000, delete);
	serverPlay3D("brickBreakSound", %brick.getPosition());

	%this.schedule(1, destroy);
}