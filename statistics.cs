//statistics are now all global vars
exec("./roundNum.cs");
if ($Statistics::round $= "") {
	$Statistics::round = 0;
}

function setStatistic(%statistic, %val, %client) {
	//talk("Setting statistic of " @ %statistic);
	if (isObject(%client)) {
		$Statistics::round[$Statistics::round @ "_" @ %statistic @ "_" @ %client.bl_id] = %val;
	} else {
		$Statistics::round[$Statistics::round @ "_" @ %statistic @ "_Total"] = %val;
	}
	return %val;
}

function getStatistic(%statistic, %client) {
	//talk("Getting statistic of " @ %statistic);
	if (isObject(%client)) {
		return $Statistics::round[$Statistics::round @ "_" @ %statistic @ "_" @ %client.bl_id];
	} else {
		return $Statistics::round[$Statistics::round @ "_" @ %statistic @ "_Total"];
	}
}

function collectStatistics()
{
	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		%cl = ClientGroup.getObject(%i);

		if (%cl.isGuard) {
			if (%bestGuard $= "") {
				%bestGuard = %cl.bl_id;
				%rank = %cl.rankValue = getGuardRankValue(%cl);
				continue;
			} else {
				%newGuardRank = %cl.rankValue = getGuardRankValue(%cl);

				if (%newGuardRank > %rank) {
					%rank = %newGuardRank;
					%bestGuard = %cl.bl_id;
				}
			}
		} else {
			if (%bestPrisoner $= "") {
				%bestPrisoner = %cl.bl_id;
				%rank = %cl.rankValue = getPrisonerRankValue(%cl);
				continue;
			} else {
				%newPrisonerRank = %cl.rankValue = getPrisonerRankValue(%cl);

				if (%newPrisonerRank > %rank) {
					%rank = %newPrisonerRank;
					%bestPrisoner = %cl.bl_id;
				}
			}
		}
	}

	$bestPrisoner = %bestPrisoner;
	$bestPrisonerName = findClientByBL_ID(%bestPrisoner).name;
	$bestGuard = %bestGuard;
	$bestGuardName = findClientByBL_ID(%bestGuard).name;

	$fakeClient.bl_id = $bestPrisoner;
	$fakeClient.bl_id = $bestGuard;
}

function getPrisonerRankValue(%cl) {
	%chiselAttacks = %cl.chiselAttacks = getStatistic("ChiselAttacks", %cl);
	%chiselHits = %cl.chiselHits = getStatistic("ChiselHits", %cl);

	%buffHits = %cl.buffHits = getStatistic("BuffHits", %cl);

	%objectiveHits = %cl.objectiveHits = getNumObjectiveHits(%cl);
	%deaths = %cl.deaths = getStatistic("Deaths", %cl);
	%timeAlive = %cl.timeAlive = getStatistic("timeAlive", %cl);

	return (%objectiveHits * 2 - %buffHits - %deaths*10 + %timeAlive/1000/60 + (%chiselHits/%chiselAttacks - 0.3) * 20);
}

function getGuardRankValue(%cl) {
	%SniperShotsFired = %cl.SniperShotsFired = getStatistic("SniperShotsFired", %cl);
	%SniperShotsHit = %cl.SniperShotsHit = getStatistic("SniperShotsHit", %cl);

	%LMGShotsFired = %cl.LMGShotsFired = getStatistic("LMGShotsFired", %cl);
	%LMGShotsHit = %cl.LMGShotsHit = getStatistic("LMGShotsHit", %cl);

	%kills = %cl.score;

	return (%kills * 2 + %SniperShotsHit + (%LMGShotsHit/%LMGShotsFired - 0.2) * 20 + (%SniperShotsHit/%SniperShotsFired) * 100);
}

function getNumObjectiveHits(%cl) {
	if (%cl.isGuard) {
		return 0;
	}

	%commDish = getStatistic("CommDishHit", %cl);
	%plates = getStatistic("PlatesHit", %cl);
	%supports = getStatistic("TowerSupportsHit", %cl);
	%generator = getStatistic("GeneratorWindowsHit", %cl);

	return %commDish * 2 + %generator * 1.5 + %plates + %supports;
}

