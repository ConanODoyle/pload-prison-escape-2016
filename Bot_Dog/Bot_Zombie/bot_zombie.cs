datablock fxDTSBrickData (BrickZombie_HoleSpawnData)
{
	brickFile = "Add-ons/Bot_Hole/4xSpawn.blb";
	category = "Special";
	subCategory = "Holes";
	uiName = "Zombie Hole";
	iconName = "Add-Ons/Bot_Zombie/icon_zombie";

	bricktype = 2;
	cancover = 0;
	orientationfix = 1;
	indestructable = 1;

	isBotHole = 1;
	holeBot = "ZombieHoleBot";
};

datablock PlayerData(ZombieHoleBot : PlayerStandardArmor)
{
	uiName = "";
	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;
	maxItems   = 0;
	maxWeapons = 0;
	maxTools = 0;
	runforce = 100 * 90;
	maxForwardSpeed = 8;
	maxBackwardSpeed = 4;
	maxSideSpeed = 8;
	attackpower = 10;
	rideable = false;
	canRide = true;
	maxdamage = 50;//Health
	jumpSound = "";
	
	//Hole Attributes
	isHoleBot = 1;

	//Spawning option
	hSpawnTooClose = 0;//Doesn't spawn when player is too close and can see it
	  hSpawnTCRange = 8;//above range, set in brick units
	hSpawnClose = 0;//Only spawn when close to a player, can be used with above function as long as hSCRange is higher than hSpawnTCRange
	  hSpawnCRange = 64;//above range, set in brick units

	hType = zombie; //Enemy,Friendly, Neutral
	  hNeutralAttackChance = 100;
	//can have unique types, nazis will attack zombies but nazis will not attack other bots labeled nazi
	hName = "Zombie";//cannot contain spaces
	hTickRate = 3000;
	
	//Wander Options
	hWander = 1;//Enables random walking
	  hSmoothWander = 1;
	  hReturnToSpawn = 1;//Returns to spawn when too far
	  hSpawnDist = 32;//Defines the distance bot can travel away from spawnbrick
	
	//Searching options
	hSearch = 1;//Search for Players
	  hSearchRadius = 256;//in brick units
	  hSight = 1;//Require bot to see player before pursuing
	  hStrafe = 1;//Randomly strafe while following player
	hSearchFOV = 1;//if enabled disables normal hSearch
	  hFOVRadius = 32;//max 10

	  hAlertOtherBots = 1;//Alerts other bots when he sees a player, or gets attacked

	//Attack Options
	hMelee = 1;//Melee
	  hAttackDamage = 15;//15;//Melee Damage
	hShoot = 0;
	  hWep = "gunImage";
	  hShootTimes = 4;//Number of times the bot will shoot between each tick
	  hMaxShootRange = 30;//The range in which the bot will shoot the player
	  hAvoidCloseRange = 1;//
		hTooCloseRange = 7;//in brick units
	//hHerding = 0;
	//hSound = 1;
	//hSpawnDetect = -1;//Will not spawn when user is too close and can see spawn
	

	
	//Misc options
	hAvoidObstacles = 1;
	hSuperStacker = 1;
	hSpazJump = 0;//Makes bot jump when the user their following is higher than them

	hAFKOmeter = 1;//Determines how often the bot will wander or do other idle actions, higher it is the less often he does things
	hIdle = 1;// Enables use of idle actions, actions which are done when the bot is not doing anything else
	  hIdleAnimation = 1;//Plays random animations/emotes, sit, click, love/hate/etc
	  hIdleLookAtOthers = 1;//Randomly looks at other players/bots when not doing anything else
	    hIdleSpam = 0;//Makes them spam click and spam hammer/spraycan
	  hSpasticLook = 1;//Makes them look around their environment a bit more.
	hEmote = 1;
};

function ZombieHoleBot::onAdd(%this,%obj)
{
	armor::onAdd(%this,%obj);
	%obj.playthread(1,"ArmReadyBoth");
	%obj.hDefaultThread = "ArmReadyBoth";
	//Appearance zombie
	%obj.llegColor =  "0 0.141 0.333 1";
	%obj.secondPackColor =  "0 0.435 0.831 1";
	%obj.lhand =  "0";
	%obj.hip =  "0";
	%obj.faceName =  "asciiTerror";
	%obj.rarmColor =  "0.593 0 0 1";
	%obj.hatColor =  "1 1 1 1";
	%obj.hipColor =  "0 0.141 0.333 1";
	%obj.chest =  "0";
	%obj.rarm =  "0";
	%obj.packColor =  "0.2 0 0.8 1";
	%obj.pack =  "0";
	%obj.decalName =  "AAA-None";
	%obj.larmColor =  "0.593 0 0 1";
	%obj.secondPack =  "0";
	%obj.larm =  "0";
	%obj.chestColor =  "0.75 0.75 0.75 1";
	%obj.accentColor =  "0.990 0.960 0 0.700";
	%obj.rhandColor =  "0.626 0.71 0.453 1";
	%obj.rleg =  "0";
	%obj.rlegColor =  "0 0.141 0.333 1";
	%obj.accent =  "1";
	%obj.headColor =  "0.626 0.71 0.453 1";
	%obj.rhand =  "0";
	%obj.lleg =  "0";
	%obj.lhandColor =  "0.626 0.71 0.453 1";
	%obj.hat =  "0";

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
	
	%obj.hIsInfected = 1;
}

