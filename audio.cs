datablock AudioProfile(RiotOverSound)
{
   filename    = "./riotOver.wav";
   description = AudioClose3d;
   preload = true;
};

datablock AudioProfile(RiotStartSound)
{
   filename    = "./riotStart.wav";
   description = AudioClose3d;
   preload = true;
};

 
function setRiotMusic(%riot) {
   if (isObject(SM_Music)) {
      %profile = SM_Music.profile;
      %prevRiot = getSubStr(%profile, strLen(%profile) - 2, strLen(%profile));
      SM_Music.delete();
   }
   
   // if (%riot == 1) {
   //    serverPlay2D(RiotTransition1);
   // } else if (%riot == 2) { 
   //    if (%prevRiot == 1) {
   //       serverPlay2D(RiotTransition1Sound);
   //    } else if (%prevRiot == 3) {
   //       serverPlay2D(RiotTransition2Sound);
   //    }
   // } else if (%riot == 3) {
   //    serverPlay2D(RiotTransition2Sound);
   // }

   // schedule(1000, 0, createRiotMusic, %riot);
   createRiotMusic(%riot);
}

function createRiotMusic(%riot) {
   // %profile = "musicData_Riot" @ %riot;
   
   // new AudioEmitter(SM_Music) {
   //    position = "0 0 0";
   //    profile = %profile;
   //    useProfileDescription = 0;
   //    description = "AudioLooping2D";
   //    type = 0;
   //    volume = 10;
   //    outsideAmbient = 1;
   //    ReferenceDistance = 4;
   //    maxDistance = 9001;
   //    isLooping = 1;
   //    is3D = 0;
   // };
}