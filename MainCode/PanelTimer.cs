using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using ColossalFramework.Globalization;
using IINS.UIExt;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using IINS.ExtendedInfo.TranslationFramework;

using System.Reflection; // For More Simulation Speed


namespace IINS.ExtendedInfo
{
    public class PanelTimer : ExtendedPanel
    {
        private static readonly Translation translation = new Translation();

        private FieldInfo simulationSpeedField;
        //private UIButton MoreSpeedButton;
        //private UIMultiStateButton MoreSpeedBar;
        private Color32 white = new Color32(255, 255, 255, 255);
        private Color32 red = new Color32(255, 0, 0, 255);

        UISprite gameTimeSprite;
        public PanelTimer()
        {
            name = this.GetType().Name;
            relevantComponent = parentPanel.Find("PanelTime");
            gameTimeSprite = relevantComponent.Find<UISprite>("Sprite");
        }

        
        private UISprite RushHourSprite = null;
        private UILabel RushHourTimeLabel = null;
        private static UIPanel _savePanel;

        public override void Awake()
        {
            base.Awake();
            this.size = new Vector2(310, PANEL_HEIGHT);
            //this.eventDoubleClick += DoDoubleClick;
            var pnl = UIView.library.Get<PauseMenu>("PauseMenu");
            if (pnl != null)
                _savePanel = pnl.Find<UIPanel>("Menu");

        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MoreSpeedBar.isVisible = false;
            Destroy(MoreSpeedButton.gameObject);
            DoneControls();
        }



        public override void Start()
        {
            base.Start();
            InitControls();
            UpdateLocalTimeVisible();

            //btnDayNightSwitch.tooltip = "Left Click: " + Locale.Get("TIMECONTROL") + "\nRight Click: " + Locale.Get("OPTIONS_ENABLE_DAY_NIGHT");
            //btnWeatherSwitch.tooltip = "Left Click: " + Locale.Get("THEME_WORLD_WEATHER") + "\nRight Click: " + Locale.Get("OPTIONS_ENABLE_WEATHER");
            //btnDayNightSwitch.tooltip = translation.GetTranslation("IINSEI_CLICK_LEFT") + Locale.Get("TIMECONTROL") + "\n" + translation.GetTranslation("IINSEI_CLICK_RIGHT") + Locale.Get("OPTIONS_ENABLE_DAY_NIGHT");
            //btnWeatherSwitch.tooltip = translation.GetTranslation("IINSEI_CLICK_LEFT") + Locale.Get("THEME_WORLD_WEATHER") + "\n" + translation.GetTranslation("IINSEI_CLICK_RIGHT") + Locale.Get("OPTIONS_ENABLE_WEATHER");

            //btnRadioIconContainer.tooltip = translation.GetTranslation("IINSEI_RADIO_TOGGLE");
            //btnMuteAudioContainer.tooltip = translation.GetTranslation("IINSEI_AUDIO_MUTE");
            //if (MODUtil.IsChinaLanguage())
            //{
            //    btnRadioIconContainer.tooltip = "城市電臺 開/關";
            //    btnMuteAudioContainer.tooltip = "靜音";
            //}
            //else
            //{
            //    btnRadioIconContainer.tooltip = "Toggle Radio";
            //    btnMuteAudioContainer.tooltip = "Mute Audio";
            //}

            btnRadioIconContainer.opacity = IsRadioToggle() ? 0.7f : 0.1f;


            // 兼容 Rush Hours 
            if (CityInfoDatas.RushHourUI != null && ExtendedInfoManager.infopanel != null)
            {
                UIPanel _panelTime = ExtendedInfoManager.infopanel.Find<UIPanel>("PanelTime");
                if (_panelTime != null)
                {
                    RushHourSprite = _panelTime.Find<UISprite>("NewSprite");
                    RushHourTimeLabel = _panelTime.Find<UILabel>("NewTime");
                }
            }
        }

        UIMultiStateButton btnPlay = null;
        UIMultiStateButton btnSpeed = null;

        UIButton MoreSpeedButton = null;
        UIMultiStateButton MoreSpeedBar = null;

        UISprite barTime = null;
        MUILabel lblGameTime = null;
        UISprite lblDayTimeA = null;
        MUILabel lblDayTime = null;
        MUILabel lblLocalTime = null;
        UISprite lblLocalTimeA = null;
        UISprite lblThermometerA = null;
        MUILabel lblThermometer = null;
        UIButton btnDayNightSwitch = null;
        UIButton btnWeatherSwitch = null;
        UIButton btnChirperContainer = null;
        UIButton btnRadioIconContainer = null;
        UIButton btnMuteAudioContainer = null;

        int showTimeTag = 0;

