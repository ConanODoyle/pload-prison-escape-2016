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
	%guards = getStatistic("Tower" @ %towerNum @ "Guard");
	setStatistic("Tower" @ %towerNum @ "Guard", trim(strReplace(strReplace(%guards, %tower.guard.bl_id, "") SPC %tower.guard.bl_id, "  ", " ")));
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

datablock TriggerData(swol_killTrigger){tickPeriodMS = 100;};

function swol_killTrigger::onEnterTrigger(%db,%trig,%pl)
{
	if($Server::PrisonEscape::RoundPhase != 0)
	{
		if(%trig.genKill)
		{
			if($Server::PrisonEscape::GeneratorOpened)
				return;
			talk(%pl.client.getPlayerName() SPC "was killed for exploiting genroom");
			%pl.kill();
			return;
		}
		if(!isObject(%cl = %pl.client))
			return;
		if(!%cl.minigame)
			return;
		if(%cl.guard)
		{
			spawnGuard(%cl.tower);
		}
		else
		{
			%pl.kill();
		}
	}
}
function spawnGenRoomKill()
{
	%pos = "-66 -123.5 34.1";
	%scale = "16 11 6.6";
	if(isObject($Swol_GenKill))
		$Swol_GenKill.delete();
	$Swol_GenKill = new Trigger()
	{
		datablock = swol_killTrigger;
		scale = %scale;
		polyhedron = "-0.5 -0.5 -0.5 1 0 0 0 1 0 0 0 1";
		position = %pos;
		rotation = "0 0 0 0";
		genKill = 1;
	};
}
function spawnKillGround()
{
	if(isObject($Swol_KillGround))
		$Swol_KillGround.delete();
	$Swol_KillGround = new Trigger()
	{
		datablock = swol_killTrigger;
		scale = "300 300 1";
		polyhedron = "-0.5 -0.5 -0.5 1 0 0 0 1 0 0 0 1";
		position = "0 0 1";
		rotation = "0 0 0 0";
	};
}