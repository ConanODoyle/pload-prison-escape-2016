function collectStatistics()
{
	//calculate just as round ends so players leaving right after round ends still show up on the list
	//optionally package gameconnection::onDisconnect to save the stats into global vars. Nice touch to have.
	//do the second option once the fundamentals for the gamemode are complete
	%mvpAcc = 0;
	%mvpChisel = 0;
	%mostAcc = 0;
	//iterate through clients and calculate statistics
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		//find mvp guard, mvp prisoner
		//have a baseline cutoff for MVP status
		//guard accuracy weighted based on number of kills
		%client.weightedaccuracy = (%client.shotsHit+%client.getScore()/2)/%client.shotsFired;
		%client.accuracy = %client.shotsHit/%client.shotsFired;
		%client.efficiency = %client.chiselHit/%client.chiselAttack;

		if (%client.isGuard && %client.accuracy > %mostAcc) 
		{
			$Server::PrisonEscape::SharpshooterGuard = %client;
			%mostAcc = %client.accuracy;
		}
		if (%client.isGuard && %client.weightedaccuracy > %mvpAcc) 
		{
			$Server::PrisonEscape::MVPGuard = %client;
			%mvpAcc = %client.weightedaccuracy;
		}
		else if (!%client.isGuard && %client.chiselHit > %mvpChisel)
		{
			$Server::PrisonEscape::MVPPrisoner = %client;
			%mvpChisel = %client.chiselHit;
		}
	}
	$Server::PrisonEscape::TopAcc = %mvpAcc;
	$Server::PrisonEscape::TopChisel = %mvpChisel;
}

function clearStatistics()
{
	$Server::PrisonEscape::TopAcc = 0;
	$Server::PrisonEscape::TopChisel = 0;
	$Server::PrisonEscape::MVPGuard = 0;
	$Server::PrisonEscape::SharpshooterGuard = 0;
	$Server::PrisonEscape::MVPPrisoner = 0;
	$Server::PrisonEscape::GuardMessagesSent = 0;
	$Server::PrisonEscape::PrisonerMessagesSent = 0;
	$Server::PrisonEscape::PrisonerDeaths = 0;
	$Server::PrisonEscape::BricksDestroyed = 0;
	$Server::PrisonEscape::SniperRifleBullets = 0;
	$Server::PrisonEscape::ChiselAttacks = 0;
	$Server::PrisonEscape::TraysUsed = 0;
	$Server::PrisonEscape::BucketsUsed = 0;
	$Server::PrisonEscape::SteaksEaten = 0;
	$Server::PrisonEscape::SmokeGrenadesThrown = 0;
	$Server::PrisonEscape::firstPrisonerOut = 0;


	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObjects(%i);
		%client.isGuard = 0; %client.tower = "";

		%client.shotsHit = 0; %client.shotsFired = 0;
		%client.chiselAttack = 0; %client.chiselHit = 0;
		%client.bucketsUsed = 0; %client.traysUsed = 0;

		%client.whiteOutTime = 0; %client.stunnedTime = 0;
		%client.electrocutedTime = 0; %client.aliveTime = 0;

		%client.SmokeGrenadesThrown = 0;
		%client.trays = 0; %client.bucketsPlonked = 0;

		%client.accuracy = 0; %client.weightedaccuracy = 0; %client.efficiency = 0;
		%client.setScore(0);
	}
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

$Server::PrisonEscape::GuardMessagesSent = 0;
$Server::PrisonEscape::MVPGuard = 0;
$Server::PrisonEscape::SharpshooterGuard = 0;
$Server::PrisonEscape::MVPPrisoner = 0;
$Server::PrisonEscape::PrisonerMessagesSent = 0;
$Server::PrisonEscape::PrisonerDeaths = 0;
$Server::PrisonEscape::BricksDestroyed = 0;
$Server::PrisonEscape::SniperRifleBullets = 0;
$Server::PrisonEscape::ChiselAttacks = 0;
$Server::PrisonEscape::TraysUsed = 0;
$Server::PrisonEscape::BucketsUsed = 0;
$Server::PrisonEscape::SteaksEaten = 0;
$Server::PrisonEscape::SmokeGrenadesThrown = 0;
$Server::PrisonEscape::firstPrisonerOut = 0;