        void InitControls()
        {
            LocaleManager.eventLocaleChanged += OnLocaleChanged;
            // 昼夜转换
            btnDayNightSwitch = this.AddUIComponent<UIButton>();
            btnDayNightSwitch.size = new Vector2(17, 17); // new Vector2(18, 9)
            btnDayNightSwitch.relativePosition = new Vector3(2, 2);
            btnDayNightSwitch.atlas = TextureUtils.LoadSpriteAtlas("EI_DayAndNight");
            btnDayNightSwitch.spritePadding = new RectOffset();
            btnDayNightSwitch.normalBgSprite = "EIIcon1";
            //btnDayNightSwitch.normalBgSprite = "IconSunMoon";
            btnDayNightSwitch.playAudioEvents = true;
            //btnDayNightSwitch.tooltipLocaleID = "TIMECONTROL";
            btnDayNightSwitch.eventClick += OnDayNightSwitchClicked;
            btnDayNightSwitch.eventMouseUp += OnDayNightSwitchMouseUp;
            btnDayNightSwitch.opacity = 0.7f;
            btnDayNightSwitch.isTooltipLocalized = true;
            btnDayNightSwitch.spritePadding = new RectOffset();
            // 天气转换
            btnWeatherSwitch = this.AddUIComponent<UIButton>();
            btnWeatherSwitch.size = new Vector2(22, 22); // new Vector2(14, 14)
            btnWeatherSwitch.relativePosition = new Vector3(21, 0);
            btnWeatherSwitch.atlas = TextureUtils.LoadSpriteAtlas("EI_Weather");
            btnWeatherSwitch.spritePadding = new RectOffset();
            btnWeatherSwitch.normalBgSprite = "EIIcon1";
            //btnWeatherSwitch.normalBgSprite = "IconPolicyOnlyElectricity";
            btnWeatherSwitch.playAudioEvents = true;
            //btnWeatherSwitch.tooltipLocaleID = "THEME_WORLD_WEATHER";
            btnWeatherSwitch.eventClick += OnWeatherSwitchClicked;
            btnWeatherSwitch.eventMouseUp += OnWeatherSwitchMouseUp;
            btnWeatherSwitch.opacity = 0.7f;
            btnWeatherSwitch.isTooltipLocalized = true;
            btnWeatherSwitch.spritePadding = new RectOffset();
            // 啾啾显示
            btnChirperContainer = this.AddUIComponent<UIButton>();
            btnChirperContainer.size = new Vector2(16, 16);
            btnChirperContainer.relativePosition = new Vector3(44, 2);
            btnChirperContainer.normalBgSprite = "ChirperIcon";
            btnChirperContainer.playAudioEvents = true;
            btnChirperContainer.isTooltipLocalized = true;
            btnChirperContainer.spritePadding = new RectOffset();
            btnChirperContainer.eventClick += OnChirperTogglerClicked;
            btnChirperContainer.tooltip = translation.GetTranslation("IINSEI_CHIRPER_TOOLTIP");
            //btnChirperContainer.tooltipLocaleID = "CHIRPER_NAME";
            if (ChirpPanel.instance != null)
            {
                btnChirperContainer.isVisible = true;
                btnChirperContainer.opacity = ChirpPanel.instance.gameObject.activeSelf ? 0.7f : 0.1f;
            }
            else
                btnChirperContainer.isVisible = false;

            // 电台显示
            btnRadioIconContainer = this.AddUIComponent<UIButton>();
            btnRadioIconContainer.size = new Vector2(17, 17); // new Vector2(16, 16)
            btnRadioIconContainer.relativePosition = new Vector3(64, 2);
            btnRadioIconContainer.atlas = TextureUtils.LoadSpriteAtlas("EI_Radio");
            btnRadioIconContainer.spritePadding = new RectOffset();
            btnRadioIconContainer.normalBgSprite = "EIIcon1";
            //btnRadioIconContainer.normalFgSprite = "ADIcon";
            btnRadioIconContainer.playAudioEvents = true;
            btnRadioIconContainer.isTooltipLocalized = true;
            btnRadioIconContainer.spritePadding = new RectOffset();
            btnRadioIconContainer.eventClick += OnRadioTogglerClicked;

            // 静音
            btnMuteAudioContainer = this.AddUIComponent<UIButton>();
            btnMuteAudioContainer.size = new Vector2(17, 17); // new Vector2(16, 16)
            btnMuteAudioContainer.relativePosition = new Vector3(84, 3);
            btnMuteAudioContainer.atlas = TextureUtils.LoadSpriteAtlas("EI_MuteAudio");
            btnMuteAudioContainer.spritePadding = new RectOffset();
            btnMuteAudioContainer.normalBgSprite = "EIIcon1";
            
            //btnMuteAudioContainer.normalFgSprite = "IconPolicyNoLoudNoisesHovered";
            btnMuteAudioContainer.playAudioEvents = true;
            btnMuteAudioContainer.isTooltipLocalized = true;
            btnMuteAudioContainer.spritePadding = new RectOffset();
            btnMuteAudioContainer.eventClick += (UIComponent comp, UIMouseEventParameter p) =>
            {
                    ExtendedInfoManager.AudioMuteAll = !ExtendedInfoManager.AudioMuteAll;
                    Singleton<AudioManager>.instance.MuteAll = ExtendedInfoManager.AudioMuteAll;
                    btnMuteAudioContainer.opacity = Singleton<AudioManager>.instance.MuteAll ? 0.1f : 0.7f;
            };
            btnMuteAudioContainer.opacity = Singleton<AudioManager>.instance.MuteAll ? 0.1f : 0.7f;

            // 暂停按钮
            btnPlay = this.AddUIComponent<UIMultiStateButton>();
            btnPlay.size = new Vector2(12, 15);
            btnPlay.relativePosition = new Vector3(165, 18); //btnPlay.relativePosition = new Vector3(165, LINE2 - 2);
            btnPlay.isTooltipLocalized = true;
            btnPlay.tooltipLocaleID = "MAIN_PLAYPAUSE";
            btnPlay.eventClick += OnPlayClicked;
            btnPlay.playAudioEvents = true;
            btnPlay.spritePadding = new RectOffset();

            UIMultiStateButton.SpriteSet btnPlaySpriteSet0 = btnPlay.foregroundSprites[0];
            btnPlaySpriteSet0.normal = "ButtonPause";
            btnPlaySpriteSet0.hovered = "ButtonPauseHovered";
            btnPlaySpriteSet0.pressed = "ButtonPausePressed";
            btnPlaySpriteSet0.focused = "ButtonPauseFocused";

            btnPlay.backgroundSprites.AddState();
            btnPlay.foregroundSprites.AddState();
            UIMultiStateButton.SpriteSet btnPlaySpriteSet1 = btnPlay.foregroundSprites[1];
            btnPlaySpriteSet1.normal = "ButtonPlay";
            btnPlaySpriteSet1.hovered = "ButtonPlayHovered";
            btnPlaySpriteSet1.pressed = "ButtonPlayPressed";
            btnPlaySpriteSet1.focused = "ButtonPlayFocused";



            // 速度按钮
            btnSpeed = this.AddUIComponent<UIMultiStateButton>();
            btnSpeed.size = new Vector2(40, 22);
            btnSpeed.relativePosition = new Vector3(263, LINE2 - 4);
            btnSpeed.isTooltipLocalized = true;
            btnSpeed.tooltip = translation.GetTranslation("IINSEI_SPEED_TOOLTIP");
            //btnSpeed.tooltipLocaleID = "MAIN_SPEED";
            btnSpeed.eventClick += MoreSpeedIconLeftClick;
            //btnSpeed.eventMouseUp += OnRightMoreSpeedIconClick;
            //btnSpeed.eventClick += OnSpeedClicked;
            btnSpeed.playAudioEvents = true;
            btnSpeed.atlas = TextureUtils.LoadSpriteAtlas("EI_SpeedIcon");
            btnSpeed.spritePadding = new RectOffset();
            UIMultiStateButton.SpriteSet btnSpeedSpriteSet0 = btnSpeed.foregroundSprites[0];
            btnSpeedSpriteSet0.normal = "EIIcon1";
            btnSpeedSpriteSet0.hovered = "EIIcon2";
            btnSpeedSpriteSet0.pressed = "EIIcon2";

            btnSpeed.backgroundSprites.AddState();
            btnSpeed.foregroundSprites.AddState();
            UIMultiStateButton.SpriteSet btnSpeedSpriteSet1 = btnSpeed.foregroundSprites[1];
            btnSpeedSpriteSet1.normal = "EIIcon2";
            btnSpeedSpriteSet1.hovered = "EIIcon3";
            btnSpeedSpriteSet1.pressed = "EIIcon3";

            btnSpeed.backgroundSprites.AddState();
            btnSpeed.foregroundSprites.AddState();
            UIMultiStateButton.SpriteSet btnSpeedSpriteSet2 = btnSpeed.foregroundSprites[2];
            btnSpeedSpriteSet2.normal = "EIIcon3";
            btnSpeedSpriteSet2.hovered = "EIIcon4";
            btnSpeedSpriteSet2.pressed = "EIIcon4";

            btnSpeed.backgroundSprites.AddState();
            btnSpeed.foregroundSprites.AddState();
            UIMultiStateButton.SpriteSet btnSpeedSpriteSet3 = btnSpeed.foregroundSprites[3];
            btnSpeedSpriteSet3.normal = "EIIcon4";
            btnSpeedSpriteSet3.hovered = "EIIcon5";
            btnSpeedSpriteSet3.pressed = "EIIcon5";

            btnSpeed.backgroundSprites.AddState();
            btnSpeed.foregroundSprites.AddState();
            UIMultiStateButton.SpriteSet btnSpeedSpriteSet4 = btnSpeed.foregroundSprites[4];
            btnSpeedSpriteSet4.normal = "EIIcon5";
            btnSpeedSpriteSet4.hovered = "EIIcon6";
            btnSpeedSpriteSet4.pressed = "EIIcon6";

            btnSpeed.backgroundSprites.AddState();
            btnSpeed.foregroundSprites.AddState();
            UIMultiStateButton.SpriteSet btnSpeedSpriteSet5 = btnSpeed.foregroundSprites[5];
            btnSpeedSpriteSet5.normal = "EIIcon6";
            btnSpeedSpriteSet5.hovered = "EIIcon7";
            btnSpeedSpriteSet5.pressed = "EIIcon7";

            btnSpeed.backgroundSprites.AddState();
            btnSpeed.foregroundSprites.AddState();
            UIMultiStateButton.SpriteSet btnSpeedSpriteSet6 = btnSpeed.foregroundSprites[6];
            btnSpeedSpriteSet6.normal = "EIIcon7";
            btnSpeedSpriteSet6.hovered = "EIIcon7";
            btnSpeedSpriteSet6.pressed = "EIIcon1";


            //btnSpeed.eventMouseDown += (component, param) =>
//            {
//                var speed = Util.GetFieldValue<int>(simulationSpeedField, SimulationManager.instance);

//                if (Input.GetKey(KeyCode.LeftControl))
//                {
//                    Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 99);
//                }

//            };

            //btnSpeed.spritePadding = new RectOffset();
//            UIMultiStateButton.SpriteSet btnSpeedSpriteSet0 = btnSpeed.foregroundSprites[0];
//            btnSpeedSpriteSet0.normal = "IconSpeed1Normal";
//            btnSpeedSpriteSet0.hovered = "IconSpeed1Hover";
//            btnSpeedSpriteSet0.pressed = "IconSpeed2Normal";

            //btnSpeed.backgroundSprites.AddState();
//            btnSpeed.foregroundSprites.AddState();
//            UIMultiStateButton.SpriteSet btnSpeedSpriteSet1 = btnSpeed.foregroundSprites[1];
//            btnSpeedSpriteSet1.normal = "IconSpeed2Normal";
//            btnSpeedSpriteSet1.hovered = "IconSpeed2Hover";
//            btnSpeedSpriteSet1.pressed = "IconSpeed3Normal";

            //btnSpeed.backgroundSprites.AddState();
//            btnSpeed.foregroundSprites.AddState();
//            UIMultiStateButton.SpriteSet btnSpeedSpriteSet2 = btnSpeed.foregroundSprites[2];
//            btnSpeedSpriteSet2.normal = "IconSpeed3Normal";
//            btnSpeedSpriteSet2.hovered = "IconSpeed3Hover";
//            btnSpeedSpriteSet2.pressed = "IconSpeed1Normal";


            simulationSpeedField = Util.FindField(SimulationManager.instance, "m_simulationSpeed");
                
            var multiStateButtons = GameObject.FindObjectsOfType<UIMultiStateButton>();
            foreach (var button in multiStateButtons)
            {
                if (button.name == "Speed")
                {
                    MoreSpeedBar = button;
                    break;
                }
            }

            MoreSpeedBar.isVisible = false;

            // Create a GameObject with a ColossalFramework.UI.UIButton component.
            var buttonObject = new GameObject("MoreSimulationSpeedOptionsButton", typeof(UIButton));

            // Make the buttonObject a child of the uiView.
            buttonObject.transform.parent = MoreSpeedBar.transform.parent.transform;

            // Get the button component.
            MoreSpeedButton = buttonObject.GetComponent<UIButton>();

            // Set the text to show on the button.
            MoreSpeedButton.text = "x1";

            // Set the button dimensions.
            //MoreSpeedButton.width = MoreSpeedBar.width;
            //MoreSpeedButton.height = MoreSpeedBar.height;

            // Style the button to look like a menu button.
            //MoreSpeedButton = this.AddUIComponent<UIButton>(); // Show added Button, hide because when clicking it nothing happend.
            MoreSpeedButton.size = new Vector2(28, 22);
            MoreSpeedButton.relativePosition = new Vector3(310, 30);
            MoreSpeedButton.isTooltipLocalized = true;
            MoreSpeedButton.tooltipLocaleID = "MAIN_SPEED";
            //MoreSpeedButton.spritePadding = new RectOffset();
            MoreSpeedButton.normalBgSprite = "ButtonMenu";
            MoreSpeedButton.disabledBgSprite = "ButtonMenuDisabled";
            MoreSpeedButton.hoveredBgSprite = "ButtonMenuHovered";
            MoreSpeedButton.focusedBgSprite = "ButtonMenu";
            MoreSpeedButton.pressedBgSprite = "ButtonMenuPressed";
            MoreSpeedButton.textColor = new Color32(255, 255, 255, 255);
            MoreSpeedButton.disabledTextColor = new Color32(7, 7, 7, 255);
            MoreSpeedButton.hoveredTextColor = new Color32(255, 255, 255, 255);
            MoreSpeedButton.focusedTextColor = new Color32(255, 255, 255, 255);
            MoreSpeedButton.pressedTextColor = new Color32(30, 30, 44, 255);
            //MoreSpeedButton.BringToFront();

            // Place the button.
            //MoreSpeedButton.transformPosition = MoreSpeedBar.transformPosition;

            // Respond to button click. 
            //MoreSpeedButton.eventClick += MoreSpeedIconLeftClick;
            MoreSpeedButton.eventClicked += (component, value) => { MoreSpeedIconLeftClick(component, value); };
            MoreSpeedButton.playAudioEvents = true;



            // 时间条
            if (gameTimeSprite != null)
            {
                barTime = this.AddUIComponent<UISprite>();
                barTime.name = "NewSprite";
                barTime.size = new Vector2(140, 12);
                barTime.relativePosition = new Vector3(166, 4);
                barTime.atlas = gameTimeSprite.atlas;
                barTime.spriteName = gameTimeSprite.spriteName;
                barTime.fillAmount = 0.5f;
                barTime.fillDirection = UIFillDirection.Horizontal;
                barTime.color = gameTimeSprite.color;
                barTime.fillAmount = gameTimeSprite.fillAmount;
            }
            // 游戏日期标签
            lblGameTime = this.AddUIComponent<MUILabel>();
            lblGameTime.size = new Vector2(60, LINEW);
            lblGameTime.relativePosition = new Vector3(192, LINE2 - 3);
            lblGameTime.textColor = ExtendedPanel.COLOR_DARK_TEXT;
            lblGameTime.textAlignment = UIHorizontalAlignment.Left;
            lblGameTime.fontStyle = FontStyle.Bold;
            lblGameTime.fontSize = (int)MUISkin.UIToScreen(10f);

            // 游戏时间标签
            lblDayTimeA = this.AddUIComponent<UISprite>();
            lblDayTimeA.size = new Vector2(16, 16); // new Vector2(14, 14)
            lblDayTimeA.relativePosition = new Vector3(105, 2);
            //lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
            //lblDayTimeA.spriteName = "EIIcon1";
            //lblDayTimeA.name = "Icon";
            lblDayTimeA.tooltip = translation.GetTranslation("IINSEI_DAYTIME_TOOLTIP");
            //lblDayTimeA.spriteName = "InfoIconEntertainmentFocused";
            lblDayTimeA.opacity = 0.7f;
            lblDayTimeA.playAudioEvents = (CityInfoDatas.TimeWarpMod_sunManager == null);
            lblDayTimeA.eventClick += OnDayTimeClicked;

            lblDayTime = this.AddUIComponent<MUILabel>();
            lblDayTime.size = new Vector2(40, LINEW);
            lblDayTime.relativePosition = new Vector3(115, LINE1 - 3);
            lblDayTime.textColor = ExtendedPanel.COLOR_TEXT;
            lblDayTime.textAlignment = UIHorizontalAlignment.Right;
            lblDayTime.fontStyle = FontStyle.Bold;
            lblDayTime.fontSize = (int)MUISkin.UIToScreen(10f);
            lblDayTime.playAudioEvents = (CityInfoDatas.TimeWarpMod_sunManager == null);
            lblDayTime.eventClick += OnDayTimeClicked;
            lblDayTime.tooltip = translation.GetTranslation("IINSEI_DAYTIME_TOOLTIP");

            // 本地时间
            lblLocalTimeA = this.AddUIComponent<UISprite>();
            lblLocalTimeA.size = new Vector2(16, 16); // new Vector2(7, 7)
            lblLocalTimeA.relativePosition = new Vector3(2, LINE2 + 3);
            lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
            lblLocalTimeA.spriteName = "EIIcon1";
            lblLocalTimeA.name = "Icon";
            lblLocalTimeA.tooltip = translation.GetTranslation("IINSEI_REALWORLDTIME_TOOLTIP");
            //lblLocalTimeA.spriteName = "OptionBaseFocused";
            lblLocalTimeA.opacity = 0.7f;

            lblLocalTime = this.AddUIComponent<MUILabel>();
            lblLocalTime.size = new Vector2(45, LINEW);
            lblLocalTime.relativePosition = new Vector3(23, LINE2 - 2);
            lblLocalTime.textColor = ExtendedPanel.COLOR_DARK_YELLOW;
            lblLocalTime.textAlignment = UIHorizontalAlignment.Left;
            lblLocalTime.fontStyle = FontStyle.Bold;
            lblLocalTime.fontSize = (int)MUISkin.UIToScreen(10f);
            lblLocalTime.eventClick += OnLocalTimeClick;
            lblLocalTime.playAudioEvents = true;
            lblLocalTime.tooltip = translation.GetTranslation("IINSEI_REALWORLDTIME_TOOLTIP");

            // 温度
            lblThermometerA = this.AddUIComponent<UISprite>();
            lblThermometerA.size = new Vector2(14, 14); // new Vector2(16, 16)
            lblThermometerA.relativePosition = new Vector3(106, LINE2);
            //lblThermometerA.atlas = TextureUtils.LoadSpriteAtlas("EI_Temperature");
            //lblThermometerA.spriteName = "EISpeed1";
            //lblThermometerA.name = "Icon";
            lblThermometerA.tooltip = translation.GetTranslation("IINSEI_TEMPERATURE_TOOLTIP");
            //lblThermometerA.spriteName = "ThermometerIcon";
            lblThermometerA.opacity = 0.7f;


            lblThermometer = this.AddUIComponent<MUILabel>();
            lblThermometer.size = new Vector2(40, LINEW);
            lblThermometer.relativePosition = new Vector3(115, LINE2 - 2);
            //lblThermometer.textColor = ExtendedPanel.COLOR_DARK_TEXT;
            lblThermometer.textAlignment = UIHorizontalAlignment.Right;
            //lblThermometer.fontStyle = FontStyle.Bold;
            lblThermometer.fontSize = (int)MUISkin.UIToScreen(10f);
            lblThermometer.tooltip = translation.GetTranslation("IINSEI_TEMPERATURE_TOOLTIP");
            //lblThermometer.tooltipLocaleID = "MAIN_TEMPERATURE";

            setGameState();
        }

