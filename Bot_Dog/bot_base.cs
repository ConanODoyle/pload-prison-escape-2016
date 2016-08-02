datablock fxDTSBrickData (BrickShepherdDogBot_HoleSpawnData)
{
	brickFile = "Add-ons/Bot_Hole/4xSpawn.blb";
	category = "Special";
	subCategory = "Holes";
	uiName = "Shepherd Dog Hole";
	iconName = "Add-Ons/Bot_ShepherdDog/icon_ShepherdDog";

	bricktype = 2;
	cancover = 0;
	orientationfix = 1;
	indestructable = 1;

	isBotHole = 1;
	holeBot = "ShepherdDogHoleBot";
};

datablock PlayerData(ShepherdDogHoleBot : ShepherdDogArmor)
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
	rideable = false;
	canRide = true;

	maxdamage = 150;//Bot Health
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
	hName = "ShepherdDog";//cannot contain spaces
	hTickRate = 3000;
	
	//Wander Options
	hWander = 1;//Enables random walking
	  hSmoothWander = 1;//This is in addition to regular wander, makes them walk a bit longer, and a bit smoother
	  hReturnToSpawn = 0;//Returns to spawn when too far
	  hSpawnDist = 64;//Defines the distance bot can travel away from spawnbrick
	  hGridWander = 0;//Locks the bot to a grid, overwrites other settings
	
	//Searching options
	hSearch = 1;//Search for Players
	  hSearchRadius = 64;//in brick units
	  hSight = 1;//Require bot to see player before pursuing
	  hStrafe = 0;//Randomly strafe while following player
	hSearchFOV = 1;//if enabled disables normal hSearch
	  hFOVRadius = 8;//max 10
	  hHearing = 1;//If it hears a player it'll look in the direction of the sound

	  hAlertOtherBots = 1;//Alerts other bots when he sees a player, or gets attacked

	//Attack Options
	hMelee = 1;//Melee
	  hAttackDamage = 20;//Melee Damage
	hShoot = 0;
	  hWep = "";
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
	  hIdleAnimation = 1;//Plays random animations/emotes, sit, click, love/hate/etc
	  hIdleLookAtOthers = 1;//Randomly looks at other players/bots when not doing anything else
	    hIdleSpam = 0;//Makes them spam click and spam hammer/spraycan
	  hSpasticLook = 1;//Makes them look around their environment a bit more.
	hEmote = 1;
};

function ShepherdDogHoleBot::onAdd(%this,%obj)
{
	armor::onAdd(%this,%obj);
	
	%color = getRandom(0,3);
	if(%color == 0)
		%obj.chestColor = "1 1 1 1";
	if(%color == 1)
		%obj.chestColor = "0.6 0.6 0.7 1";
	if(%color == 2)
		%obj.chestColor = "0.5 0.27 0.05 1";
	if(%color == 3)
		%obj.chestColor = "0.98 0.86 0.67 1";

	%obj.canBark = 1;

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
	
	// allow people to take control of the bot when they mount it
	//%obj.controlOnMount = 1;
}


datablock AudioProfile(ShepherdDogDeath1Sound)
{
   filename    = "./death1.wav";
   description = AudioDefault3d;
   preload = true;
};

datablock AudioProfile(ShepherdDogDeath2Sound)
{
   filename    = "./death2.wav";
   description = AudioDefault3d;
   preload = true;
};

datablock AudioProfile(ShepherdDogBark1Sound)
{
   filename    = "./bark1.wav";
   description = AudioDefault3d;
   preload = true;
};

datablock AudioProfile(ShepherdDogBark2Sound)
{
   filename    = "./bark2.wav";
   description = AudioDefault3d;
   preload = true;
};

datablock AudioProfile(ShepherdDogBark3Sound)
{
   filename    = "./bark3.wav";
   description = AudioDefault3d;
   preload = true;
};


