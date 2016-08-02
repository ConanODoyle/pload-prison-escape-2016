datablock fxDTSBrickData (BrickHorseBot_HoleSpawnData)
{
	brickFile = "Add-ons/Bot_Hole/6xSpawn.blb";
	category = "Special";
	subCategory = "Holes";
	uiName = "Horse Hole";
	iconName = "Add-Ons/Bot_Horse/icon_horse";

	bricktype = 2;
	cancover = 0;
	orientationfix = 1;
	indestructable = 1;

	isBotHole = 1;
	holeBot = "HorseHoleBot";
};

datablock PlayerData(HorseHoleBot : HorseArmor)
{
	uiName = "";
	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;
	maxItems   = 0;
	maxWeapons = 0;
	maxTools = 0;
	//runforce = 100 * 90;
	//maxForwardSpeed = 8;
	//maxBackwardSpeed = 4;
	//maxSideSpeed = 8;
	//attackpower = 10;
	rideable = true;
	canRide = false;

	maxdamage = 250;//Bot Health
	jumpSound = "";//Removed due to bots jumping a lot
	
	//Hole Attributes
	isHoleBot = 1;

	//Spawning option
	hSpawnTooClose = 0;//Doesn't spawn when player is too close and can see it
	  hSpawnTCRange = 8;//above range, set in brick units
	hSpawnClose = 0;//Only spawn when close to a player, can be used with above function as long as hSCRange is higher than hSpawnTCRange
	  hSpawnCRange = 32;//above range, set in brick units

	hType = Neutral; //Enemy,Friendly, Neutral
	  hNeutralAttackChance = 0;
	//can have unique types, nazis will attack zombies but nazis will not attack other bots labeled nazi
	hName = "Horse";//cannot contain spaces
	hTickRate = 3000;
	
	//Wander Options
	hWander = 1;//Enables random walking
	  hSmoothWander = 1;//This is in addition to regular wander, makes them walk a bit longer, and a bit smoother
	  hReturnToSpawn = 0;//Returns to spawn when too far
	  hSpawnDist = 48;//Defines the distance bot can travel away from spawnbrick
	  hGridWander = 0;//Locks the bot to a grid, overwrites other settings
	
	//Searching options
	hSearch = 0;//Search for Players
	  hSearchRadius = 64;//in brick units
	  hSight = 1;//Require bot to see player before pursuing
	  hStrafe = 1;//Randomly strafe while following player
	hSearchFOV = 0;//if enabled disables normal hSearch
	  hFOVRadius = 6;//max 10
	  hHearing = 1;//If it hears a player it'll look in the direction of the sound

	  hAlertOtherBots = 1;//Alerts other bots when he sees a player, or gets attacked

	//Attack Options
	hMelee = 0;//Melee
	  hAttackDamage = 15;//Melee Damage
	hShoot = 0;
	  hWep = "gunImage";
	  hShootTimes = 4;//Number of times the bot will shoot between each tick
	  hMaxShootRange = 256;//The range in which the bot will shoot the player
	  hAvoidCloseRange = 1;//
		hTooCloseRange = 7;//in brick units

	//Misc options
	hAvoidObstacles = 1;
	hSuperStacker = 0;//When enabled makes the bots stack a bit better, in other words, jumping on each others heads to get to a player
	hSpazJump = 0;//Makes bot jump when the user their following is higher than them

	hAFKOmeter = 1;//Determines how often the bot will wander or do other idle actions, higher it is the less often he does things

	hIdle = 1;// Enables use of idle actions, actions which are done when the bot is not doing anything else
	  hIdleAnimation = 0;//Plays random animations/emotes, sit, click, love/hate/etc
	  hIdleLookAtOthers = 1;//Randomly looks at other players/bots when not doing anything else
	    hIdleSpam = 0;//Makes them spam click and spam hammer/spraycan
	  hSpasticLook = 1;//Makes them look around their environment a bit more.
	hEmote = 1;
};