        void DoneControls()
        {
            LocaleManager.eventLocaleChanged -= OnLocaleChanged;
            Destroy(btnPlay); btnPlay = null;
            Destroy(btnSpeed); btnSpeed = null;
            Destroy(MoreSpeedButton); MoreSpeedButton = null;
            Destroy(MoreSpeedBar); MoreSpeedBar = null;
            Destroy(lblGameTime); lblGameTime = null;
            Destroy(lblDayTime); lblDayTime = null;
            Destroy(barTime); barTime = null;
            Destroy(lblLocalTime); lblLocalTime = null;
            Destroy(lblLocalTimeA); lblLocalTimeA = null;
            Destroy(lblThermometer); lblThermometer = null;
            Destroy(lblThermometerA); lblThermometerA = null;
            Destroy(btnDayNightSwitch); btnDayNightSwitch = null;
            Destroy(btnWeatherSwitch); btnWeatherSwitch = null;
            Destroy(btnChirperContainer); btnChirperContainer = null;
            Destroy(btnRadioIconContainer); btnRadioIconContainer = null;
        }

        //#pragma warning disable 0108
        new void OnLocaleChanged()
        {
            
        }

        public override void OnScreenSizeChagned()
        {
            base.OnScreenSizeChagned();
            if (btnPlay != null)
            {
                btnPlay.size = new Vector2(12, 15);
                btnPlay.relativePosition = new Vector3(165, 18);
                btnSpeed.size = new Vector2(40, 22);
                btnSpeed.relativePosition = new Vector3(263, LINE2 - 4);
            }

            //if (MoreSpeedButton != null)
            //{
            //    MoreSpeedButton.size = new Vector2(28, 22);
            //    MoreSpeedButton.relativePosition = new Vector3(310, 30);
            //}

            if (lblGameTime != null)
            {
                lblGameTime.size = new Vector2(60, LINEW);
                lblGameTime.relativePosition = new Vector3(192, LINE2 - 3);
                lblGameTime.fontSize = (int)MUISkin.UIToScreen(10f);

                lblDayTime.size = new Vector2(40, LINEW);
                lblDayTime.relativePosition = new Vector3(115, LINE1 - 3);
                lblLocalTimeA.size = new Vector2(16, 16);
                lblLocalTimeA.relativePosition = new Vector3(2, LINE2 + 3);
                lblLocalTime.size = new Vector2(45, LINEW);
                lblLocalTime.relativePosition = new Vector3(23, LINE2 - 2);
            }

            if (lblThermometer != null)
            {
                lblThermometer.size = new Vector2(40, LINEW);
                lblThermometer.relativePosition = new Vector3(115, LINE2 - 2);
                lblThermometerA.size = new Vector2(14, 14);
                lblThermometerA.relativePosition = new Vector3(106, LINE2 + 2);
            }
        }

