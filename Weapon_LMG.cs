//audio
datablock AudioProfile(LightMachinegunFire1Sound)
{
   filename    = "./LMG_fire.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(LightMachinegunClickSound)
{
   filename    = "./LMG_clickofdeath.wav";
   description = AudioClose3d;
   preload = true;
};

AddDamageType("LMG",   '<bitmap:add-ons/Weapon_Package_Tier2/ci_lmg1> %1',    '%2 <bitmap:add-ons/Weapon_Package_Tier2/ci_lmg1> %1',0.75,1);
datablock ProjectileData(LightMachinegunProjectile)
{
   projectileShapeName = "add-ons/Weapon_Gun/bullet.dts";
   directDamage        = 22;
   directDamageType    = $DamageType::LMG;
   radiusDamageType    = $DamageType::LMG;

   brickExplosionRadius = 0;
   brickExplosionImpact = true;          //destroy a brick if we hit it directly?
   brickExplosionForce  = 0;
   brickExplosionMaxVolume = 0;          //max volume of bricks that we can destroy
   brickExplosionMaxVolumeFloating = 0;  //max volume of bricks that we can destroy if they aren't connected to the ground

   impactImpulse	     = 0;
   verticalImpulse     = 20;
   explosion           = gunExplosion;

   muzzleVelocity      = 200;
   velInheritFactor    = 1;

   armingDelay         = 0;
   lifetime            = 4000;
   fadeDelay           = 3500;
   bounceElasticity    = 0.0;
   bounceFriction      = 0.0;
   isBallistic         = true;
   gravityMod = 0.2;
   explodeOnDeath = true;
   explodeOnPlayerImpact = false;

   hasLight    = false;
   lightRadius = 3.0;
   lightColor  = "0 0 0.5";
};

//////////
// item //
//////////
datablock ItemData(LightMachinegunItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./LMG.2.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Light MG";
	iconName = "./lightmg";
	doColorShift = true;
	colorShiftColor = "0.4 0.4 0.42 1.000";

	 // Dynamic properties defined by the scripts
	image = LightMachinegunImage;
	canDrop = true;
	
	maxAmmo = 70;
	canReload = 0;
};

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(LightMachinegunImage)
{
   // Basic Item properties
   shapeFile = "./LMG.2.dts";
   emap = true;

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = 0; //"0.7 1.2 -0.5";
   rotation = eulerToMatrix( "0 0 0" );

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.  
   correctMuzzleVector = true;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   className = "WeaponImage";

   // Projectile && Ammo.
   item = LightMachinegunItem;
   ammo = " ";
   projectile = LightMachinegunProjectile;
   projectileType = Projectile;

   casing = GunShellDebris;
   shellExitDir        = "1.0 0.1 1.0";
   shellExitOffset     = "0 0 0";
   shellExitVariance   = 10.0;	
   shellVelocity       = 5.0;

   //melee particles shoot from eye node for consistancy
   melee = false;
   //raise your arm up or not
   armReady = true;

   doColorShift = true;
   colorShiftColor = LightMachinegunItem.colorShiftColor;

   // Images have a state system which controls how the animations
   // are run, which sounds are played, script callbacks, etc. This
   // state system is downloaded to the client so that clients can
   // predict state changes and animate accordingly.  The following
   // system supports basic ready->fire->reload transitions as
   // well as a no-ammo->dryfire idle state.

   // Initial start up state
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.05;
	stateTransitionOnTimeout[0]       = "LoadCheckA";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]                     = "Ready";
	stateTransitionOnNoAmmo[1]       = "LoadCheckA";
	stateTransitionOnTriggerDown[1]  = "Click";
	stateTransitionOnTimeout[1]      = "LoadCheckA";
	stateTimeoutValue[1]             = 0.3;
	stateWaitForTimeout[1]           = 0;
	stateScript[1]                   = "onReady";
	stateAllowImageChange[1]         = true;

	stateName[2]                    = "Fire";
	stateTransitionOnTimeout[2]     = "Delay";
	stateTimeoutValue[2]            = 0.01;
	stateSound[2]				= LightMachinegunfire1Sound;
	stateFire[2]                    = true;
	stateAllowImageChange[2]        = false;
	stateSequence[2]                = "Fire";
	stateScript[2]                  = "onFire";
	stateEjectShell[2]       	  = true;
	stateEmitter[2]					= gunFlashEmitter;
	stateEmitterTime[2]				= 0.05;
	stateEmitterNode[2]				= "muzzleNode";
	stateWaitForTimeout[2]			= true;

	stateName[3]			= "Delay";
	stateTransitionOnTimeout[3]     = "FireLoadCheckA";
	stateTimeoutValue[3]            = 0.01;
	
	stateName[4]				= "LoadCheckA";
	stateScript[4]				= "onLoadCheck";
	stateTimeoutValue[4]			= 0.01;
	stateTransitionOnTimeout[4]		= "LoadCheckB";
	
	stateName[5]				= "LoadCheckB";
	stateTransitionOnAmmo[5]		= "Ready";
	stateTransitionOnNoAmmo[5]		= "Reload";

	stateName[6]				= "Reload";
	stateTimeoutValue[6]			= 0.1;
	stateScript[6]				= "onReloadStart";
	stateTransitionOnTimeout[6]		= "Wait";
	stateWaitForTimeout[6]			= true;
	
	stateName[7]				= "Wait";
	stateTimeoutValue[7]			= 0.1;
	stateScript[7]				= "onReloadWait";
	stateTransitionOnTimeout[7]		= "Reloaded";
	
	stateName[8]				= "FireLoadCheckA";
	stateScript[8]				= "onLoadCheck";
	stateTimeoutValue[8]			= 0.01;
	stateTransitionOnTimeout[8]		= "FireLoadCheckB";
	
	stateName[9]				= "FireLoadCheckB";
	stateTransitionOnAmmo[9]		= "Smoke";
	stateTransitionOnNoAmmo[9]		= "ReloadSmoke";
	
	stateName[10] 				= "Smoke";
	stateEmitter[10]			= gunSmokeEmitter;
	stateEmitterTime[10]			= 0.3;
	stateEmitterNode[10]			= "muzzleNode";
	stateTimeoutValue[10]			= 0.2;
	stateTransitionOnTimeout[10]		= "Halt";
	stateTransitionOnTriggerDown[10]	= "Fire";
	
	stateName[11] 				= "ReloadSmoke";
	stateEmitter[11]			= gunSmokeEmitter;
	stateEmitterTime[11]			= 0.3;
	stateEmitterNode[11]			= "muzzleNode";
	stateTimeoutValue[11]			= 0.2;
	stateTransitionOnTimeout[11]		= "Reload";
	
	stateName[12]				= "Reloaded";
	stateTimeoutValue[12]			= 0.04;
	stateScript[12]				= "onReloaded";
	stateTransitionOnTimeout[12]		= "Ready";

	stateName[13]			= "Halt";
	stateTransitionOnTimeout[13]     = "Ready";
	stateTimeoutValue[13]            = 0.1;
	stateEmitter[13]					= gunSmokeEmitter;
	stateEmitterTime[13]				= 0.48;
	stateEmitterNode[13]				= "muzzleNode";
	stateScript[13]                  = "onHalt";

	stateName[14]                     = "Click";
	stateTransitionOnTimeout[14]      = "Fire";
	stateTimeoutValue[14]             = 0.2;
	stateWaitForTimeout[14]           = 1;
	stateScript[14]                   = "onClick";
	stateAllowImageChange[14]         = true;
	stateSound[14]				= LightMachinegunClickSound;

	// stateName[15]                    = "Fire2";
	// stateTransitionOnTimeout[15]     = "Delay";
	// stateTimeoutValue[15]            = 0.03;
	// stateSound[15]				= LightMachinegunfire1Sound;
	// stateFire[15]                    = true;
	// stateAllowImageChange[15]        = false;
	// stateSequence[15]                = "Fire";
	// stateScript[15]                  = "onFire2";
	// stateEjectShell[15]       	  = true;
	// stateEmitter[15]					= gunFlashEmitter;
	// stateEmitterTime[15]				= 0.05;
	// stateEmitterNode[15]				= "muzzleNode";
	// stateWaitForTimeout[15]			= true;

};

