function spawnGuard(%towerNum, %suppress) {
	%tower = $Server::PrisonEscape::Towers.tower[%towerNum];
	if (!isObject(%tower)) {
		PPE_messageAdmins("!!! \c5Cannot spawn guard " @ %tower.client.name @ " on tower " @ %towerNum @ " - tower not found!");
		return 0;
	}
	%spawnpoint = %tower.spawn.getSpawnPoint();
	if (!isObject(%tower.spawn)) {
		PPE_messageAdmins("!!! \c5Cannot spawn guard " @ %tower.client.name @ " on tower " @ %towerNum @ " - spawnpoint not found!");
		return 0;
	}

	if (isObject(%tower.guard.player)) {
		%tower.guard.player.delete();
	}
	if (!isObject(%tower.guard)) {
		PPE_messageAdmins("\c3!!! \c6Warning: guard not found for tower "@ %towerNum);
	}
	%tower.guard.createPlayer(%spawnpoint);
	setStatistic("Tower" @ %towerNum @ "Guard", trim(getStatistic("Tower" @ %towerNum @ "Guard") SPC %tower.guard.bl_id));
	if (!%suppress) {
		messageAll('', "\c3" @ %tower.guard.name @ "\c4 has been deployed at tower " @ %towerNum @ "!");
	}
}

function spawnGuards() {
	spawnGuard(1, 0);
	spawnGuard(2, 0);
	spawnGuard(3, 0);
	spawnGuard(4, 0);

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
			%cl.centerprint("<br><br><br><br><br><font:Arial Black:48><shadowcolor:555555><shadow:0:4><color:E65714>Conan's Prison Break! <br><font:Arial Bold:26><color:ffffff>Prevent the prisoners from escaping!", 20);
		} else {
			%cl.centerprint("<br><br><br><br><br><font:Arial Black:48><shadowcolor:555555><shadow:0:4><color:E65714>Conan's Prison Break! <br><font:Arial Bold:26><color:ffffff>Team up and escape!", 20);
		}
	}
}

$outBound = "25.5 -8.5 7.2";
$inBound = "-95.5 -129 7.2";

function startSpotlights() {
	%x1 = getWord($outBound, 0); %x2 = getWord($inBound, 0);
	%y1 = getWord($outBound, 1); %y2 = getWord($inBound, 1);
	%z = getWord($outBound, 2);
	for (%i = 1; %i < 5; %i++) {
		%x = getRandom(getMin(%x1, %x2) * 10, getMax(%x1, %x2) * 10);
		%y = getRandom(getMin(%y1, %y2) * 10, getMax(%y1, %y2) * 10);
		%pos = %x/10 SPC %y/10 SPC %z;

		startLightBeamLoop($Server::PrisonEscape::Towers.tower[%i].spotlight);
		$Server::PrisonEscape::Towers.tower[%i].spotlight.spotlightTargetLocation = %pos;
	}
}