package PrisonStatistics
{
	function GameConnection::createPlayer(%client, %pos)
	{
		if (%client.spawnTime !$= "") {
			%client.aliveTime += $Sim::Time - %client.spawnTime;
		}
		%client.spawnTime = $Sim::Time;
		return parent::createPlayer(%client, %pos);
	}

	function Armor::onRemove(%this, %obj)
	{
		if (isObject(%client = %obj.client))
		{
			%client.aliveTime += $Sim::Time - %client.spawnTime;
			%client.spawnTime = "";
			if (!%client.isGuard)
				$Server::PrisonEscape::PrisonerDeaths++;
		}
		return parent::onRemove(%this, %obj);
	}

	function fxDTSBrick::onDeath(%brick)
	{
		$Server::PrisonEscape::BricksDestroyed++;
		return parent::onDeath(%brick);
	}
};
activatePackage(PrisonStatistics);

function getStatistic() {
	switch ($Server::PrisonEscape::currentStatistic)
	{
		case 0: %stat = "Sharpshooter\c6: " @ 
			(strLen($Server::PrisonEscape::SharpshooterGuard.name) > 10 ? getSubStr($Server::PrisonEscape::SharpshooterGuard.name, 9) @ "." : $Server::PrisonEscape::SharpshooterGuard.name)
			SPC "-" SPC ($Server::PrisonEscape::TopAcc <= 0 ? 0 : $Server::PrisonEscape::TopAcc) SPC "acc";
		case 1: %stat = "Escape Artist\c6: " @ 
			(strLen($Server::PrisonEscape::MVPPrisoner.name) > 10 ? getSubStr($Server::PrisonEscape::MVPPrisoner.name, 9) @ "." : $Server::PrisonEscape::MVPPrisoner.name)
			SPC "-" SPC ($Server::PrisonEscape::TopChisel <= 0 ? 0 : $Server::PrisonEscape::TopChisel) SPC "bricks";
		case 2: %stat = "Riot Control\c6: "	@
			(strLen($Server::PrisonEscape::MVPGuard.name) > 10 ? getSubStr($Server::PrisonEscape::MVPGuard.name, 9) @ "." : $Server::PrisonEscape::MVPGuard.name)
			SPC "-" SPC ($Server::PrisonEscape::MVPGuard.getScore() <= 0 ? 0 : $Server::PrisonEscape::MVPGuard.getScore()) SPC "kills";
		case 3: %stat = "Guard Messages Sent\c6: " @ ($Server::PrisonEscape::GuardMessagesSent <= 0 ? 0 : $Server::PrisonEscape::GuardMessagesSent);
		case 4: %stat = "Prisoner Messages Sent\c6: " @ ($Server::PrisonEscape::PrisonerMessagesSent <= 0 ? 0 : $Server::PrisonEscape::PrisonerMessagesSent);
		case 5: %stat = "Prisoner Deaths\c6: " @ ($Server::PrisonEscape::PrisonerDeaths <= 0 ? 0 : $Server::PrisonEscape::PrisonerDeaths);
		case 6: %stat = "Bricks Destroyed\c6: " @ ($Server::PrisonEscape::BricksDestroyed <= 0 ? 0 : $Server::PrisonEscape::BricksDestroyed);
		case 7: %stat = "Bullets Fired\c6: " @ ($Server::PrisonEscape::SniperRifleBullets <= 0 ? 0 : $Server::PrisonEscape::SniperRifleBullets);
		case 8: %stat = "Chisel Swings\c6: " @ ($Server::PrisonEscape::ChiselAttacks <= 0 ? 0 : $Server::PrisonEscape::ChiselAttacks);
		case 9: %stat = "Trays Used\c6: " @ ($Server::PrisonEscape::TraysUsed <= 0 ? 0 : $Server::PrisonEscape::TraysUsed);
		case 10: %stat = "Buckets Used\c6: " @ ($Server::PrisonEscape::BucketsUsed <= 0 ? 0 : $Server::PrisonEscape::BucketsUsed);
		case 11: %stat = "Steaks Eaten\c6: " @ ($Server::PrisonEscape::SteaksEaten <= 0 ? 0 : $Server::PrisonEscape::SteaksEaten);
	}
	return %stat;
}