function LightMachinegunImage::onFire(%this,%obj,%slot)
{ 
	%fX = getWord(%fvec,0);
	%fY = getWord(%fvec,1);
	
	%evec = %obj.getEyeVector();
	%eX = getWord(%evec,0);
	%eY = getWord(%evec,1);
	%eZ = getWord(%evec,2);
	
	%eXY = mSqrt(%eX*%eX+%eY*%eY);
	
	%aimVec = %fX*%eXY SPC %fY*%eXY SPC %eZ;

	%obj.setVelocity(VectorAdd(%obj.getVelocity(),VectorScale(%aimVec,"-1")));
	
	%obj.lastShotTime = getSimTime();
	%shellcount = 1;
	
	if(vectorLen(%obj.getVelocity()) < 0.1 && (getSimTime() - %obj.lastShotTime) > 1000)
	{
		%spread = 0.00016;
	}
	else
	{
		%spread = 0.00025;
	}

	%projectile = LightMachinegunProjectile;
	
	%obj.playThread(2, plant);
	%shellcount = 1;

	%obj.LMGHeat++;
	%obj.isFiring = 1;

	commandToClient(%obj.client,'bottomPrint',"<just:right><font:impact:24><color:fff000>Heat <font:impact:34>\c6" @ %obj.LMGHeat @ "/" @ $LMGMaxHeat, 4, 2, 3, 4); 

	%obj.spawnExplosion(TTLittleRecoilProjectile,"1 1 1");

	for(%shell=0; %shell<%shellcount; %shell++)
	{
		%vector = %obj.getMuzzleVector(%slot);
		%vector1 = VectorScale(%vector, %projectile.muzzleVelocity);
		%vector2 = VectorScale(%objectVelocity, %projectile.velInheritFactor);
		%velocity = VectorAdd(%vector1,%vector2);
		%x = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%y = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%z = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
		%mat = MatrixCreateFromEuler(%x @ " " @ %y @ " " @ %z);
		%velocity = MatrixMulVector(%mat, %velocity);

		%p = new (%this.projectileType)()
		{
			dataBlock = %projectile;
			initialVelocity = %velocity;
			initialPosition = %obj.getMuzzlePoint(%slot);
			sourceObject = %obj;
			sourceSlot = %slot;
			client = %obj.client;
		};
		MissionCleanup.add(%p);
	}
	return %p;
}