function ShepherdDogHoleBot::onBotLoop(%this,%obj)
{
	if (%obj.getState() $= "Dead")
		return;

	if (%obj.canBark && (getRandom(1, 10) == 1 || (%obj.hIsFollowing && %obj.hIsFollowing != %obj.owner)))
	{
		serverPlay3D("ShepherdDogBark" @ getRandom(1, 3) @ "Sound", %obj.getPosition());
		%obj.canBark = 0;
		schedule(getRandom(1000, 4000), 0, eval, %obj.getID() @ ".canBark = 1;");
	}

	if (isObject(%obj.hFollowing))
	{
		%obj.setAimObject(%obj.hFollowing);
	}

	//follow its owner if its not targeting anyone else
	if (isObject(%obj.owner) && !%obj.hFollowing)
	{
		%obj.hFollowPlayer(%obj.owner, 1, 1);
		%obj.emote(loveImage);
		%obj.lastEmote = getSimTime();
	}
	//look for a steak if its not targeting anyone
	if (!%obj.hFollowing)
	{
		%closestSteak = 0;
		%distance = 10000;
		%pos = %obj.getEyePoint();
		%typeMasks = $Typemasks::FxBrickObjectType | $Typemasks::TerrainObjectType | 
			$TypeMasks::StaticObjectType;

		//iterate through backwards in case a steak gets deleted while searching for one
		for (%i = $SteakGroup.numSteaks-1; %i >= 0; %i--)
		{
			echo("Looking for steaks: steakCount " @ $SteakGroup.numSteaks);
			%steak = $SteakGroup.steak[%i];
			echo("Steak " @ %steak);
			if (!isObject(%steak))
			{
				echo("    A steak that doesn't exist");
				$SteakGroup.removeSteak(%steak);
				continue;
			}

			%steakPos = vectorAdd(%steak.getPosition(), "0 0 0.2");
			%distanceToSteak = vectorDist(%pos, %steakPos);
			//if distance > 2 check for line of sight
			if (%distanceToSteak > 2)
			{
				echo("    A steak far away - checking for LOS");
				%ray = containerRaycast(%pos, %steakPos, %typemasks);
				if (!isObject(getWord(%ray, 0)))
				{
					if (%distanceToSteak < %distance)
					{
						%distance = %distanceToSteak;
						%closestSteak = %steak;
					}
				}
			}
			else
			{
				echo("    A steak very close");
				if (%distanceToSteak < %distance)
				{
					%distance = %distanceToSteak;
					%closestSteak = %steak;
				}
			}
		}

		//target the steak
		if (isObject(%closestSteak))
		{
			echo("Pathing to steak " @ %closestSteak);
			%obj.setMoveDestination(%closestSteak.getPosition());
			%obj.setAimObject(%closestSteak);
		}
	}
}

function ShepherdDogHoleBot::onBotCollision( %this, %obj, %col, %normal, %speed )
{
	if (%obj.getState() $= "Dead")
		return;

	if (%obj.canBark)
	{
		serverPlay3D("ShepherdDogBark" @ getRandom(1, 3) @ "Sound", %obj.getPosition());
		%obj.canBark = 0;
		schedule(getRandom(500, 1000), 0, eval, %obj.getID() @ ".canBark = 1;");
	}
}

function ShepherdDogHoleBot::onBotFollow(%this,%obj,%targ)
{
	//Called when the target follows a player each tick, or is running away
	%pos = %obj.getPosition();
	%targpos = %targ.getPosition();
	%xypos = getWord(%pos, 0) SPC getWord(%pos, 1) SPC 0;
	%xytargpos = getWord(%targpos, 0) SPC getWord(%targpos, 1) SPC 0;

	%xydist = vectorDist(%xypos, %xytargpos);
	%zdist = getWord(%targpos, 2) - getWord(%pos, 2);

	//bark at players too high for them to reach, or emote love if their target is their owner
	if (%zdist > 2.4 && %xydist < 2 && !%obj.hIsRunning)
		if (%targ.getID() == %obj.owner && %obj.lastEmote < getSimTime()-5000)
		{
			%obj.emote(loveImage);
			%obj.lastEmote = getSimTime();
		}
		else if (%obj.canBark)
		{
			serverPlay3D("ShepherdDogBark" @ getRandom(1, 3) @ "Sound", %obj.getPosition());
			%obj.canBark = 0;
			schedule(getRandom(500, 1000), 0, eval, %obj.getID() @ ".canBark = 1;");
		}
}

function ShepherdDogHoleBot::onBotDamage(%this,%obj,%source,%pos,%damage,%type)
{
	//Called when the bot is being damaged
	if (%obj.isEating)
	{
		DogStopEatSteak(%obj);
		%obj.setAimObject(%source);
	}
}

