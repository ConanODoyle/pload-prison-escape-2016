exec("./Item_Tray/server.cs");
exec("./Item_Bucket/server.cs");
exec("./Weapon_Sniper_Rifle_Spotlight/server.cs");
exec("./Item_Smoke_Grenade/server.cs");
exec("./Item_Chisel/server.cs");
exec("./Item_Steak/server.cs");

datablock ParticleData(goldenParticleA)
{
	textureName			 = "base/lighting/flare";
	dragCoefficient		= 0.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0; 
	inheritedVelFactor	= 1;
	lifetimeMS			  = 300;
	lifetimeVarianceMS	= 100;
	useInvAlpha = false;
	spinRandomMin = 280.0;
	spinRandomMax = 281.0;

	colors[0]	  = "1 1 0 0";
	colors[1]	  = "1 1 0 1";
	colors[2]	  = "1 1 0 0";

	sizes[0]		= 0;
	sizes[1]		= 2;
	sizes[2]		= 0;

	times[0]		= 0.0;
	times[1]		= 0.5;
	times[2]		= 1.0;
};

datablock ParticleData(goldenParticleB)
{
	textureName			 = "base/lighting/flare";
	dragCoefficient		= 0.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0; 
	inheritedVelFactor	= 1;
	lifetimeMS			  = 300;
	lifetimeVarianceMS	= 100;
	useInvAlpha = false;
	spinRandomMin = 280.0;
	spinRandomMax = 281.0;

	colors[0]	  = "1 1 0 0";
	colors[1]	  = "1 1 0 1";
	colors[2]	  = "1 1 0 0";

	sizes[0]		= 0;
	sizes[1]		= 1;
	sizes[2]		= 0;

	times[0]		= 0.0;
	times[1]		= 0.5;
	times[2]		= 1.0;
};


datablock ParticleEmitterData(goldenEmitter)
{
	ejectionPeriodMS = 280;
	periodVarianceMS = 110;

	ejectionOffset = 0.8;
	ejectionOffsetVariance = 0.5;
	
	ejectionVelocity = 0;
	velocityVariance = 0;

	thetaMin			= 0.0;
	thetaMax			= 180.0;  

	phiReferenceVel  = 0;
	phiVariance		= 360;

	particles = "goldenParticleA goldenParticleB";	

	useEmitterColors = false;

	uiName = "Golden Shine";
};


exec("./Item_Tray/golden/server.cs");
exec("./Item_Bucket/golden/server.cs");
//exec("./Weapon_Sniper_Rifle_Spotlight/golden/server.cs");
exec("./Item_Smoke_Grenade/golden/server.cs");
exec("./Item_Chisel/golden/server.cs");