        void UpdateLocalTimeVisible()
        {
            switch (showTimeTag)
            {
                case 0:
                    lblLocalTime.textColor = ExtendedPanel.COLOR_DARK_YELLOW;
                    break;
                case 1:
                    lblLocalTime.textColor = ExtendedPanel.COLOR_DARK_PURPLE;
                    break;
            }

            UpdateLocalTimeText();
        }

        void UpdateLocalTimeText()
        {
            switch (showTimeTag)
            {
                case 0:
                    lblLocalTime.text = DateTime.Now.ToString("HH:mm:ss", LocaleManager.cultureInfo);
                    break;
                case 1:
                    lblLocalTime.text = CityInfoDatas.PlayingTime.text;
                    break;
            }

        }

        //public override void OnGUI()
        //{
        //    base.OnGUI();
        //    GUI.Label(new Rect(100, 100, 1000, 20), "aspect = " + Camera.main.aspect + "/" + mainAspectRatio);
        //    //GUI.Label(new Rect(100, 130, 1000, 20), "sprites = " + (sprites));
        //}

        private float h;
        private void updateRushHourEventSprites(UISprite[] sprite)
        {
            sprites = 0;
            float per = barTime.width / RushHourSprite.width;
            for (int i = 0; i < sprite.Length; i++)
            {
                if (sprite[i].name.Equals("UISprite"))
                {
                    sprites += 1;
                    sprite[i].transform.parent = barTime.transform;
                    //sprite[i].width = sprite[i].width * per;
                    sprite[i].height = barTime.height - 4;

                    float startPercent = (float)(sprite[i].relativePosition.x / RushHourSprite.width);
                    if (sprites == 1)
                    {
                        h = sprite[i].relativePosition.x; // (int)Mathf.Round((float)(startPercent * 24D));
                    }
                    float endPosition = (float)(sprite[i].width + sprite[i].relativePosition.x);
                    float endPercent = (float)(endPosition / RushHourSprite.width);
                    float startPosition = (float)(barTime.width * startPercent);
                    endPosition = (float)(barTime.width * endPercent);
                    int endWidth = (int)Mathf.Round(endPosition - startPosition);

                    //float xpos = (float)((sprite[i].relativePosition.x / RushHourSprite.width) * barTime.width);
                    sprite[i].absolutePosition = barTime.absolutePosition;
                    sprite[i].relativePosition = new Vector3(85 + startPosition, 2);
                    sprite[i].width = endWidth;
                }
            }
        }




