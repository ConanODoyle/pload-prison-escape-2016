datablock AudioProfile(trayDeflect1Sound)
{
   filename    = "./tray1.wav";
   description = AudioClosest3d;
   preload = true;
};

datablock AudioProfile(trayDeflect2Sound)
{
   filename    = "./tray2.wav";
   description = AudioClosest3d;
   preload = true;
};

datablock AudioProfile(trayDeflect3Sound)
{
   filename    = "./tray3.wav";
   description = AudioClosest3d;
   preload = true;
};

datablock AudioProfile(trayEquipSound)
{
   filename    = "./tray_pullup1.wav";
   description = AudioClosest3d;
   preload = true;
};

datablock ItemData(PrisonTrayItem)
{
	category = "Weapon";// Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./flattray.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Tray";
	iconName = "";
	doColorShift = true;
	colorshiftColor = "1 1 1 1";
	rotation = eulerToMatrix("0 90 0");

	 // Dynamic properties defined by the scripts
	image = PrisonTrayImage;
	canDrop = true;
	
	maxAmmo = 1;
	canReload = 0;
};

datablock ShapeBaseImageData(PrisonTrayImage)
{
	// Basic Item properties
	shapeFile = "./Tray.dts";
	emap = true;

	// Specify mount point & offset for 3rd person, and eye offset
	// for first person rendering.
	mountPoint = 0;
	eyeoffset = "0 0.8 -0.8";
	offset = "-0.53 0.12 -0.12";
	rotation = eulerToMatrix("0 0 0");

	// When firing from a point offset from the eye, muzzle correction
	// will adjust the muzzle vector to point to the eye LOS point.
	// Since this weapon doesn't actually fire from the muzzle point,
	// we need to turn this off.
	correctMuzzleVector = true;

	// Add the WeaponImage namespace as a parent, WeaponImage namespace
	// provides some hooks into the inventory system.
	className = "WeaponImage";

	// Projectile && Ammo.
	item = PrisonTrayItem;
	ammo = " ";

	//melee particles shoot from eye node for consistancy
	melee = false;
	//raise your arm up or not
	armReady = true;

	doColorShift = true;
	colorshiftColor = "1 1 1 1";

	// Images have a state system which controls how the animations
	// are run, which sounds are played, script callbacks, etc. This
	// state system is downloaded to the client so that clients can
	// predict state changes and animate accordingly.The following
	// system supports basic ready->fire->reload transitions as
	// well as a no-ammo->dryfire idle state.

	// Initial start up state
	stateName[0]					= "Activate";
	stateTimeoutValue[0]			= 0.1;
	stateTransitionOnTimeout[0]	 	= "Ready";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]					= "Ready";
	stateAllowImageChange[1]		= true;
};

function PrisonTrayImage::onMount(%this, %obj, %slot)
{
	%obj.playThread(2, armReadyBoth);
	%obj.isHoldingTray = 1;
}

function PrisonTrayImage::onUnMount(%this, %obj, %slot)
{
	%obj.playThread(2, root);
	%obj.isHoldingTray = 0;
}

datablock DebrisData(PrisonTrayDebris)
{
	shapeFile = "./tray.dts";

	lifetime = 5.0;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 2;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;
	spinSpeed			= 300.0;
	minSpinSpeed = -600.0;
	maxSpinSpeed = 600.0;

	gravModifier = 6;
};

datablock ExplosionData(PrisonTrayExplosion)
{
	//explosionShape = "";
	soundProfile = "";

	lifeTimeMS = 150;

	debris = PrisonTrayDebris;
	debrisNum = 1;
	debrisNumVariance = 0;
	debrisPhiMin = 0;
	debrisPhiMax = 360;
	debrisThetaMin = 45;
	debrisThetaMax = 115;
	debrisVelocity = 9;
	debrisVelocityVariance = 8;

	faceViewer	  = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "10.0 11.0 10.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 1;
	camShakeRadius = 2.0;

	// Dynamic light
	lightStartRadius = 0;
	lightEndRadius = 2;
	lightStartColor = "0.3 0.6 0.7";
	lightEndColor = "0 0 0";

	impulseRadius = 0;
	impulseForce = 0;
};

datablock ProjectileData(PrisonTrayProjectile)
{
	projectileShapeName = "";
	explosion           = PrisonTrayExplosion;
	explodeondeath 		= true;
	armingDelay         = 0;
	hasLight    = false;
};

package PrisonItems
{
	function ProjectileData::onCollision(%data, %obj, %col, %fade, %pos, %normal)
	{
		if (%col.isHoldingTray)
		{
			%targetVector = vectorNormalize(vectorSub(%obj.getPosition(), %col.getHackPosition()));
			%angle = mACos(vectorDot(%col.getMuzzleVector(0), %targetVector));
			if (%angle < 0.73)
			{
				//statistics
				$Server::PrisonEscape::TraysUsed++;
				%col.client.traysUsed++;
				%obj.sourceObject.client.traysPlonked++;

				%col.tool[%col.currtool] = 0;
				%col.weaponCount--;
				messageClient(%col.client,'MsgItemPickup','',%col.currtool,0);
				serverCmdUnUseTool(%col.client);
				%col.unMountImage();

				%sound = getRandom(1, 3);
				%sound = "trayDeflect" @ %sound @ "Sound";
				serverPlay3D(%sound, %col.getHackPosition());

				%proj = new Projectile()
				{
					dataBlock = PrisonTrayProjectile;
					initialPosition = %col.getHackPosition();
					initialVelocity = %col.getEyeVector();
					client = %col.client;
				};
				MissionCleanup.add(%proj);
				%proj.explode();
				%obj.delete();
				return;
			}
		}
		if (%col.isWearingBucket)
		{
			%head = getWord(%col.getHackPosition(), 2)*1.56;
			if (getWord(%pos, 2) > %head)
			{
				for (%i=0; %i < %col.getDatablock().maxTools; %i++)
				{
					if (%col.tool[%i].getName() $= "PrisonBucketItem")
					{
						//statistics
						$Server::PrisonEscape::BucketsUsed++;
						%col.client.bucketsUsed++;
						%obj.sourceObject.client.bucketPlonked++;

						%col.tool[%i] = 0;
						%col.weaponCount--;
						messageClient(%col.client,'MsgItemPickup','',%i,0);

						%sound = getRandom(1, 3);
						%sound = "trayDeflect" @ %sound @ "Sound";
						serverPlay3D(%sound, %col.getHackPosition());


						%col.unmountImage(2);
						%col.client.applyBodyParts();
						%col.client.applyBodyColors();
						%col.unhideNode("headskin");
						%col.isWearingBucket = 0;

						%proj = new Projectile()
						{
							dataBlock = PrisonBucketProjectile;
							initialPosition = %col.getHackPosition();
							initialVelocity = %col.getEyeVector();
							client = %col.client;
						};
						MissionCleanup.add(%proj);
						%proj.explode();
						%obj.delete();
						return;
					}
				}
			}


		}
		parent::onCollision(%data, %obj, %col, %fade, %pos, %normal);
	}
};
activatePackage(PrisonItems);