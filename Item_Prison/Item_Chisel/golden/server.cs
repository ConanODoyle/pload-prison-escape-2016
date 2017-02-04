//projectile
datablock ProjectileData(chiselGoldenProjectile)
{
   directDamage        = 8;
   directDamageType  = $DamageType::chiselDirect;
   radiusDamageType  = $DamageType::chiselRadius;
   explosion           = swordExplosion;
   //particleEmitter     = as;

   muzzleVelocity      = 50;
   velInheritFactor    = 1;

   armingDelay         = 0;
   lifetime            = 100;
   fadeDelay           = 70;
   bounceElasticity    = 0;
   bounceFriction      = 0;
   isBallistic         = false;
   gravityMod = 0.0;

   hasLight    = false;
   lightRadius = 3.0;
   lightColor  = "0 0 0.5";
};


//////////
// item //
//////////
datablock ItemData(chiselGoldenItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./Chisel.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Golden Chisel";
	iconName = "./knife";
	doColorShift = true;
	colorShiftColor = "1 0.9 0 1.000";

	 // Dynamic properties defined by the scripts
	image = chiselGoldenImage;
	canDrop = true;
};

//function chiselGolden::onUse(%this,%user)
//{
//	//mount the image in the right hand slot
//	%user.mountimage(%this.image, $RightHandSlot);
//}

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(chiselGoldenImage)
{
   // Basic Item properties
   shapeFile = "./chisel.dts";
   emap = true;

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   offset = "0 0 0";
   //eyeOffset = "0.1 0.2 -0.55";

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.  
   correctMuzzleVector = true;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   className = "WeaponImage";

   // Projectile && Ammo.
   item = chiselGoldenItem;
   ammo = " ";
   projectile = chiselGoldenProjectile;
   projectileType = Projectile;

   //melee particles shoot from eye node for consistancy
   melee = false;
   //raise your arm up or not
   armReady = true;

   //casing = " ";
   doColorShift = true;
   colorShiftColor = "1 0.9 0 1.000";

   // Images have a state system which controls how the animations
   // are run, which sounds are played, script callbacks, etc. This
   // state system is downloaded to the client so that clients can
   // predict state changes and animate accordingly.  The following
   // system supports basic ready->fire->reload transitions as
   // well as a no-ammo->dryfire idle state.

   // Initial start up state
	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 0.5;
	stateTransitionOnTimeout[0]	= "Ready";
	stateSound[0]					= weaponSwitchSound;
	stateEmitter[0]					= "GoldenEmitter";
	stateEmitterNode[0]				= "emitterPoint";
	stateEmitterTime[0]				= 1000;

	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Charge";
	stateAllowImageChange[1]	= true;
	stateEmitter[1]					= "GoldenEmitter";
	stateEmitterNode[1]				= "emitterPoint";
	stateEmitterTime[1]				= 1000;
	
	stateName[2]                    = "Charge";
	stateTransitionOnTimeout[2]	= "Armed";
	stateTimeoutValue[2]            = 0.8;
	stateScript[2]			= "oncharge";
	stateWaitForTimeout[2]		= false;
	stateTransitionOnTriggerUp[2]	= "AbortCharge";
	stateAllowImageChange[2]        = false;
	stateEmitter[2]					= "GoldenEmitter";
	stateEmitterNode[2]				= "emitterPoint";
	stateEmitterTime[2]				= 1000;
	
	stateName[3]			= "Armed";
	stateTransitionOnTriggerUp[3]	= "Fire";
	stateAllowImageChange[3]	= false;
	stateEmitter[3]					= "GoldenEmitter";
	stateEmitterNode[3]				= "emitterPoint";
	stateEmitterTime[3]				= 1000;

	stateName[4]			= "Fire";
	stateTransitionOnTimeout[4]	= "Ready";
	stateTimeoutValue[4]		= 0.2;
	stateFire[4]			= true;
	stateScript[4]			= "onFire";
	stateWaitForTimeout[4]		= true;
	stateAllowImageChange[4]	= false;
	stateEmitter[4]					= "GoldenEmitter";
	stateEmitterNode[4]				= "emitterPoint";
	stateEmitterTime[4]				= 1000;

	stateName[5]			= "AbortCharge";
	stateScript[5]			= "onAbortCharge";
	stateTransitionOnTimeout[5] = "Ready";
	stateTimeoutValue[5] = 0.1;
	stateEmitter[5]					= "GoldenEmitter";
	stateEmitterNode[5]				= "emitterPoint";
	stateEmitterTime[5]				= 1000;
};

function chiselGoldenImage::onCharge(%this, %obj, %slot)
{
	%obj.playthread(2, spearReady);
}
function chiselGoldenImage::onFire(%this, %obj, %slot)
{
	//statistics
	%obj.client.chiselAttack++;
	$Server::PrisonEscape::chiselAttacks++;

	%obj.playthread(2, spearThrow);
	Parent::onFire(%this, %obj, %slot);
}
function chiselGoldenImage::onAbortCharge(%this, %obj, %slot)
{
	%obj.playthread(2, spearThrow);
}

function isBreakableBrick(%brick, %player)
{
	%db = %brick.getDatablock().getName();
	%pole = "brick1x1fpoleData";
	%pole2 = "brick1x1poleData";
	%plate = "brick1x3fData";
	%plate2 = "brick1x1fData";
	%window = "brick4x5x2WindowData";
	if (%brick.willCauseChainKill() || %brick.getName() !$= "")
		return "";
	if (%db $= %pole || %db $= %pole2 || %db $= %window)
		return %db;
	if ((%db $= %plate || %db $= %plate2) && (getRegion(%player) $= "Yard" || getRegion(%player) $="Outside"))
		return %db;
	return "";
}

package ChiselHit
{
	function chiselGoldenProjectile::onCollision(%data, %obj, %col, %fade, %pos, %normal)
	{
		if (%col.getClassName() $= "FxDTSBrick" && %obj.sourceObject.getClassName() $= "Player")
		{
			%type = isBreakableBrick(%col, %obj.sourceObject);
			if (%type !$= "")
			{
				//statistics
				%obj.client.chiselHit++;
				if (%type $= "brick1x1fpoleData" || %type $= "brick1x3fData" || %type $= "brick1x1fData")
					%col.killbrick();
				else
					%col.damage(5);
				%obj.client.incScore(1);
			}
		}
		return parent::onCollision(%data, %obj, %col, %fade, %pos, %normal);
	}
};
activatePackage(ChiselHit);

function FxDTSBrick::damage(%brick, %damage)
{
	if(isEventPending(%brick.recolorSchedule))
		cancel(%brick.recolorSchedule);

	if(!%brick.hasOriginalData)
	{
		%brick.origColorID = %brick.getColorID();
		%brick.hasOriginalData = 1;
	}
	if(!%brick.maxDamage)
	{
		%db = %brick.getDatablock();
		%brick.maxDamage = %db.brickSizeX * %db.brickSizeY * %db.brickSizeZ / 3 + 20;
	}

	%brick.damage += %damage;
	if (%brick.damage > %brick.maxDamage)
		%brick.killbrick();

	%brick.setColor(45);
	%brick.playSound(glassExplosionSound);
	%brick.recolorSchedule = %brick.schedule(50, setColor, %brick.origColorID);
}