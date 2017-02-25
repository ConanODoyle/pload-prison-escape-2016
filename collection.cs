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

if (!isObject($Server::PrisonEscape::InfoBricks)) {
	$Server::PrisonEscape::InfoBricks = new SimSet() {};
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
	$Server::PrisonEscape::InfoBricks.clear();
	$Server::PrisonEscape::Cameras.clear();
	$Server::PrisonEscape::Generator = 0;
	$Server::PrisonEscape::CommDish = 0;
	$Server::PrisonEscape::DogSpawn = 0;

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

function serverCmdReplaceGuard(%cl, %towerNum, %name) {
	if (!%cl.isAdmin) {
		return;
	}
	%client = fcn(%name);
	if (!isObject(%client)) {
		return;
	}
	%tower = $Server::PrisonEscape::Towers.tower[%towerNum];
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
		$Server::PrisonEscape::haveAssignedBricks = 1;
		return;
	}
	%brick = %brickgroup.getObject(%i);
	%name = %brick.getName();

	//reset brick health
	%brick.damage = 0;

	if (%brick.getDatablock().isOpen) {
		%brick.door(4);
	}

	if (isObject(%brick.originalItem)) {
		%brick.setItem(%brick.originalItem);
	}

	if (%brick.getDatablock().specialBrickType $= "VehicleSpawn") {
		%brick.respawnVehicle();
	}

	//skip if there is no name
	if (%name $= "") {
		schedule(0, 0, prisonEscape_saveBricks, %brickgroup, %i+1);
		return;
	}
	%name = getSubStr(%name, 1, strLen(%name)); //removes underscore in name

	%brick.numViewers = 0;
	%brick.endLoopToggle();
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
		if (strPos(%name, "support") >= 0) {
			%brick.setColor($towerColor0);
		}
		if (strPos(%name, "info") >= 0) {
			$Server::PrisonEscape::InfoBricks.add(%brick);
		}
		if (isObject(%brick.vehicle)) {
			$Server::PrisonEscape::Towers.tower[%tower].spotlight = %brick.vehicle;
			%brick.vehicle.setShapeName("Tower " @ %tower, "8564862");
			%brick.vehicle.setShapeNameDistance(300);
		}
		%brick.tower = %tower;
	} else if (strPos(%name, "info") >= 0) {
		$Server::PrisonEscape::InfoBricks.add(%brick);
	} else if (strPos(%name, "dogSpawn") >= 0) {
		$Server::PrisonEscape::DogSpawn = %brick;
	} else if (strPos(%name, "Generator") >= 0) {
		$Server::PrisonEscape::Generator = %brick;
		%brick.setRaycasting(1);
		%brick.setColorFX(4);
	} else if (strPos(%name, "indicator") >= 0) {
		%brick.setColor(4);
	} else if (%name $= "BronsonDoor") {
		%brick.setRaycasting(1);
	} else if (strPos(%name, "generatorDoors") >= 0) {
		%brick.setRaycasting(1);
	} else if (strPos(%name, "generatorDoor") >= 0) {
		%brick.setEventEnabled("2", 0);
		%brick.setEventEnabled("0 1", 1);
	} else if (strPos(%name, "garageDoor") >= 0) {
		%brick.door(4);
	} else if (strPos(%name, "garageDoorSwitch") >= 0) {
		%brick.setEventEnabled("2 5 6", 0);
		%brick.setEventEnabled("1 3 4", 1);
		%brick.setColor(4);
	} else if (strPos(%name, "smokeGrenadeDoor") >= 0) {
		%brick.door(4);
	} else if (strPos(%name, "winBrick") >= 0) {
		%brick.setColliding(1);
	} else if (strPos(%name, "dog_spawn") >= 0) {
		%brick.setBotType(ShepherdDogHoleBot.getID());
		%brick.respawnBot();
	} else if (strPos(%name, "CommDish") >= 0) {
		$Server::PrisonEscape::CommDish = %brick; 
	} else if (strPos(%name, "camera") >= 0) {
		$Server::PrisonEscape::Cameras.add(%brick);
	} else if (strPos(strlwr(%name), "spawn") >= 0) {
		if (%name $= "Spawn") {
			$Server::PrisonEscape::PrisonerSpawnPoints.add(%brick);
		} else if (%name $= "infirmarySpawn") {
			$Server::PrisonEscape::InfirmarySpawnPoints.add(%brick);
		} else {
			PPE_messageAdmins("!!! \c5Out of place spawnpoint found! ID: " @ %brick);
		}
	}

	schedule(1, %brickgroup, prisonEscape_saveBricks, %brickgroup, %i+1);
}
