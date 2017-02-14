//assumes the brickgroup is of blid $buildBLID

if (!isObject($Server::PrisonEscape::Towers)) {
	$Server::PrisonEscape::Towers = new ScriptObject() {};
}

if (!isObject($Server::PrisonEscape::PrisonerSpawnPoints)) {
	$Server::PrisonEscape::PrisonerSpawnPoints = new SimSet() {};
}

if (!isObject($Server::PrisonEscape::InfirmarySpawnPoints)) {
	$Server::PrisonEscape::InfirmarySpawnPoints = new SimSet() {};
}

if (!isObject($Server::PrisonEscape::Cameras)) {
	$Server::PrisonEscape::Cameras = new SimSet() {};
}

$buildBLID = 4928;

function assignBricks() {
	%brickgroup = "BrickGroup_" @ $buildBLID; //probably public, but set up this way to test with nonpublic bricks
	%towerGroup = $Server::PrisonEscape::Towers;

	for (%i = 1; %i < 5; %i++) {
		if (!isObject(%towerGroup.tower[%i]))
			%towerGroup.tower[%i] = new SimSet() {};
		else {
			%towerGroup.tower[%i].clear();
			%towerGroup.tower[%i].guard = "";
			%towerGroup.tower[%i].spawn = "";
			%towerGroup.tower[%i].isDestroyed = 0;
			%towerGroup.tower[%i].spotlight = 0;
			%towerGroup.tower[%i].origCount = 0;
		}
	}
	$Server::PrisonEscape::PrisonerSpawnPoints.clear();
	$Server::PrisonEscape::InfirmarySpawnPoints.clear();
	$Server::PrisonEscape::Cameras.clear();
	$Server::PrisonEscape::Generator = 0;
	$Server::PrisonEscape::CommDish = 0;

	PPE_messageAdmins("!!! \c5Beginning search for gamemode bricks...");
	PPE_messageAdmins("!!! \c5Brickgroup brickcount: " @ %brickgroup.getCount());
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
	%client.tower = %towernum;

	$Server::PrisonEscape::Towers.tower[%towernum].guard = %client;
}

function replaceGuard(%client, %towerNum) {
	%tower = $Server::PrisonEsape::Towers.tower[%towerNum];
	if (!isObject(%tower) || %tower.isDestroyed) {
		PPE_messageAdmins("!!! \c5Failed to replace guard at tower " @ %tower @ " - tower does not exist!");
		return;
	}

	if (isObject(%tower.guard)) {
		%tower.guard.isGuard = 0;
		//guard spawn as prisoner
	}
	%tower.guard = %client;
	%client.isGuard = 1;
	//respawn client at tower
	spawnGuard(%towerNum, 0);
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
	%tower.destroy();
}

function validateTower(%id, %brick) {
	%tower = $Server::PrisonEscape::Towers.tower[%id];
	%tower.remove(%brick);
	if (%tower.getCount() <= %tower.origCount - 4) {
		killTower(%id);
	}
	validateGameWin();
}

