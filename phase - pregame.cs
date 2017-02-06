function spawnGuard(%tower, %suppress) {
	%tower = $Server::PrisonEscape::Towers.tower[%tower];
	if (!isObject(%tower)) {
		PPE_messageAdmins("!!! \c5Cannot spawn guard on tower " @ %tower @ " - tower not found!");
		return 0;
	}
	%spawnpoint = %tower.spawn.getSpawnPoint();
	if (!isObject(%spawnpoint)) {
		PPE_messageAdmins("!!! \c5Cannot spawn guard on tower " @ %tower @ " - spawnpoint not found!");
		return 0;
	}

	if (isObject(%tower.guard.player)) {
		%tower.guard.player.delete();
	}
	%tower.guard.createPlayer(%spawnpoint);
	if (!%suppress) {
		messageAll('', "\c3" @ %tower.guard.name @ "\c4 has been deployed at tower " @ %tower @ "!");
	}
}

function spawnGuards() {
	spawnGuard(1, 1);
	spawnGuard(2, 1);
	spawnGuard(3, 1);
	spawnGuard(4, 1);

	messageAll('', "\c4The guards have been spawned at their towers!");

	startLightBeamLoop($Server::PrisonEscape::Towers.tower1.spotlight);
	startLightBeamLoop($Server::PrisonEscape::Towers.tower2.spotlight);
	startLightBeamLoop($Server::PrisonEscape::Towers.tower3.spotlight);
	startLightBeamLoop($Server::PrisonEscape::Towers.tower4.spotlight);
}

function displayIntroCenterprint() {
	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		%cl = ClientGroup.getObject(%i);
		if (%cl.isGuard) {
			%cl.centerprint("<font:Arial Bold:34><shadowcolor:666666><shadow:0:4><color:E65714>JailBreak! <br><font:Arial Bold:26>\c6Prevent the prisoners from escaping!", 10);
		} else {
			%cl.centerprint("<font:Arial Bold:34><shadowcolor:666666><shadow:0:4><color:E65714>JailBreak! <br><font:Arial Bold:26>\c6Team up and escape!", 10);
		}
	}
}

