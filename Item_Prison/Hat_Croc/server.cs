datablock ShapeBaseImageData(CrocHatImage)
{
	shapeFile = "./CrocHat.dts";
	emap = true;
	mountPoint = $HeadSlot;
	canMountToBronson = 1;
	offset = "0 0 -0.022";
	eyeOffset = "0 0 0.01";
	rotation = eulerToMatrix("0 0 0");
	scale = "1 1 1";
	doColorShift = true;
	colorshiftColor = "0.4 0.36 0.1 1";
};