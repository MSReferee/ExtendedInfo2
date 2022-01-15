using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using ColossalFramework.Globalization;
using IINS.UIExt;
using System;
using System.IO;

namespace IINS.ExtendedInfo
{
    public class PanelCapacities : ExtendedPanel
    {
        protected UIComponent DemandComponent = null;
        public PanelCapacities()
        {
            name = this.GetType().Name;
            DemandComponent = parentPanel.Find("Demand");
        }

        public override void Awake()
        {
            base.Awake();
            this.size = new Vector2(159, PANEL_HEIGHT);
            //this.eventDoubleClick += DoDoubleClick;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            DoneControls();
        }

        public override void Start()
        {
            base.Start();
            InitControls();
            ShowControls(0);
        }

        public override void ResetPositionSize()
        {
            if (DemandComponent != null)
            {
                absolutePosition = DemandComponent.absolutePosition;
                if (mainAspectRatio > 0f && mainAspectRatio < 1.9f)
                    relativePosition = new Vector3(706f, 4.0f);
                else
                    relativePosition = new Vector3(relativePosition.x + 68, 4.0f);
            }
        }

        UIButton btnPrev = null;
        //UIButton btnNext = null;

        void InitControls()
        {
            LocaleManager.eventLocaleChanged += OnLocaleChanged;
            // 切换按钮
            btnPrev = this.AddUIComponent<UIButton>();
            btnPrev.size = new Vector2(16, 16);
            btnPrev.relativePosition = new Vector3(4, 6);
            btnPrev.normalFgSprite = "ArrowLeft";
            btnPrev.hoveredFgSprite = "ArrowLeftFocused";
            btnPrev.pressedFgSprite = "ArrowLeftPressed";
            btnPrev.playAudioEvents = true;
            btnPrev.eventClick += OnSwitchClick;

            //btnNext = this.AddUIComponent<UIButton>();
            //btnNext.size = new Vector2(14, 12);
            //btnNext.relativePosition = new Vector3(10, 6);
            //btnNext.normalFgSprite = "UnlockingArrowRight";
            //btnNext.hoveredFgSprite = "UnlockingArrowRightFocused";
            //btnNext.pressedFgSprite = "UnlockingArrowRightPressed";
            //btnNext.playAudioEvents = true;
            //btnNext.eventClick += OnSwitchClick;

            CreateElectricityWaterControls();
            CreateEducationControls();
        }

        UIPanel educationPanel = null;
        UISprite lblEducation = null;
        UISlider meterElementary = null;
        UISlider meterHighSchool = null;
        UISlider meterUniversity = null;
        UISlider meterPublicLibrary = null;

        void CreateEducationControls()
        {
            educationPanel = this.AddUIComponent<UIPanel>();
            educationPanel.name = "educationPanel";
            educationPanel.size = new Vector2(135, 34);
            educationPanel.relativePosition = new Vector3(25, 1);
            educationPanel.isVisible = false;
            educationPanel.eventClick += OnSetInfoModeClick;
            educationPanel.eventMouseUp += OnRightSetInfoModeClick;
            educationPanel.playAudioEvents = true;

            lblEducation = educationPanel.AddUIComponent<UISprite>();
            lblEducation.size = new Vector2(18, 18);
            lblEducation.relativePosition = new Vector3(-1, 0);
            lblEducation.spriteName = "ToolbarIconEducationHovered";
            lblEducation.opacity = 0.3f;
            lblEducation.isTooltipLocalized = true;
            lblEducation.tooltipLocaleID = "INFO_EDUCATION_SCHOOLS";
            lblEducation.playAudioEvents = true;


            meterElementary = CreateAvailabilityMeter(educationPanel, 5, "Elementary");
            meterHighSchool = CreateAvailabilityMeter(educationPanel, 13, "HighSchool");
            meterUniversity = CreateAvailabilityMeter(educationPanel, 21, "University");
            meterPublicLibrary = CreateAvailabilityMeter(educationPanel, 28, "Public Library");
            meterElementary.tooltipLocaleID = "INFO_EDUCATION_ELEMENTARY";
            meterHighSchool.tooltipLocaleID = "INFO_EDUCATION_HIGH";
            meterUniversity.tooltipLocaleID = "INFO_EDUCATION_UNIVERSITY";
            meterPublicLibrary.tooltipLocaleID = "INFO_EDUCATION_LIBRARY";
        }

        UIPanel electWaterPanel = null;
        UISprite lblElectricity = null;
        UISprite lblWater = null;
        UISprite lblHeating = null;
        UISlider meterElectricity = null;
        UISlider meterWater = null;
        UISlider meterSewage = null;
        UISlider meterHeating = null;

