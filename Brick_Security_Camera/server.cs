%error = ForceRequiredAddOn( "Support_Doors" );
if( %error == $Error::AddOn_NotFound )
{
	error("Brick Doors: Support_Doors is missing somehow, what did you do?");
}

datablock fxDTSBrickData(brickSecurityCameraRightData)
{
	brickFile = "./rcam.blb";
	uiName = "Security Camera";
	//iconName = "Add-Ons/Brick_Security_Camera/Security Camera Right";

	isDoor = 1;
	isOpen = 1;

	closedCW = "brickSecurityCameraData";
	openCW = "brickSecurityCameraLeftData";

	closedCCW = "brickSecurityCameraData";
	openCCW = "brickSecurityCameraRightData";
};

datablock fxDTSBrickData(brickSecurityCameraLeftData : brickSecurityCameraRightData)
{
	brickFile = "./lcam.blb";
	
	isOpen = 1;
	//iconName = "Add-Ons/Brick_Security_Camera/Security Camera";
};


datablock fxDTSBrickData(brickSecurityCameraData : brickSecurityCameraRightData)
{
	brickFile = "./fcam.blb";
	
	isOpen = 0;

	category = "Special";
	subCategory = "Doors";
	iconName = "Add-Ons/Brick_Security_Camera/Security Camera";
};