function prisonEscape_saveBricks(%brickgroup, %i) {									//would make it easier to spawn spotlights and such
	//talk(%brickgroup.getName() SPC %i);
	//look for bricks with the name of tower#
	if (%i >= %brickgroup.getCount()) {
		PPE_messageAdmins("!!! \c5Tower Bricks saved to Towers from " @ %brickgroup);
		PPE_messageAdmins("!!! \c5--T1 bc: " @ $Server::PrisonEscape::Towers.tower1.getCount());
		PPE_messageAdmins("!!! \c5--T2 bc: " @ $Server::PrisonEscape::Towers.tower2.getCount());
		PPE_messageAdmins("!!! \c5--T3 bc: " @ $Server::PrisonEscape::Towers.tower3.getCount());
		PPE_messageAdmins("!!! \c5--T4 bc: " @ $Server::PrisonEscape::Towers.tower4.getCount());
		PPE_messageAdmins("!!! \c5Generator: " @ $Server::PrisonEscape::Generator SPC "CommDish: " @ $Server::PrisonEscape::CommDish);
		PPE_messageAdmins("!!! \c5# Prisoner Spawns: " @ $Server::PrisonEscape::PrisonerSpawnPoints.getcount());
		PPE_messageAdmins("!!! \c5# Infirmary Spawns: " @ $Server::PrisonEscape::InfirmarySpawnPoints.getcount());

		$Server::PrisonEscape::Towers.tower1.origCount = $Server::PrisonEscape::Towers.tower1.getCount();
		$Server::PrisonEscape::Towers.tower2.origCount = $Server::PrisonEscape::Towers.tower2.getCount();
		$Server::PrisonEscape::Towers.tower3.origCount = $Server::PrisonEscape::Towers.tower3.getCount();
		$Server::PrisonEscape::Towers.tower4.origCount = $Server::PrisonEscape::Towers.tower4.getCount();
		return;
	}
	%brick = %brickgroup.getObject(%i);
	%name = %brick.getName();

	//reset brick health
	%brick.damage = 0;

	if (isObject(%brick.originalItem)) {
		%brick.setItem(%brick.originalItem);
	}

	//skip if there is no name
	if (%name $= "") {
		schedule(0, 0, prisonEscape_saveBricks, %brickgroup, %i+1);
		return;
	}
	%name = getSubStr(%name, 1, strLen(%name)); //removes underscore in name

	if (%name $= "LoadingCamBrick") {
		$Server::PrisonEscape::LoadingCamBrick = %brick;
	} else if (%name $= "LoadingCamBrickTarget") {
		$Server::PrisonEscape::LoadingCamBrickTarget = %brick;
	} else if (%name $= "PrisonPreview") {
		$Server::PrisonEscape::PrisonPreview = %brick;
	} else if (%name $= "PrisonPreviewTarget") {
		$Server::PrisonEscape::PrisonPreviewTarget = %brick;
	} else if (strPos(%name, "tower") >= 0) {
		%subName = getSubStr(%name, 0, 6);

		%tower = -1;
		switch$ (%subName) {
			case "tower1": %tower = 1; $Server::PrisonEscape::Towers.tower1.add(%brick);
			case "tower2": %tower = 2; $Server::PrisonEscape::Towers.tower2.add(%brick);
			case "tower3": %tower = 3; $Server::PrisonEscape::Towers.tower3.add(%brick);
			case "tower4": %tower = 4; $Server::PrisonEscape::Towers.tower4.add(%brick);
		}

		if (strPos(%name, "spawn") >= 0) {
			$Server::PrisonEscape::Towers.tower[%tower].spawn = %brick;
			echo("Tower " @ %name @ " spawnpoint has been saved.");
		}
		if (isObject(%brick.vehicle)) {
			$Server::PrisonEscape::Towers.tower[%tower].spotlight = %brick.vehicle;
		}
		%brick.tower = %tower;
	} else if (strPos(%name, "Generator") >= 0) {
		$Server::PrisonEscape::Generator = %brick;
	} else if (strPos(%name, "CommDish") >= 0) {
		$Server::PrisonEscape::CommDish = %brick; 
	} else if (strPos(%name, "camera") >= 0) {
		$Server::PrisonEscape::Cameras.add(%brick);
	} else if (strPos(%brick.getDatablock().getName(), "Spawn") >= 0) {
		if (%name $= "spawn") {
			$Server::PrisonEscape::PrisonerSpawnPoints.add(%brick);
		} else if (%name $= "infirmarySpawn") {
			$Server::PrisonEscape::InfirmarySpawnPoints.add(%brick);
		} else {
			PPE_messageAdmins("!!! \c5Out of place spawnpoint found! ID: " @ %brick);
		}
	}

	schedule(1, %brickgroup, prisonEscape_saveBricks, %brickgroup, %i+1);
}

function SimSet::destroy(%this) {
	if (%this.getCount() <= 0)
		return;
	%brick = %this.getObject(%this.getCount()-1);
	%brick.fakeKillBrick((getRandom()-0.5)*20 SPC (getRandom()-0.5)*20 SPC 15, 2);
	%brick.schedule(2000, delete);
	%this.remove(%brick);
	if (isObject(%brick.item))
		%brick.item.delete();
	if (isObject(%brick.vehicle))
		%brick.vehicle.kill();
	serverPlay3D("brickBreakSound", %brick.getPosition());

	%this.schedule(1, destroy);
}