// function collectStatistics()
// {
// 	//calculate just as round ends so players leaving right after round ends still show up on the list
// 	//optionally package gameconnection::onDisconnect to save the stats into global vars. Nice touch to have.
// 	//do the second option once the fundamentals for the gamemode are complete
// 	%mvpAcc = 0;
// 	%mvpChisel = 0;
// 	%mostAcc = 0;
// 	//iterate through clients and calculate statistics
// 	for (%i = 0; %i < ClientGroup.getCount(); %i++)
// 	{
// 		%client = ClientGroup.getObject(%i);
// 		//find mvp guard, mvp prisoner
// 		//have a baseline cutoff for MVP status
// 		//guard accuracy weighted based on number of kills
// 		%client.weightedaccuracy = (%client.shotsHit+%client.getScore()/2)/%client.shotsFired;
// 		%client.accuracy = %client.shotsHit/%client.shotsFired;
// 		%client.efficiency = %client.chiselHit/%client.chiselAttack;

// 		if (%client.isGuard && %client.accuracy > %mostAcc) 
// 		{
// 			$Server::PrisonEscape::SharpshooterGuard = %client;
// 			%mostAcc = %client.accuracy;
// 		}
// 		if (%client.isGuard && %client.weightedaccuracy > %mvpAcc) 
// 		{
// 			$Server::PrisonEscape::MVPGuard = %client;
// 			%mvpAcc = %client.weightedaccuracy;
// 		}
// 		else if (!%client.isGuard && %client.chiselHit > %mvpChisel)
// 		{
// 			$Server::PrisonEscape::MVPPrisoner = %client;
// 			%mvpChisel = %client.chiselHit;
// 		}
// 	}
// 	$Server::PrisonEscape::TopAcc = %mvpAcc;
// 	$Server::PrisonEscape::TopChisel = %mvpChisel;
// }  

function clearStatistics()
{
	$Statistics::round[$Statistics::round @ "_Date"] = getDateTime();
	export("$Statistics::round" @ $Statistics::round @ "*", "config/PPE Statistics/Round " @ $Statistics::round @ ".cs");
	$Statistics::round++;
	export("$Statistics::round", "Add-ons/Gamemode_PPE/roundNum.cs");

	for (%i = 0; %i < ClientGroup.getCount(); %i++) {
		ClientGroup.getObject(%i).setScore(0);
	}
	// $Server::PrisonEscape::TopAcc = 0;
	// $Server::PrisonEscape::TopChisel = 0;
	// $Server::PrisonEscape::MVPGuard = 0;
	// $Server::PrisonEscape::SharpshooterGuard = 0;
	// $Server::PrisonEscape::MVPPrisoner = 0;
	// $Server::PrisonEscape::GuardMessagesSent = 0;
	// $Server::PrisonEscape::PrisonerMessagesSent = 0;
	// $Server::PrisonEscape::PrisonerDeaths = 0;
	// $Server::PrisonEscape::BricksDestroyed = 0;
	// $Server::PrisonEscape::SniperRifleBullets = 0;
	// $Server::PrisonEscape::ChiselAttacks = 0;
	// $Server::PrisonEscape::TraysUsed = 0;
	// $Server::PrisonEscape::BucketsUsed = 0;
	// $Server::PrisonEscape::SteaksEaten = 0;
	// $Server::PrisonEscape::SmokeGrenadesThrown = 0;
	// $Server::PrisonEscape::firstPrisonerOut = 0;
	// $Server::PrisonEscape::soapThrown = 0;


	// for (%i = 0; %i < ClientGroup.getCount(); %i++)
	// {
	// 	%client = ClientGroup.getObject(%i);
	// 	%client.isGuard = 0; %client.tower = "";

	// 	%client.shotsHit = 0; %client.shotsFired = 0;
	// 	%client.chiselAttack = 0; %client.chiselHit = 0;
	// 	%client.bucketsUsed = 0; %client.traysUsed = 0;

	// 	%client.whiteOutTime = 0; %client.stunnedTime = 0;
	// 	%client.electrocutedTime = 0; %client.aliveTime = 0;

	// 	%client.SmokeGrenadesThrown = 0;
	// 	%client.trays = 0; %client.bucketsPlonked = 0;

	// 	%client.accuracy = 0; %client.weightedaccuracy = 0; %client.efficiency = 0;
	// 	%client.setScore(0);
	// }
}