        void CreateElectricityWaterControls()
        {
            electWaterPanel = this.AddUIComponent<UIPanel>();
            electWaterPanel.name = "electWaterPanel";
            electWaterPanel.size = new Vector2(135, 34);
            electWaterPanel.relativePosition = new Vector3(25, 1);
            electWaterPanel.isVisible = false;
            electWaterPanel.eventClick += OnSetInfoModeClick;
            electWaterPanel.eventMouseUp += OnRightSetInfoModeClick;
            electWaterPanel.playAudioEvents = true;

            lblElectricity = electWaterPanel.AddUIComponent<UISprite>();
            lblElectricity.size = new Vector2(13, 13);
            lblElectricity.relativePosition = new Vector3(0, 0);
            lblElectricity.spriteName = "ToolbarIconElectricityHovered";
            lblElectricity.opacity = 0.3f;
            lblElectricity.playAudioEvents = true;

            meterElectricity = CreateAvailabilityMeter(electWaterPanel, 5, "Electricity");
            meterElectricity.tooltipLocaleID = "INFO_ELECTRICITY_AVAILABILITY"; 

            lblWater = electWaterPanel.AddUIComponent<UISprite>();
            lblWater.size = new Vector2(13, 13);
            lblWater.relativePosition = new Vector3(0, 11);
            lblWater.spriteName = "InfoIconWaterFocused";
            lblWater.opacity = 0.3f;
            lblWater.playAudioEvents = true;

            meterWater = CreateAvailabilityMeter(electWaterPanel, 13, "Water");
            meterWater.tooltipLocaleID = "INFO_WATER_WATERAVAILABILITY";

            meterSewage = CreateAvailabilityMeter(electWaterPanel, 21, "Sewage");
            meterSewage.tooltipLocaleID = "INFO_WATER_SEWAGEAVAILABILITY";

            lblHeating = electWaterPanel.AddUIComponent<UISprite>();
            lblHeating.size = new Vector2(13, 13);
            lblHeating.relativePosition = new Vector3(0, 22);
            lblHeating.spriteName = "InfoIconHeating";
            lblHeating.opacity = 0.3f;
            lblHeating.playAudioEvents = true;

            meterHeating = CreateAvailabilityMeter(electWaterPanel, 28, "Heating");
            meterHeating.tooltipLocaleID = "INFO_HEATING_AVAILABILITY";
        }

        private UISlider CreateAvailabilityMeter(UIComponent parent, int top, string Name)
        {
            GameObject mobject = new GameObject(Name + "_Meter");
            mobject.transform.parent = parent.transform;
            UISlider meter = mobject.AddComponent<UISlider>();
            meter.size = new Vector2(110, 2.6f);
            meter.backgroundSprite = "MeterBackground";
            meter.relativePosition = new Vector3(18, top);
            meter.opacity = 0.35f;
            meter.isTooltipLocalized = true;

            GameObject ibject = new GameObject(Name + "_Indicator");
            ibject.transform.parent = meter.transform;
            UISprite indicator = ibject.AddComponent<UISprite>();
            indicator.spriteName = "LocationMarkerActiveNormal";
            indicator.size = new Vector2(8, 8);

            meter.thumbOffset = new Vector2(0, 3);
            meter.thumbObject = indicator;
            meter.Disable();
            meter.value = 0.0f;
            return meter;
        }

