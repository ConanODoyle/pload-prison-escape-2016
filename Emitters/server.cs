datablock ParticleData(InfoBronsonParticle)
{
	textureName			 = "./Bronson";
	dragCoefficient		= 10.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0; 
	inheritedVelFactor	= 0;
	lifetimeMS			  = 5000;
	lifetimeVarianceMS	= 0;
	useInvAlpha = false;
	spinRandomMin = 0;
	spinRandomMax = 0;

	colors[0]	  = "1 1 1 1";
	colors[1]	  = "1 1 1 1";
	colors[2]	  = "1 1 1 1";
	colors[3]	  = "1 1 1 1";

	sizes[0]		= 0;
	sizes[1]		= 2;
	sizes[2]		= 1.8;
	sizes[3]		= 1.8;

	times[0]		= 0.0;
	times[1]		= 0.05;
	times[2]		= 0.1;
	times[3]		= 1;
};

datablock ParticleEmitterData(InfoBronsonEmitter)
{
	ejectionPeriodMS = 15000;
	periodVarianceMS = 0;

	ejectionOffset = 0;
	ejectionOffsetVariance = 0;
	
	ejectionVelocity = 0;
	velocityVariance = 0;

	thetaMin			= 0.0;
	thetaMax			= 1.0;  

	phiReferenceVel  = 0;
	phiVariance		= 1;

	particles = "InfoBronsonParticle";	

	useEmitterColors = false;

	uiName = "Info - Bronson";
};

datablock ParticleData(InfoBucketParticle : InfoBronsonParticle)
{
	textureName			 = "./Bucket";
};

datablock ParticleEmitterData(InfoBucketEmitter : InfoBronsonEmitter)
{
	particles = "InfoBucketParticle";	

	uiName = "Info - Bucket";
};


datablock ParticleData(InfoTrayParticle : InfoBronsonParticle)
{
	textureName			 = "./Tray";
};

datablock ParticleEmitterData(InfoTrayEmitter : InfoBronsonEmitter)
{
	particles = "InfoTrayParticle";	

	uiName = "Info - Tray";
};


datablock ParticleData(InfoSmokeGrenadeParticle : InfoBronsonParticle)
{
	textureName			 = "./SmokeGrenade";
};

datablock ParticleEmitterData(InfoSmokeGrenadeEmitter : InfoBronsonEmitter)
{
	particles = "InfoSmokeGrenadeParticle";	

	uiName = "Info - SmokeGrenade";
};


datablock ParticleData(InfoCamerasParticle : InfoBronsonParticle)
{
	textureName			 = "./Cameras";
};

datablock ParticleEmitterData(InfoCamerasEmitter : InfoBronsonEmitter)
{
	particles = "InfoCamerasParticle";	

	uiName = "Info - Cameras";
};


datablock ParticleData(InfoGeneratorParticle : InfoBronsonParticle)
{
	textureName			 = "./Generator";
};

datablock ParticleEmitterData(InfoGeneratorEmitter : InfoBronsonEmitter)
{
	particles = "InfoGeneratorParticle";	

	uiName = "Info - Generator";
};


datablock ParticleData(InfoLaundryCartParticle : InfoBronsonParticle)
{
	textureName			 = "./LaundryCart";
};

datablock ParticleEmitterData(InfoLaundryCartEmitter : InfoBronsonEmitter)
{
	particles = "InfoLaundryCartParticle";	

	uiName = "Info - LaundryCart";
};


datablock ParticleData(InfoSatDishParticle : InfoBronsonParticle)
{
	textureName			 = "./SatDish";
};

datablock ParticleEmitterData(InfoSatDishEmitter : InfoBronsonEmitter)
{
	particles = "InfoSatDishParticle";	

	uiName = "Info - SatDish";
};


datablock ParticleData(InfoSniperRifleParticle : InfoBronsonParticle)
{
	textureName			 = "./SniperRifle";
};

datablock ParticleEmitterData(InfoSniperRifleEmitter : InfoBronsonEmitter)
{
	particles = "InfoSniperRifleParticle";	

	uiName = "Info - SniperRifle";
};