        private int sprites;
        public override void UpdateData()
        {
            setGameState();

            if (CityInfoDatas.RushHourUI != null)
            {
                if (RushHourTimeLabel != null/* && RushHourTimeLabel.isVisible*/)
                {
                    lblGameTime.text = RushHourTimeLabel.text;
                    lblGameTime.tooltip = RushHourSprite.tooltip;
                }
                else
                    lblGameTime.text = CityInfoDatas.GameTime.text;
            }
            else
                lblGameTime.text = CityInfoDatas.GameTime.text;

            lblDayTime.text = CityInfoDatas.GameTimeOfDay.text;






            if (btnDayNightSwitch != null)
            {
                btnDayNightSwitch.tooltip = translation.GetTranslation("IINSEI_CLICK_LEFT") + Locale.Get("TIMECONTROL") + "\n" + translation.GetTranslation("IINSEI_CLICK_RIGHT") + Locale.Get("OPTIONS_ENABLE_DAY_NIGHT");
                btnWeatherSwitch.tooltip = translation.GetTranslation("IINSEI_CLICK_LEFT") + Locale.Get("THEME_WORLD_WEATHER") + "\n" + translation.GetTranslation("IINSEI_CLICK_RIGHT") + Locale.Get("OPTIONS_ENABLE_WEATHER");
                //btnDayNightSwitch.tooltip = "Left Click: " + Locale.Get("TIMECONTROL") + "\nRight Click: " + Locale.Get("OPTIONS_ENABLE_DAY_NIGHT");
                //btnWeatherSwitch.tooltip = "Left Click: " + Locale.Get("THEME_WORLD_WEATHER") + "\nRight Click: " + Locale.Get("OPTIONS_ENABLE_WEATHER");
            }

            if (btnRadioIconContainer != null)
            {
                btnRadioIconContainer.tooltip = translation.GetTranslation("IINSEI_RADIO_TOGGLE");
                btnMuteAudioContainer.tooltip = translation.GetTranslation("IINSEI_AUDIO_MUTE");
                //if (MODUtil.IsChinaLanguage())
                //{
                //    btnRadioIconContainer.tooltip = "城市電臺 開/關";
                //    btnMuteAudioContainer.tooltip = "靜音";
                //}
                //else
                //{
                //    btnRadioIconContainer.tooltip = "Toggle Radio";
                //    btnMuteAudioContainer.tooltip = "Mute Audio";
                //}
            }

            btnChirperContainer.tooltip = translation.GetTranslation("IINSEI_CHIRPER_TOOLTIP");
            btnSpeed.tooltip = translation.GetTranslation("IINSEI_SPEED_TOOLTIP");
            lblDayTimeA.tooltip = translation.GetTranslation("IINSEI_DAYTIME_TOOLTIP");
            lblDayTime.tooltip = translation.GetTranslation("IINSEI_DAYTIME_TOOLTIP");
            lblLocalTimeA.tooltip = translation.GetTranslation("IINSEI_REALWORLDTIME_TOOLTIP");
            lblLocalTime.tooltip = translation.GetTranslation("IINSEI_REALWORLDTIME_TOOLTIP");
            lblThermometerA.tooltip = translation.GetTranslation("IINSEI_TEMPERATURE_TOOLTIP");
            lblThermometer.tooltip = translation.GetTranslation("IINSEI_TEMPERATURE_TOOLTIP");





            // A Game Day Icon Changed
            float AGameDayIconChanged = Singleton<SimulationManager>.instance.m_currentDayTimeHour;

            if (AGameDayIconChanged >= 0f && AGameDayIconChanged < 1f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon1";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 1f && AGameDayIconChanged < 2f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon2";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 2f && AGameDayIconChanged < 3f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon3";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 3f && AGameDayIconChanged < 4f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon4";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 4f && AGameDayIconChanged < 5f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon5";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 5f && AGameDayIconChanged < 6f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon6";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 6f && AGameDayIconChanged < 7f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon7";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 7f && AGameDayIconChanged < 8f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon8";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 8f && AGameDayIconChanged < 9f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon9";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 9f && AGameDayIconChanged < 10f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon10";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 10f && AGameDayIconChanged < 11f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon11";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 11f && AGameDayIconChanged < 12f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon12";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 12f && AGameDayIconChanged < 13f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon1";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 13f && AGameDayIconChanged < 14f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon2";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 14f && AGameDayIconChanged < 15f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon3";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 15f && AGameDayIconChanged < 16f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon4";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 16f && AGameDayIconChanged < 17f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon5";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 17f && AGameDayIconChanged < 18f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon6";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 18f && AGameDayIconChanged < 19f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon7";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 19f && AGameDayIconChanged < 20f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon8";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 20f && AGameDayIconChanged < 21f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon9";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 21f && AGameDayIconChanged < 22f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon10";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 22f && AGameDayIconChanged < 23f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon11";
                lblDayTimeA.name = "Icon";
                
            }
            else if (AGameDayIconChanged >= 23f && AGameDayIconChanged < 24f)
            {
                lblDayTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_GameDayTime");
                lblDayTimeA.spriteName = "EIIcon12";
                lblDayTimeA.name = "Icon";
                
            }







//            // Earth 24 hour Time Icon Changed (Now not work)
//            //float WorldTimeOfDay = Singleton<SimulationManager>.instance.m_currentDayTimeHour;
//
//            if (WorldTimeOfDay >= 0f && WorldTimeOfDay < 1f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon1";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 1f && WorldTimeOfDay < 2f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon2";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 2f && WorldTimeOfDay < 3f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon3";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 3f && WorldTimeOfDay < 4f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon4";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 4f && WorldTimeOfDay < 5f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon5";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 5f && WorldTimeOfDay < 6f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon6";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 6f && WorldTimeOfDay < 7f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon7";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 7f && WorldTimeOfDay < 8f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon8";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 8f && WorldTimeOfDay < 9f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon9";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 9f && WorldTimeOfDay < 10f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon10";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 10f && WorldTimeOfDay < 11f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon11";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 11f && WorldTimeOfDay < 12f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon12";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 12f && WorldTimeOfDay < 13f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon1";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 13f && WorldTimeOfDay < 14f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon2";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 14f && WorldTimeOfDay < 15f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon3";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 15f && WorldTimeOfDay < 16f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon4";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 16f && WorldTimeOfDay < 17f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon5";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 17f && WorldTimeOfDay < 18f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon6";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 18f && WorldTimeOfDay < 19f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon7";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 19f && WorldTimeOfDay < 20f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon8";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 20f && WorldTimeOfDay < 21f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon9";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 21f && WorldTimeOfDay < 22f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon10";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 22f && WorldTimeOfDay < 23f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon11";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            else if (WorldTimeOfDay >= 23f && WorldTimeOfDay < 24f)
//            {
//                lblLocalTimeA.atlas = TextureUtils.LoadSpriteAtlas("EI_TimeBar");
//                lblLocalTimeA.spriteName = "EIIcon12";
//                lblLocalTimeA.name = "Icon";
//                
//            }
//            
            




            
            //Temperature Color and Icon Changed
            float ChangeColorTemp = Singleton<WeatherManager>.instance.m_currentTemperature;

            if (ChangeColorTemp >= 40f)
            {
                lblThermometerA.atlas = TextureUtils.LoadSpriteAtlas("EI_Temperature");
                lblThermometerA.spriteName = "EIIcon1";
                lblThermometerA.name = "Icon";
                lblThermometer.text = CityInfoDatas.Temperatur.text;
                //labelCrimeRate.textColor = Color.red; // 8 Bit Color
                lblThermometer.textColor = new Color32(255, 20, 20, 255); // Red, Super Hot
                
            }
            else if (ChangeColorTemp >= 34f && ChangeColorTemp < 40f)
            {
                lblThermometerA.atlas = TextureUtils.LoadSpriteAtlas("EI_Temperature");
                lblThermometerA.spriteName = "EIIcon2";
                lblThermometerA.name = "Icon";
                lblThermometer.text = CityInfoDatas.Temperatur.text;
                //labelCrimeRate.textColor = Color.red; // 8 Bit Color
                lblThermometer.textColor = new Color32(240, 100, 20, 255); // Orange, Very Hot
                
            }
            else if (ChangeColorTemp >= 29f && ChangeColorTemp < 34f)
            {
                lblThermometerA.atlas = TextureUtils.LoadSpriteAtlas("EI_Temperature");
                lblThermometerA.spriteName = "EIIcon3";
                lblThermometerA.name = "Icon";
                lblThermometer.text = CityInfoDatas.Temperatur.text;
                //labelCrimeRate.textColor = Color.red; // 8 Bit Color
                lblThermometer.textColor = new Color32(255, 255, 0, 255); // Yellow, Hot
                
            }
            else if (ChangeColorTemp >= 20f && ChangeColorTemp < 29f)
            {
                lblThermometerA.atlas = TextureUtils.LoadSpriteAtlas("EI_Temperature");
                lblThermometerA.spriteName = "EIIcon4";
                lblThermometerA.name = "Icon";
                lblThermometer.text = CityInfoDatas.Temperatur.text;
                //labelCrimeRate.textColor = Color.red; // 8 Bit Color
                lblThermometer.textColor = new Color32(20, 255, 20, 255); // Green, Comfortable
                
            }
            else if (ChangeColorTemp >= 15f && ChangeColorTemp < 20f)
            {
                lblThermometerA.atlas = TextureUtils.LoadSpriteAtlas("EI_Temperature");
                lblThermometerA.spriteName = "EIIcon5";
                lblThermometerA.name = "Icon";
                lblThermometer.text = CityInfoDatas.Temperatur.text;
                //labelCrimeRate.textColor = Color.red; // 8 Bit Color
                lblThermometer.textColor = new Color32(145, 255, 160, 255); // Light Green, Little cold
                
            }
            else if (ChangeColorTemp >= 0f && ChangeColorTemp < 15f)
            {
                lblThermometerA.atlas = TextureUtils.LoadSpriteAtlas("EI_Temperature");
                lblThermometerA.spriteName = "EIIcon6";
                lblThermometerA.name = "Icon";
                lblThermometer.text = CityInfoDatas.Temperatur.text;
                //labelCrimeRate.textColor = Color.red; // 8 Bit Color
                lblThermometer.textColor = new Color32(0, 140, 255, 255); // Light Blue, Cold
                
            }
            else if (ChangeColorTemp < 0f)
            {
                    lblThermometerA.atlas = TextureUtils.LoadSpriteAtlas("EI_Temperature");
                    lblThermometerA.spriteName = "EIIcon7";
                    lblThermometerA.name = "Icon";
                    lblThermometer.text = CityInfoDatas.Temperatur.text;
                    lblThermometer.textColor = new Color32(0, 200, 255, 255); //Blue, Super Cold
            }
            





            //UpdateLocalTimeVisible();

            btnDayNightSwitch.opacity = Singleton<CityInfoDatas>.instance.enableDayNight ? 0.7f : 0.05f;
            btnDayNightSwitch.playAudioEvents = Singleton<CityInfoDatas>.instance.enableDayNight;
            btnWeatherSwitch.opacity = Singleton<CityInfoDatas>.instance.enableWeather ? 0.7f : 0.1f;
            btnWeatherSwitch.playAudioEvents = Singleton<CityInfoDatas>.instance.enableWeather;

            var timeControler = ExtendedInfoManager.timeControler;
            if (timeControler != null)
            {
                lblDayTime.textColor = (timeControler.speed.num == 0) ? MUIUtil.DarkenColor(ExtendedPanel.COLOR_TEXT, 0.7f, 1.0f) : ExtendedPanel.COLOR_TEXT;
                lblDayTimeA.opacity = (timeControler.speed.num == 0) ? 0.2f : 1f;
            }

            if (ChirpPanel.instance != null)
            {
                btnChirperContainer.opacity = ChirpPanel.instance.gameObject.activeSelf ? 0.7f : 0.1f;
            }
        }





