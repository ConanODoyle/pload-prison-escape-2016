//projectile

datablock ProjectileData(RiotSmokeGrenadeProjectile)
{
	directDamage		  = 8;
	directDamageType  = $DamageType::rocketDirect;
	radiusDamageType  = $DamageType::rocketRadius;
	explosion			  = riotSmokeGrenadeExplosion;
	//particleEmitter	  = as;

	muzzleVelocity		= 50;
	velInheritFactor	 = 1;

	armingDelay			= 0;
	lifetime				= 100;
	fadeDelay			  = 70;
	bounceElasticity	 = 0;
	bounceFriction		= 0;
	isBallistic			= false;
	gravityMod = 0.0;

	hasLight	 = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
};

datablock StaticShapeData(SmokeGrenadeShape)
{
	shapeFile = "./smoke sphere.dts";
};

function createSmokeSphereAt(%pos) {
	%shape = new StaticShape(Smoke) {
		datablock = SmokeGrenadeShape;
		position = %pos;
	};
	MissionCleanup.add(%shape);
	return (%shape);
}

datablock ParticleData(smokeParticleA)
{
	textureName			 = "base/data/particles/cloud";
	dragCoefficient		= 1.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= -0.02; 
	inheritedVelFactor	= 0.2;
	lifetimeMS			  = 5000;
	lifetimeVarianceMS	= 0;
	useInvAlpha = true;
	spinRandomMin = 0.0;
	spinRandomMax = 0.0;

	colors[0]	  = "0.8 0.8 0.8 0.0";
	colors[1]	  = "0.8 0.8 0.8 1";
	colors[2]	  = "0.8 0.8 0.8 1";
	colors[3]	  = "0.8 0.8 0.8 0";

	sizes[0]		= 5;
	sizes[1]		= 12.5;
	sizes[2]		= 14;
	sizes[3]		= 13.8;

	times[0]		= 0.0;
	times[1]		= 0.1;
	times[2]		= 0.8;
	times[3]		= 1.0;
};

datablock ParticleEmitterData(smokeAEmitter)
{
	ejectionPeriodMS = 50;
	periodVarianceMS = 0;

	ejectionOffset = 0;
	ejectionOffsetVariance = 0.0;
	
	ejectionVelocity = 12;
	velocityVariance = 3.0;

	thetaMin			= 0.0;
	thetaMax			= 180.0;  

	phiReferenceVel  = 0;
	phiVariance		= 360;

	particles = smokeParticleA;	

	useEmitterColors = true;

	uiName = "Smoke A";
};


//////////
// item //
//////////
datablock ItemData(riotSmokeGrenadeItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./smoke grenade.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Smoke Grenade";
	iconName = "";
	doColorShift = false;
	colorShiftColor = "0.400 0.196 0 1.000";

	 // Dynamic properties defined by the scripts
	image = riotSmokeGrenadeImage;
	canDrop = true;
};

//function chisel::onUse(%this,%user)
//{
//	//mount the image in the right hand slot
//	%user.mountimage(%this.image, $RightHandSlot);
//}

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(riotSmokeGrenadeImage)
{
	// Basic Item properties
	shapeFile = "./smoke grenade.dts";
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
	item = chiselItem;
	ammo = " ";
	projectile = RiotSmokeGrenadeProjectile;
	projectileType = Projectile;

	//melee particles shoot from eye node for consistancy
	melee = false;
	//raise your arm up or not
	armReady = true;

	//casing = " ";
	doColorShift = false;
	colorShiftColor = "0.400 0.196 0 1.000";

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

	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Charge";
	stateAllowImageChange[1]	= true;
	
	stateName[2]						  = "Charge";
	stateTransitionOnTimeout[2]	= "Armed";
	stateTimeoutValue[2]				= 0.8;
	stateScript[2]			= "oncharge";
	stateWaitForTimeout[2]		= false;
	stateTransitionOnTriggerUp[2]	= "AbortCharge";
	stateAllowImageChange[2]		  = false;
	
	stateName[3]			= "Armed";
	stateTransitionOnTriggerUp[3]	= "Fire";
	stateAllowImageChange[3]	= false;

	stateName[4]			= "Fire";
	stateTransitionOnTimeout[4]	= "Ready";
	stateTimeoutValue[4]		= 0.2;
	stateFire[4]			= true;
	stateScript[4]			= "onFire";
	stateWaitForTimeout[4]		= true;
	stateAllowImageChange[4]	= false;

	stateName[5]			= "AbortCharge";
	stateScript[5]			= "onAbortCharge";
	stateTransitionOnTimeout[5] = "Ready";
	stateTimeoutValue[5] = 0.1;
};

function riotSmokeGrenadeImage::onCharge(%this, %obj, %slot)
{
	%obj.playthread(2, spearReady);
}
function riotSmokeGrenadeImage::onFire(%this, %obj, %slot)
{
	//statistics
	%obj.client.smokeGrenadesThrown++;
	$Server::PrisonEscape::smokeGrenadesThrown++;

	%obj.playthread(2, spearThrow);
	Parent::onFire(%this, %obj, %slot);
}
function riotSmokeGrenadeImage::onAbortCharge(%this, %obj, %slot)
{
	%obj.playthread(2, activate);
}