%error = ForceRequiredAddOn( "Support_Doors" );
if( %error == $Error::AddOn_NotFound )
{
	error("Brick Doors: Support_Doors is missing somehow, what did you do?");
}

datablock fxDTSBrickData(brickGarageDoorUpData)
{
	brickFile = "./garagedoor_up.blb";
	uiName = "Garage Door";
	//iconName = "Add-Ons/Brick_Security_Camera/Security Camera Right";

	isDoor = 1;
	isOpen = 1;

	closedCW = "brickGarageDoorData";
	openCW = "brickGarageDoorUpData";

	closedCCW = "brickGarageDoorData";
	openCCW = "brickGarageDoorHalfData";
};

datablock fxDTSBrickData(brickGarageDoorHalfData : brickGarageDoorUpData)
{
	brickFile = "./garagedoor_half.blb";
	
	isOpen = 1;
	//iconName = "Add-Ons/Brick_Security_Camera/Security Camera";
};


datablock fxDTSBrickData(brickGarageDoorData : brickGarageDoorUpData)
{
	brickFile = "./garagedoor.blb";
	
	isOpen = 0;

	category = "Special";
	subCategory = "Doors";
	iconName = "Add-Ons/Brick_Garage_Door/garagedoor";
};