        private float showtime = 0;
        private bool GamePauseState = false;
        private int GameSpeed = 0;

        public override void Update()
        {
            base.Update();
            if (gameTimeSprite != null && CityInfoDatas.RushHourUI != null)
            {
                if (RushHourSprite != null /*&& RushHourSprint.isVisible*/)
                {
                    barTime.color = RushHourSprite.color;
                    barTime.fillAmount = RushHourSprite.fillAmount;
                    var sprite = RushHourSprite.GetComponentsInChildren<UISprite>();
                    if (sprite.Length > 1)
                        updateRushHourEventSprites(sprite);
                }
                else
                {
                    barTime.color = gameTimeSprite.color;
                    barTime.fillAmount = gameTimeSprite.fillAmount;
                }
            }





            var speed = Util.GetFieldValue<int>(simulationSpeedField, SimulationManager.instance);
            MoreSpeedButton.text = "x" + speed.ToString();
            MoreSpeedButton.transformPosition = MoreSpeedBar.transformPosition;
            MoreSpeedButton.transform.position = MoreSpeedBar.transform.position;



            
            //if (speed == 3)
            //{
            //    Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 4);
            //}
          


            if (speed > 3)
            {
                MoreSpeedButton.textColor = red;
                MoreSpeedButton.focusedTextColor = red;
                MoreSpeedButton.hoveredTextColor = red;
            }
            else
            {
                MoreSpeedButton.textColor = white;
                MoreSpeedButton.focusedTextColor = white;
                MoreSpeedButton.hoveredTextColor = white;
            }




            // Hotkey for change x6, x9 and x99
            if (Input.GetKey(KeyCode.LeftControl))
            {
                 
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    int speed1 = btnSpeed.activeStateIndex;
                    speed1 = 5;
                    Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 6);
                    SimulationSpeed(speed1);
                }

                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    int speed1 = btnSpeed.activeStateIndex;
                    speed1 = 6;
                    Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 9);
                    SimulationSpeed(speed1);
                   
                }

                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    int speed1 = btnSpeed.activeStateIndex;
                    speed1 = 7;
                    Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 99);
                    SimulationSpeed(speed1);
                }


            }

            if (Input.GetKey(KeyCode.RightControl))
            {
                 
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    int speed1 = btnSpeed.activeStateIndex;
                    speed1 = 5;
                    Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 6);
                    SimulationSpeed(speed1);
                }

                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    int speed1 = btnSpeed.activeStateIndex;
                    speed1 = 6;
                    Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 9);
                    SimulationSpeed(speed1);
                   
                }

                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    int speed1 = btnSpeed.activeStateIndex;
                    speed1 = 7;
                    Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 99);
                    SimulationSpeed(speed1);
                }


            }





            
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            if (GamePauseState != CityInfoDatas.isGamePaused())
            {
                GamePauseState = CityInfoDatas.isGamePaused();
                setGameState();
            }

            if (Singleton<SimulationManager>.exists)
            {
                if (GameSpeed != Singleton<SimulationManager>.instance.SelectedSimulationSpeed)
                {
                    GameSpeed = Singleton<SimulationManager>.instance.SelectedSimulationSpeed;
                    setGameState();
                }
            }

            showtime += Time.deltaTime;

            if (showtime >= 1f) // 每秒显示一次
            {
                showtime = 0f;
                UpdateLocalTimeText();
            }

            if (Singleton<AudioManager>.exists && _savePanel != null)
            {
                if (!_savePanel.isVisible)
                {
                    Singleton<AudioManager>.instance.MuteAll = ExtendedInfoManager.AudioMuteAll;
                    if (btnMuteAudioContainer != null)
                        btnMuteAudioContainer.opacity = Singleton<AudioManager>.instance.MuteAll ? 0.1f : 0.7f;
                }
            }

            if (gameTimeSprite != null)
            {
                if (CityInfoDatas.RushHourUI == null)
                {

                    barTime.color = gameTimeSprite.color;
                    barTime.fillAmount = gameTimeSprite.fillAmount;
                }
            }
        }

        public override void OnDrawPanel()
        {
            base.OnDrawPanel();
            if (CanOnGUIDraw())
            {
                float W = MUISkin.UIToScreen(160);
                float T = 4f;
                float H = MUISkin.UIToScreen(PANEL_HEIGHT) - T * 2;

                GUI.DrawTexture(new Rect(W, T, 1, H), lineTexture);
            }
        }

        


        public void OnChirperTogglerClicked(UIComponent comp, UIMouseEventParameter p)
        {
            if (ChirpPanel.instance != null)
            {
                ChirpPanel.instance.gameObject.SetActive(!ChirpPanel.instance.gameObject.activeSelf);
                ExtendedInfoManager.ChirperVisible = ChirpPanel.instance.gameObject.activeSelf;
            }

            if (ChirpPanel.instance != null)
                btnChirperContainer.opacity = ChirpPanel.instance.gameObject.activeSelf ? 0.7f : 0.1f;
        }

        private static RadioChannelInfo savedRadioInfo;
        public static void ToggleRadio(bool enabled)
        {
            var radioPanel = GameObject.Find("RadioPanel");
            if (radioPanel != null)
            {
                RadioPanel rp = radioPanel.GetComponent(typeof(RadioPanel)) as RadioPanel;
                if (rp != null)
                {
                    var btn = rp.Find<UIButton>("RadioButton");
                    var pnl = rp.Find<UIPanel>("RadioPlayingPanel");
                    if (btn != null && pnl != null)
                    {
                        if (pnl.isVisible)
                        {
                            pnl.isVisible = enabled;
                        }
                        else
                        {
                            btn.isVisible = enabled;
                        }

                        bool isDisabled = !btn.isVisible && !pnl.isVisible;

                        AudioManager AM = Singleton<AudioManager>.instance;
                        if (isDisabled)
                        {
                            savedRadioInfo = AM.GetActiveRadioChannelInfo();
                            AM.SetActiveRadioChannel(0);
                        }
                        else
                        {
                            AM.SetActiveRadioChannelInfo(savedRadioInfo);
                            AM.PlayAudio(AM.CurrentListenerInfo);
                        }
                        AM.MuteRadio = isDisabled;
                    }
                }
            }

            ExtendedInfoManager.RadionVisible = PanelTimer.IsRadioToggle();
        }

        public static bool IsRadioToggle()
        {
            var radioPanel = GameObject.Find("RadioPanel");
            if (radioPanel != null)
            {
                RadioPanel rp = radioPanel.GetComponent(typeof(RadioPanel)) as RadioPanel;
                if (rp != null)
                {
                    var btn = rp.Find<UIButton>("RadioButton");
                    var pnl = rp.Find<UIPanel>("RadioPlayingPanel");
                    if (btn != null && pnl != null)
                    {
                        bool isDisabled = !btn.isVisible && !pnl.isVisible;
                        Singleton<AudioManager>.instance.MuteRadio = isDisabled;
                        return !isDisabled;
                    }
                }
            }

            return false;
        }

        public void OnRadioTogglerClicked(UIComponent comp, UIMouseEventParameter p)
        {
            ToggleRadio(!IsRadioToggle());
            btnRadioIconContainer.opacity = IsRadioToggle() ? 0.7f : 0.1f;
        }



        public void OnDayTimeClicked(UIComponent comp, UIMouseEventParameter p)
        {
            var timeControler = ExtendedInfoManager.timeControler;

            //if (CityInfoDatas.TimeWarpMod_sunManager != null)
            //{
                // 想控制 TimeWarpMod 的值，但是没有找到入口。
            //}
            if (timeControler != null) // "else if (timeControler != null)" this for using "if (CityInfoDatas.TimeWarpMod_sunManager != null)"
            {
                if (timeControler.speed.num == 0)
                    timeControler.speedIndex = 1;
                else
                    timeControler.speedIndex = 0;

                timeControler.speed = TimeControler.TimeSpeeds[timeControler.speedIndex];
                UpdateData();
            }
        }

        public void OnPlayClicked(UIComponent comp, UIMouseEventParameter p)
        {
            SimulationPause();
        }

        [Serializable]
        public class Settings
        {
            public int speed;
            public uint dayOffsetFrames;
            public float longitude;
            public float lattitude;
            public float sunSize;
            public float sunIntensity;
        }

        public void OnDayNightSwitchClicked(UIComponent comp, UIMouseEventParameter p)
        {
            var time = Singleton<CityInfoDatas>.instance.WorldTimeOfDay;
            if (time >= 8f && time <= 19f)  // 8:00 AM -- 19:00 PM is Day
                Singleton<CityInfoDatas>.instance.WorldTimeOfDay = 0.0f;
            else
                Singleton<CityInfoDatas>.instance.WorldTimeOfDay = 12.001f;
        }

        public void OnDayNightSwitchMouseUp(UIComponent comp, UIMouseEventParameter p)
        {
            if (p.buttons == UIMouseButton.Right)
            {
                UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                Singleton<CityInfoDatas>.instance.enableDayNight = !Singleton<CityInfoDatas>.instance.enableDayNight;
                UpdateData();
            }
        }

        public void OnWeatherSwitchMouseUp(UIComponent comp, UIMouseEventParameter p)
        {
            if (p.buttons == UIMouseButton.Right)
            {
                UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                Singleton<CityInfoDatas>.instance.enableWeather = !Singleton<CityInfoDatas>.instance.enableWeather;
                UpdateData();
            }
        }

        public void OnWeatherSwitchClicked(UIComponent comp, UIMouseEventParameter p)
        {
            var WM = Singleton<WeatherManager>.instance;

            if (WM.m_currentRain <= 0.05f) // 晴
                Singleton<CityInfoDatas>.instance.WeatherRainIntensity = CityInfoDatas.RainSprinkle;
            else if (WM.m_currentRain < 0.2f) // 小雨
                Singleton<CityInfoDatas>.instance.WeatherRainIntensity = CityInfoDatas.RainMiddle;
            else if (WM.m_currentRain < 1.0f) //中雨
                Singleton<CityInfoDatas>.instance.WeatherRainIntensity = CityInfoDatas.RainHeavy;
            else if (WM.m_currentRain >= 1.5f) // 大雨
                Singleton<CityInfoDatas>.instance.WeatherRainIntensity = 0f;
        }




        public void MoreSpeedIconLeftClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            
            var speed = Util.GetFieldValue<int>(simulationSpeedField, SimulationManager.instance);
            UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                
                    switch (speed)
                    {
                        case 1:
                            Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 2);
                            break;
                        case 2:
                            Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 4);
                            break;
                        case 4:
                            Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 6);
                            break;
                        case 6:
                            Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 9);
                            break;
                        case 9:
                        case 99:
                            Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 1);
                            break;
                    }

                
                //if (Input.GetKeyDown(KeyCode.LeftControl))
                //{
                //    Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 1);
                //}


                // Click speed arrow start to simulate the game
                int speed1 = btnSpeed.activeStateIndex;
                speed1 += 1;
                if (speed1 == 3) speed1 = 3;
                if (speed1 == 4) speed1 = 4;
                if (speed1 == 5) speed1 = 5;
                if (speed1 == 6) speed1 = 6;
                if (speed1 == 7) speed1 = 7;
                if (speed1 > 7) speed1 = 1;
                SimulationSpeed(speed1);

        }


         // Right click for slow speed, but now not work.
