datablock fxDTSBrickData(brickTrayData)
{
	brickFile = "./tray.blb";
	category = "Special";
	subCategory = "Misc";
	uiName = "Trays";
	iconName = "Add-Ons/Brick_Trays/tray";
};

datablock fxDTSBrickData(brickPhoneData)
{
	brickFile = "./phone.blb";
	category = "Special";
	subCategory = "Misc";
	uiName = "Phone";
	iconName = "";
};

datablock fxDTSBrickData(brickGeneratorData)
{
	brickFile = "./Generator.blb";
	category = "Special";
	subCategory = "Misc";
	uiName = "Generator";
	iconName = "";
	//orientationFix = 1;
	//collisionShapeName = "./generator.dts";
};

datablock fxDTSBrickData(brickSatDishData)
{
	brickFile = "./satdish.blb";
	category = "Special";
	subCategory = "Misc";
	uiName = "Satellite Dish";
	iconName = "";
	collisionShapeName = "./dishCOL.dts";
};