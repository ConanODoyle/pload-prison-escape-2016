datablock ParticleData(InfoBronsonParticle)
{
	textureName			 = "./Bronson";
	dragCoefficient		= 10.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0; 
	inheritedVelFactor	= 0;
	lifetimeMS			  = 8000;
	lifetimeVarianceMS	= 0;
	useInvAlpha = true;
	spinRandomMin = 0;
	spinRandomMax = 0;

	colors[0]	  = "1 1 1 1";
	colors[1]	  = "1 1 1 1";
	colors[2]	  = "1 1 1 1";
	colors[3]	  = "1 1 1 1";

	sizes[0]		= 0;
	sizes[1]		= 4;
	sizes[2]		= 3.8;
	sizes[3]		= 3.8;

	times[0]		= 0.0;
	times[1]		= 0.05;
	times[2]		= 0.1;
	times[3]		= 1;
};

datablock ParticleEmitterData(InfoBronsonEmitter)
{
	ejectionPeriodMS = 1000;
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
	orientParticles = false;

	uiName = "Info - Bronson";
};

datablock ExplosionData(InfoBronsonExplosion) {
	cameraShakeFalloff = 1;
	camShakeFrequency = "0 0 0";
	camShakeRadius = 0;
	camShakeAmp = "0 0 0";
	camShakeDuration = 0;

	particleRadius = 0.1;
	particleEmitter = InfoBronsonEmitter;
	particleDensity = 1;
	delayMS = 0;

	lifetimeMS = 50;
	sizes[0]		= "1 1 1";
	sizes[1]		= "1 1 1";

	times[0]		= 0;
	times[1]		= 1;
};

datablock ProjectileData(InfoBronsonProjectile) {
	projectileShapeName = "base/data/shapes/empty.dts";
	explosion = InfoBronsonExplosion;
	muzzleVelocity = 10;
	lifetime = 30;
	explodeOnDeath = true;
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

datablock ExplosionData(InfoBucketExplosion : InfoBronsonExplosion) {
	particleEmitter = InfoBucketEmitter;
};

datablock ProjectileData(InfoBucketProjectile : InfoBronsonProjectile) {
	explosion = InfoBucketExplosion;
	explodeOnDeath = 1;
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

datablock ExplosionData(InfoTrayExplosion : InfoBronsonExplosion) {
	particleEmitter = InfoTrayEmitter;
};

datablock ProjectileData(InfoTrayProjectile : InfoBronsonProjectile) {
	explosion = InfoTrayExplosion;
	explodeOnDeath = 1;
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

datablock ExplosionData(InfoSmokeGrenadeExplosion : InfoBronsonExplosion) {
	particleEmitter = InfoSmokeGrenadeEmitter;
};

datablock ProjectileData(InfoSmokeGrenadeProjectile : InfoBronsonProjectile) {
	explosion = InfoSmokeGrenadeExplosion;
	explodeOnDeath = 1;
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

datablock ExplosionData(InfoCamerasExplosion : InfoBronsonExplosion) {
	particleEmitter = InfoCamerasEmitter;
};

datablock ProjectileData(InfoCamerasProjectile : InfoBronsonProjectile) {
	explosion = InfoCamerasExplosion;
	explodeOnDeath = 1;
};



datablock ParticleData(InfoGeneratorParticle : InfoBronsonParticle)
{
	textureName			 = "./Generator";
	lifetimeMS			  = 10000;
};

datablock ParticleEmitterData(InfoGeneratorEmitter : InfoBronsonEmitter)
{
	particles = "InfoGeneratorParticle";	

	uiName = "Info - Generator";
};

datablock ExplosionData(InfoGeneratorExplosion : InfoBronsonExplosion) {
	particleEmitter = InfoGeneratorEmitter;
};

datablock ProjectileData(InfoGeneratorProjectile : InfoBronsonProjectile) {
	explosion = InfoGeneratorExplosion;
	explodeOnDeath = 1;
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

datablock ExplosionData(InfoLaundryCartExplosion : InfoBronsonExplosion) {
	particleEmitter = InfoLaundryCartEmitter;
};

datablock ProjectileData(InfoLaundryCartProjectile : InfoBronsonProjectile) {
	explosion = InfoLaundryCartExplosion;
	explodeOnDeath = 1;
};



datablock ParticleData(InfoSatDishParticle : InfoBronsonParticle)
{
	textureName			 = "./SatDish";
	lifetimeMS			  = 10000;
};

datablock ParticleEmitterData(InfoSatDishEmitter : InfoBronsonEmitter)
{
	particles = "InfoSatDishParticle";	

	uiName = "Info - SatDish";
};

datablock ExplosionData(InfoSatDishExplosion : InfoBronsonExplosion) {
	particleEmitter = InfoSatDishEmitter;
};

datablock ProjectileData(InfoSatDishProjectile : InfoBronsonProjectile) {
	explosion = InfoSatDishExplosion;
	explodeOnDeath = 1;
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

datablock ExplosionData(InfoSniperRifleExplosion : InfoBronsonExplosion) {
	particleEmitter = InfoSniperRifleEmitter;
};

datablock ProjectileData(InfoSniperRifleProjectile : InfoBronsonProjectile) {
	explosion = InfoSniperRifleExplosion;
	explodeOnDeath = 1;
};



datablock ParticleData(InfoSoapParticle : InfoBronsonParticle)
{
	textureName			 = "./Soap";
};

datablock ParticleEmitterData(InfoSoapEmitter : InfoBronsonEmitter)
{
	particles = "InfoSoapParticle";	

	uiName = "Info - Soap";
};

datablock ExplosionData(InfoSoapExplosion : InfoBronsonExplosion) {
	particleEmitter = InfoSoapEmitter;
};

datablock ProjectileData(InfoSoapProjectile : InfoBronsonProjectile) {
	explosion = InfoSoapExplosion;
	explodeOnDeath = 1;
};



datablock ParticleData(InfoBurgerParticle : InfoBronsonParticle)
{
	textureName			 = "./Burger";
};

datablock ParticleEmitterData(InfoBurgerEmitter : InfoBronsonEmitter)
{
	particles = "InfoBurgerParticle";	

	uiName = "Info - Burger";
};

datablock ExplosionData(InfoBurgerExplosion : InfoBronsonExplosion) {
	particleEmitter = InfoBurgerEmitter;
};

datablock ProjectileData(InfoBurgerProjectile : InfoBronsonProjectile) {
	explosion = InfoBurgerExplosion;
	explodeOnDeath = 1;
};