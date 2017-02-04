datablock AudioProfile(SniperRifleSpotlightBoltSound)
{
   filename    = "./sniperBolt_sniperSpotlight.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(SniperRifleSpotlightFireSound)
{
   filename    = "./fire2.wav";
   description = AudioDefault3d;
   preload = true;
};

datablock AudioProfile(SniperRifleSpotlightUnholsterSound)
{
   filename    = "./sniperunholster_sniperSpotlight.wav";
   description = AudioClose3d;
   preload = true;
};

datablock ParticleData(SniperRifleSpotlightSmokeParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.5;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 1525;
	lifetimeVarianceMS   = 55;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 10.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.5 0.5 0.5 0.9";
	colors[1]     = "0.5 0.5 0.5 0.0";
	sizes[0]      = 0.15;
	sizes[1]      = 0.25;

	useInvAlpha = false;
};
datablock ParticleEmitterData(SniperRifleSpotlightSmokeEmitter)
{
   ejectionPeriodMS = 10;
   periodVarianceMS = 0;
   ejectionVelocity = 1.0;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "SniperRifleSpotlightSmokeParticle";
};


//bullet trail effects
datablock ParticleData(SniperRifleSpotlightBulletTrailParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.0;
	inheritedVelFactor   = 0.0;
	constantAcceleration = 0.0;
	lifetimeMS           = 625;
	lifetimeVarianceMS   = 55;
	textureName          = "base/data/particles/thinRing";
	spinSpeed		= 10.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.3 0.3 0.9 0.4";
	colors[1]     = "0.5 0.5 0.5 0.0";
	sizes[0]      = 0.15;
	sizes[1]      = 0.25;

	useInvAlpha = false;
};
datablock ParticleEmitterData(SniperRifleSpotlightBulletTrailEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 0.0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "SniperRifleSpotlightBulletTrailParticle";
};

AddDamageType("SniperrifleSpotlight",   '<bitmap:Add-Ons/Item_Prison/Weapon_Sniper_Rifle_Spotlight/CI_sniperrifle> %1',    '%2 <bitmap:Add-Ons/Item_Prison/Weapon_Sniper_Rifle_Spotlight/CI_sniperrifle> %1',0.5,1);
datablock ProjectileData(SniperRifleSpotlightProjectile)
{
   projectileShapeName = "./bullet.dts";
   directDamage        = 150;
   directDamageType    = $DamageType::SniperrifleSpotlight;
   radiusDamageType    = $DamageType::SniperrifleSpotlight;

   brickExplosionRadius = 0;
   brickExplosionImpact = true;          //destroy a brick if we hit it directly?
   brickExplosionForce  = 25;
   brickExplosionMaxVolume = 25;          //max volume of bricks that we can destroy
   brickExplosionMaxVolumeFloating = 15;  //max volume of bricks that we can destroy if they aren't connected to the ground

   impactImpulse	     = 1200;
   verticalImpulse	  = 1400;
   explosion           = GunExplosion;
   particleEmitter     = SniperRifleSpotlightBulletTrailEmitter;

   muzzleVelocity      = 200;
   velInheritFactor    = 1;

   armingDelay         = 00;
   lifetime            = 4000;
   fadeDelay           = 3500;
   bounceElasticity    = 0.5;
   bounceFriction      = 0.20;
   isBallistic         = false;
   gravityMod = 0.0;

   hasLight    = false;
   lightRadius = 3.0;
   lightColor  = "0 0 0.5";
};

//////////
// item //
//////////
datablock ItemData(SniperRifleSpotlightItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./sniper rifle v2.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Sniper Rifle Spotlight";
	iconName = "./icon_sniperrifle";
	doColorShift = false;
	colorShiftColor = "0.25 0.25 0.25 1.000";

	 // Dynamic properties defined by the scripts
	image = SniperRifleSpotlightImage;
	canDrop = true;
};


////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(SniperRifleSpotlightImage)
{
   // Basic Item properties
   shapeFile = "./sniper rifle v2.dts";
   emap = true;

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = "0.35 0.55 -0.35";
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
   item = SniperRifleSpotlightItem;
   ammo = " ";
   projectile = SniperRifleSpotlightProjectile;
   projectileType = Projectile;

	casing = GunShellDebris;
	shellExitDir        = "1.0 -1.3 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 6.0;

   //melee particles shoot from eye node for consistancy
   melee = false;
   //raise your arm up or not
   armReady = true;
   minShotTime = 2.00;   //minimum time allowed between shots (needed to prevent equip/dequip exploit)

   doColorShift = false;
   colorShiftColor = SniperRifleSpotlightItem.colorShiftColor;//"0.400 0.196 0 1.000";

   //casing = " ";

   // Images have a state system which controls how the animations
   // are run, which sounds are played, script callbacks, etc. This
   // state system is downloaded to the client so that clients can
   // predict state changes and animate accordingly.  The following
   // system supports basic ready->fire->reload transitions as
   // well as a no-ammo->dryfire idle state.

   // Initial start up state
	stateName[0]                    = "Activate";
	stateTimeoutValue[0]            = 0.05;
	stateTransitionOnTimeout[0]		= "PreReady";
	stateSequence[0]				= "Ready";
	stateSound[0]					= SniperRifleSpotlightUnholsterSound;

	stateName[6]					="PreReady";
	stateTimeoutValue[6]            = 0.05;
	stateTransitionOnNoAmmo[6]      = "Reload";
	stateTransitionOnAmmo[6]       	= "Ready";
	stateWaitForTimeout[6]			= true;

	stateName[1]                     = "Ready";
	stateTimeoutValue[1]            = 0.14;
	stateTransitionOnTriggerDown[1]  = "Fire";
	stateAllowImageChange[1]         = true;

	stateName[2]                    = "Fire";
	stateTransitionOnTimeout[2]     = "Smoke";
	stateTimeoutValue[2]            = 0.1;
	stateFire[2]                    = true;
	stateAllowImageChange[2]        = false;
	stateSequence[2]                = "Fire";
	stateScript[2]                  = "onFire";
	stateWaitForTimeout[2]			= true;
	stateEmitter[2]					= GunFlashEmitter;
	stateEmitterTime[2]				= 0.05;
	stateEmitterNode[2]				= "muzzlePoint";
	stateSound[2]					= gunShot1Sound;

	stateName[3] 					= "Smoke";
	stateEmitter[3]					= SniperRifleSpotlightSmokeEmitter;
	stateEmitterTime[3]				= 0.8;
	stateEmitterNode[3]				= "muzzlePoint";
	stateTimeoutValue[3]            = 0.8;
	stateTransitionOnTimeout[3]     = "Reload";
	stateWaitForTimeout[3]			= true;

	stateName[4]			= "Reload";
	stateSequence[4]		= "Reload";
	stateTimeoutValue[4]	= 1.23;
	stateTransitionOnTimeout[4]     = "PostReload";
	stateSound[4]					= "SniperRifleSpotlightBoltSound";
	stateEjectShell[4]				= true;

	stateName[5]			= "PostReload";
	stateScript[5]			= "onReload";
	stateTransitionOnTimeout[5] = "Ready";
	stateTimeoutValue[5]            = 0.05;
	stateWaitForTimeout[5]			= true;
	//stateSequence[4]	= "Ready";

};

function SniperRifleSpotlightImage::onFire(%this,%obj,%slot)
{
	//statistics
	$Server::PrisonEscape::SniperRifleBullets++;
	%obj.client.shotsFired++;

	%obj.playThread(2, plant);
	Parent::onFire(%this,%obj,%slot);
	%obj.setImageAmmo(%slot, 0);
}

function SniperRifleSpotlightImage::onReload(%this, %obj, %slot)
{
	%obj.setImageAmmo(%slot, 1);
}

function SniperRifleSpotlightImage::onMount(%this, %obj, %slot)
{
}

function SniperRifleSpotlightImage::onUnMount(%this, %obj, $slot)
{
	%client = %obj.client;
	if (!isObject(%client))
		return;
	%client.applyBodyParts();
	%client.applyBodyColors();
	%obj.playThread(2, "root");
}

package SniperRifleSpotlight
{
	function SniperRifleSpotlightImage::setImageAmmo(%slot, %hasAmmo)
	{
		talk(%slot SPC %hasAmmo);
		parent::setImageAmmo(%slot, %hasAmmo);
	}

	function SniperRifleSpotlightProjectile::onCollision(%data, %obj, %col, %fade, %pos, %normal)
	{
		%guard = %obj.originPoint;
		%type = $TypeMasks::PlayerObjectType;
		%radius = 10;

		//look for nearby spotlight from firing position
		initContainerRadiusSearch(%guard, %radius, %type);
		%target = 0;
		while (isObject(%next = containerSearchNext()))
		{
			if (%next.getDatablock().getName() $= "SpotlightArmor")
			{
				%target = %next;
				break;
			}
		}

		//end it here if no spotlight found
		if (!isObject(%target))
			return parent::onCollision(%data, %obj, %col, %fade, %pos, %normal);

		//track player if hit player directly
		if (%col.getClassName() $= "Player" && !%col.isGuard)
		{
			%target.spotLightTarget = %col;
			%target.spotLightTargetLocation = "";

			//statistics: increment %client.shotsHit
			%obj.client.shotsHit++;
		}
		else
		{
			//do container search for nearby players (4x stud radius) to track
			initContainerRadiusSearch(%pos, 2, $TypeMasks::PlayerObjectType);
			%player = 0;
			while (isObject(%next = containerSearchNext()))
			{
				if (%next.getClassName() $= "Player")
				{
					%player = %next;
					break;
				}
			}

			if (!isobject(%player) && !%player.isGuard)
			{
				%target.spotLightTarget = 0;
				%target.spotLightTargetLocation = %pos;	
			}
			else
			{
				%target.spotLightTarget = %player;
				%target.spotLightTargetLocation = "";	
			}	
		}

		//dont be able to hurt the dog
		if (%col.getDatablock().getName() $= "ShepherdDogArmor")
		{
			%obj.delete();
			return;
		}

		parent::onCollision(%data, %obj, %col, %fade, %pos, %normal);
	}
};
activatePackage(SniperRifleSpotlight);