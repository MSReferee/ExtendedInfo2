using ICities;
using IINS.UIExt;
using ColossalFramework.PlatformServices;
using IINS.ExtendedInfo.TranslationFramework;

namespace IINS.ExtendedInfo
{

    public class ExtendedInfoMod : MODLoadingExtension, IUserMod
    {
        private static readonly Translation translation = new Translation();
        //public static string Version = "(2.0.0)";

        //public ExtendedInfoMod()
        //{
            //Debugger.Prefix = "[" + Name + " " + Version + "] "; 
        //}

        public string Name
        {
            get
            {
                return translation.GetTranslation("IINSEI_MOD_NAME") + translation.GetTranslation("IINSEI_MOD_VERSION");
            }
        }
        public string Description
        {
            get
            {
                return translation.GetTranslation("IINSEI_MOD_DESCRIPTION");
            }
        }
        //public string Name
        //{
        //    get
        //    {
		//return "擴展訊息面板（Extended InfoPanel）" + " " + Version;
        //    }
        //}
        //public string Description
        //{
        //    get
        //    {
		//return "在遊戲底部面板中增加更多功能和顯示更多數據與資訊。（Show more information data in game bottom panel.）";
        //    }
        //}

        public override void OnReleased() 
        {
            base.OnReleased();
            ExtendedInfoManager.stop();    
        }


        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
                if (PlatformService.apiBackend == APIBackend.Steam)
                    ExtendedInfoManager.run();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();                   
        }

    }
}