function HorseHoleBot::onAdd(%this,%obj)
{
	armor::onAdd(%this,%obj);
	
	%color = getRandom(0,3);
	if(%color == 0)
		%obj.chestColor = "1 1 1 1";
	if(%color == 1)
		%obj.chestColor = "0.1 0.1 0.1 1";
	if(%color == 2)
		%obj.chestColor = "0.5 0.27 0.05 1";
	if(%color == 3)
		%obj.chestColor = "0.98 0.86 0.67 1";

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
	
	// allow people to take control of the bot when they mount it
	%obj.controlOnMount = 1;
}

function HorseHoleBot::onBotLoop(%this,%obj)
{
	//Called every cycle
	//Useful for doing unique behaviors during normal loop
}

function HorseHoleBot::onBotCollision( %this, %obj, %col, %normal, %speed )
{
	//Called once every second the object is colliding with something
	//echo(%obj.isStampeding SPC %col.isHoleBot SPC %obj.getState() !$= "Dead" SPC %col.getState() !$= "Dead" SPC !%col.isStampeding SPC %col.getDataBlock().getName() $= "HorseHoleBot");
	if(%obj.isStampeding && %col.isHoleBot && %obj.getState() !$= "Dead" && %col.getState() !$= "Dead" && !%col.isStampeding && %col.getDataBlock().getName() $= "HorseHoleBot")
	{
		//echo("Spreading stampede");
		%col.emote("AlarmProjectile");
		%col.stopHoleLoop();
		hDoStampede(%col);
		scheduleNoQuota(2000+getRandom(-750,750),%col,hDoStampede,%col);
		scheduleNoQuota(4000+getRandom(-750,750),%col,hDoStampede,%col);
		scheduleNoQuota(6000,%col,hStopStampede,%col);
	}
}

function HorseHoleBot::onBotFollow(%this,%obj,%targ)
{
	//Called when the target follows a player each tick, or is running away
}

function HorseHoleBot::onBotDamage(%this,%obj,%source,%pos,%damage,%type)
{
	//Called when the bot is being damaged
	if(!%obj.isStampeding && %obj.getState() !$= "Dead" && !%obj.hMelee)
	{
		%obj.stopHoleLoop();
		%obj.emote("AlarmProjectile");
		hDoStampede(%obj);
		scheduleNoQuota(2000+getRandom(-750,750),%obj,hDoStampede,%obj);
		scheduleNoQuota(4000+getRandom(-750,750),%obj,hDoStampede,%obj);
		scheduleNoQuota(6000,%obj,hStopStampede,%obj);
	}
	// %driver = %obj.getMountedObject(0);
	// if( isObject(%driver) && %obj.getDamagePercent() >= 0.5)
	// {
	// if( !getRandom( 0, 1 ) )
	if( %obj.getDamagePercent() >= 0.5 )
		%obj.scheduleNoQuota( 200, ejectRandomPlayer );
	// }
}

function hDoStampede(%obj)
{
	if(%obj.hMelee)
		return;
	
	// %obj.setImageTrigger(2,1);
	%obj.setJumping(1);
	//%obj.setImageTrigger(2,0);

	%obj.isStampeding = 1;

	%x = hGetRandomFloat(0,10,1);

	%y = hGetRandomFloat(0,10,1);
	
	%z = hGetRandomFloat(0,3,1);

	%vec = %x SPC %y SPC %z;
	// hSetAimVector(%obj,%vec);
	%obj.setAimVector( %vec );

	%obj.setMoveY(1);//hGetRandomFloat(6,10,0));

	%obj.hAvoidObstacle();
	%obj.hDetectWall(1);
	// hAvoidObstacle(%obj);
	// hWanderSearchWall(%obj,1);

	//schedule(3000,%obj,hAvoidObstacle,%obj,1);
	
	//%obj.schedule(7000,startHoleLoop);
}

function hStopStampede(%obj)
{
	%obj.startHoleLoop();
	%obj.isStampeding = 0;
}