//         public void OnRightMoreSpeedIconClick(UIComponent component, UIMouseEventParameter P)
//         {
//            if (P.buttons == UIMouseButton.Right)
//            {
//                var speed = Util.GetFieldValue<int>(simulationSpeedField, SimulationManager.instance);
//                UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
//                
//                    switch (speed)
//                    {
//                        case 1:
//                            Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 0.5);
//                            break;
//                        case 2:
//                            Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 0.2);
//                            break;
//                        case 4:
//                            Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 0.09);
//                            break;
//                        case 6:
//                            Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 0.06);
//                            break;
//                        case 9:
//                        case 99:
//                            Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 0.8);
//                            break;
//                    }
//                
//                if (Input.GetKey(KeyCode.RightControl))
//                {
//                    Util.SetFieldValue(simulationSpeedField, SimulationManager.instance, 0.01);
//                }
//            }
//         }

        //public void OnSpeedClicked(UIComponent comp, UIMouseEventParameter p)
        //{
        //    int speed = btnSpeed.activeStateIndex;
        //    speed += 1;
        //    if (speed > 3) speed = 1;
        //    SimulationSpeed(speed);
        //}

        public void OnLocalTimeClick(UIComponent comp, UIMouseEventParameter p)
        {
            showTimeTag += 1;
            if (showTimeTag > 1)
                showTimeTag = 0;

            UpdateLocalTimeVisible();
        }


        public void setGameState()
        {
            bool pause = CityInfoDatas.isGamePaused();
            if (btnPlay != null)
            {
                btnPlay.activeStateIndex = (pause ? 0 : 1);
                int speed1 = Singleton<SimulationManager>.instance.SelectedSimulationSpeed;
                btnSpeed.activeStateIndex = speed1 - 1;
            }
        }

        protected void SimulationPause()
        {

            if (Singleton<SimulationManager>.exists)
            {
                Singleton<SimulationManager>.instance.ForcedSimulationPaused = !Singleton<SimulationManager>.instance.ForcedSimulationPaused;
            }
        }

        protected void SimulationSpeed(int speed1)
        {
            if (Singleton<SimulationManager>.exists)
            {
                Singleton<SimulationManager>.instance.SelectedSimulationSpeed = speed1;
            }
        }


        //private void DoDoubleClick(UIComponent component, UIMouseEventParameter eventParam)
        //{
        //}

    }
}