package BotHole_Dogs
{
	function ShepherdDogHoleBot::onDisabled(%this, %obj, %state) //makes bots have death sound and animation and runs the required bot hole command
	{
		if (!%obj.getState() $= "Dead")
			return;

		holeBotDisabled(%obj);
		serverPlay3D("ShepherdDogDeath" @ getRandom(1, 2) @ "Sound", %obj.getPosition());
		%obj.playThread(2, "death1");
	}

	function ShepherdDogArmor::onDisabled(%this, %obj, %state) //for players; slightly different behavior
	{
		if (!%obj.getState() $= "Dead")
			return;

		holeBotDisabled(%obj);
		serverPlay3D("ShepherdDogDeath" @ getRandom(1, 2) @ "Sound", %obj.getPosition());
		%obj.playThread(2, "death1");

		parent::onDisabled(%this, %obj, %state);
	}

	function AIPlayer::hMeleeAttack(%obj, %col)
	{
		if (%obj.getDatablock().getName() !$= "ShepherdDogHoleBot")
			return parent::hMeleeAttack(%obj, %col);

		if (!isObject(%col))
			return parent::hMeleeAttack(%obj, %col);

		if (%col.getType() & $TypeMasks::PlayerObjectType)
		{
			if (%col.isTumbling)
				return;
			if (!parent::hMeleeAttack(%obj, %col))
				return;

			%col.setVelocity(vectorAdd(%col.getVelocity(), "0 0 5"));
			tumble(%col, 40);
			schedule(2000, 0, clearTumble, %col);
			%col.isTumbling = true;
		}
	}

	function Observer::onTrigger(%this, %obj, %triggerNum, %val)
	{
		%player = %obj.getControllingClient().player;
		if (isObject(%player) && %player.isTumbling)
			return;
		parent::onTrigger(%this, %obj, %triggerNum, %val);
	}

	function Player::playDeathCry(%this)
	{
		//this is for players
		if (%this.getDatablock().getName() $= "ShepherdDogArmor")
			return;
		parent::playDeathCry(%this);
	}

	function ShepherdDogArmor::onTrigger(%this, %player, %slot, %val)
	{
		if (isObject(%player.client) && %player.getDatablock().getName() $= "ShepherdDogArmor" && %slot == 0 && %val)
		{
			serverPlay3D("ShepherdDogBark" @ getRandom(1, 3) @ "Sound", %player.getPosition());
		}
		parent::onTrigger(%this, %player, %slot, %val);
	}

	function ShepherdDogHoleBot::hSpazzClick(%obj, %amount, %panic)
	{
		//dont want the dog to spazzclick
		return;
	}
};
activatePackage(BotHole_Dogs);

function clearTumble(%player)
{
	echo("clearing tumble");

	if(isObject(%player))
	{
		%player.canDismount = true;
		%player.stopSkiing();
		%player.dismount();
		%player.isTumbling = false;
		%player.client.setControlObject(%player);
	}
	%obj.unmountobject(%player);
	%obj.schedule(0, delete);
}

function DogEatSteak(%obj, %health, %eatTime)
{
	//talk("Dog starting to eat for " @ %eatTime);
	//make it hold the steak
	%obj.mountImage(SteakDogImage, 0);
	%obj.playThread(0, sit);

	//stop bot hole activity
	%obj.stopHoleLoop();

	%obj.isEating = 1;

	%x = hGetRandomFloat(0,10,1);

	%y = hGetRandomFloat(0,10,1);
	
	%z = hGetRandomFloat(0,3,1);

	%vec = %x SPC %y SPC %z;
	%obj.setAimVector( %vec );

	%obj.emote(loveImage);
	%obj.eatEmitter = new ParticleEmitterNode()
	{
		dataBlock = GenericEmitterNode;
		emitter = healCrossEmitter;
		scale = "0 0 0";
	};
	%obj.eatEmitter.setTransform(%obj.getPosition());
	MissionCleanup.add(%obj.eatEmitter);

	%obj.newHealth = %obj.getDamageLevel() - %health;
	%obj.eatSteakSchedule = schedule(%eatTime, 0, DogStopEatSteak, %obj);
	schedule(%eatTime, 0, eval, %obj @ ".finishedSteak = 1;");
}

function DogStopEatSteak(%obj)
{
	//talk("Dog finished eating");
	//remove the image from it
	%obj.unMountImage(0);
	%obj.playThread(0, root);
	%obj.startHoleLoop();
	%obj.isEating = 0;

	%obj.eatEmitter.delete();

	//if dog was interrupted while eating, drop the steak
	if (isEventPending(%obj.eatSteakSchedule))
	{
		%pos = vectorSub(vectorAdd(%obj.getPosition(), "0 0 3"), %obj.getEyeVector());
		%velocity = vectorAdd(vectorScale(%obj.getEyeVector(), -1), "0 0 4");

		%i = new Item()
		{
			minigame = %obj.client.minigame;
			datablock = SteakItem;
			canPickup = true;
			rotate = false;
			timeToFinish = getTimeRemaining(%obj.eatSteakSchedule);

			position = %pos;
		};
		MissionCleanup.add(%i);
		%i.schedule(30000 - 500, fadeOut);
		%i.schedule(30000, delete);
		$SteakGroup.schedule(30000, removeSteak, %i);
		%i.setVelocity(%velocity);
		
		$SteakGroup.addSteak(%i);

		cancel(%obj.eatSteakSchedule);
	}
	else
	{
		//statistics
		$Server::PrisonEscape::SteaksEaten++;

		%obj.setDamageLevel(%obj.newHealth);
	}
}