function ZombieHoleBot::onBotLoop(%this,%obj)
{
	//BreakBrickTest(%obj);
	//schedule(%obj.getDatablock().hTickRate/2,0,BreakBrickTest,%obj);
}

function ZombieHoleBot::onBotFollow( %this, %obj, %targ )
{
	%pos1 = %obj.getPosition();
	%pos2 = %targ.getPosition();
	
	// echo( "Enaged @" SPC vectorDist( %pos1, %pos2 )*2 );
	// %obj.kill();
}

function ZombieHoleBot::onBotCollision( %this, %obj, %col, %normal, %speed )
{
	//Called once every second the object is colliding with something
	//Turn the enemy bot into a zombie if his health is low
	// return;
}

//If you do some neat things with your bots/add-ons try to keep clean functions so that others can interface with it properly
//This function is broken off from the main group of code so that it can be called whenever i want to turn a bot into a zombie
function holeZombieInfect(%obj,%col)
{
	//Appearance zombie
	if(%col.getDataBlock().shapeFile $= "base/data/shapes/player/m.dts")
	{
		%col.setDataBlock(ZombieHoleBot);
	}
	else if(%col.getDataBlock().shapeFile $= "Add-Ons/Bot_Shark/shark.dts")
	{
		%doNotApply = 1;
		%col.setnodecolor("chest", %obj.headColor);
		%col.setnodecolor("head", %obj.headColor);
	}
	else
	{
		%doNotApply = 1;
		%col.setnodecolor("ALL", %obj.headColor);
	}

	%sc = %obj.headColor;
	for(%a = 0; %a <= $aCL; %a++)
	{
		%cur = $avatarColorLoop[%a];
		//eval("%objC = %obj." @ %cur @ ";");
		eval("%colC = %col." @ %cur @ ";");
		if(%colC $= %col.headColor)
			eval("%col." @ %cur @ "= %sc;");
	}

	if(%col.faceName !$= "memePBear")
		%col.faceName =  %obj.faceName;

	//%col.headColor =  %obj.headColor;

	//if(%col.rhand == 0 && %col.rHandColor $= %obj.headColor)
	//	%col.rhandColor = %obj.rhandColor;

	//if(%col.lhand == 0 && %col.lHandColor $= %obj.headColor)
	//	%col.lhandColor = %obj.lhandColor;
	
	if(!%doNotApply)
	{
		GameConnection::ApplyBodyParts(%col);
		GameConnection::ApplyBodyColors(%col);
	}

	%col.setHealth(%col.getDataBlock().maxdamage);

	//Change his team
	if(getWord(%col.name,0) !$= "Zombie")
		%col.name = "Zombie" SPC %col.name;


	%col.setWeapon(-1);
	%col.playthread(1,"ArmReadyBoth");
	%col.hDefaultThread = "ArmReadyBoth";
	
	if(%col.hAttackDamage > %obj.hAttackDamage)
		%col.setMeleeDamage(%col.hAttackDamage);
	else
		%col.setMeleeDamage(%obj.hAttackDamage);
	
	%col.hNeutralAttackChance = %obj.hNeutralAttackChance;
	%col.hType = %obj.hType;
	%col.hSearch = %obj.hSearch;
	%col.hSearchRadius = %obj.hSearchRadius;
	%col.hSight = %obj.hSight;
	%col.hSearchFov = %obj.hSearchFov;
	%col.hSuperStacker = %obj.hSuperStacker;

	%obj.resetHoleLoop();
	%col.resetHoleLoop();
	

	//%col.spawnExplosion(horseRayProjectile,0.5);
	//Also if whatever you're doing happens to be a state that something can be in do something like this
	%col.hIsInfected = 1;
	//A good example of this is the invisibility watches, they add the tag %obj.isInvisible to the players when they're invisible, which allows the bots to interface with them nicely
}

// this requires a package since it needs to affect every bot not just the zombie bot
package holeZombiePackage
{
	function armor::onCollision(%this,%obj,%col,%a,%b,%c,%d)
	{
		if( (%obj.hIsInfected || %obj.getDataBlock().getName() $= "ZombieHoleBot") && %obj.isHoleBot && %obj.getClassname() $= "AIPlayer" && getSimTime() > %obj.lastattacked)
		{	
			if(%col.isBot && checkHoleBotTeams(%obj,%col))
			{
				if( %col.getDamagePercent() >= 0.5 && %obj.getState() !$= "Dead" && %col.getState() !$= "Dead" )
				{
					if(%col.getDataBLock().getName() $= "ZombieHoleBot" && getRandom(0,1) || %col.zMarkedForDeath)
					{
						%col.zMarkedForDeath = 1;
						return;
					}

					holeZombieInfect(%obj,%col);
				}
				//%obj.lastattacked = getsimtime()+1000;
				//return;
			}
		}
		parent::onCollision(%this,%obj,%col,%a,%b,%c,%d);
	}
};

activatePackage(holeZombiePackage);