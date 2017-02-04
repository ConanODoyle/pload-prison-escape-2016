registerOutputEvent("Player", "electrocute", "int 0 60", 1);

function Player::electrocute(%player, %time) 
{
	if (!isObject(%client = %player.client))
		return;

	%client.camera.setMode(Corpse, %player);
	%client.setControlObject(%client.camera);
	%client.elecrocutedTime += %time;
	
	electrocute(%player, %time);
}

if (isPackage(EventElectrocute))
	deactivatePackage(EventElectrocute);

package EventElectrocute {
	
	function Observer::onTrigger(%this, %obj, %trig, %state) {
		%client = %obj.getControllingClient();
		
		if (%client.isBeingStunned)
			return;
		
		Parent::onTrigger(%this, %obj, %trig, %state);
	}
	
};
activatePackage(EventElectrocute);

datablock WheeledVehicleData(deathVehicle)
{
	//tagged fields
	doSimpleDismount = true;		//just unmount the player, dont look for a free space


   category = "Vehicles";
   shapeFile = "./deathVehicle.dts";
   emap = true;

   maxDamage = 1.0;
   destroyedLevel = 0.5;

   maxSteeringAngle = 0.885;  // Maximum steering angle, should match animation
   //tireEmitter = TireEmitter; // All the tires use the same dust emitter

   // 3rd person camera settings
   cameraRoll = false;         // Roll the camera with the vehicle
   cameraMaxDist = 6;         // Far distance from vehicle
   cameraOffset = 0;        // Vertical offset from camera mount point
   cameraLag = 0.0;           // Velocity lag of camera
   cameraDecay = 0.0;        // Decay per sec. rate of velocity lag

   // Rigid Body
   mass = 90;		//3100 lbs
   density = 0.5;
   massCenter = "0.0 0.0 1.25";    // Center of mass for rigid body
   massBox = "1.25 1.25 2.65";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.6;                // Drag coefficient
   bodyFriction = 0.6;
   bodyRestitution = 0.7;
   minImpactSpeed = 5;        // Impacts over this invoke the script callback
   softImpactSpeed = 1;       // Play SoftImpact Sound
   hardImpactSpeed = 1;      // Play HardImpact Sound
   integration = 10;          // Physics integration: TickSec/Rate
   collisionTol = 0.1;        // Collision distance tolerance
   contactTol = 0.1;          // Contact velocity tolerance

   // Engine
   engineTorque = 0;       // Engine power
   engineBrake = 0;         // Braking when throttle is 0
   brakeTorque = 0;        // When brakes are applied
   maxWheelSpeed = 0;        // Engine scale by current speed / max speed

	forwardThrust		= 0;
	reverseThrust		= 0;
	lift			= 0;
	maxForwardVel		= 40;
	maxReverseVel		= 10;
	horizontalSurfaceForce	= 0;   // Horizontal center "wing" (provides "bite" into the wind for climbing/diving and turning)
	verticalSurfaceForce	= 0; 
	rollForce		= 0;
	yawForce		= 0;
	pitchForce		= 0;
	rotationalDrag		= 1.0;
	stallSpeed		= 0;


   // Energy
   maxEnergy = 0;
   jetForce = 0;
   minJetEnergy = 0;
   jetEnergyDrain = 0;

   // Sounds
//   jetSound = ScoutThrustSound;
   //engineSound = Impact1ASound;
   //squealSound = Impact1ASound;
   softImpactSound = Impact2ASound;
   hardImpactSound = Impact2ASound;
   //wheelImpactSound = Impact1BSound;

//   explosion = VehicleExplosion;
};

function stun(%player, %time) {
	if (!isObject(%client = %player.client))
		return;

	if (isEventPending(%player.stunLoop))
		cancel(%player.stunLoop);
	if (%time <= 0)
	{
		clearTumble(%player);
		%client.isBeingStunned = 0;
		%player.unmountImage(3);
		%player.setControlObject(%player);
		return;
	} else if (!%client.isBeingStunned) {
		%player.setControlObject(%client.camera);
		%player.setVelocity(vectorAdd(%player.getVelocity(), getRandom() * 3 SPC getRandom() * 3 SPC "5"));
		%player.mountImage(stunImage, 3);
		tumble(%player, %time);
		%client.setControlObject(%player);
		serverPlay3D(playerMountSound, %player.getHackPosition());
	}

	%client.stunnedTime += 1;
	%client.isBeingStunned = 1;
	
	//emitter here


	%player.stunLoop = schedule(1000, 0, stun, %player, %time - 1);
}

function electrocute(%player, %time)
{
	if (!isObject(%client = %player.client))
		return;

	if (isEventPending(%player.electrocuteLoop))
		cancel(%player.electrocuteLoop);
	if (%time <= 0)
	{
		%client.applyBodyColors();
		%client.camera.setMode(Observer);
		%client.setControlObject(%player);
		%client.isBeingStunned = 0;
		return;
	}

	%client.isBeingStunned = 1;

	%player.setNodeColor("ALL", "1 1 1 1");
	%player.schedule(100, setNodeColor, "ALL", "0 0 0 1");
	%player.schedule(200, setNodeColor, "ALL", "1 1 1 1");
	%player.schedule(300, setNodeColor, "ALL", "0 0 0 1");
	%player.schedule(400, setNodeColor, "ALL", "1 1 1 1");
	%player.schedule(500, setNodeColor, "ALL", "0 0 0 1");
	%player.schedule(600, setNodeColor, "ALL", "1 1 1 1");
	%player.schedule(700, setNodeColor, "ALL", "0 0 0 1");
	%player.schedule(800, setNodeColor, "ALL", "1 1 1 1");
	%player.schedule(900, setNodeColor, "ALL", "0 0 0 1");

	%player.playThread(2, plant);
	%player.schedule(100, playThread, 2, plant);
	%player.schedule(200, playThread, 2, plant);
	%player.schedule(300, playThread, 2, plant);
	%player.schedule(400, playThread, 2, plant);
	%player.schedule(500, playThread, 2, plant);
	%player.schedule(600, playThread, 2, plant);
	%player.schedule(700, playThread, 2, plant);
	%player.schedule(800, playThread, 2, plant);
	%player.schedule(900, playThread, 2, plant);

	%player.electrocuteLoop = schedule(1000, 0, electrocute, %player, %time - 1);

	spawnRadioWaves(%player);
	schedule(100, 0, spawnRadioWaves, %player);
	schedule(200, 0, spawnRadioWaves, %player);
	schedule(300, 0, spawnRadioWaves, %player);
	schedule(400, 0, spawnRadioWaves, %player);
	schedule(500, 0, spawnRadioWaves, %player);
	schedule(600, 0, spawnRadioWaves, %player);
	schedule(700, 0, spawnRadioWaves, %player);
	schedule(800, 0, spawnRadioWaves, %player);
	schedule(900, 0, spawnRadioWaves, %player);
}

function spawnRadioWaves(%player)
{
	%pos = %player.getMuzzlePoint(2);
	%scale = getWord(%player.getScale(), 2)*0.5+getRandom()*1.5;

	%proj = new Projectile(){
		datablock = radioWaveProjectile;
		initialPosition = %pos;
		initialVelocity = "0 0 0";
		scale = %scale SPC %scale SPC %scale;
	};
	%proj.explode();
}