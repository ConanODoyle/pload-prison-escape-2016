function calculateStatistics()
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


	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObjects(%i);

		%client.shotsHit = 0; %client.shotsFired = 0;
		%client.chiselAttack = 0; %client.chiselHit = 0;
		%client.bucketsUsed = 0; %client.traysUsed = 0;
		%client.whiteOutTime = 0;
		%client.trays = 0; %client.bucketsPlonked = 0;
		%client.accuracy = 0; %client.weightedaccuracy = 0; %client.efficiency = 0;
		%client.setScore(0); /////////////////////////////////////////////////////////////////is this even a valid method?
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

package PrisonStatistics
{
	function GameConnection::spawnPlayer(%client) //or should I use createPlayer?
	{
		%client.spawnTime = $Sim::Time;
		return parent::spawnPlayer(%client);
	}

	function Armor::onRemove(%this, %obj)
	{
		if (isObject(%client = %obj.client))
		{
			%client.aliveTime += $Sim::Time - %client.spawnTime;
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