//statistics to store:
//////Projectile::onCollision() - shots that hit players/bricks (check if package isActive in item prison cause dead end)
//////GameConnection::respawn() - Armor::onDisabled()/Armor::onRemove() - record how long player has been alive; also time as El Chapo
//////Armor::setWhiteOut() - record time blinded (need to increment value in whiteout not package)
//////fxDTSBrick::onDeath() - record num bricks destroyed (excluding towers)
//////serverCmdMessageSent() - record number of messages sent during the round (special statistic for those who say the most words?? or total messages sent in round)
//////SniperRifleProjectile::onCollision() - record number of buckets/trays plonked
//////SniperRifleSpotlightImage::onFire() - record number of shots made
//////ChiselImage::onFire() - record number of chisel attacks made
//
//%client.shotsHit/shotsFired
//%client.chiselAttack/Hit
//%client.bucketsUsed/traysUsed
//%client.whiteOutTime
//%client.trays/bucketsPlonked

// $Server::PrisonEscape::GuardMessagesSent = 0;
// $Server::PrisonEscape::MVPGuard = 0;
// $Server::PrisonEscape::SharpshooterGuard = 0;
// $Server::PrisonEscape::MVPPrisoner = 0;
// $Server::PrisonEscape::PrisonerMessagesSent = 0;
// $Server::PrisonEscape::PrisonerDeaths = 0;
// $Server::PrisonEscape::BricksDestroyed = 0;
// $Server::PrisonEscape::SniperRifleBullets = 0;
// $Server::PrisonEscape::ChiselAttacks = 0;
// $Server::PrisonEscape::TraysUsed = 0;
// $Server::PrisonEscape::BucketsUsed = 0;
// $Server::PrisonEscape::SteaksEaten = 0;
// $Server::PrisonEscape::SmokeGrenadesThrown = 0;
// $Server::PrisonEscape::firstPrisonerOut = 0;

// package PrisonStatistics
// {
// 	function GameConnection::createPlayer(%client, %pos)
// 	{
// 		if (%client.spawnTime !$= "") {
// 			%client.aliveTime += $Sim::Time - %client.spawnTime;
// 		}
// 		%client.spawnTime = $Sim::Time;
// 		return parent::createPlayer(%client, %pos);
// 	}

// 	function Armor::onRemove(%this, %obj)
// 	{
// 		if (isObject(%client = %obj.client))
// 		{
// 			%client.aliveTime += $Sim::Time - %client.spawnTime;
// 			%client.spawnTime = "";
// 			if (!%client.isGuard)
// 				$Server::PrisonEscape::PrisonerDeaths++;
// 		}
// 		return parent::onRemove(%this, %obj);
// 	}

// 	function fxDTSBrick::onDeath(%brick)
// 	{
// 		$Server::PrisonEscape::BricksDestroyed++;
// 		return parent::onDeath(%brick);
// 	}
// };
// activatePackage(PrisonStatistics);

function getStatisticToDisplay() {
	switch ($Server::PrisonEscape::currentStatistic)
	{
		case 0: %stat = "<color:8AD88D>MVP Guard\c6: " @ $bestGuardname;
		case 1: %stat = "<color:ff8724>MVP Prisoner\c6: " @ $bestPrisonername;
		case 2: %stat = "Trays Used\c6: " @ getStatistic("TraysPickedUp") + 0;
		case 3: %stat = "Buckets Used\c6: " @ getStatistic("BucketsPickedUp") + 0;		
		case 4: %stat = "Bricks Destroyed\c6: " @ getStatistic("BricksDestroyed") + 0;
		case 5: %stat = "Prisoners Killed\c6: " @ getStatistic("Deaths") + 0;
	}
	$Server::PrisonEscape::currentStatistic++;
	$Server::PrisonEscape::currentStatistic %= 6;
	return %stat;
}

function displayScoreBoard(%stat) {
	if (!isObject($textGroup)) {
		$textGroup = new ScriptGroup(TextGroup) {};
	}

	for (%i = 1; %i < 9; %i++) {

	}
}