// function LightMachinegunImage::onFire2(%this,%obj,%slot)
// { 
// 	%fX = getWord(%fvec,0);
// 	%fY = getWord(%fvec,1);
	
// 	%evec = %obj.getEyeVector();
// 	%eX = getWord(%evec,0);
// 	%eY = getWord(%evec,1);
// 	%eZ = getWord(%evec,2);
	
// 	%eXY = mSqrt(%eX*%eX+%eY*%eY);
	
// 	%aimVec = %fX*%eXY SPC %fY*%eXY SPC %eZ;

// 	%obj.setVelocity(VectorAdd(%obj.getVelocity(),VectorScale(%aimVec,"-1")));
	
// 	%obj.lastShotTime = getSimTime();
// 	%shellcount = 1;
	
// 	if(vectorLen(%obj.getVelocity()) < 0.1 && (getSimTime() - %obj.lastShotTime) > 1000)
// 	{
// 		%spread = 0.0002;
// 	}
// 	else
// 	{
// 		%spread = 0.0004;
// 	}

// 	%projectile = LightMachinegunProjectile;
	
// 	%obj.playThread(2, plant);
// 	%shellcount = 1;
// 	if($Pref::Server::TTAmmo == 0 || $Pref::Server::TTAmmo == 1)
// 	{
// 		commandToClient(%obj.client,'bottomPrint',"<just:right><font:impact:24><color:fff000>Heat <font:impact:34>\c6" @ %obj.LMGHeat @ "/" @ $LMGMaxHeat, 4, 2, 3, 4); 
// 	}

// 	%obj.spawnExplosion(TTLittleRecoilProjectile,"1 1 1");