        private void OnSetInfoModeClick(UIComponent component, UIMouseEventParameter P)
        {
            
            if (component == educationPanel)
            {
                if (P.position.y <= 10)
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Education, InfoManager.SubInfoMode.LibraryEducation);
                else if (P.position.y <= 17)
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Education, InfoManager.SubInfoMode.University);
                else if (P.position.y <= 24)
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Education, InfoManager.SubInfoMode.HighSchool);
                else if (P.position.y <= 33)
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Education, InfoManager.SubInfoMode.Default);
            }
            else if (component == meterElementary)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Education, InfoManager.SubInfoMode.Default);
            }
            else if (component == meterHighSchool)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Education, InfoManager.SubInfoMode.HighSchool);
            }
            else if (component == meterUniversity)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Education, InfoManager.SubInfoMode.University);
            }
            else if (component == meterPublicLibrary)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Education, InfoManager.SubInfoMode.LibraryEducation);
            }
            else if (component == electWaterPanel)
            {
                if (P.position.y <= 10)
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Heating, InfoManager.SubInfoMode.Default);
                else if (P.position.y <= 22)
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Water, InfoManager.SubInfoMode.Default);
                else if (P.position.y <= 33)
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Electricity, InfoManager.SubInfoMode.Default);
            }
            else if (component == meterElectricity)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Electricity, InfoManager.SubInfoMode.Default);
            }
            else if (component == meterWater || component == meterSewage)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Water, InfoManager.SubInfoMode.Default);
            }
            else if (component == meterHeating)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Heating, InfoManager.SubInfoMode.Default);
            }
        }



        private void OnRightSetInfoModeClick(UIComponent component, UIMouseEventParameter P)
        {
            if (P.buttons == UIMouseButton.Right)
            {
                if (component == electWaterPanel || component == meterElectricity || component == meterWater || component == meterSewage || component == meterHeating)
                {
                    UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Garbage, InfoManager.SubInfoMode.Default);
                } else
                 if (component == educationPanel || component == lblEducation || component == meterElementary || component == meterHighSchool || component == meterUniversity || component == meterPublicLibrary)
                {
                    UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Education, InfoManager.SubInfoMode.Campus);
                }

            }
        }


        private int PanelIndex = 0;
        void ShowControls(int pIndex)
        {
            electWaterPanel.isVisible = false;
            educationPanel.isVisible = false;

            switch (pIndex)
            {
                case 0:
                    electWaterPanel.isVisible = true;                    
                    break;
                case 1:
                    educationPanel.isVisible = true;
                    break;
            }

            CityInfoDatas.instance.UpdateCapacities();
            this.UpdateData();
        }

        void DoneControls()
        {
            LocaleManager.eventLocaleChanged -= OnLocaleChanged;
            Destroy(btnPrev); btnPrev = null;

            Destroy(electWaterPanel); electWaterPanel = null;
            Destroy(educationPanel); electWaterPanel = null;
            Destroy(lblWater); lblWater = null;
            Destroy(lblElectricity); lblElectricity = null;
            Destroy(lblHeating); lblHeating = null;
            Destroy(lblEducation); lblEducation = null;

            Destroy(meterElectricity); meterElectricity = null;
            Destroy(meterWater); meterWater = null;
            Destroy(meterSewage); meterSewage = null;
            Destroy(meterHeating); meterHeating = null;
            Destroy(meterElementary); meterElementary = null;
            Destroy(meterHighSchool); meterHighSchool = null;
            Destroy(meterUniversity); meterUniversity = null;
            Destroy(meterPublicLibrary); meterPublicLibrary = null;
        }

        //#pragma warning disable 0108
        new void OnLocaleChanged()
        {
            
        }

        public override void OnScreenSizeChagned()
        {
            base.OnScreenSizeChagned();
        }

        public override void UpdateData()
        {
            if (electWaterPanel.isVisible)
            {
                meterElectricity.value = CityInfoDatas.Electricity.extvalue;
                meterWater.value = CityInfoDatas.Water.extvalue;
                meterSewage.value = CityInfoDatas.Sewage.extvalue;
                meterHeating.value = CityInfoDatas.Heating.extvalue;
            }
            if (educationPanel.isVisible)
            {
                meterElementary.value = CityInfoDatas.ElementarySchool.extvalue;
                meterHighSchool.value = CityInfoDatas.HighSchool.extvalue;
                meterUniversity.value = CityInfoDatas.University.extvalue;
                meterPublicLibrary.value = CityInfoDatas.PublicLibrary.extvalue;
            }


            lblEducation.tooltipLocaleID = "INFO_EDUCATION_SCHOOLS";
            meterElementary.tooltipLocaleID = "INFO_EDUCATION_ELEMENTARY";
            meterHighSchool.tooltipLocaleID = "INFO_EDUCATION_HIGH";
            meterUniversity.tooltipLocaleID = "INFO_EDUCATION_UNIVERSITY";
            meterPublicLibrary.tooltipLocaleID = "INFO_EDUCATION_LIBRARY";
            meterElectricity.tooltipLocaleID = "INFO_ELECTRICITY_AVAILABILITY"; 
            meterWater.tooltipLocaleID = "INFO_WATER_WATERAVAILABILITY";
            meterSewage.tooltipLocaleID = "INFO_WATER_SEWAGEAVAILABILITY";
            meterHeating.tooltipLocaleID = "INFO_HEATING_AVAILABILITY";
            
        }

        public override void OnDrawPanel()
        {
            base.OnDrawPanel();
        }



        private void OnSwitchClick(UIComponent comp, UIMouseEventParameter p)
        {
            PanelIndex += 1;
            if (PanelIndex > 1) PanelIndex = 0;
            ShowControls(PanelIndex);
        } 



        //private void DoDoubleClick(UIComponent component, UIMouseEventParameter eventParam)
        //{
        //}

        //public override void OnGUI()
        //{
        //    base.OnGUI();
        //    var SM = Singleton<SimulationManager>.instance;
        //    var d1 = SM.FrameToTime(SM.m_currentBuildIndex);
        //    var d2 = SM.FrameToTime(SM.m_dayTimeOffsetFrames + SM.m_currentBuildIndex);
        //    var d3 = SM.FrameToTime(SM.m_currentFrameIndex); 
        //    GUI.Label(new Rect(100, 100, 200, 20), d1.ToString());
        //    GUI.Label(new Rect(100, 120, 200, 20), d2.ToString());
        //    GUI.Label(new Rect(100, 140, 200, 20), d3.ToString());
        //}
    }
}