// 	for(%shell=0; %shell<%shellcount; %shell++)
// 	{
// 		%vector = %obj.getMuzzleVector(%slot);
// 		%vector1 = VectorScale(%vector, %projectile.muzzleVelocity);
// 		%vector2 = VectorScale(%objectVelocity, %projectile.velInheritFactor);
// 		%velocity = VectorAdd(%vector1,%vector2);
// 		%x = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
// 		%y = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
// 		%z = (getRandom() - 0.5) * 10 * 3.1415926 * %spread;
// 		%mat = MatrixCreateFromEuler(%x @ " " @ %y @ " " @ %z);
// 		%velocity = MatrixMulVector(%mat, %velocity);

// 		%p = new (%this.projectileType)()
// 		{
// 			dataBlock = %projectile;
// 			initialVelocity = %velocity;
// 			initialPosition = %obj.getMuzzlePoint(%slot);
// 			sourceObject = %obj;
// 			sourceSlot = %slot;
// 			client = %obj.client;
// 		};
// 		MissionCleanup.add(%p);
// 	}
// 	return %p;
// }

function LightMachinegunImage::onReloadStart(%this,%obj,%slot)
{           		
	commandToClient(%obj.client,'bottomPrint',"<just:right><font:impact:24><color:fff000>Heat <font:impact:34>\c6" @ %obj.LMGHeat @ "/" @ $LMGMaxHeat, 4, 2, 3, 4); 
	if(%obj.LMGHeat >= 1 && !isEventPending(%obj.heatSchedule))
	{
		releaseHeat(%obj);
	}
}

function LightMachinegunImage::onReloadWait(%this,%obj,%slot)
{
	commandToClient(%obj.client,'bottomPrint',"<just:right><font:impact:24><color:fff000>Heat <font:impact:34>\c6" @ %obj.LMGHeat @ "/" @ $LMGMaxHeat, 4, 2, 3, 4); 
}

function LightMachinegunImage::onReloaded(%this,%obj,%slot)
{
	%obj.isFiring = 0;
}

function LightMachinegunImage::onHalt(%this,%obj,%slot)
{
	if($Pref::Server::TTAmmo == 0 || $Pref::Server::TTAmmo == 1)
	{
        commandToClient(%obj.client,'bottomPrint',"<just:right><font:impact:24><color:fff000>Heat <font:impact:34>\c6" @ %obj.LMGHeat @ "/" @ $LMGMaxHeat, 4, 2, 3, 4);
	}
	%obj.isFiring = 0;
}

function LightMachinegunImage::onMount(%this,%obj,%slot)
{
   Parent::onMount(%this,%obj,%slot);
	if($Pref::Server::TTAmmo == 0 || $Pref::Server::TTAmmo == 1)
	{
		commandToClient(%obj.client,'bottomPrint',"<just:right><font:impact:24><color:fff000>Heat <font:impact:34>\c6" @ %obj.LMGHeat @ "/" @ $LMGMaxHeat, 4, 2, 3, 4);
	}
}

function LightMachinegunImage::onUnMount(%this,%obj,%slot)
{
   Parent::onUnMount(%this,%obj,%slot);
}

function LightMachinegunImage::onLoadCheck(%this,%obj,%slot)
{
	if(%obj.LMGHeat >= $LMGMaxHeat) 
		%obj.setImageAmmo(%slot,0);
	else
		%obj.setImageAmmo(%slot,1);

	if (%obj.LMGHeat $= "") {
		%obj.LMGHeat = 0;
	}
	commandToClient(%obj.client,'bottomPrint',"<just:right><font:impact:24><color:fff000>Heat <font:impact:34>\c6" @ %obj.LMGHeat @ "/" @ $LMGMaxHeat, 4, 2, 3, 4);

	if(%obj.LMGHeat >= 1 && !isEventPending(%obj.heatSchedule))
	{
		releaseHeat(%obj);
	}
}

if ($LMGMaxHeat $= "") {
	$LMGMaxHeat = 50;
}

function releaseHeat(%obj) {
	if (isEventPending(%obj.heatSchedule) || %obj.isFiring) {
		return;
	}

	if (%obj.LMGHeat > 0) {
		%obj.LMGHeat--;
		%obj.heatSchedule = schedule(1000, %obj, releaseHeat, %obj);
	}
}